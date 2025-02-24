# API & Reverse Proxy Dokumentasjon

## Arkitektur og Konfigurasjon

Dette prosjektet består av tre hovedkomponenter:

- 1: Product API: Dette er en RESTful API som gir tilgang til produkter i systemet. API-et er bygget på en enkel applikasjon som kjører på port 8080.

- 2: MySQL Database: MySQL fungerer som databasen for produktdataene. Den lagrer informasjon om produkter som API-et henter og administrerer. Tjenesten bruker Docker og er koblet til product-db-databasen.

- 3: Nginx Reverse Proxy: Nginx fungerer som en reverse proxy for Product API-et, og ruter trafikken til riktig tjeneste. Nginx håndterer også en health-check endpoint for å kontrollere at API-et er oppe og kjører. Nginx-tjenesten kjører på port 80.

##  Docker Konfigurasjon

Alle komponentene er containerisert ved hjelp av Docker og er koblet sammen i et delt Docker-nettverk (product_network), som gjør at de kan kommunisere internt uten at de trenger å være eksponert direkte mot offentlig nettverk.

## Nettverksstruktur:

- Alle tjenestene kjører på et eget Docker-nettverk kalt product_network, som sørger for at Nginx kan kommunisere med API-et og MySQL-databasen.

- Nginx-proxyen er tilgjengelig på port 80 og videresender forespørsler til Product API-et på port 8080.
## Docker Compose Tjenester
-  Product API: Kjører på port 8080, og er avhengig av MySQL-tjenesten for databaseoperasjoner.
- MySQL: Kjører på standard MySQL-port 3306, og håndterer lagring og uthenting av produktdata.
- Nginx: Kjører på port 80 og fungerer som en reverse proxy for Product API-et.

## Hvordan Løsningene Samhandler
Nginx Reverse Proxy: Nginx fungerer som en mellommann som videresender innkommende forespørsler til Product API-et. Når en forespørsel gjøres til en av følgende URL-er, håndterer Nginx ruten:

http://localhost/api/products → Ruter til Product API for å hente alle produkter.
http://localhost/api/products/{id} → Ruter til Product API for å hente et spesifikt produkt basert på ID.
http://localhost/api/health → Returnerer en enkel helse-sjekk ("API OK") for å indikere at systemet er oppe og kjører.
Product API: Produkt-API-et håndterer forespørsler om produktdata og samhandler med MySQL-databasen for å hente produktinformasjon.

MySQL Database: MySQL-databasen lagrer produktdata og sørger for at Product API-et har tilgang til den nødvendige informasjonen.

## Database
MySQL-databasen er konfigurert med følgende miljøvariabler:

`MYSQL_ROOT_PASSWORD`: gokstad

`MYSQL_DATABASE`: -product-db

`MYSQL_USER`: product_api

`MYSQL_PASSWORD`: securepass

# Kommandoer for Testing

### Bygg og kjør prosjektet, fra rootmappen der yml filen er.

1. **Start Docker**:

   ```bash
   docker-compose up -d
2. **Verifiser at tjenestene kjører**:

   ```bash
   docker ps
3. **Tilgang til applikasjonen**:

## Testing i Postman

#### Hente alle produkter 
   Denne kommandoen gjør en GET-forespørsel til /api/products for å hente en liste over alle produkter.

   ````
   GET http://localhost/api/products
   ````
#### Hente et spesifikt produkt 
Denne kommandoen gjør en GET-forespørsel til /api/products/{id} for å hente et spesifikt produkt basert på ID. Erstatt {id} med et gyldig produkt-ID.

````
GET http://localhost/api/products/1
````
#### Health Check 
Denne kommandoen gjør en GET-forespørsel til /api/health for å sjekke om API-et er oppe og kjører.
````
GET http://localhost/api/health
````


## Teste API with curl

For å teste API-et ditt, kan du bruke curl:

1. **For å se om apiet kjører:**
   Åpne terminalen og kjør:

   ```bash
   curl -v http://localhost/api/product/health
   ````
2. **For å hente produkter:**
   Åpne terminalen og kjør:

   ```bash
   curl -v http://localhost/api/product
   ````
3. **For å hente ett spesifikt produkt:**
   Åpne terminalen og kjør:

   ```bash
   curl -v http://localhost/api/product/1
   ````

## Teste API i GitBash
````
winpty docker exec -it mysql mysql -u product_api -p
`````
Skriv inn passordet: `securepass`

Du er nå inne i mysql containeren og kan bruke sql spørringer mot tabellen.
````
SHOW DATABASES;

USE product-db;

SHOW TABLES;

SELECT * FROM Products;
````
# Feilsøking og Logg
Hvis du støter på problemer, kan du sjekke loggene for å få mer informasjon om hva som skjer.



# Publisering av Docker-images til Docker Hub

1. Logg på docker hub

2. Lag nytt reposistory

3. Bygg Nginx Docker-image:

   Du må være i samme mappe som dockerfilen for å bygge imaget
   ````
   docker build -t renatehem/eksamen:nginx-v1 ./nginx
   ````
4. Push til Docker HUB:

   Når bildet er bygget, kan du pushe det til Docker Hub:
   ````
   docker push renatehem/eksamen:nginx-v1
   ````

5. Bruk Docker Hub-imagen i docker-compose.yml:

   I docker-compose.yml kan du referere til bildet som ligger på Docker Hub, 
   i stedet for å bygge det lokalt.
   ````
   nginx:
      image: renatehem/eksamen:nginx-v1
      ports:
         - "80:80"
      depends_on:
         - productapi
   ````



# Oppgave 3: Deploy Api til Aws Ec2 Nginx.

 1.  **Sett opp EC2-instansen**

     Logg inn på AWS-kontoen og start en EC2-instans.
     Konfigurer sikkerhetsgruppen for å åpne portene 80 (HTTP) og 8080 (API) for offentlig tilgang.
   

   Launch instances -> gi den ett navn -> velg amazone Linux > key pair (ssh) 

   Network settings
   - Legg inn vpc du laget
   - Subnet velg public 1 for å nå api
   - Velg public IP **Enable**
   - Security group name lag et passende navn eks eksamen-sg
   - Tilate hva slags trafikk som skal komme inn:
     Vi må tilate ssh(for å komme inn med terminalen) og http (for å komme inn med browser)

   Launch instance

   Connect -> ssh client
   kopier ssh nøkkelen Eksempel

   ````
   ssh -i "eksamen.pem" ec2-user@ec2-13-49-225-186.eu-north-1.compute.amazonaws.com

   ````

   Åpne en terminal feks Power shell  og naviger til pem fila som automatisk ble lastet ned når man laget vpc.
   ```Power shell	
   cd /c/Users/Renate/Downloads
   ```

   lim inn ssh nøkkel
   ````Power shell
   ssh -i "eksamen.pem" ec2-user@ec2-13-49-225-186.eu-north-1.compute.amazonaws.com
   ````

   Hvis man får problemer med nøkkel, legg inn dette
   ```sh
   chmod 400 eksamen.pem
   ````

Før man installerer docker sørg for at systemet er oppdatert
````
sudo yum update -y
````

2.  **Installer Docker og Docker Compose på EC2**

    Kjør følgende kommandoer for å installere Docker på EC2:
   ````
   sudo yum install -y docker

   docker --version

   sudo service docker start
   ````

   Lag en ny mappe der docker-compose skal ligge for eks: 
   ````
   mkdir app && cd app
   ````

   Installer Docker Compose med følgende kommandoer:
   ````
   sudo curl -L https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m) -o /usr/local/bin/docker-compose
   ````

   Gi den kjørbare rettigheter:
   ````
   sudo chmod +x /usr/local/bin/docker-compose
   ````

Sjekk om  docker-compose er installert 
````
sudo docker-compose --version
````

3. **Last ned og konfigurere Docker Compose-fil**

   Opprettet en `docker-compose.yml-fil` på EC2-instansen for å konfigurere tjenestene:

 Opprett filen:
   ```sh
   nano docker-compose.yml
   ````

   Eksempel på innhold i docker-compose.yml:

```yml
services:
  productapi:
    image: renatehem/eksamen:productapi-v1
    container_name: productapi
    ports:
      - "8080:8080"
    depends_on:
      mysql:
        condition: service_healthy
    networks:
      - backend

  mysql:
    image: mysql:8.0
    container_name: mysql
    environment:
      MYSQL_ROOT_PASSWORD: gokstad
      MYSQL_DATABASE: product-db
    expose:
      - "3306"
    networks:
      - backend
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost", "-u", "root", "-p$MYSQL_ROOT_PASSWORD"]
      interval: 10s
      retries: 5
      start_period: 60s
      timeout: 10s

  nginx:
    image: renatehem/eksamen:nginx-v1
    container_name: nginx
    volumes:
      - "./nginx/nginx.conf:/etc/nginx/nginx.conf"
    ports:
      - "80:80"
    depends_on:
      - productapi
    networks:
      - backend

volumes:
  mysql_data:

networks:
  backend:
  ````

4. Kjør Docker Compose for å starte tjenestene

   Kjør følgende kommando for å starte tjenestene definert i docker-compose.yml:
   ```sh
   sudo docker-compose up -d
   ```` 

5. Test API-et
   Etter at tjenestene ble startet, fikk API-et tilgang gjennom Nginx på port 80.
   Testet API-et ved å bruke cURL eller Postman med følgende URL:
  
   http://13.49.225.186/api/product

   http://13.49.225.186/api/product/1

   http://13.49.225.186/api/product/health
   
6. Verifisering av at API-et fungerer

JSON:
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "brand": "Dell",
    "price": 12999.00,
    "stock": 50
  },
  {
    "id": 2,
    "name": "Smartphone",
    "brand": "Samsung",
    "price": 7999.00,
    "stock": 100
  },
  {
    "id": 3,
    "name": "Tablet",
    "brand": "Apple",
    "price": 9999.00,
    "stock": 30
  },
  {
    "id": 4,
    "name": "Smartwatch",
    "brand": "Fitbit",
    "price": 2499.00,
    "stock": 80
  }
]
````
7.  IP-adresse for den kjørende tjenesten

    Offentlig IP-adresse: 51.20.184.218


8.  Dokumentasjon av konfigurasjon
    Docker-tjenestene:

    `productapi:` Kjørte API-applikasjonen som eksponerte produktdata.

    `mysql:` Kjørte en MySQL-database for lagring av data.

    `nginx:` Reverse proxy for å eksponere API-et på port 80.

    `Nginx-konfigurasjon:` HTTP-forespørsler på port 80 ble videresendt til productapi på port 8080.

9.  **Testing og verifisering**

    Testet API-et via URL-en `http://51.20.184.218/api/product` i nettleser og cURL for å bekrefte at dataene ble returnert korrekt.


# Opprett en MySQL RDS-instans i Free Tier innenfor samme VPC som EC2-instansen
Følg disse stegene:

1. Logg inn på AWS Console
  - Gå til AWS RDS-konsollen.

2. Opprett en ny RDS-instans

  - Klikk på Create database
  - Velg MySQL som database-engine
  - Velg Free Tier
  - Velg MySQL 8.0 (eller nyere)
  - DB-instance identifier: product-db
  - Master username: product-api
  - Master password: securepass
  - Velg t3.micro som instance type (gratisnivå)

3. Velg nettverksinnstillinger

  - VPC: Velg samme VPC som EC2-instansen kjører i
  - Subnet group: Bruk standard eller lag en ny
  - Public access: NEI (forbedrer sikkerheten)
  - Security group: Velg EC2-instansens sikkerhetsgruppe
  - Database name: product_db

4. Opprett RDS-instansen

  - Klikk på Create database
  - Vent noen minutter til den blir tilgjengelig

# 2. Tillat tilkobling fra EC2-instansen

For at EC2 skal kunne koble til RDS:

  1. Gå til EC2 > Security Groups

  2. Finn Security Groupen som RDS-instansen bruker

  3. Klikk på Inbound Rules > Edit Inbound Rules

  4. Legg til en regel:

    - Type: MySQL/Aurora (port 3306)
    - Source: Security Groupen til EC2-instansen
    - Klikk Save rules