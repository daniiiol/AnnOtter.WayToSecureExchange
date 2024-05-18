# CLI Tool

Depending on your needs, you can find a CLI tool at the following link, which locally encrypts a desired secret message, uploads it to a Secret Exchange Server, and generates a correct URL:

- [ao-w2se.py](../src/cli-tool/ao-w2se.py)

## Prerequisites
- Python 3.6 or higher
- Python module: pycryptodome

### Install pyryptodome

```bash
pip install pyryptodome
```

## Usage CLI Tool

Below a sample expression of the CLI:
```bash
python3 .\ao-w2se.py --server 'https://secret-exchange.<mydomain.com>' --text 'Hello World!'
```

Use option `-h` for further help and an overview of the available parameters.