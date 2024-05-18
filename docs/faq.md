# FAQ

## Q: Is this application really secure?

**A:** While no system can be guaranteed to be completely secure, it is our primary goal to ensure the highest levels of security for this application. We have implemented multiple layers of encryption and robust security measures to safeguard sensitive data during transmission and storage. Strong encryption algorithms are used both on the frontend and backend to secure the data. Additionally, best security practices such as using secure hash functions and regularly purging sensitive data from the database are implemented.

## Q: Which Docker Container do you use and why?
**A:** We utilize a Docker container based on the nightly build variant of Ubuntu 22.04 Jammy Chiseled. This image has been specifically curated by Microsoft to establish a hardened runtime environment. Its robust security features and compatibility with our application's requirements make it an ideal choice for ensuring the safety and stability of our services. Have a look at https://devblogs.microsoft.com/dotnet/dotnet-6-is-now-in-ubuntu-2204/ for more insides about the Docker base image.

## Q: Why do you encrypt with AEM-GCM in the frontend and ChaCha20Poly1305 in the backend?
**A:** We have deliberately chosen to employ two different encryption algorithms to ensure an additional layer of security. This strategy allows us to maintain data security even if one algorithm is found to be weak or compromised. By utilizing distinct algorithms for frontend and backend encryption, we enhance the application's overall resilience to potential security vulnerabilities.

The choice of encryption algorithm is based on the requirements of data transmission and performance needs. We utilize AEM-GCM in the frontend due to its robust support within the existing Web Crypto API and its optimal performance for symmetric encryption. In the backend, we have chosen ChaCha20Poly1305 for its feature of authenticated encryption, ensuring that any data modifications at the database level would be detected even before decryption, thus bolstering the application's overall data integrity.

## Q: What happens if the link is found by a malicious person?

**A:** If the data is intercepted by a malicious entity, it can only be loaded from the backend once. Consequently, the intended recipient would be unable to receive the data correctly, likely leading them to promptly notify the sender. Depending on the sensitivity of the content, the sender can respond accordingly, such as resetting passwords, rotating keys, or deactivating specific systems based on the nature of the compromised data.

## Q: How likely is it that a URL can be discovered through brute-forcing?

**A:** At present, we do not foresee the URL being feasibly discovered through brute-force methods. This is due to the fact that an attacker would need to know a data ID existing within the system, which corresponds to a standard GUID. Even if they were to discover an existing GUID, they would additionally need to possess the corresponding 32-byte key and the 12-byte IV to generate a correct link in that combination. Based on current knowledge, we do not consider this to be a viable method of attack.

## Q: What is the purpose of this project?
**A:** The primary objective of this project was to create a simple yet effective system for securely exchanging sensitive information. Many organizations often transmit various confidential data through insecure communication channels such as chat platforms or email systems, potentially exposing them to access by system administrators. With this small-scale web project, our aim was to mitigate this threat and provide a more secure means of data exchange.

## Q: I'm a system administrator, and I'd like to generate a valid 32-byte key for the backend system. What's the simplest way to do this?
**A:** There are many ways to achieve this, and nearly every modern programming or scripting language offers options for key generation. However, the quickest method is to open your web browser, switch to developer tools (Internet tools), and then enter the following code into the console:

```javascript
"Base64 Encoded Key: " + btoa(String.fromCharCode.apply(null, window.crypto.getRandomValues(new Uint8Array(32))))
```

This method utilizes a pseudo-random number generator algorithm (PRNG). For more information about the randomness of the `crypto.getRandomValues()` method, you can also refer to this [link](https://developer.mozilla.org/en-US/docs/Web/API/Crypto/getRandomValues).