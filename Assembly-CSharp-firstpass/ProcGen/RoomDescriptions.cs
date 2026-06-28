using System;
using System.Collections.Generic;
using Klei;

namespace ProcGen
{
	public class RoomDescriptions : YamlIO<RoomDescriptions>
	{
		public RoomDescriptions()
		{
			this.rooms = new Dictionary<string, Room>();
		}

		public Dictionary<string, Room> rooms { get; private set; }

		public Room GetDesription(Tag item)
		{
			Room room;
			if (this.rooms.ContainsKey(item.Name))
			{
				room = this.rooms[item.Name];
			}
			else
			{
				room = null;
			}
			return room;
		}
	}
}
