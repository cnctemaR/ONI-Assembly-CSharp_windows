using System;

namespace UnityEngine.NVIDIA
{
	public struct DLSSCommandExecutionData
	{
		public int reset
		{
			get
			{
				return this.m_Reset;
			}
			set
			{
				this.m_Reset = value;
			}
		}

		public float sharpness
		{
			get
			{
				return this.m_Sharpness;
			}
			set
			{
				this.m_Sharpness = value;
			}
		}

		public float mvScaleX
		{
			get
			{
				return this.m_MVScaleX;
			}
			set
			{
				this.m_MVScaleX = value;
			}
		}

		public float mvScaleY
		{
			get
			{
				return this.m_MVScaleY;
			}
			set
			{
				this.m_MVScaleY = value;
			}
		}

		public float jitterOffsetX
		{
			get
			{
				return this.m_JitterOffsetX;
			}
			set
			{
				this.m_JitterOffsetX = value;
			}
		}

		public float jitterOffsetY
		{
			get
			{
				return this.m_JitterOffsetY;
			}
			set
			{
				this.m_JitterOffsetY = value;
			}
		}

		public float preExposure
		{
			get
			{
				return this.m_PreExposure;
			}
			set
			{
				this.m_PreExposure = value;
			}
		}

		public uint subrectOffsetX
		{
			get
			{
				return this.m_SubrectOffsetX;
			}
			set
			{
				this.m_SubrectOffsetX = value;
			}
		}

		public uint subrectOffsetY
		{
			get
			{
				return this.m_SubrectOffsetY;
			}
			set
			{
				this.m_SubrectOffsetY = value;
			}
		}

		public uint subrectWidth
		{
			get
			{
				return this.m_SubrectWidth;
			}
			set
			{
				this.m_SubrectWidth = value;
			}
		}

		public uint subrectHeight
		{
			get
			{
				return this.m_SubrectHeight;
			}
			set
			{
				this.m_SubrectHeight = value;
			}
		}

		public uint invertXAxis
		{
			get
			{
				return this.m_InvertXAxis;
			}
			set
			{
				this.m_InvertXAxis = value;
			}
		}

		public uint invertYAxis
		{
			get
			{
				return this.m_InvertYAxis;
			}
			set
			{
				this.m_InvertYAxis = value;
			}
		}

		internal uint featureSlot
		{
			get
			{
				return this.m_FeatureSlot;
			}
			set
			{
				this.m_FeatureSlot = value;
			}
		}

		private int m_Reset;

		private float m_Sharpness;

		private float m_MVScaleX;

		private float m_MVScaleY;

		private float m_JitterOffsetX;

		private float m_JitterOffsetY;

		private float m_PreExposure;

		private uint m_SubrectOffsetX;

		private uint m_SubrectOffsetY;

		private uint m_SubrectWidth;

		private uint m_SubrectHeight;

		private uint m_InvertXAxis;

		private uint m_InvertYAxis;

		private uint m_FeatureSlot;

		internal enum Textures
		{
			ColorInput,
			ColorOutput,
			Depth,
			MotionVectors,
			TransparencyMask,
			ExposureTexture,
			BiasColorMask
		}
	}
}
