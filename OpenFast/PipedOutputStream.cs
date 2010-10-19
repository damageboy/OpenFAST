using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpenFAST
{
    public sealed class PipedOutputStream:StreamWriter
    {
        public PipedOutputStream()
            : base(new MemoryStream())
        {
        }
        public PipedOutputStream(Stream stream)
            : base(stream)
        {
        }
    }
}
