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
using NUnit.Framework;
using OpenFAST.Template;
using openfast.Template.Operator;
using OpenFAST.Template.Type;
using OpenFAST;

namespace UnitTest
{
    [TestFixture]
public class MessageTest {
    [Test]
    public void TestEquals() {
        MessageTemplate template = new MessageTemplate("",
                new Field[] {
                    new Scalar("1", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false)
                });
        GroupValue message = new Message(template);
        message.SetInteger(1, 1);

        GroupValue other = new Message(template);
        other.SetInteger(1, 1);

        Assert.AreEqual(message, other);
    }
    [Test]
    public void TestNotEquals() {
        MessageTemplate template = new MessageTemplate("",
                new Field[] {
                    new Scalar("1", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false)
                });
        Message message = new Message(template);
        message.SetInteger(1, 2);

        Message other = new Message(template);
        Assert.IsFalse(message.equals(other));
        Assert.IsFalse(other.equals(message));
        other.SetInteger(1, 1);

        Assert.IsFalse(message.equals(other));
        Assert.IsFalse(other.equals(message));
    }
}

}
