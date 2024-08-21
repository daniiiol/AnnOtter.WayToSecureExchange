
/**
 * Start logic after DOM content is loaded.
 */
document.addEventListener('DOMContentLoaded', async function () {

    const copyUrlButton = document.getElementById("copyUrlButton");
    if (copyUrlButton) {
        copyUrlButton.addEventListener('click', function () {
            copyToClipboard('encryptedKeyUrl');
        });
    }

    const copyKeyButton = document.getElementById("copyKeyButton");
    if (copyKeyButton) {
        copyKeyButton.addEventListener('click', function () {
            copyToClipboard('protectUrlPassword');
        });
    }

    const refreshKeyButton = document.getElementById("refreshKeyButton");
    if (refreshKeyButton) {
        refreshKeyButton.addEventListener('click', function () {
            initializeUrlProtectionField(true);
        });
    }

    const copyErrorButton = document.getElementById("copyErrorButton");
    if (copyErrorButton) {
        copyErrorButton.addEventListener('click', function () {
            copyToClipboard('errorMessage');
        });
    }

    const encryptDataButton = document.getElementById("encryptDataButton");
    if (encryptDataButton) {
        encryptDataButton.addEventListener('click', function () {
            generateASecureExchange();
        });
    }

    const plaintextData = document.getElementById("plaintextData");
    if (plaintextData) {
        plaintextData.addEventListener('keyup', function () {
            countChars();
        });
    }

    const copyPlaintextButton = document.getElementById("copyPlaintextButton");
    if (copyPlaintextButton) {
        copyPlaintextButton.addEventListener('click', function () {
            copyToClipboard('plaintextData');
        });
    }

    const revealDataButton = document.getElementById("revealDataButton");
    if (revealDataButton) {
        revealDataButton.addEventListener('click', function () {
            revealData();
        });
    }

    const resetExchangeButton = document.getElementById("resetExchangeButton");
    if (resetExchangeButton) {
        resetExchangeButton.addEventListener('click', function () {
            resetASecureExchange();
        });
    }

    const protectUrlSwitcher = document.getElementById("protectUrl");
    if (protectUrlSwitcher) {
        protectUrlSwitcher.addEventListener('click', function () {

            if (protectUrlSwitcher.hasAttribute('checked')) {
                protectUrlSwitcher.removeAttribute('checked');
            }
            else {
                protectUrlSwitcher.setAttribute('checked','checked');
            }

            initializeUrlProtectionField(protectUrlSwitcher.hasAttribute('checked'));
        });
    }

    const keyInput = document.getElementById("keyInput");
    if (keyInput) {
        keyInput.addEventListener("keypress", (event) => {
            if (event.code == 'Enter') {
                unprotectUrlAndGoTo();
            }
        });
    }

    const decryptUrlButton = document.getElementById("decryptUrlButton");
    if (decryptUrlButton) {
        decryptUrlButton.addEventListener('click', function () {
            unprotectUrlAndGoTo();
        });
    }

    // Protect View
    if (window.location.pathname.startsWith("/protect")) {

        const hashString = window.location.hash;
        const keyInput = document.getElementById("keyInput")

        if (keyInput && hashString.startsWith("#P-")) {
            const decryptUrlButton = document.getElementById("decryptUrlButton");
            decryptUrlButton.disabled = '';
        }
        else {
            const urlInvalidMessage = document.getElementById("urlInvalidMessage")
            urlInvalidMessage.style.display = '';
            keyInput.disabled = 'disabled';
            decryptUrlButton.disabled = 'disabled';
        }
    }
        
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
                success();
            }
            else {
                const hintWarning = document.getElementById("hintWarning");
                const hintNotFound = document.getElementById("hintNotFound");
                const statusIcon = document.getElementById("statusIcon");

                fail();
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
 * Loader: Set fail state
 */
function fail() {
    const loader = document.querySelector('.loader');
    loader.classList.add('failSign');
}

/**
 * Loader: Set success state
 */
function success() {
    const loader = document.querySelector('.loader');
    loader.classList.add('successSign');
}

/**
 * Loader: Reset loader
 */
function reset  () {
    const loader = document.querySelector('.loader');
    loader.classList.remove('failSign');
    loader.classList.remove('successSign');
}

/**
 * Try to unprotect the URL and go to the secret exchange address
 */
async function unprotectUrlAndGoTo() {

    const hashString = window.location.hash;
    const keyInput = document.getElementById("keyInput")

    if (keyInput && hashString.startsWith("#P-")) {
        const keyInvalidMessage = document.getElementById("keyInvalidMessage");
        keyInvalidMessage.style.display = 'none';

        const hashValue = window.location.hash.replace("#P-", "");
        const decryptKeyData = hashValue.split("-");

        const decryptUrlButton = document.getElementById("decryptUrlButton");
        decryptUrlButton.disabled = '';

        try {
            const decryptedText = await decrypt(keyInput.value, decodeURIComponent(decryptKeyData[1]), decodeURIComponent(decryptKeyData[0]));
            const host = window.location.protocol + "//" + window.location.host;
            window.location.href = host + "/exchange/" + decryptedText;

        } catch (e) {
            keyInvalidMessage.style.display = '';
        }
    }
}

/**
 * Show or Hide the URL Protection PasswordField
 * @param {boolean} checked
 */
async function initializeUrlProtectionField(checked) {
    const protectUrlPasswordBox = document.getElementById("protectUrlPasswordBox");
    const protectUrlPassword = document.getElementById("protectUrlPassword");

    if (protectUrlPasswordBox) {
        if (checked) {
            protectUrlPasswordBox.style.display = '';
            const key = await generateKey();
            const exportedKey = await exportKey2base64(key);
            protectUrlPassword.value = exportedKey
        }
        else {
            protectUrlPasswordBox.value = '';
            protectUrlPasswordBox.style.display = 'none';
        }
    }
}

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
    else if (countValue == 0) {
        dataCounter.className = 'counter';
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
    const protectUrl = document.getElementById("protectUrl");
    const refreshKeyButton = document.getElementById("refreshKeyButton");
    const protectUrlPasswordBox = document.getElementById("protectUrlPasswordBox");
    
    plainData.value = '';
    plainData.disabled = '';
    encryptedKeyUrl.value = '';
    resultbox.style.display = 'none';
    encryptDataButton.style.display = '';
    encryptDataButton.disabled = 'disabled';
    resetDataButton.style.display = 'none';
    resultboxError.style.display = 'none';
    errorMessage.value = "<ExceptionText>";

    if (protectUrlPasswordBox.style.display == '') {
        initializeUrlProtectionField(true);
    }

    countChars();

    protectUrl.disabled = '';
    refreshKeyButton.style.display = '';
}

/**
 * Encrypts the plaintext and upload it to the server.
 */
async function generateASecureExchange() {
    const plainData = document.getElementById("plaintextData");

    try {
        if (plainData && plainData.value) {
            const textToEncrypt = plainData.value;
            const key = await generateKey();
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
                    const exportedKey = await exportKey2base64(key);
                    const host = window.location.protocol + "//" + window.location.host;
                    let urlParameters = "?data=" + encodeURIComponent(responseJson.uploadId) + "#" + encodeURIComponent(exportedKey) + "-" + encodeURIComponent(encryptedData.iv);
                    let urlController = "exchange";

                    const protectUrlPassword = document.getElementById("protectUrlPassword");
                    const protectUrlSwitcher = document.getElementById("protectUrl");

                    if (protectUrlSwitcher.hasAttribute('checked') && protectUrlPassword.value) {
                        const key = await importKeyFromBase64(protectUrlPassword.value);
                        const encryptedUrl = await encrypt(key, urlParameters);
                        urlParameters = "#P-" + encodeURIComponent(encryptedUrl.ciphertext) + "-" + encodeURIComponent(encryptedUrl.iv)
                        urlController = "protect";
                    }

                    const link = host + "/" + urlController + "/" + urlParameters
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
    const refreshKeyButton = document.getElementById("refreshKeyButton");
    const protectUrl = document.getElementById("protectUrl");

    localHashEncrypted.innerText = localHash;
    serverHashEncrypted.innerText = serverHash;
    plainData.disabled = 'disabled';
    resetDataButton.style.display = '';
    encryptDataButton.style.display = 'none';
    resultbox.style.display = '';
    resultboxError.style.display = 'none';
    refreshKeyButton.style.display = 'none';
    protectUrl.disabled = 'disabled';

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
    const allowedMethods = ['GET', 'HEAD'];
    if (!allowedMethods.includes(httpMethod.toUpperCase())) {
        throw new Error('Invalid HTTP method');
    }

    // Validate dataId against UUID v4 pattern
    if (!dataId.match(/^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i)) {
        throw new Error('Invalid Data-ID format');
    }

    const host = window.location.protocol + "//" + window.location.host;

    const params = new URLSearchParams();
    params.append('data', dataId);
    const url = `${host}/api/exchange?${params.toString()}`;

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

    // Wrap the ciphertext in an object with the "data" property
    const payload = {
        data: ciphertext
    };

    const requestOptions = {
        method: 'POST',
        body: JSON.stringify(payload),
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
    reset();
    const plaintextData = document.getElementById("plaintextData");
    plaintextData.rows = 10;

    const dataParameter = getParameterByName('data', window.location.search);

    if (dataParameter) {

        const apiResponse = await GetSecret(dataParameter);

        if (apiResponse.status != -1) {
            const plaintextArea = document.getElementById("plaintext-area");
            plaintextArea.style.display = '';
            decryptDataResponse(apiResponse.message);
        }
        else {
            const plaintextData = document.getElementById("plaintextData");
            const revealDataButton = document.getElementById("revealDataButton");
            fail();
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
        const decryptedText = await decrypt(decodeURIComponent(decryptKeyData[0]), decodeURIComponent(decryptKeyData[1]), responseObject.data);
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

    fail();
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

    success();
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