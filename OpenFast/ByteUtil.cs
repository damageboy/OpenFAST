using System;
using System.Text;

namespace OpenFAST
{
	public class ByteUtil
	{
		public static sbyte[] ConvertBitStringToFastByteArray(string bitString)
		{
			if (bitString.Length == 0)
			{
				return new sbyte[0];
			}
			
			string[] bitStrings = bitString.Split(' ');
			sbyte[] bytes = new sbyte[bitStrings.Length];
			
			for (int i = 0; i < bitStrings.Length; i++)
			{
				bytes[i] = (sbyte) System.Convert.ToInt32(bitStrings[i], 2);
			}
			
			return bytes;
		}
		
		public static sbyte[] ConvertHexStringToByteArray(string hexString)
		{
			if (hexString == null)
			{
				return new sbyte[0];
			}
			
			hexString = hexString.Replace(" ", "");
			sbyte[] bytes = new sbyte[hexString.Length / 2];
			
			for (int i = 0; i < hexString.Length; i += 2)
			{
				bytes[i / 2] = (sbyte) System.Convert.ToInt32(hexString.Substring(i, (i + 2) - (i)), 16);
			}
			
			return bytes;
		}

		public static string ConvertByteArrayToBitString(sbyte[] bytes)
		{
			return ConvertByteArrayToBitString(bytes, bytes.Length);
		}
		
		public static string ConvertByteArrayToBitString(sbyte[] bytes, int length)
		{
			if (bytes.Length == 0)
			{
				return "";
			}
			
			StringBuilder builder = new StringBuilder();
			
			for (int i = 0; i < length; i++)
			{
				string bits = System.Convert.ToString(bytes[i] & 0xFF, 2);
				
				for (int j = 0; j < (8 - bits.Length); j++)
					builder.Append('0');
				
				builder.Append(bits).Append(' ');
			}
			
            //if (builder.length() > 0)//OVERLOOK
            //    builder.deleteCharAt(builder.length() - 1);
            if (builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);
			
			return builder.ToString();
		}
		
		public static System.IO.Stream CreateByteStream(string bitString)
		{
			return new System.IO.MemoryStream(SupportClass.ToByteArray(ConvertBitStringToFastByteArray(bitString)));
		}
		
		public static System.IO.Stream CreateByteStreamFromHexBytes(string hexString)
		{
			return new System.IO.MemoryStream(SupportClass.ToByteArray(ConvertHexStringToByteArray(hexString)));
		}
		
		public static sbyte[] Combine(sbyte[] first, sbyte[] second)
		{
			sbyte[] result = new sbyte[first.Length + second.Length];
			Array.Copy(first, 0, result, 0, first.Length);
			Array.Copy(second, 0, result, first.Length, second.Length);
			return result;
		}
		
		public static bool IsEmpty(sbyte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
				if ((bytes[i] & 0x7f) != 0)
					return false;
			return true;
		}
	}
}