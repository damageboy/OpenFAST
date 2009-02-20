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