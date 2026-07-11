using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class UxmlAttributeDescription
	{
		protected UxmlAttributeDescription()
		{
			this.use = UxmlAttributeDescription.Use.Optional;
			this.restriction = null;
		}

		public string name { get; set; }

		public string[] obsoleteNames { get; set; }

		public string type { get; protected set; }

		public string typeNamespace { get; protected set; }

		public abstract string defaultValueAsString { get; }

		public UxmlAttributeDescription.Use use { get; set; }

		public UxmlTypeRestriction restriction { get; set; }

		internal bool TryGetValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value)
		{
			bool flag;
			if (this.name == null)
			{
				if (this.obsoleteNames == null || this.obsoleteNames.Length == 0)
				{
					Debug.LogError("Attribute description has no name.");
					value = null;
					flag = false;
				}
				else
				{
					for (int i = 0; i < this.obsoleteNames.Length; i++)
					{
						if (bag.TryGetAttributeValue(this.obsoleteNames[i], out value))
						{
							if (cc.visualTreeAsset != null)
							{
							}
							return true;
						}
					}
					value = null;
					flag = false;
				}
			}
			else if (!bag.TryGetAttributeValue(this.name, out value))
			{
				if (this.obsoleteNames != null)
				{
					for (int j = 0; j < this.obsoleteNames.Length; j++)
					{
						if (bag.TryGetAttributeValue(this.obsoleteNames[j], out value))
						{
							if (cc.visualTreeAsset != null)
							{
							}
							return true;
						}
					}
				}
				value = null;
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		protected T GetValueFromBag<T>(IUxmlAttributes bag, CreationContext cc, Func<string, T, T> converterFunc, T defaultValue)
		{
			string text;
			T t;
			if (this.TryGetValueFromBagAsString(bag, cc, out text))
			{
				t = converterFunc(text, defaultValue);
			}
			else
			{
				t = defaultValue;
			}
			return t;
		}

		protected const string k_XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

		public enum Use
		{
			None,
			Optional,
			Prohibited,
			Required
		}
	}
}
