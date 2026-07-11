using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct GlyphPairAdjustmentRecord
	{
		public GlyphAdjustmentRecord firstAdjustmentRecord
		{
			get
			{
				return this.m_FirstAdjustmentRecord;
			}
			set
			{
				this.m_FirstAdjustmentRecord = value;
			}
		}

		public GlyphAdjustmentRecord secondAdjustmentRecord
		{
			get
			{
				return this.m_SecondAdjustmentRecord;
			}
			set
			{
				this.m_SecondAdjustmentRecord = value;
			}
		}

		[SerializeField]
		[NativeName("firstAdjustmentRecord")]
		private GlyphAdjustmentRecord m_FirstAdjustmentRecord;

		[SerializeField]
		[NativeName("secondAdjustmentRecord")]
		private GlyphAdjustmentRecord m_SecondAdjustmentRecord;
	}
}
