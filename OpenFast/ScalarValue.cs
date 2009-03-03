/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>

*/
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
		virtual public byte[] Bytes
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
		
		public virtual byte ToByte()
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