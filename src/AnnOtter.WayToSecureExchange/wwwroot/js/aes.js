// Basic AES-GCM logic comes from enyachoke: https://gist.githubusercontent.com/enyachoke/5c60f5eebed693d9b4bacddcad693b47/raw/4d9f2acf7401002bdc065c2405a020e42565876c/aes.js

/**
 * Creates a random 64 char string. 
 * (A-Z, a-z and following special chars: +,-,/,.)
 * @returns {String}
 */
function generateRandomKey() {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+_/.';
    let randomKey = '';
    for (let i = 0; i < 64; i++) {
        const randomIndex = Math.floor(Math.random() * characters.length);
        randomKey += characters.charAt(randomIndex);
    }
    return randomKey;
}

/**
 * Generates a SHA-256 hash of a string.
 * @param {any} message
 * @returns {String}
 */
async function sha256(message) {
    const msgBuffer = new TextEncoder().encode(message);
    const hashBuffer = await crypto.subtle.digest('SHA-256', msgBuffer);
    const hashArray = Array.from(new Uint8Array(hashBuffer));
    const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join('');

    return hashHex;
}

/**
 * Encodes a utf8 string as a byte array.
 * @param {String} str 
 * @returns {Uint8Array}
 */
function str2buf(str) {
    return new TextEncoder("utf-8").encode(str);
}

/**
 * Decodes a byte array as a utf8 string.
 * @param {Uint8Array} buffer 
 * @returns {String}
 */
function buf2str(buffer) {
    return new TextDecoder("utf-8").decode(buffer);
}

/**
 * Decodes a string of hex to a byte array.
 * @param {String} hexStr
 * @returns {Uint8Array} 
 */
function hex2buf(hexStr) {
    return new Uint8Array(hexStr.match(/.{2}/g).map(h => parseInt(h, 16)));
}

/**
 * Encodes a byte array as a string of hex.
 * @param {Uint8Array} buffer
 * @returns {String} 
 */
function buf2hex(buffer) {
    return Array.prototype.slice
        .call(new Uint8Array(buffer))
        .map(x => [x >> 4, x & 15])
        .map(ab => ab.map(x => x.toString(16)).join(""))
        .join("");
}

/**
 * Given a passphrase, this generates a crypto key
 * using `PBKDF2` with SHA256 and 1000 iterations.
 * If no salt is given, a new one is generated.
 * The return value is an array of `[key, salt]`.
 * @param {String} passphrase 
 * @param {UInt8Array} salt [salt=random bytes]
 * @returns {Promise<[CryptoKey,UInt8Array]>} 
 */
function deriveKey(passphrase, salt) {
    salt = salt || crypto.getRandomValues(new Uint8Array(8));
    return crypto.subtle
        .importKey("raw", str2buf(passphrase), "PBKDF2", false, ["deriveKey"])
        .then(key =>
            crypto.subtle.deriveKey(
                { name: "PBKDF2", salt, iterations: 1000, hash: "SHA-256" },
                key,
                { name: "AES-GCM", length: 256 },
                false,
                ["encrypt", "decrypt"],
            ),
        )
        .then(key => [key, salt]);
}

/**
 * Given a passphrase and some plaintext, this derives a key
 * (generating a new salt), and then encrypts the plaintext with the derived
 * key using AES-GCM. The ciphertext, salt, and iv are hex encoded.
 * @param {String} passphrase 
 * @param {String} plaintext
 * @returns {Promise<String>} 
 */
async function encrypt(passphrase, plaintext) {
    const ivValue = crypto.getRandomValues(new Uint8Array(12));
    const saltValue = crypto.getRandomValues(new Uint8Array(8));
    const data = str2buf(plaintext);

    const derivedKey = await deriveKey(passphrase, saltValue);
    const ciphertextValue = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivValue }, derivedKey[0], data);

    return {
        iv: buf2hex(ivValue),
        salt: buf2hex(saltValue),
        ciphertext: buf2hex(ciphertextValue)
    }
}

/**
 * Given a passphrase, salt, iv and ciphertext as given by `encrypt`,
 * this decrypts the ciphertext and returns the original plaintext
 * @param {String} passphrase 
 * @param {String} salt 
 * @param {String} iv
 * @param {String} ciphertext
 * @returns {Promise<String>}
 */
function decrypt(passphrase, salt, iv, ciphertext) {
    const saltValue = hex2buf(salt);
    const ivValue = hex2buf(iv);
    const dataValue = hex2buf(ciphertext);

    return deriveKey(passphrase, saltValue)
        .then(([key]) => crypto.subtle.decrypt({ name: "AES-GCM", iv: ivValue }, key, dataValue))
        .then(v => buf2str(new Uint8Array(v)));
}