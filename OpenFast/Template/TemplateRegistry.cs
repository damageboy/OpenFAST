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