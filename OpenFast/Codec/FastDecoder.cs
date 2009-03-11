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
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Codec
{
	public sealed class FastDecoder : Coder
	{
		private readonly System.IO.Stream in_Renamed;
		
		private readonly Context context;
		
		public FastDecoder(Context context, System.IO.Stream in_Renamed)
		{
			this.in_Renamed = in_Renamed;
			this.context = context;
		}
		
		public Message ReadMessage()
		{
            var bitVectorValue = (BitVectorValue)TypeCodec.BIT_VECTOR.Decode(in_Renamed);
			
			if (bitVectorValue == null)
			{
				return null; // Must have reached end of stream;
			}
			
			var pmap = bitVectorValue.value_Renamed;
			var presenceMapReader = new BitVectorReader(pmap);
			
			// if template id is not present, use previous, else decode template id
			int templateId = (presenceMapReader.Read())?((IntegerValue) TypeCodec.UINT.Decode(in_Renamed)).value_Renamed:context.LastTemplateId;
			var template = context.GetTemplate(templateId);
			
			if (template == null)
			{
				return null;
			}
			context.NewMessage(template);
			
			context.LastTemplateId = templateId;
			
			return template.Decode(in_Renamed, templateId, presenceMapReader, context);
		}
		
        public void Reset()
        {
            context.Reset();
        }
    }
}