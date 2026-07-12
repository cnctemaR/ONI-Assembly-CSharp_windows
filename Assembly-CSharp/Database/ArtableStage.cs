using System;

namespace Database
{
	public class ArtableStage : Resource
	{
		public ArtableStage(string id, string name, string animFile, string anim, int decor_value, bool cheer_on_complete, StatusItem status_item, string prefabId, string symbolName = "")
			: base(id, name)
		{
			this.id = id;
			this.name = name;
			this.animFile = animFile;
			this.anim = anim;
			this.symbolName = symbolName;
			this.decor = decor_value;
			this.cheerOnComplete = cheer_on_complete;
			this.statusItem = status_item;
			this.prefabId = prefabId;
		}

		public string id;

		public string name;

		public string anim;

		public string animFile;

		public string prefabId;

		public string symbolName;

		public int decor;

		public bool cheerOnComplete;

		public StatusItem statusItem;
	}
}
