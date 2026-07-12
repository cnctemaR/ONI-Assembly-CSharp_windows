using System;

namespace UnityEngine.NVIDIA
{
	public struct DLSSCommandInitializationData
	{
		public uint inputRTWidth
		{
			get
			{
				return this.m_InputRTWidth;
			}
			set
			{
				this.m_InputRTWidth = value;
			}
		}

		public uint inputRTHeight
		{
			get
			{
				return this.m_InputRTHeight;
			}
			set
			{
				this.m_InputRTHeight = value;
			}
		}

		public uint outputRTWidth
		{
			get
			{
				return this.m_OutputRTWidth;
			}
			set
			{
				this.m_OutputRTWidth = value;
			}
		}

		public uint outputRTHeight
		{
			get
			{
				return this.m_OutputRTHeight;
			}
			set
			{
				this.m_OutputRTHeight = value;
			}
		}

		public DLSSQuality quality
		{
			get
			{
				return this.m_Quality;
			}
			set
			{
				this.m_Quality = value;
			}
		}

		public DLSSFeatureFlags featureFlags
		{
			get
			{
				return this.m_Flags;
			}
			set
			{
				this.m_Flags = value;
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

		public void SetFlag(DLSSFeatureFlags flag, bool value)
		{
			if (value)
			{
				this.m_Flags |= flag;
			}
			else
			{
				this.m_Flags &= ~flag;
			}
		}

		public bool GetFlag(DLSSFeatureFlags flag)
		{
			return (this.m_Flags & flag) > DLSSFeatureFlags.None;
		}

		private uint m_InputRTWidth;

		private uint m_InputRTHeight;

		private uint m_OutputRTWidth;

		private uint m_OutputRTHeight;

		private DLSSQuality m_Quality;

		private DLSSFeatureFlags m_Flags;

		private uint m_FeatureSlot;
	}
}
