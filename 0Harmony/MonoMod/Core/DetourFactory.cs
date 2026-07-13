using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Core.Platforms;
using MonoMod.Utils;

namespace MonoMod.Core
{
	[NullableContext(1)]
	[Nullable(0)]
	[CLSCompliant(true)]
	internal static class DetourFactory
	{
		public static IDetourFactory Default
		{
			get
			{
				return Helpers.GetOrInitWithLock<IDetourFactory>(ref DetourFactory.lazyDefault, DetourFactory.currentLock, ldftn(CreateDefault));
			}
		}

		private static IDetourFactory CreateDefault()
		{
			return new PlatformTripleDetourFactory(PlatformTriple.Current);
		}

		public static IDetourFactory Current
		{
			get
			{
				return Helpers.GetOrInitWithLock<IDetourFactory>(ref DetourFactory.lazyCurrent, DetourFactory.currentLock, ldftn(CreateCurrent));
			}
		}

		private static IDetourFactory CreateCurrent()
		{
			return DetourFactory.Default;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetCurrentFactory(Func<IDetourFactory, IDetourFactory> creator)
		{
			Helpers.ThrowIfArgumentNull<Func<IDetourFactory, IDetourFactory>>(creator, "creator");
			object obj = DetourFactory.currentLock;
			lock (obj)
			{
				DetourFactory.lazyCurrent = creator(DetourFactory.Current);
			}
		}

		public static ICoreDetour CreateDetour(this IDetourFactory factory, MethodBase source, MethodBase target, bool applyByDefault = true)
		{
			Helpers.ThrowIfArgumentNull<IDetourFactory>(factory, "factory");
			return factory.CreateDetour(new CreateDetourRequest(source, target)
			{
				ApplyByDefault = applyByDefault
			});
		}

		public static ICoreNativeDetour CreateNativeDetour(this IDetourFactory factory, IntPtr source, IntPtr target, bool applyByDefault = true)
		{
			Helpers.ThrowIfArgumentNull<IDetourFactory>(factory, "factory");
			return factory.CreateNativeDetour(new CreateNativeDetourRequest(source, target)
			{
				ApplyByDefault = applyByDefault
			});
		}

		private static object currentLock = new object();

		[Nullable(2)]
		private static IDetourFactory lazyDefault;

		[Nullable(2)]
		private static IDetourFactory lazyCurrent;
	}
}
