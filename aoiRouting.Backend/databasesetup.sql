-- Drop the existing database
DROP DATABASE nasa;

-- Create the database
CREATE DATABASE nasa;

-- Connect to the database
\c nasa;

-- Create the users table
CREATE TABLE IF NOT EXISTS users (
    id UUID NOT NULL PRIMARY KEY,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    created TIMESTAMP NOT NULL
);

-- Create the credentials table
create table if not exists credentials
(
    id UUID NOT NULL PRIMARY KEY,
    user_id UUID NOT NULL,
    kind INTEGER NOT NULL,
    identifier VARCHAR(1023) NOT NULL,
    secret VARCHAR(1023) NOT NULL
);

-- Create the pins table
CREATE TABLE IF NOT EXISTS pins (
    id UUID NOT NULL PRIMARY KEY,
    aoi_id UUID NOT NULL,
    user_id UUID NOT NULL,
    point_id INT NOT NULL,
    lat DOUBLE PRECISION NOT NULL, 
    lon DOUBLE PRECISION NOT NULL,
    notes TEXT,
    created TIMESTAMP NOT NULL,
    collected TIMESTAMP
);

-- Create the aois table
CREATE TABLE IF NOT EXISTS aois (
    id UUID NOT NULL PRIMARY KEY,
    user_id UUID NOT NULL,
    centroid_id UUID,
    notes TEXT,
    created TIMESTAMP NOT NULL
);

-- Add foreign key constraints
ALTER TABLE credentials ADD CONSTRAINT credentials_user_id_fk FOREIGN KEY (user_id) REFERENCES users(id);
ALTER TABLE pins ADD CONSTRAINT pins_aoi_id_fk FOREIGN KEY (aoi_id) REFERENCES aois(id);
ALTER TABLE pins ADD CONSTRAINT pins_user_id_fk FOREIGN KEY (user_id) REFERENCES users(id);
ALTER TABLE aois ADD CONSTRAINT aois_user_id_fk FOREIGN KEY (user_id) REFERENCES users(id);

-- Set table owners to postgres
ALTER TABLE users OWNER TO postgres;
ALTER TABLE pins OWNER TO postgres;
ALTER TABLE aois OWNER TO postgres;