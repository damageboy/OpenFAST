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
using System.Xml;
using OpenFAST.Template.Type;

namespace OpenFAST.Template.Loader
{
    public class SequenceParser : AbstractFieldParser
    {
        private ScalarParser sequenceLengthParser;

        public SequenceParser() : base("sequence")
        {
            InitBlock();
        }

        private void InitBlock()
        {
            sequenceLengthParser = new SequenceLengthParser(this, "length");
        }

        public override Field Parse(XmlElement sequenceElement, bool optional, ParsingContext context)
        {
            var sequence = new Sequence(context.Name,
                                        ParseSequenceLengthField(context.Name, sequenceElement, optional, context),
                                        GroupParser.ParseFields(sequenceElement, context), optional);
            GroupParser.ParseMore(sequenceElement, sequence.Group, context);
            return sequence;
        }

        private Scalar ParseSequenceLengthField(QName name, XmlElement sequence, bool optional,
                                                ParsingContext parent)
        {
            XmlNodeList lengthElements = sequence.GetElementsByTagName("length");

            if (lengthElements.Count == 0)
            {
                var implicitLength = new Scalar(Global.CreateImplicitName(name), FASTType.U32, Operator.Operator.NONE,
                                                ScalarValue.Undefined, optional)
                                         {Dictionary = parent.Dictionary};
                return implicitLength;
            }

            var length = (XmlElement) lengthElements.Item(0);
            var context = new ParsingContext(length, parent);
            return (Scalar) sequenceLengthParser.Parse(length, optional, context);
        }

        #region Nested type: SequenceLengthParser

        public class SequenceLengthParser : ScalarParser
        {
            private SequenceParser enclosingInstance;

            internal SequenceLengthParser(SequenceParser enclosingInstance, string Param1) : base(Param1)
            {
                InitBlock(enclosingInstance);
            }

            public SequenceParser Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            private void InitBlock(SequenceParser internalInstance)
            {
                enclosingInstance = internalInstance;
            }

            protected internal override FASTType GetType(XmlElement fieldNode, ParsingContext context)
            {
                return FASTType.U32;
            }

            protected internal override QName GetName(XmlElement fieldNode, ParsingContext context)
            {
                if (context.Name == null)
                    return Global.CreateImplicitName(context.Parent.Name);
                return context.Name;
            }
        }

        #endregion
    }
}