using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public struct MultipleSubstitutionRecord
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
		private uint m_TargetGlyphID;

		[SerializeField]
		private uint[] m_SubstituteGlyphIDs;
	}
}
