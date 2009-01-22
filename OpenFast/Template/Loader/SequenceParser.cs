using System;
using Global = OpenFAST.Global;
using QName = OpenFAST.QName;
using ScalarValue = OpenFAST.ScalarValue;
using Field = OpenFAST.Template.Field;
using Scalar = OpenFAST.Template.Scalar;
using Sequence = OpenFAST.Template.Sequence;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.Loader
{
	
	public class SequenceParser:AbstractFieldParser
	{
		public class SequenceLengthParser:ScalarParser
		{
			private void  InitBlock(SequenceParser enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private SequenceParser enclosingInstance;
			public SequenceParser Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal SequenceLengthParser(SequenceParser enclosingInstance, string Param1):base(Param1)
			{
				InitBlock(enclosingInstance);
			}
			protected internal override FASTType GetType(System.Xml.XmlElement fieldNode, ParsingContext context)
			{
				return FASTType.U32;
			}
			
			protected internal override QName GetName(System.Xml.XmlElement fieldNode, ParsingContext context)
			{
				if (context.GetName() == null)
					return Global.CreateImplicitName(context.Parent.GetName());
				return context.GetName();
			}
		}
		private void  InitBlock()
		{
			sequenceLengthParser = new SequenceLengthParser(this, "length");
		}
		
		private ScalarParser sequenceLengthParser;
		
		public SequenceParser():base("sequence")
		{
			InitBlock();
		}
		
		public override Field Parse(System.Xml.XmlElement sequenceElement, bool optional, ParsingContext context)
		{
			Sequence sequence = new Sequence(context.GetName(), ParseSequenceLengthField(context.GetName(), sequenceElement, optional, context), GroupParser.ParseFields(sequenceElement, context), optional);
			GroupParser.ParseMore(sequenceElement, sequence.Group, context);
			return sequence;
		}

		private Scalar ParseSequenceLengthField(QName name, System.Xml.XmlElement sequence, bool optional, ParsingContext parent)
		{
			System.Xml.XmlNodeList lengthElements = sequence.GetElementsByTagName("length");
			
			if (lengthElements.Count == 0)
			{
				Scalar implicitLength = new Scalar(Global.CreateImplicitName(name), FASTType.U32, Operator.NONE, ScalarValue.UNDEFINED, optional);
				implicitLength.Dictionary = parent.Dictionary;
				return implicitLength;
			}
			
			System.Xml.XmlElement length = (System.Xml.XmlElement) lengthElements.Item(0);
			ParsingContext context = new ParsingContext(length, parent);
			return (Scalar) sequenceLengthParser.Parse(length, optional, context);
		}
	}
}