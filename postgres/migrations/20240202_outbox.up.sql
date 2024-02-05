CREATE TABLE IF NOT EXISTS public.outbox(
   id uuid PRIMARY KEY,
   data jsonb NOT NULL,
   type VARCHAR (50) NOT NULL
);