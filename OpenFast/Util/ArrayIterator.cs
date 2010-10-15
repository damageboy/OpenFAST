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
using System.Collections;

namespace OpenFAST.util
{
    public sealed class ArrayIterator : IEnumerator
    {
        private readonly Object[] array;
        private int position;

        public ArrayIterator(Object[] array)
        {
            this.array = array;
        }

        #region IEnumerator Members

        public Object Current
        {
            get { return array[position++]; }
        }

        public bool MoveNext()
        {
            return position < array.Length;
        }

        public void Reset()
        {
        }

        #endregion
    }
}