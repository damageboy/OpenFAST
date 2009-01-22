using System;
using Global = OpenFAST.Global;
using IntegerValue = OpenFAST.IntegerValue;
using QName = OpenFAST.QName;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using ComposedScalar = OpenFAST.Template.ComposedScalar;
using Scalar = OpenFAST.Template.Scalar;
using TwinValue = OpenFAST.Template.TwinValue;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using DecimalConverter = OpenFAST.Template.Type.DecimalConverter;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.util
{
	public class Util
	{
		private static readonly TwinValue NO_DIFF = new TwinValue(new IntegerValue(0), new StringValue(""));
		
		public static bool IsBiggerThanInt(long value_Renamed)
		{
			return (value_Renamed > System.Int32.MaxValue) || (value_Renamed < System.Int32.MinValue);
		}
		
		public static ScalarValue GetDifference(StringValue newValue, StringValue priorValue)
		{
			string value_Renamed = newValue.value_Renamed;
			
			if ((priorValue == null) || (priorValue.value_Renamed.Length == 0))
			{
				return new TwinValue(new IntegerValue(0), newValue);
			}
			
			if (priorValue.Equals(newValue))
			{
				return NO_DIFF;
			}
			
			string base_Renamed = priorValue.value_Renamed;
			int appendIndex = 0;
			
			while ((appendIndex < base_Renamed.Length) && (appendIndex < value_Renamed.Length) && (value_Renamed[appendIndex] == base_Renamed[appendIndex]))
				appendIndex++;
			
			string append = value_Renamed.Substring(appendIndex);
			
			int prependIndex = 1;
			
			while ((prependIndex <= value_Renamed.Length) && (prependIndex <= base_Renamed.Length) && (value_Renamed[value_Renamed.Length - prependIndex] == base_Renamed[base_Renamed.Length - prependIndex]))
				prependIndex++;
			
			string prepend = value_Renamed.Substring(0, (value_Renamed.Length - prependIndex + 1) - (0));
			
			if (prepend.Length < append.Length)
			{
				return new TwinValue(new IntegerValue(prependIndex - base_Renamed.Length - 2), new StringValue(prepend));
			}
			
			return new TwinValue(new IntegerValue(base_Renamed.Length - appendIndex), new StringValue(append));
		}
		
		public static StringValue ApplyDifference(StringValue baseValue, TwinValue diffValue)
		{
			int subtraction = ((IntegerValue) diffValue.first).value_Renamed;
			string base_Renamed = baseValue.value_Renamed;
			string diff = ((StringValue) diffValue.second).value_Renamed;
			
			if (subtraction < 0)
			{
				subtraction = ((- 1) * subtraction) - 1;
				
				return new StringValue(diff + base_Renamed.Substring(subtraction, (base_Renamed.Length) - (subtraction)));
			}
			
			return new StringValue(base_Renamed.Substring(0, (base_Renamed.Length - subtraction) - (0)) + diff);
		}
		
		public static string CollectionToString(System.Collections.ICollection set_Renamed)
		{
			return CollectionToString(set_Renamed, ",");
		}
		
		public static string CollectionToString(System.Collections.ICollection set_Renamed, string sep)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			System.Collections.IEnumerator iter = set_Renamed.GetEnumerator();
			buffer.Append("{");
			while (iter.MoveNext())
			{
				buffer.Append(iter.Current).Append(sep);
			}
			buffer.Remove(buffer.Length - sep.Length, 1);
			buffer.Append("}");
			return buffer.ToString();
		}
		
		public static int MillisecondsSinceMidnight(ref System.DateTime date)
		{
			System.Globalization.Calendar cal = new System.Globalization.GregorianCalendar();
			SupportClass.CalendarManager.manager.SetDateTime(cal, date);
			return SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.HOUR_OF_DAY) * 3600000 + SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.MINUTE) * 60000 + SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.SECOND) * 1000 + SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.MILLISECOND);
		}
		

		public static System.DateTime Date(int year, int month, int day)
		{
			System.Globalization.Calendar cal = new System.Globalization.GregorianCalendar();
			SupportClass.CalendarManager.manager.Set(cal, year - 1900, month - 1, day);
			return SupportClass.CalendarManager.manager.GetDateTime(cal);
		}
		
		public static int DateToInt(ref System.DateTime date)
		{
			System.Globalization.Calendar cal = new System.Globalization.GregorianCalendar();
			SupportClass.CalendarManager.manager.SetDateTime(cal, date);
			return SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.YEAR) * 10000 + (SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.MONTH) + 1) * 100 + SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.DATE);
		}
		
		public static int TimeToInt(ref System.DateTime date)
		{
			System.Globalization.Calendar cal = new System.Globalization.GregorianCalendar();
			SupportClass.CalendarManager.manager.SetDateTime(cal, date);
			return SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.HOUR_OF_DAY) * 10000000 + SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.MINUTE) * 100000 + SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.SECOND) * 1000 + SupportClass.CalendarManager.manager.Get(cal, SupportClass.CalendarManager.MILLISECOND);
		}
		
		public static int TimestampToInt(ref System.DateTime date)
		{
			return DateToInt(ref date) * 1000000000 + TimeToInt(ref date);
		}
		
		public static System.DateTime ToTimestamp(long value_Renamed)
		{
			System.Globalization.Calendar cal = new System.Globalization.GregorianCalendar();
			int year = (int) (value_Renamed / 10000000000000L);
			value_Renamed %= 10000000000000L;
			int month = (int) (value_Renamed / 100000000000L);
			value_Renamed %= 100000000000L;
			int day = (int) (value_Renamed / 1000000000);
			value_Renamed %= 1000000000;
			int hour = (int) (value_Renamed / 10000000);
			value_Renamed %= 10000000;
			int min = (int) (value_Renamed / 100000);
			value_Renamed %= 100000;
			int sec = (int) (value_Renamed / 1000);
			int ms = (int) (value_Renamed % 1000);
			SupportClass.CalendarManager.manager.Set(cal, year, month - 1, day, hour, min, sec);
			SupportClass.CalendarManager.manager.Set(cal, SupportClass.CalendarManager.MILLISECOND, ms);
			return SupportClass.CalendarManager.manager.GetDateTime(cal);
		}
		
		public static ComposedScalar ComposedDecimal(QName name, Operator exponentOp, ScalarValue exponentVal, Operator mantissaOp, ScalarValue mantissaVal, bool optional)
		{
			Scalar exponentScalar = new Scalar(Global.CreateImplicitName(name), Type.I32, exponentOp, exponentVal, optional);
			Scalar mantissaScalar = new Scalar(Global.CreateImplicitName(name), Type.I64, mantissaOp, mantissaVal, false);
			return new ComposedScalar(name, Type.DECIMAL, new Scalar[]{exponentScalar, mantissaScalar}, optional, new DecimalConverter());
		}
		
		public static int ToInt(string attribute)
		{
			try
			{
				return System.Int32.Parse(attribute);
			}
			catch (System.FormatException)
			{
				return 0;
			}
		}
	}
}