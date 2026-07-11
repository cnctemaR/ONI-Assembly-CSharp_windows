using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace System.Xml
{
	internal class SecureStringHasher : IEqualityComparer<string>
	{
		public SecureStringHasher()
		{
			this.hashCodeRandomizer = Environment.TickCount;
		}

		public bool Equals(string x, string y)
		{
			return string.Equals(x, y, StringComparison.Ordinal);
		}

		[SecuritySafeCritical]
		public int GetHashCode(string key)
		{
			if (SecureStringHasher.hashCodeDelegate == null)
			{
				SecureStringHasher.hashCodeDelegate = SecureStringHasher.GetHashCodeDelegate();
			}
			return SecureStringHasher.hashCodeDelegate(key, key.Length, (long)this.hashCodeRandomizer);
		}

		[SecurityCritical]
		private static int GetHashCodeOfString(string key, int sLen, long additionalEntropy)
		{
			int num = (int)additionalEntropy;
			for (int i = 0; i < key.Length; i++)
			{
				num += (num << 7) ^ (int)key[i];
			}
			num -= num >> 17;
			num -= num >> 11;
			return num - (num >> 5);
		}

		[SecuritySafeCritical]
		[ReflectionPermission(SecurityAction.Assert, Unrestricted = true)]
		private static SecureStringHasher.HashCodeOfStringDelegate GetHashCodeDelegate()
		{
			MethodInfo method = typeof(string).GetMethod("InternalMarvin32HashString", BindingFlags.Static | BindingFlags.NonPublic);
			if (method != null)
			{
				return (SecureStringHasher.HashCodeOfStringDelegate)Delegate.CreateDelegate(typeof(SecureStringHasher.HashCodeOfStringDelegate), method);
			}
			return new SecureStringHasher.HashCodeOfStringDelegate(SecureStringHasher.GetHashCodeOfString);
		}

		[SecurityCritical]
		private static SecureStringHasher.HashCodeOfStringDelegate hashCodeDelegate;

		private int hashCodeRandomizer;

		[SecurityCritical]
		private delegate int HashCodeOfStringDelegate(string s, int sLen, long additionalEntropy);
	}
}
