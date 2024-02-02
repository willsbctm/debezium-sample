CREATE TABLE IF NOT EXISTS outbox(
   id serial PRIMARY KEY,
   data jsonb (50) NOT NULL,
   type VARCHAR (50) NOT NULL
);