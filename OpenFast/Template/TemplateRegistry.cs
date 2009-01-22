using System;
using QName = OpenFAST.QName;

namespace OpenFAST.Template
{
    public struct TemplateRegistry_Fields{
		public readonly static TemplateRegistry NULL;
		static TemplateRegistry_Fields()
		{
			NULL = new NullTemplateRegistry();
		}
	}
	public interface TemplateRegistry
	{
		MessageTemplate[] Templates
		{
			get;
			
		}
		
		void  RegisterAll(TemplateRegistry registry);
		void  Register(int id, MessageTemplate template);
		void  Register(int id, string name);
		void  Register(int id, QName name);
		
		void  Define(MessageTemplate template);
		
		void  Remove(string name);
		void  Remove(QName name);
		void  Remove(MessageTemplate template);
		void  Remove(int id);
		
		MessageTemplate get_Renamed(int id);
		MessageTemplate get_Renamed(string name);
		MessageTemplate get_Renamed(QName name);
		
		int GetId(string name);
		int GetId(QName name);
		int GetId(MessageTemplate template);
		
		bool IsRegistered(string name);
		bool IsRegistered(QName name);
		bool IsRegistered(int id);
		bool IsRegistered(MessageTemplate template);
		bool IsDefined(string name);
		bool IsDefined(QName name);
		
		void  AddTemplateRegisteredListener(TemplateRegisteredListener templateRegisteredListener);
		void  RemoveTemplateRegisteredListener(TemplateRegisteredListener templateRegisteredListener);
		
		System.Collections.IEnumerator NameIterator();
		
		System.Collections.IEnumerator Iterator();
	}
}