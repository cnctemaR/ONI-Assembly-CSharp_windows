using System;
using System.IO;
using Klei.AI;
using KSerialization;

namespace Database
{
	public class DupesWithTrait : ColonyAchievementRequirement
	{
		public DupesWithTrait(string traitId, bool hasTrait = true)
		{
			this.traitId = traitId;
		}

		public override bool Success()
		{
			foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
			{
				if (minionIdentity.GetComponent<Traits>().HasTrait(this.traitId) != this.hasTrait)
				{
					return false;
				}
			}
			return true;
		}

		public override bool Fail()
		{
			return !this.Success();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write((!this.hasTrait) ? 0 : 1);
			writer.WriteKleiString(this.traitId);
		}

		public override void Deserialize(IReader reader)
		{
			this.hasTrait = reader.ReadByte() != 0;
			this.traitId = reader.ReadKleiString();
		}

		private bool hasTrait;

		private string traitId;
	}
}
