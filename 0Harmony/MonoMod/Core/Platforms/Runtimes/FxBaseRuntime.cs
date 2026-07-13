using System;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	internal abstract class FxBaseRuntime : FxCoreBaseRuntime
	{
		public override RuntimeKind Target
		{
			get
			{
				return RuntimeKind.Framework;
			}
		}

		[NullableContext(1)]
		public static FxBaseRuntime CreateForVersion(Version version, ISystem system)
		{
			if (version.Major == 4)
			{
				return new FxCLR4Runtime(system);
			}
			if (version.Major == 2)
			{
				return new FxCLR2Runtime(system);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
			defaultInterpolatedStringHandler.AppendLiteral("CLR version ");
			defaultInterpolatedStringHandler.AppendFormatted<Version>(version);
			defaultInterpolatedStringHandler.AppendLiteral(" is not suppoted.");
			throw new PlatformNotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
