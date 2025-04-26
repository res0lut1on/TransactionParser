using BsvParser.Models;
using BsvParser.Utilities;
using System.Security.Cryptography;

namespace BsvParser.Services
{
    public class BsvTransactionParser
    {
        private readonly byte[] _data;
        private int _position;

        public BsvTransactionParser(string hex)
        {
            _data = ConvertHexToBytes(hex);
            _position = 0;
        }

        public Transaction Parse()
        {
            int startPosition = _position;

            var tx = new Transaction
            {
                Version = ReadUInt32()
            };

            var inputCount = ReadVarInt();
            for (int i = 0; i < (int)inputCount; i++)
            {
                tx.Inputs.Add(ParseInput());
            }

            var outputCount = ReadVarInt();
            for (int i = 0; i < (int)outputCount; i++)
            {
                tx.Outputs.Add(ParseOutput());
            }

            var txData = _data.AsSpan(startPosition, _position - startPosition).ToArray();
            tx.TransactionId = CalculateTransactionId(txData);

            return tx;
        }

        private TransactionInput ParseInput()
        {
            var prevTxId = ReadBytes(32).Reverse().ToArray();
            var outputIndex = ReadUInt32();
            var scriptLength = ReadVarInt();
            var scriptSig = ReadBytes((int)scriptLength);
            var sequence = ReadUInt32();

            return new TransactionInput
            {
                PreviousTransactionId = BitConverter.ToString(prevTxId).Replace("-", "").ToLower(),
                OutputIndex = outputIndex,
                ScriptSig = scriptSig,
                Sequence = sequence
            };
        }

        private TransactionOutput ParseOutput()
        {
            var value = ReadUInt64();
            var scriptLength = ReadVarInt();
            var scriptPubKey = ReadBytes((int)scriptLength);

            return new TransactionOutput
            {
                Value = value,
                ScriptPubKey = scriptPubKey
            };
        }

        private uint ReadUInt32()
        {
            EnsureAvailable(4);
            var value = BitConverter.ToUInt32(_data, _position);
            _position += 4;
            return value;
        }

        private ulong ReadUInt64()
        {
            EnsureAvailable(8);
            var value = BitConverter.ToUInt64(_data, _position);
            _position += 8;
            return value;
        }

        private ushort ReadUInt16()
        {
            EnsureAvailable(2);
            var value = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            return value;
        }

        private byte[] ReadBytes(int length)
        {
            EnsureAvailable(length);
            var result = _data.AsSpan(_position, length).ToArray();
            _position += length;
            return result;
        }

        private ulong ReadVarInt()
        {
            EnsureAvailable(1);
            byte prefix = _data[_position++];

            return (ulong)prefix switch
            {
                < 0xfd => prefix,
                0xfd => ReadUInt16(),
                0xfe => ReadUInt32(),
                0xff => ReadUInt64(),
                _ => throw new InvalidOperationException("Invalid VarInt prefix")
            };
        }

        private static byte[] ConvertHexToBytes(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Invalid hex string length");

            return Enumerable.Range(0, hex.Length / 2)
                .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
                .ToArray();
        }

        private void EnsureAvailable(int length)
        {
            if (_position + length > _data.Length)
                throw new IndexOutOfRangeException("Unexpected end of transaction data");
        }

        private static string CalculateTransactionId(byte[] txData)
        {
            using var sha256 = SHA256.Create();
            var first = sha256.ComputeHash(txData);
            var second = sha256.ComputeHash(first);
            return BitConverter.ToString(second.Reverse().ToArray()).Replace("-", "").ToLower();
        }
    }
}
