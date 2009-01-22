using System;
using DateValue = OpenFAST.DateValue;
using ScalarValue = OpenFAST.ScalarValue;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;
using OpenFAST;

namespace OpenFAST.Template.Type
{
	
	[Serializable]
	public class DateType:FASTType
	{
		override public ScalarValue DefaultValue
		{
			get
			{
				System.DateTime tempAux = new System.DateTime(0);
				return new DateValue(ref tempAux);
			}
			
		}
		private const long serialVersionUID = 1L;
		private TypeCodec dateCodec;
		private System.Globalization.DateTimeFormatInfo dateFormatter;

		//OVERLOOK
        //public DateType(string dateFormat, TypeCodec dateCodec):this(new SimpleDateFormat(dateFormat).ToString(), dateCodec)
        //{
        //}
		public DateType(System.Globalization.DateTimeFormatInfo dateFormat, TypeCodec dateCodec):base("date")
		{
			this.dateFormatter = dateFormat;
			this.dateCodec = dateCodec;
		}
		public override bool IsValueOf(ScalarValue previousValue)
		{
			return previousValue is DateValue;
		}
		public override TypeCodec GetCodec(Operator operator_Renamed, bool optional)
		{
			return dateCodec;
		}
		public override ScalarValue GetValue(string value_Renamed)
		{
			if (value_Renamed == null)
				return ScalarValue.UNDEFINED;
			try
			{
				System.DateTime tempAux = System.DateTime.Parse(value_Renamed, dateFormatter);
				return new DateValue(ref tempAux);
			}
			catch (System.FormatException e)
			{
				throw new RuntimeException(e);
			}
		}
		public override string Serialize(ScalarValue value_Renamed)
		{
			return SupportClass.FormatDateTime(dateFormatter, ((DateValue) value_Renamed).value_Renamed);
		}
		public override int GetHashCode()
		{
			int prime = 31;
			int result = 1;
			result = prime * result + ((dateCodec == null)?0:dateCodec.GetHashCode());
			result = prime * result + ((dateFormatter == null)?0:dateFormatter.GetHashCode());
			return result;
		}
		public  override bool Equals(System.Object obj)
		{
			if (this == obj)
				return true;
			if (obj == null)
				return false;
			if (GetType() != obj.GetType())
				return false;
			DateType other = (DateType) obj;
			if (dateCodec == null)
			{
				if (other.dateCodec != null)
					return false;
			}
			else if (!dateCodec.Equals(other.dateCodec))
				return false;
			if (dateFormatter == null)
			{
				if (other.dateFormatter != null)
					return false;
			}
			else if (!dateFormatter.Equals(other.dateFormatter))
				return false;
			return true;
		}
	}
}