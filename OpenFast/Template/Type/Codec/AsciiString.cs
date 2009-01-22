using System;
using ByteUtil = OpenFAST.ByteUtil;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using OpenFAST;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	sealed class AsciiString:TypeCodec
	{
		private const long serialVersionUID = 1L;
		private const string ZERO_TERMINATOR = "\u0000";

		private static readonly sbyte[] ZERO_PREAMBLE = new sbyte[]{0, 0};
		
		internal AsciiString()
		{
		}
		
		public override sbyte[] EncodeValue(ScalarValue value_Renamed)
		{
			if ((value_Renamed == null) || value_Renamed.Null)
			{
				throw new System.SystemException("Only nullable strings can represent null values.");
			}
			
			string string_Renamed = value_Renamed.ToString();
			
			if ((string_Renamed != null) && (string_Renamed.Length == 0))
			{
				return TypeCodec.NULL_VALUE_ENCODING;
			}
			if (string_Renamed.StartsWith(ZERO_TERMINATOR))
			{
				return ZERO_PREAMBLE;
			}
			return SupportClass.ToSByteArray(SupportClass.ToByteArray(string_Renamed));
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			System.IO.MemoryStream buffer = new System.IO.MemoryStream();
			int byt;
			
			try
			{
				do 
				{
					byt = in_Renamed.ReadByte();
					buffer.WriteByte((System.Byte) byt);
				}
				while ((byt & 0x80) == 0);
			}
			catch (System.IO.IOException e)
			{
				throw new RuntimeException(e);
			}
			
			sbyte[] bytes = SupportClass.ToSByteArray(buffer.ToArray());
			bytes[bytes.Length - 1] &= (sbyte) (0x7f);
			
			if (bytes[0] == 0)
			{
				if (!ByteUtil.IsEmpty(bytes))
					Global.HandleError(OpenFAST.Error.FastConstants.R9_STRING_OVERLONG, null);
				if (bytes.Length > 1 && bytes[1] == 0)
					return new StringValue("\u0000");
				return new StringValue("");
			}
			
			return new StringValue(new string(SupportClass.ToCharArray(SupportClass.ToByteArray(bytes))));
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