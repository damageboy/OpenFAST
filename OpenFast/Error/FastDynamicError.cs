using System;

namespace OpenFAST.Error
{
	[Serializable]
	public class FastDynamicError:System.SystemException
	{
		virtual public ErrorCode Error
		{
			get
			{
				return error;
			}
			
		}
		private const long serialVersionUID = 2L;
		private ErrorCode error;
		
		public FastDynamicError(ErrorCode error):base(error.ShortName + ": " + error.Description)
		{
			this.error = error;
		}
	}
}