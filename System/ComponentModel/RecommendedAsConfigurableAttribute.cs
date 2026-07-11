using System;

namespace System.ComponentModel
{
	[Obsolete("Use SettingsBindableAttribute instead of RecommendedAsConfigurableAttribute")]
	[AttributeUsage(AttributeTargets.Property)]
	public class RecommendedAsConfigurableAttribute : Attribute
	{
		public RecommendedAsConfigurableAttribute(bool recommendedAsConfigurable)
		{
			this.recommendedAsConfigurable = recommendedAsConfigurable;
		}

		public bool RecommendedAsConfigurable
		{
			get
			{
				return this.recommendedAsConfigurable;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is RecommendedAsConfigurableAttribute && ((RecommendedAsConfigurableAttribute)obj).RecommendedAsConfigurable == this.recommendedAsConfigurable;
		}

		public override int GetHashCode()
		{
			return this.recommendedAsConfigurable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.recommendedAsConfigurable == RecommendedAsConfigurableAttribute.Default.RecommendedAsConfigurable;
		}

		private bool recommendedAsConfigurable;

		public static readonly RecommendedAsConfigurableAttribute Default = new RecommendedAsConfigurableAttribute(false);

		public static readonly RecommendedAsConfigurableAttribute No = new RecommendedAsConfigurableAttribute(false);

		public static readonly RecommendedAsConfigurableAttribute Yes = new RecommendedAsConfigurableAttribute(true);
	}
}
