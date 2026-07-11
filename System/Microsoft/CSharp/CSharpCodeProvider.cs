using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;
using Mono.CSharp;

namespace Microsoft.CSharp
{
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class CSharpCodeProvider : global::System.CodeDom.Compiler.CodeDomProvider
	{
		public CSharpCodeProvider()
		{
		}

		public CSharpCodeProvider(IDictionary<string, string> providerOptions)
		{
			this.providerOptions = providerOptions;
		}

		public override string FileExtension
		{
			get
			{
				return "cs";
			}
		}

		[Obsolete("Use CodeDomProvider class")]
		public override global::System.CodeDom.Compiler.ICodeCompiler CreateCompiler()
		{
			if (this.providerOptions != null && this.providerOptions.Count > 0)
			{
				return new Mono.CSharp.CSharpCodeCompiler(this.providerOptions);
			}
			return new Mono.CSharp.CSharpCodeCompiler();
		}

		[Obsolete("Use CodeDomProvider class")]
		public override global::System.CodeDom.Compiler.ICodeGenerator CreateGenerator()
		{
			if (this.providerOptions != null && this.providerOptions.Count > 0)
			{
				return new Mono.CSharp.CSharpCodeGenerator(this.providerOptions);
			}
			return new Mono.CSharp.CSharpCodeGenerator();
		}

		[global::System.MonoTODO]
		public override global::System.ComponentModel.TypeConverter GetConverter(Type Type)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override void GenerateCodeFromMember(global::System.CodeDom.CodeTypeMember member, TextWriter writer, global::System.CodeDom.Compiler.CodeGeneratorOptions options)
		{
			throw new NotImplementedException();
		}

		private IDictionary<string, string> providerOptions;
	}
}
