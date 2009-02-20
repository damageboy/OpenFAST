using System;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using TwinValue = OpenFAST.Template.TwinValue;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public sealed class StringDelta:TypeCodec
	{
		public ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}
		private const long serialVersionUID = 1L;
		
		public StringDelta()
		{
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			ScalarValue subtractionLength = TypeCodec.INTEGER.Decode(in_Renamed);
			ScalarValue difference = TypeCodec.ASCII.Decode(in_Renamed);
			
			return new TwinValue(subtractionLength, difference);
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			if ((value_Renamed == null) || (value_Renamed == ScalarValue.NULL))
			{
				throw new System.SystemException("Cannot have null values for non-nullable string delta");
			}
			
			TwinValue diff = (TwinValue) value_Renamed;
			byte[] subtractionLength = TypeCodec.INTEGER.Encode(diff.first);
			byte[] difference = TypeCodec.ASCII.Encode(diff.second);
			byte[] encoded = new byte[subtractionLength.Length + difference.Length];
			Array.Copy(subtractionLength, 0, encoded, 0, subtractionLength.Length);
			Array.Copy(difference, 0, encoded, subtractionLength.Length, difference.Length);
			
			return encoded;
		}
		
		public ScalarValue FromString(string value_Renamed)
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