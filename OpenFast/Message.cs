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
using OpenFAST.Template;
using OpenFAST.Utility;

namespace OpenFAST
{
    public class Message : GroupValue
    {
        private readonly MessageTemplate _template;

        public Message(MessageTemplate template, IFieldValue[] fieldValues)
            : base(template, fieldValues)
        {
            if (template == null) throw new ArgumentNullException("template");
            _template = template;
        }

        public Message(MessageTemplate template)
            : this(template, InitializeFieldValues(template.FieldCount))
        {
        }

        public MessageTemplate Template
        {
            get { return _template; }
        }

        private static IFieldValue[] InitializeFieldValues(int fieldCount)
        {
            var fields = new IFieldValue[fieldCount];
            return fields;
        }

        public override IFieldValue Copy()
        {
            var copy = (GroupValue) base.Copy();
            return new Message(_template, copy.Values);
        }

        #region Equals

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            
            var other = obj as Message;
            if (ReferenceEquals(null, other)) return false;
            
#warning bug: ?? Ignore base, because we ignore the first field
            return Equals(other._template, _template) && Util.ArrayEqualsSlow(Values, other.Values, 1);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // bug: ?? Ignore base, because we ignore the first field
                return (Util.GetCollectionHashCode(Values)*397) ^ _template.GetHashCode();
            }
        }

        #endregion
    }
}