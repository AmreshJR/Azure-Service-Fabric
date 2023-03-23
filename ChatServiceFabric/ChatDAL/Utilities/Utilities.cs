using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ChatDAL.Utilities
{
    public class Utilities
    {
        //convert Encrypted data to decrypt data
        //send encrypt data from frontend
        private static readonly string EncKey = "41593A4F1A669FSA";
        public static string EncryptStringAESUsingCSharp(string cipherText)
        {
            try
            {
                var keybytes = Encoding.UTF8.GetBytes(EncKey);
                var iv = Encoding.UTF8.GetBytes(EncKey);
                var decriptedFromJavascript = EncryptStringToBytes(cipherText, keybytes, iv);
                cipherText = Convert.ToBase64String(decriptedFromJavascript);
                string cipher = cipherText.Replace("+", "b1b");
                cipher = cipher.Replace("/", "a1a");



                return cipher;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }


        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;



                rijAlg.Key = key;
                rijAlg.IV = iv;



                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);



                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }



            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        //Decrypt String
        public static string DecryptFrontEndData(string cipherText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText))
                {
                    return string.Empty;

                }
                var keybytes = Encoding.UTF8.GetBytes(EncKey);
                    var iv = Encoding.UTF8.GetBytes(EncKey);
                    string cipher = cipherText.Replace("b1b", "+");
                    cipher = cipher.Replace("a1a", "/");
                    var encrypted = Convert.FromBase64String(cipher);
                    var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
                    return decriptedFromJavascript;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Decrypt(string cipherText)
        {
            string password = EncKey;
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                var salt = cipherBytes.Take(16).ToArray();
                var iv = cipherBytes.Skip(16).Take(16).ToArray();
                var encrypted = cipherBytes.Skip(32).ToArray();
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt, 100);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.Padding = PaddingMode.PKCS7;
                encryptor.Mode = CipherMode.CBC;
                encryptor.IV = iv;
                using (MemoryStream ms = new MemoryStream(encrypted))
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (var reader = new StreamReader(cs, Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncKey);
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }


        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

        public static string CreateRandomPassword(int PasswordLength)
        {
            try
            {
                string _allowedChars = "0123456789@#*$%&abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
                string _capsAlphChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
                string _smallAlphChars = "abcdefghijkmnopqrstuvwxyz";
                string _intChars = "0123456789";
                string _specialChars = "@#*$%&";
                Random randNum = new Random();
                char[] chars = new char[PasswordLength];
                int allowedCharCount = _allowedChars.Length;
                for (int i = 0; i < PasswordLength - 4; i++)
                {
                    chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
                }
                chars[PasswordLength - 4] = _capsAlphChars[(int)((_capsAlphChars.Length) * randNum.NextDouble())];
                chars[PasswordLength - 3] = _smallAlphChars[(int)((_smallAlphChars.Length) * randNum.NextDouble())];
                chars[PasswordLength - 2] = _intChars[(int)((_intChars.Length) * randNum.NextDouble())];
                chars[PasswordLength - 1] = _specialChars[(int)((_specialChars.Length) * randNum.NextDouble())];
                return new string(chars);
            }
            catch (Exception exp)
            {
                
                return null;
            }
        }
/*        public static DateTime ConvertDateAsDateTimeFormat(string date, bool inputIsTFormatDate)
        {
            DateTime result = new DateTime();
            try
            {
                if (inputIsTFormatDate && !string.IsNullOrEmpty(date))
                {
                    var split = date.Split('-');
                    if (split.Length > 0)
                    {
                        var year = int.Parse(split[0]);
                        var month = int.Parse(split[1]);
                        var day = 0;
                        if (split[2].Split('T').Length != 0 && split[2].Split('T').Length != 0)
                        {
                            day = int.Parse(split[2].Split('T')[0]);
                        }



                        DateTime dateVal = new DateTime(year, month, day);
                        //result = month + "/" + day + "/" + year;
                        result = dateVal;
                        return result;
                    }
                }
                else if (!inputIsTFormatDate && !string.IsNullOrEmpty(date))
                {
                    var split = date.Split('-');
                    if (split.Length > 0)
                    {
                        var year = int.Parse(split[2].Split(' ')[0]);
                        var day = int.Parse(split[0]);
                        var month = int.Parse(split[1]);
                        DateTime dateVal = new DateTime(year, month, day); ;
                        result = dateVal;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        public static string ConvertDateAsDateTimeFormat(string date)
        {
            
            try
            {
                string strdate="";
               
                    var split = date.Split('-');
                    if (split.Length > 0)
                    {
                        var year = int.Parse(split[2].Split(' ')[0]);
                        var day = int.Parse(split[0]);
                        var month = int.Parse(split[1]);
                        //DateTime dateVal = new DateTime(year, month, day); ;
                        
                        strdate = string.Concat(month,"-",day,"-",year);
                    }
               
                return strdate;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ConvertDateAsTFormat(string inputDate)
        {
            string result = inputDate;
            try
            {
                var splitChar = "/";
                if (!string.IsNullOrEmpty(inputDate))
                {
                    if (!inputDate.Contains("/"))
                    {
                        splitChar = "-";
                    }
                    var split = inputDate.Split (Convert.ToChar( splitChar));
                    if (split.Length > 0)
                    {
                        var year = split[2];
                        var date = split[0];
                        var month = split[1];
                        if (date.Length == 1)
                        {
                            date = "0" + date;
                        }
                        if (month.Length == 1)
                        {
                            month = "0" + month;
                        }
                        result = date + "/" + month + "/" + year;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
*/

    }
}
