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
using System.Collections.Generic;
using System.Xml;
using OpenFAST.Error;
using OpenFAST.Template.Types;

namespace OpenFAST.Template.Loader
{
    public class ParsingContext
    {
        public static readonly ParsingContext Null = new ParsingContext();
        private readonly QName _name;

        private readonly ParsingContext _parent;

        private string _dictionary;
        private IErrorHandler _errorHandler;
        private List<IFieldParser> _fieldParsers;

        private string _namespace;
        private string _templateNamespace;
        private ITemplateRegistry _templateRegistry;
        private Dictionary<string, FASTType> _typeMap;

        static ParsingContext()
        {
            Null.Dictionary = DictionaryFields.Global;
            Null.Namespace = "";
            Null.TemplateNamespace = "";
        }

        public ParsingContext() : this(Null)
        {
        }

        public ParsingContext(ParsingContext parent)
        {
            _parent = parent;
        }

        public ParsingContext(XmlElement node, ParsingContext parent)
        {
            _parent = parent;
            if (node.HasAttribute("templateNs"))
                TemplateNamespace = node.GetAttribute("templateNs");
            if (node.HasAttribute("ns"))
                Namespace = node.GetAttribute("ns");
            if (node.HasAttribute("dictionary"))
                Dictionary = node.GetAttribute("dictionary");
            if (node.HasAttribute("name"))
                _name = new QName(node.GetAttribute("name"), Namespace);
        }

        public string TemplateNamespace
        {
            get { return _templateNamespace ?? _parent.TemplateNamespace; }
            set { _templateNamespace = value; }
        }

        public string Namespace
        {
            get { return _namespace ?? _parent.Namespace; }
            set { _namespace = value; }
        }

        public string Dictionary
        {
            get { return _dictionary ?? _parent.Dictionary; }
            set { _dictionary = DictionaryFields.InternDictionaryName(value); }
        }

        public virtual IErrorHandler ErrorHandler
        {
            get { return _errorHandler ?? _parent.ErrorHandler; }
            set { _errorHandler = value; }
        }

        public virtual ITemplateRegistry TemplateRegistry
        {
            get { return _templateRegistry ?? _parent.TemplateRegistry; }
            set { _templateRegistry = value; }
        }

        public virtual Dictionary<string, FASTType> TypeMap
        {
            get { return _typeMap ?? _parent.TypeMap; }
            set { _typeMap = value; }
        }

        public virtual List<IFieldParser> FieldParsers
        {
            get { return _fieldParsers ?? _parent.FieldParsers; }
            set { _fieldParsers = value; }
        }

        public virtual ParsingContext Parent
        {
            get { return _parent; }
        }

        public virtual QName Name
        {
            get { return _name; }
        }

        public virtual IFieldParser GetFieldParser(XmlElement element)
        {
            List<IFieldParser> parsers = FieldParsers;
            for (int i = parsers.Count - 1; i >= 0; i--)
            {
                IFieldParser fieldParser = parsers[i];
                if (fieldParser.CanParse(element, this))
                    return fieldParser;
            }
            return null;
        }

        public virtual void AddFieldParser(IFieldParser parser)
        {
            FieldParsers.Add(parser);
        }
    }
}