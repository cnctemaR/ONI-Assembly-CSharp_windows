using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[RequireComponent(typeof(Health))]
[AddComponentMenu("KMonoBehaviour/scripts/OxygenBreather")]
public class OxygenBreather : KMonoBehaviour, ISim200ms
{
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

	protected override void OnPrefabInit()
	{
		GameUtil.SubscribeToTags<OxygenBreather>(this, OxygenBreather.OnDeadTagChangedDelegate);
	}

	public bool IsLowOxygen()
	{
		return this.GetOxygenPressure(this.mouthCell) < this.lowOxygenThreshold;
	}

	protected override void OnSpawn()
	{
		this.airConsumptionRate = Db.Get().Attributes.AirConsumptionRate.Lookup(this);
		this.o2Accumulator = Game.Instance.accumulators.Add("O2", this);
		this.co2Accumulator = Game.Instance.accumulators.Add("CO2", this);
		KSelectable component = base.GetComponent<KSelectable>();
		component.AddStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, this);
		component.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, this);
		this.temperature = Db.Get().Amounts.Temperature.Lookup(this);
		NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this, false);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(this.o2Accumulator);
		Game.Instance.accumulators.Remove(this.co2Accumulator);
		this.SetGasProvider(null);
		base.OnCleanUp();
	}

	public void Consume(Sim.MassConsumedCallback mass_consumed)
	{
		if (this.onSimConsume != null)
		{
			this.onSimConsume(mass_consumed);
		}
	}

	public void Sim200ms(float dt)
	{
		if (!base.gameObject.HasTag(GameTags.Dead))
		{
			float num = this.airConsumptionRate.GetTotalValue() * dt;
			bool flag = this.gasProvider.ConsumeGas(this, num);
			if (flag)
			{
				if (this.gasProvider.ShouldEmitCO2())
				{
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
				else if (this.gasProvider.ShouldStoreCO2())
				{
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
			}
			if (flag != this.hasAir)
			{
				this.hasAirTimer.Start();
				if (this.hasAirTimer.TryStop(2f))
				{
					this.hasAir = flag;
					return;
				}
			}
			else
			{
				this.hasAirTimer.Stop();
			}
		}
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
		KSelectable component = base.GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, false);
		component.RemoveStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, false);
	}

	private int GetMouthCellAtCell(int cell, CellOffset[] offsets)
	{
		float num = 0f;
		int num2 = cell;
		foreach (CellOffset cellOffset in offsets)
		{
			int num3 = Grid.OffsetCell(cell, cellOffset);
			float oxygenPressure = this.GetOxygenPressure(num3);
			if (oxygenPressure > num && oxygenPressure > this.noOxygenThreshold)
			{
				num = oxygenPressure;
				num2 = num3;
			}
		}
		return num2;
	}

	public int mouthCell
	{
		get
		{
			int num = Grid.PosToCell(this);
			return this.GetMouthCellAtCell(num, this.breathableCells);
		}
	}

	public bool IsBreathableElementAtCell(int cell, CellOffset[] offsets = null)
	{
		return this.GetBreathableElementAtCell(cell, offsets) != SimHashes.Vacuum;
	}

	public SimHashes GetBreathableElementAtCell(int cell, CellOffset[] offsets = null)
	{
		if (offsets == null)
		{
			offsets = this.breathableCells;
		}
		int mouthCellAtCell = this.GetMouthCellAtCell(cell, offsets);
		if (!Grid.IsValidCell(mouthCellAtCell))
		{
			return SimHashes.Vacuum;
		}
		Element element = Grid.Element[mouthCellAtCell];
		if (!element.IsGas || !element.HasTag(GameTags.Breathable) || Grid.Mass[mouthCellAtCell] <= this.noOxygenThreshold)
		{
			return SimHashes.Vacuum;
		}
		return element.id;
	}

	public bool IsUnderLiquid
	{
		get
		{
			return Grid.Element[this.mouthCell].IsLiquid;
		}
	}

	public bool IsSuffocating
	{
		get
		{
			return !this.hasAir;
		}
	}

	public SimHashes GetBreathableElement
	{
		get
		{
			return this.GetBreathableElementAtCell(Grid.PosToCell(this), null);
		}
	}

	public bool IsBreathableElement
	{
		get
		{
			return this.IsBreathableElementAtCell(Grid.PosToCell(this), null);
		}
	}

	private float GetOxygenPressure(int cell)
	{
		if (Grid.IsValidCell(cell) && Grid.Element[cell].HasTag(GameTags.Breathable))
		{
			return Grid.Mass[cell];
		}
		return 0f;
	}

	public OxygenBreather.IGasProvider GetGasProvider()
	{
		return this.gasProvider;
	}

	public void SetGasProvider(OxygenBreather.IGasProvider gas_provider)
	{
		if (this.gasProvider != null)
		{
			this.gasProvider.OnClearOxygenBreather(this);
		}
		this.gasProvider = gas_provider;
		if (this.gasProvider != null)
		{
			this.gasProvider.OnSetOxygenBreather(this);
		}
	}

	public static CellOffset[] DEFAULT_BREATHABLE_OFFSETS = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(0, 1),
		new CellOffset(1, 1),
		new CellOffset(-1, 1),
		new CellOffset(1, 0),
		new CellOffset(-1, 0)
	};

	public float O2toCO2conversion = 0.5f;

	public float lowOxygenThreshold;

	public float noOxygenThreshold;

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

	private HandleVector<int>.Handle o2Accumulator = HandleVector<int>.InvalidHandle;

	private HandleVector<int>.Handle co2Accumulator = HandleVector<int>.InvalidHandle;

	private AmountInstance temperature;

	private AttributeInstance airConsumptionRate;

	public CellOffset[] breathableCells;

	public Action<Sim.MassConsumedCallback> onSimConsume;

	private OxygenBreather.IGasProvider gasProvider;

	private static readonly EventSystem.IntraObjectHandler<OxygenBreather> OnDeadTagChangedDelegate = GameUtil.CreateHasTagHandler<OxygenBreather>(GameTags.Dead, delegate(OxygenBreather component, object data)
	{
		component.OnDeath(data);
	});

	public interface IGasProvider
	{
		void OnSetOxygenBreather(OxygenBreather oxygen_breather);

		void OnClearOxygenBreather(OxygenBreather oxygen_breather);

		bool ConsumeGas(OxygenBreather oxygen_breather, float amount);

		bool ShouldEmitCO2();

		bool ShouldStoreCO2();
	}
}
