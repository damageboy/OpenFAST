using System;
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using BitVectorReader = OpenFAST.BitVectorReader;
using Context = OpenFAST.Context;
using FieldValue = OpenFAST.FieldValue;

namespace OpenFAST.Template
{
	
	[Serializable]
	public class StaticTemplateReference:Field
	{
		override public string TypeName
		{
			get
			{
				return null;
			}
			
		}
		override public System.Type ValueType
		{
			get
			{
				return null;
			}
			
		}
		virtual public MessageTemplate Template
		{
			get
			{
				return template;
			}
			
		}
		private const long serialVersionUID = 1L;
		private MessageTemplate template;
		
		public StaticTemplateReference(MessageTemplate template):base(template.QName, false)
		{
			this.template = template;
		}
		
		public override FieldValue CreateValue(string value_Renamed)
		{
			return null;
		}
		
		public override FieldValue Decode(System.IO.Stream in_Renamed, Group template, Context context, BitVectorReader pmapReader)
		{
			return null;
		}
		
		public override byte[] Encode(FieldValue value_Renamed, Group template, Context context, BitVectorBuilder presenceMapBuilder)
		{
			return null;
		}
		
		public override bool IsPresenceMapBitSet(byte[] encoding, FieldValue fieldValue)
		{
			return false;
		}
		
		public override bool UsesPresenceMapBit()
		{
			return false;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (obj == this)
				return true;
			if (obj == null || obj.GetType() != this.GetType())
				return false;
			StaticTemplateReference other = (StaticTemplateReference) obj;
			return template.Equals(other.template);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}