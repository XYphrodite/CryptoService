using CryptoService.Service;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CryptoService
{
    public partial class MainForm : Form
    {
        private CryptoWorker cryptoWorker = new CryptoWorker(Encoding.UTF8);


        public MainForm()
        {
            InitializeComponent();
        }

        private void encryptBtnClick(object sender, EventArgs e)//enc
        {
            var key = GenerateKey();
            keyRTB.Text = Convert.ToBase64String(key) + Environment.NewLine;
            //encrypt
            var encrypted = cryptoWorker.des.Crypt(initTextRTB.Text, key, CryptoWorker.DES.Mode.Encryptor);
            //set text
            ciphroTextRTB.Text = Convert.ToBase64String(encrypted.ToArray());
        }

        private void decryptBtnClick(object sender, EventArgs e)
        {
            var key = ReadKey();
            var decrypted = cryptoWorker.des.Crypt(ciphroTextRTB.Text, key, CryptoWorker.DES.Mode.Decryptor);
            decryptedTextRTB.Text = cryptoWorker.des.GetText(decrypted.ToArray());
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

