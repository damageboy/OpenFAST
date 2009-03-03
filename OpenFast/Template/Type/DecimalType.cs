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
using DecimalValue = OpenFAST.DecimalValue;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	sealed class DecimalType:SimpleType
	{
		override public ScalarValue DefaultValue
		{
			get
			{
				return new DecimalValue(0.0);
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal DecimalType():base("decimal", TypeCodec.SF_SCALED_NUMBER, TypeCodec.NULLABLE_SF_SCALED_NUMBER)
		{
		}
		
		public override TypeCodec GetCodec(Operator operator_Renamed, bool optional)
		{
			return base.GetCodec(operator_Renamed, optional);
		}

        public override ScalarValue GetVal(string value_Renamed)
		{
			try
			{
				return new DecimalValue(System.Double.Parse(value_Renamed));
			}
			catch (System.FormatException)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.S3_INITIAL_VALUE_INCOMP, "The value \"" + value_Renamed + "\" is not compatible with type " + this);
				return null;
			}
		}
		
		public override bool IsValueOf(ScalarValue previousValue)
		{
			return previousValue is DecimalValue;
		}
	}
}