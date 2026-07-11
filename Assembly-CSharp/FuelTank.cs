using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class FuelTank : Storage
{
	public bool IsSuspended
	{
		get
		{
			return this.isSuspended;
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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		base.gameObject.Subscribe(1366341636, new Action<object>(this.OnReturn));
		RocketModule component = base.GetComponent<RocketModule>();
		FuelTank.HasFuel hasFuel = new FuelTank.HasFuel(this);
		component.AddCondition(hasFuel);
	}

	public void FillTank()
	{
		base.AddLiquid(ElementLoader.GetElementID(this.fuelType), this.minimumLaunchMass, ElementLoader.GetElement(this.fuelType).defaultValues.temperature, 0, 0, false, true);
	}

	private void OnReturn(object data)
	{
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			Util.KDestroyGameObject(this.items[i]);
		}
		this.items.Clear();
	}

	private bool isSuspended;

	[SerializeField]
	private Tag fuelType;

	public float minimumLaunchMass = global::TUNING.BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];

	public class HasFuel : RocketLaunchCondition
	{
		public HasFuel(FuelTank fuelTank)
		{
			this.fuelTank = fuelTank;
		}

		public override bool EvaluateLaunchCondition()
		{
			return this.fuelTank.MassStored() >= this.fuelTank.minimumLaunchMass;
		}

		public override string GetLaunchStatusMessage(bool ready)
		{
			return (!ready) ? UI.STARMAP.FULLTANK.TOOLTIP : UI.STARMAP.EMPTYTANK.NAME;
		}

		public override string GetLaunchStatusTooltip(bool ready)
		{
			return (!ready) ? UI.STARMAP.EMPTYTANK.TOOLTIP : UI.STARMAP.FULLTANK.NAME;
		}

		public override RocketLaunchCondition GetParentCondition()
		{
			return null;
		}

		private FuelTank fuelTank;
	}
}
