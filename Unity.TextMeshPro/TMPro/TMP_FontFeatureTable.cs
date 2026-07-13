using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace TMPro
{
	[Serializable]
	public class TMP_FontFeatureTable
	{
		public List<MultipleSubstitutionRecord> multipleSubstitutionRecords
		{
			get
			{
				return this.m_MultipleSubstitutionRecords;
			}
			set
			{
				this.m_MultipleSubstitutionRecords = value;
			}
		}

		public List<LigatureSubstitutionRecord> ligatureRecords
		{
			get
			{
				return this.m_LigatureSubstitutionRecords;
			}
			set
			{
				this.m_LigatureSubstitutionRecords = value;
			}
		}

		public List<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords
		{
			get
			{
				return this.m_GlyphPairAdjustmentRecords;
			}
			set
			{
				this.m_GlyphPairAdjustmentRecords = value;
			}
		}

		public List<MarkToBaseAdjustmentRecord> MarkToBaseAdjustmentRecords
		{
			get
			{
				return this.m_MarkToBaseAdjustmentRecords;
			}
			set
			{
				this.m_MarkToBaseAdjustmentRecords = value;
			}
		}

		public List<MarkToMarkAdjustmentRecord> MarkToMarkAdjustmentRecords
		{
			get
			{
				return this.m_MarkToMarkAdjustmentRecords;
			}
			set
			{
				this.m_MarkToMarkAdjustmentRecords = value;
			}
		}

		public TMP_FontFeatureTable()
		{
			this.m_LigatureSubstitutionRecords = new List<LigatureSubstitutionRecord>();
			this.m_LigatureSubstitutionRecordLookup = new Dictionary<uint, List<LigatureSubstitutionRecord>>();
			this.m_GlyphPairAdjustmentRecords = new List<GlyphPairAdjustmentRecord>();
			this.m_GlyphPairAdjustmentRecordLookup = new Dictionary<uint, GlyphPairAdjustmentRecord>();
			this.m_MarkToBaseAdjustmentRecords = new List<MarkToBaseAdjustmentRecord>();
			this.m_MarkToBaseAdjustmentRecordLookup = new Dictionary<uint, MarkToBaseAdjustmentRecord>();
			this.m_MarkToMarkAdjustmentRecords = new List<MarkToMarkAdjustmentRecord>();
			this.m_MarkToMarkAdjustmentRecordLookup = new Dictionary<uint, MarkToMarkAdjustmentRecord>();
		}

		public void SortGlyphPairAdjustmentRecords()
		{
			if (this.m_GlyphPairAdjustmentRecords.Count > 0)
			{
				this.m_GlyphPairAdjustmentRecords = (from s in this.m_GlyphPairAdjustmentRecords
					orderby s.firstAdjustmentRecord.glyphIndex, s.secondAdjustmentRecord.glyphIndex
					select s).ToList<GlyphPairAdjustmentRecord>();
			}
		}

		public void SortMarkToBaseAdjustmentRecords()
		{
			if (this.m_MarkToBaseAdjustmentRecords.Count > 0)
			{
				this.m_MarkToBaseAdjustmentRecords = (from s in this.m_MarkToBaseAdjustmentRecords
					orderby s.baseGlyphID, s.markGlyphID
					select s).ToList<MarkToBaseAdjustmentRecord>();
			}
		}

		public void SortMarkToMarkAdjustmentRecords()
		{
			if (this.m_MarkToMarkAdjustmentRecords.Count > 0)
			{
				this.m_MarkToMarkAdjustmentRecords = (from s in this.m_MarkToMarkAdjustmentRecords
					orderby s.baseMarkGlyphID, s.combiningMarkGlyphID
					select s).ToList<MarkToMarkAdjustmentRecord>();
			}
		}

		[SerializeField]
		internal List<MultipleSubstitutionRecord> m_MultipleSubstitutionRecords;

		[SerializeField]
		internal List<LigatureSubstitutionRecord> m_LigatureSubstitutionRecords;

		[SerializeField]
		internal List<GlyphPairAdjustmentRecord> m_GlyphPairAdjustmentRecords;

		[SerializeField]
		internal List<MarkToBaseAdjustmentRecord> m_MarkToBaseAdjustmentRecords;

		[SerializeField]
		internal List<MarkToMarkAdjustmentRecord> m_MarkToMarkAdjustmentRecords;

		internal Dictionary<uint, List<LigatureSubstitutionRecord>> m_LigatureSubstitutionRecordLookup;

		internal Dictionary<uint, GlyphPairAdjustmentRecord> m_GlyphPairAdjustmentRecordLookup;

		internal Dictionary<uint, MarkToBaseAdjustmentRecord> m_MarkToBaseAdjustmentRecordLookup;

		internal Dictionary<uint, MarkToMarkAdjustmentRecord> m_MarkToMarkAdjustmentRecordLookup;
	}
}
