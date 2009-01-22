using System;
using DecimalValue = OpenFAST.DecimalValue;
using Global = OpenFAST.Global;
using IntegerValue = OpenFAST.IntegerValue;
using NumericValue = OpenFAST.NumericValue;
using ScalarValue = OpenFAST.ScalarValue;
using LongValue = OpenFAST.Template.LongValue;
using OpenFAST;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	sealed class NullableSingleFieldDecimal:TypeCodec
	{
		public ScalarValue DefaultValue
		{
			get
			{
				return new DecimalValue(0.0);
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
		
		internal NullableSingleFieldDecimal()
		{
		}
		
		public override sbyte[] EncodeValue(ScalarValue v)
		{
			if (v == ScalarValue.NULL)
			{
				return TypeCodec.NULL_VALUE_ENCODING;
			}
			
			System.IO.MemoryStream buffer = new System.IO.MemoryStream();
			DecimalValue value_Renamed = (DecimalValue) v;
			
			try
			{
				if (System.Math.Abs(value_Renamed.exponent) > 63)
				{
					Global.HandleError(OpenFAST.Error.FastConstants.R1_LARGE_DECIMAL, "");
				}
				
				sbyte[] temp_sbyteArray;
				temp_sbyteArray = TypeCodec.NULLABLE_INTEGER.Encode(new IntegerValue(value_Renamed.exponent));
				buffer.Write(SupportClass.ToByteArray(temp_sbyteArray), 0, temp_sbyteArray.Length);
				sbyte[] temp_sbyteArray2;
				temp_sbyteArray2 = TypeCodec.INTEGER.Encode(new LongValue(value_Renamed.mantissa));
				buffer.Write(SupportClass.ToByteArray(temp_sbyteArray2), 0, temp_sbyteArray2.Length);
			}
			catch (System.IO.IOException e)
			{
				throw new RuntimeException(e);
			}
			
			return SupportClass.ToSByteArray(buffer.ToArray());
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			ScalarValue exp = TypeCodec.NULLABLE_INTEGER.Decode(in_Renamed);
			
			if ((exp == null) || exp.Null)
			{
				return null;
			}
			
			int exponent = ((NumericValue) exp).ToInt();
			long mantissa = ((NumericValue) TypeCodec.INTEGER.Decode(in_Renamed)).ToLong();
			DecimalValue decimalValue = new DecimalValue(mantissa, exponent);
			
			return decimalValue;
		}
		
		public ScalarValue FromString(string value_Renamed)
		{
			return new DecimalValue(System.Double.Parse(value_Renamed));
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