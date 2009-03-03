using System;

namespace OpenFAST.util
{
	
	
	public sealed class Key
	{
		private System.Object[] keys;
		
		public Key(System.Object key1, System.Object key2):this(new System.Object[]{key1, key2})
		{
		}
		
		public Key(System.Object key1, System.Object key2, System.Object key3):this(new System.Object[]{key1, key2, key3})
		{
		}
		
		public Key(System.Object[] keys)
		{
			this.keys = keys;
			CheckNull();
		}
		
		private void  CheckNull()
		{
			for (int i = 0; i < keys.Length; i++)
				if (keys[i] == null)
					throw new System.NullReferenceException();
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (this == obj)
				return true;
			if ((obj == null) || !(obj is Key))
				return false;
			
			Key other = ((Key) obj);
			if (other.keys.Length != keys.Length)
				return false;
			for (int i = 0; i < keys.Length; i++)
				if (!other.keys[i].Equals(keys[i]))
					return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			for (int i = 0; i < keys.Length; i++)
				hashCode += keys[i].GetHashCode() * (37 ^ i);
			return hashCode;
		}
		
		public override string ToString()
		{
            return "";
		}
	}
}