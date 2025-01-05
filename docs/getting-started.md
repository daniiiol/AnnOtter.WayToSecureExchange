# Getting started

Before you can run the web application, make sure you have the latest version of Docker installed on your system.

Start by cloning the repository to your local machine or downloading it from the repository's page on GitHub.


## Fast start with Docker Run

```bash
docker run -p 8081:8080 daniiiol/annotter.way2secexchange:latest
```

Launch your browser on http://localhost:8081

## Compile and run with Docker Build

If you want to compile and run the application locally, you can do so with the following instruction:

### Navigate to Dockerfile
Navigate to `./src/AnnOtter.WayToSecureExchange/` and execute the following command in your favorite terminal application:

```bash
docker build . -t annotter.way2secexchange:dev
```

### Start the application (local image)
Now, start the web application with the following command:

```bash
docker run -p 8082:8080 annotter.way2secexchange:dev
```

Launch your browser on http://localhost:8082

## Run with Docker-Compose

Navigate to `./src/docker-compose/` and execute this command in your favorite terminal application: 

```bash
docker-compose -f .\compose.yml up
```

The application will then start with default settings and a local SQLite database on port `8080`. 

You can directly navigate to `http://localhost:8080`.

### Configuration

You can modify the application by activating the commented lines in the `docker-compose.yml` file and filling them with proper values.

The following is a brief explanation:

| Env Variable | Description | Example | Example Description |
|---|---|---|--|
| ASPNETCORE_ENVIRONMENT | The environment in which the web application is running (e.g., Development, Production). | `Production` | Runs the application in "Production" mode
| ASPNETCORE_URLS | The URLs on which the web application listens for incoming requests (e.g., http://+:80). | `http://+:80` | Runs the application on HTTP port 80
| POSTGRESQLCONNSTR_DatabaseConnection_AO_ExchangeDatabase | The connection string for the PostgreSQL database used by the application. | `Server=db.server.local;Database=ao_exchangedb;Port=5432;User Id=myUser;Password=myPassword;` | Uses the specified PostgreSQL database instead of an internal SQLite DB.
| ENCRYPTION__KEY | The base64-encoded 256-bit key used for double encryption. | `PmMRx1J8EccLYqC4boe62R8s/Ahkh1YnzueM96MqAgc=` | Uses the sample key for server-side encryption.
| MAIN__RETENTIONSPAN | The time span for retaining secrets in the application, e.g., deleting all secrets older than 30 days.<br><br>If you set `00.00:00:00`, the deletion of secrets will be disabled (not recommended). | `30.00:00:00` | Deletes all secrets older than 30 days
| RATELIMITER__SHORTBURSTPERMITLIMIT | The permit limit for short burst rate limiting. | `6` | Maximum 6 requests possible within the valid SHORTBURSTWINDOW.
| RATELIMITER__SHORTBURSTWINDOW | The window duration for short burst rate limiting. | `2` | The time limit for SHORTBURSTPERMITLIMIT is 2 seconds
| RATELIMITER__SHORTBURSTAUTOREPLENISHMENT | Whether short burst rate limiting auto-replenishes permits (true or false). | `true` | Specifies that the FixedWindowRateLimiter refreshes the counters automatically.
| RATELIMITER__GENERALPERMITLIMIT | The permit limit for general rate limiting. | `20` | Maximum 20 requests possible within the valid GENERALWINDOW
| RATELIMITER__GENERALWINDOW | The window duration for general rate limiting. | `30` | The time limit for GENERALPERMITLIMIT is 30 seconds
| RATELIMITER__GENERALAUTOREPLENISHMENT | Whether general rate limiting auto-replenishes permits (true or false). | `true` | Specifies that the FixedWindowRateLimiter refreshes the counters automatically.
| APPEARANCE__FAVICON | Base64 string of an `image/x-icon` image to overwrite the default favicon. | `AAABAAEAICAQAAAAAADoAgAAFgAAACgAAAAgAAAAQAAAAAEABAAAAAAAgAIAAAAAAAA....` | The favicon will be displayed on the browser tab.
| APPEARANCE__SHOWLOGO | Whether to show the application's logo (true or false). | `true` | The logo will be displayed on the page.
| APPEARANCE__LOGOPATH | The path to the application's logo image. <br><br>As other examples there are: `/img/aow2se-logo2.png` or `/img/aow2se-logo.png` built-in. | `/img/aow2se-logo3.png` | The specified logo will be loaded according to the sample address.
| APPEARANCE__ _colorname_ | If you would like to change the app colors, you can overwrite almost all hex-codes.  | See `appsettings.json` for all colors. | A valid HTML hex color code starts with a hash (#) followed by exactly 3 or 6 hexadecimal characters. E.g. `#123456`
| SECURITYHEADERS__ _headername_ | A specific security header name. <br><br>If you set an empty value, the specific security header will be disabled. | See `appsettings.json` for all security headers. | -
| LABELS__ _labelname_ | If you don't like the pirate language, you can overwrite almost all static display texts.  | See `appsettings.json` for all label names. | -