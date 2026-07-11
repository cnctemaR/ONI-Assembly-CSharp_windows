using System;
using System.IO;
using KSerialization;

namespace Database
{
	public class BuildRoomType : ColonyAchievementRequirement
	{
		public BuildRoomType(RoomType roomType)
		{
			this.roomType = roomType;
		}

		public override bool Success()
		{
			foreach (Room room in Game.Instance.roomProber.rooms)
			{
				if (room.roomType == this.roomType)
				{
					return true;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(this.roomType.Id);
		}

		public override void Deserialize(IReader reader)
		{
			string text = reader.ReadKleiString();
			this.roomType = Db.Get().RoomTypes.Get(text);
		}

		private RoomType roomType;
	}
}
