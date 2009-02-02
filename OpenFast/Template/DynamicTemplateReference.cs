using System;
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using BitVectorReader = OpenFAST.BitVectorReader;
using Context = OpenFAST.Context;
using FieldValue = OpenFAST.FieldValue;
using Message = OpenFAST.Message;
using QName = OpenFAST.QName;
using FastDecoder = OpenFAST.Codec.FastDecoder;

namespace OpenFAST.Template
{
	[Serializable]
	public sealed class DynamicTemplateReference:Field
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
		private const long serialVersionUID = 1L;
		public static readonly DynamicTemplateReference INSTANCE = new DynamicTemplateReference();
		
		private DynamicTemplateReference():base(QName.NULL, false)
		{
		}
		
		public override FieldValue CreateValue(string value_Renamed)
		{
			return null;
		}
		
		
		public override FieldValue Decode(System.IO.Stream in_Renamed, Group template, Context context, BitVectorReader pmapReader)
		{
			return new FastDecoder(context, in_Renamed).ReadMessage();
		}
		
		
		public override byte[] Encode(FieldValue value_Renamed, Group template, Context context, BitVectorBuilder presenceMapBuilder)
		{
			Message message = (Message) value_Renamed;
			return message.Template.Encode(message, context);
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
			return obj != null && obj.GetType().Equals(this.GetType());
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}