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
		foreach (EquipmentSlot equipmentSlot in EquipmentSet.Get().slotSet)
		{
			EquipmentSlotInstance equipmentSlotInstance = new EquipmentSlotInstance(component, equipmentSlot);
			component.Add(equipmentSlotInstance);
		}
		Ownables component2 = base.GetComponent<Ownables>();
		foreach (OwnableSlot ownableSlot in Db.Get().OwnableSlots)
		{
			OwnableSlotInstance ownableSlotInstance = new OwnableSlotInstance(component2, ownableSlot);
			component2.Add(ownableSlotInstance);
		}
		this.nextNeedDay = GameClock.Instance.GetDay() + 3;
		this.choreConsumer.AddProvider(GlobalChoreProvider.Instance);
		this.choreConsumer.AddProvider(base.GetComponent<ChoreProvider>());
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(-1901442097, new Action<object>(this.OnEffectAdded));
		this.Subscribe(1623392196, new Action<object>(this.OnDeath));
		this.Subscribe(-1506069671, new Action<object>(this.OnAttachFollowCam));
		this.Subscribe(-485480405, new Action<object>(this.OnDetachFollowCam));
		this.Subscribe(-1988963660, new Action<object>(this.OnBeginChore));
		AmountInstance amountInstance = this.GetAmounts().Get("Stress");
		amountInstance.OnDelta = (Action<float>)Delegate.Combine(amountInstance.OnDelta, new Action<float>(delegate(float delta)
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.StressDelta, delta, base.gameObject.GetProperName(), null);
		}));
		AmountInstance amountInstance2 = this.GetAmounts().Get("Calories");
		amountInstance2.OnMaxValueReached = (global::System.Action)Delegate.Combine(amountInstance2.OnMaxValueReached, new global::System.Action(this.OnMaxCaloriesReached));
		Vector3 position = this.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		this.transform.SetPosition(position);
		base.gameObject.AddComponent<DecorNeed>();
		base.gameObject.AddComponent<FoodQualityNeed>();
		base.gameObject.layer = LayerMask.NameToLayer("Default");
	}

	public void OnNewDay()
	{
		if (GameClock.Instance.GetDay() < this.nextNeedDay)
		{
			return;
		}
		this.nextNeedDay = GameClock.Instance.GetDay() + 3;
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
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, effect.Name, this.transform, 1.5f, false);
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
			component.DropAll();
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

	[ReadOnly]
	[Serialize]
	public int nextNeedDay;
}
