using System;
using System.ComponentModel;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public struct Resolution
	{
		public int width
		{
			get
			{
				return this.m_Width;
			}
			set
			{
				this.m_Width = value;
			}
		}

		public int height
		{
			get
			{
				return this.m_Height;
			}
			set
			{
				this.m_Height = value;
			}
		}

		public RefreshRate refreshRateRatio
		{
			get
			{
				return this.m_RefreshRate;
			}
			set
			{
				this.m_RefreshRate = value;
			}
		}

		[Obsolete("Resolution.refreshRate is obsolete. Use refreshRateRatio instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int refreshRate
		{
			get
			{
				return (int)Math.Round(this.m_RefreshRate.value);
			}
			set
			{
				this.m_RefreshRate.numerator = (uint)value;
				this.m_RefreshRate.denominator = 1U;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} x {1} @ {2}Hz", this.m_Width, this.m_Height, this.m_RefreshRate);
		}

		private int m_Width;

		private int m_Height;

		private RefreshRate m_RefreshRate;
	}
}
