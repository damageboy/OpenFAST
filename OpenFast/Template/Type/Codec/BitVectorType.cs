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

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public sealed class BitVectorType:TypeCodec
	{
		public static ScalarValue DefaultValue
		{
			get
			{
				return new BitVectorValue(new BitVector(0));
			}
			
		}

	    internal BitVectorType()
		{
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			return ((BitVectorValue) value_Renamed).value_Renamed.Bytes;
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			var buffer = new System.IO.MemoryStream();
			int byt;
			do 
			{
				try
				{
					byt = in_Renamed.ReadByte();
					
					if (byt < 0)
					{
						return null;
					}
				}
				catch (System.IO.IOException e)
				{
					throw new RuntimeException(e);
				}
				
				buffer.WriteByte((Byte) byt);
			}
			while ((byt & STOP_BIT) == 0);
			
			return new BitVectorValue(new BitVector(buffer.ToArray()));
		}
		
		public ScalarValue FromString(string value_Renamed)
		{
			return null;
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