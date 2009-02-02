using System;
using QName = OpenFAST.QName;

namespace OpenFAST.Template
{
	//Optimized by SHARIQ
	public sealed class BasicTemplateRegistry:AbstractTemplateRegistry
	{
		override public MessageTemplate[] Templates
		{
			get
			{
                return SupportClass.ICollectionSupport.ToArray <MessageTemplate>(new SupportClass.HashSetSupport(templateMap.Keys));
			}
			
		}

        //Used as a quick search and unique pair of QName vs MessageTemplate
        private System.Collections.Generic.Dictionary<object, MessageTemplate> nameMap = new System.Collections.Generic.Dictionary<object, MessageTemplate>();
        private System.Collections.Generic.Dictionary<int, MessageTemplate> idMap = new System.Collections.Generic.Dictionary<int, MessageTemplate>();
        private System.Collections.Generic.Dictionary<MessageTemplate, int> templateMap = new System.Collections.Generic.Dictionary<MessageTemplate, int>();
		private System.Collections.IList templates = new System.Collections.ArrayList();
		
		public override void  Register(int id, MessageTemplate template)
		{
			Define(template);
			System.Int32 tid = (System.Int32) id;
			idMap[tid] = template;
			templateMap[template] = tid;
			NotifyTemplateRegistered(template, id);
		}
		
		public override void  Register(int id, QName name)
		{
			if (!nameMap.ContainsKey(name))
			{
				throw new System.ArgumentException("The template named " + name + " is not defined.");
			}
			System.Int32 tid = (System.Int32) id;
			MessageTemplate template = nameMap[name];
			templateMap[template] = tid;
			idMap[tid] = template;
			NotifyTemplateRegistered(template, id);
		}
		
		public override void  Define(MessageTemplate template)
		{
			if (!templates.Contains(template))
			{
				nameMap[template.QName] = template;
				templates.Add(template);
			}
		}
		
		public override int GetId(QName name)
		{
			MessageTemplate template = nameMap[name];
			if (template == null || !templateMap.ContainsKey(template))
				return - 1;
			return ((System.Int32) templateMap[template]);
		}
		
		public override MessageTemplate get_Renamed(int templateId)
		{
			return (MessageTemplate) idMap[(System.Int32) templateId];
		}
		
		public override MessageTemplate get_Renamed(QName name)
		{
			return nameMap[name];
		}
		
		public override int GetId(MessageTemplate template)
		{
			if (!IsRegistered(template))
				return - 1;
			return ((System.Int32) templateMap[template]);
		}
		
		public override bool IsRegistered(QName name)
		{
			return nameMap.ContainsKey(name);
		}
		
		public override bool IsRegistered(int templateId)
		{
			return idMap.ContainsKey(templateId);
		}
		
		public override bool IsRegistered(MessageTemplate template)
		{
			return templateMap.ContainsKey(template);
		}
		
		public override bool IsDefined(QName name)
		{
			return nameMap.ContainsKey(name);
		}
		
		public override void  Remove(QName name)
		{
			System.Object tempObject;
			tempObject = nameMap[name];
			nameMap.Remove(name);
			MessageTemplate template = (MessageTemplate) tempObject;
			int id = templateMap[template];
			templateMap.Remove(template);
			idMap.Remove(id);
			templates.Remove(template);
		}
		[Obsolete("dont call this method")]
		public override void  Remove(MessageTemplate template)
		{
			int id = templateMap[template];
			templateMap.Remove(template);
			nameMap.Remove(template.Name);//wrong approach, what if the hashcode is matched for the string.... because its an algo in QNameclass GetHashCode() dont use it.
			idMap.Remove(id);
		}
		
		public override void  Remove(int id)
		{
            MessageTemplate template;
            template = idMap[id];
			idMap.Remove(id);
			templateMap.Remove(template);
			nameMap.Remove(template.Name);
		}
		
		public override void  RegisterAll(TemplateRegistry registry)
		{
			MessageTemplate[] templates = registry.Templates;
			for (int i = 0; i < templates.Length; i++)
			{
				Register(registry.GetId(templates[i]), templates[i]);
			}
		}
		
		public override System.Collections.IEnumerator NameIterator()
		{
			return new SupportClass.HashSetSupport(nameMap.Keys).GetEnumerator();
		}
		
		public override System.Collections.IEnumerator Iterator()
		{
			return templates.GetEnumerator();
		}
	}
}