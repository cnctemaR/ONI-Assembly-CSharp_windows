using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.CodeDom.Compiler
{
	[ComVisible(true)]
	[global::System.ComponentModel.ToolboxItem(false)]
	public abstract class CodeDomProvider : global::System.ComponentModel.Component
	{
		public virtual string FileExtension
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual LanguageOptions LanguageOptions
		{
			get
			{
				return LanguageOptions.None;
			}
		}

		[Obsolete("ICodeCompiler is obsolete")]
		public abstract ICodeCompiler CreateCompiler();

		[Obsolete("ICodeGenerator is obsolete")]
		public abstract ICodeGenerator CreateGenerator();

		public virtual ICodeGenerator CreateGenerator(string fileName)
		{
			return this.CreateGenerator();
		}

		public virtual ICodeGenerator CreateGenerator(TextWriter output)
		{
			return this.CreateGenerator();
		}

		[Obsolete("ICodeParser is obsolete")]
		public virtual ICodeParser CreateParser()
		{
			return null;
		}

		public virtual global::System.ComponentModel.TypeConverter GetConverter(Type type)
		{
			return global::System.ComponentModel.TypeDescriptor.GetConverter(type);
		}

		public virtual CompilerResults CompileAssemblyFromDom(CompilerParameters options, params CodeCompileUnit[] compilationUnits)
		{
			ICodeCompiler codeCompiler = this.CreateCompiler();
			if (codeCompiler == null)
			{
				throw this.GetNotImplemented();
			}
			return codeCompiler.CompileAssemblyFromDomBatch(options, compilationUnits);
		}

		public virtual CompilerResults CompileAssemblyFromFile(CompilerParameters options, params string[] fileNames)
		{
			ICodeCompiler codeCompiler = this.CreateCompiler();
			if (codeCompiler == null)
			{
				throw this.GetNotImplemented();
			}
			return codeCompiler.CompileAssemblyFromFileBatch(options, fileNames);
		}

		public virtual CompilerResults CompileAssemblyFromSource(CompilerParameters options, params string[] fileNames)
		{
			ICodeCompiler codeCompiler = this.CreateCompiler();
			if (codeCompiler == null)
			{
				throw this.GetNotImplemented();
			}
			return codeCompiler.CompileAssemblyFromSourceBatch(options, fileNames);
		}

		public virtual string CreateEscapedIdentifier(string value)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			return codeGenerator.CreateEscapedIdentifier(value);
		}

		[ComVisible(false)]
		[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
		public static CodeDomProvider CreateProvider(string language)
		{
			CompilerInfo compilerInfo = CodeDomProvider.GetCompilerInfo(language);
			return (compilerInfo != null) ? compilerInfo.CreateProvider() : null;
		}

		public virtual string CreateValidIdentifier(string value)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			return codeGenerator.CreateValidIdentifier(value);
		}

		public virtual void GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, TextWriter writer, CodeGeneratorOptions options)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			codeGenerator.GenerateCodeFromCompileUnit(compileUnit, writer, options);
		}

		public virtual void GenerateCodeFromExpression(CodeExpression expression, TextWriter writer, CodeGeneratorOptions options)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			codeGenerator.GenerateCodeFromExpression(expression, writer, options);
		}

		public virtual void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
		{
			throw this.GetNotImplemented();
		}

		public virtual void GenerateCodeFromNamespace(CodeNamespace codeNamespace, TextWriter writer, CodeGeneratorOptions options)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			codeGenerator.GenerateCodeFromNamespace(codeNamespace, writer, options);
		}

		public virtual void GenerateCodeFromStatement(CodeStatement statement, TextWriter writer, CodeGeneratorOptions options)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			codeGenerator.GenerateCodeFromStatement(statement, writer, options);
		}

		public virtual void GenerateCodeFromType(CodeTypeDeclaration codeType, TextWriter writer, CodeGeneratorOptions options)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			codeGenerator.GenerateCodeFromType(codeType, writer, options);
		}

		[ComVisible(false)]
		[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
		public static CompilerInfo[] GetAllCompilerInfo()
		{
			return (CodeDomProvider.Config != null) ? CodeDomProvider.Config.CompilerInfos : null;
		}

		[ComVisible(false)]
		[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
		public static CompilerInfo GetCompilerInfo(string language)
		{
			if (language == null)
			{
				throw new ArgumentNullException("language");
			}
			if (CodeDomProvider.Config == null)
			{
				return null;
			}
			CompilerCollection compilers = CodeDomProvider.Config.Compilers;
			return compilers[language];
		}

		[ComVisible(false)]
		[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
		public static string GetLanguageFromExtension(string extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			if (CodeDomProvider.Config != null)
			{
				return CodeDomProvider.Config.Compilers.GetLanguageFromExtension(extension);
			}
			return null;
		}

		public virtual string GetTypeOutput(CodeTypeReference type)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			return codeGenerator.GetTypeOutput(type);
		}

		[ComVisible(false)]
		[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
		public static bool IsDefinedExtension(string extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			return CodeDomProvider.Config != null && CodeDomProvider.Config.Compilers.GetCompilerInfoForExtension(extension) != null;
		}

		[ComVisible(false)]
		[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
		public static bool IsDefinedLanguage(string language)
		{
			if (language == null)
			{
				throw new ArgumentNullException("language");
			}
			return CodeDomProvider.Config != null && CodeDomProvider.Config.Compilers.GetCompilerInfoForLanguage(language) != null;
		}

		public virtual bool IsValidIdentifier(string value)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			return codeGenerator.IsValidIdentifier(value);
		}

		public virtual CodeCompileUnit Parse(TextReader codeStream)
		{
			ICodeParser codeParser = this.CreateParser();
			if (codeParser == null)
			{
				throw this.GetNotImplemented();
			}
			return codeParser.Parse(codeStream);
		}

		public virtual bool Supports(GeneratorSupport supports)
		{
			ICodeGenerator codeGenerator = this.CreateGenerator();
			if (codeGenerator == null)
			{
				throw this.GetNotImplemented();
			}
			return codeGenerator.Supports(supports);
		}

		private static CodeDomConfigurationHandler Config
		{
			get
			{
				return ConfigurationManager.GetSection("system.codedom") as CodeDomConfigurationHandler;
			}
		}

		private Exception GetNotImplemented()
		{
			return new NotImplementedException();
		}
	}
}
