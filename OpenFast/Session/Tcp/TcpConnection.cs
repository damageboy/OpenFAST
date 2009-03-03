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
using Connection = OpenFAST.Session.Connection;

namespace OpenFAST.Session.Tcp
{
	sealed class TcpConnection : Connection
	{
        System.IO.StreamReader in_stream ;
        System.IO.StreamWriter out_stream ;
        public System.IO.StreamReader InputStream
		{
			get
			{
                return in_stream;
			}
			
		}
		public System.IO.StreamWriter OutputStream
		{
			get
			{
                return out_stream;
			}
			
		}

        private System.Net.Sockets.TcpClient socket;
		
		public TcpConnection(System.Net.Sockets.TcpClient socket)
		{
			if (socket == null)
				throw new System.NullReferenceException();
			this.socket = socket;
            in_stream = new System.IO.StreamReader(socket.GetStream());
            out_stream = new System.IO.StreamWriter(socket.GetStream());

		}
		public void  Close()
		{
			try
			{
				socket.Close();
			}
			catch (System.IO.IOException)
			{
			}
		}
	}
}