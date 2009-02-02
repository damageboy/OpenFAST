using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OpenFAST;
using UnitTest.Test;

namespace UnitTest
{
    [TestFixture]
    public class ApplicationTypeDictionaryTest : OpenFastTestCase
    {
        [Test]
        public void TestLookup()
        {
            ObjectMother.AllocationInstruction().TypeReference = new QName("AllocationInstruction");
            ObjectMother.Allocations().TypeReference = new QName("Allocation");

            Context context = new Context();

            context.Store("type", ObjectMother.AllocationInstruction(), new QName("ID"), String("1234"));

            Assert.AreEqual(String("1234"), context.Lookup("type", ObjectMother.AllocationInstruction(), new QName("ID")));
            Assert.AreEqual(ScalarValue.UNDEFINED, context.Lookup("type", ObjectMother.Allocations().Group, new QName("ID")));
        }
    }
}
