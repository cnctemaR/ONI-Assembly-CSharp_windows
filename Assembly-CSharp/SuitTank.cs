using System;
using System.Collections.Generic;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SuitTank")]
public class SuitTank : KMonoBehaviour, IGameObjectEffectDescriptor, OxygenBreather.IGasProvider
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<SuitTank>(-1617557748, SuitTank.OnEquippedDelegate);
		base.Subscribe<SuitTank>(-170173755, SuitTank.OnUnequippedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.amount != 0f)
		{
			this.storage.AddGasChunk(SimHashes.Oxygen, this.amount, base.GetComponent<PrimaryElement>().Temperature, byte.MaxValue, 0, false, true);
			this.amount = 0f;
		}
	}

	public float GetTankAmount()
	{
		if (this.storage == null)
		{
			this.storage = base.GetComponent<Storage>();
		}
		return this.storage.GetMassAvailable(this.elementTag);
	}

	public float PercentFull()
	{
		return this.GetTankAmount() / this.capacity;
	}

	public bool IsEmpty()
	{
		return this.GetTankAmount() <= 0f;
	}

	public bool IsFull()
	{
		return this.PercentFull() >= 1f;
	}

	public bool NeedsRecharging()
	{
		return this.PercentFull() < 0.25f;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.elementTag == GameTags.Breathable)
		{
			string text = (this.underwaterSupport ? string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK_UNDERWATER, GameUtil.GetFormattedMass(this.GetTankAmount(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")) : string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK, GameUtil.GetFormattedMass(this.GetTankAmount(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")));
			list.Add(new Descriptor(text, text, Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	private void OnEquipped(object data)
	{
		Equipment equipment = (Equipment)data;
		NameDisplayScreen.Instance.SetSuitTankDisplay(equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), new Func<float>(this.PercentFull), true);
		GameObject targetGameObject = equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
		OxygenBreather component = targetGameObject.GetComponent<OxygenBreather>();
		if (component != null)
		{
			component.GetComponent<Sensors>().GetSensor<SafeCellSensor>().AddIgnoredFlagsSet("SuitTank", this.SafeCellFlagsToIgnoreOnEquipped);
			component.AddGasProvider(this);
		}
		targetGameObject.AddTag(GameTags.HasSuitTank);
	}

	private void OnUnequipped(object data)
	{
		Equipment equipment = (Equipment)data;
		if (!equipment.destroyed)
		{
			NameDisplayScreen.Instance.SetSuitTankDisplay(equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), new Func<float>(this.PercentFull), false);
			GameObject targetGameObject = equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
			OxygenBreather component = targetGameObject.GetComponent<OxygenBreather>();
			if (component != null)
			{
				component.GetComponent<Sensors>().GetSensor<SafeCellSensor>().RemoveIgnoredFlagsSet("SuitTank");
				component.RemoveGasProvider(this);
			}
			targetGameObject.RemoveTag(GameTags.HasSuitTank);
		}
	}

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float amount, Action<SimHashes, float, float, byte, int> onConsumptionCompletedCallback)
	{
		if (this.IsEmpty())
		{
			return false;
		}
		float num = 0f;
		SimHashes simHashes = SimHashes.Vacuum;
		float num2;
		SimUtil.DiseaseInfo diseaseInfo;
		this.storage.ConsumeAndGetDisease(this.elementTag, amount, out num2, out diseaseInfo, out num, out simHashes);
		if (onConsumptionCompletedCallback != null)
		{
			onConsumptionCompletedCallback(simHashes, num2, num, diseaseInfo.idx, diseaseInfo.count);
		}
		base.Trigger(608245985, base.gameObject);
		return true;
	}

	public bool ShouldEmitCO2()
	{
		return !base.GetComponent<KPrefabID>().HasTag(GameTags.AirtightSuit);
	}

	public bool ShouldStoreCO2()
	{
		return base.GetComponent<KPrefabID>().HasTag(GameTags.AirtightSuit);
	}

	public bool IsLowOxygen()
	{
		return this.NeedsRecharging();
	}

	[ContextMenu("SetToRefillAmount")]
	public void SetToRefillAmount()
	{
		float tankAmount = this.GetTankAmount();
		float num = 0.25f * this.capacity;
		if (tankAmount > num)
		{
			this.storage.ConsumeIgnoringDisease(this.elementTag, tankAmount - num);
		}
	}

	[ContextMenu("Empty")]
	public void Empty()
	{
		this.storage.ConsumeIgnoringDisease(this.elementTag, this.GetTankAmount());
	}

	[ContextMenu("Fill Tank")]
	public void FillTank()
	{
		this.Empty();
		this.storage.AddGasChunk(SimHashes.Oxygen, this.capacity, 15f, 0, 0, false, false);
	}

	public bool HasOxygen()
	{
		return !this.IsEmpty();
	}

	public bool IsBlocked()
	{
		return false;
	}

	public SafeCellQuery.SafeFlags SafeCellFlagsToIgnoreOnEquipped = (SafeCellQuery.SafeFlags)464;

	[Serialize]
	public string element;

	[Serialize]
	public float amount;

	public Tag elementTag;

	[MyCmpReq]
	public Storage storage;

	public float capacity;

	public const float REFILL_PERCENT = 0.25f;

	public bool underwaterSupport;

	private static readonly EventSystem.IntraObjectHandler<SuitTank> OnEquippedDelegate = new EventSystem.IntraObjectHandler<SuitTank>(delegate(SuitTank component, object data)
	{
		component.OnEquipped(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitTank> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<SuitTank>(delegate(SuitTank component, object data)
	{
		component.OnUnequipped(data);
	});
}
