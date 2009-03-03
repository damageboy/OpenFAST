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

namespace OpenFAST.Session
{
	
	public struct SessionFactory_Fields{
		public readonly static SessionFactory NULL;
		static SessionFactory_Fields()
		{
			NULL = new NULLSessionFactory();
		}
	}
	public class NULLSessionFactory : SessionFactory
	{
		virtual public Session Session
		{
			get
			{
				return null;
			}
			
		}
		public virtual void  Close()
		{
		}
		
		public virtual Client GetClient(string serverName)
		{
			return null;
		}
	}
	public interface SessionFactory
	{
		Session Session
		{
			get;
			
		}
		
		Client GetClient(string serverName);
		
		void  Close();
	}
}