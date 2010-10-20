using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFAST
{
    public class UnsupportedOperationException:Exception
    {
        public UnsupportedOperationException():base("Type is unsupported")
        {
            
        }
    }
}
