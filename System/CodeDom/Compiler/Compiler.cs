using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;

namespace System.CodeDom.Compiler
{
	internal sealed class Compiler : ConfigurationElement
	{
		internal Compiler()
		{
		}

		public Compiler(string compilerOptions, string extension, string language, string type, int warningLevel)
		{
			this.CompilerOptions = compilerOptions;
			this.Extension = extension;
			this.Language = language;
			this.Type = type;
			this.WarningLevel = warningLevel;
		}

		static Compiler()
		{
			Compiler.properties.Add(Compiler.compilerOptionsProp);
			Compiler.properties.Add(Compiler.extensionProp);
			Compiler.properties.Add(Compiler.languageProp);
			Compiler.properties.Add(Compiler.typeProp);
			Compiler.properties.Add(Compiler.warningLevelProp);
			Compiler.properties.Add(Compiler.providerOptionsProp);
		}

		[ConfigurationProperty("compilerOptions", DefaultValue = "")]
		public string CompilerOptions
		{
			get
			{
				return (string)base[Compiler.compilerOptionsProp];
			}
			internal set
			{
				base[Compiler.compilerOptionsProp] = value;
			}
		}

		[ConfigurationProperty("extension", DefaultValue = "")]
		public string Extension
		{
			get
			{
				return (string)base[Compiler.extensionProp];
			}
			internal set
			{
				base[Compiler.extensionProp] = value;
			}
		}

		[ConfigurationProperty("language", DefaultValue = "", Options = ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey)]
		public string Language
		{
			get
			{
				return (string)base[Compiler.languageProp];
			}
			internal set
			{
				base[Compiler.languageProp] = value;
			}
		}

		[ConfigurationProperty("type", DefaultValue = "", Options = ConfigurationPropertyOptions.IsRequired)]
		public string Type
		{
			get
			{
				return (string)base[Compiler.typeProp];
			}
			internal set
			{
				base[Compiler.typeProp] = value;
			}
		}

		[ConfigurationProperty("warningLevel", DefaultValue = "0")]
		[IntegerValidator(MinValue = 0, MaxValue = 4)]
		public int WarningLevel
		{
			get
			{
				return (int)base[Compiler.warningLevelProp];
			}
			internal set
			{
				base[Compiler.warningLevelProp] = value;
			}
		}

		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public CompilerProviderOptionsCollection ProviderOptions
		{
			get
			{
				return (CompilerProviderOptionsCollection)base[Compiler.providerOptionsProp];
			}
			internal set
			{
				base[Compiler.providerOptionsProp] = value;
			}
		}

		public Dictionary<string, string> ProviderOptionsDictionary
		{
			get
			{
				return this.ProviderOptions.ProviderOptions;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return Compiler.properties;
			}
		}

		private static ConfigurationProperty compilerOptionsProp = new ConfigurationProperty("compilerOptions", typeof(string), string.Empty);

		private static ConfigurationProperty extensionProp = new ConfigurationProperty("extension", typeof(string), string.Empty);

		private static ConfigurationProperty languageProp = new ConfigurationProperty("language", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

		private static ConfigurationProperty typeProp = new ConfigurationProperty("type", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty warningLevelProp = new ConfigurationProperty("warningLevel", typeof(int), 0, global::System.ComponentModel.TypeDescriptor.GetConverter(typeof(int)), new IntegerValidator(0, 4), ConfigurationPropertyOptions.None);

		private static ConfigurationProperty providerOptionsProp = new ConfigurationProperty(string.Empty, typeof(CompilerProviderOptionsCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
