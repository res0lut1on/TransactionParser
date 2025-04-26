using System;
using Xunit;
using BsvParser;
using BsvParser.Services;

namespace BsvParserTests
{
    public class BsvTransactionParserTests
    {
        [Fact]
        public void Should_Parse_Transaction_Correctly()
        {
            string hexTransaction = "0100000002a0f60c41fea58a2a7524c4259b81ec124c083ffe585886c77f0266647f7bb569220000006a47304402203ea090f01766dc60e720cca1334e1c4ef649ec4f4b8a379e3d82e6276bb1d376022076240da2d8f8bd622c90d32f7400bd1670995c1423ebfa1766ce46f7980c8f1641210391d37e2a3d6f6098c3ed487b839b5667dc279b43eadfba17000c00f384bd513cffffffffa0f60c41fea58a2a7524c4259b81ec124c083ffe585886c77f0266647f7bb569230000006b483045022100959e8f586141635158e4eb4f60d502a7e58b608e0105e3fd3034e9f05d69a2c1022050c15495e12fc23fd5096961426f580e0001e6a48bd2f954522091066e70658d41210391d37e2a3d6f6098c3ed487b839b5667dc279b43eadfba17000c00f384bd513cffffffff02cc040000000000001976a9147381c1cfdc3ede7b3d983b9ec809219424754b9f88acf9000000000000001976a914a0f1ce04cfb9d879613e03f1e5902f5caf272dc388ac00000000";

            var parser = new BsvTransactionParser(hexTransaction);

            var transaction = parser.Parse();

            Assert.Equal(2, transaction.Inputs.Count);
            
            Assert.Equal("69b57b7f6466027fc7865858fe3f084c12ec819b25c424752a8aa5fe410cf6a0", transaction.Inputs[0].PreviousTransactionId);
            Assert.Equal((uint)34, transaction.Inputs[0].OutputIndex);

            Assert.Equal("69b57b7f6466027fc7865858fe3f084c12ec819b25c424752a8aa5fe410cf6a0", transaction.Inputs[1].PreviousTransactionId);
            Assert.Equal((uint)35, transaction.Inputs[1].OutputIndex);

            Assert.Equal(2, transaction.Outputs.Count);

            Assert.Equal(1228UL, transaction.Outputs[0].Value);
            Assert.Equal("12FAjcoRpwbJm3PwCDMpSUYcQ8tE2X1z9Xm1", transaction.Outputs[0].Address);

            Assert.Equal(249UL, transaction.Outputs[1].Value);
            Assert.Equal("1puwwiun1kJvxud8MybooaBsnDw9FkdqyEF", transaction.Outputs[1].Address);
        }
    }
}
