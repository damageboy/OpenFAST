using System;

namespace OpenFAST.Session
{
	[Serializable]
	public class FastConnectionException:System.Exception
	{
		private const long serialVersionUID = 1L;
	
		public FastConnectionException(System.Exception t):base(t.ToString())
		{
		}
		
		public FastConnectionException(string message):base(message)
		{
		}
	}
}