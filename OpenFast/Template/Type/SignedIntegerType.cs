using System;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	public class SignedIntegerType:IntegerType
	{
		private const long serialVersionUID = 1L;
		
		public SignedIntegerType(int numberBits, long min, long max):base("int" + numberBits, min, max, TypeCodec.INTEGER, TypeCodec.NULLABLE_INTEGER)
		{
		}
	}
}