using System;
using Message = OpenFAST.Message;

namespace OpenFAST.Session
{
	public interface MessageListener
	{
		void  OnMessage(Session session, Message message);
	}
}