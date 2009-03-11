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
using RecordingInputStream = OpenFAST.util.RecordingInputStream;
using RecordingOutputStream = OpenFAST.util.RecordingOutputStream;

namespace OpenFAST.Session
{
	public class RecordingEndpoint : Endpoint
	{
		virtual public ConnectionListener ConnectionListener
		{
			set
			{
				underlyingEndpoint.ConnectionListener = value;
			}
			
		}
		
		private readonly Endpoint underlyingEndpoint;
		
		public RecordingEndpoint(Endpoint endpoint)
		{
			underlyingEndpoint = endpoint;
		}
		
		public virtual void  Accept()
		{
			underlyingEndpoint.Accept();
		}
		
		public virtual Connection Connect()
		{
			Connection connection = underlyingEndpoint.Connect();
			Connection connectionWrapper = new RecordingConnection(connection);
			return connectionWrapper;
		}

		private sealed class RecordingConnection : Connection
		{
		    readonly System.IO.StreamReader in_stream;
		    readonly System.IO.StreamWriter out_stream;
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

			private readonly RecordingInputStream recordingInputStream;
			private readonly RecordingOutputStream recordingOutputStream;
			
			internal RecordingConnection(Connection connection)
			{
				try
				{
					recordingInputStream = new RecordingInputStream(connection.InputStream.BaseStream);
					recordingOutputStream = new RecordingOutputStream(connection.OutputStream.BaseStream);
                    in_stream = new System.IO.StreamReader(recordingInputStream);
                    out_stream = new System.IO.StreamWriter(recordingOutputStream);
				}
				catch (System.IO.IOException e)
				{
					throw new RuntimeException(e);
				}
			}
			
			public void  Close()
			{
			}
		}
		
		public virtual void  Close()
		{
			underlyingEndpoint.Close();
		}
	}
}