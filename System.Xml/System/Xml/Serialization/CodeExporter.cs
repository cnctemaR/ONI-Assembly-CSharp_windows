using System;
using System.CodeDom;

namespace System.Xml.Serialization
{
	public abstract class CodeExporter
	{
		internal CodeExporter()
		{
		}

		public CodeAttributeDeclarationCollection IncludeMetadata
		{
			get
			{
				return this.codeGenerator.IncludeMetadata;
			}
		}

		internal MapCodeGenerator codeGenerator;
	}
}
