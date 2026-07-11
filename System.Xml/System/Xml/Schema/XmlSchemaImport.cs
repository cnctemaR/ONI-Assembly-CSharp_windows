using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaImport : XmlSchemaExternal
	{
		public XmlSchemaImport()
		{
			base.Compositor = Compositor.Import;
		}

		[XmlAttribute("namespace", DataType = "anyURI")]
		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.ns = value;
			}
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

		private string ns;

		private XmlSchemaAnnotation annotation;
	}
}
