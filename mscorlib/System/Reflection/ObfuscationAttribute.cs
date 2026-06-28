using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	public sealed class ObfuscationAttribute : Attribute
	{
		public ObfuscationAttribute()
		{
			this.exclude = true;
			this.strip = true;
			this.applyToMembers = true;
			this.feature = "all";
		}

		public bool Exclude
		{
			get
			{
				return this.exclude;
			}
			set
			{
				this.exclude = value;
			}
		}

		public bool StripAfterObfuscation
		{
			get
			{
				return this.strip;
			}
			set
			{
				this.strip = value;
			}
		}

		public bool ApplyToMembers
		{
			get
			{
				return this.applyToMembers;
			}
			set
			{
				this.applyToMembers = value;
			}
		}

		public string Feature
		{
			get
			{
				return this.feature;
			}
			set
			{
				this.feature = value;
			}
		}

		private bool exclude;

		private bool strip;

		private bool applyToMembers;

		private string feature;
	}
}
