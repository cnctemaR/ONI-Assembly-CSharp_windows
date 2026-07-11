using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeCompileUnit : CodeObject
	{
		public CodeAttributeDeclarationCollection AssemblyCustomAttributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new CodeAttributeDeclarationCollection();
				}
				return this.attributes;
			}
		}

		public CodeNamespaceCollection Namespaces
		{
			get
			{
				if (this.namespaces == null)
				{
					this.namespaces = new CodeNamespaceCollection();
				}
				return this.namespaces;
			}
		}

		public global::System.Collections.Specialized.StringCollection ReferencedAssemblies
		{
			get
			{
				if (this.assemblies == null)
				{
					this.assemblies = new global::System.Collections.Specialized.StringCollection();
				}
				return this.assemblies;
			}
		}

		public CodeDirectiveCollection StartDirectives
		{
			get
			{
				if (this.startDirectives == null)
				{
					this.startDirectives = new CodeDirectiveCollection();
				}
				return this.startDirectives;
			}
		}

		public CodeDirectiveCollection EndDirectives
		{
			get
			{
				if (this.endDirectives == null)
				{
					this.endDirectives = new CodeDirectiveCollection();
				}
				return this.endDirectives;
			}
		}

		private CodeAttributeDeclarationCollection attributes;

		private CodeNamespaceCollection namespaces;

		private global::System.Collections.Specialized.StringCollection assemblies;

		private CodeDirectiveCollection startDirectives;

		private CodeDirectiveCollection endDirectives;
	}
}
