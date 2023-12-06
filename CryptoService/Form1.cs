using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CryptoService
{
    public partial class MainForm : Form
    {
        byte[] key = new byte[8];

        public MainForm()
        {
            InitializeComponent();
        }

        private void encryptBtnClick(object sender, EventArgs e)//enc
        {
            //get text
            string initText = initTextRTB.Text;

            var words = SplitTextTo8BytesWord(initText);
            //var toEnc = MergeLists(words);
            //Get random key
            Random r = new Random();
            r.NextBytes(key);
            foreach (var word in words)
            {
                DESCrypt(word, key);
            }






            List<List<byte>> SplitTextTo8BytesWord(string text)
            {
                //get encodeing
                ASCIIEncoding encoding = new ASCIIEncoding();
                //text to bytes
                var bytes = encoding.GetBytes(initText).ToList();
                //split text to 8bytes words
                List<List<byte>> wordsToEnc = new List<List<byte>>();
                //splitting
                var word = new List<byte>();
                for (int i = 0, w = 0; (w * 8 + i) < bytes.Count;)
                {
                    if (i == 8)
                    {
                        wordsToEnc.Add(word);
                        word = new List<byte>();
                        w += 1;
                        i = 0;
                    }
                    else
                    {
                        word.Add(bytes[w * 8 + i]);
                        i++;
                    }
                }
                //filling last word
                if (word.Count != 0 && word.Count < 8)
                {
                    while (word.Count < 8)
                    {
                        word.Add(0);
                    }
                    wordsToEnc.Add(word);
                }

                return wordsToEnc;
            }
            List<byte> MergeLists(List<List<byte>> lists)
            {
                List<byte> list = new List<byte>();
                foreach (var l in lists)
                {
                    foreach (var b in l)
                    {
                        list.Add(b);
                    }
                }
                return list;
            }

        }


        private void decryptBtnClick(object sender, EventArgs e)//dec
        {
        }

        void DESCrypt(List<byte> word, byte[] key)
        {
            List<byte> permutationTable = new List<byte>
                {
                    58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
                    62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
                    57, 49, 41, 33, 25, 17, 9, 1,  59, 51, 43, 35, 27, 19, 11, 3,
                    61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7
                };


            string strWord = ListOfBytesToString(word);
            var permutatedData = string.Empty;
            foreach (var perm in permutationTable)
                permutatedData += strWord[perm - 1].ToString();

            List<byte> forKey = new List<byte>
                {
                    57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18,
                    10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
                    63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22,
                    14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4
                };

            string strKey = ListOfBytesToString(key.ToList());
            var permutatedKey = string.Empty;
            foreach (var perm in forKey)
                permutatedKey += strKey[perm - 1].ToString();

            Dictionary<int, int> RoundShift = new Dictionary<int, int>
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

            for (int i = 0; i < 16; i++)
            {
                string lKey = strKey.Substring(0, 28);
                string rKey = strKey.Substring(32, 28);

            }


            string L0 = permutatedData.Substring(0, 32);
            string R0 = permutatedData.Substring(32, 32);





            string ListOfBytesToString(List<byte> data)
            {
                string strData = string.Empty;
                foreach (var d in data)
                    strData += FillToEight(Convert.ToString(d, 2));
                return strData;
            }
            string FillToEight(string v)
            {
                while (v.Length < 8)
                    v = "0" + v;
                return v;
            }

            string ShiftBytes(string str, int amount) 
                => str.Substring(amount, str.Length - amount) + str.Substring(str.Length - amount, amount);
        }
    }
}
