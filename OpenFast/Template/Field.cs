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
using System.Collections.Generic;
using System.IO;

namespace OpenFAST.Template
{
    [Serializable]
    public abstract class Field : IEquatable<Field>
    {
        private readonly bool _isOptional;
        private readonly QName _name;
        private Dictionary<QName, string> _attributes;
        private string _id;
        private QName _key;
        private MessageTemplate template;

        protected Field(QName name, bool isOptional)
        {
            _name = name;
            _key = name;
            _isOptional = isOptional;
        }


        protected Field(QName name, QName key, bool isOptional)
        {
            _name = name;
            _key = key;
            _isOptional = isOptional;
        }


        protected Field(string name, string key, bool isOptional, string id)
        {
            _name = new QName(name);
            _key = new QName(key);
            _isOptional = isOptional;
            _id = id;
        }

        public string Name
        {
            get { return _name.Name; }
        }

        public QName QName
        {
            get { return _name; }
        }

        public bool IsOptional
        {
            get { return _isOptional; }
        }

        public QName Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public string Id
        {
            get { return _id ?? ""; }
            set { _id = value; }
        }

        public abstract System.Type ValueType { get; }
        public abstract string TypeName { get; }

        #region IEquatable<Field> Members

        public bool Equals(Field other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name) && other._isOptional.Equals(_isOptional) &&
                   Equals(other._attributes, _attributes) && Equals(other.Id, Id) && Equals(other._key, _key);
        }

        #endregion

        public bool IsIdNull()
        {
            return _id == null;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Field)) return false;
            return Equals((Field) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_name != null ? _name.GetHashCode() : 0);
                result = (result*397) ^ _isOptional.GetHashCode();
                result = (result*397) ^ (_attributes != null ? _attributes.GetHashCode() : 0);
                result = (result*397) ^ (_id != null ? _id.GetHashCode() : 0);
                result = (result*397) ^ (_key != null ? _key.GetHashCode() : 0);
                return result;
            }
        }

        public virtual bool HasAttribute(QName attributeName)
        {
            return _attributes != null && _attributes.ContainsKey(attributeName);
        }

        public virtual bool TryGetAttribute(QName qname, out string value)
        {
            if (_attributes != null)
                return _attributes.TryGetValue(qname, out value);

            value = null;
            return false;
        }

        public void AddAttribute(QName qname, string value)
        {
            if (_attributes == null)
                _attributes = new Dictionary<QName, string>();
            _attributes[qname] = value;
        }

        public void SetMessageTemplate(MessageTemplate template)
        {
            this.template = template;
        }

        public MessageTemplate GetTemplate()
        {
            return template;
        }

        protected bool IsPresent(BitVectorReader presenceMapReader)
        {
            return (!UsesPresenceMapBit()) || presenceMapReader.Read();
        }

        public abstract byte[] Encode(IFieldValue value, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder);

        public abstract IFieldValue Decode(Stream inStream, Group decodeTemplate, Context context,
                                           BitVectorReader presenceMapReader);

        public abstract bool UsesPresenceMapBit();

        public abstract bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue);

        public abstract IFieldValue CreateValue(string value);
    }
}