using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mono
{
	internal static class DependencyInjector
	{
		internal static ISystemDependencyProvider SystemProvider
		{
			get
			{
				if (DependencyInjector.systemDependency != null)
				{
					return DependencyInjector.systemDependency;
				}
				object obj = DependencyInjector.locker;
				ISystemDependencyProvider systemDependencyProvider;
				lock (obj)
				{
					if (DependencyInjector.systemDependency != null)
					{
						systemDependencyProvider = DependencyInjector.systemDependency;
					}
					else
					{
						DependencyInjector.systemDependency = DependencyInjector.ReflectionLoad();
						if (DependencyInjector.systemDependency == null)
						{
							throw new PlatformNotSupportedException("Cannot find 'Mono.SystemDependencyProvider, System' dependency");
						}
						systemDependencyProvider = DependencyInjector.systemDependency;
					}
				}
				return systemDependencyProvider;
			}
		}

		internal static void Register(ISystemDependencyProvider provider)
		{
			object obj = DependencyInjector.locker;
			lock (obj)
			{
				if (DependencyInjector.systemDependency != null && DependencyInjector.systemDependency != provider)
				{
					throw new InvalidOperationException();
				}
				DependencyInjector.systemDependency = provider;
			}
		}

		[PreserveDependency("get_Instance()", "Mono.SystemDependencyProvider", "System")]
		private static ISystemDependencyProvider ReflectionLoad()
		{
			Type type = Type.GetType("Mono.SystemDependencyProvider, System");
			if (type == null)
			{
				return null;
			}
			PropertyInfo property = type.GetProperty("Instance", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
			if (property == null)
			{
				return null;
			}
			return (ISystemDependencyProvider)property.GetValue(null);
		}

		private const string TypeName = "Mono.SystemDependencyProvider, System";

		private static object locker = new object();

		private static ISystemDependencyProvider systemDependency;
	}
}
