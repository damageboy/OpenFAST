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