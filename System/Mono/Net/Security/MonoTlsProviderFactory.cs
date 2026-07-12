using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mono.Btls;
using Mono.Unity;

namespace Mono.Net.Security
{
	internal static class MonoTlsProviderFactory
	{
		internal static MobileTlsProvider GetProviderInternal()
		{
			object obj = MonoTlsProviderFactory.locker;
			MobileTlsProvider mobileTlsProvider;
			lock (obj)
			{
				MonoTlsProviderFactory.InitializeInternal();
				mobileTlsProvider = MonoTlsProviderFactory.defaultProvider;
			}
			return mobileTlsProvider;
		}

		internal static void InitializeInternal()
		{
			object obj = MonoTlsProviderFactory.locker;
			lock (obj)
			{
				if (!MonoTlsProviderFactory.initialized)
				{
					SystemDependencyProvider.Initialize();
					MonoTlsProviderFactory.InitializeProviderRegistration();
					MobileTlsProvider mobileTlsProvider;
					try
					{
						mobileTlsProvider = MonoTlsProviderFactory.CreateDefaultProviderImpl();
					}
					catch (Exception ex)
					{
						throw new NotSupportedException("TLS Support not available.", ex);
					}
					if (mobileTlsProvider == null)
					{
						throw new NotSupportedException("TLS Support not available.");
					}
					if (!MonoTlsProviderFactory.providerCache.ContainsKey(mobileTlsProvider.ID))
					{
						MonoTlsProviderFactory.providerCache.Add(mobileTlsProvider.ID, mobileTlsProvider);
					}
					MonoTlsProviderFactory.defaultProvider = mobileTlsProvider;
					MonoTlsProviderFactory.initialized = true;
				}
			}
		}

		internal static void InitializeInternal(string provider)
		{
			object obj = MonoTlsProviderFactory.locker;
			lock (obj)
			{
				if (MonoTlsProviderFactory.initialized)
				{
					throw new NotSupportedException("TLS Subsystem already initialized.");
				}
				SystemDependencyProvider.Initialize();
				MonoTlsProviderFactory.defaultProvider = MonoTlsProviderFactory.LookupProvider(provider, true);
				MonoTlsProviderFactory.initialized = true;
			}
		}

		private static Type LookupProviderType(string name, bool throwOnError)
		{
			object obj = MonoTlsProviderFactory.locker;
			Type type;
			lock (obj)
			{
				MonoTlsProviderFactory.InitializeProviderRegistration();
				Tuple<Guid, string> tuple;
				if (!MonoTlsProviderFactory.providerRegistration.TryGetValue(name, out tuple))
				{
					if (throwOnError)
					{
						throw new NotSupportedException(string.Format("No such TLS Provider: `{0}'.", name));
					}
					type = null;
				}
				else
				{
					Type type2 = Type.GetType(tuple.Item2, false);
					if (type2 == null && throwOnError)
					{
						throw new NotSupportedException(string.Format("Could not find TLS Provider: `{0}'.", tuple.Item2));
					}
					type = type2;
				}
			}
			return type;
		}

		private static MobileTlsProvider LookupProvider(string name, bool throwOnError)
		{
			object obj = MonoTlsProviderFactory.locker;
			MobileTlsProvider mobileTlsProvider;
			lock (obj)
			{
				MonoTlsProviderFactory.InitializeProviderRegistration();
				Tuple<Guid, string> tuple;
				MobileTlsProvider mobileTlsProvider2;
				if (!MonoTlsProviderFactory.providerRegistration.TryGetValue(name, out tuple))
				{
					if (throwOnError)
					{
						throw new NotSupportedException(string.Format("No such TLS Provider: `{0}'.", name));
					}
					mobileTlsProvider = null;
				}
				else if (MonoTlsProviderFactory.providerCache.TryGetValue(tuple.Item1, out mobileTlsProvider2))
				{
					mobileTlsProvider = mobileTlsProvider2;
				}
				else
				{
					Type type = Type.GetType(tuple.Item2, false);
					if (type == null && throwOnError)
					{
						throw new NotSupportedException(string.Format("Could not find TLS Provider: `{0}'.", tuple.Item2));
					}
					try
					{
						mobileTlsProvider2 = (MobileTlsProvider)Activator.CreateInstance(type, true);
					}
					catch (Exception ex)
					{
						throw new NotSupportedException(string.Format("Unable to instantiate TLS Provider `{0}'.", type), ex);
					}
					if (mobileTlsProvider2 == null)
					{
						if (throwOnError)
						{
							throw new NotSupportedException(string.Format("No such TLS Provider: `{0}'.", name));
						}
						mobileTlsProvider = null;
					}
					else
					{
						MonoTlsProviderFactory.providerCache.Add(tuple.Item1, mobileTlsProvider2);
						mobileTlsProvider = mobileTlsProvider2;
					}
				}
			}
			return mobileTlsProvider;
		}

		[Conditional("MONO_TLS_DEBUG")]
		private static void InitializeDebug()
		{
			if (Environment.GetEnvironmentVariable("MONO_TLS_DEBUG") != null)
			{
				MonoTlsProviderFactory.enableDebug = true;
			}
		}

		[Conditional("MONO_TLS_DEBUG")]
		internal static void Debug(string message, params object[] args)
		{
			if (MonoTlsProviderFactory.enableDebug)
			{
				Console.Error.WriteLine(message, args);
			}
		}

		private static void InitializeProviderRegistration()
		{
			object obj = MonoTlsProviderFactory.locker;
			lock (obj)
			{
				if (MonoTlsProviderFactory.providerRegistration == null)
				{
					MonoTlsProviderFactory.providerRegistration = new Dictionary<string, Tuple<Guid, string>>();
					MonoTlsProviderFactory.providerCache = new Dictionary<Guid, MobileTlsProvider>();
					if (UnityTls.IsSupported)
					{
						MonoTlsProviderFactory.PopulateUnityProviders();
					}
					else
					{
						MonoTlsProviderFactory.PopulateProviders();
					}
				}
			}
		}

		private static void PopulateUnityProviders()
		{
			Tuple<Guid, string> tuple = new Tuple<Guid, string>(MonoTlsProviderFactory.UnityTlsId, "Mono.Unity.UnityTlsProvider");
			MonoTlsProviderFactory.providerRegistration.Add("default", tuple);
			MonoTlsProviderFactory.providerRegistration.Add("unitytls", tuple);
		}

		private static void PopulateProviders()
		{
			Tuple<Guid, string> tuple = null;
			Tuple<Guid, string> tuple2 = null;
			if (MonoTlsProviderFactory.IsBtlsSupported())
			{
				tuple2 = new Tuple<Guid, string>(MonoTlsProviderFactory.BtlsId, typeof(MonoBtlsProvider).FullName);
				MonoTlsProviderFactory.providerRegistration.Add("btls", tuple2);
			}
			Tuple<Guid, string> tuple3 = tuple ?? tuple2;
			if (tuple3 != null)
			{
				MonoTlsProviderFactory.providerRegistration.Add("default", tuple3);
				MonoTlsProviderFactory.providerRegistration.Add("legacy", tuple3);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsBtlsSupported();

		private static MobileTlsProvider CreateDefaultProviderImpl()
		{
			string text = Environment.GetEnvironmentVariable("MONO_TLS_PROVIDER");
			if (string.IsNullOrEmpty(text))
			{
				text = "default";
			}
			if (!(text == "default") && !(text == "legacy"))
			{
				if (!(text == "btls"))
				{
					if (!(text == "unitytls"))
					{
						return MonoTlsProviderFactory.LookupProvider(text, true);
					}
					goto IL_006E;
				}
			}
			else
			{
				if (UnityTls.IsSupported)
				{
					goto IL_006E;
				}
				if (!MonoTlsProviderFactory.IsBtlsSupported())
				{
					throw new NotSupportedException("TLS Support not available.");
				}
			}
			return new MonoBtlsProvider();
			IL_006E:
			return new UnityTlsProvider();
		}

		internal static MobileTlsProvider GetProvider()
		{
			return MonoTlsProviderFactory.GetProviderInternal();
		}

		internal static bool IsProviderSupported(string name)
		{
			object obj = MonoTlsProviderFactory.locker;
			bool flag2;
			lock (obj)
			{
				MonoTlsProviderFactory.InitializeProviderRegistration();
				flag2 = MonoTlsProviderFactory.providerRegistration.ContainsKey(name);
			}
			return flag2;
		}

		internal static MobileTlsProvider GetProvider(string name)
		{
			return MonoTlsProviderFactory.LookupProvider(name, false);
		}

		internal static bool IsInitialized
		{
			get
			{
				object obj = MonoTlsProviderFactory.locker;
				bool flag2;
				lock (obj)
				{
					flag2 = MonoTlsProviderFactory.initialized;
				}
				return flag2;
			}
		}

		internal static void Initialize()
		{
			MonoTlsProviderFactory.InitializeInternal();
		}

		internal static void Initialize(string provider)
		{
			MonoTlsProviderFactory.InitializeInternal(provider);
		}

		private static object locker = new object();

		private static bool initialized;

		private static MobileTlsProvider defaultProvider;

		private static Dictionary<string, Tuple<Guid, string>> providerRegistration;

		private static Dictionary<Guid, MobileTlsProvider> providerCache;

		private static bool enableDebug;

		internal static readonly Guid UnityTlsId = new Guid("06414A97-74F6-488F-877B-A6CA9BBEB82E");

		internal static readonly Guid AppleTlsId = new Guid("981af8af-a3a3-419a-9f01-a518e3a17c1c");

		internal static readonly Guid BtlsId = new Guid("432d18c9-9348-4b90-bfbf-9f2a10e1f15b");
	}
}
