# ECC HSM Encryptor

> Application is in **alpha** state!

## Intro

A small application to encrypt and decrypt files with a Nitrokey HSM.

The proof of concept is working! You need to install [OpenSC](https://github.com/OpenSC/OpenSC/wiki) and a [NitroKey HSM](https://shop.nitrokey.com/shop/product/nitrokey-hsm-7).

The actual state is more or less a minimum viable product (MVP), it has a few limitations and the most dialogs are not implemented.

The Crypto-Stuff is here: https://github.com/dhcgn/Encryption-Suite

## Sample

[![Screencast](http://i.imgur.com/9GkkKBZ.png)](https://vimeo.com/229488098)

### Limitations

The following ECDSA GF(p) 192-320 bit elliptic curves are supported by the HSM, but only brainpoolP320r1 is at the moment implemented.

- secp192r1 (prime192v1)
- secp256r1 (prime256v1)
- brainpoolP192r1
- brainpoolP224r1
- brainpoolP256r1
- brainpoolP320r1 **Implemented!**
- secp192k1
- secp256k1

## Features

- Encryption and decryption of files with one or more elliptic curves
- Decryption is only possible with a NitroKey HSM **private key can not leave HSM at any time**  
  (No software-based elliptic curve handling is implemented)
- Encryption with **AES-256**, ENCRYPT-THAN-MAC (HMAC SHA512), Derived Secret hashed with SHA 512 against weak bits

## Roadmap

- Reach beta stadium, see issues 
  - https://github.com/dhcgn/Encryption-Suite/issues
  - https://github.com/dhcgn/ECC-HSM-Encryptor/issues

## Screenshots

![HSM Dialog](http://i.imgur.com/KFJq7Zs.png)
