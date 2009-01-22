using System;

namespace OpenFAST.Template
{

	public interface FieldSet
	{
		int FieldCount
		{
			get;
			
		}
		Field GetField(int index);
	}
}