using System;
using System.Collections.Generic;
using UnityEngine;

public class Exhaust : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-592767678, new EventSystem.EventHandler(this.OnConduitStateChanged));
		this.Subscribe(-111137758, new EventSystem.EventHandler(this.OnConduitStateChanged));
		base.GetComponent<RequireInputs>().visualizeRequirements = false;
	}

	protected override void OnSpawn()
	{
		this.OnConduitStateChanged(null);
	}

	private void OnConduitStateChanged(object data)
	{
		this.operational.SetActive(this.operational.IsOperational && !this.vent.IsBlocked, false);
	}

	private void SimUpdate(float dt)
	{
		this.operational.SetFlag(Exhaust.canExhaust, !this.vent.IsBlocked);
		if (!this.operational.IsOperational)
		{
			if (this.isAnimating)
			{
				this.isAnimating = false;
				this.recentlyExhausted = false;
				this.Trigger(-793429877, null);
			}
			return;
		}
		int cell = this.vent.Cell;
		if (!Grid.Solid[cell] && this.consumer.ConsumptionRate != 0f)
		{
			List<GameObject> items = this.storage.items;
			if (items.Count > 0)
			{
				ConduitType typeOfConduit = this.consumer.TypeOfConduit;
				if (typeOfConduit != ConduitType.Gas)
				{
					if (typeOfConduit == ConduitType.Liquid)
					{
						int num = Grid.CellBelow(cell);
						bool flag = Grid.IsValidCell(num) && !Grid.Solid[num];
						for (int i = 0; i < items.Count; i++)
						{
							PrimaryElement component = items[i].GetComponent<PrimaryElement>();
							if (component.Mass > 0f && component.Element.IsLiquid)
							{
								if (flag)
								{
									byte b = (byte)ElementLoader.elements.IndexOf(component.Element);
									FallingWater.instance.AddParticle(cell, b, component.Mass, component.Temperature, true, false);
								}
								else
								{
									SimMessages.AddRemoveSubstance(cell, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, -1);
								}
								this.recentlyExhausted = true;
								component.KeepZeroMassObject = true;
								component.Mass = 0f;
								break;
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < items.Count; j++)
					{
						PrimaryElement component2 = items[j].GetComponent<PrimaryElement>();
						if (component2.Mass > 0f && component2.Element.IsGas)
						{
							SimMessages.AddRemoveSubstance(cell, component2.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component2.Mass, component2.Temperature, -1);
							component2.KeepZeroMassObject = true;
							component2.Mass = 0f;
							this.recentlyExhausted = true;
							break;
						}
					}
				}
			}
		}
		this.elapsedSwitchTime -= dt;
		if (this.elapsedSwitchTime <= 0f)
		{
			this.elapsedSwitchTime = 1f;
			if (this.recentlyExhausted != this.isAnimating)
			{
				this.isAnimating = this.recentlyExhausted;
				this.Trigger(-793429877, null);
			}
			this.recentlyExhausted = false;
		}
	}

	public bool IsAnimating()
	{
		return this.isAnimating;
	}

	private const float MinSwitchTime = 1f;

	[MyCmpGet]
	private Vent vent;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private ConduitConsumer consumer;

	private static Operational.Flag canExhaust = new Operational.Flag("canExhaust", Operational.Flag.Type.Requirement);

	private bool isAnimating;

	private bool recentlyExhausted;

	private float elapsedSwitchTime;
}
