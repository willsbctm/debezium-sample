CREATE TABLE IF NOT EXISTS outbox(
   id uuid PRIMARY KEY,
   data jsonb NOT NULL,
   type VARCHAR (50) NOT NULL
);