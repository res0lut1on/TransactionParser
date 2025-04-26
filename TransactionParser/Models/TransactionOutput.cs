namespace BsvParser.Models
{
    public class TransactionOutput
    {
        public ulong Value { get; set; }
        public byte[] ScriptPubKey { get; set; } = Array.Empty<byte>();

        public string? Address => Utilities.ScriptPubKeyHelper.ExtractAddress(ScriptPubKey);
    }
}
