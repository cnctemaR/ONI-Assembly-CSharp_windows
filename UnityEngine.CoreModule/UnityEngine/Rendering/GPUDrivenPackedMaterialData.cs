using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	internal struct GPUDrivenPackedMaterialData
	{
		public bool isTransparent
		{
			get
			{
				return (this.data & 1U) > 0U;
			}
		}

		public bool isMotionVectorsPassEnabled
		{
			get
			{
				return (this.data & 2U) > 0U;
			}
		}

		public bool isIndirectSupported
		{
			get
			{
				return (this.data & 4U) > 0U;
			}
		}

		public bool supportsCrossFade
		{
			get
			{
				return (this.data & 8U) > 0U;
			}
		}

		public GPUDrivenPackedMaterialData()
		{
			this.data = 0U;
		}

		public GPUDrivenPackedMaterialData(bool isTransparent, bool isMotionVectorsPassEnabled, bool isIndirectSupported, bool supportsCrossFade)
		{
			this.data = (isTransparent ? 1U : 0U);
			this.data |= (isMotionVectorsPassEnabled ? 2U : 0U);
			this.data |= (isIndirectSupported ? 4U : 0U);
			this.data |= (supportsCrossFade ? 8U : 0U);
		}

		public bool Equals(GPUDrivenPackedMaterialData other)
		{
			return (other.data & 7U) == (this.data & 7U);
		}

		private uint data;
	}
}
