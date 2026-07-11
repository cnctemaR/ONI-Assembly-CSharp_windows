using System;

namespace System.Net.NetworkInformation
{
	public class PingOptions
	{
		public PingOptions()
		{
		}

		public PingOptions(int ttl, bool dontFragment)
		{
			if (ttl <= 0)
			{
				throw new ArgumentOutOfRangeException("Must be greater than zero.", "ttl");
			}
			this.ttl = ttl;
			this.dont_fragment = dontFragment;
		}

		public bool DontFragment
		{
			get
			{
				return this.dont_fragment;
			}
			set
			{
				this.dont_fragment = value;
			}
		}

		public int Ttl
		{
			get
			{
				return this.ttl;
			}
			set
			{
				this.ttl = value;
			}
		}

		private int ttl = 128;

		private bool dont_fragment;
	}
}
