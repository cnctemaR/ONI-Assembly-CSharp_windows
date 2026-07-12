using System;

namespace UnityEngine.SubsystemsImplementation.Extensions
{
	public static class SubsystemDescriptorExtensions
	{
		public static SubsystemProxy<TSubsystem, TProvider> CreateProxy<TSubsystem, TProvider>(this SubsystemDescriptorWithProvider<TSubsystem, TProvider> descriptor) where TSubsystem : SubsystemWithProvider, new() where TProvider : SubsystemProvider<TSubsystem>
		{
			TProvider tprovider = descriptor.CreateProvider();
			return (tprovider != null) ? new SubsystemProxy<TSubsystem, TProvider>(tprovider) : null;
		}
	}
}
