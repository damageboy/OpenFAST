using System;

namespace OpenFAST
{
	/// <summary>
	/// Quick name.
    /// Provides efficient name searching in template registry. 
	/// </summary>
	[Serializable]
	public sealed class QName
	{
		public string Namespace
		{
			get
			{
				return namespace_Renamed;
			}
			
		}

        //must be less then or equal to 31 bytes
		public string Name
		{
			get
			{
				return name;
			}
			
		}
		private const long serialVersionUID = 1L;
		
		public static readonly QName NULL = new QName("", "");
		
		private string namespace_Renamed;
		
		private string name;
		
		public QName(string name):this(name, "")
		{
		}
		
		public QName(string name, string namespace_Renamed)
		{
			if (name == null)
				throw new System.NullReferenceException();
			this.name = name;
			this.namespace_Renamed = namespace_Renamed == null?"":namespace_Renamed;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (obj == this)
				return true;
			if (obj == null || obj.GetType() != this.GetType())
				return false;
			QName other = (QName) obj;
			return other.namespace_Renamed.Equals(namespace_Renamed) && other.name.Equals(name);
		}
		
		public override int GetHashCode()//efficient algo to quick search
		{
			return name.GetHashCode() + 31 * namespace_Renamed.GetHashCode();
		}
		
		public override string ToString()
		{
			if (namespace_Renamed.Equals(""))
				return name;
			return name + "[" + namespace_Renamed + "]";
		}
	}
}