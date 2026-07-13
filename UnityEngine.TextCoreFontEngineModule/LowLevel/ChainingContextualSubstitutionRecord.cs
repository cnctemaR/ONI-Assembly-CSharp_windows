using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct ChainingContextualSubstitutionRecord
	{
		public GlyphIDSequence[] backtrackGlyphSequences
		{
			get
			{
				return this.m_BacktrackGlyphSequences;
			}
			set
			{
				this.m_BacktrackGlyphSequences = value;
			}
		}

		public GlyphIDSequence[] inputGlyphSequences
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

		public GlyphIDSequence[] lookaheadGlyphSequences
		{
			get
			{
				return this.m_LookaheadGlyphSequences;
			}
			set
			{
				this.m_LookaheadGlyphSequences = value;
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

		[SerializeField]
		[NativeName("backtrackGlyphSequences")]
		private GlyphIDSequence[] m_BacktrackGlyphSequences;

		[NativeName("inputGlyphSequences")]
		[SerializeField]
		private GlyphIDSequence[] m_InputGlyphSequences;

		[NativeName("lookaheadGlyphSequences")]
		[SerializeField]
		private GlyphIDSequence[] m_LookaheadGlyphSequences;

		[NativeName("sequenceLookupRecords")]
		[SerializeField]
		private SequenceLookupRecord[] m_SequenceLookupRecords;
	}
}
