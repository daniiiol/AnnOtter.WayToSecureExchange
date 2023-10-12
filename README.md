# Ann Otter Way To Secure Exchange

This repository contains a .NET web application designed for secure information exchange. The web application facilitates the exchange of confidential data with several layers of encryption and security measures:

- **Sender-Side Encryption:** Before the sender uploads any confidential text to the server, it is already encrypted, adding an initial layer of security.
- **Optional API Gateway Encryption:** Depending on the image's configuration, the confidential text can be further encrypted at the API Gateway before being stored in the database. This optional step provides an additional layer of security for your data.
- **Secure Sharing:** The sender is provided with a URL that can be shared with the intended recipient. Once accessed, the recipient can view the confidential text just once.
- **Self-Destructing Data:** After the text is decrypted by the recipient, it is automatically removed from the database, ensuring that sensitive information is not stored indefinitely.

## Getting started

Before you can run the web application, make sure you have the latest version of Docker installed on your system.

Start by cloning the repository to your local machine or downloading it from the repository's page on GitHub.

### Fast run with Docker-Compose

Navigate to `./src/docker-compose/` and execute this command in your favorite terminal application: 

```bash
docker-compose -f .\compose.yml up
```

The application will then start with default settings and a local SQLite database on port `8080`. 

You can directly navigate to `http://localhost:8080`.

#### Configuration

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
| APPEARANCE__SHOWLOGO | Whether to show the application's logo (true or false). | `true` | The logo will be displayed on the page.
| APPEARANCE__LOGOPATH | The path to the application's logo image. <br><br>As other examples there are: `/img/aow2se-logo2.png` or `/img/aow2se-logo.png` built-in. | `/img/aow2se-logo3.png` | The specified logo will be loaded according to the sample address.
| SECURITYHEADERS__ _headername_ | A specific security header name. <br><br>If you set an empty value, the specific security header will be disabled. | See `appsettings.json` for all security headers. | -
| LABELS__ _labelname_ | If you don't like the pirate language, you can overwrite almost all static display texts.  | See `appsettings.json` for all label names. | -

### Compile and run with Docker Build

If you want to compile and run the application locally, you can do so with the following instruction:

#### Navigate to Dockerfile
Navigate to `./src/AnnOtter.WayToSecureExchange/` and execute the following command in your favorite terminal application:

```bash
docker build . -t annotter.way2secexchange:latest
```

#### Start the application
Now, start the web application with the following command:

```bash
docker run -p 8080:80 annotter.way2secexchange:latest
```

## How it works

This section provides an overview of how the application facilitates the secure sharing of temporary secret information from Alice to Bob.

![Sample Screenshot of a communication between Alice and Bob.](./docs/images/sample_screenshot_1.png)

### Generating and Encrypting the Secret
1. **Alice's Input:** Alice begins the process by entering her secret information into the application's text field.

1. **Generation of a Random Key:** When Alice clicks the "Generate" button, a 64-character random passphrase is generated and the application creates a PBKDF2-based crypto key with SHA256 and 1000 iterations.

1. **AES-GCM Encryption:** This random key is utilized to encrypt Alice's secret text using the AES-GCM encryption method. Notably, all these operations are conducted within the user's web browser, leveraging the Web Crypto API.

### Uploading and Storing the Encrypted Data
1. **Uploading to the Application Server:** The encrypted ciphertext is then uploaded to an application server.

1. **Confirmation by the API:** The server's API confirms the successful upload, and Alice can compare the SHA256 hash of the application server's response with her locally stored hash.

1. **Secondary Encryption on the Server:** To prevent potential data leaks caused by database access, the application server further encrypts the received ciphertext using ChaCha20Poly1305. The 256 bit key for ChaCha20 is exclusively stored on the application server.

### Sharing with Bob
1. **Unique URL Generation:** After encryption and successful data storage, the browser generates a unique URL for the secret, which Alice can share with Bob.

### Bob's Access and Decryption
1. **Bob Receives the URL:** Alice shares the URL with Bob, who uses it to access the secret.

1. **URL Contents:** The URL contains the secret's ID (query param) from the database and decryption information that is never transmitted to the server (utilizing hashtag information in the link).

1. **Verification:** Upon accessing the URL, the system checks if a secret with the corresponding ID exists on the server.

1. **Revealing the Secret:** If a match is found, Bob can click the "Reveal" button to retrieve the secret from the server.

1. **Server Decryption:** The application server retrieves data from the database, decrypts the ChaCha20 ciphertext, and transmits the already AES-GCM-encrypted information to Bob's browser.

1. **Server-Side Key Deletion:** After successful decryption on the server, the key is irrevocably deleted from the database server.

1. **Local Decryption:** Bob's browser uses the keys obtained from the URL's hashtag to locally decrypt the received ciphertext, allowing him to read Alice's message in plain text.

In the event that the local keys are incorrect or if someone has intercepted the message (e.g., Eve), an appropriate error message will appear, enabling Bob to notify Alice of potential issues with the transmission or the possibility of interception.

### Housekeeping

The application allows you to configure the "retention time" for secrets. This feature enables the automatic deletion of secrets that have exceeded the specified retention time on the server.