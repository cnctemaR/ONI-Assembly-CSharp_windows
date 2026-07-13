using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	public struct ShadowDrawingSettings : IEquatable<ShadowDrawingSettings>
	{
		public CullingResults cullingResults
		{
			get
			{
				return this.m_CullingResults;
			}
			set
			{
				this.m_CullingResults = value;
			}
		}

		public int lightIndex
		{
			get
			{
				return this.m_LightIndex;
			}
			set
			{
				this.m_LightIndex = value;
			}
		}

		public int splitIndex
		{
			get
			{
				return this.m_SplitIndex;
			}
			set
			{
				this.m_SplitIndex = value;
			}
		}

		public bool useRenderingLayerMaskTest
		{
			get
			{
				return this.m_UseRenderingLayerMaskTest != 0;
			}
			set
			{
				this.m_UseRenderingLayerMaskTest = (value ? 1 : 0);
			}
		}

		public uint batchLayerMask
		{
			get
			{
				return this.m_BatchLayerMask;
			}
			set
			{
				this.m_BatchLayerMask = value;
			}
		}

		[Obsolete("ShadowDrawingSettings.splitData is deprecated. The equivalent data must be passed to ScriptableRenderContext.CullShadowCasters.")]
		public ShadowSplitData splitData
		{
			get
			{
				return this.m_SplitData;
			}
			set
			{
				this.m_SplitData = value;
			}
		}

		public ShadowObjectsFilter objectsFilter
		{
			get
			{
				return this.m_ObjectsFilter;
			}
			set
			{
				this.m_ObjectsFilter = value;
			}
		}

		[Obsolete("ShadowDrawingSettings.projectionType is deprecated. There is no replacement for this parameter. You don't need to set it anymore.")]
		public BatchCullingProjectionType projectionType
		{
			get
			{
				return this.m_ProjectionType;
			}
			set
			{
				this.m_ProjectionType = value;
			}
		}

		public ShadowDrawingSettings(CullingResults cullingResults, int lightIndex)
		{
			this.m_CullingResults = cullingResults;
			this.m_LightIndex = lightIndex;
			this.m_SplitIndex = -1;
			this.m_UseRenderingLayerMaskTest = 0;
			this.m_BatchLayerMask = uint.MaxValue;
			this.m_SplitData = default(ShadowSplitData);
			this.m_SplitData.shadowCascadeBlendCullingFactor = 1f;
			this.m_ObjectsFilter = ShadowObjectsFilter.AllObjects;
			this.m_ProjectionType = BatchCullingProjectionType.Unknown;
		}

		[Obsolete("ShadowDrawingSettings(CullingResults, int, BatchCullingProjectionType) is deprecated. Use ShadowDrawingSettings(CullingResults, int) instead.")]
		public ShadowDrawingSettings(CullingResults cullingResults, int lightIndex, BatchCullingProjectionType projectionType)
		{
			this = new ShadowDrawingSettings(cullingResults, lightIndex);
			this.m_ProjectionType = projectionType;
		}

		public bool Equals(ShadowDrawingSettings other)
		{
			return this.m_CullingResults.Equals(other.m_CullingResults) && this.m_LightIndex == other.m_LightIndex && this.m_SplitIndex == other.m_SplitIndex && this.m_SplitData.Equals(other.m_SplitData) && this.m_UseRenderingLayerMaskTest.Equals(other.m_UseRenderingLayerMaskTest) && this.m_BatchLayerMask == other.m_BatchLayerMask && this.m_ObjectsFilter.Equals(other.m_ObjectsFilter);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is ShadowDrawingSettings && this.Equals((ShadowDrawingSettings)obj);
		}

		public override int GetHashCode()
		{
			int num = this.m_CullingResults.GetHashCode();
			num = (num * 397) ^ this.m_LightIndex;
			num = (num * 397) ^ this.m_SplitIndex;
			num = (num * 397) ^ this.m_UseRenderingLayerMaskTest;
			num = (num * 397) ^ (int)this.m_BatchLayerMask;
			num = (num * 397) ^ this.m_SplitData.GetHashCode();
			return (num * 397) ^ (int)this.m_ObjectsFilter;
		}

		public static bool operator ==(ShadowDrawingSettings left, ShadowDrawingSettings right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ShadowDrawingSettings left, ShadowDrawingSettings right)
		{
			return !left.Equals(right);
		}

		private CullingResults m_CullingResults;

		private int m_LightIndex;

		private int m_SplitIndex;

		private int m_UseRenderingLayerMaskTest;

		private uint m_BatchLayerMask;

		private ShadowSplitData m_SplitData;

		private ShadowObjectsFilter m_ObjectsFilter;

		private BatchCullingProjectionType m_ProjectionType;
	}
}
