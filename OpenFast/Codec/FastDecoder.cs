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
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OpenFAST.Template;
using OpenFAST.Template.Type.Codec;

namespace OpenFAST.Codec
{
    public sealed class FastDecoder : ICoder, IEnumerable<Message>
    {
        private readonly Context _context;
        private readonly Stream _inStream;

        public FastDecoder(Context context, Stream inStream)
        {
            _inStream = inStream;
            _context = context;
        }

        #region Coder Members

        public void Reset()
        {
            _context.Reset();
        }

        #endregion

        public Message ReadMessage()
        {
            var bitVectorValue = (BitVectorValue) TypeCodec.BIT_VECTOR.Decode(_inStream);

            if (bitVectorValue == null)
                return null; // Must have reached end of stream;

            BitVector pmap = bitVectorValue.Value;
            var presenceMapReader = new BitVectorReader(pmap);

            // if template id is not present, use previous, else decode template id
            int templateId = (presenceMapReader.Read())
                                 ? ((IntegerValue) TypeCodec.UINT.Decode(_inStream)).Value
                                 : _context.LastTemplateId;
            MessageTemplate template = _context.GetTemplate(templateId);

            if (template == null)
            {
                return null;
            }
            _context.NewMessage(template);

            _context.LastTemplateId = templateId;

            return template.Decode(_inStream, templateId, presenceMapReader, _context);
        }

        public IEnumerator<Message> GetEnumerator()
        {
            Reset();
            Message msg;
            while ((msg = ReadMessage()) != null)
                yield return msg;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}