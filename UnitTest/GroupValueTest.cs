using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OpenFAST;
using UnitTest.Test;

namespace UnitTest
{
    [TestFixture]
    public class GroupValueTest 
    {
        [Test]
        public void TestEquals()
        {
            GroupValue v = ObjectMother.NewAllocation(null, 10.0, 11.0);
            GroupValue v2 = ObjectMother.NewAllocation(null, 10.0, 11.0);
            Assert.AreEqual(v, v2);
        }
    }
}
