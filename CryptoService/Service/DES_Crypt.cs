using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CryptoService.Service
{
    public class CryptoWorker
    {
        public DES des;
        public CryptoWorker(Encoding encoding)
        {
            _des = new DES(encoding);
        }
        public class DES
        {
            string _initialText = string.Empty;
            private Encoding _encoding;
            public DES(Encoding encoding)
            {
                _encoding = encoding;
            }
            public IEnumerable<byte> Crypt(string initialtext, byte[] key, Mode mode)
            {
                //check
                if (string.IsNullOrEmpty(initialtext)) return new List<byte>();
                List<List<byte>> words = new();
                switch (mode)
                {
                    case Mode.Encryptor:
                        //set
                        _initialText = initialtext;
                        //split text to 64bit blocks
                        SplitTextTo8BytesWord(out List<List<byte>> w2);
                        words = w2;
                        break;
                    case Mode.Decryptor:
                        var w1 = Convert.FromBase64String(initialtext);
                        for (var i = 0; i < w1.Count(); i += 8)
                        {
                            var word = new List<byte> { w1[i], w1[i + 1], w1[i + 2], w1[i + 3], w1[i + 4], w1[i + 5], w1[i + 6], w1[i + 7] };
                            words.Add(word);
                        }
                        break;
                    default: return new List<byte>();
                }

                //create container for keys
                List<ulong> keys48b = new List<ulong>(16);
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
                    feistel_cipher(mode, ref N1, ref N2, keys48b);
                    split_64bits_to_8bits(
                        final_permutation(
                            join_32bits_to_64bits(N1, N2)
                        ),
                        words[i]
                    );
                }
                return words.SelectMany(w => w);

            }

            public string GetText(IEnumerable<byte> data) => _encoding.GetString(data.ToArray());

            private void feistel_cipher(Mode mode, ref uint N1, ref uint N2, List<ulong> keys48b)
            {
                switch (mode)
                {
                    case Mode.Encryptor:
                        {
                            for (byte round = 0; round < 16; ++round)
                                round_feistel_cipher(ref N1, ref N2, keys48b[round]);
                            swap(ref N1, ref N2);
                            break;
                        }
                    case Mode.Decryptor:
                        {
                            for (sbyte round = 15; round >= 0; round--)
                                round_feistel_cipher(ref N1, ref N2, keys48b[round]);
                            swap(ref N1, ref N2);
                            break;
                        }
                }



                void swap(ref uint N1, ref uint N2)
                {
                    uint temp = N1;
                    N1 = N2;
                    N2 = temp;
                }
            }

            private void round_feistel_cipher(ref uint N1, ref uint N2, ulong key48b)
            {
                uint temp = N2;
                N2 = func_F(N2, key48b) ^ N1;
                N1 = temp;

                uint func_F(uint block32b, ulong key48b)
                {
                    ulong block48b = expansion_permutation(block32b);
                    block48b ^= key48b;
                    block32b = substitutions(block48b);
                    return permutation(block32b);


                    uint permutation(uint block32b)
                    {
                        uint new_block32b = 0;
                        for (byte i = 0; i < 32; ++i)
                            new_block32b |= ((block32b >> (32 - __P[i])) & 0x01) << (31 - i);
                        return new_block32b;
                    }
                }
            }


            uint substitutions(ulong block48b)
            {
                List<byte> blocks4b = new List<byte>(), blocks6b = new List<byte>();
                split_48bits_to_6bits(block48b, ref blocks6b);
                substitution_6bits_to_4bits(blocks6b, ref blocks4b);
                return join_4bits_to_32bits(blocks4b);

                void split_48bits_to_6bits(ulong block48b, ref List<byte> blocks6b)
                {
                    for (byte i = 0; i < 8; ++i)
                        blocks6b.Add((byte)((block48b >> (58 - (i * 6))) << 2));
                }

                void substitution_6bits_to_4bits(List<byte> blocks6b, ref List<byte> blocks4b)
                {
                    byte block2b, block4b;

                    for (byte i = 0, j = 0; i < 8; i += 2, ++j)
                    {
                        block2b = extreme_bits(blocks6b.ElementAt(i));
                        block4b = middle_bits(blocks6b.ElementAt(i));
                        blocks4b.Add(__Sbox[i][block2b][block4b]);

                        block2b = extreme_bits(blocks6b.ElementAt(i + 1));
                        block4b = middle_bits(blocks6b.ElementAt(i + 1));
                        blocks4b.Add((byte)((blocks4b.ElementAt(j) << 4) | __Sbox[i + 1][block2b][block4b]));
                    }
                }

                uint join_4bits_to_32bits(IEnumerable<byte> blocks4b)
                {
                    uint block32b = 0;
                    for (byte p = 0; p < 8; ++p)
                        block32b = (block32b << 8) | blocks4b.ElementAt(p);
                    return block32b;
                }
            }



            ulong join_8bits_to_64bits(IEnumerable<byte> blocks8b)
            {
                ulong block64b = 0;
                for (byte p = 0; p < 8; ++p)
                    block64b = (block64b << 8) | blocks8b.ElementAt(p);
                return block64b;
            }


            byte extreme_bits(byte block6b) => (byte)(((block6b >> 6) & 0x2) | ((block6b >> 2) & 0x1));

            byte middle_bits(byte block6b) => (byte)((block6b >> 3) & 0xF);

            ulong expansion_permutation(uint block32b)
            {
                ulong block48b = 0;
                for (byte i = 0; i < 48; ++i)
                    block48b |= (ulong)((block32b >> (32 - __EP[i])) & 0x01) << (63 - i);
                return block48b;
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
            void key_expansion(ulong key64b, ref List<ulong> keys48b)
            {
                uint K1 = 0, K2 = 0;
                key_permutation_56bits_to_28bits(key64b, ref K1, ref K2);
                key_expansion_to_48bits(K1, K2, ref keys48b);
            }

            void key_permutation_56bits_to_28bits(ulong block56b, ref uint block28b_1, ref uint block28b_2)
            {
                for (byte i = 0; i < 28; ++i)
                {
                    block28b_1 |= (uint)(((block56b >> (64 - __K1P[i])) & 0x01) << (31 - i));
                    block28b_2 |= (uint)(((block56b >> (64 - __K2P[i])) & 0x01) << (31 - i));
                }
            }

            void key_expansion_to_48bits(uint block28b_1, uint block28b_2, ref List<ulong> keys48b)
            {
                ulong block56b;
                byte n;
                for (byte i = 0; i < 16; ++i)
                {
                    switch (i)
                    {
                        case 0: case 1: case 8: case 15: n = 1; break;
                        default: n = 2; break;
                    }

                    block28b_1 = LSHIFT_28BIT(block28b_1, n);
                    block28b_2 = LSHIFT_28BIT(block28b_2, n);
                    block56b = join_28bits_to_56bits(block28b_1, block28b_2);
                    keys48b.Add(key_contraction_permutation(block56b));
                }


                ulong join_28bits_to_56bits(uint block28b_1, uint block28b_2)
                {
                    ulong block56b;
                    block56b = (block28b_1 >> 4);
                    block56b = ((block56b << 32) | block28b_2) << 4;
                    return block56b;
                }

                uint LSHIFT_28BIT(uint x, byte L) => (uint)((((x) << (L)) | ((x) >> (-(L) & 27))) & (((ulong)1 << 32) - 1));
            }

            ulong key_contraction_permutation(ulong block56b)
            {
                ulong block48b = 0;
                for (byte i = 0; i < 48; ++i)
                {
                    block48b |= ((block56b >> (64 - __CP[i])) & 0x01) << (63 - i);
                }
                return block48b;
            }

            private void SplitTextTo8BytesWord(out List<List<byte>> words)
            {
                words = new List<List<byte>>();
                //text to bytes
                var bytes = _encoding.GetBytes(_initialText).ToList();
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
                        words.Add(word);
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
                    words.Add(word);
                }
                else words.Add(word);
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
            private static readonly byte[] __EP =
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
            private static readonly List<List<List<byte>>> __Sbox = new List<List<List<byte>>> {
    new List<List<byte>> { // 0
        new List<byte>{14, 4 , 13, 1 , 2 , 15, 11, 8 , 3 , 10, 6 , 12, 5 , 9 , 0 , 7 },
        new List<byte>{0 , 15, 7 , 4 , 14, 2 , 13, 1 , 10, 6 , 12, 11, 9 , 5 , 3 , 8 },
        new List<byte>{4 , 1 , 14, 8 , 13, 6 , 2 , 11, 15, 12, 9 , 7 , 3 , 10, 5 , 0 },
        new List<byte>{15, 12, 8 , 2 , 4 , 9 , 1 , 7 , 5 , 11, 3 , 14, 10, 0 , 6 , 13},
    },
    new List<List<byte>>{ // 1
        new List<byte>{15, 1 , 8 , 14, 6 , 11, 3 , 4 , 9 , 7 , 2 , 13, 12, 0 , 5 , 10},
        new List<byte>{3 , 13, 4 , 7 , 15, 2 , 8 , 14, 12, 0 , 1 , 10, 6 , 9 , 11, 5 },
        new List<byte>{0 , 14, 7 , 11, 10, 4 , 13, 1 , 5 , 8 , 12, 6 , 9 , 3 , 2 , 15},
        new List<byte>{13, 8 , 10, 1 , 3 , 15, 4 , 2 , 11, 6 , 7 , 12, 0 , 5 , 14, 9 },
    },
    new List<List<byte>>{ // 2
        new List<byte>{10, 0 , 9 , 14, 6 , 3 , 15, 5 , 1 , 13, 12, 7 , 11, 4 , 2 , 8 },
        new List<byte>{13, 7 , 0 , 9 , 3 , 4 , 6 , 10, 2 , 8 , 5 , 14, 12, 11, 15, 1 },
        new List<byte>{13, 6 , 4 , 9 , 8 , 15, 3 , 0 , 11, 1 , 2 , 12, 5 , 10, 14, 7 },
        new List<byte>{1 , 10, 13, 0 , 6 , 9 , 8 , 7 , 4 , 15, 14, 3 , 11, 5 , 2 , 12},
    },
    new List<List<byte>>{ // 3
        new List<byte>{7 , 13, 14, 3 , 0 , 6 , 9 , 10, 1 , 2 , 8 , 5 , 11, 12, 4 , 15},
        new List<byte>{13, 8 , 11, 5 , 6 , 15, 0 , 3 , 4 , 7 , 2 , 12, 1 , 10, 14, 9 },
        new List<byte>{10, 6 , 9 , 0 , 12, 11, 7 , 13, 15, 1 , 3 , 14, 5 , 2 , 8 , 4 },
        new List<byte>{3 , 15, 0 , 6 , 10, 1 , 13, 8 , 9 , 4 , 5 , 11, 12, 7 , 2 , 14},
    },
    new List<List<byte>>{ // 4
        new List<byte>{2 , 12, 4 , 1 , 7 , 10, 11, 6 , 8 , 5 , 3 , 15, 13, 0 , 14, 9 },
        new List<byte>{14, 11, 2 , 12, 4 , 7 , 13, 1 , 5 , 0 , 15, 10, 3 , 9 , 8 , 6 },
        new List<byte>{4 , 2 , 1 , 11, 10, 13, 7 , 8 , 15, 9 , 12, 5 , 6 , 3 , 0 , 14},
        new List<byte>{11, 8 , 12, 7 , 1 , 14, 2 , 13, 6 , 15, 0 , 9 , 10, 4 , 5 , 3 },
    },
    new List<List<byte>>{ // 5
        new List<byte>{12, 1 , 10, 15, 9 , 2 , 6 , 8 , 0 , 13, 3 , 4 , 14, 7 , 5 , 11},
        new List<byte>{10, 15, 4 , 2 , 7 , 12, 9 , 5 , 6 , 1 , 13, 14, 0 , 11, 3 , 8 },
        new List<byte>{9 , 14, 15, 5 , 2 , 8 , 12, 3 , 7 , 0 , 4 , 10, 1 , 13, 11, 6 },
        new List<byte>{4 , 3 , 2 , 12, 9 , 5 , 15, 10, 11, 14, 1 , 7 , 6 , 0 , 8 , 13},
    },
    new List<List<byte>>{ // 6
        new List<byte>{4 , 11, 2 , 14, 15, 0 , 8 , 13, 3 , 12, 9 , 7 , 5 , 10, 6 , 1 },
        new List<byte>{13, 0 , 11, 7 , 4 , 9 , 1 , 10, 14, 3 , 5 , 12, 2 , 15, 8 , 6 },
        new List<byte>{1 , 4 , 11, 13, 12, 3 , 7 , 14, 10, 15, 6 , 8 , 0 , 5 , 9 , 2 },
        new List<byte>{6 , 11, 13, 8 , 1 , 4 , 10, 7 , 9 , 5 , 0 , 15, 14, 2 , 3 , 12},
    },
    new List<List<byte>>{ // 7
        new List<byte>{13, 2 , 8 , 4 , 6 , 15, 11, 1 , 10, 9 , 3 , 14, 5 , 0 , 12, 7 },
        new List<byte>{1 , 15, 13, 8 , 10, 3 , 7 , 4 , 12, 5 , 6 , 11, 0 , 14, 9 , 2 },
        new List<byte>{7 , 11, 4 , 1 , 9 , 12, 14, 2 , 0 , 6 , 10, 13, 15, 3 , 5 , 8 },
        new List<byte>{2 , 1 , 14, 7 , 4 , 10, 8 , 13, 15, 12, 9 , 0 , 3 , 5 , 6 , 11},
    },
};
            private static readonly byte[] __P = {
            16, 7 , 20, 21, 29, 12, 28, 17, 1 , 15, 23, 26, 5 , 18, 31, 10,
            2 , 8 , 24, 14, 32, 27, 3 , 9 , 19, 13, 30, 6 , 22, 11, 4 , 25,
        };
            private static readonly byte[] __K1P = {
            57, 49, 41, 33, 25, 17, 9 , 1 , 58, 50, 42, 34, 26, 18,
            10, 2 , 59, 51, 43, 35, 27, 19, 11, 3 , 60, 52, 44, 36,
        };
            private static readonly byte[] __K2P = {
            63, 55, 47, 39, 31, 23, 15, 7 , 62, 54, 46, 38, 30, 22,
            14, 6 , 61, 53, 45, 37, 29, 21, 13, 5 , 28, 20, 12, 4 ,
        };
            private static readonly byte[] __CP = {
                14, 17, 11, 24, 1 , 5 , 3 , 28, 15, 6 , 21, 10,
                23, 19, 12, 4 , 26, 8 , 16, 7 , 27, 20, 13, 2 ,
                41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48,
                44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32,
            };
        }
        public enum Mode : byte
        {
            Encryptor,
            Decryptor
        }
    }
}
