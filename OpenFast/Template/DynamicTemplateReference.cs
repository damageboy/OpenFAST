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
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using BitVectorReader = OpenFAST.BitVectorReader;
using Context = OpenFAST.Context;
using FieldValue = OpenFAST.FieldValue;
using Message = OpenFAST.Message;
using QName = OpenFAST.QName;
using FastDecoder = OpenFAST.Codec.FastDecoder;

namespace OpenFAST.Template
{
	[Serializable]
	public sealed class DynamicTemplateReference:Field
	{

		override public string TypeName
		{
			get
			{
				return null;
			}
			
		}

		override public System.Type ValueType
		{
			get
			{
				return null;
			}
			
		}
		private const long serialVersionUID = 1L;
		public static readonly DynamicTemplateReference INSTANCE = new DynamicTemplateReference();
		
		private DynamicTemplateReference():base(QName.NULL, false)
		{
		}
		
		public override FieldValue CreateValue(string value_Renamed)
		{
			return null;
		}
		
		
		public override FieldValue Decode(System.IO.Stream in_Renamed, Group template, Context context, BitVectorReader pmapReader)
		{
			return new FastDecoder(context, in_Renamed).ReadMessage();
		}
		
		
		public override byte[] Encode(FieldValue value_Renamed, Group template, Context context, BitVectorBuilder presenceMapBuilder)
		{
			Message message = (Message) value_Renamed;
			return message.Template.Encode(message, context);
		}
		
		
		public override bool IsPresenceMapBitSet(byte[] encoding, FieldValue fieldValue)
		{
			return false;
		}
		
		
		public override bool UsesPresenceMapBit()
		{
			return false;
		}
		
		public  override bool Equals(System.Object obj)
		{
			return obj != null && obj.GetType().Equals(this.GetType());
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}