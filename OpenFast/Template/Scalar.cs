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
using System.IO;
using OpenFAST.Error;
using OpenFAST.Template.Operator;
using OpenFAST.Template.Type;
using OpenFAST.Template.Type.Codec;
using OpenFAST.Utility;

namespace OpenFAST.Template
{
    [Serializable]
    public sealed class Scalar : Field
    {
        private readonly ScalarValue _initialValue;
        private readonly Operator.Operator _operator;
        private readonly OperatorCodec _operatorCodec;
        private readonly FASTType _type;
        private readonly TypeCodec _typeCodec;
        private ScalarValue _defaultValue;
        private string _dictionary;

        public Scalar(string name, FASTType type, Operator.Operator op, ScalarValue defaultValue,
                      bool optional)
            : this(new QName(name), type, op, defaultValue, optional)
        {
        }

        public Scalar(QName name, FASTType type, Operator.Operator op, ScalarValue defaultValue,
                      bool optional)
            : base(name, optional)
        {
            InitBlock();
            _operator = op;
            _operatorCodec = op.GetCodec(type);
            _dictionary = DictionaryFields.GLOBAL;
            _defaultValue = defaultValue ?? ScalarValue.UNDEFINED;
            _type = type;
            _typeCodec = type.GetCodec(op, optional);
            _initialValue = ((defaultValue == null) || defaultValue.Undefined) ? _type.DefaultValue : defaultValue;
            op.Validate(this);
        }

        public Scalar(QName name, FASTType type, OperatorCodec operatorCodec, ScalarValue defaultValue, bool optional)
            : base(name, optional)
        {
            InitBlock();
            _operator = operatorCodec.Operator;
            _operatorCodec = operatorCodec;
            _dictionary = "global";
            _defaultValue = defaultValue ?? ScalarValue.UNDEFINED;
            _type = type;
            _typeCodec = type.GetCodec(_operator, optional);
            _initialValue = ((defaultValue == null) || defaultValue.Undefined) ? _type.DefaultValue : defaultValue;
            _operator.Validate(this);
        }

        public FASTType Type
        {
            get { return _type; }
        }

        public OperatorCodec OperatorCodec
        {
            get { return _operatorCodec; }
        }

        public Operator.Operator Operator
        {
            get { return _operator; }
        }

        public string Dictionary
        {
            get { return _dictionary; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _dictionary = value;
            }
        }

        public ScalarValue DefaultValue
        {
            get { return _defaultValue; }
        }

        public override System.Type ValueType
        {
            get { return typeof (ScalarValue); }
        }

        public override string TypeName
        {
            get { return "scalar"; }
        }

        public ScalarValue BaseValue
        {
            get { return _initialValue; }
        }

        public TypeCodec TypeCodec
        {
            get { return _typeCodec; }
        }

        private void InitBlock()
        {
            _defaultValue = ScalarValue.UNDEFINED;
        }

        public override byte[] Encode(IFieldValue fieldValue, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder)
        {
            IDictionary dict = context.GetDictionary(Dictionary);

            ScalarValue priorValue = context.Lookup(dict, encodeTemplate, Key);
            var value = (ScalarValue) fieldValue;
            if (!_operatorCodec.CanEncode(value, this))
            {
                Global.HandleError(FastConstants.D3_CANT_ENCODE_VALUE,
                                   "The scalar " + this + " cannot encode the value " + value);
            }
            ScalarValue valueToEncode = _operatorCodec.GetValueToEncode(value, priorValue, this,
                                                                        presenceMapBuilder);
            if (_operator.ShouldStoreValue(value))
            {
                context.Store(dict, encodeTemplate, Key, value);
            }
            if (valueToEncode == null)
            {
                return new byte[0];
            }
            byte[] encoding = _typeCodec.Encode(valueToEncode);
            if (context.TraceEnabled && encoding.Length > 0)
            {
                context.GetEncodeTrace().Field(this, fieldValue, valueToEncode, encoding, presenceMapBuilder.Index);
            }
            return encoding;
        }

        public ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue)
        {
            return _operatorCodec.DecodeValue(newValue, previousValue, this);
        }

        public ScalarValue Decode(ScalarValue previousValue)
        {
            return _operatorCodec.DecodeEmptyValue(previousValue, this);
        }

        public override bool UsesPresenceMapBit()
        {
            return _operatorCodec.UsesPresenceMapBit(Optional);
        }

        public override bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue)
        {
            return _operatorCodec.IsPresenceMapBitSet(encoding, fieldValue);
        }

        public override IFieldValue Decode(Stream inStream, Group decodeTemplate, Context context,
                                           BitVectorReader presenceMapReader)
        {
            try
            {
                ScalarValue previousValue = null;
                IDictionary dict = null;
                if (_operator.UsesDictionary)
                {
                    dict = context.GetDictionary(Dictionary);
                    previousValue = context.Lookup(dict, decodeTemplate, Key);
                    ValidateDictionaryTypeAgainstFieldType(previousValue, _type);
                }

                ScalarValue value;
                int pmapIndex = presenceMapReader.Index;
                if (IsPresent(presenceMapReader))
                {
                    if (context.TraceEnabled)
                        inStream = new RecordingInputStream(inStream);
                    if (!_operatorCodec.ShouldDecodeType)
                    {
                        return _operatorCodec.DecodeValue(null, null, this);
                    }
                    ScalarValue decodedValue = _typeCodec.Decode(inStream);
                    value = DecodeValue(decodedValue, previousValue);
                    if (context.TraceEnabled)
                        context.DecodeTrace.Field(this, value, decodedValue,
                                                  ((RecordingInputStream) inStream).Buffer, pmapIndex);
                }
                else
                {
                    value = Decode(previousValue);
                }

                ValidateDecodedValueIsCorrectForType(value, _type);
                if (Operator != Template.Operator.Operator.DELTA || value != null)
                {
                    context.Store(dict ?? context.GetDictionary(Dictionary), decodeTemplate, Key, value);
                    return value;
                }
                return value;
            }
            catch (FastException e)
            {
                throw new FastException("Error occurred while decoding " + this, e.Code, e);
            }
        }

        private static void ValidateDecodedValueIsCorrectForType(ScalarValue value, FASTType type)
        {
            if (value == null)
                return;
            type.ValidateValue(value);
        }

        private static void ValidateDictionaryTypeAgainstFieldType(ScalarValue previousValue, FASTType type)
        {
            if (previousValue == null || previousValue.Undefined)
                return;
            if (!type.IsValueOf(previousValue))
            {
                Global.HandleError(FastConstants.D4_INVALID_TYPE,
                                   "The value \"" + previousValue + "\" is not valid for the type " + type);
            }
        }

        public override string ToString()
        {
            return "Scalar [name=" + Name + ", operator=" + _operator + ", type=" + _type + ", dictionary=" +
                   _dictionary + "]";
        }

        public override IFieldValue CreateValue(string value)
        {
            return _type.GetValue(value);
        }

        public string Serialize(ScalarValue value)
        {
            return _type.Serialize(value);
        }

        public override bool Equals(Object other)
        {
            if (ReferenceEquals(other, this))
                return true;
            if (ReferenceEquals(other, null) || !(other is Scalar))
                return false;
            return Equals((Scalar) other);
        }

        internal bool Equals(Scalar other)
        {
            return QName.Equals(other.QName) && _type.Equals(other._type) && _typeCodec.Equals(other._typeCodec) &&
                   _operator.Equals(other._operator) && _operatorCodec.Equals(other._operatorCodec) &&
                   _initialValue.Equals(other._initialValue) && _dictionary.Equals(other._dictionary);
        }

        public override int GetHashCode()
        {
            return QName.GetHashCode() + _type.GetHashCode() + _typeCodec.GetHashCode() + _operator.GetHashCode() +
                   _operatorCodec.GetHashCode() + _initialValue.GetHashCode() + _dictionary.GetHashCode();
        }
    }
}