using System;
using System.IO;
using KSerialization;

namespace Database
{
	public class BuildNRoomTypes : ColonyAchievementRequirement
	{
		public BuildNRoomTypes(RoomType roomType, int numToCreate = 1)
		{
			this.roomType = roomType;
			this.numToCreate = numToCreate;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (Room room in Game.Instance.roomProber.rooms)
			{
				if (room.roomType == this.roomType)
				{
					num++;
				}
			}
			return num >= this.numToCreate;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(this.roomType.Id);
			writer.Write(this.numToCreate);
		}

		public override void Deserialize(IReader reader)
		{
			string text = reader.ReadKleiString();
			this.roomType = Db.Get().RoomTypes.Get(text);
			this.numToCreate = reader.ReadInt32();
		}

		private RoomType roomType;

		private int numToCreate;
	}
}
