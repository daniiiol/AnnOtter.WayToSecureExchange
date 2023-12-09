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
 * Exports a key to a base64 string
 * @param {any} key
 * @returns {string}
 */
async function exportKey2base64(key) {
    let exportedKeyBuffer = await crypto.subtle.exportKey('raw', key);
    let exportedKeyArray = new Uint8Array(exportedKeyBuffer);
    let binaryString = Array.from(exportedKeyArray).map(byte => String.fromCharCode(byte)).join('');
    return btoa(binaryString);

}

/**
 * Imports a key from a base64 string
 * @param {string} base64key
 * @returns {CryptoKey}
 */
async function importKeyFromBase64(base64key) {
    let buffer = base64tobuf(base64key);
    let key = await crypto.subtle.importKey(
        'raw',
        buffer,
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
 * Decodes a base64 string to a byte array.
 * @param {String} base64str
 * @returns {Uint8Array} 
 */
function base64tobuf(base64str) {
    let binaryStr = atob(base64str);
    let bytes = new Uint8Array(binaryStr.length);

    for (let i = 0; i < binaryStr.length; i++) {
        bytes[i] = binaryStr.charCodeAt(i);
    }
    
    return bytes;
}

/**
 * Encodes a byte array as a base64 string.
 * @param {Uint8Array} buffer
 * @returns {String} 
 */
function buf2base64(buffer) {
    let uint8Array = new Uint8Array(buffer);
    let binaryString = Array.from(uint8Array).map(byte => String.fromCharCode(byte)).join('');

    return btoa(binaryString);
}

/**
 * Given a passphrase and some plaintext, this derives a key
 * (generating a new salt), and then encrypts the plaintext with the derived
 * key using AES-GCM. The ciphertext, salt, and iv are base64 encoded.
 * @param {String} key 
 * @param {String} plaintext
 * @returns {Promise<String>} 
 */
async function encrypt(key, plaintext) {
    const ivValue = crypto.getRandomValues(new Uint8Array(12));
    const data = str2buf(plaintext);

    const ciphertextValue = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivValue }, key, data);

    return {
        iv: buf2base64(ivValue),
        ciphertext: buf2base64(ciphertextValue)
    }
}

/**
 * Given a key, iv and ciphertext as given by `encrypt`,
 * this decrypts the ciphertext and returns the original plaintext
 * @param {String} base64key
 * @param {String} iv
 * @param {String} ciphertext
 * @returns {Promise<String>}
 */
async function decrypt(base64key, iv, ciphertext) {
    const key = await importKeyFromBase64(base64key)
    const ivValue = base64tobuf(iv);
    const dataValue = base64tobuf(ciphertext);

    return crypto.subtle.decrypt({ name: "AES-GCM", iv: ivValue }, key, dataValue)
        .then(v => buf2str(new Uint8Array(v)));
}