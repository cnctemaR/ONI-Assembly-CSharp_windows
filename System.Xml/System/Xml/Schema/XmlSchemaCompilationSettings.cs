using System;

namespace System.Xml.Schema
{
	public sealed class XmlSchemaCompilationSettings
	{
		public bool EnableUpaCheck
		{
			get
			{
				return this.enable_upa_check;
			}
			set
			{
				this.enable_upa_check = value;
			}
		}

		private bool enable_upa_check = true;
	}
}
