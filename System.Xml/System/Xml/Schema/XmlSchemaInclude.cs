using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaInclude : XmlSchemaExternal
	{
		public XmlSchemaInclude()
		{
			base.Compositor = Compositor.Include;
		}

		[XmlElement("annotation", typeof(XmlSchemaAnnotation))]
		public XmlSchemaAnnotation Annotation
		{
			get
			{
				return this.annotation;
			}
			set
			{
				this.annotation = value;
			}
		}

		internal override void AddAnnotation(XmlSchemaAnnotation annotation)
		{
			this.annotation = annotation;
		}

		private XmlSchemaAnnotation annotation;
	}
}
