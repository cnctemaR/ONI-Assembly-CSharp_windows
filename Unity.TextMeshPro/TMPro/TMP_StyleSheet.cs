using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMPro
{
	[ExcludeFromPreset]
	[Serializable]
	public class TMP_StyleSheet : ScriptableObject
	{
		internal List<TMP_Style> styles
		{
			get
			{
				return this.m_StyleList;
			}
		}

		public static TMP_StyleSheet instance
		{
			get
			{
				if (TMP_StyleSheet.s_Instance == null)
				{
					TMP_StyleSheet.s_Instance = TMP_Settings.defaultStyleSheet;
					if (TMP_StyleSheet.s_Instance == null)
					{
						TMP_StyleSheet.s_Instance = Resources.Load<TMP_StyleSheet>("Style Sheets/TMP Default Style Sheet");
					}
					if (TMP_StyleSheet.s_Instance == null)
					{
						return null;
					}
					TMP_StyleSheet.s_Instance.LoadStyleDictionaryInternal();
				}
				return TMP_StyleSheet.s_Instance;
			}
		}

		private void Reset()
		{
			this.LoadStyleDictionaryInternal();
		}

		public TMP_Style GetStyle(int hashCode)
		{
			if (this.m_StyleLookupDictionary == null)
			{
				this.LoadStyleDictionaryInternal();
			}
			TMP_Style tmp_Style;
			if (this.m_StyleLookupDictionary.TryGetValue(hashCode, out tmp_Style))
			{
				return tmp_Style;
			}
			return null;
		}

		public TMP_Style GetStyle(string name)
		{
			if (this.m_StyleLookupDictionary == null)
			{
				this.LoadStyleDictionaryInternal();
			}
			int hashCode = TMP_TextParsingUtilities.GetHashCode(name);
			TMP_Style tmp_Style;
			if (this.m_StyleLookupDictionary.TryGetValue(hashCode, out tmp_Style))
			{
				return tmp_Style;
			}
			return null;
		}

		public void AddStyle(TMP_Style style)
		{
			TMP_Style tmp_Style = this.m_StyleList.FirstOrDefault<TMP_Style>((TMP_Style p) => p.hashCode == style.hashCode);
			if (tmp_Style != null)
			{
				this.m_StyleList.Remove(tmp_Style);
			}
			this.m_StyleList.Add(style);
		}

		public void RefreshStyles()
		{
			this.LoadStyleDictionaryInternal();
		}

		private void LoadStyleDictionaryInternal()
		{
			if (this.m_StyleLookupDictionary == null)
			{
				this.m_StyleLookupDictionary = new Dictionary<int, TMP_Style>();
			}
			else
			{
				this.m_StyleLookupDictionary.Clear();
			}
			for (int i = 0; i < this.m_StyleList.Count; i++)
			{
				this.m_StyleList[i].RefreshStyle();
				if (!this.m_StyleLookupDictionary.ContainsKey(this.m_StyleList[i].hashCode))
				{
					this.m_StyleLookupDictionary.Add(this.m_StyleList[i].hashCode, this.m_StyleList[i]);
				}
			}
			int hashCode = TMP_TextParsingUtilities.GetHashCode("Normal");
			if (!this.m_StyleLookupDictionary.ContainsKey(hashCode))
			{
				TMP_Style tmp_Style = new TMP_Style("Normal", string.Empty, string.Empty);
				this.m_StyleList.Add(tmp_Style);
				this.m_StyleLookupDictionary.Add(hashCode, tmp_Style);
			}
		}

		private static TMP_StyleSheet s_Instance;

		[SerializeField]
		private List<TMP_Style> m_StyleList = new List<TMP_Style>(1);

		private Dictionary<int, TMP_Style> m_StyleLookupDictionary;
	}
}
