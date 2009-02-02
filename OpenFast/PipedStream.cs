using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpenFAST
{
    public sealed class PipedStream : Stream
    {
        long position = 0;
        Stream baseStream;
        public PipedStream()
        {
            baseStream = new MemoryStream();
        }
        public PipedStream(Stream stream)
        {
            baseStream = stream;
        }


        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get { return baseStream.Length; }
        }

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (baseStream)
            {
                long oldPosition = baseStream.Position;
                baseStream.Position = position;
                int bytesRead = baseStream.Read(buffer, offset, count);
                position = position + bytesRead;
                baseStream.Position = oldPosition;
                return bytesRead;
            }

        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetLength(long value)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Write(byte[] buffer)
        {
            this.Write(buffer, 0, buffer.Length);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (baseStream)
            {
                long oldPosition = baseStream.Position;
                baseStream.Position = position;
                baseStream.Write(buffer, offset, count);
                position = position + count;
                baseStream.Position = oldPosition;
            }
        }
    }
}
