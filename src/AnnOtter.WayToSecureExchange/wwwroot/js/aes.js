// Basic AES-GCM logic comes from enyachoke: https://gist.githubusercontent.com/enyachoke/5c60f5eebed693d9b4bacddcad693b47/raw/4d9f2acf7401002bdc065c2405a020e42565876c/aes.js

/**
 * Generates a new AES-GCM 256 Key
 * @returns {CryptoKey}
 */
async function generateKey() {
    let key = await crypto.subtle.generateKey(
        {
            name: "AES-GCM",
            length: 256
        },
        true,
        ["encrypt", "decrypt"]
    );

    return key;
}

/**
 * Exports a key to a hex string
 * @param {any} key
 * @returns {string}
 */
async function exportKey2hex(key) {
    let exportedKeyBuffer = await crypto.subtle.exportKey('raw', key);
    let exportedKeyArray = new Uint8Array(exportedKeyBuffer);
    return Array.from(exportedKeyArray).map(byte => byte.toString(16).padStart(2, '0')).join('');
}

/**
 * Imports a key from a hex string
 * @param {string} hexKey
 * @returns {CryptoKey}
 */
async function importKeyFromHex(hexKey) {
    let buffer = hex2buf(hexKey);

    let key = await crypto.subtle.importKey(
        'raw',
        buffer,
        {
            name: "AES-GCM",
            length: 256
        },
        true,
        ["decrypt"]
    );

    return key;
}

/**
 * Generates a SHA-256 hash of a string.
 * @param {any} message
 * @returns {String}
 */
async function sha256(message) {
    const msgBuffer = new TextEncoder("utf-8").encode(message);
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
 * Given a passphrase and some plaintext, this derives a key
 * (generating a new salt), and then encrypts the plaintext with the derived
 * key using AES-GCM. The ciphertext, salt, and iv are hex encoded.
 * @param {String} passphrase 
 * @param {String} plaintext
 * @returns {Promise<String>} 
 */
async function encrypt(key, plaintext) {
    const ivValue = crypto.getRandomValues(new Uint8Array(12));
    const data = str2buf(plaintext);

    const ciphertextValue = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivValue }, key, data);

    return {
        iv: buf2hex(ivValue),
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
async function decrypt(hexKey, iv, ciphertext) {
    const key = await importKeyFromHex(hexKey)
    const ivValue = hex2buf(iv);
    const dataValue = hex2buf(ciphertext);

    return crypto.subtle.decrypt({ name: "AES-GCM", iv: ivValue }, key, dataValue)
        .then(v => buf2str(new Uint8Array(v)));
}