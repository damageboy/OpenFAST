using System;

namespace OpenFAST.Error
{
	[Serializable]
	public class FastException:System.SystemException
	{
		virtual public ErrorCode Code
		{
			get
			{
				return code;
			}
			
		}
		private const long serialVersionUID = 2L;
		private ErrorCode code;
		
		public FastException(string message, ErrorCode code):base(message)
		{
			this.code = code;
		}
		public FastException(string message, ErrorCode code, System.Exception cause):base(message, cause)
		{
			this.code = code;
		}
	}
}