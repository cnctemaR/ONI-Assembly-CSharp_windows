using System;
using System.Collections.Generic;
using Klei;

namespace ProcGen
{
	public class MobSettings : YamlIO<MobSettings>
	{
		public MobSettings()
		{
			this.MobLookupTable = new Dictionary<string, Mob>();
		}

		public Dictionary<string, Mob> MobLookupTable { get; private set; }

		public bool HasMob(string id)
		{
			return this.MobLookupTable.ContainsKey(id);
		}

		public Mob GetMob(string id)
		{
			Mob mob = null;
			this.MobLookupTable.TryGetValue(id, out mob);
			return mob;
		}

		public TagSet GetMobTags()
		{
			if (this.mobkeys == null)
			{
				this.mobkeys = new TagSet();
				Dictionary<string, Mob>.Enumerator enumerator = this.MobLookupTable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TagSet tagSet = this.mobkeys;
					KeyValuePair<string, Mob> keyValuePair = enumerator.Current;
					tagSet.Add(new Tag(keyValuePair.Key));
				}
			}
			return this.mobkeys;
		}

		public static int AmbientMobDensity = 1;

		private TagSet mobkeys = null;
	}
}
