using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text
{
	[ExcludeFromPreset]
	[ExcludeFromObjectFactory]
	[Serializable]
	public class TextStyleSheet : ScriptableObject
	{
		internal List<TextStyle> styles
		{
			get
			{
				return this.m_StyleList;
			}
		}

		private void Reset()
		{
			this.LoadStyleDictionaryInternal();
		}

		public TextStyle GetStyle(int hashCode)
		{
			bool flag = this.m_StyleLookupDictionary == null;
			if (flag)
			{
				this.LoadStyleDictionaryInternal();
			}
			TextStyle textStyle;
			bool flag2 = this.m_StyleLookupDictionary.TryGetValue(hashCode, out textStyle);
			TextStyle textStyle2;
			if (flag2)
			{
				textStyle2 = textStyle;
			}
			else
			{
				textStyle2 = null;
			}
			return textStyle2;
		}

		public TextStyle GetStyle(string name)
		{
			bool flag = this.m_StyleLookupDictionary == null;
			if (flag)
			{
				this.LoadStyleDictionaryInternal();
			}
			int hashCodeCaseInSensitive = TextUtilities.GetHashCodeCaseInSensitive(name);
			TextStyle textStyle;
			bool flag2 = this.m_StyleLookupDictionary.TryGetValue(hashCodeCaseInSensitive, out textStyle);
			TextStyle textStyle2;
			if (flag2)
			{
				textStyle2 = textStyle;
			}
			else
			{
				textStyle2 = null;
			}
			return textStyle2;
		}

		public void RefreshStyles()
		{
			this.LoadStyleDictionaryInternal();
		}

		private void LoadStyleDictionaryInternal()
		{
			bool flag = this.m_StyleLookupDictionary == null;
			if (flag)
			{
				this.m_StyleLookupDictionary = new Dictionary<int, TextStyle>();
			}
			else
			{
				this.m_StyleLookupDictionary.Clear();
			}
			for (int i = 0; i < this.m_StyleList.Count; i++)
			{
				this.m_StyleList[i].RefreshStyle();
				bool flag2 = !this.m_StyleLookupDictionary.ContainsKey(this.m_StyleList[i].hashCode);
				if (flag2)
				{
					this.m_StyleLookupDictionary.Add(this.m_StyleList[i].hashCode, this.m_StyleList[i]);
				}
			}
			int hashCodeCaseInSensitive = TextUtilities.GetHashCodeCaseInSensitive("Normal");
			bool flag3 = !this.m_StyleLookupDictionary.ContainsKey(hashCodeCaseInSensitive);
			if (flag3)
			{
				TextStyle textStyle = new TextStyle("Normal", string.Empty, string.Empty);
				this.m_StyleList.Add(textStyle);
				this.m_StyleLookupDictionary.Add(hashCodeCaseInSensitive, textStyle);
			}
		}

		[SerializeField]
		private List<TextStyle> m_StyleList = new List<TextStyle>(1);

		private Dictionary<int, TextStyle> m_StyleLookupDictionary;
	}
}
