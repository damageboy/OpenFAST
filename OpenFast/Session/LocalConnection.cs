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
using OpenFAST;
using System.IO;

namespace OpenFAST.Session
{
	public class LocalConnection : Connection
	{
		virtual public System.IO.StreamReader InputStream
		{
			get
			{
				return in_Renamed;
			}
			
		}
		virtual public System.IO.StreamWriter OutputStream
		{
			get
			{
				return out_Renamed;
			}
			
		}
		
		private System.IO.StreamReader in_Renamed;
		private System.IO.StreamWriter out_Renamed;
		
        public LocalConnection(LocalEndpoint remote, LocalEndpoint local)
        {
            this.in_Renamed = new System.IO.StreamReader(new MemoryStream());//PipedInputStream
            this.out_Renamed = new System.IO.StreamWriter(new MemoryStream());//PipedOutputStream
        }
		
		public LocalConnection(LocalConnection localConnection)
		{
			try
			{
				this.in_Renamed = new System.IO.StreamReader(localConnection.OutputStream.BaseStream);
				this.out_Renamed = new System.IO.StreamWriter(localConnection.InputStream.BaseStream);
			}
			catch (System.IO.IOException e)
			{
				throw new RuntimeException(e);
			}
		}
		
		public virtual void  Close()
		{
			try
			{
				in_Renamed.Close();
			}
			catch (System.IO.IOException)
			{
			}
			try
			{
				out_Renamed.Close();
			}
			catch (System.IO.IOException)
			{
			}
		}
	}
}