using System;

namespace Mono
{
	internal static class X509Pal
	{
		public static X509PalImpl Instance
		{
			get
			{
				return SystemDependencyProvider.Instance.X509Pal;
			}
		}
	}
}
