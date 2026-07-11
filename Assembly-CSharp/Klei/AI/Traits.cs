using System;
using System.Collections.Generic;
using KSerialization;
using TUNING;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Traits : KMonoBehaviour, ISaveLoadable
	{
		public List<string> GetTraitIds()
		{
			return this.TraitIds;
		}

		public void SetTraitIds(List<string> traits)
		{
			this.TraitIds = traits;
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
			if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 15))
			{
				List<DUPLICANTSTATS.TraitVal> joytraits = DUPLICANTSTATS.JOYTRAITS;
				if (base.GetComponent<MinionIdentity>())
				{
					bool flag = true;
					foreach (DUPLICANTSTATS.TraitVal traitVal in joytraits)
					{
						if (this.HasTrait(traitVal.id))
						{
							flag = false;
						}
					}
					if (flag)
					{
						DUPLICANTSTATS.TraitVal random = joytraits.GetRandom<DUPLICANTSTATS.TraitVal>();
						Trait trait2 = Db.Get().traits.Get(random.id);
						this.Add(trait2);
					}
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
			using (List<Trait>.Enumerator enumerator = this.TraitList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Id == trait_id)
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		public bool HasTrait(Trait trait)
		{
			using (List<Trait>.Enumerator enumerator = this.TraitList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == trait)
					{
						return true;
					}
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
					return;
				}
			}
		}

		public List<Trait> TraitList = new List<Trait>();

		[Serialize]
		private List<string> TraitIds = new List<string>();
	}
}
