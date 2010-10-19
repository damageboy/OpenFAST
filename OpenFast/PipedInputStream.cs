using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpenFAST
{
    public sealed class PipedInputStream : StreamReader
    {
        public PipedInputStream()
            : base(new MemoryStream())
        {
        }
        public PipedInputStream(Stream stream)
            : base(stream)
        {
        }
    }
}
