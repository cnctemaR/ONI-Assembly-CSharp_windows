using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms
{
	internal readonly struct FeatureFlags : IEquatable<FeatureFlags>
	{
		public ArchitectureFeature Architecture { get; }

		public SystemFeature System { get; }

		public RuntimeFeature Runtime { get; }

		public FeatureFlags(ArchitectureFeature archFlags, SystemFeature sysFlags, RuntimeFeature runtimeFlags)
		{
			this.Runtime = runtimeFlags;
			this.Architecture = archFlags;
			this.System = sysFlags;
		}

		public bool Has(RuntimeFeature feature)
		{
			return (this.Runtime & feature) == feature;
		}

		public bool Has(ArchitectureFeature feature)
		{
			return (this.Architecture & feature) == feature;
		}

		public bool Has(SystemFeature feature)
		{
			return (this.System & feature) == feature;
		}

		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			if (obj is FeatureFlags)
			{
				FeatureFlags featureFlags = (FeatureFlags)obj;
				return this.Equals(featureFlags);
			}
			return false;
		}

		public bool Equals(FeatureFlags other)
		{
			return this.Runtime == other.Runtime && this.Architecture == other.Architecture && this.System == other.System;
		}

		public override int GetHashCode()
		{
			return global::System.HashCode.Combine<RuntimeFeature, ArchitectureFeature, SystemFeature>(this.Runtime, this.Architecture, this.System);
		}

		[NullableContext(1)]
		public override string ToString()
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
			defaultInterpolatedStringHandler.AppendLiteral("(");
			defaultInterpolatedStringHandler.AppendFormatted<ArchitectureFeature>(this.Architecture);
			defaultInterpolatedStringHandler.AppendLiteral(")(");
			defaultInterpolatedStringHandler.AppendFormatted<SystemFeature>(this.System);
			defaultInterpolatedStringHandler.AppendLiteral(")(");
			defaultInterpolatedStringHandler.AppendFormatted<RuntimeFeature>(this.Runtime);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		public static bool operator ==(FeatureFlags left, FeatureFlags right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(FeatureFlags left, FeatureFlags right)
		{
			return !(left == right);
		}
	}
}
