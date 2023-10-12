
/**
 * Start logic after DOM content is loaded.
 */
document.addEventListener('DOMContentLoaded', async function () {

    // Exchange View
    if (window.location.pathname.startsWith("/exchange")) {

        const dataParameter = getParameterByName('data', window.location.search);

        if (dataParameter) {
            const result = await CheckIfSecretExists(dataParameter);
            const plaintextData = document.getElementById("plaintextData");
            const revealDataButton = document.getElementById("revealDataButton");

            if (result != -1) {
                plaintextData.value = '------- DATA FOUND -------';
                revealDataButton.disabled = '';
            }
            else {
                const hintWarning = document.getElementById("hintWarning");
                const hintNotFound = document.getElementById("hintNotFound");
                const statusIcon = document.getElementById("statusIcon");

                plaintextData.value = '------- DATA NOT FOUND -------';
                revealDataButton.disabled = 'disabled';
                revealDataButton.style.display = 'none';
                hintNotFound.style.display = '';
                hintWarning.style.display = 'none';
                statusIcon.className = 'fa-solid fa-ghost';

            }
        }
    }
});

/**
 * Extracts a specific GET parameter from the URL.
 * @param {String} name
 * @param {String} url
 * @returns {String}
 */
function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var params = {};
    var paramPairs = url.slice(url.indexOf('?') + 1).split('&');

    for (var i = 0; i < paramPairs.length; i++) {
        var paramPair = paramPairs[i].split('=');
        var paramName = decodeURIComponent(paramPair[0]);
        var paramValue = decodeURIComponent(paramPair[1] || "");
        params[paramName] = paramValue;
    }

    return params[name] || null;
}

/** 
 * Counts the characters in the plaintext field.
 */
function countChars() {
    const plainData = document.getElementById("plaintextData");
    const dataCounter = document.getElementById("dataCounter");
    const encryptDataButton = document.getElementById("encryptDataButton");
    const countValue = plainData.value.length;

    dataCounter.innerText = countValue + " / " + "2000";

    if (countValue > 2000) {
        dataCounter.className = 'counter warning';
        encryptDataButton.disabled = 'disabled';
    }
    else {
        dataCounter.className = 'counter';
        encryptDataButton.disabled = '';
    }
}

/**
 * Resets the Home-Site.
 */
function resetASecureExchange() {
    const plainData = document.getElementById("plaintextData");
    const encryptedKeyUrl = document.getElementById("encryptedKeyUrl");
    const resultbox = document.getElementById("resultbox");
    const resetDataButton = document.getElementById("resetDataButton");
    const encryptDataButton = document.getElementById("encryptDataButton");
    const resultboxError = document.getElementById("resultboxError");
    const errorMessage = document.getElementById("errorMessage");

    plainData.value = '';
    plainData.disabled = '';
    encryptedKeyUrl.value = '';
    resultbox.style.display = 'none';
    encryptDataButton.style.display = '';
    resetDataButton.style.display = 'none';
    resultboxError.style.display = 'none';
    errorMessage.value = "<ExceptionText>";
}

/**
 * Encrypts the plaintext and upload it to the server.
 */
async function generateASecureExchange() {
    const plainData = document.getElementById("plaintextData");

    try {
        if (plainData && plainData.value) {
            const textToEncrypt = plainData.value;
            const key = generateRandomKey();
            const encryptedData = await encrypt(key, textToEncrypt);
            const apiResponse = await UploadCiphertext(encryptedData.ciphertext);

            if (apiResponse.status == 0 &&
                encryptedData.ciphertext.length > 0) {

                const responseJson = JSON.parse(apiResponse.message);
                const localHash = await sha256(encryptedData.ciphertext);
                const serverHash = responseJson.uploadOriginHash;

                if (localHash != serverHash) {
                    ShowGenerationErrorBox("Mismatch of SHA256 detected. Do you have a man in the middle? Local-Hash: " + localHash + " / Server-Hash: " + serverHash);
                }
                else {
                    const host = window.location.protocol + "//" + window.location.host;
                    const link = host + "/exchange/?data=" + responseJson.uploadId + "#" + key + "-" + encryptedData.salt + "-" + encryptedData.iv;
                    ShowGenerationSuccessBox(link, localHash, serverHash);
                }

            }
            else {
                ShowErrorBox(apiResponse.message);
            }

        }
        else {
            console.info('No Input Data found.');
        }
    } catch (error) {
        console.error('Could not generate a secure exchange. Please make sure that you are using an updated browser with Web Crypto API support. Error: ' + error);
    }
}

/**
 * Shows a success box with a recipient link and additional hashing information.
 * @param {String} link
 * @param {String} localHash
 * @param {String} serverHash
 */
function ShowGenerationSuccessBox(link, localHash, serverHash) {
    const plainData = document.getElementById("plaintextData");
    const encryptedKeyUrl = document.getElementById("encryptedKeyUrl");
    const resetDataButton = document.getElementById("resetDataButton");
    const encryptDataButton = document.getElementById("encryptDataButton");
    const resultboxError = document.getElementById("resultboxError");
    const resultbox = document.getElementById("resultbox");
    const localHashEncrypted = document.getElementById("localHashEncrypted");
    const serverHashEncrypted = document.getElementById("serverHashEncrypted");

    localHashEncrypted.innerText = localHash;
    serverHashEncrypted.innerText = serverHash;
    plainData.disabled = 'disabled';
    resetDataButton.style.display = '';
    encryptDataButton.style.display = 'none';
    resultbox.style.display = '';
    resultboxError.style.display = 'none';

    encryptedKeyUrl.value = link;
}

/**
 * Shows a error box with a specific message.
 * @param {String} message
 */
function ShowGenerationErrorBox(message) {
    const plainData = document.getElementById("plaintextData");
    const resetDataButton = document.getElementById("resetDataButton");
    const encryptDataButton = document.getElementById("encryptDataButton");
    const resultboxError = document.getElementById("resultboxError");

    plainData.disabled = 'disabled';
    resetDataButton.style.display = '';
    encryptDataButton.style.display = 'none';
    errorMessage.value = message;
    resultboxError.style.display = '';
}

/**
 * Creates a request url and urlOptions object to fetch the secret data.
 * @param {String} httpMethod
 * @param {String} dataId
 * @returns {Object}
 */
function prepareSecretRequest(httpMethod, dataId) {
    const host = window.location.protocol + "//" + window.location.host;
    const url = host + '/api/exchange?data=' + dataId;

    return {
        requestOptions: {
            method: httpMethod,
            headers: {
                'Content-Type': 'application/json',
            }
        },
        url: url
    };
}

/**
 * Checks if a secret exists on the server with a specific dataId.
 * @param {String} dataId
 * @returns {Number}
 */
async function CheckIfSecretExists(dataId) {
    const request = prepareSecretRequest("HEAD", dataId);

    return fetch(request.url, request.requestOptions)
        .then(response => {
            if (!response.ok) {
                throw new Error('HTTP-Exception, Statuscode: ' + response.status);
            }

            return response.text();
        })
        .then(() => {
            return 0; // Exists
        })
        .catch(() => {
            return -1; // Failed or not Exists
        });
}

/**
 * Loads a specific secret from the server with a given dataId.
 * @param {String} dataId
 * @returns {Number}
 */
async function GetSecret(dataId) {
    const request = prepareSecretRequest("GET", dataId);

    return fetch(request.url, request.requestOptions)
        .then(response => {
            if (!response.ok) {
                throw new Error('HTTP-Exception, Statuscode: ' + response.status);
            }

            return response.text();
        })
        .then(data => {
            return {
                status: 0,
                message: data
            };
        })
        .catch(error => {
            return {
                status: -1,
                message: error
            };
        });
}

/**
 * Uploads a specific ciphertext to the server.
 * @param {String} ciphertext
 * @returns {Object}
 */
async function UploadCiphertext(ciphertext) {
    const host = window.location.protocol + "//" + window.location.host;
    const url = host + '/api/exchange/upload';

    const requestOptions = {
        method: 'POST',
        body: JSON.stringify(ciphertext),
        headers: {
            'Content-Type': 'application/json',
        },
    };

    return fetch(url, requestOptions)
        .then(response => {
            if (!response.ok) {
                throw new Error('HTTP-Exception, Statuscode: ' + response.status);
            }

            return response.text();
        })
        .then(data => {
            return {
                status: 0,
                message: data
            };
        })
        .catch(error => {
            return {
                status: -1,
                message: error
            };
        });
}

/**
 * Copies a URL to the OS clipboard.
 * @param {String} htmlElement
 */
async function copyToClipboard(htmlElement) {
    const plaintextData = document.getElementById(htmlElement);
    if (plaintextData) {

        try {
            await navigator.clipboard.writeText(plaintextData.value);
        } catch (err) {
            console.error('Could not copy page URL to clipboard. Please make sure that you are using an updated browser with Clipboard API support.');
        }
    }
}

/**
 * Reveals the secret based on the given dataId in the URL.
 */
async function revealData() {

    const plaintextData = document.getElementById("plaintextData");
    plaintextData.rows = 10;

    const dataParameter = getParameterByName('data', window.location.search);

    if (dataParameter) {

        const apiResponse = await GetSecret(dataParameter);

        if (apiResponse.status != -1) {
            decryptDataResponse(apiResponse.message);
        }
        else {
            const plaintextData = document.getElementById("plaintextData");
            const revealDataButton = document.getElementById("revealDataButton");

            plaintextData.value = apiResponse.message;
            revealDataButton.disabled = ''
            revealDataButton.innerText = "Try again to reveal";
        }
    }

}

/**
 * Decrypts the data object of the server and refreshs the UI
 * @param {Object} responseData
 */
async function decryptDataResponse(responseData) {
    try {
        const responseObject = JSON.parse(responseData);
        const hashValue = window.location.hash.replace("#", "");
        const decryptKeyData = hashValue.split("-");
        const decryptedText = await decrypt(decryptKeyData[0], decryptKeyData[1], decryptKeyData[2], responseObject.data);
        const localHash = await sha256(responseObject.data);

        updateFrontendAfterDecryption(decryptedText, responseObject.hash, localHash);
    } catch (error) {
        console.error('Could not decrypt the data object. Please make sure that you are using an updated browser with Web Crypto API support. Error: ' + error);
        updateFrontendAfterDecryptionError();
    }
}

/**
 * Updates the frontend after decryption error 
 */
function updateFrontendAfterDecryptionError() {
    const plaintextData = document.getElementById("plaintextData");
    const revealDataButton = document.getElementById("revealDataButton");
    const statusIcon = document.getElementById("statusIcon");
    const hintWarning = document.getElementById("hintWarning");

    plaintextData.value = "Your decryption keys were not correct. For security reasons, the secret was removed.\n\nPlease contact the sender of this information and request a re-generation of the secret.\n\nWe have no way to restore your secret.";
    revealDataButton.style.display = 'none';
    revealDataButton.innerText = "Error";
    statusIcon.className = 'fa-solid fa-triangle-exclamation';
    hintWarning.style.display = 'none';
}

/**
 * Updates the frontend after decryption of the secret
 * @param {String} decryptedText
 * @param {String} serverHash
 * @param {String} localHash
 */
function updateFrontendAfterDecryption(decryptedText, serverHash, localHash) {
    const plaintextData = document.getElementById("plaintextData");
    const revealDataButton = document.getElementById("revealDataButton");
    const serverHashEncrypted = document.getElementById("serverHashEncrypted");
    const localHashEncrypted = document.getElementById("localHashEncrypted");
    const resultLeadElement = document.getElementById("resultLeadElement");
    const statusIcon = document.getElementById("statusIcon");
    const hintWarning = document.getElementById("hintWarning");
    const hintSuccess = document.getElementById("hintSuccess");

    serverHashEncrypted.innerText = serverHash;
    localHashEncrypted.innerText = localHash;

    plaintextData.value = decryptedText;
    plaintextData.className = 'form-control';
    revealDataButton.disabled = 'disabled';
    revealDataButton.innerText = "Revealed";
    revealDataButton.style.display = 'none';
    resultLeadElement.style.display = '';
    statusIcon.className = 'fa-solid fa-lock-open';
    hintWarning.style.display = 'none';
    hintSuccess.style.display = '';
}