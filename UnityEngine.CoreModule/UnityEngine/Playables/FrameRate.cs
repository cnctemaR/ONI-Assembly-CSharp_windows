using System;
using System.Globalization;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[NativeHeader("Runtime/Director/Core/FrameRate.h")]
	[UsedByNativeCode("FrameRate")]
	internal struct FrameRate : IEquatable<FrameRate>
	{
		public bool dropFrame
		{
			get
			{
				return this.m_Rate < 0;
			}
		}

		public double rate
		{
			get
			{
				return this.dropFrame ? ((double)(-(double)this.m_Rate) * 0.999000999000999) : ((double)this.m_Rate);
			}
		}

		public FrameRate(uint frameRate = 0U, bool drop = false)
		{
			this.m_Rate = (int)((drop ? uint.MaxValue : 1U) * frameRate);
		}

		public bool IsValid()
		{
			return this.m_Rate != 0;
		}

		public bool Equals(FrameRate other)
		{
			return this.m_Rate == other.m_Rate;
		}

		public override bool Equals(object obj)
		{
			return obj is FrameRate && this.Equals((FrameRate)obj);
		}

		public static bool operator ==(FrameRate a, FrameRate b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(FrameRate a, FrameRate b)
		{
			return !a.Equals(b);
		}

		public static bool operator <(FrameRate a, FrameRate b)
		{
			return a.rate < b.rate;
		}

		public static bool operator <=(FrameRate a, FrameRate b)
		{
			return a.rate <= b.rate;
		}

		public static bool operator >(FrameRate a, FrameRate b)
		{
			return a.rate > b.rate;
		}

		public static bool operator >=(FrameRate a, FrameRate b)
		{
			return a.rate <= b.rate;
		}

		public override int GetHashCode()
		{
			return this.m_Rate;
		}

		public override string ToString()
		{
			return this.ToString(null, null);
		}

		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			bool flag = string.IsNullOrEmpty(format);
			if (flag)
			{
				format = (this.dropFrame ? "F2" : "F0");
			}
			bool flag2 = formatProvider == null;
			if (flag2)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return UnityString.Format("{0} Fps", new object[] { this.rate.ToString(format, formatProvider) });
		}

		internal static int FrameRateToInt(FrameRate framerate)
		{
			return framerate.m_Rate;
		}

		internal static FrameRate DoubleToFrameRate(double framerate)
		{
			uint num = (uint)Math.Ceiling(framerate);
			bool flag = num <= 0U;
			FrameRate frameRate;
			if (flag)
			{
				frameRate = new FrameRate(1U, false);
			}
			else
			{
				FrameRate frameRate2 = new FrameRate(num, true);
				bool flag2 = Math.Abs(framerate - frameRate2.rate) < Math.Abs(framerate - num);
				if (flag2)
				{
					frameRate = frameRate2;
				}
				else
				{
					frameRate = new FrameRate(num, false);
				}
			}
			return frameRate;
		}

		[Ignore]
		public static readonly FrameRate k_24Fps = new FrameRate(24U, false);

		[Ignore]
		public static readonly FrameRate k_23_976Fps = new FrameRate(24U, true);

		[Ignore]
		public static readonly FrameRate k_25Fps = new FrameRate(25U, false);

		[Ignore]
		public static readonly FrameRate k_30Fps = new FrameRate(30U, false);

		[Ignore]
		public static readonly FrameRate k_29_97Fps = new FrameRate(30U, true);

		[Ignore]
		public static readonly FrameRate k_50Fps = new FrameRate(50U, false);

		[Ignore]
		public static readonly FrameRate k_60Fps = new FrameRate(60U, false);

		[Ignore]
		public static readonly FrameRate k_59_94Fps = new FrameRate(60U, true);

		[SerializeField]
		private int m_Rate;
	}
}
