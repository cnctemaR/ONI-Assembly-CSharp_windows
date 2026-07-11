using System;
using System.IO;

namespace Database
{
	public class DiscoverNum : ColonyAchievementRequirement
	{
		public DiscoverNum(int numToDiscover)
		{
			this.numToDiscover = numToDiscover;
		}

		public override bool Success()
		{
			return WorldInventory.Instance.GetDiscovered().Count >= this.numToDiscover;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.numToDiscover);
		}

		public override void Deserialize(IReader reader)
		{
			this.numToDiscover = reader.ReadInt32();
		}

		private int numToDiscover;
	}
}
