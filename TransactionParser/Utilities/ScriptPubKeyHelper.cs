using System.Numerics;
using System.Security.Cryptography;

namespace BsvParser.Utilities
{
    public static class ScriptPubKeyHelper
    {
        public static string? ExtractAddress(byte[] scriptPubKey)
        {
            if (scriptPubKey.Length == 25 &&
                scriptPubKey[0] == 0x76 &&
                scriptPubKey[1] == 0xa9 &&
                scriptPubKey[2] == 0x14 &&
                scriptPubKey[^2] == 0x88 &&
                scriptPubKey[^1] == 0xac)
            {
                var pubKeyHash = scriptPubKey.Skip(3).Take(20).ToArray();
                return ToBase58Check(pubKeyHash, 0x00);
            }

            return null;
        }

        private static string ToBase58Check(byte[] payload, byte version)
        {
            var data = new byte[payload.Length + 5];
            data[0] = version;
            Buffer.BlockCopy(payload, 0, data, 1, payload.Length);

            var checksum = SHA256Twice(data.AsSpan(0, payload.Length + 1));
            Buffer.BlockCopy(checksum, 0, data, payload.Length + 1, 4);

            return Base58Encode(data);
        }

        private static byte[] SHA256Twice(ReadOnlySpan<byte> data)
        {
            using var sha = SHA256.Create();
            var first = sha.ComputeHash(data.ToArray());
            return sha.ComputeHash(first);
        }

        private static string Base58Encode(byte[] array)
        {
            const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            var intData = new BigInteger(array.Concat(new byte[] { 0 }).ToArray());

            var result = string.Empty;
            while (intData > 0)
            {
                int remainder = (int)(intData % 58);
                intData /= 58;
                result = Alphabet[remainder] + result;
            }

            foreach (var b in array)
            {
                if (b == 0x00)
                    result = '1' + result;
                else
                    break;
            }

            return result;
        }
    }
}
