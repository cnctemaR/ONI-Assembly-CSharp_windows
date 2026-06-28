using System;
using System.Collections.Generic;
using KSerialization;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Traits : KMonoBehaviour, ISaveLoadable
	{
		public IEnumerator<Trait> GetEnumerator()
		{
			return this.TraitList.GetEnumerator();
		}

		protected override void OnSpawn()
		{
			foreach (string text in this.TraitIds)
			{
				if (Db.Get().traits.Exists(text))
				{
					Trait trait = Db.Get().traits.Get(text);
					this.AddInternal(trait);
				}
			}
		}

		private void AddInternal(Trait trait)
		{
			if (!this.HasTrait(trait))
			{
				this.TraitList.Add(trait);
				trait.AddTo(this.GetAttributes());
				if (trait.OnAddTrait != null)
				{
					trait.OnAddTrait(base.gameObject);
				}
			}
		}

		public void Add(Trait trait)
		{
			if (trait.ShouldSave)
			{
				this.TraitIds.Add(trait.Id);
			}
			this.AddInternal(trait);
		}

		public bool HasTrait(string trait_id)
		{
			bool flag = false;
			foreach (Trait trait in this.TraitList)
			{
				if (trait.Id == trait_id)
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public bool HasTrait(Trait trait)
		{
			foreach (Trait trait2 in this.TraitList)
			{
				if (trait2 == trait)
				{
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			while (this.TraitList.Count > 0)
			{
				this.Remove(this.TraitList[0]);
			}
		}

		public void Remove(Trait trait)
		{
			for (int i = 0; i < this.TraitList.Count; i++)
			{
				if (this.TraitList[i] == trait)
				{
					this.TraitList.RemoveAt(i);
					this.TraitIds.Remove(trait.Id);
					trait.RemoveFrom(this.GetAttributes());
					break;
				}
			}
		}

		public List<Trait> TraitList = new List<Trait>();

		[Serialize]
		private List<string> TraitIds = new List<string>();
	}
}
