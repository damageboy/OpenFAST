using System;

namespace OpenFAST.Session
{
	public interface Connection
	{
        System.IO.StreamReader InputStream
		{
			get;
			
		}
		System.IO.StreamWriter OutputStream
		{
			get;
			
		}
		void  Close();
	}
}