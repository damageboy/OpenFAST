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
using System.IO;
using System.Text;
using OpenFAST.Template;

namespace OpenFAST.Debug
{
    public sealed class BasicEncodeTrace : ITrace
    {
        private readonly Stack<TraceGroup> _stack = new Stack<TraceGroup>();

        private StreamWriter _writer = new StreamWriter(Console.OpenStandardOutput(), Encoding.Default);

        public StreamWriter Writer
        {
            set { _writer = value; }
        }

        #region ITrace Members

        public void GroupStart(Group group)
        {
            var traceGroup = new TraceGroup(group);

            if (_stack.Count != 0)
                _stack.Peek().AddGroup(traceGroup);

            _stack.Push(traceGroup);
        }

        public void Field(Field field, IFieldValue value, IFieldValue encoded, byte[] encoding, int pmapIndex)
        {
            _stack.Peek().AddField(field, value, encoded, pmapIndex, encoding);
        }

        public void GroupEnd()
        {
            TraceGroup group = _stack.Pop();
            if (_stack.Count == 0)
                _writer.WriteLine(group);
        }

        public void Pmap(byte[] pmap)
        {
            _stack.Peek().Pmap = pmap;
        }

        #endregion

        public static string Indent(int indent)
        {
            return new string(' ', indent);
        }

        #region Nested type: ITraceNode

        private interface ITraceNode
        {
            StringBuilder Serialize(StringBuilder builder, int indent);
        }

        #endregion

        #region Nested type: TraceField

        private sealed class TraceField : ITraceNode
        {
            private readonly IFieldValue _encoded;
            private readonly byte[] _encoding;
            private readonly Field _field;

            private readonly int _pmapIndex;

            private readonly IFieldValue _value;

            public TraceField(Field field, IFieldValue value, IFieldValue encoded, int pmapIndex, byte[] encoding)
            {
                _field = field;
                _value = value;
                _encoded = encoded;
                _pmapIndex = pmapIndex;
                _encoding = encoding;
            }

            #region ITraceNode Members

            public StringBuilder Serialize(StringBuilder builder, int indent)
            {
                builder.Append(Indent(indent));
                builder.Append(_field.Name).Append("[");
                if (_field.UsesPresenceMapBit())
                    builder.Append("pmapIndex:").Append(_pmapIndex);
                builder.Append("]: ").Append(_value).Append(" = ").Append(_encoded).Append(" -> ");
                builder.Append(ByteUtil.ConvertByteArrayToBitString(_encoding));
                builder.Append("\n");
                return builder;
            }

            #endregion
        }

        #endregion

        #region Nested type: TraceGroup

        private sealed class TraceGroup : ITraceNode
        {
            private readonly Group _group;
            private readonly List<ITraceNode> _nodes;

            private byte[] _pmap;

            public TraceGroup(Group group)
            {
                _group = group;
                _nodes = new List<ITraceNode>(group.FieldCount);
            }

            public byte[] Pmap
            {
                set { _pmap = value; }
            }

            #region ITraceNode Members

            public StringBuilder Serialize(StringBuilder builder, int indent)
            {
                builder.Append(Indent(indent)).Append(_group.Name).Append("\n");
                indent += 2;
                if (_pmap != null)
                    builder.Append(Indent(indent)).Append("PMAP: ").Append(ByteUtil.ConvertByteArrayToBitString(_pmap)).
                        Append("\n");

                foreach (ITraceNode t in _nodes)
                    t.Serialize(builder, indent);

                //indent -= 2;
                return builder;
            }

            #endregion

            public void AddField(Field field, IFieldValue value, IFieldValue encoded, int fieldIndex,
                                 byte[] encoding)
            {
                _nodes.Add(new TraceField(field, value, encoded, fieldIndex, encoding));
            }

            public void AddGroup(TraceGroup traceGroup)
            {
                _nodes.Add(traceGroup);
            }

            public override string ToString()
            {
                return Serialize(new StringBuilder(), 0).ToString();
            }
        }

        #endregion
    }
}