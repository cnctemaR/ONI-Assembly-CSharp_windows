using System;
using System.Collections.Generic;
using System.Linq;

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
				this.m_ObsoleteNames = value.ToArray<string>();
			}
		}

		public string type { get; protected set; }

		public string typeNamespace { get; protected set; }

		public abstract string defaultValueAsString { get; }

		public UxmlAttributeDescription.Use use { get; set; }

		public UxmlTypeRestriction restriction { get; set; }

		internal bool TryGetValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value)
		{
			bool flag = this.name == null && (this.m_ObsoleteNames == null || this.m_ObsoleteNames.Length == 0);
			bool flag2;
			if (flag)
			{
				Debug.LogError("Attribute description has no name.");
				value = null;
				flag2 = false;
			}
			else
			{
				string text;
				bag.TryGetAttributeValue("name", out text);
				bool flag3 = !string.IsNullOrEmpty(text) && cc.attributeOverrides != null;
				if (flag3)
				{
					for (int i = 0; i < cc.attributeOverrides.Count; i++)
					{
						bool flag4 = cc.attributeOverrides[i].m_ElementName != text;
						if (!flag4)
						{
							bool flag5 = cc.attributeOverrides[i].m_AttributeName != this.name;
							if (flag5)
							{
								bool flag6 = this.m_ObsoleteNames != null;
								if (!flag6)
								{
									goto IL_0147;
								}
								bool flag7 = false;
								for (int j = 0; j < this.m_ObsoleteNames.Length; j++)
								{
									bool flag8 = cc.attributeOverrides[i].m_AttributeName == this.m_ObsoleteNames[j];
									if (flag8)
									{
										flag7 = true;
										break;
									}
								}
								bool flag9 = !flag7;
								if (flag9)
								{
									goto IL_0147;
								}
							}
							value = cc.attributeOverrides[i].m_Value;
							return true;
						}
						IL_0147:;
					}
				}
				bool flag10 = this.name == null;
				if (flag10)
				{
					for (int k = 0; k < this.m_ObsoleteNames.Length; k++)
					{
						bool flag11 = bag.TryGetAttributeValue(this.m_ObsoleteNames[k], out value);
						if (flag11)
						{
							bool flag12 = cc.visualTreeAsset != null;
							if (flag12)
							{
							}
							return true;
						}
					}
					value = null;
					flag2 = false;
				}
				else
				{
					bool flag13 = !bag.TryGetAttributeValue(this.name, out value);
					if (flag13)
					{
						bool flag14 = this.m_ObsoleteNames != null;
						if (flag14)
						{
							for (int l = 0; l < this.m_ObsoleteNames.Length; l++)
							{
								bool flag15 = bag.TryGetAttributeValue(this.m_ObsoleteNames[l], out value);
								if (flag15)
								{
									bool flag16 = cc.visualTreeAsset != null;
									if (flag16)
									{
									}
									return true;
								}
							}
						}
						value = null;
						flag2 = false;
					}
					else
					{
						flag2 = true;
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
