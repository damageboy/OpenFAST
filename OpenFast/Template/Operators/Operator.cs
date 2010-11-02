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
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System;
using System.Collections.Generic;
using OpenFAST.Error;
using OpenFAST.Template.Types;

namespace OpenFAST.Template.Operators
{
    public class Operator
    {
        private static readonly Dictionary<string, Operator> OperatorNameMap = new Dictionary<string, Operator>();

        public static readonly Operator None;
        public static readonly Operator Constant;
        public static readonly Operator Default;
        public static readonly Operator Copy;
        public static readonly Operator Increment = new Operator("increment");
        public static readonly Operator Delta;
        public static readonly Operator Tail = new Operator("tail");

        private readonly string _name;

        static Operator()
        {
            None = new NoneOperator("none");
            Constant = new ConstantOperator("constant");
            Default = new DefaultOperator("default");
            Copy = new CopyOperator("copy");
            Delta = new DeltaOperator("delta");
        }

        public Operator(string name)
        {
            _name = name;
            OperatorNameMap[name] = this;
        }

        public string Name
        {
            get { return _name; }
        }

        public static Operator GetOperator(string name)
        {
            Operator value;
            if (OperatorNameMap.TryGetValue(name, out value))
                return value;

            throw new ArgumentOutOfRangeException("name", name, "Operator does not exist");
        }

        public virtual OperatorCodec GetCodec(FastType type)
        {
            return OperatorCodec.GetCodec(this, type);
        }

        public override string ToString()
        {
            return _name;
        }

        public virtual bool ShouldStoreValue(ScalarValue value)
        {
            return true;
        }

        public virtual void Validate(Scalar scalar)
        {
        }

        public override bool Equals(object other) //POINTP
        {
            if (ReferenceEquals(other, this))
                return true;
            if (ReferenceEquals(other, null))
                    return false;
            var t = other as Operator;
            if (t == null)
                return false;
            return t.Equals(_name);
        }

        internal bool Equals(Operator other)
        {
            return _name.Equals(other._name);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        #region Nested type: ConstantOperator

        public sealed class ConstantOperator : Operator
        {
            internal ConstantOperator(string name) : base(name)
            {
            }

            public override void Validate(Scalar scalar)
            {
                if (scalar.DefaultValue.IsUndefined)
                {
                    Global.ErrorHandler.OnError(null, StaticError.NoInitialValueForConst,
                                                "The field {0} must have a default value defined.", scalar);
                }
            }

            public override bool ShouldStoreValue(ScalarValue value)
            {
                return false;
            }
        }

        #endregion

        #region Nested type: CopyOperator

        public sealed class CopyOperator : Operator
        {
            internal CopyOperator(string name) : base(name)
            {
            }

            public override OperatorCodec GetCodec(FastType type)
            {
                return OperatorCodec.CopyAll;
            }
        }

        #endregion

        #region Nested type: DefaultOperator

        public sealed class DefaultOperator : Operator
        {
            internal DefaultOperator(string name) : base(name)
            {
            }

            public override void Validate(Scalar scalar)
            {
                if (!scalar.IsOptional && scalar.DefaultValue.IsUndefined)
                {
                    Global.ErrorHandler.OnError(null, StaticError.NoInitvalMndtryDfalt,
                                                "The field {0} must have a default value defined.", scalar);
                }
            }

            public override bool ShouldStoreValue(ScalarValue value)
            {
                return value != null;
            }
        }

        #endregion

        #region Nested type: DeltaOperator

        public sealed class DeltaOperator : Operator
        {
            internal DeltaOperator(string name) : base(name)
            {
            }

            public override bool ShouldStoreValue(ScalarValue value)
            {
                return value != null;
            }
        }

        #endregion

        #region Nested type: NoneOperator

        public sealed class NoneOperator : Operator
        {
            internal NoneOperator(string name) : base(name)
            {
            }

            public override bool ShouldStoreValue(ScalarValue value)
            {
                return false;
            }
        }

        #endregion
    }
}