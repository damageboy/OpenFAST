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
using ByteVectorValue = OpenFAST.ByteVectorValue;
using ScalarValue = OpenFAST.ScalarValue;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	sealed class ByteVectorType:SimpleType
	{
		override public ScalarValue DefaultValue
		{
			get
			{
				return new ByteVectorValue(new byte[]{});
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal ByteVectorType():base("byteVector", TypeCodec.BYTE_VECTOR, TypeCodec.NULLABLE_BYTE_VECTOR_TYPE)
		{
		}
		
		public override ScalarValue GetVal(string value_Renamed)
		{
			return new ByteVectorValue(SupportClass.ToByteArray(value_Renamed));
		}
		
		public override bool IsValueOf(ScalarValue previousValue)
		{
			return previousValue is ByteVectorValue;
		}
	}
}