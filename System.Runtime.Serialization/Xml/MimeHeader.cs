using System;
using System.Runtime.Serialization;

namespace System.Xml
{
	internal class MimeHeader
	{
		public MimeHeader(string name, string value)
		{
			if (name == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
			}
			this.name = name;
			this.value = value;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private string name;

		private string value;
	}
}
