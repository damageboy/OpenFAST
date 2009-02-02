using System;
using System.Collections.Generic;
using System.Text;
using UnitTest.Test;
using OpenFAST.Error;
using NUnit.Framework;
using OpenFAST;

namespace UnitTest
{
    [TestFixture]
    public class MessageInputStreamTest : OpenFastTestCase
    {
        [Test]
        public void TestReadMessage()
        {
            MessageInputStream input = new MessageInputStream(BitStream("11000000 10000100"));
            try
            {
                input.ReadMessage();
                Assert.Fail();
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.D9_TEMPLATE_NOT_REGISTERED, e.Code);
            }
        }

    }
}
