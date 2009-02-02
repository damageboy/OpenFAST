using System;
using ByteUtil = OpenFAST.ByteUtil;
using FieldValue = OpenFAST.FieldValue;
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
				this.out_Renamed = value;
			}
			
		}
		private string indent = "";
		private System.IO.StreamWriter out_Renamed = new System.IO.StreamWriter(System.Console.OpenStandardOutput(), System.Text.Encoding.Default);
		
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
			StringBuilder scalarDecode = new StringBuilder();
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