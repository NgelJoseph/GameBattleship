CREATE TABLE IF NOT EXISTS shipPosition (
    id VARCHAR(50),
	shipName VARCHAR(50),
    rowId int,
	columnId int
);

GRANT ALL ON TABLE public.shipPosition TO battleship;