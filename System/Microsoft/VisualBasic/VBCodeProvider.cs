using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;

namespace Microsoft.VisualBasic
{
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class VBCodeProvider : global::System.CodeDom.Compiler.CodeDomProvider
	{
		public VBCodeProvider()
		{
		}

		public VBCodeProvider(IDictionary<string, string> providerOptions)
		{
		}

		public override string FileExtension
		{
			get
			{
				return "vb";
			}
		}

		public override global::System.CodeDom.Compiler.LanguageOptions LanguageOptions
		{
			get
			{
				return global::System.CodeDom.Compiler.LanguageOptions.CaseInsensitive;
			}
		}

		[Obsolete("Use CodeDomProvider class")]
		public override global::System.CodeDom.Compiler.ICodeCompiler CreateCompiler()
		{
			return new VBCodeCompiler();
		}

		[Obsolete("Use CodeDomProvider class")]
		public override global::System.CodeDom.Compiler.ICodeGenerator CreateGenerator()
		{
			return new VBCodeGenerator();
		}

		public override global::System.ComponentModel.TypeConverter GetConverter(Type type)
		{
			return global::System.ComponentModel.TypeDescriptor.GetConverter(type);
		}

		[global::System.MonoTODO]
		public override void GenerateCodeFromMember(global::System.CodeDom.CodeTypeMember member, TextWriter writer, global::System.CodeDom.Compiler.CodeGeneratorOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
