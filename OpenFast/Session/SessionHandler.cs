using System;

namespace OpenFAST.Session
{
	public struct SessionHandler_Fields{
		public readonly static SessionHandler NULL;
		static SessionHandler_Fields()
		{
			NULL = new NULLSessionHandler();
		}
	}
	public class NULLSessionHandler : SessionHandler
	{
		public virtual void  NewSession(Session session)
		{
		}
	}
	public interface SessionHandler
	{
		void  NewSession(Session session);
	}
}