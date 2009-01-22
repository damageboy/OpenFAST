using System;
using ByteUtil = OpenFAST.ByteUtil;

namespace OpenFAST.util
{
	public sealed class RecordingInputStream: System.IO.Stream
	{
		public sbyte[] Buffer
		{
			get
			{
				sbyte[] b = new sbyte[index];
				Array.Copy(buffer, 0, b, 0, index);
				
				return b;
			}
			
		}
		private sbyte[] buffer = new sbyte[1024];
		private int index = 0;
		private System.IO.Stream in_Renamed;
		
		public RecordingInputStream(System.IO.Stream inputStream)
		{
			this.in_Renamed = inputStream;
		}
		
		
		public override int ReadByte()
		{
			int read = in_Renamed.ReadByte();
			buffer[index++] = (sbyte) read;
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

		public override System.Int64 Seek(System.Int64 offset, System.IO.SeekOrigin origin)
		{
			return 0;
		}

		public override void  SetLength(System.Int64 value)
		{
		}

		public override System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
		{
			return 0;
		}

		public override void  Write(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
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