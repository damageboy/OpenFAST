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

namespace OpenFAST
{
    public class BitVectorReader
    {
        public static readonly BitVectorReader Null;
        public static readonly BitVectorReader InfiniteTrue;

        private readonly BitVector _vector;
        private int _index;

        static BitVectorReader()
        {
            Null = new NullBitVectorReader(null);
            InfiniteTrue = new InfiniteBitVectorReader(null);
        }

        public BitVectorReader(BitVector vector)
        {
            _vector = vector;
        }

        public int Index
        {
            get { return _index; }
        }

        public virtual bool Read()
        {
            return _vector.IsSet(_index++);
        }

        public virtual bool HasMoreBitsSet
        {
            get { return _vector.IndexOfLastSet() > _index; }
        }

        public override string ToString()
        {
            return _vector.ToString();
        }

        #region Nested type: InfiniteBitVectorReader

        private sealed class InfiniteBitVectorReader : BitVectorReader
        {
            internal InfiniteBitVectorReader(BitVector vector) : base(vector)
            {
            }

            public override bool Read()
            {
                return true;
            }
        }

        #endregion

        #region Nested type: NullBitVectorReader

        private sealed class NullBitVectorReader : BitVectorReader
        {
            internal NullBitVectorReader(BitVector vector) : base(vector)
            {
            }

            public override bool Read()
            {
                throw new InvalidOperationException();
            }

            public override bool HasMoreBitsSet
            {
                get { return false; }
            }
        }

        #endregion
    }
}