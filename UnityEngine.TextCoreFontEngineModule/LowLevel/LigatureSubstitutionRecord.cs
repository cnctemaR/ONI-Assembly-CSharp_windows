using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule", "UnityEditor.TextCoreTextEngineModule" })]
	[Serializable]
	internal struct LigatureSubstitutionRecord : IEquatable<LigatureSubstitutionRecord>
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

		public bool Equals(LigatureSubstitutionRecord other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is LigatureSubstitutionRecord)
			{
				LigatureSubstitutionRecord ligatureSubstitutionRecord = (LigatureSubstitutionRecord)obj;
				flag = this.Equals(ligatureSubstitutionRecord);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this.m_ComponentGlyphIDs.GetHashCode();
		}

		public static bool operator ==(LigatureSubstitutionRecord lhs, LigatureSubstitutionRecord rhs)
		{
			bool flag = lhs.componentGlyphIDs != null && rhs.componentGlyphIDs != null;
			if (flag)
			{
				int num = lhs.m_ComponentGlyphIDs.Length;
				bool flag2 = num != rhs.m_ComponentGlyphIDs.Length;
				if (flag2)
				{
					return false;
				}
				for (int i = 0; i < num; i++)
				{
					bool flag3 = lhs.m_ComponentGlyphIDs[i] != rhs.m_ComponentGlyphIDs[i];
					if (flag3)
					{
						return false;
					}
				}
			}
			else
			{
				bool flag4 = lhs.componentGlyphIDs != null || rhs.componentGlyphIDs != null;
				if (flag4)
				{
					return false;
				}
			}
			return lhs.ligatureGlyphID == rhs.m_LigatureGlyphID;
		}

		public static bool operator !=(LigatureSubstitutionRecord lhs, LigatureSubstitutionRecord rhs)
		{
			return !(lhs == rhs);
		}

		[NativeName("componentGlyphs")]
		[SerializeField]
		private uint[] m_ComponentGlyphIDs;

		[NativeName("ligatureGlyph")]
		[SerializeField]
		private uint m_LigatureGlyphID;
	}
}
