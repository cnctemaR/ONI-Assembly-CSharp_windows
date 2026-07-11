using System;
using System.IO;
using KSerialization;

namespace Database
{
	public class EquipNDupes : ColonyAchievementRequirement
	{
		public EquipNDupes(AssignableSlot equipmentSlot, int numToEquip)
		{
			this.equipmentSlot = equipmentSlot;
			this.numToEquip = numToEquip;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (MinionIdentity minionIdentity in Components.MinionIdentities.Items)
			{
				Equipment equipment = minionIdentity.GetEquipment();
				if (equipment != null && equipment.IsSlotOccupied(this.equipmentSlot))
				{
					num++;
				}
			}
			return num >= this.numToEquip;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(this.equipmentSlot.Id);
			writer.Write(this.numToEquip);
		}

		public override void Deserialize(IReader reader)
		{
			string text = reader.ReadKleiString();
			this.equipmentSlot = Db.Get().AssignableSlots.Get(text);
			this.numToEquip = reader.ReadInt32();
		}

		private AssignableSlot equipmentSlot;

		private int numToEquip;
	}
}
