using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct SingleSubstitutionRecord
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

		public uint substituteGlyphID
		{
			get
			{
				return this.m_SubstituteGlyphID;
			}
			set
			{
				this.m_SubstituteGlyphID = value;
			}
		}

		[NativeName("targetGlyphID")]
		[SerializeField]
		private uint m_TargetGlyphID;

		[SerializeField]
		[NativeName("substituteGlyphID")]
		private uint m_SubstituteGlyphID;
	}
}
