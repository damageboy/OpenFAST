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
using OpenFAST.Template.Operator;
using OpenFAST.Template.Types;

namespace OpenFAST.UnitTests
{
    [TestFixture]
    public class MessageTest
    {
        [Test]
        public void TestEquals()
        {
            var template = new MessageTemplate(
                "",
                new Field[]
                    {
                        new Scalar("1", FASTType.U32, Operator.Copy, ScalarValue.Undefined, false)
                    });
            GroupValue message = new Message(template);
            message.SetInteger(1, 1);

            GroupValue other = new Message(template);
            other.SetInteger(1, 1);

            Assert.AreEqual(message, other);
            //Assert.AreEqual(message.GetHashCode(), other.GetHashCode());
        }

        [Test]
        public void TestNotEquals()
        {
            var template = new MessageTemplate(
                "",
                new Field[]
                    {
                        new Scalar("1", FASTType.U32, Operator.Copy, ScalarValue.Undefined, false)
                    });
            var message = new Message(template);
            var other = new Message(template);

            message.SetInteger(1, 2);
            Assert.IsFalse(message.Equals(other));
            Assert.IsFalse(other.Equals(message));

            other.SetInteger(1, 1);
            Assert.IsFalse(message.Equals(other));
            Assert.IsFalse(other.Equals(message));
        }
    }
}