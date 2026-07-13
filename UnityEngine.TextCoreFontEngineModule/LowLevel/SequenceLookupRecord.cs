using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct SequenceLookupRecord
	{
		public uint glyphSequenceIndex
		{
			get
			{
				return this.m_GlyphSequenceIndex;
			}
			set
			{
				this.m_GlyphSequenceIndex = value;
			}
		}

		public uint lookupListIndex
		{
			get
			{
				return this.m_LookupListIndex;
			}
			set
			{
				this.m_LookupListIndex = value;
			}
		}

		[NativeName("glyphSequenceIndex")]
		[SerializeField]
		private uint m_GlyphSequenceIndex;

		[NativeName("lookupListIndex")]
		[SerializeField]
		private uint m_LookupListIndex;
	}
}
