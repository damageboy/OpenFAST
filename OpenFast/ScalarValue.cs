using System;

namespace OpenFAST
{
	[Serializable]
	public class ScalarValue : FieldValue
	{
		[Serializable]
		public sealed class UndefinedScalarValue:ScalarValue
		{
			override public bool Undefined
			{
				get
				{
					return true;
				}
				
			}
			private new const long serialVersionUID = 1L;
			
			public override string ToString()
			{
				return "UNDEFINED";
			}
		}
		[Serializable]
		public sealed class NullScalarValue:ScalarValue
		{
			override public bool Null
			{
				get
				{
					return true;
				}
				
			}
			private new const long serialVersionUID = 1L;
			
			public override string ToString()
			{
				return "NULL";
			}
		}

		virtual public bool Undefined
		{
			get
			{
				return false;
			}
			
		}
		virtual public bool Null
		{
			get
			{
				return false;
			}
			
		}
		virtual public sbyte[] Bytes
		{
			get
			{
				throw new System.NotSupportedException();
			}
			
		}
		private const long serialVersionUID = 1L;
		
		public static readonly ScalarValue UNDEFINED;
		
		public static readonly ScalarValue NULL;
		
		public virtual bool EqualsValue(string defaultValue)
		{
			return false;
		}
		
		public virtual FieldValue Copy()
		{
			return this; // immutable objects don't need actual copies.
		}
		
		public virtual sbyte ToByte()
		{
			throw new System.NotSupportedException();
		}
		
		public virtual short ToShort()
		{
			throw new System.NotSupportedException();
		}
		
		public virtual int ToInt()
		{
			throw new System.NotSupportedException();
		}
		
		public virtual long ToLong()
		{
			throw new System.NotSupportedException();
		}
		
		public override string ToString()
		{
			throw new System.NotSupportedException();
		}
		
		public virtual double ToDouble()
		{
			throw new System.NotSupportedException();
		}

        public virtual System.Decimal ToBigDecimal()
		{
			throw new System.NotSupportedException();
		}
		
        static ScalarValue()
		{
			UNDEFINED = new UndefinedScalarValue();
			NULL = new NullScalarValue();
		}
	}
}