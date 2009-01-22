using System;

namespace OpenFAST.Session
{
	
	public struct SessionFactory_Fields{
		public readonly static SessionFactory NULL;
		static SessionFactory_Fields()
		{
			NULL = new NULLSessionFactory();
		}
	}
	public class NULLSessionFactory : SessionFactory
	{
		virtual public Session Session
		{
			get
			{
				return null;
			}
			
		}
		public virtual void  Close()
		{
		}
		
		public virtual Client GetClient(string serverName)
		{
			return null;
		}
	}
	public interface SessionFactory
	{
		Session Session
		{
			get;
			
		}
		
		Client GetClient(string serverName);
		
		void  Close();
	}
}