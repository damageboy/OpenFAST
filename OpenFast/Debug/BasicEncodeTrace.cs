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
using System.IO;
using System.Text;
using OpenFAST.Template;

namespace OpenFAST.Debug
{
    public sealed class BasicEncodeTrace : Trace
    {
        private readonly ArrayList stack = new ArrayList();

        private StreamWriter out_Renamed = new StreamWriter(Console.OpenStandardOutput(),
                                                            Encoding.Default);

        public StreamWriter Writer
        {
            set { out_Renamed = value; }
        }

        #region Trace Members

        public void GroupStart(Group group)
        {
            var traceGroup = new TraceGroup(group);
            if (!(stack.Count == 0))
            {
                ((TraceGroup) stack[stack.Count - 1]).AddGroup(traceGroup);
            }
            stack.Add(traceGroup);
        }

        public void Field(Field field, FieldValue value_Renamed, FieldValue encoded, byte[] encoding, int pmapIndex)
        {
            ((TraceGroup) stack[stack.Count - 1]).AddField(field, value_Renamed, encoded, pmapIndex, encoding);
        }

        public void GroupEnd()
        {
            var group = (TraceGroup) SupportClass.StackSupport.Pop(stack);
            if ((stack.Count == 0))
            {
                out_Renamed.WriteLine(group);
            }
        }

        public void Pmap(byte[] pmap)
        {
            ((TraceGroup) stack[stack.Count - 1]).Pmap = pmap;
        }

        #endregion

        public static string Indent(int indent)
        {
            string tab = "";
            for (int i = 0; i < indent; i++)
                tab += " ";
            return tab;
        }

        #region Nested type: TraceField

        private class TraceField : TraceNode
        {
            private readonly FieldValue encoded;
            private readonly byte[] encoding;
            private readonly Field field;

            private readonly int pmapIndex;

            private readonly FieldValue value_Renamed;

            public TraceField(Field field, FieldValue value_Renamed, FieldValue encoded, int pmapIndex, byte[] encoding)
            {
                this.field = field;
                this.value_Renamed = value_Renamed;
                this.encoded = encoded;
                this.pmapIndex = pmapIndex;
                this.encoding = encoding;
            }

            #region TraceNode Members

            public virtual StringBuilder Serialize(StringBuilder builder, int indent)
            {
                builder.Append(Indent(indent));
                builder.Append(field.Name).Append("[");
                if (field.UsesPresenceMapBit())
                    builder.Append("pmapIndex:").Append(pmapIndex);
                builder.Append("]: ").Append(value_Renamed).Append(" = ").Append(encoded).Append(" -> ");
                builder.Append(ByteUtil.ConvertByteArrayToBitString(encoding));
                builder.Append("\n");
                return builder;
            }

            #endregion
        }

        #endregion

        #region Nested type: TraceGroup

        private class TraceGroup : TraceNode
        {
            private readonly Group group;
            private readonly IList nodes;

            private byte[] pmap;

            public TraceGroup(Group group)
            {
                this.group = group;
                nodes = new ArrayList(group.FieldCount);
            }

            public virtual byte[] Pmap
            {
                set { pmap = value; }
            }

            #region TraceNode Members

            public virtual StringBuilder Serialize(StringBuilder builder, int indent)
            {
                builder.Append(Indent(indent)).Append(group.Name).Append("\n");
                indent += 2;
                if (pmap != null)
                    builder.Append(Indent(indent)).Append("PMAP: ").Append(ByteUtil.ConvertByteArrayToBitString(pmap)).
                        Append("\n");
                for (int i = 0; i < nodes.Count; i++)
                {
                    ((TraceNode) nodes[i]).Serialize(builder, indent);
                }
                //indent -= 2;
                return builder;
            }

            #endregion

            public virtual void AddField(Field field, FieldValue value_Renamed, FieldValue encoded, int fieldIndex,
                                         byte[] encoding)
            {
                nodes.Add(new TraceField(field, value_Renamed, encoded, fieldIndex, encoding));
            }

            public virtual void AddGroup(TraceGroup traceGroup)
            {
                nodes.Add(traceGroup);
            }

            public override string ToString()
            {
                return Serialize(new StringBuilder(), 0).ToString();
            }
        }

        #endregion

        #region Nested type: TraceNode

        private interface TraceNode
        {
            StringBuilder Serialize(StringBuilder builder, int indent);
        }

        #endregion
    }
}