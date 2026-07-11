using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using STRINGS;

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
			using (List<Room>.Enumerator enumerator = Game.Instance.roomProber.rooms.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.roomType == this.roomType)
					{
						return true;
					}
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

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_A_ROOM, this.roomType.Name);
		}

		private RoomType roomType;
	}
}
