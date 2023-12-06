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

            //permuate word
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
            //permuate key
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
            List<int> keyCompression = new List<int>
                {
                    14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4,
                    26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40,
                    51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
                };
            List<int> Etable = new List<int>
                {
                    32, 1, 2, 3, 4, 5,
                    4, 5, 6, 7, 8, 9,
                    8, 9, 10, 11, 12, 13,
                    12, 13, 14, 15, 16, 17,
                    16, 17, 18, 19, 20, 21,
                    20, 21, 22, 23, 24, 25,
                    24, 25, 26, 27, 28, 29,
                    28, 29, 30, 31, 32, 1
                };

            for (int i = 0; i < 16; i++)
            {
                //split key
                string lKey = strKey.Substring(0, 28);
                string rKey = strKey.Substring(32, 28);
                //shift bytes
                lKey = ShiftBytes(lKey, RoundShift.GetValueOrDefault(i));
                rKey = ShiftBytes(rKey, RoundShift.GetValueOrDefault(i));

                string fullKey = lKey + rKey;
                //compress key
                string compressedKey = string.Empty;
                foreach (var perm in keyCompression)
                    compressedKey += fullKey[perm - 1];

                string L0 = permutatedData.Substring(0, 32);
                string R0 = permutatedData.Substring(32, 32);

                //work with R
                string handledR = string.Empty;
                foreach (var perm in Etable)
                    handledR += R0[perm - 1];

                string afterXOR = string.Empty;
                for (int j = 0; i < 48; i++)
                {
                    afterXOR += bool.Parse(handledR[j].ToString()) ^ bool.Parse(compressedKey[j].ToString());
                }

            }








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
