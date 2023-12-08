using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoService.Service
{
    public class DES_Crypt
    {
        string _initialText = string.Empty;
        public IEnumerable<byte> Encrypt(string initialtext, byte[] key, Encoding encoding)
        {
            //check
            if(string.IsNullOrEmpty(initialtext)) return new List<byte>();

            //set text
            _initialText = initialtext;
            //split text to 64bit blocks
            SplitTextTo8BytesWord(out List<List<byte>> words);

            //create container for keys
            ulong[] keys48b = new ulong[16];
            //create L and R
            UInt32 N1, N2;

            key_expansion(join_8bits_to_64bits(key), ref keys48b);

            for (int i = 0; i < words.Count; i++)
            {
                split_64bits_to_32bits(
                    initial_permutation(
                        join_8bits_to_64bits(words, i)
                    ),
                    &N1, &N2
                );
                feistel_cipher(mode, &N1, &N2, keys48b);
                split_64bits_to_8bits(
                    final_permutation(
                        join_32bits_to_64bits(N1, N2)
                    ),
                    (to + i)
                );
            }









            string ShiftBytes(string str, int amount)
                => str.Substring(amount, str.Length - amount) + str.Substring(str.Length - amount, amount);
            char MyXOR(char a, char b)
            {
                bool bA = a == '1' ? true : false;
                bool bB = b == '1' ? true : false;
                bool x = bA ^ bB;
                return x == true ? '1' : '0';
            }

        }

        private void SplitTextTo8BytesWord(out List<List<byte>> words)
        {
            words = new List<List<byte>>();
            //get encodeing
            ASCIIEncoding encoding = new ASCIIEncoding();
            //text to bytes
            var bytes = encoding.GetBytes(_initialText).ToList();
            //split text to 8bytes words
            List<List<byte>> wordsToEnc = new List<List<byte>>();
            //splitting
            var word = new List<byte>();
            if (bytes.Count is 0)
            {
                MessageBox.Show("No text","No text",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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
                    word.Add(0);
                wordsToEnc.Add(word);
            }
            else wordsToEnc.Add(word);
        }
        private void key_expansion(object v, ref ulong[] keys48b)
        {
            throw new NotImplementedException();
        }
        private ulong join_8bits_to_64bits(IEnumerable<byte> arr)
        {
            ulong block64b = 0;
            for (int i = 0; i < arr.Count(); i++)
            {
                block64b = (block64b << 8) | arr.ElementAt(i);
            }
            return block64b;
        }
        string FillToEight(string v)
        {
            while (v.Length < 8)
                v = "0" + v;
            return v;
        }
        string ListOfBytesToString(List<byte> data)
        {
            string strData = string.Empty;
            foreach (var d in data)
                strData += FillToEight(Convert.ToString(d, 2));
            return strData;
        }
        List<byte> BinaryStringToBytes(string bdata)
        {
            var BinaryData = new List<byte>();
            for (int i = 0; i < bdata.Length; i += 8)
            {
                string oneByte = bdata.Substring(i, 8);
                BinaryData.Add(Convert.ToByte(oneByte, 2));
            }
            return BinaryData;
        }
    }
}
