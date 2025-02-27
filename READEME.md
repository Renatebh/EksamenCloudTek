# API & Reverse Proxy Dokumentasjon

Git hub  repository: https://github.com/Renatebh/EksamenCloudTek


docker repository: https://hub.docker.com/repository/docker/renatehem/eksamen
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
-  **Product API:** Kjører på port 8080, og er avhengig av MySQL-tjenesten for databaseoperasjoner.
- **MySQL:** Kjører på standard MySQL-port 3306, og håndterer lagring og uthenting av produktdata.
- **Nginx:** Kjører på port 80 og fungerer som en reverse proxy for Product API-et.

## Hvordan Løsningene Samhandler
**Nginx Reverse Proxy:** 

Nginx fungerer som en mellommann som videresender innkommende forespørsler til Product API-et. Når en forespørsel gjøres til en av følgende URL-er, håndterer Nginx ruten:

http://localhost/api/products → Ruter til Product API for å hente alle produkter.

http://localhost/api/products/{id} → Ruter til Product API for å hente et spesifikt produkt basert på ID.

http://localhost/api/health → Returnerer en enkel helse-sjekk ("API OK") for å indikere at systemet er oppe og kjører.

**Product API:** 

Produkt-API-et håndterer forespørsler om produktdata og samhandler med MySQL-databasen for å hente produktinformasjon.

**MySQL Database:**

MySQL-databasen lagrer produktdata og sørger for at Product API-et har tilgang til den nødvendige informasjonen.


# Kommandoer for Testing

### Bygg og kjør prosjektet, fra rootmappen der yml filen er.

1. **Start Docker**:

   ```bash
   docker-compose up -d
2. **Verifiser at tjenestene kjører**:

   ```bash
   docker ps
   ````


## Teste API with curl

For å teste API-et, kan du også bruke curl:

1. **For å se om apiet kjører:**

   ```bash
   curl http://localhost/api/product/health
   ````
2. **For å hente produkter:**

   ```bash
   curl http://localhost/api/product
   ````
3. **For å hente ett spesifikt produkt:**
  

   ```bash
   curl http://localhost/api/product/1
   ````


<br/>

# Publisering av Docker-images til Docker Hub

1. Logg på docker hub

2. Lag nytt reposistory

3. Bygg Nginx Docker-image:

   Du må være i samme mappe som dockerfilen for å bygge imaget
   ````
   docker build -t renatehem/eksamen:nginx-v1 .
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


<br/>


# Oppgave 3: Deploy Api til Aws Ec2 Nginx.


## 1. Deploy

 1.  **Sett opp EC2-instansen**

     Logg inn på AWS-kontoen og start en EC2-instans.
     Konfigurer sikkerhetsgruppen for å åpne portene 80 (HTTP) og 8080 (API) for offentlig tilgang.
   

    Launch instances -> gi den ett navn -> velg amazone Linux > key pair (ssh) 

  2. **Network settings**

    - Legg inn vpc du laget

    - Subnet velg public 1 for å nå api

    - Velg public IP **Enable**

    - Security group name lag et passende navn eks eksamen-sg

    - Tilate hva slags trafikk som skal komme inn:

      Vi må tilate ssh(for å komme inn med terminalen) og http (for å komme inn med browser)

  3. **Launch instance**

      - Connect -> ssh client

      - Kopier ssh nøkkelen Eksempel

   ````
   ssh -i "eksamen.pem" ec2-user@ec2-13-49-225-186.eu-north-1.compute.amazonaws.com

   ````

   4. Åpne en terminal feks Power shell  og naviger til pem fila som automatisk ble lastet ned når man laget vpc.
   ```Power shell	
   cd /c/Users/Renate/Downloads
   ```

   5. Lim inn ssh nøkkel
   ````Power shell
   ssh -i "eksamen.pem" ec2-user@ec2-13-49-225-186.eu-north-1.compute.amazonaws.com
   ````

Før man installerer docker sørg for at systemet er oppdatert
````
sudo yum update -y
````

### 2. **Installer Docker og Docker Compose på EC2**

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


# Oppgave 4: Migrering til AWS RDS (20%)
### Opprett en MySQL RDS-instans i Free Tier

  **Handling:**

  Opprettet en MySQL RDS-instans via AWS Management Console.
  Instansen ble plassert i samme VPC som EC2-instansen for å sikre intern kommunikasjon.

  **Konfigurasjon:**
  ````
  Database: product_db
  Brukernavn: product-api
  Passord: securepass
````

### Konfigurer RDS

 **Handling:**

  Oppdaterte RDS-sikkerhetsgruppen slik at den tillater tilkobling fra EC2-instansens sikkerhetsgruppe på port 3306.

  **Resultat:**

   RDS-databasen kan nå nås fra EC2-instansen via dens endepunkt.

### Modifiser docker-compose.yml på EC2-instansen

**Handling:**

  Fjernet MySQL-containeren fra docker-compose.yml for å benytte RDS i stedet.
  Oppdatert API-konfigurasjonen (environment) til å bruke den nye connection stringen med RDS-endepunktet. 
  
  Jeg måtte også lage ett nytt bilde av api med connection til RDS databasen.
  
  ```json

  {
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "DefaultConnection": "server=product-db.c9y6cqkmwx18.eu-north-1.rds.amazonaws.com;database=product_db;uid=product_api;pwd=securepass;port=3306"
    },
    "Kestrel": {
        "Endpoints": {
            "Http": {
                "Url": "http://0.0.0.0:8080"
            }
        }
    }
}

  ````
  Eksempel på oppdatert docker-compose.yml:

```yaml
services:
  productapi:
    image: renatehem/eksamen:productapi-v2
    container_name: productapi
    ports:
      - "8080:8080"
    environment:
      - CONNECTION_STRING=server=product-db.c9y6cqkmwx18.eu-north-1.rds.amazonaws.com;database=product_db;uid=product_api;pwd=securepass;port=3306
    networks:
      - backend

  nginx:
    image: renatehem/eksamen:nginx-v1
    container_name: nginx
    ports:
      - "80:80"
    depends_on:
      - productapi
    networks:
      - backend

networks:
  backend:
  ````
### Oppsett av den initielle Products-tabellen i RDS

**Handling:**

Tabellen i RDS ble opprettet igjennom entityframework så jeg måtte slette den tabellen jeg allerede hadde lagt inn.

### Test API-et for å verifisere løsningen
**Handling:**

Etter oppdateringene ble API-et startet jeg docker-compose på EC2-instansen.
Testet API-endepunktene (f.eks. via curl eller Postman) for å bekrefte at API-et nå kobler seg til RDS-databasen.

**Verifisering:**

Endepunkter som ``http://13.15.161.23/api/products`` bekreftet at data ble hentet riktig via RDS.
```bash
$ curl http://13.51.161.23/api/product
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed
100   285    0   285    0     0    446      0 --:--:-- --:--:-- --:--:--   446[{"id":1,"name":"Laptop","brand":"Dell","price":12999.00,"stock":50},{"id":2,"name":"Smartphone","brand":"Samsung","price":7999.00,"stock":100},{"id":3,"name":"Tablet","brand":"Apple","price":9999.00,"stock":30},{"id":4,"name":"Smartwatch","brand":"Fitbit","price":2499.00,"stock":80}]
````
<br />

# Oppgave 5: AWS CloudWatch Monitorering

<br />

### IAM-rolle konfigurasjonen
 Opprette en IAM-rolle med nødvendige tillatelser:

- Gå til IAM-konsollen.

- Velg "Roles" og deretter "Create role".

- Velg "AWS-servicec" og under "Use Cases velg "EC2".

- Add persmissions valgte jeg CloudWatchEventsFullAccess - for alle retigheter

- Role name ga jeg "CloudWatchRole" -> "Create".

<br/>

###  CloudWatch Agent oppsett 
**Amazon Linux og Ubuntu:** 

  - Åpne terminalen og kjør følgende kommandoer:

  ```bash
  wget https://s3.amazonaws.com/amazoncloudwatch-agent/amazon_linux/amd64/latest/amazon-cloudwatch-agent.rpm
  sudo rpm -U ./amazon-cloudwatch-agent.rpm
  ````

 - Kjør konfigurasjonsveiviseren:

 ```bash
 sudo /opt/aws/amazon-cloudwatch-agent/bin/amazon-cloudwatch-agent-config-wizard
 ```

<br />

 ## Konfigurasjon av CloudWatch Dashboard

#### 1. Opprette et Dashboard:

    - Gå til CloudWatch-konsollen.

    - Klikk på "Dashboards" i navigasjonsmenyen.

    - Velg "Create dashboard" og gi det et navn.

#### 2. Legge til Widgets:

    - Etter å ha opprettet dashboardet, klikk "Add widget".

    - Velg ønsket widget-type (f.eks. Line, Stacked area, etc.).

    - Klikk "Configure" for å velge hvilke metrikker som skal vises.

    - Velg riktig namespace (f.eks. "ProductApi").

    - Velg de metrikker du ønsker å inkludere og konfigurer visningsalternativer etter behov.

<br/> 

## 3. Kodeendringer i API-et for metrikk-logging



#### Opprettet en tjeneste for for sending av metrikker til cloudWatch.

```csharp
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;

public class CloudWatchMetricsService
{
    private readonly IAmazonCloudWatch _cloudWatchClient;

    public CloudWatchMetricsService(IAmazonCloudWatch cloudWatchClient)
    {
        _cloudWatchClient = cloudWatchClient;
    }

    public async Task SendMetricAsync(string metricName, double value)
    {
        var metricData = new MetricDatum
        {
            MetricName = metricName,
            Unit = StandardUnit.Count,
            Value = value,
            Timestamp = DateTime.UtcNow
        };

        var request = new PutMetricDataRequest
        {
            Namespace = "ProductApi",
            MetricData = new List<MetricDatum> { metricData }
        };

        await _cloudWatchClient.PutMetricDataAsync(request);
    }
}

````

#### Ett middleware for å telle api call.

```charp
using ProductApi.Services;

namespace ProductApi.Middleware
{
    public class ApiCallCount
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiCallCount> _logger;
        private readonly CloudWatchMetricsService _metricsService;
        private static int _requestCount = 0;

        public ApiCallCount(RequestDelegate next, ILogger<ApiCallCount> logger, CloudWatchMetricsService metricsService)
        {
            _next = next;
            _logger = logger;
            _metricsService = metricsService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            _requestCount++;
            _logger.LogInformation("API call to {Path} count: {Count}", context.Request.Path, _requestCount);

  
            await _metricsService.SendApiCallCountMetricAsync(_requestCount);

            await _next(context);
        }
    }
}

````

### Integrasjon i applikasjonen:

For å ta i bruk denne midddleware i ASP.NET Core-applikasjon, registreres CloudWatchMetricsService som en tjeneste og legges til ApiCallCount-middleware i HTTP-pipelinen.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Registrer CloudWatchMetricsService som en singleton
builder.Services.AddSingleton<CloudWatchMetricsService>();

var app = builder.Build();

// Legger til ApiCallCount-middleware i pipelinen
app.UseMiddleware<ApiCallCount>();

app.Run();
````

Lage nytt bilde med den oppdaterte funksjonaliteten og bruke det nye bildet i compose

```yml
services:
  productapi:
    image: renatehem/eksamen:productapi-v5
    container_name: productapi
    ports:
      - "8080:8080"
    environment:
      - CONNECTION_STRING=server=product-db.c9y6cqkmwx18.eu-north-1.rds.amazonaws.com;database=product_db;uid=product_api;pwd=securepass;port=3306
    networks:
      - backend

  nginx:
    image: renatehem/eksamen:nginx-v1
    container_name: nginx
    ports:
      - "80:80"
    depends_on:
      - productapi
    networks:
      - backend

networks:
  backend:
````