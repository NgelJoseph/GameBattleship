## Running locally

### Pre-requisites

* Docker (https://docs.docker.com/docker-for-windows/)
* pgAdmin [Install pgAdmin](https://www.pgadmin.org/download/windows4.php)
* Visual Studio

### Before hitting F5
* Start Postgres ```docker run -it --rm --name battleship-postgres -e POSTGRES_PASSWORD=password -d -p 5432:5432 postgres```
* Open pgAdmin and add connection to localhost ```user: postgres```, ```password: password```
* Now hit ```F5```

### Future Improvements
* Add more tests
* Move in memory cache to distributed cache
* Add DB connection to appsettings.<environment> files
* Deploy the code