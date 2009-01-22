using System;

namespace OpenFAST.Error
{
	public sealed class ErrorType
	{
		public string Name
		{
			get
			{
				return name;
			}
			
		}
		private string name;
		
		public ErrorType(string name)
		{
			this.name = name;
		}
	}
}