using System;

namespace OpenFAST.Session
{
	public struct SessionListener_Fields{
		public readonly static SessionListener NULL;
		static SessionListener_Fields()
		{
			NULL = new NULLSessionListener();
		}
	}
	public class NULLSessionListener : SessionListener
	{
		public virtual void  OnClose()
		{
		}
	}
	public interface SessionListener
	{
		void  OnClose();
	}
}