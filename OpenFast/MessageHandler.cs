using System;
using Coder = OpenFAST.Codec.Coder;

namespace OpenFAST
{
	public interface MessageHandler
	{
		void  HandleMessage(Message readMessage, Context context, Coder coder);
	}
}