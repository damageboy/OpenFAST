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
using FastException = OpenFAST.Error.FastException;
using OperatorCodec = OpenFAST.Template.Operator.OperatorCodec;
using FASTType = OpenFAST.Template.Type.FASTType;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;
using RecordingInputStream = OpenFAST.util.RecordingInputStream;

namespace OpenFAST.Template
{
	
	[Serializable]
	public sealed class Scalar:Field
	{
		private void  InitBlock()
		{
			defaultValue = ScalarValue.UNDEFINED;
		}

		public FASTType Type
		{
			get
			{
				return type;
			}
			
		}

		public OperatorCodec OperatorCodec
		{
			get
			{
				return operatorCodec;
			}
			
		}

        public Operator.Operator Operator
		{
			get
			{
				return operator_Renamed;
			}
			
		}

		public string Dictionary
		{
			get
			{
				return dictionary;
			}
			
			set
			{
				if (value == null)
					throw new NullReferenceException();
				dictionary = value;
			}
			
		}

		public ScalarValue DefaultValue
		{
			get
			{
				return defaultValue;
			}
			
		}

		override public System.Type ValueType
		{
			get
			{
				return typeof(ScalarValue);
			}
			
		}

		override public string TypeName
		{
			get
			{
				return "scalar";
			}
			
		}

		public ScalarValue BaseValue
		{
			get
			{
				return initialValue;
			}
			
		}

		public TypeCodec TypeCodec
		{
			get
			{
				return typeCodec;
			}
			
		}

        private readonly Operator.Operator operator_Renamed;
		private readonly OperatorCodec operatorCodec;
		private readonly FASTType type;
		private readonly TypeCodec typeCodec;
		private string dictionary;
		private ScalarValue defaultValue;
		private readonly ScalarValue initialValue;

        public Scalar(string name, FASTType type, Operator.Operator operator_Renamed, ScalarValue defaultValue, bool optional)
            : this(new QName(name), type, operator_Renamed, defaultValue, optional)
		{
		}
        public Scalar(QName name, FASTType type, Operator.Operator operator_Renamed, ScalarValue defaultValue, bool optional)
            : base(name, optional)
		{
			InitBlock();
			this.operator_Renamed = operator_Renamed;
			operatorCodec = operator_Renamed.GetCodec(type);
			dictionary = Dictionary_Fields.GLOBAL;
			this.defaultValue = defaultValue ?? ScalarValue.UNDEFINED;
			this.type = type;
			typeCodec = type.GetCodec(operator_Renamed, optional);
			initialValue = ((defaultValue == null) || defaultValue.Undefined)?this.type.DefaultValue:defaultValue;
			operator_Renamed.Validate(this);
		}

		public Scalar(QName name, FASTType type, OperatorCodec operatorCodec, ScalarValue defaultValue, bool optional):base(name, optional)
		{
			InitBlock();
			operator_Renamed = operatorCodec.Operator;
			this.operatorCodec = operatorCodec;
			dictionary = "global";
			this.defaultValue = defaultValue ?? ScalarValue.UNDEFINED;
			this.type = type;
			typeCodec = type.GetCodec(operator_Renamed, optional);
			initialValue = ((defaultValue == null) || defaultValue.Undefined)?this.type.DefaultValue:defaultValue;
			operator_Renamed.Validate(this);
		}

		public override byte[] Encode(FieldValue fieldValue, Group encodeTemplate, Context context, BitVectorBuilder presenceMapBuilder)
		{
			var priorValue = context.Lookup(Dictionary, encodeTemplate, Key);
			var value_Renamed = (ScalarValue) fieldValue;
			if (!operatorCodec.CanEncode(value_Renamed, this))
			{
				Global.HandleError(Error.FastConstants.D3_CANT_ENCODE_VALUE, "The scalar " + this + " cannot encode the value " + value_Renamed);
			}
			var valueToEncode = operatorCodec.GetValueToEncode(value_Renamed, priorValue, this, presenceMapBuilder);
			if (operator_Renamed.ShouldStoreValue(value_Renamed))
			{
				context.Store(Dictionary, encodeTemplate, Key, value_Renamed);
			}
			if (valueToEncode == null)
			{
				return new byte[0];
			}
			byte[] encoding = typeCodec.Encode(valueToEncode);
			if (context.TraceEnabled && encoding.Length > 0)
			{
				context.GetEncodeTrace().Field(this, fieldValue, valueToEncode, encoding, presenceMapBuilder.Index);
			}
			return encoding;
		}

		public ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue)
		{
			return operatorCodec.DecodeValue(newValue, previousValue, this);
		}

		public ScalarValue Decode(ScalarValue previousValue)
		{
			return operatorCodec.DecodeEmptyValue(previousValue, this);
		}

		public override bool UsesPresenceMapBit()
		{
			return operatorCodec.UsesPresenceMapBit(optional);
		}

		public override bool IsPresenceMapBitSet(byte[] encoding, FieldValue fieldValue)
		{
			return operatorCodec.IsPresenceMapBitSet(encoding, fieldValue);
		}

		public override FieldValue Decode(System.IO.Stream in_Renamed, Group decodeTemplate, Context context, BitVectorReader presenceMapReader)
		{
			try
			{
				ScalarValue previousValue = null;
				if (operator_Renamed.UsesDictionary())
				{
					previousValue = context.Lookup(Dictionary, decodeTemplate, Key);
					ValidateDictionaryTypeAgainstFieldType(previousValue, type);
				}
				ScalarValue value_Renamed;
				int pmapIndex = presenceMapReader.Index;
				if (IsPresent(presenceMapReader))
				{
					if (context.TraceEnabled)
						in_Renamed = new RecordingInputStream(in_Renamed);
					if (!operatorCodec.ShouldDecodeType())
					{
						return operatorCodec.DecodeValue(null, null, this);
					}
					ScalarValue decodedValue = typeCodec.Decode(in_Renamed);
					value_Renamed = DecodeValue(decodedValue, previousValue);
					if (context.TraceEnabled)
						context.DecodeTrace.Field(this, value_Renamed, decodedValue, ((RecordingInputStream) in_Renamed).Buffer, pmapIndex);
				}
				else
				{
					value_Renamed = Decode(previousValue);
				}
				ValidateDecodedValueIsCorrectForType(value_Renamed, type);
                if (!((Operator == Template.Operator.Operator.DELTA) && (value_Renamed == null)))
				{
					context.Store(Dictionary, decodeTemplate, Key, value_Renamed);
				}
				return value_Renamed;
			}
			catch (FastException e)
			{
				throw new FastException("Error occurred while decoding " + this, e.Code, e);
			}
		}

		private static void  ValidateDecodedValueIsCorrectForType(ScalarValue value_Renamed, FASTType type)
		{
			if (value_Renamed == null)
				return ;
			type.ValidateValue(value_Renamed);
		}

		private static void  ValidateDictionaryTypeAgainstFieldType(ScalarValue previousValue, FASTType type)
		{
			if (previousValue == null || previousValue.Undefined)
				return ;
			if (!type.IsValueOf(previousValue))
			{
				Global.HandleError(Error.FastConstants.D4_INVALID_TYPE, "The value \"" + previousValue + "\" is not valid for the type " + type);
			}
		}

		public override string ToString()
		{
			return "Scalar [name=" + name.Name + ", operator=" + operator_Renamed + ", type=" + type + ", dictionary=" + dictionary + "]";
		}

		public override FieldValue CreateValue(string value_Renamed)
		{
			return type.GetValue(value_Renamed);
		}
		public string Serialize(ScalarValue value_Renamed)
		{
			return type.Serialize(value_Renamed);
		}
		public  override bool Equals(Object other)
		{
			if (other == this)
				return true;
			if (other == null || !(other is Scalar))
				return false;
			return Equals((Scalar) other);
		}
		internal bool Equals(Scalar other)
		{
			return name.Equals(other.name) && type.Equals(other.type) && typeCodec.Equals(other.typeCodec) && operator_Renamed.Equals(other.operator_Renamed) && operatorCodec.Equals(other.operatorCodec) && initialValue.Equals(other.initialValue) && dictionary.Equals(other.dictionary);
		}
		public override int GetHashCode()
		{
			return name.GetHashCode() + type.GetHashCode() + typeCodec.GetHashCode() + operator_Renamed.GetHashCode() + operatorCodec.GetHashCode() + initialValue.GetHashCode() + dictionary.GetHashCode();
		}
	}
}