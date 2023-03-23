var CryptoJS = CryptoJS

function EncryptFieldData(data) {
  var key = CryptoJS.enc.Utf8.parse('41593A4F1A669FSA');

  var iv = CryptoJS.enc.Utf8.parse('41593A4F1A669FSA');

  var encryptedData = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(data), key, {

    keySize: 128 / 8, iv: iv,

    mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7

  });

  var encrypt = encryptedData.toString();

  // encrypt = encrypt.split("+").join("@npt");

  encrypt = encrypt.split("/").join("a1a");

  encrypt = encrypt.split("+").join("b1b");

  //encrypt = encrypt.split("/").join("b2b");

  return encrypt;

}

function FrontEndEncryption(data) {

  try {
    return CryptoJS.AES.encrypt(JSON.stringify(data), '41593A4F1A669FSA').toString();
  } catch (e) {
    console.log(e);
  }
}
function FrontEndEncryption(data) {

  try {
    return CryptoJS.AES.encrypt(JSON.stringify(data), '41593A4F1A669FSA').toString();
  } catch (e) {
    console.log(e);
  }
}
function FrontEndDecryption(data) {
  try {
    if (data) {
      const bytes = CryptoJS.AES.decrypt(data, '41593A4F1A669FSA');
      if (bytes.toString()) {
        return JSON.parse(bytes.toString(CryptoJS.enc.Utf8));
      }
    }
    return data;
  } catch (e) {
    console.log(e);
  }
}
function DecryptBackend(ciphertextB64) {

  var key = CryptoJS.enc.Utf8.parse('41593A4F1A669FSA');
  var iv = CryptoJS.lib.WordArray.create([0x00, 0x00, 0x00, 0x00]);

  var decrypted = CryptoJS.AES.decrypt(ciphertextB64, key, { iv: iv });
  return JSON.parse(decrypted.toString(CryptoJS.enc.Utf8));
}
function FrontEndDecryptionMessage(data) {
  try {
    if (data) {
      const bytes = CryptoJS.AES.decrypt(data, '41593A4F1A669FSA');
      if (bytes.toString()) {
        return JSON.parse(bytes.toString(CryptoJS.enc.Utf8));
      }
    }
    return data;
  } catch (e) {
    console.log(e);
  }

}