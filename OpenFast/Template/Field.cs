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
using System.Collections.Generic;
using System.IO;

namespace OpenFAST.Template
{
    [Serializable]
    public abstract class Field : IEquatable<Field>
    {
        private readonly QName _name;
        private readonly bool _optional;
        private Dictionary<QName, string> _attributes;
        private string _id;
        private QName _key;

        protected Field(QName name, bool optional)
        {
            _name = name;
            _key = name;
            _optional = optional;
        }


        protected Field(QName name, QName key, bool optional)
        {
            _name = name;
            _key = key;
            _optional = optional;
        }


        protected Field(string name, string key, bool optional, string id)
        {
            _name = new QName(name);
            _key = new QName(key);
            _optional = optional;
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

        public bool Optional
        {
            get { return _optional; }
        }

        public QName Key
        {
            get { return _key; }

            set { _key = value; }
        }

        public string Id
        {
            get
            {
                if (_id == null)
                    return "";
                return _id;
            }

            set { _id = value; }
        }

        public abstract System.Type ValueType { get; }
        public abstract string TypeName { get; }

        #region IEquatable<Field> Members

        public bool Equals(Field other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name) && other._optional.Equals(_optional) &&
                   Equals(other._attributes, _attributes) && Equals(other._id, _id) && Equals(other._key, _key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Field)) return false;
            return Equals((Field)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_name != null ? _name.GetHashCode() : 0);
                result = (result * 397) ^ _optional.GetHashCode();
                result = (result * 397) ^ (_attributes != null ? _attributes.GetHashCode() : 0);
                result = (result * 397) ^ (_id != null ? _id.GetHashCode() : 0);
                result = (result * 397) ^ (_key != null ? _key.GetHashCode() : 0);
                return result;
            }
        }

        #endregion

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