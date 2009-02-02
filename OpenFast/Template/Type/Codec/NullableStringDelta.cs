using System;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using TwinValue = OpenFAST.Template.TwinValue;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public class NullableStringDelta:TypeCodec
	{

		virtual public ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}

		override public bool Nullable
		{
			get
			{
				return true;
			}
			
		}
		private const long serialVersionUID = 1L;
		
		public NullableStringDelta()
		{
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			ScalarValue subtractionLength = TypeCodec.NULLABLE_INTEGER.Decode(in_Renamed);
			if (subtractionLength == null)
				return null;
			
			ScalarValue difference = TypeCodec.ASCII.Decode(in_Renamed);
			
			return new TwinValue(subtractionLength, difference);
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			if (value_Renamed.Null)
				return TypeCodec.NULL_VALUE_ENCODING;
			
			TwinValue diff = (TwinValue) value_Renamed;
			byte[] subtractionLength = TypeCodec.NULLABLE_INTEGER.Encode(diff.first);
			byte[] difference = TypeCodec.ASCII.Encode(diff.second);
			byte[] encoded = new byte[subtractionLength.Length + difference.Length];
			Array.Copy(subtractionLength, 0, encoded, 0, subtractionLength.Length);
			Array.Copy(difference, 0, encoded, subtractionLength.Length, difference.Length);
			
			return encoded;
		}
		
		public virtual ScalarValue FromString(string value_Renamed)
		{
			return new StringValue(value_Renamed);
		}
		
		public  override bool Equals(System.Object obj)
		{
			return obj != null && obj.GetType() == GetType();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}