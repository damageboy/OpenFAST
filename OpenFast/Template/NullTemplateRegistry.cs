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