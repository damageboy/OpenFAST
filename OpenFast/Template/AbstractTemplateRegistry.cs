using System;
using QName = OpenFAST.QName;

namespace OpenFAST.Template
{
	
	public abstract class AbstractTemplateRegistry : TemplateRegistry
	{
		public abstract OpenFAST.Template.MessageTemplate[] Templates{get;}
		private System.Collections.ArrayList listeners = new System.Collections.ArrayList();
		
		public virtual MessageTemplate get_Renamed(string name)
		{
			return get_Renamed(new QName(name, ""));
		}
		
		public virtual int GetId(string name)
		{
			return GetId(new QName(name, ""));
		}
		
		public virtual bool IsDefined(string name)
		{
			return IsDefined(new QName(name, ""));
		}
		
		public virtual bool IsRegistered(string name)
		{
			return IsRegistered(new QName(name, ""));
		}
		
		public virtual void  Register(int templateId, string name)
		{
			Register(templateId, new QName(name, ""));
		}
		
		public virtual void  Remove(string name)
		{
			Remove(new QName(name, ""));
		}
		
		protected internal virtual void  NotifyTemplateRegistered(MessageTemplate template, int id)
		{
			for (int i = 0; i < listeners.Count; i++)
				((TemplateRegisteredListener) listeners[i]).TemplateRegistered(template, id);
		}
		
		public virtual void  AddTemplateRegisteredListener(TemplateRegisteredListener templateRegisteredListener)
		{
			this.listeners.Add(templateRegisteredListener);
		}
		
		public virtual void  RemoveTemplateRegisteredListener(TemplateRegisteredListener templateRegisteredListener)
		{
			this.listeners.Remove(templateRegisteredListener);
		}
		public abstract void  Define(OpenFAST.Template.MessageTemplate param1);
		public abstract OpenFAST.Template.MessageTemplate get_Renamed(int param1);
		public abstract int GetId(OpenFAST.QName param1);
		public abstract void  Remove(OpenFAST.QName param1);
		public abstract OpenFAST.Template.MessageTemplate get_Renamed(OpenFAST.QName param1);
		public abstract bool IsRegistered(int param1);
		public abstract System.Collections.IEnumerator NameIterator();
		public abstract void  Register(int param1, OpenFAST.Template.MessageTemplate param2);
		public abstract void  Remove(OpenFAST.Template.MessageTemplate param1);
		public abstract bool IsDefined(OpenFAST.QName param1);
		public abstract System.Collections.IEnumerator Iterator();
		public abstract bool IsRegistered(OpenFAST.Template.MessageTemplate param1);
		public abstract bool IsRegistered(OpenFAST.QName param1);
		public abstract void  RegisterAll(OpenFAST.Template.TemplateRegistry param1);
		public abstract void  Remove(int param1);
		public abstract int GetId(OpenFAST.Template.MessageTemplate param1);
		public abstract void  Register(int param1, OpenFAST.QName param2);
	}
}