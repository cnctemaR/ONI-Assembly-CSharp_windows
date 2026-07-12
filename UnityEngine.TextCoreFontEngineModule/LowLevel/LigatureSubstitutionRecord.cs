using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct LigatureSubstitutionRecord
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

		[SerializeField]
		[NativeName("componentGlyphs")]
		private uint[] m_ComponentGlyphIDs;

		[SerializeField]
		[NativeName("ligatureGlyph")]
		private uint m_LigatureGlyphID;
	}
}
