using System;
using RecordingInputStream = OpenFAST.util.RecordingInputStream;
using RecordingOutputStream = OpenFAST.util.RecordingOutputStream;
using OpenFAST;

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
		
		private Endpoint underlyingEndpoint;
		
		public RecordingEndpoint(Endpoint endpoint)
		{
			this.underlyingEndpoint = endpoint;
		}
		
		public virtual void  Accept()
		{
			underlyingEndpoint.Accept();
		}
		
		public virtual Connection Connect()
		{
			Connection connection = underlyingEndpoint.Connect();
			Connection connectionWrapper = new RecordingConnection(this, connection);
			return connectionWrapper;
		}

		private sealed class RecordingConnection : Connection
		{
			private void  InitBlock(RecordingEndpoint enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private RecordingEndpoint enclosingInstance;
            System.IO.StreamReader in_stream;
            System.IO.StreamWriter out_stream;
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
			public RecordingEndpoint Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			private RecordingInputStream recordingInputStream;
			private RecordingOutputStream recordingOutputStream;
			
			internal RecordingConnection(RecordingEndpoint enclosingInstance, Connection connection)
			{
				InitBlock(enclosingInstance);
				try
				{
					this.recordingInputStream = new RecordingInputStream(connection.InputStream.BaseStream);
					this.recordingOutputStream = new RecordingOutputStream(connection.OutputStream.BaseStream);
                    in_stream = new System.IO.StreamReader(this.recordingInputStream);
                    out_stream = new System.IO.StreamWriter(this.recordingOutputStream);
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