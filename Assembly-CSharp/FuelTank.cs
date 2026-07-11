using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class FuelTank : Storage, ISingleSliderControl, ISliderControl
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

	public Tag FuelType
	{
		get
		{
			return this.fuelType;
		}
		set
		{
			this.fuelType = value;
			if (this.storageFilters == null)
			{
				this.storageFilters = new List<Tag>();
			}
			this.storageFilters.Add(this.fuelType);
		}
	}

	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.BUILDINGS.PREFABS.LIQUIDFUELTANK.NAME";
		}
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.MASS.KILOGRAM;
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
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.UserSpecified, Grid.SceneLayer.TransferArm, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
		base.Subscribe(-1697596308, delegate(object data)
		{
			this.meter.SetPositionPercent(base.MassStored() / this.capacityKg);
		});
	}

	public void FillTank()
	{
		CommandModule commandModule = null;
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>());
		foreach (GameObject gameObject in attachedNetwork)
		{
			commandModule = gameObject.GetComponent<CommandModule>();
			if (commandModule)
			{
				break;
			}
		}
		if (commandModule != null)
		{
			RocketEngine mainEngine = commandModule.rocketStats.GetMainEngine();
			if (mainEngine != null)
			{
				base.AddLiquid(ElementLoader.GetElementID(mainEngine.fuelTag), this.minimumLaunchMass - base.MassStored(), ElementLoader.GetElement(mainEngine.fuelTag).defaultValues.temperature, 0, 0, false, true);
			}
		}
		else
		{
			global::Debug.LogWarning("Fuel tank couldn't find command module", null);
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

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 900f;
	}

	public float GetSliderValue(int index)
	{
		return this.TargetFillMass;
	}

	public void SetSliderValue(float mass, int index)
	{
		this.TargetFillMass = mass;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.LIQUIDFUELTANK.FUELAMOUNT";
	}

	private bool isSuspended;

	private MeterController meter;

	[Serialize]
	public float targetFillMass = global::TUNING.BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];

	[SerializeField]
	private Tag fuelType;

	public float minimumLaunchMass = global::TUNING.BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];
}
