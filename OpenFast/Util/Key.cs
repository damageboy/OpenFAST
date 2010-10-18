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

namespace OpenFAST.util
{
    public sealed class Key
    {
        private readonly Object[] _keys;

        public Key(Object key1, Object key2) : this(new[] {key1, key2})
        {
        }

        public Key(Object key1, Object key2, Object key3) : this(new[] {key1, key2, key3})
        {
        }

        public Key(Object[] keys)
        {
            _keys = keys;
            CheckNull();
        }

        private void CheckNull()
        {
            for (int i = 0; i < _keys.Length; i++)
                if (_keys[i] == null)
                    throw new NullReferenceException();
        }

        public override bool Equals(Object obj)
        {
            if(ReferenceEquals(obj,this))
                return true;
            if (ReferenceEquals(obj, null))// || !(obj is Key))
                return false;

            var other = ((Key) obj);
            if (other._keys.Length != _keys.Length)
                return false;
            for (int i = 0; i < _keys.Length; i++)
                if (!other._keys[i].Equals(_keys[i]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int i = 0; i < _keys.Length; i++)
                hashCode += _keys[i].GetHashCode()*(37 ^ i);
            return hashCode;
        }

        public override string ToString()
        {
            return "";
        }
    }
}