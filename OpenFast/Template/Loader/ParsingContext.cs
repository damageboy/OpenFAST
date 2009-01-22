using System;
using QName = OpenFAST.QName;
using ErrorHandler = OpenFAST.Error.ErrorHandler;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST.Template.Loader
{
	public class ParsingContext
	{
		virtual public string TemplateNamespace
		{
			get
			{
				if (templateNamespace == null)
					return parent.TemplateNamespace;
				return templateNamespace;
			}
			
			set
			{
				this.templateNamespace = value;
			}
			
		}
		virtual public string Namespace
		{
			get
			{
				if (namespace_Renamed == null)
					return parent.Namespace;
				return namespace_Renamed;
			}
			
			set
			{
				this.namespace_Renamed = value;
			}
			
		}
		virtual public string Dictionary
		{
			get
			{
				if (dictionary == null)
					return parent.Dictionary;
				return dictionary;
			}
			
			set
			{
				this.dictionary = value;
			}
			
		}
		virtual public ErrorHandler ErrorHandler
		{
			get
			{
				if (errorHandler == null)
					return parent.ErrorHandler;
				return errorHandler;
			}
			
			set
			{
				this.errorHandler = value;
			}
			
		}
		virtual public TemplateRegistry TemplateRegistry
		{
			get
			{
				if (templateRegistry == null)
					return parent.TemplateRegistry;
				return templateRegistry;
			}
			
			set
			{
				this.templateRegistry = value;
			}
			
		}
		virtual public System.Collections.IDictionary TypeMap
		{
			get
			{
				if (typeMap == null)
					return parent.TypeMap;
				return typeMap;
			}
			
			set
			{
				this.typeMap = value;
			}
			
		}
		virtual public System.Collections.IList FieldParsers
		{
			get
			{
				if (fieldParsers == null)
					return parent.FieldParsers;
				return fieldParsers;
			}
			
			set
			{
				this.fieldParsers = value;
			}
			
		}
		virtual public ParsingContext Parent
		{
			get
			{
				return parent;
			}
			
		}
		
		internal static readonly ParsingContext NULL = new ParsingContext();
		
		private ParsingContext parent;
		
		private string templateNamespace = null;
		private string namespace_Renamed = null;
		private string dictionary = null;
		private ErrorHandler errorHandler;
		private TemplateRegistry templateRegistry;
		private System.Collections.IDictionary typeMap;
		private System.Collections.IList fieldParsers;
		
		private QName name;
		
		public ParsingContext():this(NULL)
		{
		}
		
		public ParsingContext(ParsingContext parent)
		{
			this.parent = parent;
		}
		
		public ParsingContext(System.Xml.XmlElement node, ParsingContext parent)
		{
			this.parent = parent;
			if (node.HasAttribute("templateNs"))
				TemplateNamespace = node.GetAttribute("templateNs");
			if (node.HasAttribute("ns"))
				Namespace = node.GetAttribute("ns");
			if (node.HasAttribute("dictionary"))
				Dictionary = node.GetAttribute("dictionary");
			if (node.HasAttribute("name"))
				setName(new QName(node.GetAttribute("name"), Namespace));
		}
		
		private void  setName(QName name)
		{
			this.name = name;
		}
		
		public virtual FieldParser GetFieldParser(System.Xml.XmlElement element)
		{
			System.Collections.IList parsers = FieldParsers;
			for (int i = parsers.Count - 1; i >= 0; i--)
			{
				FieldParser fieldParser = ((FieldParser) parsers[i]);
				if (fieldParser.CanParse(element, this))
					return fieldParser;
			}
			return null;
		}
		
		public virtual QName GetName()
		{
			return name;
		}
		
		public virtual void  AddFieldParser(FieldParser parser)
		{
			FieldParsers.Add(parser);
		}
		static ParsingContext()
		{
			
				NULL.Dictionary = "global";
				NULL.Namespace = "";
				NULL.TemplateNamespace = "";
			
		}
	}
}