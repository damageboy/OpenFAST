using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpenFAST
{
    public class ByteBuffer : MemoryStream
    {
        internal static ByteBuffer Allocate(int BUFFER_SIZE)
        {
            ByteBuffer buff = new ByteBuffer();
            buff.SetLength(BUFFER_SIZE);
            return buff;
        }

        public void flip()
        {

            throw new Exception("The method or operation is not implemented.");
        }

        public bool hasRemaining()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int get_Renamed()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public byte[] array()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void limit(int p)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
