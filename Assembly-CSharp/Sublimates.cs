using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Sublimates : KMonoBehaviour
{
	public float Temperature
	{
		get
		{
			return this.primaryElement.Temperature;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.flowAccumulator = new Accumulator("EmittedMass", this, 3f);
		this.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
		this.Subscribe(1335436905, new Action<object>(this.OnSplitFromChunk));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.info.sublimatedElement == SimHashes.Oxygen)
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingOxygenAvg, this);
		}
		else
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingGasAvg, this);
		}
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable != null)
		{
			Sublimates component = pickupable.GetComponent<Sublimates>();
			if (component != null)
			{
				this.sublimatedMass += component.sublimatedMass;
			}
		}
	}

	private void OnSplitFromChunk(object data)
	{
		Pickupable pickupable = data as Pickupable;
		PrimaryElement component = pickupable.GetComponent<PrimaryElement>();
		Sublimates component2 = pickupable.GetComponent<Sublimates>();
		float mass = this.primaryElement.Mass;
		float mass2 = component.Mass;
		float num = mass / (mass2 + mass);
		this.sublimatedMass = component2.sublimatedMass * num;
		float num2 = 1f - num;
		component2.sublimatedMass *= num2;
	}

	private void SimUpdate(float dt)
	{
		Pickupable component = base.GetComponent<Pickupable>();
		if (component != null && component.storage != null && !component.storage.allowSublimation)
		{
			return;
		}
		int num = Grid.PosToCell(this.transform.position);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		float mass = Grid.Cell[num].mass;
		if (mass < this.info.maxDestinationMass)
		{
			float num2 = this.primaryElement.Mass;
			if (num2 > 0f)
			{
				float num3 = Mathf.Pow(num2, this.info.massPower);
				float num4 = Mathf.Max(this.info.sublimationRate, this.info.sublimationRate * num3);
				num4 *= dt;
				num4 = Mathf.Min(num4, num2);
				this.sublimatedMass += num4;
				num2 -= num4;
				if (this.sublimatedMass > this.info.minSublimationAmount)
				{
					PrimaryElement component2 = base.GetComponent<PrimaryElement>();
					if (component2 != null)
					{
						component2.Mass = Mathf.Max(0f, component2.Mass - this.sublimatedMass);
					}
					this.Emit(num, this.sublimatedMass, this.primaryElement.Temperature);
					this.sublimatedMass = 0f;
				}
			}
			else
			{
				if (this.sublimatedMass > 0f)
				{
					this.Emit(num, this.sublimatedMass, this.primaryElement.Temperature);
				}
				Util.KDestroyGameObject(base.gameObject);
			}
		}
	}

	private void Emit(int cell, float mass, float temperature)
	{
		SimMessages.AddRemoveSubstance(cell, this.info.sublimatedElement, CellEventLogger.Instance.SublimatesEmit, mass, temperature, -1);
		this.flowAccumulator.Accumulate(mass);
		if (this.info.sublimatedElement == SimHashes.ContaminatedOxygen && BaseArea.Instance.IsInsideBase(cell))
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.ContaminatedOxygenSublimation, mass, null);
		}
		if (this.spawnFXHash != SpawnFXHashes.None)
		{
			this.transform.position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
			Game.Instance.SpawnFX(this.spawnFXHash, this.transform.position, 0f);
		}
	}

	public float AvgFlowRate()
	{
		return this.flowAccumulator.AvgRate;
	}

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private KSelectable selectable;

	[SerializeField]
	public SpawnFXHashes spawnFXHash;

	[SerializeField]
	public Sublimates.Info info;

	[Serialize]
	private float sublimatedMass;

	private Accumulator flowAccumulator;

	[Serializable]
	public struct Info
	{
		public Info(float rate, float min_amount, float max_destination_mass, float mass_power, SimHashes element)
		{
			this.sublimationRate = rate;
			this.minSublimationAmount = min_amount;
			this.maxDestinationMass = max_destination_mass;
			this.massPower = mass_power;
			this.sublimatedElement = element;
		}

		public float sublimationRate;

		public float minSublimationAmount;

		public float maxDestinationMass;

		public float massPower;

		[HashedEnum]
		public SimHashes sublimatedElement;
	}
}
