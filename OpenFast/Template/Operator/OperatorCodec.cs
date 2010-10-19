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
using OpenFAST.Template.Type;
using OpenFAST.Utility;

namespace OpenFAST.Template.Operator
{
    [Serializable]
    public abstract class OperatorCodec
    {
        private static readonly Dictionary<Tuple<Operator, FASTType>, OperatorCodec> OperatorMap =
            new Dictionary<Tuple<Operator, FASTType>, OperatorCodec>();

        protected internal static readonly OperatorCodec NoneAll =
            new NoneOperatorCodec(Operator.NONE, FASTType.ALL_TYPES());

        protected internal static readonly OperatorCodec ConstantAll
            = new ConstantOperatorCodec(Operator.CONSTANT, FASTType.ALL_TYPES());

        protected internal static readonly OperatorCodec DefaultAll
            = new DefaultOperatorCodec(Operator.DEFAULT, FASTType.ALL_TYPES());

        protected internal static readonly OperatorCodec CopyAll
            = new CopyOperatorCodec();

        protected internal static readonly OperatorCodec IncrementInteger
            = new IncrementIntegerOperatorCodec(Operator.INCREMENT, FASTType.INTEGER_TYPES);

        protected internal static readonly OperatorCodec DeltaInteger
            = new DeltaIntegerOperatorCodec(Operator.DELTA, FASTType.INTEGER_TYPES);

        protected internal static readonly OperatorCodec DeltaString
            = new DeltaStringOperatorCodec();

        protected internal static readonly OperatorCodec DeltaDecimal
            = new DeltaDecimalOperatorCodec();

        protected internal static readonly OperatorCodec Tail =
            new TailOperatorCodec(
                Operator.TAIL, new[] {FASTType.ASCII, FASTType.STRING, FASTType.UNICODE, FASTType.BYTE_VECTOR});

        private readonly Operator _operator;

        protected internal OperatorCodec(Operator op, FASTType[] types)
        {
            _operator = op;
            foreach (FASTType t in types)
                OperatorMap[Tuple.Create(op, t)] = this;
        }

        public virtual Operator Operator
        {
            get { return _operator; }
        }

        public virtual bool ShouldDecodeType
        {
            get { return true; }
        }

        public static OperatorCodec GetCodec(Operator op, FASTType type)
        {
            Tuple<Operator, FASTType> key = Tuple.Create(op, type);

            OperatorCodec codec;
            if (OperatorMap.TryGetValue(key, out codec))
                return codec;

            Global.HandleError(FastConstants.S2_OPERATOR_TYPE_INCOMP,
                               "The operator \"" + op + "\" is not compatible with type \"" + type +
                               "\"");
            throw new ArgumentOutOfRangeException("op" + ",type", key, "Not found");
        }

        public abstract ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field);

        public abstract ScalarValue DecodeValue(ScalarValue newValue, ScalarValue priorValue, Scalar field);

        public virtual bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue)
        {
            return encoding.Length != 0;
        }

        public abstract ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field);

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

        public override bool Equals(object obj) //POINTP
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override string ToString()
        {
            return _operator.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}