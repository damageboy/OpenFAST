using System;
using QName = OpenFAST.QName;

namespace OpenFAST.Template
{
	
	sealed class NullTemplateRegistry : TemplateRegistry
	{
		public MessageTemplate[] Templates
		{
			get
			{
				return null;
			}
			
		}
		public void  AddTemplateRegisteredListener(TemplateRegisteredListener templateRegisteredListener)
		{
		}
		
		public MessageTemplate get_Renamed(int templateId)
		{
			return null;
		}
		
		public MessageTemplate get_Renamed(string templateName)
		{
			return null;
		}
		
		public int GetTemplateId(string templateName)
		{
			return 0;
		}
		
		public int GetTemplateId(MessageTemplate template)
		{
			return 0;
		}
		
		public bool IsRegistered(string templateName)
		{
			return false;
		}
		
		public bool IsRegistered(int templateId)
		{
			return false;
		}
		
		public bool IsRegistered(MessageTemplate template)
		{
			return false;
		}
		
		public void  Register(int templateId, MessageTemplate template)
		{
		}
		
		public void  Remove(string name)
		{
		}
		
		public void  Remove(MessageTemplate template)
		{
		}
		
		public void  Remove(int id)
		{
		}
		
		public void  Add(MessageTemplate template)
		{
		}
		
		public void  Define(MessageTemplate template)
		{
		}
		
		public MessageTemplate GetTemplate(string name)
		{
			return null;
		}
		
		public MessageTemplate GetTemplate(QName name)
		{
			return null;
		}
		
		public MessageTemplate GetTemplate(int id)
		{
			return null;
		}
		
		public bool HasTemplate(string name)
		{
			return false;
		}
		
		public bool HasTemplate(QName name)
		{
			return false;
		}
		
		public bool HasTemplate(int id)
		{
			return false;
		}
		
		public bool IsDefined(MessageTemplate template)
		{
			return false;
		}
		
		public MessageTemplate[] ToArray()
		{
			return null;
		}
		
		public MessageTemplate get_Renamed(QName name)
		{
			return null;
		}
		
		public int GetId(string name)
		{
			return 0;
		}
		
		public int GetId(MessageTemplate template)
		{
			return 0;
		}
		
		public bool IsDefined(QName name)
		{
			return false;
		}
		
		public bool IsDefined(string name)
		{
			return false;
		}
		
		public void  Register(int templateId, QName name)
		{
		}
		
		public void  Register(int templateId, string name)
		{
		}
		
		public void  RemoveTemplateRegisteredListener(TemplateRegisteredListener templateRegisteredListener)
		{
		}
		
		public int GetId(QName name)
		{
			return 0;
		}
		
		public bool IsRegistered(QName name)
		{
			return false;
		}
		
		public void  Remove(QName name)
		{
		}
		
		public void  RegisterAll(TemplateRegistry registry)
		{
		}
		
		public System.Collections.IEnumerator NameIterator()
		{
			return new System.Collections.ArrayList().GetEnumerator();
		}
		
		public System.Collections.IEnumerator Iterator()
		{
			return null;
		}
	}
}