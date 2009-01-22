using System;

namespace OpenFAST.Session
{
	public interface Client
	{
		string Name
		{
			get;
			
		}
		string VendorId
		{
			get;
			
		}
	}
}