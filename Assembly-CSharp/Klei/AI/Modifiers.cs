using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Modifiers : KMonoBehaviour, ISaveLoadableDetails
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.amounts = new Amounts(base.gameObject);
			this.diseases = new Diseases(base.gameObject);
			this.attributes = new Attributes(base.gameObject);
			foreach (string text in this.initialAmounts)
			{
				this.amounts.Add(new AmountInstance(Db.Get().Amounts.Get(text), base.gameObject));
			}
			Traits component = base.GetComponent<Traits>();
			if (this.initialTraits != null)
			{
				foreach (string text2 in this.initialTraits)
				{
					Trait trait = Db.Get().traits.Get(text2);
					component.Add(trait);
				}
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			this.OnSerialize(writer);
		}

		public void Deserialize(IReader reader)
		{
			this.OnDeserialize(reader);
		}

		public virtual void OnSerialize(BinaryWriter writer)
		{
			this.amounts.Serialize(writer);
			this.diseases.Serialize(writer);
		}

		public virtual void OnDeserialize(IReader reader)
		{
			this.amounts.Deserialize(reader);
			this.diseases.Deserialize(reader);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			if (this.amounts != null)
			{
				this.amounts.Cleanup();
			}
		}

		public Amounts amounts;

		public Attributes attributes;

		public Diseases diseases;

		public string[] initialTraits;

		public List<string> initialAmounts = new List<string>();
	}
}
