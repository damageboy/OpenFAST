using System;

namespace OpenFAST.Error
{
	
	public struct ErrorHandler_Fields{
		public readonly static ErrorHandler DEFAULT;
		public readonly static ErrorHandler NULL;
		static ErrorHandler_Fields()
		{
			DEFAULT = new DefaultErrorHandler();
			NULL = new NullErrorHandler();
		}
	}
	public class DefaultErrorHandler : ErrorHandler
	{
		public virtual void  Error(ErrorCode code, string message)
		{
			code.ThrowException(message);
		}
		
		public virtual void  Error(ErrorCode code, string message, System.Exception t)
		{
			throw new FastException(message, code, t);
		}
	}
	public class NullErrorHandler : ErrorHandler
	{
		public virtual void  Error(ErrorCode code, string message)
		{
		}
		public virtual void  Error(ErrorCode code, string message, System.Exception t)
		{
		}
	}
	public interface ErrorHandler
	{
		void  Error(ErrorCode code, string message);
		void  Error(ErrorCode code, string message, System.Exception t);
	}
}