using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class OxygenBreather : KMonoBehaviour
{
	public Accumulator o2Accumulator { get; private set; }

	public float O2ConsumptionRate
	{
		get
		{
			return this.o2Accumulator.AvgRate;
		}
	}

	public float CO2EmitRate
	{
		get
		{
			return this.CO2Accumulator.AvgRate;
		}
	}

	protected override void OnPrefabInit()
	{
		base.Subscribe(1623392196, new Action<object>(this.OnDeath));
		base.Subscribe(-1117766961, new Action<object>(this.OnRevived));
	}

	public bool IsLowOxygen()
	{
		return this.GetOxygenPressure(this.mouthCell) < this.lowOxygenThreshold;
	}

	protected override void OnSpawn()
	{
		this.airConsumptionRate = Db.Get().Attributes.AirConsumptionRate.Lookup(this);
		this.o2Accumulator = new Accumulator("O2", this, 3f);
		this.CO2Accumulator = new Accumulator("CO2", this, 3f);
		KSelectable component = base.GetComponent<KSelectable>();
		component.AddStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, this);
		component.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, this);
		this.temperature = Db.Get().Amounts.Temperature.Lookup(this);
		NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this);
	}

	private void SimUpdate(float dt)
	{
		if (!base.gameObject.HasTag(GameTags.Dead))
		{
			float num = this.airConsumptionRate.GetTotalValue() * dt;
			bool flag = this.gasProvider.ConsumeGas(this, num);
			if (flag && this.gasProvider.ShouldEmitCO2())
			{
				float num2 = num * this.O2toCO2conversion;
				this.CO2Accumulator.Accumulate(num2);
				this.accumulatedCO2 += num2;
				if (this.accumulatedCO2 >= this.minCO2ToEmit)
				{
					this.accumulatedCO2 -= this.minCO2ToEmit;
					Vector3 position = base.transform.position;
					position.x += ((!this.facing.GetFacing()) ? this.mouthOffset.x : (-this.mouthOffset.x));
					position.y += this.mouthOffset.y;
					position.z -= 0.5f;
					CO2Manager.instance.SpawnBreath(position, this.minCO2ToEmit, this.temperature.value);
				}
			}
			if (flag != this.hasAir)
			{
				this.hasAirTimer.Start();
				if (this.hasAirTimer.TryStop(2f))
				{
					this.hasAir = flag;
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

	private void OnRevived(object data)
	{
		base.enabled = true;
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
		bool flag = element.IsGas && element.HasTag(GameTags.Breathable) && Grid.Cell[mouthCellAtCell].mass > this.noOxygenThreshold;
		return (!flag) ? SimHashes.Vacuum : element.id;
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
		if (Grid.IsValidCell(cell))
		{
			Element element = Grid.Element[cell];
			if (element.HasTag(GameTags.Breathable))
			{
				return Grid.Cell[cell].mass;
			}
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
		this.gasProvider.OnSetOxygenBreather(this);
	}

	public float O2toCO2conversion = 0.5f;

	public float lowOxygenThreshold;

	public float noOxygenThreshold;

	public Vector2 mouthOffset;

	[Serialize]
	public float accumulatedCO2;

	[SerializeField]
	private float minCO2ToEmit = 0.3f;

	private bool hasAir = true;

	private Timer hasAirTimer = new Timer();

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpGet]
	private Facing facing;

	private Accumulator CO2Accumulator;

	private AmountInstance temperature;

	private AttributeInstance airConsumptionRate;

	public CellOffset[] breathableCells;

	private OxygenBreather.IGasProvider gasProvider;

	public interface IGasProvider
	{
		void OnSetOxygenBreather(OxygenBreather oxygen_breather);

		void OnClearOxygenBreather(OxygenBreather oxygen_breather);

		bool ConsumeGas(OxygenBreather oxygen_breather, float amount);

		bool ShouldEmitCO2();
	}
}
