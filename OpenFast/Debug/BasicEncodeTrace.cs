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
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using System.Text;

namespace OpenFAST.Debug
{
	public sealed class BasicEncodeTrace : Trace
	{
		public System.IO.StreamWriter Writer
		{
			set
			{
				out_Renamed = value;
			}
			
		}
		
		private readonly System.Collections.ArrayList stack = new System.Collections.ArrayList();
		private System.IO.StreamWriter out_Renamed = new System.IO.StreamWriter(System.Console.OpenStandardOutput(), Encoding.Default);
		
		public void  GroupStart(Group group)
		{
			var traceGroup = new TraceGroup(group);
			if (!(stack.Count == 0))
			{
				((TraceGroup) stack[stack.Count - 1]).AddGroup(traceGroup);
			}
			stack.Add(traceGroup);
		}
		
		public void  Field(Field field, FieldValue value_Renamed, FieldValue encoded, byte[] encoding, int pmapIndex)
		{
			((TraceGroup) stack[stack.Count - 1]).AddField(field, value_Renamed, encoded, pmapIndex, encoding);
		}
		
		public void  GroupEnd()
		{
			var group = (TraceGroup) SupportClass.StackSupport.Pop(stack);
			if ((stack.Count == 0))
			{
				out_Renamed.WriteLine(group);
			}
		}
		
		public void  Pmap(byte[] pmap)
		{
			((TraceGroup) stack[stack.Count - 1]).Pmap = pmap;
		}
		
		private class TraceGroup : TraceNode
		{
		    virtual public byte[] Pmap
			{
				set
				{
					pmap = value;
				}
				
			}

		    private readonly System.Collections.IList nodes;
			
			private byte[] pmap;
			
			private readonly Group group;
			
			public TraceGroup(Group group)
			{
				this.group = group;
				nodes = new System.Collections.ArrayList(group.FieldCount);
			}
			
			public virtual void  AddField(Field field, FieldValue value_Renamed, FieldValue encoded, int fieldIndex, byte[] encoding)
			{
				nodes.Add(new TraceField(field, value_Renamed, encoded, fieldIndex, encoding));
			}
			
			public virtual void  AddGroup(TraceGroup traceGroup)
			{
				nodes.Add(traceGroup);
			}
			
			public virtual StringBuilder Serialize(StringBuilder builder, int indent)
			{
				builder.Append(Indent(indent)).Append(group.Name).Append("\n");
				indent += 2;
				if (pmap != null)
					builder.Append(Indent(indent)).Append("PMAP: ").Append(ByteUtil.ConvertByteArrayToBitString(pmap)).Append("\n");
				for (int i = 0; i < nodes.Count; i++)
				{
					((TraceNode) nodes[i]).Serialize(builder, indent);
				}
				//indent -= 2;
				return builder;
			}
			
			public override string ToString()
			{
				return Serialize(new StringBuilder(), 0).ToString();
			}
		}

        private class TraceField : TraceNode
		{
            private readonly Field field;
			
			private readonly int pmapIndex;
			
			private readonly byte[] encoding;
			
			private readonly FieldValue value_Renamed;
			
			private readonly FieldValue encoded;
			
			public TraceField(Field field, FieldValue value_Renamed, FieldValue encoded, int pmapIndex, byte[] encoding)
			{
				this.field = field;
				this.value_Renamed = value_Renamed;
				this.encoded = encoded;
				this.pmapIndex = pmapIndex;
				this.encoding = encoding;
			}
			
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
		}
		
		private interface TraceNode
		{
			StringBuilder Serialize(StringBuilder builder, int indent);
		}
		
		public static string Indent(int indent)
		{
			string tab = "";
			for (int i = 0; i < indent; i++)
				tab += " ";
			return tab;
		}
	}
}