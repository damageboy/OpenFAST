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
using OpenFAST.Template;

namespace OpenFAST.Sessions.Template.Exchange
{
    public class ConversionContext
    {
        private readonly Dictionary<Group, IFieldInstructionConverter> _converterTemplateMap =
            new Dictionary<Group, IFieldInstructionConverter>();

        private readonly List<IFieldInstructionConverter> _converters =
            new List<IFieldInstructionConverter>();

        public virtual void AddFieldInstructionConverter(IFieldInstructionConverter converter)
        {
            foreach (Group t in converter.TemplateExchangeTemplates)
                _converterTemplateMap[t] = converter;

            _converters.Add(converter);
        }

        public virtual IFieldInstructionConverter GetConverter(Group group)
        {
            return _converterTemplateMap[group];
        }

        public virtual IFieldInstructionConverter GetConverter(Field field)
        {
            for (int i = _converters.Count - 1; i >= 0; i--)
                if (_converters[i].ShouldConvert(field))
                    return _converters[i];

            throw new ArgumentOutOfRangeException("field", field, "No valid converter found for the field");
        }
    }
}