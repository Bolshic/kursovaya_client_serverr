-- =============================================
-- Очистка таблиц (в правильном порядке)
-- =============================================
TRUNCATE TABLE Payment CASCADE;
TRUNCATE TABLE Invoice CASCADE;
TRUNCATE TABLE Residence CASCADE;
TRUNCATE TABLE Student CASCADE;
TRUNCATE TABLE "Group" CASCADE;
TRUNCATE TABLE Room CASCADE;
TRUNCATE TABLE Building CASCADE;
TRUNCATE TABLE Faculty CASCADE;

-- Сброс последовательностей
ALTER SEQUENCE invoice_number_seq RESTART WITH 1;
ALTER SEQUENCE payment_number_seq RESTART WITH 1;
ALTER SEQUENCE residence_order_number_seq RESTART WITH 1;

-- =============================================
-- 1. Факультеты
-- =============================================
INSERT INTO Faculty (faculty_name) VALUES
('Факультет информатики'),
('Факультет экономики'),
('Факультет юриспруденции'),
('Факультет иностранных языков'),
('Факультет физики');

-- =============================================
-- 2. Корпуса
-- =============================================
INSERT INTO Building (building_name, address) VALUES
('Корпус А', 'ул. Университетская, д. 1'),
('Корпус Б', 'ул. Университетская, д. 2'),
('Корпус В', 'ул. Студенческая, д. 5'),
('Корпус Г', 'ул. Студенческая, д. 7');

-- =============================================
-- 3. Группы
-- =============================================
INSERT INTO "Group" (group_name, faculty_name, course) VALUES
('ИВТ-101', 'Факультет информатики', 1),
('ИВТ-102', 'Факультет информатики', 1),
('ИВТ-201', 'Факультет информатики', 2),
('ЭК-101', 'Факультет экономики', 1),
('ЭК-202', 'Факультет экономики', 2),
('ЮР-301', 'Факультет юриспруденции', 3),
('ИЯ-401', 'Факультет иностранных языков', 4),
('ФИЗ-501', 'Факультет физики', 5);

-- =============================================
-- 4. Студенты
-- =============================================
INSERT INTO Student (passport_num, fio, group_name) VALUES
('1234567890', 'Иванов Иван Иванович', 'ИВТ-101'),
('1234567891', 'Петров Петр Петрович', 'ИВТ-101'),
('1234567892', 'Сидоров Сидор Сидорович', 'ИВТ-102'),
('1234567893', 'Кузнецова Анна Сергеевна', 'ЭК-101'),
('1234567894', 'Смирнова Елена Владимировна', 'ЭК-202'),
('1234567895', 'Васильев Василий Васильевич', 'ЮР-301'),
('1234567896', 'Николаев Николай Николаевич', 'ИЯ-401'),
('1234567897', 'Алексеев Алексей Алексеевич', 'ФИЗ-501'),
('1234567898', 'Дмитриев Дмитрий Дмитриевич', 'ИВТ-201'),
('1234567899', 'Сергеев Сергей Сергеевич', 'ИВТ-102');

-- =============================================
-- 5. Комнаты
-- =============================================
INSERT INTO Room (room_number, places_count, lockers_count, chairs_count, extra_info, building_name) VALUES
(101, 3, 3, 3, 'Комната с видом на парк', 'Корпус А'),
(102, 2, 2, 2, 'Недавно отремонтирована', 'Корпус А'),
(103, 4, 4, 4, 'На 4 этаже', 'Корпус А'),
(201, 3, 3, 3, 'Угловая комната', 'Корпус Б'),
(202, 2, 2, 2, NULL, 'Корпус Б'),
(301, 1, 1, 1, 'Одноместная VIP', 'Корпус В'),
(401, 6, 6, 6, 'Самая большая комната', 'Корпус Г');

-- =============================================
-- 6. Проживание (триггер автоматически проставит date_out)
-- =============================================
INSERT INTO Residence (passport_num, room_id, date_in) VALUES
('1234567890', 1, '2025-09-01'),
('1234567891', 1, '2025-09-01'),
('1234567892', 2, '2025-09-01'),
('1234567893', 3, '2025-09-01'),
('1234567894', 4, '2025-09-01'),
('1234567895', 5, '2025-09-01'),
('1234567896', 6, '2025-09-01'),
('1234567897', 7, '2025-09-01'),
('1234567898', 2, '2025-09-15');

-- =============================================
-- 7. Квитанции за сентябрь 2025
-- =============================================
CALL generate_monthly_invoices(2025, 9);

-- =============================================
-- 8. Квитанции на стирку
-- =============================================
DO $$
DECLARE
    rec RECORD;
BEGIN
    FOR rec IN SELECT order_number FROM Residence WHERE passport_num IN ('1234567890', '1234567891') LOOP
        CALL create_laundry_invoice(rec.order_number, 2, '2025-09-10');
    END LOOP;
    FOR rec IN SELECT order_number FROM Residence WHERE passport_num IN ('1234567892') LOOP
        CALL create_laundry_invoice(rec.order_number, 1, '2025-09-12');
    END LOOP;
END $$;

-- =============================================
-- 9. Платежи
-- =============================================
-- Оплатим первые квитанции (номера квитанций узнаем через подзапрос)
DO $$
DECLARE
    v_invoice_id INTEGER;
BEGIN
    -- Оплата проживания для Иванова
    SELECT invoice_number INTO v_invoice_id FROM Invoice 
    WHERE order_number = (SELECT order_number FROM Residence WHERE passport_num = '1234567890') 
    AND service_type = 'Проживание' LIMIT 1;
    IF v_invoice_id IS NOT NULL THEN
        CALL make_payment(v_invoice_id, 5000.00, '2025-09-05');
    END IF;

    -- Оплата коммунальных для Иванова
    SELECT invoice_number INTO v_invoice_id FROM Invoice 
    WHERE order_number = (SELECT order_number FROM Residence WHERE passport_num = '1234567890') 
    AND service_type = 'Коммунальные' LIMIT 1;
    IF v_invoice_id IS NOT NULL THEN
        CALL make_payment(v_invoice_id, 2000.00, '2025-09-05');
    END IF;

    -- Оплата проживания для Петрова
    SELECT invoice_number INTO v_invoice_id FROM Invoice 
    WHERE order_number = (SELECT order_number FROM Residence WHERE passport_num = '1234567891') 
    AND service_type = 'Проживание' LIMIT 1;
    IF v_invoice_id IS NOT NULL THEN
        CALL make_payment(v_invoice_id, 5000.00, '2025-09-06');
    END IF;
END $$;

-- =============================================
-- 10. Проверка
-- =============================================
SELECT 'Факультеты' AS таблица, COUNT(*) AS записей FROM Faculty
UNION ALL SELECT 'Корпуса', COUNT(*) FROM Building
UNION ALL SELECT 'Группы', COUNT(*) FROM "Group"
UNION ALL SELECT 'Студенты', COUNT(*) FROM Student
UNION ALL SELECT 'Комнаты', COUNT(*) FROM Room
UNION ALL SELECT 'Проживание', COUNT(*) FROM Residence
UNION ALL SELECT 'Квитанции', COUNT(*) FROM Invoice
UNION ALL SELECT 'Платежи', COUNT(*) FROM Payment;
