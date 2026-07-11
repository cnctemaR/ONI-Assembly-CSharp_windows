using System;

namespace System.Xml.Schema
{
	public sealed class XmlSchemaCompilationSettings
	{
		public XmlSchemaCompilationSettings()
		{
			this.enableUpaCheck = true;
		}

		public bool EnableUpaCheck
		{
			get
			{
				return this.enableUpaCheck;
			}
			set
			{
				this.enableUpaCheck = value;
			}
		}

		private bool enableUpaCheck;
	}
}
