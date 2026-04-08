-- =============================================
-- Таблицы
-- =============================================

CREATE TABLE IF NOT EXISTS Faculty (
    faculty_name VARCHAR(50) PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS Building (
    building_name VARCHAR(20) PRIMARY KEY,
    address VARCHAR(200) NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS "Group" (
    group_name VARCHAR(20) PRIMARY KEY,
    faculty_name VARCHAR(50) NOT NULL,
    course INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (faculty_name) REFERENCES Faculty(faculty_name),
    CHECK (course BETWEEN 1 AND 6)
);

CREATE TABLE IF NOT EXISTS Student (
    passport_num VARCHAR(10) PRIMARY KEY,
    fio VARCHAR(50) NOT NULL,
    group_name VARCHAR(20) NOT NULL,
    FOREIGN KEY (group_name) REFERENCES "Group"(group_name),
    CHECK (passport_num ~ '^[0-9]+$')
);

CREATE TABLE IF NOT EXISTS Room (
    room_id SERIAL PRIMARY KEY,
    room_number INTEGER NOT NULL,
    places_count INTEGER NOT NULL,
    lockers_count INTEGER NOT NULL,
    chairs_count INTEGER NOT NULL,
    extra_info TEXT,
    building_name VARCHAR(20) NOT NULL,
    FOREIGN KEY (building_name) REFERENCES Building(building_name),
    CHECK (room_number >= 1),
    CHECK (places_count BETWEEN 1 AND 6),
    CHECK (lockers_count >= 0),
    CHECK (chairs_count >= 0)
);

CREATE TABLE IF NOT EXISTS Residence (
    order_number SERIAL PRIMARY KEY,
    passport_num VARCHAR(10) NOT NULL,
    room_id INTEGER NOT NULL,
    date_in DATE NOT NULL,
    date_out DATE,
    FOREIGN KEY (passport_num) REFERENCES Student(passport_num),
    FOREIGN KEY (room_id) REFERENCES Room(room_id),
    CHECK (date_in >= '2000-01-01'),
    CHECK (date_out IS NULL OR date_out >= date_in)
);

CREATE SEQUENCE IF NOT EXISTS invoice_number_seq START 1;
CREATE SEQUENCE IF NOT EXISTS payment_number_seq START 1;

CREATE TABLE IF NOT EXISTS Invoice (
    invoice_number INTEGER PRIMARY KEY DEFAULT nextval('invoice_number_seq'),
    order_number INTEGER NOT NULL,
    service_type VARCHAR(30) NOT NULL,
    period_start DATE NOT NULL,
    period_end DATE NOT NULL,
    amount NUMERIC(10,2) NOT NULL,
    paid_amount NUMERIC(10,2) DEFAULT 0,
    status VARCHAR(20) DEFAULT 'Не оплачена',
    invoice_date DATE NOT NULL DEFAULT CURRENT_DATE,
    FOREIGN KEY (order_number) REFERENCES Residence(order_number),
    CHECK (period_end >= period_start),
    CHECK (amount > 0),
    CHECK (paid_amount BETWEEN 0 AND amount),
    CHECK (service_type IN ('Проживание', 'Коммунальные', 'Стирка', 'Уборка')),
    CHECK (status IN ('Не оплачена', 'Оплачена'))
);

CREATE TABLE IF NOT EXISTS Payment (
    payment_number INTEGER PRIMARY KEY DEFAULT nextval('payment_number_seq'),
    invoice_number INTEGER NOT NULL,
    pay_date DATE NOT NULL,
    amount NUMERIC(10,2) NOT NULL,
    FOREIGN KEY (invoice_number) REFERENCES Invoice(invoice_number),
    CHECK (amount > 0)
);

-- =============================================
-- Функции и триггеры
-- =============================================

CREATE OR REPLACE FUNCTION calculate_eviction_date(p_date_in DATE, p_course INTEGER)
RETURNS DATE AS $$
DECLARE
    v_year INTEGER;
    v_month INTEGER;
BEGIN
    v_year := EXTRACT(YEAR FROM p_date_in)::INTEGER;
    v_month := EXTRACT(MONTH FROM p_date_in)::INTEGER;
    
    IF v_month > 6 THEN
        v_year := v_year + 1;
    END IF;
    
    v_year := v_year + (5 - p_course);
    
    RETURN make_date(v_year, 6, 30);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION set_eviction_date()
RETURNS TRIGGER AS $$
DECLARE
    v_course INTEGER;
BEGIN
    IF NEW.date_out IS NULL THEN
        SELECT g.course INTO v_course
        FROM Student s
        JOIN "Group" g ON s.group_name = g.group_name
        WHERE s.passport_num = NEW.passport_num;
        NEW.date_out := calculate_eviction_date(NEW.date_in, v_course);
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS residence_set_eviction_date ON Residence;
CREATE TRIGGER residence_set_eviction_date
    BEFORE INSERT ON Residence
    FOR EACH ROW
    EXECUTE FUNCTION set_eviction_date();

CREATE OR REPLACE FUNCTION check_room_capacity()
RETURNS TRIGGER AS $$
DECLARE
    max_places INTEGER;
    current_occupants INTEGER;
BEGIN
    SELECT places_count INTO max_places FROM Room WHERE room_id = NEW.room_id;
    SELECT COUNT(*) INTO current_occupants
    FROM Residence
    WHERE room_id = NEW.room_id AND (date_out IS NULL OR date_out >= CURRENT_DATE);
    IF current_occupants >= max_places THEN
        RAISE EXCEPTION 'В комнате нет свободных мест (занято %, всего %)', current_occupants, max_places;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS room_capacity_check ON Residence;
CREATE TRIGGER room_capacity_check
    BEFORE INSERT ON Residence
    FOR EACH ROW
    EXECUTE FUNCTION check_room_capacity();

CREATE OR REPLACE FUNCTION check_double_booking()
RETURNS TRIGGER AS $$
DECLARE
    existing INTEGER;
BEGIN
    SELECT COUNT(*) INTO existing
    FROM Residence
    WHERE passport_num = NEW.passport_num
      AND (date_out IS NULL OR date_out >= CURRENT_DATE)
      AND order_number != COALESCE(NEW.order_number, -1);
    IF existing > 0 THEN
        RAISE EXCEPTION 'Студент уже проживает в другой комнате';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS double_booking_check ON Residence;
CREATE TRIGGER double_booking_check
    BEFORE INSERT ON Residence
    FOR EACH ROW
    EXECUTE FUNCTION check_double_booking();

CREATE OR REPLACE FUNCTION update_invoice_after_payment()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Invoice SET
        paid_amount = paid_amount + NEW.amount,
        status = CASE WHEN paid_amount + NEW.amount >= amount THEN 'Оплачена' ELSE 'Не оплачена' END
    WHERE invoice_number = NEW.invoice_number;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS after_payment_insert ON Payment;
CREATE TRIGGER after_payment_insert
    AFTER INSERT ON Payment
    FOR EACH ROW
    EXECUTE FUNCTION update_invoice_after_payment();

-- =============================================
-- Хранимые процедуры
-- =============================================

CREATE OR REPLACE PROCEDURE settle_student(
    p_passport_num VARCHAR(10),
    p_room_id INTEGER,
    p_date_in DATE DEFAULT CURRENT_DATE
)
LANGUAGE plpgsql AS $$
DECLARE
    next_order INTEGER;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Student WHERE passport_num = p_passport_num) THEN
        RAISE EXCEPTION 'Студент не найден';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM Room WHERE room_id = p_room_id) THEN
        RAISE EXCEPTION 'Комната не найдена';
    END IF;
    SELECT COALESCE(MAX(order_number),0)+1 INTO next_order FROM Residence;
    INSERT INTO Residence (order_number, passport_num, room_id, date_in, date_out)
    VALUES (next_order, p_passport_num, p_room_id, p_date_in, NULL);
    RAISE NOTICE 'Заселение выполнено, номер приказа: %', next_order;
END;
$$;

CREATE OR REPLACE PROCEDURE evict_student(
    p_order_number INTEGER,
    p_date_out DATE DEFAULT CURRENT_DATE
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE Residence SET date_out = p_date_out
    WHERE order_number = p_order_number AND date_out IS NULL;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Приказ не найден или студент уже выселен';
    END IF;
    RAISE NOTICE 'Выселение выполнено';
END;
$$;

CREATE OR REPLACE PROCEDURE relocate_student(
    p_order_number INTEGER,
    p_new_room_id INTEGER,
    p_relocation_date DATE DEFAULT CURRENT_DATE
)
LANGUAGE plpgsql AS $$
DECLARE
    v_passport VARCHAR(10);
BEGIN
    SELECT passport_num INTO v_passport
    FROM Residence WHERE order_number = p_order_number AND date_out IS NULL;
    IF v_passport IS NULL THEN
        RAISE EXCEPTION 'Активное проживание с таким приказом не найдено';
    END IF;
    UPDATE Residence SET date_out = p_relocation_date - INTERVAL '1 day'
    WHERE order_number = p_order_number;
    CALL settle_student(v_passport, p_new_room_id, p_relocation_date);
    RAISE NOTICE 'Переселение выполнено';
END;
$$;

CREATE OR REPLACE PROCEDURE update_room_info(
    p_room_id INTEGER,
    p_places_count INTEGER DEFAULT NULL,
    p_lockers_count INTEGER DEFAULT NULL,
    p_chairs_count INTEGER DEFAULT NULL,
    p_extra_info TEXT DEFAULT NULL
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE Room SET
        places_count = COALESCE(p_places_count, places_count),
        lockers_count = COALESCE(p_lockers_count, lockers_count),
        chairs_count = COALESCE(p_chairs_count, chairs_count),
        extra_info = COALESCE(p_extra_info, extra_info)
    WHERE room_id = p_room_id;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Комната не найдена';
    END IF;
    RAISE NOTICE 'Информация о комнате обновлена';
END;
$$;

CREATE OR REPLACE PROCEDURE generate_monthly_invoices(
    p_year INTEGER,
    p_month INTEGER
)
LANGUAGE plpgsql AS $$
DECLARE
    v_start DATE := make_date(p_year, p_month, 1);
    v_end DATE := (v_start + INTERVAL '1 month - 1 day')::DATE;
BEGIN
    INSERT INTO Invoice (order_number, service_type, period_start, period_end, amount)
    SELECT order_number, 'Проживание', v_start, v_end, 5000.00
    FROM Residence
    WHERE date_in <= v_end AND (date_out IS NULL OR date_out >= v_start);

    INSERT INTO Invoice (order_number, service_type, period_start, period_end, amount)
    SELECT order_number, 'Коммунальные', v_start, v_end, 2000.00
    FROM Residence
    WHERE date_in <= v_end AND (date_out IS NULL OR date_out >= v_start);

    RAISE NOTICE 'Квитанции за %-% созданы', p_month, p_year;
END;
$$;

CREATE OR REPLACE PROCEDURE create_laundry_invoice(
    p_order_number INTEGER,
    p_quantity INTEGER,
    p_service_date DATE
)
LANGUAGE plpgsql AS $$
BEGIN
    INSERT INTO Invoice (order_number, service_type, period_start, period_end, amount)
    VALUES (p_order_number, 'Стирка', p_service_date, p_service_date, 100.00 * p_quantity);
END;
$$;

CREATE OR REPLACE PROCEDURE make_payment(
    p_invoice_number INTEGER,
    p_amount NUMERIC(10,2),
    p_pay_date DATE DEFAULT CURRENT_DATE
)
LANGUAGE plpgsql AS $$
DECLARE
    v_remaining NUMERIC(10,2);
BEGIN
    SELECT amount - paid_amount INTO v_remaining
    FROM Invoice WHERE invoice_number = p_invoice_number;
    IF v_remaining IS NULL THEN
        RAISE EXCEPTION 'Квитанция не найдена';
    END IF;
    IF p_amount > v_remaining THEN
        RAISE EXCEPTION 'Сумма платежа превышает остаток';
    END IF;
    INSERT INTO Payment (invoice_number, pay_date, amount)
    VALUES (p_invoice_number, p_pay_date, p_amount);
END;
$$;

-- =============================================
-- Представления
-- =============================================

CREATE OR REPLACE VIEW v_student_residence_history AS
SELECT
    s.passport_num,
    s.fio,
    g.course,
    s.group_name,
    g.faculty_name,
    r.order_number,
    b.building_name,
    b.address AS building_address,
    rm.room_number,
    r.date_in,
    r.date_out,
    CASE WHEN r.date_out IS NULL THEN 'Проживает' ELSE 'Выселен' END AS status
FROM Student s
JOIN "Group" g ON s.group_name = g.group_name
JOIN Residence r ON s.passport_num = r.passport_num
JOIN Room rm ON r.room_id = rm.room_id
JOIN Building b ON rm.building_name = b.building_name;

CREATE OR REPLACE VIEW v_current_residence AS
SELECT * FROM v_student_residence_history WHERE date_out IS NULL;

CREATE OR REPLACE VIEW v_faculty_students_with_rooms AS
SELECT
    faculty_name,
    fio,
    passport_num,
    course,
    group_name,
    building_name,
    building_address,
    room_number,
    date_in
FROM v_current_residence;

CREATE OR REPLACE VIEW v_free_rooms AS
SELECT
    b.building_name,
    r.room_number,
    r.places_count,
    r.places_count - COUNT(res.passport_num) AS free_places
FROM Room r
JOIN Building b ON r.building_name = b.building_name
LEFT JOIN Residence res ON r.room_id = res.room_id AND res.date_out IS NULL
GROUP BY b.building_name, r.room_number, r.places_count, r.room_id
HAVING r.places_count - COUNT(res.passport_num) > 0;

CREATE OR REPLACE VIEW v_invoice_details AS
SELECT
    i.invoice_number,
    i.service_type,
    i.period_start,
    i.period_end,
    i.amount,
    i.paid_amount,
    i.amount - i.paid_amount AS remaining,
    i.status,
    s.fio AS student_name,
    s.passport_num,
    g.course,
    g.group_name,
    b.building_name,
    rm.room_number
FROM Invoice i
JOIN Residence r ON i.order_number = r.order_number
JOIN Student s ON r.passport_num = s.passport_num
JOIN "Group" g ON s.group_name = g.group_name
JOIN Room rm ON r.room_id = rm.room_id
JOIN Building b ON rm.building_name = b.building_name;

CREATE OR REPLACE VIEW v_debts AS
SELECT
    student_name,
    passport_num,
    course,
    group_name,
    building_name,
    room_number,
    SUM(remaining) AS total_debt,
    COUNT(*) AS unpaid_invoices
FROM v_invoice_details
WHERE status = 'Не оплачена'
GROUP BY student_name, passport_num, course, group_name, building_name, room_number
ORDER BY total_debt DESC;
