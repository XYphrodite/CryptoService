using CryptoService.Service;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CryptoService
{
    public partial class MainForm : Form
    {
        List<byte> forKey = new List<byte>
                {
                    57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18,
                    10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
                    63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22,
                    14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4
                };
        Dictionary<byte, byte> RoundShift = new Dictionary<byte, byte>
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 2 },
                    { 5, 2 },
                    { 6, 2 },
                    { 7, 2 },
                    { 8, 2 },
                    { 9, 1 },
                    { 10, 2 },
                    { 11, 2 },
                    { 12, 2 },
                    { 13, 2 },
                    { 14, 2 },
                    { 15, 2 },
                    { 16, 1 }
                };
        List<byte> keyCompression = new List<byte>
                {
                    14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4,
                    26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40,
                    51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
                };

        
        List<byte> PPermuation = new List<byte>
                {
                    16, 7, 20, 21, 29, 12, 28, 17,
                    1, 15, 23, 26, 5, 18, 31, 10,
                    2, 8, 24, 14, 32, 27, 3, 9,
                    19, 13, 30, 6, 22, 11, 4, 25
                };

        private DES_Crypt _des_Crypt = new DES_Crypt(Encoding.UTF8);


        public MainForm()
        {
            InitializeComponent();
        }

        private void encryptBtnClick(object sender, EventArgs e)//enc
        {
            var key = GenerateKey();
            keyRTB.Text = Convert.ToBase64String(key) + Environment.NewLine;
            //encrypt
            var encrypted = _des_Crypt.Crypt(initTextRTB.Text, key, DES_Crypt.Mode.Encryptor);
            //set text
            ciphroTextRTB.Text = Convert.ToBase64String(encrypted.ToArray());
        }

        private void decryptBtnClick(object sender, EventArgs e)
        {
            var key = ReadKey();
            //var decrypted = _des_Crypt.Decrypt(ciphroTextRTB.Text, key, encoding);
            //decryptedTextRTB.Text = Convert.ToBase64String(decrypted.ToArray());
        }

        private byte[] GenerateKey()
        {
            byte[] key = new byte[8];
            //Get random generator
            Random random = new Random();
            //fill key with random bytes
            random.NextBytes(key);
            return key;
        }
        private byte[] ReadKey() => Convert.FromBase64String(keyRTB.Text);
    }
}

