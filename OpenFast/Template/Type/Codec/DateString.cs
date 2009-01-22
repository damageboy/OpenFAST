using System;
using DateValue = OpenFAST.DateValue;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public class DateString:TypeCodec
	{
		private const long serialVersionUID = 1L;
		private System.Globalization.DateTimeFormatInfo formatter;
        private string FormatString;
		
		public DateString(string format)
		{
			//formatter = new SimpleDateFormat(format);//OVERLOOK
            formatter = new System.Globalization.DateTimeFormatInfo();
            FormatString = format;

		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			try
			{
				System.DateTime tempAux = System.DateTime.Parse(TypeCodec.ASCII.Decode(in_Renamed).ToString(), formatter);
				return new DateValue(ref tempAux);
			}
			catch (System.FormatException e)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.PARSE_ERROR, "", e);
				return null;
			}
		}
		
		public override sbyte[] EncodeValue(ScalarValue value_Renamed)
		{
			return TypeCodec.ASCII.Encode(new StringValue(SupportClass.FormatDateTime(formatter, ((DateValue) value_Renamed).value_Renamed)));
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