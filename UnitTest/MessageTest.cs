using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OpenFAST.Template;
using OpenFAST.Template.operator_Renamed;
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
