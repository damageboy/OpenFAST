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

namespace OpenFAST.util
{
	public sealed class RecordingInputStream: System.IO.Stream
	{
		public byte[] Buffer
		{
			get
			{
				var b = new byte[index];
				Array.Copy(buffer, 0, b, 0, index);
				
				return b;
			}
			
		}
		private readonly byte[] buffer = new byte[1024];
		private int index;
		private readonly System.IO.Stream in_Renamed;
		
		public RecordingInputStream(System.IO.Stream inputStream)
		{
			in_Renamed = inputStream;
		}
		
		
		public override int ReadByte()
		{
			int read = in_Renamed.ReadByte();
			buffer[index++] = (byte) read;
			return read;
		}
		
		public override string ToString()
		{
			return ByteUtil.ConvertByteArrayToBitString(buffer, index);
		}
		
		public void  Clear()
		{
			index = 0;
		}

		public override void  Flush()
		{
		}

		public override Int64 Seek(Int64 offset, System.IO.SeekOrigin origin)
		{
			return 0;
		}

		public override void  SetLength(Int64 value)
		{
		}

		public override Int32 Read(Byte[] readBuffer, Int32 offset, Int32 count)
		{
			return 0;
		}

		public override void  Write(Byte[] writeBuffer, Int32 offset, Int32 count)
		{
		}

		public override Boolean CanRead
		{
			get
			{
				return false;
			}
			
		}

		public override Boolean CanSeek
		{
			get
			{
				return false;
			}
			
		}

        public override Boolean CanWrite
		{
			get
			{
				return false;
			}
			
		}

		public override Int64 Length
		{
			get
			{
				return 0;
			}
			
		}

		public override Int64 Position
		{
			get
			{
				return 0;
			}
			
			set
			{
			}
			
		}
	}
}