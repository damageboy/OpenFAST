using System;
using System.Collections.Generic;
using System.Text;
using UnitTest.Test;
using NUnit.Framework;
using OpenFAST.Template;
using OpenFAST.Codec;
using OpenFAST;
using OpenFAST.Template.Type;
using OpenFAST.Template.operator_Renamed;

namespace UnitTest.Scenario
{
    [TestFixture]
public class ErrorCasesTest : OpenFastTestCase {
        [Test]
	public void TestMantissaIsntPresentWhenExponentIsNull() {
        String templateXml = 
	        "<template name=\"SampleTemplate\">" +
	        "  <decimal name=\"bid\" presence=\"optional\">" +
	        "    <mantissa><copy /></mantissa>" +
	        "    <exponent><copy value=\"-2\" /></exponent>" +
	        "  </decimal>" +
	        "</template>";
		MessageTemplate template = Template(templateXml);
		FastEncoder encoder = Encoder(template);
		
		Message message = new Message(template);
		message.SetDecimal(1, 0.63);
		AssertEquals("11010000 10000001 10111111", encoder.Encode(message));
		
		message = new Message(template);
		AssertEquals("10100000 10000000", encoder.Encode(message));
	}
	[Test]
	public void TestEncodeDecodeNestedSequence() {
		Sequence nestedSequence = new Sequence("nested", new Field[] { new Scalar("string", FASTType.ASCII, Operator.COPY, ScalarValue.UNDEFINED, false) }, true);
		Group group = new Group("group", new Field[] { nestedSequence }, true);
		MessageTemplate t = new MessageTemplate("template", new Field[] { group });
		Message message = new Message(t);
		
		FastEncoder encoder = Encoder(t);
		AssertEquals("11000000 10000001", encoder.Encode(message));
		
		FastDecoder decoder = Decoder("11000000 10000001", t);
		Assert.AreEqual(message, decoder.ReadMessage());
	}
	[Test]
	public void TestDictionaryNotInherited() {
		String templateDef = "<template name=\"OptDeltaDec\" id=\"58\" dictionary=\"template\">" +
	    "    <string name=\"desc\"/>" +
	    "    <decimal id=\"1\" presence=\"optional\" name=\"Line1\">" + 
	    "         <exponent><copy/></exponent>" + 
	    "         <mantissa><copy/></mantissa>" + 
	    "    </decimal>" +
	    "    <decimal id=\"1\" presence=\"optional\" name=\"Line2\">" +
	    "         <exponent><copy/></exponent>" + 
	    "         <mantissa><copy/></mantissa>" + 
	    "    </decimal>    " +
	    "    <decimal id=\"1\" presence=\"optional\" name=\"Line3\">" +
	    "         <exponent><copy/></exponent> " +
	    "         <mantissa><copy/></mantissa>" + 
	    "    </decimal>" +    
        "</template>";
		
		MessageTemplate template = Template(templateDef);

        Message m = new Message(template);
        
        m.SetString("desc", "prev");
        m.SetDecimal("Line2", 9427.61 );     
        m.SetDecimal("Line3", 9427.6 );
        
        byte[] bytes = Encoder(template).Encode(m);
        Message m2 = Decoder(template, bytes).ReadMessage();
        
        Assert.AreEqual(m, m2);
	}
}

}
