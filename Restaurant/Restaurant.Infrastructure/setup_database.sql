-- SQL скрипт для налаштування бази даних Restaurant в PostgreSQL
-- Виконайте цей скрипт як суперкористувач PostgreSQL

-- Видаляємо існуючу базу, якщо є (для чистого старту)
DROP DATABASE IF EXISTS "RestaurantDB";

-- Видаляємо існуючого користувача, якщо є
DROP USER IF EXISTS restaurant;

-- Створюємо нового користувача
CREATE USER restaurant WITH PASSWORD 'restaurant123';

-- Створюємо базу даних з власником restaurant
CREATE DATABASE "RestaurantDB" 
    WITH OWNER = restaurant
    ENCODING = 'UTF8'
    LC_COLLATE = 'uk_UA.UTF-8'
    LC_CTYPE = 'uk_UA.UTF-8'
    TEMPLATE = template0;

-- Підключаємося до бази даних
\c "RestaurantDB"

-- Надаємо всі права на схему public
GRANT ALL ON SCHEMA public TO restaurant;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO restaurant;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO restaurant;

-- Встановлюємо права за замовчуванням для майбутніх об'єктів
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO restaurant;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO restaurant;

-- Перевірка
SELECT current_database(), current_user;

