using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public struct LigatureSubstitutionRecord
	{
		public uint[] componentGlyphIDs
		{
			get
			{
				return this.m_ComponentGlyphIDs;
			}
			set
			{
				this.m_ComponentGlyphIDs = value;
			}
		}

		public uint ligatureGlyphID
		{
			get
			{
				return this.m_LigatureGlyphID;
			}
			set
			{
				this.m_LigatureGlyphID = value;
			}
		}

		public static bool operator ==(LigatureSubstitutionRecord lhs, LigatureSubstitutionRecord rhs)
		{
			if (lhs.ligatureGlyphID != rhs.m_LigatureGlyphID)
			{
				return false;
			}
			int num = lhs.m_ComponentGlyphIDs.Length;
			if (num != rhs.m_ComponentGlyphIDs.Length)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				if (lhs.m_ComponentGlyphIDs[i] != rhs.m_ComponentGlyphIDs[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool operator !=(LigatureSubstitutionRecord lhs, LigatureSubstitutionRecord rhs)
		{
			return !(lhs == rhs);
		}

		[SerializeField]
		private uint[] m_ComponentGlyphIDs;

		[SerializeField]
		private uint m_LigatureGlyphID;
	}
}
