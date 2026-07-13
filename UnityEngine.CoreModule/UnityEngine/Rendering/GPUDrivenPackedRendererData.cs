using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	internal struct GPUDrivenPackedRendererData
	{
		public bool receiveShadows
		{
			get
			{
				return (this.data & 1U) > 0U;
			}
		}

		public bool staticShadowCaster
		{
			get
			{
				return (this.data & 2U) > 0U;
			}
		}

		public byte lodMask
		{
			get
			{
				return (byte)((this.data >> 2) & 255U);
			}
		}

		public ShadowCastingMode shadowCastingMode
		{
			get
			{
				return (ShadowCastingMode)((this.data >> 10) & 3U);
			}
		}

		public LightProbeUsage lightProbeUsage
		{
			get
			{
				return (LightProbeUsage)((this.data >> 12) & 7U);
			}
		}

		public MotionVectorGenerationMode motionVecGenMode
		{
			get
			{
				return (MotionVectorGenerationMode)((this.data >> 15) & 3U);
			}
		}

		public bool isPartOfStaticBatch
		{
			get
			{
				return (this.data & 131072U) > 0U;
			}
		}

		public bool movedCurrentFrame
		{
			get
			{
				return (this.data & 262144U) > 0U;
			}
		}

		public bool hasTree
		{
			get
			{
				return (this.data & 524288U) > 0U;
			}
		}

		public bool smallMeshCulling
		{
			get
			{
				return (this.data & 1048576U) > 0U;
			}
		}

		public bool supportsIndirect
		{
			get
			{
				return (this.data & 2097152U) > 0U;
			}
		}

		public GPUDrivenPackedRendererData()
		{
			this.data = 0U;
		}

		public GPUDrivenPackedRendererData(bool receiveShadows, bool staticShadowCaster, byte lodMask, ShadowCastingMode shadowCastingMode, LightProbeUsage lightProbeUsage, MotionVectorGenerationMode motionVecGenMode, bool isPartOfStaticBatch, bool movedCurrentFrame, bool hasTree, bool smallMeshCulling, bool supportsIndirect)
		{
			this.data = (receiveShadows ? 1U : 0U);
			this.data |= (staticShadowCaster ? 2U : 0U);
			this.data |= (uint)((uint)lodMask << 2);
			this.data |= (uint)((uint)shadowCastingMode << 10);
			this.data |= (uint)((uint)lightProbeUsage << 12);
			this.data |= (uint)((uint)motionVecGenMode << 15);
			this.data |= (isPartOfStaticBatch ? 131072U : 0U);
			this.data |= (movedCurrentFrame ? 262144U : 0U);
			this.data |= (hasTree ? 524288U : 0U);
			this.data |= (smallMeshCulling ? 1048576U : 0U);
			this.data |= (supportsIndirect ? 2097152U : 0U);
		}

		private uint data;
	}
}
