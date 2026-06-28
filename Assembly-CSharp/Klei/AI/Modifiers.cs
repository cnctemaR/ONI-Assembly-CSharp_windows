using System;
using System.IO;
using KSerialization;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Modifiers : KMonoBehaviour, ISaveLoadableDetails
	{
		protected override void OnPrefabInit()
		{
			this.amounts = new Amounts(base.gameObject);
			this.diseases = new Diseases(base.gameObject);
			this.attributes = new Attributes(base.gameObject);
			this.entityType = EntityTypeSet.Instance.TryGet(this.entityTypeId);
			if (this.entityType != null)
			{
				foreach (Attribute attribute in this.modifierSet.Attributes)
				{
					if (this.attributes.Get(attribute) == null)
					{
						this.attributes.Add(attribute);
					}
				}
				this.amounts = this.GetAmounts();
				this.entityType.Apply(this, base.gameObject);
			}
			foreach (Disease disease in Db.Get().Diseases)
			{
				AmountInstance amountInstance = this.amounts.Get(disease.amount);
				if (amountInstance != null)
				{
					amountInstance.SetValue(0f);
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

		public ModifierSet modifierSet;

		public string entityTypeId;

		public EntityType entityType;

		public Amounts amounts;

		public Attributes attributes;

		public Diseases diseases;
	}
}
