using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class OxidizerTank : Storage, ISingleSliderControl, ISliderControl
{
	public bool IsSuspended
	{
		get
		{
			return this.isSuspended;
		}
	}

	public float TargetFillMass
	{
		get
		{
			return this.targetFillMass;
		}
		set
		{
			this.targetFillMass = value;
			this.capacityKg = this.targetFillMass;
			float num = base.MassStored();
			if (this.capacityKg < num)
			{
				base.DropAll(false);
			}
		}
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 2700f;
	}

	public float GetSliderValue(int index)
	{
		return this.TargetFillMass;
	}

	public void SetSliderValue(float mass, int index)
	{
		this.TargetFillMass = mass;
	}

	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.BUILDINGS.PREFABS.OXIDIZERTANK.NAME";
		}
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.OXIDIZERTANK.FUELAMOUNT";
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.MASS.KILOGRAM;
		}
	}

	public Tag[] OxidizerTypes
	{
		get
		{
			return this.oxidizerTypes;
		}
		set
		{
			this.oxidizerTypes = value;
			if (this.storageFilters == null)
			{
				this.storageFilters = new List<Tag>();
			}
			foreach (Tag tag in this.oxidizerTypes)
			{
				this.storageFilters.Add(tag);
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		base.gameObject.Subscribe(1366341636, new Action<object>(this.OnReturn));
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
		base.Subscribe(-1697596308, delegate(object data)
		{
			this.meter.SetPositionPercent(base.MassStored() / this.capacityKg);
		});
	}

	public void FillTank(SimHashes element)
	{
		if (ElementLoader.FindElementByHash(element).IsLiquid)
		{
			base.AddLiquid(element, this.targetFillMass, ElementLoader.FindElementByHash(element).defaultValues.temperature, 0, 0, false, true);
		}
		else if (ElementLoader.FindElementByHash(element).IsSolid)
		{
			GameObject gameObject = ElementLoader.FindElementByHash(element).substance.SpawnResource(base.gameObject.transform.GetPosition(), this.TargetFillMass, 300f, byte.MaxValue, 0, false, false);
			base.Store(gameObject, false, false, true, false);
		}
	}

	private void OnReturn(object data)
	{
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			Util.KDestroyGameObject(this.items[i]);
		}
		this.items.Clear();
	}

	private MeterController meter;

	private bool isSuspended;

	[Serialize]
	public float targetFillMass = 2700f;

	[SerializeField]
	private Tag[] oxidizerTypes = new Tag[]
	{
		"Oxylite".ToTag(),
		"LiquidOxygen".ToTag()
	};

	public float minimumLaunchMass = global::TUNING.BUILDINGS.ROCKETRY_MASS_KG.OXIDIZER_TANK_OXIDIZER_MASS[0];
}
