using System;
using System.IO;
using KSerialization;

namespace Database
{
	public class EquipAllDupes : ColonyAchievementRequirement
	{
		public EquipAllDupes(AssignableSlot equipmentSlot)
		{
			this.equipmentSlot = equipmentSlot;
		}

		public override bool Success()
		{
			foreach (MinionIdentity minionIdentity in Components.MinionIdentities.Items)
			{
				Equipment equipment = minionIdentity.GetEquipment();
				if (equipment != null && !equipment.IsSlotOccupied(this.equipmentSlot))
				{
					return false;
				}
			}
			return true;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(this.equipmentSlot.Id);
		}

		public override void Deserialize(IReader reader)
		{
			string text = reader.ReadKleiString();
			this.equipmentSlot = Db.Get().AssignableSlots.Get(text);
		}

		private AssignableSlot equipmentSlot;
	}
}
