using System;

namespace OpenFAST
{
	
	public struct MessageBlockReader_Fields{
		public readonly static MessageBlockReader NULL;
		static MessageBlockReader_Fields()
		{
			NULL = new NullMessageBlockReader();
		}
	}
	public sealed class NullMessageBlockReader : MessageBlockReader
	{
		public bool ReadBlock(System.IO.Stream in_Renamed)
		{
			return true;
		}
		
		public void  MessageRead(System.IO.Stream in_Renamed, Message message)
		{
		}
	}
	public interface MessageBlockReader
	{
		bool ReadBlock(System.IO.Stream in_Renamed);
		void  MessageRead(System.IO.Stream in_Renamed, Message message);
	}
}