using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct GlyphValueRecord
	{
		public GlyphValueRecord(float xPlacement, float yPlacement, float xAdvance, float yAdvance)
		{
			this.m_XPlacement = xPlacement;
			this.m_YPlacement = yPlacement;
			this.m_XAdvance = xAdvance;
			this.m_YAdvance = yAdvance;
		}

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

		public static GlyphValueRecord operator +(GlyphValueRecord a, GlyphValueRecord b)
		{
			GlyphValueRecord glyphValueRecord;
			glyphValueRecord.m_XPlacement = a.xPlacement + b.xPlacement;
			glyphValueRecord.m_YPlacement = a.yPlacement + b.yPlacement;
			glyphValueRecord.m_XAdvance = a.xAdvance + b.xAdvance;
			glyphValueRecord.m_YAdvance = a.yAdvance + b.yAdvance;
			return glyphValueRecord;
		}

		[SerializeField]
		[NativeName("xPlacement")]
		private float m_XPlacement;

		[SerializeField]
		[NativeName("yPlacement")]
		private float m_YPlacement;

		[SerializeField]
		[NativeName("xAdvance")]
		private float m_XAdvance;

		[SerializeField]
		[NativeName("yAdvance")]
		private float m_YAdvance;
	}
}
