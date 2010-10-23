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
        private readonly ScalarValue _defaultValue;
        private readonly ScalarValue _initialValue;
        private readonly Operator.Operator _operator;
        private readonly OperatorCodec _operatorCodec;
        private readonly FASTType _type;
        private readonly TypeCodec _typeCodec;
        private string _dictionary;

        public Scalar(string name, FASTType type, Operator.Operator op, ScalarValue defaultValue,
                      bool optional)
            : this(new QName(name), type, op, defaultValue, optional)
        {
        }

        public Scalar(QName name, FASTType type, Operator.Operator op, ScalarValue defaultValue,
                      bool optional)
            : this(name, type, op, op.GetCodec(type), defaultValue, optional)
        {
        }

        public Scalar(QName name, FASTType type, OperatorCodec operatorCodec, ScalarValue defaultValue, bool optional)
            : this(name, type, operatorCodec.Operator, operatorCodec, defaultValue, optional)
        {
        }

        private Scalar(QName name, FASTType type, Operator.Operator op, OperatorCodec operatorCodec,
                       ScalarValue defaultValue, bool optional)
            : base(name, optional)
        {
            _operator = op;
            _operatorCodec = operatorCodec;
            _dictionary = DictionaryFields.Global;
            _defaultValue = defaultValue ?? ScalarValue.Undefined;
            _type = type;
            _typeCodec = type.GetCodec(op, optional);
            _initialValue = ((defaultValue == null) || defaultValue.IsUndefined) ? _type.DefaultValue : defaultValue;
            op.Validate(this);
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

        public override byte[] Encode(IFieldValue fieldValue, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder)
        {
            IDictionary dict = context.GetDictionary(Dictionary);

            ScalarValue priorValue = context.Lookup(dict, encodeTemplate, Key);
            var value = (ScalarValue) fieldValue;
            if (!_operatorCodec.CanEncode(value, this))
            {
                Global.ErrorHandler.OnError(null, DynError.CantEncodeValue,
                                            "The scalar {0} cannot encode the value {1}", this, value);
            }
            ScalarValue valueToEncode = _operatorCodec.GetValueToEncode(value, priorValue, this,
                                                                        presenceMapBuilder);
            if (_operator.ShouldStoreValue(value))
            {
                context.Store(dict, encodeTemplate, Key, value);
            }
            if (valueToEncode == null)
            {
                return ByteUtil.EmptyByteArray;
            }
            byte[] encoding = _typeCodec.Encode(valueToEncode);
            if (context.TraceEnabled && encoding.Length > 0)
            {
                context.GetEncodeTrace().Field(this, fieldValue, valueToEncode, encoding, presenceMapBuilder.Index);
            }
            return encoding;
        }

        public override bool UsesPresenceMapBit()
        {
            return _operatorCodec.UsesPresenceMapBit(IsOptional);
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
                ScalarValue priorValue = null;
                IDictionary dict = null;
                QName key = Key;

                ScalarValue value;
                int pmapIndex = presenceMapReader.Index;
                if (IsPresent(presenceMapReader))
                {
                    if (context.TraceEnabled)
                        inStream = new RecordingInputStream(inStream);

                    if (!_operatorCodec.ShouldDecodeType)
                        return _operatorCodec.DecodeValue(null, null, this);

                    if (_operatorCodec.DecodeNewValueNeedsPrevious)
                    {
                        dict = context.GetDictionary(Dictionary);
                        priorValue = context.Lookup(dict, decodeTemplate, key);
                        ValidateDictionaryTypeAgainstFieldType(priorValue, _type);
                    }

                    ScalarValue decodedValue = _typeCodec.Decode(inStream);
                    value = _operatorCodec.DecodeValue(decodedValue, priorValue, this);

                    if (context.TraceEnabled)
                        context.DecodeTrace.Field(this, value, decodedValue,
                                                  ((RecordingInputStream) inStream).Buffer, pmapIndex);
                }
                else
                {
                    if (_operatorCodec.DecodeEmptyValueNeedsPrevious)
                    {
                        dict = context.GetDictionary(Dictionary);
                        priorValue = context.Lookup(dict, decodeTemplate, key);
                        ValidateDictionaryTypeAgainstFieldType(priorValue, _type);
                    }

                    value = _operatorCodec.DecodeEmptyValue(priorValue, this);
                }

                ValidateDecodedValueIsCorrectForType(value, _type);

#warning TODO: Review if this previous "if" statement is needed.
                // if (Operator != Template.Operator.Operator.DELTA || value != null)
                if (value != null &&
                    (_operatorCodec.DecodeNewValueNeedsPrevious || _operatorCodec.DecodeEmptyValueNeedsPrevious))
                {
                    context.Store(dict ?? context.GetDictionary(Dictionary), decodeTemplate, key, value);
                }

                return value;
            }
            catch (DynErrorException e)
            {
                throw new DynErrorException(e, e.Error, "Error occurred while decoding {0}", this);
            }
        }

        private static void ValidateDecodedValueIsCorrectForType(ScalarValue value, FASTType type)
        {
            if (value == null)
                return;
            type.ValidateValue(value);
        }

        private static void ValidateDictionaryTypeAgainstFieldType(ScalarValue priorValue, FASTType type)
        {
            if (priorValue == null || priorValue.IsUndefined)
                return;
            if (!type.IsValueOf(priorValue))
            {
                Global.ErrorHandler.OnError(null, DynError.InvalidType,
                                            "The value '{0}' is not valid for the type {1}", priorValue, type);
            }
        }

        public override string ToString()
        {
            return string.Format("Scalar [name={0}, operator={1}, type={2}, dictionary={3}]", Name, _operator, _type,
                                 _dictionary);
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
            Scalar t = other as Scalar;
            if (t==null)
                return false;
            return Equals(t);
        }

        internal bool Equals(Scalar other)
        {
            bool equals = EqualsPrivate(Name, other.Name);
            equals = equals && EqualsPrivate(_type, other._type);
            equals = equals && EqualsPrivate(_typeCodec, other._typeCodec);
            equals = equals && EqualsPrivate(_operator, other._operator);
            equals = equals && EqualsPrivate(_operatorCodec, other._operatorCodec);
            equals = equals && EqualsPrivate(_initialValue, other._initialValue);
            equals = equals && EqualsPrivate(_dictionary, other._dictionary);
            equals = equals && EqualsPrivate(Id, other.Id);
            return equals;
        }

        private static bool EqualsPrivate(object o, object o2)
        {
            if (o == null)
            {
                if (o2 == null)
                    return true;
                return false;
            }
            return o.Equals(o2);
        }

        public override int GetHashCode()
        {
            return QName.GetHashCode() + _type.GetHashCode() + _typeCodec.GetHashCode() + _operator.GetHashCode() +
                   _operatorCodec.GetHashCode() + _initialValue.GetHashCode() + _dictionary.GetHashCode();
        }
        public OperatorCodec GetOperatorCodec()
        {
            return _operatorCodec;
        }
    }
}