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
using System.IO;

namespace OpenFAST.Session
{
    public class LocalConnection : IConnection
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        public LocalConnection()
        {
            _reader = new StreamReader(new MemoryStream());
            _writer = new StreamWriter(new MemoryStream());
        }

        public LocalConnection(IConnection localConnection)
        {
            try
            {
                _reader = new StreamReader(localConnection.OutputStream.BaseStream);
                _writer = new StreamWriter(localConnection.InputStream.BaseStream);
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }

        #region Connection Members

        public virtual StreamReader InputStream
        {
            get { return _reader; }
        }

        public virtual StreamWriter OutputStream
        {
            get { return _writer; }
        }

        public virtual void Close()
        {
            try
            {
                _reader.Close();
            }
            catch (IOException)
            {
            }
            try
            {
                _writer.Close();
            }
            catch (IOException)
            {
            }
        }

        #endregion
    }
}