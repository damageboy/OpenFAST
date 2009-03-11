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
using Util = OpenFAST.util.Util;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public abstract class IntegerCodec:TypeCodec
	{
		protected internal IntegerCodec()
		{
		}
		
		protected internal static ScalarValue CreateValue(long value_Renamed)
		{
			if (Util.IsBiggerThanInt(value_Renamed))
			{
				return new LongValue(value_Renamed);
			}
			
			return new IntegerValue((int) value_Renamed);
		}
		
		public static int GetUnsignedIntegerSize(long value_Renamed)
		{
			if (value_Renamed < 128)
			{
				return 1; // 2 ^ 7
			}
			
			if (value_Renamed <= 16384)
			{
				return 2; // 2 ^ 14
			}
			
			if (value_Renamed <= 2097152)
			{
				return 3; // 2 ^ 21
			}
			
			if (value_Renamed <= 268435456)
			{
				return 4; // 2 ^ 28
			}
			
			if (value_Renamed <= 34359738368L)
			{
				return 5; // 2 ^ 35
			}
			
			if (value_Renamed <= 4398046511104L)
			{
				return 6; // 2 ^ 42
			}
			
			if (value_Renamed <= 562949953421312L)
			{
				return 7; // 2 ^ 49
			}
			
			if (value_Renamed <= 72057594037927936L)
			{
				return 8; // 2 ^ 56
			}
			
			return 9;
		}
		
		public static int GetSignedIntegerSize(long value_Renamed)
		{
			if ((value_Renamed >= - 64) && (value_Renamed <= 63))
			{
				return 1; // - 2 ^ 6 ... 2 ^ 6 -1
			}
			
			if ((value_Renamed >= - 8192) && (value_Renamed <= 8191))
			{
				return 2; // - 2 ^ 13 ... 2 ^ 13 -1
			}
			
			if ((value_Renamed >= - 1048576) && (value_Renamed <= 1048575))
			{
				return 3; // - 2 ^ 20 ... 2 ^ 20 -1
			}
			
			if ((value_Renamed >= - 134217728) && (value_Renamed <= 134217727))
			{
				return 4; // - 2 ^ 27 ... 2 ^ 27 -1
			}
			
			if ((value_Renamed >= - 17179869184L) && (value_Renamed <= 17179869183L))
			{
				return 5; // - 2 ^ 34 ... 2 ^ 34 -1
			}
			
			if ((value_Renamed >= - 2199023255552L) && (value_Renamed <= 2199023255551L))
			{
				return 6; // - 2 ^ 41 ... 2 ^ 41 -1
			}
			
			if ((value_Renamed >= - 281474976710656L) && (value_Renamed <= 281474976710655L))
			{
				return 7; // - 2 ^ 48 ... 2 ^ 48 -1
			}
			
			if ((value_Renamed >= - 36028797018963968L) && (value_Renamed <= 36028797018963967L))
			{
				return 8; // - 2 ^ 55 ... 2 ^ 55 -1
			}
			
			if ((value_Renamed >= - 4611686018427387904L && value_Renamed <= 4611686018427387903L))
			{
				return 9;
			}
			return 10;
		}
		
		public  override bool Equals(Object obj)
		{
			return obj != null && obj.GetType() == GetType();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}