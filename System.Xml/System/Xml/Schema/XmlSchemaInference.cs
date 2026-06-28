using System;

namespace System.Xml.Schema
{
	[MonoTODO]
	public sealed class XmlSchemaInference
	{
		public XmlSchemaInference.InferenceOption Occurrence
		{
			get
			{
				return this.occurrence;
			}
			set
			{
				this.occurrence = value;
			}
		}

		public XmlSchemaInference.InferenceOption TypeInference
		{
			get
			{
				return this.typeInference;
			}
			set
			{
				this.typeInference = value;
			}
		}

		public XmlSchemaSet InferSchema(XmlReader xmlReader)
		{
			return this.InferSchema(xmlReader, new XmlSchemaSet());
		}

		public XmlSchemaSet InferSchema(XmlReader xmlReader, XmlSchemaSet schemas)
		{
			return XsdInference.Process(xmlReader, schemas, this.occurrence == XmlSchemaInference.InferenceOption.Relaxed, this.typeInference == XmlSchemaInference.InferenceOption.Relaxed);
		}

		private XmlSchemaInference.InferenceOption occurrence;

		private XmlSchemaInference.InferenceOption typeInference;

		public enum InferenceOption
		{
			Restricted,
			Relaxed
		}
	}
}
