-- Database initialization script for StackShare
-- This script is executed when the PostgreSQL container starts for the first time

-- Create database if it doesn't exist (this is handled by POSTGRES_DB env var)
-- But we can add additional setup here

-- Enable UUID extension for generating UUIDs
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create a schema for the application (optional, using public schema)
-- CREATE SCHEMA IF NOT EXISTS stackshare;

-- Grant permissions to the application user
GRANT ALL PRIVILEGES ON DATABASE stackshare TO stackshare;

-- You can add seed data here if needed
-- Example:
-- INSERT INTO technologies (id, name, category, description) VALUES 
--   (uuid_generate_v4(), 'React', 'Framework', 'JavaScript library for building user interfaces'),
--   (uuid_generate_v4(), 'Node.js', 'Language', 'JavaScript runtime environment');

SELECT 'Database initialization completed' AS status;