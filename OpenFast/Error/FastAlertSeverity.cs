using System;

namespace OpenFAST.Error
{
	public sealed class FastAlertSeverity
	{
		public int Code
		{
			get
			{
				return code;
			}
			
		}
		public string Description
		{
			get
			{
				return description;
			}
			
		}
		public string ShortName
		{
			get
			{
				return shortName;
			}
			
		}
		public static readonly FastAlertSeverity FATAL = new FastAlertSeverity(1, "FATAL", "Fatal");
		public static readonly FastAlertSeverity ERROR = new FastAlertSeverity(2, "ERROR", "Error");
		public static readonly FastAlertSeverity WARN = new FastAlertSeverity(3, "WARN", "Warning");
		public static readonly FastAlertSeverity INFO = new FastAlertSeverity(4, "INFO", "Information");
		private int code;
		private string shortName;
		private string description;
		
		public FastAlertSeverity(int code, string shortName, string description)
		{
			this.code = code;
			this.shortName = shortName;
			this.description = description;
		}
	}
}