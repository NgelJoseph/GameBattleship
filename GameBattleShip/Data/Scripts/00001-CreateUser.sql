DO
$body$
BEGIN
   IF NOT EXISTS (
      SELECT *
      FROM   pg_catalog.pg_user
      WHERE  usename = 'battleship') THEN

      CREATE USER battleship WITH
				LOGIN
				NOSUPERUSER
				NOCREATEDB
				NOCREATEROLE
				INHERIT
				NOREPLICATION
				CONNECTION LIMIT -1
				PASSWORD '$apiPassword$';
   END IF;
END
$body$;
