using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Exhaust")]
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
		this.UpdateEmission();
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

	private void UpdateEmission()
	{
		if (this.consumer.ConsumptionRate == 0f)
		{
			return;
		}
		if (this.storage.items.Count == 0)
		{
			return;
		}
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (Grid.Solid[num])
		{
			return;
		}
		ConduitType typeOfConduit = this.consumer.TypeOfConduit;
		if (typeOfConduit != ConduitType.Gas)
		{
			if (typeOfConduit == ConduitType.Liquid)
			{
				this.EmitLiquid(num);
				return;
			}
		}
		else
		{
			this.EmitGas(num);
		}
	}

	private bool EmitCommon(int cell, PrimaryElement primary_element, Exhaust.EmitDelegate emit)
	{
		if (primary_element.Mass <= 0f)
		{
			return false;
		}
		int num;
		int num2;
		this.CalculateDiseaseTransfer(this.exhaustPE, primary_element, 0.05f, out num, out num2);
		primary_element.ModifyDiseaseCount(-num, "Exhaust transfer");
		primary_element.AddDisease(this.exhaustPE.DiseaseIdx, num2, "Exhaust transfer");
		this.exhaustPE.ModifyDiseaseCount(-num2, "Exhaust transfer");
		this.exhaustPE.AddDisease(primary_element.DiseaseIdx, num, "Exhaust transfer");
		emit(cell, primary_element);
		if (this.vent != null)
		{
			this.vent.UpdateVentedMass(primary_element.ElementID, primary_element.Mass);
		}
		primary_element.KeepZeroMassObject = true;
		primary_element.Mass = 0f;
		primary_element.ModifyDiseaseCount(int.MinValue, "Exhaust.SimUpdate");
		this.recentlyExhausted = true;
		return true;
	}

	private void EmitLiquid(int cell)
	{
		int num = Grid.CellBelow(cell);
		Exhaust.EmitDelegate emitDelegate = ((Grid.IsValidCell(num) && !Grid.Solid[num]) ? Exhaust.emit_particle : Exhaust.emit_element);
		foreach (GameObject gameObject in this.storage.items)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component.Element.IsLiquid && this.EmitCommon(cell, component, emitDelegate))
			{
				break;
			}
		}
	}

	private void EmitGas(int cell)
	{
		foreach (GameObject gameObject in this.storage.items)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component.Element.IsGas && this.EmitCommon(cell, component, Exhaust.emit_element))
			{
				break;
			}
		}
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

	private static Exhaust.EmitDelegate emit_element = delegate(int cell, PrimaryElement primary_element)
	{
		SimMessages.AddRemoveSubstance(cell, primary_element.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, primary_element.Mass, primary_element.Temperature, primary_element.DiseaseIdx, primary_element.DiseaseCount, true, -1);
	};

	private static Exhaust.EmitDelegate emit_particle = delegate(int cell, PrimaryElement primary_element)
	{
		FallingWater.instance.AddParticle(cell, (byte)ElementLoader.elements.IndexOf(primary_element.Element), primary_element.Mass, primary_element.Temperature, primary_element.DiseaseIdx, primary_element.DiseaseCount, true, false, true, false);
	};

	private delegate void EmitDelegate(int cell, PrimaryElement primary_element);
}
