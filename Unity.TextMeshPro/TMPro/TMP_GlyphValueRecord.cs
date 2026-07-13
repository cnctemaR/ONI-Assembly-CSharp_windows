using System;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace TMPro
{
	[Serializable]
	public struct TMP_GlyphValueRecord
	{
		public float xPlacement
		{
			get
			{
				return this.m_XPlacement;
			}
			set
			{
				this.m_XPlacement = value;
			}
		}

		public float yPlacement
		{
			get
			{
				return this.m_YPlacement;
			}
			set
			{
				this.m_YPlacement = value;
			}
		}

		public float xAdvance
		{
			get
			{
				return this.m_XAdvance;
			}
			set
			{
				this.m_XAdvance = value;
			}
		}

		public float yAdvance
		{
			get
			{
				return this.m_YAdvance;
			}
			set
			{
				this.m_YAdvance = value;
			}
		}

		public TMP_GlyphValueRecord(float xPlacement, float yPlacement, float xAdvance, float yAdvance)
		{
			this.m_XPlacement = xPlacement;
			this.m_YPlacement = yPlacement;
			this.m_XAdvance = xAdvance;
			this.m_YAdvance = yAdvance;
		}

		internal TMP_GlyphValueRecord(GlyphValueRecord_Legacy valueRecord)
		{
			this.m_XPlacement = valueRecord.xPlacement;
			this.m_YPlacement = valueRecord.yPlacement;
			this.m_XAdvance = valueRecord.xAdvance;
			this.m_YAdvance = valueRecord.yAdvance;
		}

		internal TMP_GlyphValueRecord(GlyphValueRecord valueRecord)
		{
			this.m_XPlacement = valueRecord.xPlacement;
			this.m_YPlacement = valueRecord.yPlacement;
			this.m_XAdvance = valueRecord.xAdvance;
			this.m_YAdvance = valueRecord.yAdvance;
		}

		public static TMP_GlyphValueRecord operator +(TMP_GlyphValueRecord a, TMP_GlyphValueRecord b)
		{
			TMP_GlyphValueRecord tmp_GlyphValueRecord;
			tmp_GlyphValueRecord.m_XPlacement = a.xPlacement + b.xPlacement;
			tmp_GlyphValueRecord.m_YPlacement = a.yPlacement + b.yPlacement;
			tmp_GlyphValueRecord.m_XAdvance = a.xAdvance + b.xAdvance;
			tmp_GlyphValueRecord.m_YAdvance = a.yAdvance + b.yAdvance;
			return tmp_GlyphValueRecord;
		}

		[SerializeField]
		internal float m_XPlacement;

		[SerializeField]
		internal float m_YPlacement;

		[SerializeField]
		internal float m_XAdvance;

		[SerializeField]
		internal float m_YAdvance;
	}
}
