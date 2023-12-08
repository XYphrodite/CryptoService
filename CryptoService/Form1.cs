using CryptoService.Service;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CryptoService
{
    public partial class MainForm : Form
    {
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
            var decrypted = _des_Crypt.Crypt(ciphroTextRTB.Text, key, DES_Crypt.Mode.Decryptor);
            decryptedTextRTB.Text = _des_Crypt.GetText(decrypted.ToArray());
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

