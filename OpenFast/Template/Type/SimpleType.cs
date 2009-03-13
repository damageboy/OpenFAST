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
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	public abstract class SimpleType:FASTType
	{
		private readonly TypeCodec codec;
		private readonly TypeCodec nullableCodec;

	    protected SimpleType(string typeName, TypeCodec codec, TypeCodec nullableCodec):base(typeName)
		{
			this.codec = codec;
			this.nullableCodec = nullableCodec;
		}


        public override TypeCodec GetCodec(Operator.Operator operator_Renamed, bool optional)
		{
			if (optional)
				return nullableCodec;
			return codec;
		}
		

		public override ScalarValue GetValue(string value_Renamed)
		{
			if (value_Renamed == null)
				return null;
			return GetVal(value_Renamed);
		}
		
		public abstract ScalarValue GetVal(string value_Renamed);
	}
}