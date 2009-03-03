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

*/
using System;
using System.Collections.Generic;
using System.Text;
using UnitTest.Test;
using OpenFAST;
using NUnit.Framework;
using System.IO;
using OpenFAST.Template;
using OpenFAST.Template.Type;
using OpenFAST.Template.operator_Renamed;

namespace UnitTest
{
    [TestFixture]
    public class EncodeDecodeTest
    {
        [Test]
        public void TestComplexMessage()
        {
            MessageTemplate template = new MessageTemplate("Company",
                new Field[] {
                new Scalar("Name", FASTType.STRING, Operator.NONE, ScalarValue.UNDEFINED, false),
                new Scalar("Id", FASTType.U32, Operator.INCREMENT, ScalarValue.UNDEFINED, false),
                new Sequence("Employees",
                    new Field[] {
                        new Scalar("First Name", FASTType.STRING, Operator.COPY, ScalarValue.UNDEFINED, false),
                        new Scalar("Last Name", FASTType.STRING, Operator.COPY, ScalarValue.UNDEFINED, false),
                        new Scalar("Age", FASTType.U32, Operator.DELTA, ScalarValue.UNDEFINED, false)
                    }, false),
                new Group("Tax Information",
                    new Field[] {
                        new Scalar("EIN", FASTType.STRING, Operator.NONE, ScalarValue.UNDEFINED, false)
                    }, false)
            });
            Message aaaInsurance = new Message(template);
            aaaInsurance.SetFieldValue(1, new StringValue("AAA Insurance"));
            aaaInsurance.SetFieldValue(2, new IntegerValue(5));

            SequenceValue employees = new SequenceValue(template.GetSequence(
                        "Employees"));
            employees.Add(new FieldValue[] {
                new StringValue("John"), new StringValue("Doe"),
                new IntegerValue(45)
            });
            employees.Add(new FieldValue[] {
                new StringValue("Jane"), new StringValue("Doe"),
                new IntegerValue(48)
            });
            aaaInsurance.SetFieldValue(3, employees);
            aaaInsurance.SetFieldValue(4,
                new GroupValue(template.GetGroup("Tax Information"),
                    new FieldValue[] { new StringValue("99-99999999") }));

            MemoryStream outStream = new MemoryStream();
            MessageOutputStream output = new MessageOutputStream(outStream);
            output.RegisterTemplate(1, template);
            output.WriteMessage(aaaInsurance);

            Message abcBuilding = new Message(template);
            abcBuilding.SetFieldValue(1, new StringValue("ABC Building"));
            abcBuilding.SetFieldValue(2, new IntegerValue(6));
            employees = new SequenceValue(template.GetSequence("Employees"));
            employees.Add(new FieldValue[] {
                new StringValue("Bob"), new StringValue("Builder"),
                new IntegerValue(3)
            });
            employees.Add(new FieldValue[] {
                new StringValue("Joe"), new StringValue("Rock"),
                new IntegerValue(59)
            });
            abcBuilding.SetFieldValue(3, employees);
            abcBuilding.SetFieldValue(4,
                new GroupValue(template.GetGroup("Tax Information"),
                    new FieldValue[] { new StringValue("99-99999999") }));
            output.WriteMessage(abcBuilding);

            MessageInputStream input = new MessageInputStream(new MemoryStream(
                        outStream.ToArray()));
            input.RegisterTemplate(1, template);

            GroupValue message = input.ReadMessage();
            Assert.AreEqual(aaaInsurance, message);

            message = input.ReadMessage();
            Assert.AreEqual(abcBuilding, message);
        }
        [Test]
        public void TestMultipleMessages()
        {
            MemoryStream outStream = new MemoryStream();
            MessageOutputStream output = new MessageOutputStream(outStream);
            output.RegisterTemplate(ObjectMother.ALLOC_INSTRCTN_TEMPLATE_ID,
                ObjectMother.AllocationInstruction());

            SequenceValue allocations = new SequenceValue(ObjectMother.AllocationInstruction()
                                                                      .GetSequence("Allocations"));
            allocations.Add(ObjectMother.NewAllocation("fortyFiveFund", 22.5, 75.0));
            allocations.Add(ObjectMother.NewAllocation("fortyFund", 24.6, 25.0));

            Message ai1 = ObjectMother.NewAllocInstrctn("ltg0001", 1, 100.0, 23.4,
                    ObjectMother.NewInstrument("CTYA", "200910"), allocations);

            allocations = new SequenceValue(ObjectMother.AllocationInstruction()
                                                        .GetSequence("Allocations"));
            allocations.Add(ObjectMother.NewAllocation("fortyFiveFund", 22.5, 75.0));
            allocations.Add(ObjectMother.NewAllocation("fortyFund", 24.6, 25.0));

            Message ai2 = ObjectMother.NewAllocInstrctn("ltg0001", 1, 100.0, 23.4,
                    ObjectMother.NewInstrument("CTYA", "200910"), allocations);

            allocations = new SequenceValue(ObjectMother.AllocationInstruction()
                                                        .GetSequence("Allocations"));
            allocations.Add(ObjectMother.NewAllocation("fortyFiveFund", 22.5, 75.0));
            allocations.Add(ObjectMother.NewAllocation("fortyFund", 24.6, 25.0));

            Message ai3 = ObjectMother.NewAllocInstrctn("ltg0001", 1, 100.0, 23.4,
                    ObjectMother.NewInstrument("CTYA", "200910"), allocations);

            output.WriteMessage(ai1);
            output.WriteMessage(ai2);
            output.WriteMessage(ai3);

            byte[] bytes = outStream.ToArray();
            MessageInputStream input = new MessageInputStream(new MemoryStream(
                        bytes));
            input.RegisterTemplate(ObjectMother.ALLOC_INSTRCTN_TEMPLATE_ID,
                ObjectMother.AllocationInstruction());

            Message message = input.ReadMessage();
            Assert.AreEqual(ai1, message);
            message = input.ReadMessage();
            Assert.AreEqual(ai2, message);
            Assert.AreEqual(ai3, input.ReadMessage());
        }
    }

}
