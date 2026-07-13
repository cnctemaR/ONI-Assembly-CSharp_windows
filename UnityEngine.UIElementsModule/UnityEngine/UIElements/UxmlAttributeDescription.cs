using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public abstract class UxmlAttributeDescription
	{
		protected UxmlAttributeDescription()
		{
			this.use = UxmlAttributeDescription.Use.Optional;
			this.restriction = null;
		}

		public string name { get; set; }

		public IEnumerable<string> obsoleteNames
		{
			get
			{
				return this.m_ObsoleteNames;
			}
			set
			{
				string[] array = value as string[];
				bool flag = array != null;
				if (flag)
				{
					this.m_ObsoleteNames = array;
				}
				else
				{
					this.m_ObsoleteNames = value.ToArray<string>();
				}
			}
		}

		public string type { get; protected internal set; }

		public string typeNamespace { get; protected set; }

		public abstract string defaultValueAsString { get; }

		public UxmlAttributeDescription.Use use { get; set; }

		public UxmlTypeRestriction restriction { get; set; }

		internal bool TryFindValueInAttributeOverrides(string elementName, CreationContext cc, List<TemplateAsset.AttributeOverride> attributeOverrides, out string value)
		{
			value = null;
			TemplateAsset.AttributeOverride attributeOverride = default(TemplateAsset.AttributeOverride);
			foreach (TemplateAsset.AttributeOverride attributeOverride2 in attributeOverrides)
			{
				bool flag = cc.namesPath == null;
				if (flag)
				{
					bool flag2 = attributeOverride2.m_ElementName != elementName;
					if (flag2)
					{
						continue;
					}
				}
				else
				{
					bool flag3 = !attributeOverride2.NamesPathMatchesElementNamesPath(cc.namesPath);
					if (flag3)
					{
						continue;
					}
				}
				bool flag4 = attributeOverride2.m_AttributeName != this.name;
				if (flag4)
				{
					bool flag5 = this.m_ObsoleteNames != null;
					if (!flag5)
					{
						continue;
					}
					bool flag6 = false;
					foreach (string text in this.m_ObsoleteNames)
					{
						bool flag7 = attributeOverride2.m_AttributeName != text;
						if (!flag7)
						{
							flag6 = true;
							break;
						}
					}
					bool flag8 = !flag6;
					if (flag8)
					{
						continue;
					}
				}
				bool flag9 = attributeOverride.m_AttributeName == null;
				if (flag9)
				{
					attributeOverride = attributeOverride2;
					bool flag10 = attributeOverride.m_NamesPath == null;
					if (flag10)
					{
						break;
					}
				}
				else
				{
					bool flag11 = attributeOverride.m_NamesPath.Length < attributeOverride2.m_NamesPath.Length;
					if (flag11)
					{
						attributeOverride = attributeOverride2;
					}
				}
			}
			bool flag12 = attributeOverride.m_AttributeName != null;
			bool flag13;
			if (flag12)
			{
				value = attributeOverride.m_Value;
				flag13 = true;
			}
			else
			{
				flag13 = false;
			}
			return flag13;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool TryGetValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value)
		{
			VisualTreeAsset visualTreeAsset;
			return this.TryGetValueFromBagAsString(bag, cc, out value, out visualTreeAsset);
		}

		internal bool TryGetAttributeOverrideValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value, out VisualTreeAsset sourceAsset)
		{
			string text;
			bag.TryGetAttributeValue("name", out text);
			bool flag = !string.IsNullOrEmpty(text) && cc.attributeOverrides != null;
			if (flag)
			{
				foreach (CreationContext.AttributeOverrideRange attributeOverrideRange in cc.attributeOverrides)
				{
					bool flag2 = this.TryFindValueInAttributeOverrides(text, cc, attributeOverrideRange.attributeOverrides, out value);
					if (flag2)
					{
						sourceAsset = attributeOverrideRange.sourceAsset;
						return true;
					}
				}
			}
			sourceAsset = null;
			value = null;
			return false;
		}

		internal bool ValidateName()
		{
			bool flag = this.name == null && (this.m_ObsoleteNames == null || this.m_ObsoleteNames.Length == 0);
			bool flag2;
			if (flag)
			{
				Debug.LogError("Attribute description has no name.");
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
			return flag2;
		}

		internal bool TryGetValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value, out VisualTreeAsset sourceAsset)
		{
			value = null;
			sourceAsset = null;
			bool flag = !this.ValidateName();
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.TryGetAttributeOverrideValueFromBagAsString(bag, cc, out value, out sourceAsset);
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = this.name == null;
					if (flag4)
					{
						for (int i = 0; i < this.m_ObsoleteNames.Length; i++)
						{
							bool flag5 = bag.TryGetAttributeValue(this.m_ObsoleteNames[i], out value);
							if (flag5)
							{
								sourceAsset = cc.visualTreeAsset;
								return true;
							}
						}
						flag2 = false;
					}
					else
					{
						bool flag6 = !bag.TryGetAttributeValue(this.name, out value);
						if (flag6)
						{
							bool flag7 = this.m_ObsoleteNames != null;
							if (flag7)
							{
								for (int j = 0; j < this.m_ObsoleteNames.Length; j++)
								{
									bool flag8 = bag.TryGetAttributeValue(this.m_ObsoleteNames[j], out value);
									if (flag8)
									{
										sourceAsset = cc.visualTreeAsset;
										UxmlAsset uxmlAsset = bag as UxmlAsset;
										bool flag9 = uxmlAsset != null;
										if (flag9)
										{
											uxmlAsset.RemoveAttribute(this.m_ObsoleteNames[j]);
											uxmlAsset.SetAttribute(this.name, value);
										}
										return true;
									}
								}
							}
							flag2 = false;
						}
						else
						{
							sourceAsset = cc.visualTreeAsset;
							flag2 = true;
						}
					}
				}
			}
			return flag2;
		}

		protected bool TryGetValueFromBag<T>(IUxmlAttributes bag, CreationContext cc, Func<string, T, T> converterFunc, T defaultValue, ref T value)
		{
			string text;
			bool flag = this.TryGetValueFromBagAsString(bag, cc, out text);
			bool flag3;
			if (flag)
			{
				bool flag2 = converterFunc != null;
				if (flag2)
				{
					value = converterFunc(text, defaultValue);
				}
				else
				{
					value = defaultValue;
				}
				flag3 = true;
			}
			else
			{
				flag3 = false;
			}
			return flag3;
		}

		protected T GetValueFromBag<T>(IUxmlAttributes bag, CreationContext cc, Func<string, T, T> converterFunc, T defaultValue)
		{
			bool flag = converterFunc == null;
			if (flag)
			{
				throw new ArgumentNullException("converterFunc");
			}
			string text;
			bool flag2 = this.TryGetValueFromBagAsString(bag, cc, out text);
			T t;
			if (flag2)
			{
				t = converterFunc(text, defaultValue);
			}
			else
			{
				t = defaultValue;
			}
			return t;
		}

		protected const string xmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

		private string[] m_ObsoleteNames;

		public enum Use
		{
			None,
			Optional,
			Prohibited,
			Required
		}
	}
}
