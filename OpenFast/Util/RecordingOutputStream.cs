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
namespace OpenFAST.util
{
	public sealed class RecordingOutputStream:System.IO.Stream
	{
		private readonly byte[] buffer = new byte[1024];
		private int index;
		private readonly System.IO.Stream out_Renamed;
		
		public RecordingOutputStream(System.IO.Stream outputStream)
		{
			out_Renamed = outputStream;
		}
		
		public  void  WriteByte(int b)
		{
			buffer[index++] = (byte) b;
			out_Renamed.WriteByte((System.Byte) b);
		}

		public override  void  WriteByte(byte b)
		{
			WriteByte(b);
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

		public override System.Int64 Seek(System.Int64 offset, System.IO.SeekOrigin origin)
		{
			return 0;
		}

		public override void  SetLength(System.Int64 value)
		{
		}

		public override System.Int32 Read(System.Byte[] readBuffer, System.Int32 offset, System.Int32 count)
		{
			return 0;
		}

		public override void  Write(System.Byte[] writeBuffer, System.Int32 offset, System.Int32 count)
		{
		}

		public override System.Boolean CanRead
		{
			get
			{
				return false;
			}
			
		}

		public override System.Boolean CanSeek
		{
			get
			{
				return false;
			}
			
		}

		public override System.Boolean CanWrite
		{
			get
			{
				return false;
			}
			
		}

		public override System.Int64 Length
		{
			get
			{
				return 0;
			}
			
		}

		public override System.Int64 Position
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