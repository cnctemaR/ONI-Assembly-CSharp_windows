using System;
using System.Collections.Generic;

namespace Unity.VectorGraphics
{
	internal class SVGStyleSheet
	{
		public SVGPropertySheet this[string key]
		{
			get
			{
				int num = this.m_Selectors.FindIndex((KeyValuePair<string, SVGPropertySheet> x) => x.Key == key);
				bool flag = num != -1;
				SVGPropertySheet svgpropertySheet;
				if (flag)
				{
					svgpropertySheet = this.m_Selectors[num].Value;
				}
				else
				{
					svgpropertySheet = null;
				}
				return svgpropertySheet;
			}
			set
			{
				KeyValuePair<string, SVGPropertySheet> keyValuePair = new KeyValuePair<string, SVGPropertySheet>(key, value);
				int num = this.m_Selectors.FindIndex((KeyValuePair<string, SVGPropertySheet> x) => x.Key == key);
				bool flag = num != -1;
				if (flag)
				{
					this.m_Selectors[num] = keyValuePair;
				}
				this.m_Selectors.Add(keyValuePair);
			}
		}

		public IEnumerable<string> selectors
		{
			get
			{
				foreach (KeyValuePair<string, SVGPropertySheet> kvp in this.m_Selectors)
				{
					yield return kvp.Key;
					kvp = default(KeyValuePair<string, SVGPropertySheet>);
				}
				List<KeyValuePair<string, SVGPropertySheet>>.Enumerator enumerator = default(List<KeyValuePair<string, SVGPropertySheet>>.Enumerator);
				yield break;
				yield break;
			}
		}

		public int Count
		{
			get
			{
				return this.m_Selectors.Count;
			}
		}

		public void Clear()
		{
			this.m_Selectors.Clear();
		}

		private List<KeyValuePair<string, SVGPropertySheet>> m_Selectors = new List<KeyValuePair<string, SVGPropertySheet>>();
	}
}
