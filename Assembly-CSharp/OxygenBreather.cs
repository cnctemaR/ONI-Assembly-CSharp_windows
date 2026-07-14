using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[RequireComponent(typeof(Health))]
[AddComponentMenu("KMonoBehaviour/scripts/OxygenBreather")]
public class OxygenBreather : KMonoBehaviour, ISim200ms
{
	public KPrefabID prefabID { get; private set; }

	public float ConsumptionRate
	{
		get
		{
			if (this.airConsumptionRate != null)
			{
				return this.airConsumptionRate.GetTotalValue();
			}
			return 0f;
		}
	}

	public float CO2EmitRate
	{
		get
		{
			return Game.Instance.accumulators.GetAverageRate(this.co2Accumulator);
		}
	}

	public HandleVector<int>.Handle O2Accumulator
	{
		get
		{
			return this.o2Accumulator;
		}
	}

	public OxygenBreather.IGasProvider GetCurrentGasProvider()
	{
		if (this.gasProviders.Count == 0)
		{
			return null;
		}
		OxygenBreather.IGasProvider gasProvider = null;
		for (int i = this.gasProviders.Count - 1; i >= 0; i--)
		{
			OxygenBreather.IGasProvider gasProvider2 = this.gasProviders[i];
			if (!gasProvider2.IsBlocked())
			{
				gasProvider = gasProvider2;
				if (gasProvider2.HasOxygen())
				{
					break;
				}
			}
		}
		return gasProvider;
	}

	public bool IsLowOxygen()
	{
		OxygenBreather.IGasProvider currentGasProvider = this.GetCurrentGasProvider();
		return currentGasProvider == null || currentGasProvider.IsLowOxygen();
	}

	public bool HasOxygen
	{
		get
		{
			return this.hasAir;
		}
	}

	public bool IsOutOfOxygen
	{
		get
		{
			return !this.hasAir;
		}
	}

	protected override void OnPrefabInit()
	{
		GameUtil.SubscribeToTags<OxygenBreather>(this, OxygenBreather.OnDeadTagAddedDelegate, true);
		this.prefabID = base.GetComponent<KPrefabID>();
	}

	protected override void OnSpawn()
	{
		this.airConsumptionRate = Db.Get().Attributes.AirConsumptionRate.Lookup(this);
		this.o2Accumulator = Game.Instance.accumulators.Add("O2", this);
		this.co2Accumulator = Game.Instance.accumulators.Add("CO2", this);
		bool flag = base.gameObject.PrefabID() == BionicMinionConfig.ID;
		this.o2StatusItem = this.selectable.AddStatusItem(flag ? Db.Get().DuplicantStatusItems.BreathingO2Bionic : Db.Get().DuplicantStatusItems.BreathingO2, this);
		this.cO2StatusItem = this.selectable.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, this);
		this.temperature = Db.Get().Amounts.Temperature.Lookup(this);
		NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this, false);
	}

	private void BreathableGasConsumed(SimHashes elementConsumed, float massConsumed, float temperature, byte disseaseIDX, int disseaseCount)
	{
		if (this.prefabID.HasTag(GameTags.Dead) || this.O2Accumulator == HandleVector<int>.Handle.InvalidHandle)
		{
			return;
		}
		if (elementConsumed == SimHashes.ContaminatedOxygen)
		{
			base.BoxingTrigger<float>(-935848905, massConsumed);
		}
		Game.Instance.accumulators.Accumulate(this.O2Accumulator, massConsumed);
		float num = -massConsumed;
		ReportManager.Instance.ReportValueWithPrefabInstanceContext(ReportManager.ReportType.OxygenCreated, num, this.prefabID, this.selectable.GetProperName());
		if (this.onBreathableGasConsumed != null)
		{
			this.onBreathableGasConsumed(elementConsumed, massConsumed, temperature, disseaseIDX, disseaseCount);
		}
	}

	public static void BreathableGasConsumed(OxygenBreather breather, SimHashes elementConsumed, float massConsumed, float temperature, byte disseaseIDX, int disseaseCount)
	{
		if (breather != null)
		{
			breather.BreathableGasConsumed(elementConsumed, massConsumed, temperature, disseaseIDX, disseaseCount);
		}
	}

	public void Sim200ms(float dt)
	{
		if (this.prefabID.HasAnyTags(OxygenBreather.cannotBreathTags))
		{
			return;
		}
		float num = this.airConsumptionRate.GetTotalValue() * dt;
		OxygenBreather.IGasProvider currentGasProvider = this.GetCurrentGasProvider();
		bool flag = currentGasProvider != null && currentGasProvider.ConsumeGas(this, num);
		if (flag)
		{
			if (currentGasProvider.ShouldEmitCO2())
			{
				if (this.cO2StatusItem != Guid.Empty)
				{
					this.cO2StatusItem = this.selectable.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, this);
				}
				float num2 = num * this.O2toCO2conversion;
				Game.Instance.accumulators.Accumulate(this.co2Accumulator, num2);
				this.accumulatedCO2 += num2;
				if (this.accumulatedCO2 >= this.minCO2ToEmit)
				{
					this.accumulatedCO2 -= this.minCO2ToEmit;
					Vector3 position = base.transform.GetPosition();
					Vector3 vector = position;
					vector.x += (this.facing.GetFacing() ? (-this.mouthOffset.x) : this.mouthOffset.x);
					vector.y += this.mouthOffset.y;
					vector.z -= 0.5f;
					if (Mathf.FloorToInt(vector.x) != Mathf.FloorToInt(position.x))
					{
						vector.x = Mathf.Floor(position.x) + (this.facing.GetFacing() ? 0.01f : 0.99f);
					}
					CO2Manager.instance.SpawnBreath(vector, this.minCO2ToEmit, this.temperature.value, this.facing.GetFacing());
				}
			}
			else if (currentGasProvider.ShouldStoreCO2())
			{
				if (this.cO2StatusItem != Guid.Empty)
				{
					this.cO2StatusItem = this.selectable.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, this);
				}
				Equippable equippable = base.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
				if (equippable != null)
				{
					float num3 = num * this.O2toCO2conversion;
					Game.Instance.accumulators.Accumulate(this.co2Accumulator, num3);
					this.accumulatedCO2 += num3;
					if (this.accumulatedCO2 >= this.minCO2ToEmit)
					{
						this.accumulatedCO2 -= this.minCO2ToEmit;
						equippable.GetComponent<Storage>().AddGasChunk(SimHashes.CarbonDioxide, this.minCO2ToEmit, this.temperature.value, byte.MaxValue, 0, false, true);
					}
				}
			}
			else if (this.cO2StatusItem != Guid.Empty)
			{
				this.selectable.RemoveStatusItem(this.cO2StatusItem, false);
				this.cO2StatusItem = Guid.Empty;
			}
		}
		if (flag != this.hasAir)
		{
			this.hasAirTimer.Start();
			if (this.hasAirTimer.TryStop(2f))
			{
				this.hasAir = flag;
				base.Trigger(-933153513, BoxedBools.Box(this.hasAir));
				return;
			}
		}
		else
		{
			this.hasAirTimer.Stop();
		}
	}

	public void AddGasProvider(OxygenBreather.IGasProvider gas_provider)
	{
		global::Debug.Assert(gas_provider != null, "Error at OxygenBreather.cs  adding gas provider, the gas provider param is null!");
		global::Debug.Assert(!this.gasProviders.Contains(gas_provider), "Error at OxygenBreather.cs adding gas provider, the gas provider was already added to the gas providers list!");
		this.gasProviders.Add(gas_provider);
		gas_provider.OnSetOxygenBreather(this);
	}

	public bool RemoveGasProvider(OxygenBreather.IGasProvider provider)
	{
		if (this.gasProviders.Count > 0 && this.gasProviders.Contains(provider))
		{
			OxygenBreather.IGasProvider gasProvider = this.gasProviders[this.gasProviders.Count - 1];
			this.gasProviders.Remove(provider);
			provider.OnClearOxygenBreather(this);
			return true;
		}
		return false;
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
		this.selectable.RemoveStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, false);
		this.selectable.RemoveStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, false);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(this.o2Accumulator);
		Game.Instance.accumulators.Remove(this.co2Accumulator);
		this.o2Accumulator = HandleVector<int>.InvalidHandle;
		this.co2Accumulator = HandleVector<int>.InvalidHandle;
		while (this.gasProviders.Count > 0)
		{
			OxygenBreather.IGasProvider gasProvider = this.gasProviders[this.gasProviders.Count - 1];
			this.RemoveGasProvider(gasProvider);
		}
		base.OnCleanUp();
	}

	private static Tag[] cannotBreathTags = new Tag[]
	{
		GameTags.Dead,
		GameTags.SuffocatingIncapacitated
	};

	public float O2toCO2conversion = 0.5f;

	public Vector2 mouthOffset;

	[Serialize]
	public float accumulatedCO2;

	[SerializeField]
	public float minCO2ToEmit = 0.3f;

	private bool hasAir = true;

	private Timer hasAirTimer = new Timer();

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpGet]
	private Facing facing;

	[MyCmpGet]
	private KSelectable selectable;

	private HandleVector<int>.Handle o2Accumulator = HandleVector<int>.InvalidHandle;

	private HandleVector<int>.Handle co2Accumulator = HandleVector<int>.InvalidHandle;

	private AmountInstance temperature;

	public float lowOxygenThreshold;

	public float noOxygenThreshold;

	private AttributeInstance airConsumptionRate;

	public Action<SimHashes, float, float, byte, int> onBreathableGasConsumed;

	private static readonly EventSystem.IntraObjectHandler<OxygenBreather> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler<OxygenBreather>(GameTags.Dead, delegate(OxygenBreather component, object data)
	{
		component.OnDeath(data);
	});

	private List<OxygenBreather.IGasProvider> gasProviders = new List<OxygenBreather.IGasProvider>();

	private Guid o2StatusItem;

	private Guid cO2StatusItem;

	public interface IGasProvider
	{
		void OnSetOxygenBreather(OxygenBreather oxygen_breather);

		void OnClearOxygenBreather(OxygenBreather oxygen_breather);

		bool ConsumeGas(OxygenBreather oxygen_breather, float amount);

		bool ShouldEmitCO2();

		bool ShouldStoreCO2();

		bool IsLowOxygen();

		bool HasOxygen();

		bool IsBlocked();
	}
}
