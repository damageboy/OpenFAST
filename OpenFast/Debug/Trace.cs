using System;
using FieldValue = OpenFAST.FieldValue;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;

namespace OpenFAST.Debug
{
	
	public interface Trace
	{
		void  GroupStart(Group group);
		void  GroupEnd();
		void  Field(Field field, FieldValue value_Renamed, FieldValue encoded, sbyte[] encoding, int pmapIndex);
		void  Pmap(sbyte[] pmap);
	}
}