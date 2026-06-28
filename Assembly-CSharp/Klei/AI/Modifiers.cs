using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Modifiers : KMonoBehaviour, ISaveLoadableDetailJson
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
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();
			Components.Modifiers.Add(this);
		}

		public void DoUpdate()
		{
			List<AmountInstance> modifierList = this.amounts.ModifierList;
			float deltaTime = Time.deltaTime;
			int count = modifierList.Count;
			for (int i = 0; i < count; i++)
			{
				AmountInstance amountInstance = modifierList[i];
				float num = amountInstance.GetDelta() * deltaTime;
				if (num != 0f)
				{
					amountInstance.ApplyDelta(num);
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
			Components.Modifiers.Remove(this);
		}

		public ModifierSet modifierSet;

		public string entityTypeId;

		public EntityType entityType;

		public Amounts amounts;

		public Attributes attributes;

		public Diseases diseases;
	}
}
