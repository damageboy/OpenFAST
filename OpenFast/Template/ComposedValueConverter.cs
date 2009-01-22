using System;
using FieldValue = OpenFAST.FieldValue;

namespace OpenFAST.Template
{
	
	public interface ComposedValueConverter
	{
		FieldValue[] Split(FieldValue value_Renamed);
		FieldValue Compose(FieldValue[] values);
	}
}