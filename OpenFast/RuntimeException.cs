using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFAST
{
    public class RuntimeException:Exception
    {
        public RuntimeException(Exception e)
        { 
        }
    }
}
