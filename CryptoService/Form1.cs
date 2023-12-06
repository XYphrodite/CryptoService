using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CryptoService
{
    public partial class MainForm : Form
    {
        //byte[] key = new byte[16];
        //byte[] iv = new byte[16];

        public MainForm()
        {
            InitializeComponent();
        }

        private void encryptBtnClick(object sender, EventArgs e)//enc
        {
            //var toEncrypt = richTextBox1.Text;
            //using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            //{
            //    rng.GetBytes(key);
            //    rng.GetBytes(iv);
            //}
            //byte[] encBytes = Encrypt(toEncrypt, key, iv);
            //string enc = Convert.ToBase64String(encBytes);
            //richTextBox2.Text = enc;
            //string dec = Decrypt(encBytes, key, iv);
            //richTextBox3.Text = dec;
            string initText;
        }

        private void decryptBtnClick(object sender, EventArgs e)//dec
        {
            //var enc = Encoding.ASCII.GetBytes(richTextBox2.Text);
            //string dec = Decrypt(enc, key, iv);
            //richTextBox3.Text = dec;

        }



        //byte[] Encrypt(string simpletext, byte[] key, byte[] iv)
        //{
        //    byte[] cipheredtext;
        //    using (Aes aes = Aes.Create())
        //    {
        //        ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
        //                {
        //                    streamWriter.Write(simpletext);
        //                }

        //                cipheredtext = memoryStream.ToArray();
        //            }
        //        }
        //    }
        //    return cipheredtext;
        //}

        //string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv)
        //{
        //    string simpletext = string.Empty;
        //    using (Aes aes = Aes.Create())
        //    {
        //        ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
        //        using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader streamReader = new StreamReader(cryptoStream))
        //                {
        //                    simpletext = streamReader.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //    return simpletext;
        //}
    }
}
