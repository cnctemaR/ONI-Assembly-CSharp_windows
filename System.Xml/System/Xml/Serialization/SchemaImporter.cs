using System;
using System.Xml.Serialization.Advanced;

namespace System.Xml.Serialization
{
	public abstract class SchemaImporter
	{
		internal SchemaImporter()
		{
		}

		public SchemaImporterExtensionCollection Extensions
		{
			get
			{
				if (this.extensions == null)
				{
					this.extensions = new SchemaImporterExtensionCollection();
				}
				return this.extensions;
			}
		}

		private SchemaImporterExtensionCollection extensions;
	}
}
