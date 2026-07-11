using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property)]
	[Obsolete("Use System.ComponentModel.SettingsBindableAttribute instead to work with the new settings model.")]
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
			if (obj == this)
			{
				return true;
			}
			RecommendedAsConfigurableAttribute recommendedAsConfigurableAttribute = obj as RecommendedAsConfigurableAttribute;
			return recommendedAsConfigurableAttribute != null && recommendedAsConfigurableAttribute.RecommendedAsConfigurable == this.recommendedAsConfigurable;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return !this.recommendedAsConfigurable;
		}

		private bool recommendedAsConfigurable;

		public static readonly RecommendedAsConfigurableAttribute No = new RecommendedAsConfigurableAttribute(false);

		public static readonly RecommendedAsConfigurableAttribute Yes = new RecommendedAsConfigurableAttribute(true);

		public static readonly RecommendedAsConfigurableAttribute Default = RecommendedAsConfigurableAttribute.No;
	}
}
