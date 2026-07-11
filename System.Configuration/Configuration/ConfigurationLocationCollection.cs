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

		internal ConfigurationLocation FindBest(string location)
		{
			if (string.IsNullOrEmpty(location))
			{
				return null;
			}
			ConfigurationLocation configurationLocation = null;
			int length = location.Length;
			int num = 0;
			foreach (object obj in base.InnerList)
			{
				ConfigurationLocation configurationLocation2 = (ConfigurationLocation)obj;
				string path = configurationLocation2.Path;
				if (!string.IsNullOrEmpty(path))
				{
					int length2 = path.Length;
					if (location.StartsWith(path, StringComparison.OrdinalIgnoreCase))
					{
						if (length == length2)
						{
							return configurationLocation2;
						}
						if (length <= length2 || location[length2] == '/')
						{
							if (configurationLocation == null)
							{
								configurationLocation = configurationLocation2;
							}
							else if (num < length2)
							{
								configurationLocation = configurationLocation2;
								num = length2;
							}
						}
					}
				}
			}
			return configurationLocation;
		}
	}
}
