using System;
using System.IO;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionModifiers : Modifiers, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		foreach (Klei.AI.Attribute attribute in Db.Get().Attributes.resources)
		{
			if (this.attributes.Get(attribute) == null)
			{
				this.attributes.Add(attribute);
			}
		}
		Traits component = base.GetComponent<Traits>();
		Trait trait = Db.Get().traits.Get(MinionConfig.MINION_BASE_TRAIT_ID);
		component.Add(trait);
		foreach (Disease disease in Db.Get().Diseases)
		{
			AmountInstance amountInstance = this.AddAmount(disease.amount);
			this.attributes.Add(disease.cureSpeedBase);
			amountInstance.SetValue(0f);
		}
		Equipment component2 = base.GetComponent<Equipment>();
		if (component2 != null)
		{
			Ownables component3 = base.GetComponent<Ownables>();
			foreach (AssignableSlot assignableSlot in Db.Get().AssignableSlots)
			{
				if (assignableSlot is OwnableSlot)
				{
					OwnableSlotInstance ownableSlotInstance = new OwnableSlotInstance(component3, (OwnableSlot)assignableSlot);
					component3.Add(ownableSlotInstance);
				}
				else if (assignableSlot is EquipmentSlot)
				{
					EquipmentSlotInstance equipmentSlotInstance = new EquipmentSlotInstance(component2, (EquipmentSlot)assignableSlot);
					component2.Add(equipmentSlotInstance);
				}
			}
		}
		ChoreConsumer component4 = base.GetComponent<ChoreConsumer>();
		if (component4 != null)
		{
			component4.AddProvider(GlobalChoreProvider.Instance);
			base.gameObject.AddComponent<DecorNeed>();
			base.gameObject.AddComponent<FoodQualityNeed>();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ChoreConsumer component = base.GetComponent<ChoreConsumer>();
		if (component != null)
		{
			base.Subscribe(1623392196, new Action<object>(this.OnDeath));
			base.Subscribe(-1506069671, new Action<object>(this.OnAttachFollowCam));
			base.Subscribe(-485480405, new Action<object>(this.OnDetachFollowCam));
			base.Subscribe(-1988963660, new Action<object>(this.OnBeginChore));
			AmountInstance amountInstance = this.GetAmounts().Get("Calories");
			amountInstance.OnMaxValueReached = (global::System.Action)Delegate.Combine(amountInstance.OnMaxValueReached, new global::System.Action(this.OnMaxCaloriesReached));
			Vector3 position = base.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
			base.transform.SetPosition(position);
			base.gameObject.layer = LayerMask.NameToLayer("Default");
			this.SetupDependentAttribute(Db.Get().Attributes.CarryAmount, Db.Get().AttributeConverters.CarryAmountFromStrength);
		}
	}

	private AmountInstance AddAmount(Amount amount)
	{
		AmountInstance amountInstance = new AmountInstance(amount, base.gameObject);
		return this.amounts.Add(amountInstance);
	}

	private void SetupDependentAttribute(Klei.AI.Attribute targetAttribute, AttributeConverter attributeConverter)
	{
		Klei.AI.Attribute attribute = attributeConverter.attribute;
		AttributeInstance attributeInstance = attribute.Lookup(this);
		AttributeModifier target_modifier = new AttributeModifier(targetAttribute.Id, attributeConverter.Lookup(this).Evaluate(), attribute.Name, false, false, false);
		this.GetAttributes().Add("dependent from " + attribute.Id, target_modifier);
		AttributeInstance attributeInstance2 = attributeInstance;
		attributeInstance2.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance2.OnDirty, new global::System.Action(delegate
		{
			target_modifier.SetValue(attributeConverter.Lookup(this).Evaluate());
		}));
	}

	private void OnDeath(object data)
	{
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
		{
			minionIdentity.GetComponent<Effects>().Add("Mourning", true);
		}
	}

	private void OnMaxCaloriesReached()
	{
		base.GetComponent<Effects>().Add("WellFed", true);
	}

	private void OnBeginChore(object data)
	{
		Storage component = base.GetComponent<Storage>();
		if (component != null)
		{
			component.DropAll(false);
		}
	}

	public override void OnSerialize(BinaryWriter writer)
	{
		base.OnSerialize(writer);
	}

	public override void OnDeserialize(IReader reader)
	{
		base.OnDeserialize(reader);
	}

	private void OnAttachFollowCam(object data)
	{
		base.GetComponent<Effects>().Add("CenterOfAttention", false);
	}

	private void OnDetachFollowCam(object data)
	{
		base.GetComponent<Effects>().Remove("CenterOfAttention");
	}
}
