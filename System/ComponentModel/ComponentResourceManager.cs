using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace System.ComponentModel
{
	public class ComponentResourceManager : ResourceManager
	{
		public ComponentResourceManager()
		{
		}

		public ComponentResourceManager(Type t)
			: base(t)
		{
		}

		public void ApplyResources(object value, string objectName)
		{
			this.ApplyResources(value, objectName, null);
		}

		public virtual void ApplyResources(object value, string objectName, CultureInfo culture)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (objectName == null)
			{
				throw new ArgumentNullException("objectName");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			Hashtable hashtable = ((!this.IgnoreCase) ? new Hashtable() : global::System.Collections.Specialized.CollectionsUtil.CreateCaseInsensitiveHashtable());
			this.BuildResources(culture, hashtable);
			string text = objectName + ".";
			CompareOptions compareOptions = ((!this.IgnoreCase) ? CompareOptions.None : CompareOptions.IgnoreCase);
			Type type = value.GetType();
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			if (this.IgnoreCase)
			{
				bindingFlags |= BindingFlags.IgnoreCase;
			}
			foreach (object obj in hashtable)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				string text2 = (string)dictionaryEntry.Key;
				if (culture.CompareInfo.IsPrefix(text2, text, compareOptions))
				{
					string text3 = text2.Substring(text.Length);
					PropertyInfo property = type.GetProperty(text3, bindingFlags);
					if (property != null && property.CanWrite)
					{
						object value2 = dictionaryEntry.Value;
						if (value2 == null || property.PropertyType.IsInstanceOfType(value2))
						{
							property.SetValue(value, value2, null);
						}
					}
				}
			}
		}

		private void BuildResources(CultureInfo culture, Hashtable resources)
		{
			if (culture != culture.Parent)
			{
				this.BuildResources(culture.Parent, resources);
			}
			ResourceSet resourceSet = this.GetResourceSet(culture, true, false);
			if (resourceSet != null)
			{
				foreach (object obj in resourceSet)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					resources[(string)dictionaryEntry.Key] = dictionaryEntry.Value;
				}
			}
		}
	}
}
