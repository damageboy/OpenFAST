using System;
using Message = OpenFAST.Message;

namespace OpenFAST.Session
{
	public interface MessageListener
	{
		void  OnMessage(Message message);
	}
}