using System;
using Message = OpenFAST.Message;

namespace OpenFAST.Error
{
	public sealed class ErrorCode
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
		public FastAlertSeverity Severity
		{
			get
			{
				return severity;
			}
			
		}
		public ErrorType Type
		{
			get
			{
				return type;
			}
			
		}
		private static readonly System.Collections.IDictionary ALERT_CODES = new System.Collections.Hashtable();
		private int code;
		private string shortName;
		private string description;
		private FastAlertSeverity severity;
		private ErrorType type;
		
		public ErrorCode(ErrorType type, int code, string shortName, string description, FastAlertSeverity severity)
		{
			ALERT_CODES[(System.Int32) code] = this;
			this.type = type;
			this.code = code;
			this.shortName = shortName;
			this.description = description;
			this.severity = severity;
		}
		
		public void  ThrowException(string message)
		{
			throw new FastException(message, this);
		}
		
		public static ErrorCode GetAlertCode(Message alertMsg)
		{
			return (ErrorCode) ALERT_CODES[(System.Int32) alertMsg.GetInt(2)];
		}
		
		public override string ToString()
		{
			return shortName + ": " + description;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (obj == this)
				return true;
			if (obj == null || !(obj is ErrorCode))
				return false;
			ErrorCode other = (ErrorCode) obj;
			return other.code == this.code && other.Type.Equals(this.Type);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}