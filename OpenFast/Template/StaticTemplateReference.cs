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

namespace OpenFAST.Template
{
	
	[Serializable]
	public class StaticTemplateReference:Field
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
		virtual public MessageTemplate Template
		{
			get
			{
				return template;
			}
			
		}
		private const long serialVersionUID = 1L;
		private MessageTemplate template;
		
		public StaticTemplateReference(MessageTemplate template):base(template.QName, false)
		{
			this.template = template;
		}
		
		public override FieldValue CreateValue(string value_Renamed)
		{
			return null;
		}
		
		public override FieldValue Decode(System.IO.Stream in_Renamed, Group template, Context context, BitVectorReader pmapReader)
		{
			return null;
		}
		
		public override byte[] Encode(FieldValue value_Renamed, Group template, Context context, BitVectorBuilder presenceMapBuilder)
		{
			return null;
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
			if (obj == this)
				return true;
			if (obj == null || obj.GetType() != this.GetType())
				return false;
			StaticTemplateReference other = (StaticTemplateReference) obj;
			return template.Equals(other.template);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}