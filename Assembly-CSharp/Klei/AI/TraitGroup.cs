using System;

namespace Klei.AI
{
	public class TraitGroup : ModifierGroup<Trait>
	{
		public TraitGroup(string id, string name, bool is_spawn_trait)
			: base(id, name)
		{
			this.IsSpawnTrait = is_spawn_trait;
		}

		public bool IsSpawnTrait;
	}
}
