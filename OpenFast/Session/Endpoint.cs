using System;

namespace OpenFAST.Session
{
	public interface Endpoint
	{
		ConnectionListener ConnectionListener
		{
			set;
			
		}
		Connection Connect();
		void  Accept();
		void  Close();
	}
}