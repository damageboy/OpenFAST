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
using FieldValue = OpenFAST.FieldValue;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;
using ComposedValueConverter = OpenFAST.Template.ComposedValueConverter;
using LongValue = OpenFAST.Template.LongValue;

namespace OpenFAST.Template.Type
{
	[Serializable]
	public sealed class DecimalConverter : ComposedValueConverter
	{
		
		private static readonly FieldValue[] NULL_SET = new FieldValue[]{null, null};
		
		private static readonly FieldValue[] UNDEFINED_SET = new FieldValue[]{ScalarValue.UNDEFINED, ScalarValue.UNDEFINED};
		
		public FieldValue[] Split(FieldValue value_Renamed)
		{
			if (value_Renamed == null)
				return NULL_SET;
			else if (value_Renamed == ScalarValue.UNDEFINED)
				return UNDEFINED_SET;
			DecimalValue decimal_Renamed = (DecimalValue) value_Renamed;
			return new FieldValue[]{new IntegerValue(decimal_Renamed.exponent), new LongValue(decimal_Renamed.mantissa)};
		}
		
		public FieldValue Compose(FieldValue[] values)
		{
			if (values[0] == null)
				return null;
			if (values[0] == ScalarValue.UNDEFINED)
				return ScalarValue.UNDEFINED;
			return new DecimalValue(((ScalarValue) values[1]).ToLong(), ((IntegerValue) values[0]).value_Renamed);
		}
	}
}