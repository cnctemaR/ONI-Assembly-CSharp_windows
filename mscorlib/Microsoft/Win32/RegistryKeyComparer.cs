using System;
using System.Collections;

namespace Microsoft.Win32
{
	internal class RegistryKeyComparer : IEqualityComparer
	{
		public bool Equals(object x, object y)
		{
			return RegistryKey.IsEquals((RegistryKey)x, (RegistryKey)y);
		}

		public int GetHashCode(object obj)
		{
			string name = ((RegistryKey)obj).Name;
			if (name == null)
			{
				return 0;
			}
			return name.GetHashCode();
		}
	}
}
