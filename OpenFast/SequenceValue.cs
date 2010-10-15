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
using System.Text;
using OpenFAST.Template;

namespace OpenFAST
{
    [Serializable]
    public sealed class SequenceValue : IFieldValue
    {
        private readonly List<GroupValue> _elements = new List<GroupValue>();
        private readonly Sequence _sequence;

        public SequenceValue(Sequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence");

            _sequence = sequence;
        }

        public int Length
        {
            get { return _elements.Count; }
        }

        public Sequence Sequence
        {
            get { return _sequence; }
        }

        public IList<GroupValue> Elements
        {
            get { return _elements; }
        }

        public GroupValue this[int index]
        {
            get { return _elements[index]; }
        }

        #region FieldValue Members

        public IFieldValue Copy()
        {
            var copy = new SequenceValue(_sequence);
            for (int i = 0; i < _elements.Count; i++)
                copy.Add((GroupValue) _elements[i].Copy());
            return copy;
        }

        #endregion

        public void Add(GroupValue value)
        {
            _elements.Add(value);
        }

        public void Add(IFieldValue[] values)
        {
            _elements.Add(new GroupValue(_sequence.Group, values));
        }

        public override bool Equals(object other)
        {
            if (other == this)
                return true;

            if ((other == null) || !(other is SequenceValue))
                return false;

            return equals((SequenceValue) other);
        }

        private bool equals(SequenceValue other)
        {
            if (Length != other.Length)
                return false;

            for (int i = 0; i < Length; i++)
                if (!_elements[i].Equals(other._elements[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return _elements.GetHashCode()*37 + _sequence.GetHashCode();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[ ");
            
            foreach (var v in _elements)
                builder.Append('[').Append(v).Append("] ");

            builder.Append("]");

            return builder.ToString();
        }
    }
}