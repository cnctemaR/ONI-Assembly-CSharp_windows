using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct ContextualSubstitutionRecord
	{
		public GlyphIDSequence[] inputSequences
		{
			get
			{
				return this.m_InputGlyphSequences;
			}
			set
			{
				this.m_InputGlyphSequences = value;
			}
		}

		public SequenceLookupRecord[] sequenceLookupRecords
		{
			get
			{
				return this.m_SequenceLookupRecords;
			}
			set
			{
				this.m_SequenceLookupRecords = value;
			}
		}

		[NativeName("inputGlyphSequences")]
		[SerializeField]
		private GlyphIDSequence[] m_InputGlyphSequences;

		[NativeName("sequenceLookupRecords")]
		[SerializeField]
		private SequenceLookupRecord[] m_SequenceLookupRecords;
	}
}
