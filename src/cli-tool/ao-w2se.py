from Crypto.Cipher import AES
from Crypto.Random import get_random_bytes
import urllib.parse
import base64
import requests
import argparse

class bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

def encrypt_aes_gcm(plaintext):
    # Generate a 256-bit key (32 bytes)
    key = get_random_bytes(32)
    
    # Generate a random IV (initialization vector)
    iv = get_random_bytes(12)  # 12 Bytes für GCM-Modus
    
    # Create the AES-GCM cipher object
    cipher = AES.new(key, AES.MODE_GCM, nonce=iv)
    
    # Encrypt the plaintext
    ciphertext, tag = cipher.encrypt_and_digest(plaintext.encode('utf-8'))
    
    # Base64 encoding of the key, IV and the encrypted text (+ Tag, to be compatible with crypto subtl)
    b64_key = base64.b64encode(key).decode('utf-8')
    b64_iv = base64.b64encode(iv).decode('utf-8')
    b64_ciphertext = base64.b64encode(ciphertext + tag).decode('utf-8')
    b64_tag = base64.b64encode(tag).decode('utf-8')
    
    return b64_key, b64_iv, b64_ciphertext, b64_tag

def send_post_request(url, data):
    headers = {'Content-Type': 'application/json'}
    payload = {'data': data}
    response = requests.post(url, json=payload, headers=headers)
    return response

if __name__ == "__main__":
    # App Start
    parser = argparse.ArgumentParser(
        description=fr'''
        {bcolors.BOLD}
        {bcolors.OKCYAN}
 _____                            _____         _                            
/  ___|                          |  ___|       | |                           
\ `--.  ___  ___ _   _ _ __ ___  | |____  _____| |__   __ _ _ __   __ _  ___ 
 `--. \/ _ \/ __| | | | '__/ _ \ |  __\ \/ / __| '_ \ / _` | '_ \ / _` |/ _ \
/\__/ /  __/ (__| |_| | | |  __/ | |___>  < (__| | | | (_| | | | | (_| |  __/
\____/ \___|\___|\__,_|_|  \___| \____/_/\_\___|_| |_|\__,_|_| |_|\__, |\___|
                                                                   __/ |     
                                                                  |___/      
 _____  _     _____   _____           _                                      
/  __ \| |   |_   _| |_   _|         | |                                     
| /  \/| |     | |     | | ___   ___ | |                                     
| |    | |     | |     | |/ _ \ / _ \| |                                     
| \__/\| |_____| |_    | | (_) | (_) | |                                     
 \____/\_____/\___/    \_/\___/ \___/|_|        
        
CLI tool to encrypt a secret text locally, upload it to Secret Exchange and generate a URL for the recipient.
----
Project: Ann Otter - Way to Secure Exchange
Created by daniiiol
GitHub: https://github.com/daniiiol/AnnOtter.WayToSecureExchange
{bcolors.ENDC}
{bcolors.ENDC}
        ''',
        formatter_class=argparse.RawTextHelpFormatter
    )

    parser.add_argument('--text', required=True, type=str, help='The secret text to be encrypted')
    parser.add_argument('--server', required=True, type=str, help='The URL of the Secret Exchange server')
    parser.add_argument('--urlOnly', action='store_true', help='Only the URL is returned')

    args = parser.parse_args()
    
    plaintext = args.text
    server = args.server.rstrip('/')
    urlOnly = args.urlOnly
    
    b64_key, b64_iv, b64_ciphertext, b64_tag = encrypt_aes_gcm(plaintext)

    # Send data to the Secret Exchange Server
    data = b64_ciphertext
    response = send_post_request(f"{server}/api/exchange/upload/", data)

    if response.status_code == 200:
        response_json = response.json()
        upload_id = response_json.get('uploadId')
        
        if(urlOnly):
            print(f"{bcolors.OKBLUE}{bcolors.UNDERLINE}{server}/exchange/?data={upload_id}#{urllib.parse.quote_plus(b64_key)}-{urllib.parse.quote_plus(b64_iv)}{bcolors.ENDC}{bcolors.ENDC}")
        else:
            print(f"{bcolors.OKGREEN}→ Your Secret Exchange URL:{bcolors.ENDC} {bcolors.OKBLUE}{bcolors.UNDERLINE}{server}/exchange/?data={upload_id}#{urllib.parse.quote_plus(b64_key)}-{urllib.parse.quote_plus(b64_iv)}{bcolors.ENDC}{bcolors.ENDC}")
    else:
        print(f"{bcolors.FAIL}Error in the request: {response.status_code} - {response.text}{bcolors.ENDC}")
