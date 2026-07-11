using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Exhaust : KMonoBehaviour, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<Exhaust>(-592767678, Exhaust.OnConduitStateChangedDelegate);
		base.Subscribe<Exhaust>(-111137758, Exhaust.OnConduitStateChangedDelegate);
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

	private void CalculateDiseaseTransfer(PrimaryElement item1, PrimaryElement item2, float transfer_rate, out int disease_to_item1, out int disease_to_item2)
	{
		disease_to_item1 = (int)((float)item2.DiseaseCount * transfer_rate);
		disease_to_item2 = (int)((float)item1.DiseaseCount * transfer_rate);
	}

	public void Sim200ms(float dt)
	{
		this.operational.SetFlag(Exhaust.canExhaust, !this.vent.IsBlocked);
		if (!this.operational.IsOperational)
		{
			if (this.isAnimating)
			{
				this.isAnimating = false;
				this.recentlyExhausted = false;
				base.Trigger(-793429877, null);
			}
			return;
		}
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (!Grid.Solid[num] && this.consumer.ConsumptionRate != 0f)
		{
			List<GameObject> items = this.storage.items;
			if (items.Count > 0)
			{
				ConduitType typeOfConduit = this.consumer.TypeOfConduit;
				if (typeOfConduit != ConduitType.Liquid)
				{
					if (typeOfConduit == ConduitType.Gas)
					{
						for (int i = 0; i < items.Count; i++)
						{
							PrimaryElement component = items[i].GetComponent<PrimaryElement>();
							if (component.Mass > 0f && component.Element.IsGas)
							{
								int num2;
								int num3;
								this.CalculateDiseaseTransfer(this.exhaustPE, component, 0.05f, out num2, out num3);
								component.ModifyDiseaseCount(-num2, "Exhaust transfer");
								component.AddDisease(this.exhaustPE.DiseaseIdx, num3, "Exhaust transfer");
								this.exhaustPE.ModifyDiseaseCount(-num3, "Exhaust transfer");
								this.exhaustPE.AddDisease(component.DiseaseIdx, num2, "Exhaust transfer");
								SimMessages.AddRemoveSubstance(num, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, true, -1);
								component.KeepZeroMassObject = true;
								component.Mass = 0f;
								component.ModifyDiseaseCount(int.MinValue, "Exhaust.SimUpdate");
								this.recentlyExhausted = true;
								break;
							}
						}
					}
				}
				else
				{
					int num4 = Grid.CellBelow(num);
					bool flag = Grid.IsValidCell(num4) && !Grid.Solid[num4];
					for (int j = 0; j < items.Count; j++)
					{
						PrimaryElement component2 = items[j].GetComponent<PrimaryElement>();
						if (component2.Mass > 0f && component2.Element.IsLiquid)
						{
							int num5;
							int num6;
							this.CalculateDiseaseTransfer(this.exhaustPE, component2, 0.05f, out num5, out num6);
							component2.ModifyDiseaseCount(-num5, "Exhaust transfer");
							component2.AddDisease(this.exhaustPE.DiseaseIdx, num6, "Exhaust transfer");
							this.exhaustPE.ModifyDiseaseCount(-num6, "Exhaust transfer");
							this.exhaustPE.AddDisease(component2.DiseaseIdx, num5, "Exhaust transfer");
							if (flag)
							{
								byte b = (byte)ElementLoader.elements.IndexOf(component2.Element);
								FallingWater.instance.AddParticle(num, b, component2.Mass, component2.Temperature, component2.DiseaseIdx, component2.DiseaseCount, true, false, true, false);
							}
							else
							{
								SimMessages.AddRemoveSubstance(num, component2.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component2.Mass, component2.Temperature, component2.DiseaseIdx, component2.DiseaseCount, true, -1);
							}
							component2.KeepZeroMassObject = true;
							component2.Mass = 0f;
							component2.ModifyDiseaseCount(int.MinValue, "Exhaust.SimUpdate");
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
				base.Trigger(-793429877, null);
			}
			this.recentlyExhausted = false;
		}
	}

	public bool IsAnimating()
	{
		return this.isAnimating;
	}

	[MyCmpGet]
	private Vent vent;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private ConduitConsumer consumer;

	[MyCmpGet]
	private PrimaryElement exhaustPE;

	private static readonly Operational.Flag canExhaust = new Operational.Flag("canExhaust", Operational.Flag.Type.Requirement);

	private bool isAnimating;

	private bool recentlyExhausted;

	private const float MinSwitchTime = 1f;

	private float elapsedSwitchTime;

	private static readonly EventSystem.IntraObjectHandler<Exhaust> OnConduitStateChangedDelegate = new EventSystem.IntraObjectHandler<Exhaust>(delegate(Exhaust component, object data)
	{
		component.OnConduitStateChanged(data);
	});
}
