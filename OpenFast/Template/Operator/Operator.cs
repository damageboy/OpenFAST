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
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace openfast.Template.Operator
{
    [Serializable]
    public class Operator
    {
        [Serializable]
        public sealed class NoneOperator:Operator
        {
            internal NoneOperator(string Param1):base(Param1)
            {
            }

            public override bool UsesDictionary()
            {
                return false;
            }
			
            public override bool ShouldStoreValue(ScalarValue value_Renamed)
            {
                return false;
            }
        }
        [Serializable]
        public sealed class ConstantOperator:Operator
        {
            internal ConstantOperator(string Param1):base(Param1)
            {
            }

            public override void  Validate(Scalar scalar)
            {
                if (scalar.DefaultValue.Undefined)
                {
                    Global.HandleError(OpenFAST.Error.FastConstants.S4_NO_INITIAL_VALUE_FOR_CONST, "The field " + scalar + " must have a default value defined.");
                }
            }
			
            public override bool ShouldStoreValue(ScalarValue value_Renamed)
            {
                return false;
            }
			
            public override bool UsesDictionary()
            {
                return false;
            }
        }
        [Serializable]
        public sealed class DefaultOperator:Operator
        {
            internal DefaultOperator(string Param1):base(Param1)
            {
            }

            public override void  Validate(Scalar scalar)
            {
                if (!scalar.Optional && scalar.DefaultValue.Undefined)
                {
                    Global.HandleError(OpenFAST.Error.FastConstants.S5_NO_INITVAL_MNDTRY_DFALT, "The field " + scalar + " must have a default value defined.");
                }
            }
			
            public override bool ShouldStoreValue(ScalarValue value_Renamed)
            {
                return value_Renamed != null;
            }
        }
        [Serializable]
        public sealed class CopyOperator:Operator
        {
            internal CopyOperator(string Param1):base(Param1)
            {
            }

            public override OperatorCodec GetCodec(FASTType type)
            {
                return OperatorCodec.COPY_ALL;
            }
        }
        [Serializable]
        public sealed class DeltaOperator:Operator
        {
            internal DeltaOperator(string Param1):base(Param1)
            {
            }

            public override bool ShouldStoreValue(ScalarValue value_Renamed)
            {
                return value_Renamed != null;
            }
        }
        virtual public string Name
        {
            get
            {
                return name;
            }
			
        }

        private static readonly System.Collections.IDictionary OPERATOR_NAME_MAP = new System.Collections.Hashtable();
		
        private readonly string name;
		
        public static readonly Operator NONE;
		
        public static readonly Operator CONSTANT;
		
        public static readonly Operator DEFAULT;
		
        public static readonly Operator COPY;
		
        public static readonly Operator INCREMENT = new Operator("increment");
		
        public static readonly Operator DELTA;
		
        public static readonly Operator TAIL = new Operator("tail");
		
        public Operator(string name)
        {
            this.name = name;
            OPERATOR_NAME_MAP[name] = this;
        }
		
        public static Operator GetOperator(string name)
        {
            if (!OPERATOR_NAME_MAP.Contains(name))
                throw new ArgumentException("The operator \"" + name + "\" does not exist.");
            return (Operator) OPERATOR_NAME_MAP[name];
        }
		
        public virtual OperatorCodec GetCodec(FASTType type)
        {
            return OperatorCodec.GetCodec(this, type);
        }
		
        public override string ToString()
        {
            return name;
        }
		
        public virtual bool ShouldStoreValue(ScalarValue value_Renamed)
        {
            return true;
        }
		
        public virtual void  Validate(Scalar scalar)
        {
        }

        public override bool Equals(object other)//POINTP
        {
            if (other == this)
                return true;
            if (other == null || !(other is Operator))
                return false;
            return Equals((Operator) other);
        }
		
        internal bool Equals(Operator other)
        {
            return name.Equals(other.name);
        }
		
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
		
        public virtual bool UsesDictionary()
        {
            return true;
        }
        static Operator()
        {
            NONE = new NoneOperator("none");
            CONSTANT = new ConstantOperator("constant");
            DEFAULT = new DefaultOperator("default");
            COPY = new CopyOperator("copy");
            DELTA = new DeltaOperator("delta");
        }
    }
}