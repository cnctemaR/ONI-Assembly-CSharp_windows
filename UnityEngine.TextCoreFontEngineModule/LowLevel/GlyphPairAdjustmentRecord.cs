using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[DebuggerDisplay("First glyphIndex = {m_FirstAdjustmentRecord.m_GlyphIndex},  Second glyphIndex = {m_SecondAdjustmentRecord.m_GlyphIndex}")]
	[Serializable]
	public struct GlyphPairAdjustmentRecord : IEquatable<GlyphPairAdjustmentRecord>
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

		public FontFeatureLookupFlags featureLookupFlags
		{
			get
			{
				return this.m_FeatureLookupFlags;
			}
			set
			{
				this.m_FeatureLookupFlags = value;
			}
		}

		public GlyphPairAdjustmentRecord(GlyphAdjustmentRecord firstAdjustmentRecord, GlyphAdjustmentRecord secondAdjustmentRecord)
		{
			this.m_FirstAdjustmentRecord = firstAdjustmentRecord;
			this.m_SecondAdjustmentRecord = secondAdjustmentRecord;
			this.m_FeatureLookupFlags = FontFeatureLookupFlags.None;
		}

		[ExcludeFromDocs]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		[ExcludeFromDocs]
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		[ExcludeFromDocs]
		public bool Equals(GlyphPairAdjustmentRecord other)
		{
			return base.Equals(other);
		}

		[ExcludeFromDocs]
		public static bool operator ==(GlyphPairAdjustmentRecord lhs, GlyphPairAdjustmentRecord rhs)
		{
			return lhs.m_FirstAdjustmentRecord == rhs.m_FirstAdjustmentRecord && lhs.m_SecondAdjustmentRecord == rhs.m_SecondAdjustmentRecord;
		}

		[ExcludeFromDocs]
		public static bool operator !=(GlyphPairAdjustmentRecord lhs, GlyphPairAdjustmentRecord rhs)
		{
			return !(lhs == rhs);
		}

		[SerializeField]
		[NativeName("firstAdjustmentRecord")]
		private GlyphAdjustmentRecord m_FirstAdjustmentRecord;

		[SerializeField]
		[NativeName("secondAdjustmentRecord")]
		private GlyphAdjustmentRecord m_SecondAdjustmentRecord;

		[SerializeField]
		private FontFeatureLookupFlags m_FeatureLookupFlags;
	}
}
