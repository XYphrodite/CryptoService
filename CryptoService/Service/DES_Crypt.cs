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
        private Encoding _encoding;
        public DES_Crypt(Encoding encoding)
        {
            _encoding = encoding;
        }
        public IEnumerable<byte> Crypt(string initialtext, byte[] key, Mode mode)
        {
            //check
            if (string.IsNullOrEmpty(initialtext)) return new List<byte>();

            //set
            _initialText = initialtext;
            //split text to 64bit blocks
            SplitTextTo8BytesWord(out List<List<byte>> words);

            //create container for keys
            ulong[] keys48b = new ulong[16];
            //create L and R
            uint N1 = 0, N2 = 0;

            key_expansion(join_8bits_to_64bits(key), ref keys48b);

            for (int i = 0; i < words.Count; i++)
            {
                split_64bits_to_32bits(
                    initial_permutation(
                        join_8bits_to_64bits(words[i])
                    ),
                    ref N1, ref N2
                );
                feistel_cipher(mode, &N1, &N2, keys48b);
                split_64bits_to_8bits(
                    final_permutation(
                        join_32bits_to_64bits(N1, N2)
                    ),
                    words[i]
                );
            }

        }

        private void split_64bits_to_8bits(ulong block64b, IEnumerable<byte> blocks8b)
        {
            for (byte i = 0; i < 8; ++i)
                blocks8b.Append((byte)(block64b >> ((7 - i) * 8)));
        }

        private ulong final_permutation(ulong block64b)
        {
            ulong new_block64b = 0;
            for (byte i = 0; i < 64; ++i)
            {
                new_block64b |= ((block64b >> (64 - __FP[i])) & 0x01) << (63 - i);
            }
            return new_block64b;
        }

        private ulong join_32bits_to_64bits(uint block32b_1, uint block32b_2)
        {
            ulong block64b;
            block64b = (ulong)block32b_1;
            block64b = (ulong)(block64b << 32) | block32b_2;
            return block64b;
        }

        private void split_64bits_to_32bits(ulong block64b, ref uint block32b_1, ref uint block32b_2)
        {
            block32b_1 = (uint)(block64b >> 32);
            block32b_2 = (uint)(block64b);
        }

        private ulong initial_permutation(ulong block64b)
        {
            ulong new_block64b = 0;
            for (byte i = 0; i < 64; ++i)
                new_block64b |= ((block64b >> (64 - __IP[i])) & 0x01) << (63 - i);
            return new_block64b;
        }

        private void SplitTextTo8BytesWord(out List<List<byte>> words)
        {
            words = new List<List<byte>>();
            //text to bytes
            var bytes = _encoding.GetBytes(_initialText).ToList();
            //split text to 8bytes words
            List<List<byte>> wordsToEnc = new List<List<byte>>();
            //splitting
            var word = new List<byte>();
            if (bytes.Count is 0)
            {
                MessageBox.Show("No text", "No text", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private static readonly byte[] __IP =
                {
                    58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
                    62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
                    57, 49, 41, 33, 25, 17, 9, 1,  59, 51, 43, 35, 27, 19, 11, 3,
                    61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7
                };
        private static readonly byte[] __FP = 
                {
                    40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31,
                    38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,
                    36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27,
                    34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9 , 49, 17, 57, 25,
                };

        public enum Mode : byte
        {
            Cryptor,
            Decryptor
        }
    }
}
