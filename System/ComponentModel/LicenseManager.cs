using System;
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
				object obj = LicenseManager.lockObject;
				LicenseContext licenseContext;
				lock (obj)
				{
					if (LicenseManager.mycontext == null)
					{
						LicenseManager.mycontext = new global::System.ComponentModel.Design.RuntimeLicenseContext();
					}
					licenseContext = LicenseManager.mycontext;
				}
				return licenseContext;
			}
			set
			{
				object obj = LicenseManager.lockObject;
				lock (obj)
				{
					if (LicenseManager.contextLockUser != null)
					{
						throw new InvalidOperationException("The CurrentContext property of the LicenseManager is currently locked and cannot be changed.");
					}
					LicenseManager.mycontext = value;
				}
			}
		}

		public static LicenseUsageMode UsageMode
		{
			get
			{
				return LicenseManager.CurrentContext.UsageMode;
			}
		}

		public static object CreateWithContext(Type type, LicenseContext creationContext)
		{
			return LicenseManager.CreateWithContext(type, creationContext, new object[0]);
		}

		public static object CreateWithContext(Type type, LicenseContext creationContext, object[] args)
		{
			object obj = null;
			object obj2 = LicenseManager.lockObject;
			lock (obj2)
			{
				object obj3 = new object();
				LicenseContext currentContext = LicenseManager.CurrentContext;
				LicenseManager.CurrentContext = creationContext;
				LicenseManager.LockContext(obj3);
				try
				{
					obj = Activator.CreateInstance(type, args);
				}
				catch (TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
				finally
				{
					LicenseManager.UnlockContext(obj3);
					LicenseManager.CurrentContext = currentContext;
				}
			}
			return obj;
		}

		public static bool IsLicensed(Type type)
		{
			License license = null;
			if (!LicenseManager.privateGetLicense(type, null, false, out license))
			{
				return false;
			}
			if (license != null)
			{
				license.Dispose();
			}
			return true;
		}

		public static bool IsValid(Type type)
		{
			License license = null;
			if (!LicenseManager.privateGetLicense(type, null, false, out license))
			{
				return false;
			}
			if (license != null)
			{
				license.Dispose();
			}
			return true;
		}

		public static bool IsValid(Type type, object instance, out License license)
		{
			return LicenseManager.privateGetLicense(type, null, false, out license);
		}

		public static void LockContext(object contextUser)
		{
			object obj = LicenseManager.lockObject;
			lock (obj)
			{
				LicenseManager.contextLockUser = contextUser;
			}
		}

		public static void UnlockContext(object contextUser)
		{
			object obj = LicenseManager.lockObject;
			lock (obj)
			{
				if (LicenseManager.contextLockUser != null)
				{
					if (LicenseManager.contextLockUser != contextUser)
					{
						throw new ArgumentException("The CurrentContext property of the LicenseManager can only be unlocked with the same contextUser.");
					}
					LicenseManager.contextLockUser = null;
				}
			}
		}

		public static void Validate(Type type)
		{
			License license = null;
			if (!LicenseManager.privateGetLicense(type, null, true, out license))
			{
				throw new LicenseException(type, null);
			}
			if (license != null)
			{
				license.Dispose();
			}
		}

		public static License Validate(Type type, object instance)
		{
			License license = null;
			if (!LicenseManager.privateGetLicense(type, instance, true, out license))
			{
				throw new LicenseException(type, instance);
			}
			return license;
		}

		private static bool privateGetLicense(Type type, object instance, bool allowExceptions, out License license)
		{
			bool flag = false;
			License license2 = null;
			LicenseProviderAttribute licenseProviderAttribute = (LicenseProviderAttribute)Attribute.GetCustomAttribute(type, typeof(LicenseProviderAttribute), true);
			if (licenseProviderAttribute != null)
			{
				Type licenseProvider = licenseProviderAttribute.LicenseProvider;
				if (licenseProvider != null)
				{
					LicenseProvider licenseProvider2 = (LicenseProvider)Activator.CreateInstance(licenseProvider);
					if (licenseProvider2 != null)
					{
						license2 = licenseProvider2.GetLicense(LicenseManager.CurrentContext, type, instance, allowExceptions);
						if (license2 != null)
						{
							flag = true;
						}
					}
				}
			}
			else
			{
				flag = true;
			}
			license = license2;
			return flag;
		}

		private static LicenseContext mycontext;

		private static object contextLockUser;

		private static object lockObject = new object();
	}
}
