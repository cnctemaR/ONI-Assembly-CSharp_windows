using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;

namespace System.Data
{
	internal class CustomDataClassGenerator
	{
		public static void CreateDataSetClasses(DataSet ds, CodeNamespace cns, ICodeGenerator gen, ClassGeneratorOptions options)
		{
			new Generator(ds, cns, gen, options).Run();
		}

		public static void CreateDataSetClasses(DataSet ds, CodeNamespace cns, CodeDomProvider codeProvider, ClassGeneratorOptions options)
		{
			new Generator(ds, cns, codeProvider, options).Run();
		}

		public static void CreateDataSetClasses(DataSet ds, CodeCompileUnit cunit, CodeNamespace cns, CodeDomProvider codeProvider, ClassGeneratorOptions options)
		{
			new Generator(ds, cunit, cns, codeProvider, options).Run();
		}

		public static string MakeSafeName(string name, ICodeGenerator codeGen)
		{
			if (name == null || codeGen == null)
			{
				throw new NullReferenceException();
			}
			name = codeGen.CreateValidIdentifier(name);
			return CustomDataClassGenerator.MakeSafeNameInternal(name);
		}

		public static string MakeSafeName(string name, CodeDomProvider provider)
		{
			if (name == null || provider == null)
			{
				throw new NullReferenceException();
			}
			name = provider.CreateValidIdentifier(name);
			return CustomDataClassGenerator.MakeSafeNameInternal(name);
		}

		public static string MakeSafeNameInternal(string name)
		{
			if (name.Length == 0)
			{
				return "_";
			}
			StringBuilder stringBuilder = null;
			if (!char.IsLetter(name, 0) && name[0] != '_')
			{
				stringBuilder = new StringBuilder();
				stringBuilder.Append('_');
			}
			int num = 0;
			for (int i = 0; i < name.Length; i++)
			{
				if (!char.IsLetterOrDigit(name, i))
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append(name, num, i - num);
					stringBuilder.Append('_');
					num = i + 1;
				}
			}
			if (stringBuilder != null)
			{
				stringBuilder.Append(name, num, name.Length - num);
				return stringBuilder.ToString();
			}
			return name;
		}
	}
}
