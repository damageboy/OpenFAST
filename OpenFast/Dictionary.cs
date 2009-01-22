using System;
using Group = OpenFAST.Template.Group;

namespace OpenFAST
{
	
	
	public struct Dictionary_Fields{
		public readonly static string TEMPLATE = "template";
		public readonly static string GLOBAL = "global";
	}
	public interface Dictionary
	{
        ScalarValue Lookup(Group template, QName key, QName currentApplicationType);
        void Store(Group group, QName applicationType, QName key, ScalarValue valueToEncode);
        void Reset();
	}
}