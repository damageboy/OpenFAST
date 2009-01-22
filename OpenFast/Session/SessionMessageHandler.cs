using System;
using Message = OpenFAST.Message;

namespace OpenFAST.Session
{
	public interface SessionMessageHandler
	{
		void  HandleMessage(Session session, Message message);
	}
}