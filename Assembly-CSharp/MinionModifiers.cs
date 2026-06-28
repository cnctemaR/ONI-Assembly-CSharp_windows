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
		Equipment component = base.GetComponent<Equipment>();
		Ownables component2 = base.GetComponent<Ownables>();
		foreach (AssignableSlot assignableSlot in Db.Get().AssignableSlots)
		{
			if (assignableSlot is OwnableSlot)
			{
				OwnableSlotInstance ownableSlotInstance = new OwnableSlotInstance(component2, (OwnableSlot)assignableSlot);
				component2.Add(ownableSlotInstance);
			}
			else if (assignableSlot is EquipmentSlot)
			{
				EquipmentSlotInstance equipmentSlotInstance = new EquipmentSlotInstance(component, (EquipmentSlot)assignableSlot);
				component.Add(equipmentSlotInstance);
			}
		}
		this.nextNeedDay = GameClock.Instance.GetCycle() + 3;
		this.choreConsumer.AddProvider(GlobalChoreProvider.Instance);
		this.choreConsumer.AddProvider(base.GetComponent<ChoreProvider>());
		base.gameObject.AddComponent<DecorNeed>();
		base.gameObject.AddComponent<FoodQualityNeed>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-1901442097, new Action<object>(this.OnEffectAdded));
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

	private void SetupDependentAttribute(Klei.AI.Attribute targetAttribute, AttributeConverter attributeConverter)
	{
		Klei.AI.Attribute attribute = attributeConverter.attribute;
		AttributeInstance attributeInstance = attribute.Lookup(this);
		AttributeModifier target_modifier = new AttributeModifier(targetAttribute.Id, 0f, attribute.Name, false, false, false);
		this.GetAttributes().Add("dependent from " + attribute.Id, target_modifier);
		AttributeInstance attributeInstance2 = attributeInstance;
		attributeInstance2.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance2.OnDirty, new global::System.Action(delegate
		{
			target_modifier.SetValue(attributeConverter.Lookup(this).Evaluate());
		}));
	}

	public void OnNewDay()
	{
		if (GameClock.Instance.GetCycle() < this.nextNeedDay)
		{
			return;
		}
		this.nextNeedDay = GameClock.Instance.GetCycle() + 3;
	}

	private void OnDeath(object data)
	{
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
		{
			minionIdentity.GetComponent<Effects>().Add("Mourning", true);
		}
	}

	private void OnEffectAdded(object data)
	{
		Effect effect = (Effect)data;
		if (effect.triggerFloatingText && PopFXManager.Instance != null)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, effect.Name, base.transform, 1.5f, false);
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

	[MyCmpAdd]
	private ChoreConsumer choreConsumer;

	[Serialize]
	[ReadOnly]
	public int nextNeedDay;
}
