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
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	public sealed class StringType:SimpleType
	{
		override public ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}
		private const long serialVersionUID = 1L;
		
		public StringType(string typeName, TypeCodec codec, TypeCodec nullableCodec):base(typeName, codec, nullableCodec)
		{
		}
		
		public override ScalarValue GetVal(string value_Renamed)
		{
			return new StringValue(value_Renamed);
		}
		
		public override TypeCodec GetCodec(Operator operator_Renamed, bool optional)
		{
			if (operator_Renamed == Operator.DELTA)
				return (optional)?TypeCodec.NULLABLE_STRING_DELTA:TypeCodec.STRING_DELTA;
			return base.GetCodec(operator_Renamed, optional);
		}
		
		public override bool IsValueOf(ScalarValue previousValue)
		{
			return previousValue is StringValue;
		}
	}
}