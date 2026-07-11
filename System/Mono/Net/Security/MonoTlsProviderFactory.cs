using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Interface;
using Mono.Unity;

namespace Mono.Net.Security
{
	internal static class MonoTlsProviderFactory
	{
		internal static MonoTlsProvider GetProviderInternal()
		{
			object obj = MonoTlsProviderFactory.locker;
			MonoTlsProvider monoTlsProvider;
			lock (obj)
			{
				MonoTlsProviderFactory.InitializeInternal();
				monoTlsProvider = MonoTlsProviderFactory.defaultProvider;
			}
			return monoTlsProvider;
		}

		internal static void InitializeInternal()
		{
			object obj = MonoTlsProviderFactory.locker;
			lock (obj)
			{
				if (!MonoTlsProviderFactory.initialized)
				{
					MonoTlsProviderFactory.InitializeProviderRegistration();
					MonoTlsProvider monoTlsProvider;
					try
					{
						monoTlsProvider = MonoTlsProviderFactory.CreateDefaultProviderImpl();
					}
					catch (Exception ex)
					{
						throw new NotSupportedException("TLS Support not available.", ex);
					}
					if (monoTlsProvider == null)
					{
						throw new NotSupportedException("TLS Support not available.");
					}
					if (!MonoTlsProviderFactory.providerCache.ContainsKey(monoTlsProvider.ID))
					{
						MonoTlsProviderFactory.providerCache.Add(monoTlsProvider.ID, monoTlsProvider);
					}
					X509Helper2.Initialize();
					MonoTlsProviderFactory.defaultProvider = monoTlsProvider;
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
				MonoTlsProviderFactory.defaultProvider = MonoTlsProviderFactory.LookupProvider(provider, true);
				X509Helper2.Initialize();
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

		private static MonoTlsProvider LookupProvider(string name, bool throwOnError)
		{
			object obj = MonoTlsProviderFactory.locker;
			MonoTlsProvider monoTlsProvider;
			lock (obj)
			{
				MonoTlsProviderFactory.InitializeProviderRegistration();
				Tuple<Guid, string> tuple;
				MonoTlsProvider monoTlsProvider2;
				if (!MonoTlsProviderFactory.providerRegistration.TryGetValue(name, out tuple))
				{
					if (throwOnError)
					{
						throw new NotSupportedException(string.Format("No such TLS Provider: `{0}'.", name));
					}
					monoTlsProvider = null;
				}
				else if (MonoTlsProviderFactory.providerCache.TryGetValue(tuple.Item1, out monoTlsProvider2))
				{
					monoTlsProvider = monoTlsProvider2;
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
						monoTlsProvider2 = (MonoTlsProvider)Activator.CreateInstance(type, true);
					}
					catch (Exception ex)
					{
						throw new NotSupportedException(string.Format("Unable to instantiate TLS Provider `{0}'.", type), ex);
					}
					if (monoTlsProvider2 == null)
					{
						if (throwOnError)
						{
							throw new NotSupportedException(string.Format("No such TLS Provider: `{0}'.", name));
						}
						monoTlsProvider = null;
					}
					else
					{
						MonoTlsProviderFactory.providerCache.Add(tuple.Item1, monoTlsProvider2);
						monoTlsProvider = monoTlsProvider2;
					}
				}
			}
			return monoTlsProvider;
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
					MonoTlsProviderFactory.providerCache = new Dictionary<Guid, MonoTlsProvider>();
					if (UnityTls.IsSupported)
					{
						Tuple<Guid, string> tuple = new Tuple<Guid, string>(MonoTlsProviderFactory.UnityTlsId, "Mono.Unity.UnityTlsProvider");
						MonoTlsProviderFactory.providerRegistration.Add("default", tuple);
						MonoTlsProviderFactory.providerRegistration.Add("unitytls", tuple);
					}
					else
					{
						Tuple<Guid, string> tuple2 = new Tuple<Guid, string>(MonoTlsProviderFactory.AppleTlsId, "Mono.AppleTls.AppleTlsProvider");
						Tuple<Guid, string> tuple3 = new Tuple<Guid, string>(MonoTlsProviderFactory.LegacyId, "Mono.Net.Security.LegacyTlsProvider");
						MonoTlsProviderFactory.providerRegistration.Add("legacy", tuple3);
						Tuple<Guid, string> tuple4 = null;
						if (Platform.IsMacOS)
						{
							MonoTlsProviderFactory.providerRegistration.Add("default", tuple2);
						}
						else if (tuple4 != null)
						{
							MonoTlsProviderFactory.providerRegistration.Add("default", tuple4);
						}
						else
						{
							MonoTlsProviderFactory.providerRegistration.Add("default", tuple3);
						}
						MonoTlsProviderFactory.providerRegistration.Add("apple", tuple2);
					}
				}
			}
		}

		private static MonoTlsProvider CreateDefaultProviderImpl()
		{
			string text = Environment.GetEnvironmentVariable("MONO_TLS_PROVIDER");
			if (string.IsNullOrEmpty(text))
			{
				text = "default";
			}
			return MonoTlsProviderFactory.LookupProvider(text, true);
		}

		internal static MonoTlsProvider GetProvider()
		{
			MonoTlsProvider providerInternal = MonoTlsProviderFactory.GetProviderInternal();
			if (providerInternal == null)
			{
				throw new NotSupportedException("No TLS Provider available.");
			}
			return providerInternal;
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

		internal static MonoTlsProvider GetProvider(string name)
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

		private static MonoTlsProvider defaultProvider;

		private static Dictionary<string, Tuple<Guid, string>> providerRegistration;

		private static Dictionary<Guid, MonoTlsProvider> providerCache;

		private static bool enableDebug;

		internal static readonly Guid UnityTlsId = new Guid("06414A97-74F6-488F-877B-A6CA9BBEB82E");

		internal static readonly Guid AppleTlsId = new Guid("981af8af-a3a3-419a-9f01-a518e3a17c1c");

		internal static readonly Guid BtlsId = new Guid("432d18c9-9348-4b90-bfbf-9f2a10e1f15b");

		internal static readonly Guid LegacyId = new Guid("809e77d5-56cc-4da8-b9f0-45e65ba9cceb");
	}
}
