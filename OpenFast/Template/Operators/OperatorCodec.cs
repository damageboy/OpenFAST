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
using OpenFAST.Utility;

namespace OpenFAST.Template.Operators
{
    public abstract class OperatorCodec
    {
        private static readonly Dictionary<Tuple<Operator, FastType>, OperatorCodec> OperatorMap =
            new Dictionary<Tuple<Operator, FastType>, OperatorCodec>();

        protected internal static readonly OperatorCodec NoneAll =
            new NoneOperatorCodec(Operator.None, FastType.AllTypes());

        protected internal static readonly OperatorCodec ConstantAll
            = new ConstantOperatorCodec(Operator.Constant, FastType.AllTypes());

        protected internal static readonly OperatorCodec DefaultAll
            = new DefaultOperatorCodec(Operator.Default, FastType.AllTypes());

        protected internal static readonly OperatorCodec CopyAll
            = new CopyOperatorCodec();

        protected internal static readonly OperatorCodec IncrementInteger
            = new IncrementIntegerOperatorCodec(Operator.Increment, FastType.IntegerTypes);

        protected internal static readonly OperatorCodec DeltaInteger
            = new DeltaIntegerOperatorCodec(Operator.Delta, FastType.IntegerTypes);

        protected internal static readonly OperatorCodec DeltaString
            = new DeltaStringOperatorCodec();

        protected internal static readonly OperatorCodec DeltaDecimal
            = new DeltaDecimalOperatorCodec();

        protected internal static readonly OperatorCodec Tail =
            new TailOperatorCodec(Operator.Tail,
                                  new[] {FastType.Ascii, FastType.String, FastType.Unicode, FastType.ByteVector});

        private readonly Operator _operator;

        protected internal OperatorCodec(Operator op, IEnumerable<FastType> types)
        {
            _operator = op;
            foreach (FastType t in types)
                OperatorMap[Tuple.Create(op, t)] = this;
        }

        public Operator Operator
        {
            get { return _operator; }
        }

        public virtual bool ShouldDecodeType
        {
            get { return true; }
        }

        public virtual bool DecodeNewValueNeedsPrevious
        {
            get { return true; }
        }

        public virtual bool DecodeEmptyValueNeedsPrevious
        {
            get { return true; }
        }

        public static OperatorCodec GetCodec(Operator op, FastType type)
        {
            Tuple<Operator, FastType> key = Tuple.Create(op, type);

            OperatorCodec codec;
            if (OperatorMap.TryGetValue(key, out codec))
                return codec;

            Global.ErrorHandler.OnError(null, StaticError.OperatorTypeIncomp,
                                        "The operator '{0}' is not compatible with type '{1}'", op, type);
            throw new ArgumentOutOfRangeException("op" + ",type", key, "Not found");
        }

        public abstract ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field);

        public abstract ScalarValue DecodeValue(ScalarValue newValue, ScalarValue priorValue, Scalar field);

        public virtual bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue)
        {
            return encoding.Length != 0;
        }

        public abstract ScalarValue DecodeEmptyValue(ScalarValue priorValue, Scalar field);

        public virtual bool UsesPresenceMapBit(bool optional)
        {
            return true;
        }

        public virtual ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar scalar,
                                                    BitVectorBuilder presenceMapBuilder)
        {
            ScalarValue valueToEncode = GetValueToEncode(value, priorValue, scalar);
            if (valueToEncode == null)
                presenceMapBuilder.Skip();
            else
                presenceMapBuilder.Set();
            return valueToEncode;
        }

        public virtual bool CanEncode(ScalarValue value, Scalar field)
        {
            return true;
        }

        public override string ToString()
        {
            return _operator.ToString();
        }

        #region Equals

        public override bool Equals(object obj) //POINTP
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}