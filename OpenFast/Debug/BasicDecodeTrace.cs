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
using System.Text;
using OpenFAST.Template;

namespace OpenFAST.Debug
{
    public sealed class BasicDecodeTrace : ITrace
    {
        private string _indent = "";

        private StreamWriter _writer = new StreamWriter(Console.OpenStandardOutput(), Encoding.Default);

        public StreamWriter Writer
        {
            set { _writer = value; }
        }

        #region ITrace Members

        public void GroupStart(Group group)
        {
            Print(group);
            MoveDown();
        }

        public void GroupEnd()
        {
            MoveUp();
        }

        public void Field(Field field, IFieldValue value, IFieldValue decodedValue, byte[] encoding, int pmapIndex)
        {
            var scalarDecode = new StringBuilder();
            scalarDecode.Append(field.Name).Append(": ");
            scalarDecode.Append(ByteUtil.ConvertByteArrayToBitString(encoding));
            scalarDecode.Append(" -> ").Append(value).Append('(').Append(decodedValue).Append(')');
            Print(scalarDecode);
        }

        public void Pmap(byte[] bytes)
        {
            Print("PMAP: " + ByteUtil.ConvertByteArrayToBitString(bytes));
        }

        #endregion

        private void MoveDown()
        {
            _indent += "  ";
        }

        private void MoveUp()
        {
            _indent = _indent.Substring(0, (_indent.Length - 2) - (0));
        }

        private void Print(object obj)
        {
            _writer.Write(_indent);
            _writer.WriteLine(obj);
        }
    }
}