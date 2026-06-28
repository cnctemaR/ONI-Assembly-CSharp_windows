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
			if (this.rooms.ContainsKey(item.Name))
			{
				return this.rooms[item.Name];
			}
			return null;
		}
	}
}
