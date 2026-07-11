using System;

namespace ProcGen
{
	[Serializable]
	public class MobSettings : IMerge<MobSettings>
	{
		public ComposableDictionary<string, Mob> MobLookupTable { get; private set; }

		public MobSettings()
		{
			this.MobLookupTable = new ComposableDictionary<string, Mob>();
		}

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
				foreach (string text in this.MobLookupTable.Keys)
				{
					this.mobkeys.Add(new Tag(text));
				}
			}
			return this.mobkeys;
		}

		public void Merge(MobSettings other)
		{
			this.MobLookupTable.Merge(other.MobLookupTable);
			this.mobkeys = null;
		}

		public static float AmbientMobDensity = 1f;

		private TagSet mobkeys;
	}
}
