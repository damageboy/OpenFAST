using System;
using System.Text;

namespace OpenFAST
{
	public class ByteUtil
	{
		public static byte[] ConvertBitStringToFastByteArray(string bitString)
		{
			if (bitString.Length == 0)
			{
				return new byte[0];
			}
			
			string[] bitStrings = bitString.Split(' ');
			byte[] bytes = new byte[bitStrings.Length];
			
			for (int i = 0; i < bitStrings.Length; i++)
			{
				bytes[i] = (byte) System.Convert.ToInt32(bitStrings[i], 2);
			}
			
			return bytes;
		}
		
		public static byte[] ConvertHexStringToByteArray(string hexString)
		{
			if (hexString == null)
			{
				return new byte[0];
			}
			
			hexString = hexString.Replace(" ", "");
			byte[] bytes = new byte[hexString.Length / 2];
			
			for (int i = 0; i < hexString.Length; i += 2)
			{
				bytes[i / 2] = (byte) System.Convert.ToInt32(hexString.Substring(i, (i + 2) - (i)), 16);
			}
			
			return bytes;
		}

		public static string ConvertByteArrayToBitString(byte[] bytes)
		{
			return ConvertByteArrayToBitString(bytes, bytes.Length);
		}
		
		public static string ConvertByteArrayToBitString(byte[] bytes, int length)
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
			return new System.IO.MemoryStream(ConvertBitStringToFastByteArray(bitString));
		}
		
		public static System.IO.Stream CreateByteStreamFromHexBytes(string hexString)
		{
			return new System.IO.MemoryStream(ConvertHexStringToByteArray(hexString));
		}
		
		public static byte[] Combine(byte[] first, byte[] second)
		{
			byte[] result = new byte[first.Length + second.Length];
			Array.Copy(first, 0, result, 0, first.Length);
			Array.Copy(second, 0, result, first.Length, second.Length);
			return result;
		}
		
		public static bool IsEmpty(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
				if ((bytes[i] & 0x7f) != 0)
					return false;
			return true;
		}
	}
}