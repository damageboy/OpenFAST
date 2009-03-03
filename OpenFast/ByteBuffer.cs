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
