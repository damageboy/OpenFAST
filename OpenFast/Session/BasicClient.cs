using System;

namespace OpenFAST.Session
{
	public sealed class BasicClient : Client
	{
		public string Name
		{
			get
			{
				return name;
			}
			
		}
		public string VendorId
		{
			get
			{
				return vendorId;
			}
			
		}
		
		private string name;

        private string vendorId;
		
		public BasicClient(string clientName, string vendorId)
		{
			this.name = clientName;
			this.vendorId = vendorId;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (obj == this)
				return true;
			if (obj == null || !(obj is BasicClient))
				return false;
			return ((BasicClient) obj).name.Equals(name);
		}
		
		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
	}
}