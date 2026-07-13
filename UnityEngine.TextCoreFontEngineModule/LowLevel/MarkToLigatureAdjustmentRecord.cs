using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct MarkToLigatureAdjustmentRecord
	{
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

		public uint combiningMarkGlyphID
		{
			get
			{
				return this.m_CombiningMarkGlyphID;
			}
			set
			{
				this.m_CombiningMarkGlyphID = value;
			}
		}

		public MarkAdjustmentRecord[] combiningMarkAdjustmentRecords
		{
			get
			{
				return this.m_CombiningMarkAdjustmentRecords;
			}
			set
			{
				this.m_CombiningMarkAdjustmentRecords = value;
			}
		}

		[NativeName("ligatureGlyphID")]
		[SerializeField]
		private uint m_LigatureGlyphID;

		[SerializeField]
		[NativeName("combiningMarkGlyphID")]
		private uint m_CombiningMarkGlyphID;

		[SerializeField]
		[NativeName("adjustmentRecords")]
		private MarkAdjustmentRecord[] m_CombiningMarkAdjustmentRecords;
	}
}
