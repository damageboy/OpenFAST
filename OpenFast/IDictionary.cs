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
using OpenFAST.Template;

namespace OpenFAST
{
    public static class DictionaryFields
    {
        public const string Global = "global";
        public const string Template = "template";
        public const string Type = "type";

        /// <summary>
        /// Improve performance by using globally defined constants above
        /// <see cref="string.Intern"/> might be a better alternative.
        /// </summary>
        internal static string InternDictionaryName(string dictionary)
        {
            switch (dictionary)
            {
                case Global:
                    return Global;
                case Template:
                    return Template;
                case Type:
                    return Type;
                default:
                    return dictionary;
            }
        }
    }

    public interface IDictionary
    {
        ScalarValue Lookup(Group template, QName key, QName applicationType);
        void Store(Group template, QName key, QName applicationType, ScalarValue value);
        void Reset();
    }
}