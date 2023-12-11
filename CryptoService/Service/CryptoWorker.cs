using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace CryptoService.Service
{
    public partial class CryptWorker
    {
        public DES des;
        public AES aes;
        public CryptWorker(Encoding encoding)
        {
            des = new DES(encoding);
            aes = new AES();
        }
        public string Encrypt(string text)
        {
            //KEY
            var key = new Key();
            string encKey = EncryptKey(key);
            //DES
            var desEncryptedBytes = des.Crypt(text, key.desKey, DES.Mode.Encryptor).ToArray();
            var desEncryptedBase64 = Convert.ToBase64String(desEncryptedBytes);
            //AES
            var aesEncryptedBytes = aes.EncryptAes(desEncryptedBase64, key.aesKey, key.ivKey);
            var aesEncryptedBase64 = Convert.ToBase64String(aesEncryptedBytes);

            return aesEncryptedBase64+"к"+encKey;
        }
        public string Decrypt(string text)
        {
            var textAndKey = text.Split('к');
            if(textAndKey.Length == 2 )
            {
                string encData = textAndKey[0];
                string encKey = textAndKey[1];
                var key = DecryptKey(encKey);
                byte[] encDataBytes = Convert.FromBase64String(encData);
                var aesDecrypted = aes.DecryptAes(encDataBytes, key.aesKey, key.ivKey);
                var desDecryptedArray = des.Crypt(aesDecrypted, key.desKey, DES.Mode.Decryptor);
                return des.GetText(desDecryptedArray);
            }
            else
            {
                throw new Exception("Decryption error!!");
            }
        }

        private string EncryptKey(Key key)
        {
            List<byte> allKeys = [.. key.desKey, .. key.aesKey, .. key.ivKey];
            for(int i=0; i<allKeys.Count; i++)
            {
                allKeys[i] += key.keyD;
            }
            allKeys.Add(key.keyD);
            return Convert.ToBase64String(allKeys.ToArray());
        }
        private Key DecryptKey(string enckey)
        {
            var encKeyArr = Convert.FromBase64String(enckey);
            for(int i = 0; i < encKeyArr.Count() - 1; i++)
            {
                encKeyArr[i] -= encKeyArr[encKeyArr.Count() - 1];
            }
            Key key = new Key
            {
                desKey = encKeyArr.Take(8).ToArray(),
                aesKey = encKeyArr.Skip(8).Take(16).ToArray(),
                ivKey = encKeyArr.Skip(24).Take(16).ToArray(),
                keyD = encKeyArr.Skip(40).Take(1).FirstOrDefault()
            };
            return key;
        }

        public class Key
        {
            public byte[] desKey { get; set; } = new byte[8];
            public byte[] aesKey { get; set; } = new byte[16];
            public byte[] ivKey { get; set; } = new byte[16];
            public byte keyD { get; set; }


            public Key()
            {
                Random random = new Random();
                random.NextBytes(desKey);
                random.NextBytes(aesKey);
                random.NextBytes(ivKey);
                keyD = (byte)random.Next(255);
            }

        }

    }

}
