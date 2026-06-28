using System;
using System.Collections;

namespace System.Configuration
{
	public class ConfigurationLocationCollection : ReadOnlyCollectionBase
	{
		internal ConfigurationLocationCollection()
		{
		}

		public ConfigurationLocation this[int index]
		{
			get
			{
				return base.InnerList[index] as ConfigurationLocation;
			}
		}

		internal void Add(ConfigurationLocation loc)
		{
			base.InnerList.Add(loc);
		}

		internal ConfigurationLocation Find(string location)
		{
			foreach (object obj in base.InnerList)
			{
				ConfigurationLocation configurationLocation = (ConfigurationLocation)obj;
				if (string.Compare(configurationLocation.Path, location, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return configurationLocation;
				}
			}
			return null;
		}
	}
}
