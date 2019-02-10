CREATE TABLE IF NOT EXISTS playerInfo (
    attemptId SERIAL PRIMARY KEY,
    rowId int,
	columnId int,
	attackStatus varchar(10),
	shipId VARCHAR(50)
);

GRANT ALL ON TABLE public.playerInfo TO battleship;