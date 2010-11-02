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
using System;
using System.IO;

namespace OpenFAST.Template
{
    public class StaticTemplateReference : Field
    {
        private readonly MessageTemplate _template;

        public StaticTemplateReference(MessageTemplate template) : base(template.QName, false)
        {
            _template = template;
        }

        public override string TypeName
        {
            get { return null; }
        }

        public override Type ValueType
        {
            get { return null; }
        }

        public virtual MessageTemplate Template
        {
            get { return _template; }
        }

        public override IFieldValue CreateValue(string value)
        {
            return null;
        }

        public override IFieldValue Decode(Stream inStream, Group decodeTemplate, Context context,
                                           BitVectorReader pmapReader)
        {
            return null;
        }

        public override byte[] Encode(IFieldValue value, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder)
        {
            return null;
        }

        public override bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue)
        {
            return false;
        }

        public override bool UsesPresenceMapBit
        {
            get { return false; }
        }

        public override bool Equals(Object obj)
        {
            if (obj == this)
                return true;
            if (obj == null || obj.GetType() != GetType())
                return false;
            var other = (StaticTemplateReference) obj;
            return _template.Equals(other._template);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}