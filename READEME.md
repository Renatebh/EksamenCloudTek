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

## Kommandoer for Testing

### Bygg og kjør prosjektet, fra rootmappen der yml filen er.

1. **Start Docker**:

   ```bash
   docker-compose up -d
2. **Verifiser at tjenestene kjører**:

   ```bash
   docker ps
3. **Tilgang til applikasjonen**:

Åpne postman eller ett annet api vertøy for å få tilgang til databasen.

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

# Testing API with curl

For å teste API-et ditt, kan du bruke curl:

1. **For å hente produkter:**
   Åpne terminalen og kjør:

   ```bash
   curl -v http://localhost/api/product
   ````

## For å teste i GitBash
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
## Feilsøking og Logg
Hvis du støter på problemer, kan du sjekke loggene for å få mer informasjon om hva som skjer.



# Publisering av Docker-imgages til Docker Hub

1 Logg på docker hub
2 lag nytt reposistory
3 Bygg Nginx Docker-image:
````
docker build -t renatehem/eksamen:nginx-v1 ./nginx
````
4 Push til Docker HUB:

Når bildet er bygget, kan du pushe det til Docker Hub:
````
docker push renatehem/eksamen:nginx-v1
````

5: Bruk Docker Hub-imagen i docker-compose.yml:

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