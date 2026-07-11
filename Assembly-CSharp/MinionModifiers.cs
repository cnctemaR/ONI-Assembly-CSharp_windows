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
		foreach (Disease disease in Db.Get().Diseases.resources)
		{
			AmountInstance amountInstance = this.AddAmount(disease.amount);
			this.attributes.Add(disease.cureSpeedBase);
			amountInstance.SetValue(0f);
		}
		ChoreConsumer component2 = base.GetComponent<ChoreConsumer>();
		if (component2 != null)
		{
			component2.AddProvider(GlobalChoreProvider.Instance);
			base.gameObject.AddComponent<QualityOfLifeNeed>();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ChoreConsumer component = base.GetComponent<ChoreConsumer>();
		if (component != null)
		{
			base.Subscribe<MinionModifiers>(1623392196, MinionModifiers.OnDeathDelegate);
			base.Subscribe<MinionModifiers>(-1506069671, MinionModifiers.OnAttachFollowCamDelegate);
			base.Subscribe<MinionModifiers>(-485480405, MinionModifiers.OnDetachFollowCamDelegate);
			base.Subscribe<MinionModifiers>(-1988963660, MinionModifiers.OnBeginChoreDelegate);
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
		this.GetAttributes().Add(target_modifier);
		AttributeInstance attributeInstance2 = attributeInstance;
		attributeInstance2.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance2.OnDirty, new global::System.Action(delegate
		{
			target_modifier.SetValue(attributeConverter.Lookup(this).Evaluate());
		}));
	}

	private void OnDeath(object data)
	{
		global::Debug.LogFormat("OnDeath {0}", new object[] { data });
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
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
			component.DropAll(false, false, default(Vector3), true);
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

	private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnDeathDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>(delegate(MinionModifiers component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnAttachFollowCamDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>(delegate(MinionModifiers component, object data)
	{
		component.OnAttachFollowCam(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnDetachFollowCamDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>(delegate(MinionModifiers component, object data)
	{
		component.OnDetachFollowCam(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnBeginChoreDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>(delegate(MinionModifiers component, object data)
	{
		component.OnBeginChore(data);
	});
}
