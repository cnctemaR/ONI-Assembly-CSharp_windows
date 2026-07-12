using System;
using System.Collections.Generic;
using System.Configuration;

namespace System.CodeDom.Compiler
{
	[ConfigurationCollection(typeof(Compiler), AddItemName = "compiler", CollectionType = ConfigurationElementCollectionType.BasicMap)]
	internal sealed class CompilerCollection : ConfigurationElementCollection
	{
		static CompilerCollection()
		{
			CompilerInfo compilerInfo = new CompilerInfo(null, "Microsoft.CSharp.CSharpCodeProvider, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", new string[] { "c#", "cs", "csharp" }, new string[] { ".cs" });
			compilerInfo.ProviderOptions["CompilerVersion"] = CompilerCollection.defaultCompilerVersion;
			CompilerCollection.AddCompilerInfo(compilerInfo);
			CompilerInfo compilerInfo2 = new CompilerInfo(null, "Microsoft.VisualBasic.VBCodeProvider, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", new string[] { "vb", "vbs", "visualbasic", "vbscript" }, new string[] { ".vb" });
			compilerInfo2.ProviderOptions["CompilerVersion"] = CompilerCollection.defaultCompilerVersion;
			CompilerCollection.AddCompilerInfo(compilerInfo2);
		}

		private static void AddCompilerInfo(CompilerInfo ci)
		{
			ci.CreateProvider();
			CompilerCollection.compiler_infos.Add(ci);
			string[] languages = ci.GetLanguages();
			if (languages != null)
			{
				foreach (string text in languages)
				{
					CompilerCollection.compiler_languages[text] = ci;
				}
			}
			string[] extensions = ci.GetExtensions();
			if (extensions != null)
			{
				foreach (string text2 in extensions)
				{
					CompilerCollection.compiler_extensions[text2] = ci;
				}
			}
		}

		private static void AddCompilerInfo(Compiler compiler)
		{
			CompilerCollection.AddCompilerInfo(new CompilerInfo(null, compiler.Type, new string[] { compiler.Extension }, new string[] { compiler.Language })
			{
				CompilerParams = 
				{
					CompilerOptions = compiler.CompilerOptions,
					WarningLevel = compiler.WarningLevel
				}
			});
		}

		protected override void BaseAdd(ConfigurationElement element)
		{
			Compiler compiler = element as Compiler;
			if (compiler != null)
			{
				CompilerCollection.AddCompilerInfo(compiler);
			}
			base.BaseAdd(element);
		}

		protected override bool ThrowOnDuplicate
		{
			get
			{
				return false;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new Compiler();
		}

		public CompilerInfo GetCompilerInfoForLanguage(string language)
		{
			if (CompilerCollection.compiler_languages.Count == 0)
			{
				return null;
			}
			CompilerInfo compilerInfo;
			if (CompilerCollection.compiler_languages.TryGetValue(language, out compilerInfo))
			{
				return compilerInfo;
			}
			return null;
		}

		public CompilerInfo GetCompilerInfoForExtension(string extension)
		{
			if (CompilerCollection.compiler_extensions.Count == 0)
			{
				return null;
			}
			CompilerInfo compilerInfo;
			if (CompilerCollection.compiler_extensions.TryGetValue(extension, out compilerInfo))
			{
				return compilerInfo;
			}
			return null;
		}

		public string GetLanguageFromExtension(string extension)
		{
			CompilerInfo compilerInfoForExtension = this.GetCompilerInfoForExtension(extension);
			if (compilerInfoForExtension == null)
			{
				return null;
			}
			string[] languages = compilerInfoForExtension.GetLanguages();
			if (languages != null && languages.Length != 0)
			{
				return languages[0];
			}
			return null;
		}

		public Compiler Get(int index)
		{
			return (Compiler)base.BaseGet(index);
		}

		public Compiler Get(string language)
		{
			return (Compiler)base.BaseGet(language);
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((Compiler)element).Language;
		}

		public string GetKey(int index)
		{
			return (string)base.BaseGetKey(index);
		}

		public string[] AllKeys
		{
			get
			{
				string[] array = new string[CompilerCollection.compiler_infos.Count];
				for (int i = 0; i < base.Count; i++)
				{
					array[i] = string.Join(";", CompilerCollection.compiler_infos[i].GetLanguages());
				}
				return array;
			}
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}

		protected override string ElementName
		{
			get
			{
				return "compiler";
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return CompilerCollection.properties;
			}
		}

		public Compiler this[int index]
		{
			get
			{
				return (Compiler)base.BaseGet(index);
			}
		}

		public CompilerInfo this[string language]
		{
			get
			{
				return this.GetCompilerInfoForLanguage(language);
			}
		}

		public CompilerInfo[] CompilerInfos
		{
			get
			{
				return CompilerCollection.compiler_infos.ToArray();
			}
		}

		private static readonly string defaultCompilerVersion = "4.0";

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static List<CompilerInfo> compiler_infos = new List<CompilerInfo>();

		private static Dictionary<string, CompilerInfo> compiler_languages = new Dictionary<string, CompilerInfo>(16, StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, CompilerInfo> compiler_extensions = new Dictionary<string, CompilerInfo>(4, StringComparer.OrdinalIgnoreCase);
	}
}
