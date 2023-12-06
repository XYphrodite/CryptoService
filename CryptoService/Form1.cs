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
            List<List<byte>> S = new List<List<byte>>
            {
                 new List<byte>
                    {
                        14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,
                        0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
                        4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,
                        15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13
                    },
                 new List<byte>
                    {
                        15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,
                        3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
                        0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,
                        13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9
                    },
                 new List<byte>
                    {
                        10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,
                        13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
                        13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,
                        1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12
                    },
                 new List<byte>
                    {
                        7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,
                        13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
                        10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,
                        3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14
                    },
                 new List<byte>
                    {
                        2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,
                        14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
                        4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,
                        11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3
                    },
                 new List<byte>
                    {
                        12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,
                        10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
                        9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,
                        4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13
                    },
                 new List<byte>
                    {
                        4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,
                        13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
                        1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,
                        6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12
                    },
                 new List<byte>
                     {
                        13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,
                        1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
                        7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,
                        2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11
                    }
        };
            List<int> PPermuation = new List<int>
                {
                    16, 7, 20, 21, 29, 12, 28, 17,
                    1, 15, 23, 26, 5, 18, 31, 10,
                    2, 8, 24, 14, 32, 27, 3, 9,
                    19, 13, 30, 6, 22, 11, 4, 25
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
                //xor with key
                string afterXOR = string.Empty;
                for (int j = 0; i < 48; i++)
                    afterXOR += bool.Parse(handledR[j].ToString()) ^ bool.Parse(compressedKey[j].ToString());
                //split to blocks
                List<string> blocks = new List<string>();
                for (int j = 0; j < 8; j++)
                    blocks.Add(afterXOR.Substring(j * 6, 6));

                string resR = string.Empty;
                for (int j = 0; j < blocks.Count; j++)
                {
                    //get row and column number
                    string rowNumStr = blocks[j][0] + blocks[j][5].ToString();
                    string colNumStr = blocks[j].Substring(1, 4);
                    byte rowNum = Convert.ToByte(rowNumStr, 2);
                    byte colNum = Convert.ToByte(colNumStr, 2);
                    resR += FillToEight(Convert.ToString(S[j][rowNum * 16 + colNum], 2));
                }
                //end perm
                string endPerm = string.Empty;
                foreach (var perm in PPermuation)
                    endPerm += resR[perm];
                //xor with l0
                string afterXORwithL = string.Empty;
                for (int j = 0; i < 48; i++)
                    afterXORwithL += bool.Parse(endPerm[j].ToString()) ^ bool.Parse(L0[j].ToString());
                string R1 = afterXORwithL;
                string res = R1 + R0;
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
