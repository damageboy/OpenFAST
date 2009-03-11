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
	public sealed class BasicDecodeTrace : Trace
	{
		public System.IO.StreamWriter Writer
		{
			set
			{
				out_Renamed = value;
			}
			
		}
		private string indent = "";
		private System.IO.StreamWriter out_Renamed = new System.IO.StreamWriter(System.Console.OpenStandardOutput(), Encoding.Default);
		
		public void  GroupStart(Group group)
		{
			print(group);
			moveDown();
		}
		
		private void  moveDown()
		{
			indent += "  ";
		}
		
		private void  moveUp()
		{
			indent = indent.Substring(0, (indent.Length - 2) - (0));
		}
		
		private void  print(System.Object object_Renamed)
		{
			out_Renamed.Write(indent);
			out_Renamed.WriteLine(object_Renamed);
		}
		
		public void  GroupEnd()
		{
			moveUp();
		}
		
		public void  Field(Field field, FieldValue value_Renamed, FieldValue decodedValue, byte[] encoding, int pmapIndex)
		{
			var scalarDecode = new StringBuilder();
			scalarDecode.Append(field.Name).Append(": ");
			scalarDecode.Append(ByteUtil.ConvertByteArrayToBitString(encoding));
			scalarDecode.Append(" -> ").Append(value_Renamed).Append('(').Append(decodedValue).Append(')');
			print(scalarDecode);
		}
		
		public void  Pmap(byte[] bytes)
		{
			print("PMAP: " + ByteUtil.ConvertByteArrayToBitString(bytes));
		}
	}
}