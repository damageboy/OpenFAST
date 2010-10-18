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

namespace OpenFAST.Utility
{
    public static class Tuple
    {
        public static Tuple<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second)
        {
            return new Tuple<TFirst, TSecond>(first, second);
        }
    }

    [Serializable]
    public class Tuple<TItem1, TItem2> : IEquatable<Tuple<TItem1, TItem2>>
    {
        private readonly TItem1 _item1;
        private readonly TItem2 _item2;

        public Tuple(TItem1 item1, TItem2 item2)
        {
            _item1 = item1;
            _item2 = item2;
        }

        public TItem1 Item1
        {
            get { return _item1; }
        }

        public TItem2 Item2
        {
            get { return _item2; }
        }

        #region IEquatable<Tuple<TItem1,TItem2>> Members

        public bool Equals(Tuple<TItem1, TItem2> other)
        {
            return Equals(other._item1, _item1) && Equals(other._item2, _item2);
        }

        #endregion

        public static bool operator ==(Tuple<TItem1, TItem2> left, Tuple<TItem1, TItem2> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tuple<TItem1, TItem2> left, Tuple<TItem1, TItem2> right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Tuple<TItem1, TItem2>)) return false;
            return Equals((Tuple<TItem1, TItem2>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable CompareNonConstrainedGenericWithNull
                return ((_item1 == null ? 0 : _item1.GetHashCode())*397) ^
                       (_item2 == null ? 0 : _item2.GetHashCode());
                // ReSharper restore CompareNonConstrainedGenericWithNull
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", _item1, _item2);
        }
    }
}