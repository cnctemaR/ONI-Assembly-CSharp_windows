using System;

namespace UnityEngine.NVIDIA
{
	public readonly struct OptimalDLSSSettingsData
	{
		public uint outRenderWidth
		{
			get
			{
				return this.m_OutRenderWidth;
			}
		}

		public uint outRenderHeight
		{
			get
			{
				return this.m_OutRenderHeight;
			}
		}

		public float sharpness
		{
			get
			{
				return this.m_Sharpness;
			}
		}

		public uint maxWidth
		{
			get
			{
				return this.m_MaxWidth;
			}
		}

		public uint maxHeight
		{
			get
			{
				return this.m_MaxHeight;
			}
		}

		public uint minWidth
		{
			get
			{
				return this.m_MinWidth;
			}
		}

		public uint minHeight
		{
			get
			{
				return this.m_MinHeight;
			}
		}

		private readonly uint m_OutRenderWidth;

		private readonly uint m_OutRenderHeight;

		private readonly float m_Sharpness;

		private readonly uint m_MaxWidth;

		private readonly uint m_MaxHeight;

		private readonly uint m_MinWidth;

		private readonly uint m_MinHeight;
	}
}
