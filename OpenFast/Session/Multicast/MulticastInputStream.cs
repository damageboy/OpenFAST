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
using OpenFAST;

namespace OpenFAST.Session.Multicast
{
	public sealed class MulticastInputStream:System.IO.Stream
	{
		private const int BUFFER_SIZE = 4 * 1024 * 1024;
		private System.Net.Sockets.UdpClient socket;
		private ByteBuffer buffer;//SHARIQ
		
		public MulticastInputStream(System.Net.Sockets.UdpClient socket)
		{
			this.socket = socket;
			this.buffer = ByteBuffer.Allocate(BUFFER_SIZE);
			buffer.flip();//SHARIQ
		}
		
		public override int ReadByte()
		{
			if (!socket.Client.Connected)
				return - 1;
			if (!buffer.hasRemaining())
			{
				buffer.flip();//SHARIQ
				SupportClass.PacketSupport packet = new DatagramPacket(buffer.array(), buffer.array().Length);
				SupportClass.UdpClientSupport.Receive(socket, out packet);
				buffer.limit(packet.Length);//SHARIQ
			}
            return buffer.get_Renamed();//SHARIQ
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