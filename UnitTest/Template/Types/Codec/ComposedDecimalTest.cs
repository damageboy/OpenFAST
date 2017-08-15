/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using NUnit.Framework;
using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.UnitTests.Test;
using OpenFAST.Utility;

namespace OpenFAST.UnitTests.Template.Types.Codec
{
    [TestFixture]
    public class ComposedDecimalTest : OpenFastTestCase
    {
        private readonly QName _name = new QName("Value");

        private readonly MessageTemplate _template = new MessageTemplate("", new Field[0]);

        private static BitVectorReader PmapReader(string bits)
        {
            return new BitVectorReader(new BitVector(ByteUtil.ConvertBitStringToFastByteArray(bits)));
        }

        [Test]
        public void TestCopyExponentDefaultMantissa()
        {
            ComposedScalar decimalt = Util.ComposedDecimal(_name, Operator.Copy, ScalarValue.Undefined, Operator.Default,
                                                           new LongValue(1), false);
            var context = new Context();
            var pmapBuilder = new BitVectorBuilder(7);
            TestUtil.AssertBitVectorEquals("11111110 00000001 00010110 10101101",
                                           decimalt.Encode(Decimal(19245, -2), _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("11100000", pmapBuilder.BitVector.Bytes);

            pmapBuilder = new BitVectorBuilder(7);
            TestUtil.AssertBitVectorEquals("00000001 00010110 10101001",
                                           decimalt.Encode(Decimal(19241, -2), _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("10100000", pmapBuilder.BitVector.Bytes);

            pmapBuilder = new BitVectorBuilder(7);
            TestUtil.AssertBitVectorEquals("10000000", decimalt.Encode(Decimal(1, 0), _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("11000000", pmapBuilder.BitVector.Bytes);

            pmapBuilder = new BitVectorBuilder(7);
            Assert.AreEqual("", decimalt.Encode(Decimal(1, 0), _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("10000000", pmapBuilder.BitVector.Bytes);
            Assert.AreEqual(2, pmapBuilder.Index);
        }

        [Test]
        public void TestCopyExponentDeltaMantissa()
        {
            ComposedScalar decimalt = Util.ComposedDecimal(_name, Operator.Copy, ScalarValue.Undefined, Operator.Delta,
                                                           new IntegerValue(1), false);
            var context = new Context();
            var pmapBuilder = new BitVectorBuilder(7);
            TestUtil.AssertBitVectorEquals("11111110 00000001 00010110 10101100",
                                           decimalt.Encode(Decimal(19245, -2), _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("11000000", pmapBuilder.BitVector.Bytes);
            pmapBuilder = new BitVectorBuilder(7);
            TestUtil.AssertBitVectorEquals("11111100",
                                           decimalt.Encode(Decimal(19241, -2), _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("10000000", pmapBuilder.BitVector.Bytes);
        }

        [Test]
        public void TestInitialValues()
        {
            var context = new Context();
            ComposedScalar scalar = Util.ComposedDecimal(_name, Operator.Default, Int(-3), Operator.Delta,
                                                         ScalarValue.Undefined, false);
            TestUtil.AssertBitVectorEquals("00000101 01100000 11110101",
                                           scalar.Encode(Decimal(94325, -3), _template, context, new BitVectorBuilder(7)));
            TestUtil.AssertBitVectorEquals("11100111",
                                           scalar.Encode(Decimal(94300, -3), _template, context, new BitVectorBuilder(7)));
            TestUtil.AssertBitVectorEquals("11100111",
                                           scalar.Encode(Decimal(94275, -3), _template, context, new BitVectorBuilder(7)));
            TestUtil.AssertBitVectorEquals("00000000 11001011",
                                           scalar.Encode(Decimal(94350, -3), _template, context, new BitVectorBuilder(7)));
            TestUtil.AssertBitVectorEquals("10011001",
                                           scalar.Encode(Decimal(94375, -3), _template, context, new BitVectorBuilder(7)));
            TestUtil.AssertBitVectorEquals("10011001",
                                           scalar.Encode(Decimal(94400, -3), _template, context, new BitVectorBuilder(7)));
            TestUtil.AssertBitVectorEquals("01111111 10000011",
                                           scalar.Encode(Decimal(94275, -3), _template, context, new BitVectorBuilder(7)));
            TestUtil.AssertBitVectorEquals("11100111",
                                           scalar.Encode(Decimal(94250, -3), _template, context, new BitVectorBuilder(7)));
            TestUtil.AssertBitVectorEquals("11100111",
                                           scalar.Encode(Decimal(94225, -3), _template, context, new BitVectorBuilder(7)));
            TestUtil.AssertBitVectorEquals("00000000 11111101",
                                           scalar.Encode(Decimal(94350, -3), _template, context, new BitVectorBuilder(7)));

            context = new Context();
            Assert.AreEqual(Decimal(94325, -3),
                            scalar.Decode(BitStream("00000101 01100000 11110101"), _template, context,
                                          PmapReader("10100000")));
            Assert.AreEqual(Decimal(94300, -3),
                            scalar.Decode(BitStream("11100111"), _template, context, PmapReader("10100000")));
            Assert.AreEqual(Decimal(94275, -3),
                            scalar.Decode(BitStream("11100111"), _template, context, PmapReader("10100000")));
            Assert.AreEqual(Decimal(94350, -3),
                            scalar.Decode(BitStream("00000000 11001011"), _template, context, PmapReader("10100000")));
            Assert.AreEqual(Decimal(94375, -3),
                            scalar.Decode(BitStream("10011001"), _template, context, PmapReader("10100000")));
            Assert.AreEqual(Decimal(94400, -3),
                            scalar.Decode(BitStream("10011001"), _template, context, PmapReader("10100000")));
            Assert.AreEqual(Decimal(94275, -3),
                            scalar.Decode(BitStream("01111111 10000011"), _template, context, PmapReader("10100000")));
            Assert.AreEqual(Decimal(94250, -3),
                            scalar.Decode(BitStream("11100111"), _template, context, PmapReader("10100000")));
            Assert.AreEqual(Decimal(94225, -3),
                            scalar.Decode(BitStream("11100111"), _template, context, PmapReader("10100000")));
            Assert.AreEqual(Decimal(94350, -3),
                            scalar.Decode(BitStream("00000000 11111101"), _template, context, PmapReader("10100000")));
        }

        [Test]
        public void TestOptionalConstantExponent()
        {
            ComposedScalar decimalt = Util.ComposedDecimal(_name, Operator.Constant,
                                                           new IntegerValue(-2),
                                                           Operator.Default,
                                                           new LongValue(100), true);
            var context = new Context();
            var pmapBuilder = new BitVectorBuilder(7);

            Assert.AreEqual("", decimalt.Encode(Decimal(100, -2), _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("11000000", pmapBuilder.BitVector.Bytes);
            Assert.AreEqual(2, pmapBuilder.Index);
        }

        [Test]
        public void TestOptionalDefaultNullExponent()
        {
            ComposedScalar decimalt = Util.ComposedDecimal(_name, Operator.Default,
                                                           ScalarValue.Undefined,
                                                           Operator.Delta, new IntegerValue(
                                                                               12200), true);
            var context = new Context();
            var pmapBuilder = new BitVectorBuilder(7);

            Assert.AreEqual("", decimalt.Encode(null, _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("10000000", pmapBuilder.BitVector.Bytes);
            Assert.AreEqual(1, pmapBuilder.Index); // ONLY ONE PMAP BIT SHOULD
            // BE WRITTEN

            pmapBuilder = new BitVectorBuilder(7);
            TestUtil.AssertBitVectorEquals("11111110 10000001",
                                           decimalt.Encode(Decimal(12201, -2), _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("11000000", pmapBuilder.BitVector.Bytes);
            Assert.AreEqual(1, pmapBuilder.Index);
        }

        [Test]
        public void TestOptionalDeltaExponentCopyMantissa()
        {
            ComposedScalar decimalt = Util.ComposedDecimal(_name, Operator.Delta,
                                                           ScalarValue.Undefined,
                                                           Operator.Copy,
                                                           ScalarValue.Undefined, true);
            var context = new Context();
            var pmapBuilder = new BitVectorBuilder(7);

            TestUtil.AssertBitVectorEquals("10000000", decimalt.Encode(null, _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("10000000", pmapBuilder.BitVector.Bytes);
            Assert.AreEqual(0, pmapBuilder.Index);

            pmapBuilder = new BitVectorBuilder(7);
            TestUtil.AssertBitVectorEquals("10000001 10000001",
                                           decimalt.Encode(Decimal(1, 0), _template, context, pmapBuilder));
            TestUtil.AssertBitVectorEquals("11000000", pmapBuilder.BitVector.Bytes);
            Assert.AreEqual(1, pmapBuilder.Index);
        }

        [Test]
        public void TestSimple()
        {
            const string encoding = "11111110 00111001 01000101 10100011";

            ComposedScalar scalar = Util.ComposedDecimal(_name, Operator.Copy,
                                                         ScalarValue.Undefined,
                                                         Operator.Delta,
                                                         ScalarValue.Undefined, true);

            TestUtil.AssertBitVectorEquals(encoding,
                                           scalar.Encode(Decimal(942755, -2), _template, new Context(),
                                                         new BitVectorBuilder(7)));
            Assert.AreEqual(Decimal(942755, -2),
                            scalar.Decode(BitStream(encoding), _template, new Context(), PmapReader("11000000")));
        }
    }
}