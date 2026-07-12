using System;
using System.Collections.Generic;
using KMod;

namespace Klei
{
	internal class SaveFileRoot
	{
		public SaveFileRoot()
		{
			this.streamed = new Dictionary<string, byte[]>();
		}

		public int WidthInCells;

		public int HeightInCells;

		public Dictionary<string, byte[]> streamed;

		public string clusterID;

		public List<ModInfo> requiredMods;

		public List<Label> active_mods;
	}
}
