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
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System;
using System.IO;
using System.Net.Sockets;

namespace OpenFAST.Session.Multicast
{
    public sealed class MulticastInputStream : Stream
    {
        private const int BufferSize = 4*1024*1024;
        private readonly ByteBuffer _buffer; //SHARIQ
        private readonly UdpClient _socket;

        public MulticastInputStream(UdpClient socket)
        {
            _socket = socket;
            _buffer = ByteBuffer.Allocate(BufferSize);
            _buffer.Flip(); //SHARIQ
        }

        public override Boolean CanRead
        {
            get { return false; }
        }

        public override Boolean CanSeek
        {
            get { return false; }
        }

        public override Boolean CanWrite
        {
            get { return false; }
        }

        public override Int64 Length
        {
            get { return 0; }
        }

        public override Int64 Position
        {
            get { return 0; }

            set { }
        }

        public override int ReadByte()
        {
            if (!_socket.Client.Connected)
                return - 1;
            if (!_buffer.HasRemaining())
            {
                _buffer.Flip(); //SHARIQ
#warning Bug? why allocate DatagramPacket and than pass it in as an out param
                SupportClass.PacketSupport packet = new DatagramPacket(_buffer.Array(), _buffer.Array().Length);
                SupportClass.UdpClientSupport.Receive(_socket, out packet);
                _buffer.Limit(packet.Length); //SHARIQ
            }
            return _buffer.Get(); //SHARIQ
        }

        public override void Flush()
        {
        }

        public override Int64 Seek(Int64 offset, SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(Int64 value)
        {
        }

        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
        {
            return 0;
        }

        public override void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
        }
    }
}