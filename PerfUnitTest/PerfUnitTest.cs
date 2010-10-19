using System.IO;
using NUnit.Framework;
using OpenFAST;
using OpenFAST.Codec;
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using OpenFAST.Template.Type;

namespace PerfUnitTest
{
    [TestFixture]
    internal class PerfUnitTest
    {
        private static Context CreateContext()
        {
            var ctx = new Context();

            var zero = new IntegerValue(0);
            var emptyStr = new StringValue("");

            ctx.RegisterTemplate(
                'Q',
                new MessageTemplate(
                    "Quote",
                    new[]
                        {
                            new Scalar("SYMBOL", FASTType.STRING, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("TIME_STAMP", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("BID_EXCH_Q", FASTType.STRING, Operator.COPY, emptyStr, false),
                            new Scalar("SEQ_NUM", FASTType.U32, Operator.INCREMENT, zero, false),
                            new Scalar("BID_Q", FASTType.I32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("BID_SIZE_Q", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("DATE_STAMP", FASTType.STRING, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("SCALE", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("ITEM_TYPE", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("CONDITION", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("ASK_EXCH_Q", FASTType.STRING, Operator.COPY, emptyStr, false),
                            new Scalar("ASK_Q", FASTType.I32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("ASK_SIZE_Q", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                        }));

            ctx.RegisterTemplate(
                'T',
                new MessageTemplate(
                    "Trade",
                    new[]
                        {
                            new Scalar("SYMBOL", FASTType.STRING, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("TIME_STAMP", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("EXCH", FASTType.STRING, Operator.COPY, emptyStr, false),
                            new Scalar("SEQ_NUM", FASTType.U32, Operator.INCREMENT, zero, false),
                            new Scalar("PRICE", FASTType.I32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("VOLUME", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("DATE_STAMP", FASTType.STRING, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("SCALE", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("ITEM_TYPE", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("CONDITION", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                        }));

            ctx.RegisterTemplate(
                'B',
                new MessageTemplate(
                    "Bid",
                    new[]
                        {
                            new Scalar("SYMBOL", FASTType.STRING, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("TIME_STAMP", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("EXCH", FASTType.STRING, Operator.COPY, emptyStr, false),
                            new Scalar("SEQ_NUM", FASTType.U32, Operator.INCREMENT, zero, false),
                            new Scalar("PRICE_B", FASTType.I32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("VOLUME_B", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("DATE_STAMP", FASTType.STRING, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("SCALE", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("ITEM_TYPE", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("CONDITION", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                        }));
            ctx.RegisterTemplate(
                'A',
                new MessageTemplate(
                    "Ask",
                    new[]
                        {
                            new Scalar("SYMBOL", FASTType.STRING, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("TIME_STAMP", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("EXCH", FASTType.STRING, Operator.COPY, emptyStr, false),
                            new Scalar("SEQ_NUM", FASTType.U32, Operator.INCREMENT, zero, false),
                            new Scalar("PRICE_A", FASTType.I32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("VOLUME_A", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("DATE_STAMP", FASTType.STRING, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("SCALE", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("ITEM_TYPE", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                            new Scalar("CONDITION", FASTType.U32, Operator.COPY, ScalarValue.Undefined, false),
                        }));
            return ctx;
        }

        private const string LongFile = @"c:\projects\OpenSource\OpenFastNet\PerfUnitTest\test2.fast";

        [Test]
        public void TestPerf()
        {
            using (FileStream stream = File.Open(LongFile, FileMode.Open, FileAccess.Read))
            {
                var decoder = new FastDecoder(CreateContext(), stream);

                foreach (Message msg in decoder)
                {
                    var type = (char) msg.GetInt(0);

                    string symbol = msg.GetString(1);
                    int t = msg.GetInt(2);
                    string exch = msg.GetString(3);
                    var seqNum = (uint) msg.GetInt(4);
                    int v5 = msg.GetInt(5);
                    var v6 = (uint) msg.GetInt(6);
                    int dt = msg.GetInt(7);
                    int v8 = msg.GetInt(8);
                    var itemType = (uint) msg.GetInt(9);
                    int v10 = msg.GetInt(10);

                    if (type == 'Q')
                    {
                        string v11 = msg.GetString(11);
                        int v12 = msg.GetInt(12);
                        var v13 = (uint) msg.GetInt(13);
                    }
                }
            }
        }
    }
}