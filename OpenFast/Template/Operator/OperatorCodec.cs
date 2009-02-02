using System;
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using FieldValue = OpenFAST.FieldValue;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;
using Key = OpenFAST.util.Key;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	public abstract class OperatorCodec
	{
		virtual public Operator Operator
		{
			get
			{
				return operator_Renamed;
			}
			
		}
        private static readonly System.Collections.Generic.Dictionary<Key, OperatorCodec> OPERATOR_MAP = new System.Collections.Generic.Dictionary<Key, OperatorCodec>();
		
		protected internal static readonly OperatorCodec NONE_ALL;
		protected internal static readonly OperatorCodec CONSTANT_ALL;
		protected internal static readonly OperatorCodec DEFAULT_ALL;
		protected internal static readonly OperatorCodec COPY_ALL = new CopyOperatorCodec();
		protected internal static readonly OperatorCodec INCREMENT_INTEGER;
		protected internal static readonly OperatorCodec DELTA_INTEGER;
		protected internal static readonly OperatorCodec DELTA_STRING = new DeltaStringOperatorCodec();
		protected internal static readonly OperatorCodec DELTA_DECIMAL = new DeltaDecimalOperatorCodec();
		protected internal static readonly OperatorCodec TAIL;
		private Operator operator_Renamed;
		
		protected internal OperatorCodec(Operator operator_Renamed, FASTType[] types)
		{
			this.operator_Renamed = operator_Renamed;
			for (int i = 0; i < types.Length; i++)
			{
				Key key = new Key(operator_Renamed, types[i]);
				
				if (!OPERATOR_MAP.ContainsKey(key))
				{
					OPERATOR_MAP[key] = this;
				}
			}
		}
		
		public static OperatorCodec GetCodec(Operator operator_Renamed, FASTType type)
		{
			Key key = new Key(operator_Renamed, type);
			
			if (!OPERATOR_MAP.ContainsKey(key))
			{
				Global.HandleError(OpenFAST.Error.FastConstants.S2_OPERATOR_TYPE_INCOMP, "The operator \"" + operator_Renamed + "\" is not compatible with type \"" + type + "\"");
				throw new System.ArgumentException();
			}
			
			return OPERATOR_MAP[key];
		}
		
		public abstract ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field);
		
		public abstract ScalarValue DecodeValue(ScalarValue newValue, ScalarValue priorValue, Scalar field);
		
		public virtual bool IsPresenceMapBitSet(byte[] encoding, FieldValue fieldValue)
		{
			return encoding.Length != 0;
		}
		
		public abstract ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field);
		
		public virtual bool UsesPresenceMapBit(bool optional)
		{
			return true;
		}

		public virtual ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar scalar, BitVectorBuilder presenceMapBuilder)
		{
			ScalarValue valueToEncode = GetValueToEncode(value_Renamed, priorValue, scalar);
			if (valueToEncode == null)
				presenceMapBuilder.Skip();
			else
				presenceMapBuilder.set_Renamed();
			return valueToEncode;
		}
		
		public virtual bool CanEncode(ScalarValue value_Renamed, Scalar field)
		{
			return true;
		}
		
		public virtual bool ShouldDecodeType()
		{
			return true;
		}
		
		public  override bool Equals(System.Object obj)
		{
			return obj != null && obj.GetType() == GetType();
		}
		
		public override string ToString()
		{
			return operator_Renamed.ToString();
		}
		static OperatorCodec()
		{
			NONE_ALL = new NoneOperatorCodec(Operator.NONE, FASTType.ALL_TYPES());
			CONSTANT_ALL = new ConstantOperatorCodec(Operator.CONSTANT, FASTType.ALL_TYPES());
			DEFAULT_ALL = new DefaultOperatorCodec(Operator.DEFAULT, FASTType.ALL_TYPES());
			INCREMENT_INTEGER = new IncrementIntegerOperatorCodec(Operator.INCREMENT, FASTType.INTEGER_TYPES);
			DELTA_INTEGER = new DeltaIntegerOperatorCodec(Operator.DELTA, FASTType.INTEGER_TYPES);
			TAIL = new TailOperatorCodec(Operator.TAIL, new FASTType[]{FASTType.ASCII, FASTType.STRING, FASTType.UNICODE, FASTType.BYTE_VECTOR});
		}

        public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}