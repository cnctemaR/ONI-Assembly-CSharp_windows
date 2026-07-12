using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;

namespace System.ComponentModel
{
	public sealed class LicenseManager
	{
		private LicenseManager()
		{
		}

		public static LicenseContext CurrentContext
		{
			get
			{
				if (LicenseManager.s_context == null)
				{
					object obj = LicenseManager.s_internalSyncObject;
					lock (obj)
					{
						if (LicenseManager.s_context == null)
						{
							LicenseManager.s_context = new RuntimeLicenseContext();
						}
					}
				}
				return LicenseManager.s_context;
			}
			set
			{
				object obj = LicenseManager.s_internalSyncObject;
				lock (obj)
				{
					if (LicenseManager.s_contextLockHolder != null)
					{
						throw new InvalidOperationException("The CurrentContext property of the LicenseManager is currently locked and cannot be changed.");
					}
					LicenseManager.s_context = value;
				}
			}
		}

		public static LicenseUsageMode UsageMode
		{
			get
			{
				if (LicenseManager.s_context != null)
				{
					return LicenseManager.s_context.UsageMode;
				}
				return LicenseUsageMode.Runtime;
			}
		}

		private static void CacheProvider(Type type, LicenseProvider provider)
		{
			if (LicenseManager.s_providers == null)
			{
				LicenseManager.s_providers = new Hashtable();
			}
			LicenseManager.s_providers[type] = provider;
			if (provider != null)
			{
				if (LicenseManager.s_providerInstances == null)
				{
					LicenseManager.s_providerInstances = new Hashtable();
				}
				LicenseManager.s_providerInstances[provider.GetType()] = provider;
			}
		}

		public static object CreateWithContext(Type type, LicenseContext creationContext)
		{
			return LicenseManager.CreateWithContext(type, creationContext, Array.Empty<object>());
		}

		public static object CreateWithContext(Type type, LicenseContext creationContext, object[] args)
		{
			object obj = null;
			object obj2 = LicenseManager.s_internalSyncObject;
			lock (obj2)
			{
				LicenseContext currentContext = LicenseManager.CurrentContext;
				try
				{
					LicenseManager.CurrentContext = creationContext;
					LicenseManager.LockContext(LicenseManager.s_selfLock);
					try
					{
						obj = SecurityUtils.SecureCreateInstance(type, args);
					}
					catch (TargetInvocationException ex)
					{
						throw ex.InnerException;
					}
				}
				finally
				{
					LicenseManager.UnlockContext(LicenseManager.s_selfLock);
					LicenseManager.CurrentContext = currentContext;
				}
			}
			return obj;
		}

		private static bool GetCachedNoLicenseProvider(Type type)
		{
			return LicenseManager.s_providers != null && LicenseManager.s_providers.ContainsKey(type);
		}

		private static LicenseProvider GetCachedProvider(Type type)
		{
			Hashtable hashtable = LicenseManager.s_providers;
			return (LicenseProvider)((hashtable != null) ? hashtable[type] : null);
		}

		private static LicenseProvider GetCachedProviderInstance(Type providerType)
		{
			Hashtable hashtable = LicenseManager.s_providerInstances;
			return (LicenseProvider)((hashtable != null) ? hashtable[providerType] : null);
		}

		public static bool IsLicensed(Type type)
		{
			License license;
			bool flag = LicenseManager.ValidateInternal(type, null, false, out license);
			if (license != null)
			{
				license.Dispose();
				license = null;
			}
			return flag;
		}

		public static bool IsValid(Type type)
		{
			License license;
			bool flag = LicenseManager.ValidateInternal(type, null, false, out license);
			if (license != null)
			{
				license.Dispose();
				license = null;
			}
			return flag;
		}

		public static bool IsValid(Type type, object instance, out License license)
		{
			return LicenseManager.ValidateInternal(type, instance, false, out license);
		}

		public static void LockContext(object contextUser)
		{
			object obj = LicenseManager.s_internalSyncObject;
			lock (obj)
			{
				if (LicenseManager.s_contextLockHolder != null)
				{
					throw new InvalidOperationException("The CurrentContext property of the LicenseManager is already locked by another user.");
				}
				LicenseManager.s_contextLockHolder = contextUser;
			}
		}

		public static void UnlockContext(object contextUser)
		{
			object obj = LicenseManager.s_internalSyncObject;
			lock (obj)
			{
				if (LicenseManager.s_contextLockHolder != contextUser)
				{
					throw new ArgumentException("The CurrentContext property of the LicenseManager can only be unlocked with the same contextUser.");
				}
				LicenseManager.s_contextLockHolder = null;
			}
		}

		private static bool ValidateInternal(Type type, object instance, bool allowExceptions, out License license)
		{
			string text;
			return LicenseManager.ValidateInternalRecursive(LicenseManager.CurrentContext, type, instance, allowExceptions, out license, out text);
		}

		private static bool ValidateInternalRecursive(LicenseContext context, Type type, object instance, bool allowExceptions, out License license, out string licenseKey)
		{
			LicenseProvider licenseProvider = LicenseManager.GetCachedProvider(type);
			if (licenseProvider == null && !LicenseManager.GetCachedNoLicenseProvider(type))
			{
				LicenseProviderAttribute licenseProviderAttribute = (LicenseProviderAttribute)Attribute.GetCustomAttribute(type, typeof(LicenseProviderAttribute), false);
				if (licenseProviderAttribute != null)
				{
					Type licenseProvider2 = licenseProviderAttribute.LicenseProvider;
					licenseProvider = LicenseManager.GetCachedProviderInstance(licenseProvider2) ?? ((LicenseProvider)SecurityUtils.SecureCreateInstance(licenseProvider2));
				}
				LicenseManager.CacheProvider(type, licenseProvider);
			}
			license = null;
			bool flag = true;
			licenseKey = null;
			if (licenseProvider != null)
			{
				license = licenseProvider.GetLicense(context, type, instance, allowExceptions);
				if (license == null)
				{
					flag = false;
				}
				else
				{
					licenseKey = license.LicenseKey;
				}
			}
			if (flag && instance == null)
			{
				Type baseType = type.BaseType;
				if (baseType != typeof(object) && baseType != null)
				{
					if (license != null)
					{
						license.Dispose();
						license = null;
					}
					string text;
					flag = LicenseManager.ValidateInternalRecursive(context, baseType, null, allowExceptions, out license, out text);
					if (license != null)
					{
						license.Dispose();
						license = null;
					}
				}
			}
			return flag;
		}

		public static void Validate(Type type)
		{
			License license;
			if (!LicenseManager.ValidateInternal(type, null, true, out license))
			{
				throw new LicenseException(type);
			}
			if (license != null)
			{
				license.Dispose();
				license = null;
			}
		}

		public static License Validate(Type type, object instance)
		{
			License license;
			if (!LicenseManager.ValidateInternal(type, instance, true, out license))
			{
				throw new LicenseException(type, instance);
			}
			return license;
		}

		private static readonly object s_selfLock = new object();

		private static volatile LicenseContext s_context;

		private static object s_contextLockHolder;

		private static volatile Hashtable s_providers;

		private static volatile Hashtable s_providerInstances;

		private static readonly object s_internalSyncObject = new object();
	}
}
