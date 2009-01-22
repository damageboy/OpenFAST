using System;
using ErrorCode = OpenFAST.Error.ErrorCode;
using ErrorHandler = OpenFAST.Error.ErrorHandler;

namespace OpenFAST
{
	
	public sealed class Global
	{
		public static ErrorHandler ErrorHandler
		{
			set
			{
				if (value == null)
				{
					throw new System.NullReferenceException();
				}
				
				Global.errorHandler = value;
			}
			
		}
		private static ErrorHandler errorHandler = OpenFAST.Error.ErrorHandler_Fields.DEFAULT;
		private static int currentImplicitId = (int) ((System.DateTime.Now.Ticks - 621355968000000000) / 10000 % 10000);
		
		public static void  HandleError(ErrorCode error, string message)
		{
			errorHandler.Error(error, message);
		}
		
		public static void  HandleError(ErrorCode error, string message, System.Exception source)
		{
			errorHandler.Error(error, message, source);
		}
		
		public static QName CreateImplicitName(QName name)
		{
			return new QName(name + "@" + currentImplicitId++, name.Namespace);
		}
		
		private Global()
		{
		}
	}
}