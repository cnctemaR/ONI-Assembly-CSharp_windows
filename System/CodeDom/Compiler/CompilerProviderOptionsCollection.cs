using System;
using System.Collections.Generic;
using System.Configuration;

namespace System.CodeDom.Compiler
{
	[ConfigurationCollection(typeof(CompilerProviderOption), CollectionType = ConfigurationElementCollectionType.BasicMap, AddItemName = "providerOption")]
	internal sealed class CompilerProviderOptionsCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new CompilerProviderOption();
		}

		public CompilerProviderOption Get(int index)
		{
			return (CompilerProviderOption)base.BaseGet(index);
		}

		public CompilerProviderOption Get(string name)
		{
			return (CompilerProviderOption)base.BaseGet(name);
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((CompilerProviderOption)element).Name;
		}

		public string GetKey(int index)
		{
			return (string)base.BaseGetKey(index);
		}

		public string[] AllKeys
		{
			get
			{
				int count = base.Count;
				string[] array = new string[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = this[i].Name;
				}
				return array;
			}
		}

		protected override string ElementName
		{
			get
			{
				return "providerOption";
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return CompilerProviderOptionsCollection.properties;
			}
		}

		public Dictionary<string, string> ProviderOptions
		{
			get
			{
				int count = base.Count;
				if (count == 0)
				{
					return null;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>(count);
				for (int i = 0; i < count; i++)
				{
					CompilerProviderOption compilerProviderOption = this[i];
					dictionary.Add(compilerProviderOption.Name, compilerProviderOption.Value);
				}
				return dictionary;
			}
		}

		public CompilerProviderOption this[int index]
		{
			get
			{
				return (CompilerProviderOption)base.BaseGet(index);
			}
		}

		public CompilerProviderOption this[string name]
		{
			get
			{
				foreach (object obj in this)
				{
					CompilerProviderOption compilerProviderOption = (CompilerProviderOption)obj;
					if (compilerProviderOption.Name == name)
					{
						return compilerProviderOption;
					}
				}
				return null;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
