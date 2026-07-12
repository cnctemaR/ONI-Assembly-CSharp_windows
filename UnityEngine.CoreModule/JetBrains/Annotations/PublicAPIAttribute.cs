using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
	public sealed class PublicAPIAttribute : Attribute
	{
		public PublicAPIAttribute()
		{
		}

		public PublicAPIAttribute([NotNull] string comment)
		{
			this.Comment = comment;
		}

		[CanBeNull]
		public string Comment { get; }
	}
}
