namespace BsvParser.Models
{
    public class TransactionInput
    {
        public string PreviousTransactionId { get; set; } = string.Empty;
        public uint OutputIndex { get; set; }
        public byte[] ScriptSig { get; set; } = Array.Empty<byte>();
        public uint Sequence { get; set; }
    }
}
