using System;
using System.IO;
using KSerialization;

namespace Database
{
	public class DiscoverTag : ColonyAchievementRequirement
	{
		public DiscoverTag(Tag tagToDiscover)
		{
			this.tagToDiscover = tagToDiscover;
		}

		public override bool Success()
		{
			return WorldInventory.Instance.IsDiscovered(this.tagToDiscover);
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(this.tagToDiscover.ToString());
		}

		public override void Deserialize(IReader reader)
		{
			this.tagToDiscover = new Tag(reader.ReadKleiString());
		}

		private Tag tagToDiscover;
	}
}
