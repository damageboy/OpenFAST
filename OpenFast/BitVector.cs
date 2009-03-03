/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>

*/
using System;

namespace OpenFAST
{
	public sealed class BitVector
	{
		public byte[] Bytes
		{
			get
			{
				return bytes;
			}
			
		}
		public byte[] TruncatedBytes
		{
			get
			{
				int index = bytes.Length - 1;
				
				for (; (index > 0) && ((bytes[index] & VALUE_BITS_SET) == 0); index--)
					;
				
				if (index == (bytes.Length - 1))
				{
					return bytes;
				}
				
				byte[] truncated = new byte[index + 1];
				Array.Copy(bytes, 0, truncated, 0, index + 1);
                byte tempStop = STOP_BIT;
                truncated[truncated.Length - 1] |= (byte)(tempStop);
				
				return truncated;
			}
			
		}
		public int Size
		{
			get
			{
				return this.size;
			}
			
		}
		public bool Overlong
		{
			get
			{
				return (bytes.Length > 1) && ((bytes[bytes.Length - 1] & VALUE_BITS_SET) == 0);
			}
			
		}
		private const int VALUE_BITS_SET = 0x7F;
		private const int STOP_BIT = 0x80;
		private byte[] bytes;
		private int size;
		
		public BitVector(int size):this(new byte[((size - 1) / 7) + 1])
		{
		}
		
		public BitVector(byte[] bytes)
		{
			this.bytes = bytes;
			this.size = bytes.Length * 7;
            byte tempStop = STOP_BIT;

            bytes[bytes.Length - 1] |= (byte)(tempStop);
		}

        public void set_Renamed(int fieldIndex)
        {
            bytes[fieldIndex / 7] |= (byte)((1 << (6 - (fieldIndex % 7))));
        }

        public bool IsSet(int fieldIndex)
        {
            if (fieldIndex >= bytes.Length * 7)
                return false;
            return ((bytes[fieldIndex / 7] & (1 << (6 - (fieldIndex % 7)))) > 0);
        }

        public  override bool Equals(System.Object obj)
		{
			if ((obj == null) || !(obj is BitVector))
			{
				return false;
			}
			
			return Equals((BitVector) obj);
		}
		
		public bool Equals(BitVector other)
		{
			if (other.size != this.size)
			{
				return false;
			}
			
			for (int i = 0; i < this.bytes.Length; i++)
			{
				if (this.bytes[i] != other.bytes[i])
				{
					return false;
				}
			}
			
			return true;
		}
		
		public override int GetHashCode()
		{
			return bytes.GetHashCode();
		}
		
		public override string ToString()
		{
			return "BitVector [" + ByteUtil.ConvertByteArrayToBitString(bytes) + "]";
		}

        public int IndexOfLastSet()
        {
            int index = bytes.Length * 7 - 1;
            while (index >= 0 && !(((bytes[index / 7] & (1 << (6 - (index % 7)))) > 0)))
                index--;
            return index;
        }
	}
}