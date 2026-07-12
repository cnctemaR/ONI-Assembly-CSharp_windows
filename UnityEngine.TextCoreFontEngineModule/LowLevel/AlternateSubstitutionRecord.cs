using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct AlternateSubstitutionRecord
	{
		public uint targetGlyphID
		{
			get
			{
				return this.m_TargetGlyphID;
			}
			set
			{
				this.m_TargetGlyphID = value;
			}
		}

		public uint[] substituteGlyphIDs
		{
			get
			{
				return this.m_SubstituteGlyphIDs;
			}
			set
			{
				this.m_SubstituteGlyphIDs = value;
			}
		}

		[SerializeField]
		[NativeName("targetGlyphID")]
		private uint m_TargetGlyphID;

		[SerializeField]
		[NativeName("substituteGlyphIDs")]
		private uint[] m_SubstituteGlyphIDs;
	}
}
