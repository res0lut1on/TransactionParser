namespace BsvParser.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; } = string.Empty;
        public uint Version { get; set; }
        public List<TransactionInput> Inputs { get; set; } = new();
        public List<TransactionOutput> Outputs { get; set; } = new();
    }
}
