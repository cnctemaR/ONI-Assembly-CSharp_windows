using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[RequireComponent(typeof(Health))]
public class OxygenBreather : KMonoBehaviour
{
	public SuitTank SuitTank
	{
		get
		{
			return this.suitTank;
		}
	}

	public float TargetAirConsumptionRate
	{
		get
		{
			return this.airConsumptionRate.GetTotalValue();
		}
	}

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
		this.Subscribe(1623392196, new Action<object>(this.OnDeath));
		this.Subscribe(-1117766961, new Action<object>(this.OnRevived));
		this.Subscribe(-1195989806, new Action<object>(this.OnEquippedItem));
		this.Subscribe(-272419061, new Action<object>(this.OnUnequippedItem));
	}

	public bool isLowOxygen()
	{
		return this.GetOxygenPressure(this.mouthCell) < this.lowOxygenThreshold;
	}

	public bool isNoOxygen()
	{
		return this.GetOxygenPressure(this.mouthCell) < this.noOxygenThreshold;
	}

	protected override void OnSpawn()
	{
		this.airConsumptionRate = Db.Get().Attributes.AirConsumptionRate.Lookup(this);
		this.o2Accumulator = new Accumulator("O2", this, 3f);
		this.CO2Accumulator = new Accumulator("CO2", this, 3f);
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, this);
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, this);
		this.temperature = Db.Get().Amounts.Temperature.Lookup(this);
	}

	private void OnSimConsume(object obj)
	{
		if (!typeof(Sim.MassConsumptionCallback).IsAssignableFrom(obj.GetType()))
		{
			global::Debug.LogError("Error converting obj to MassConsumptionCallback: " + obj.GetType(), null);
		}
		Sim.MassConsumptionCallback massConsumptionCallback = (Sim.MassConsumptionCallback)obj;
		this.o2Accumulator.Accumulate(massConsumptionCallback.mass);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, -massConsumptionCallback.mass, null);
		this.Trigger(240573938, massConsumptionCallback);
	}

	private void SimUpdate(float dt)
	{
		if (!base.gameObject.HasTag(GameTags.Dead))
		{
			float num = this.airConsumptionRate.GetTotalValue() * dt;
			bool isUsingOxygenTank = this.IsUsingOxygenTank;
			SimHashes getBreathableElement = this.GetBreathableElement;
			bool flag;
			if (!isUsingOxygenTank && getBreathableElement != SimHashes.Vacuum)
			{
				HandleVector<Game.ComplexCallbackInfo>.Handle handle = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(new Action<object>(this.OnSimConsume)));
				SimMessages.ConsumeMass(this.mouthCell, getBreathableElement, num, 3, handle.index);
				float num2 = num * this.O2toCO2conversion;
				this.CO2Accumulator.Accumulate(num2);
				this.accumulatedCO2 += num2;
				if (this.accumulatedCO2 >= this.minCO2ToEmit)
				{
					this.accumulatedCO2 -= this.minCO2ToEmit;
					Vector3 position = this.transform.position;
					position.x += ((!this.facing.GetFacing()) ? this.mouthOffset.x : (-this.mouthOffset.x));
					position.y += this.mouthOffset.y;
					position.z -= 0.5f;
					CO2Manager.instance.SpawnBreath(position, this.minCO2ToEmit, this.temperature.value);
				}
				flag = true;
				if (this.isLowOxygen())
				{
					base.gameObject.GetComponent<Effects>().Add("LowOxygen", false);
				}
				else
				{
					base.gameObject.GetComponent<Effects>().Remove("LowOxygen");
				}
			}
			else if (isUsingOxygenTank)
			{
				this.suitTank.amount -= num;
				base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().DuplicantStatusItems.LowOxygen, this.suitTank.IsLow(), this);
				flag = true;
				ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, -num, null);
			}
			else
			{
				base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.LowOxygen, false);
				flag = false;
			}
			if (flag != this.hasAir && !isUsingOxygenTank)
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
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, false);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, false);
	}

	private void OnRevived(object data)
	{
		base.enabled = true;
	}

	private void OnEquippedItem(object data)
	{
		KPrefabID kprefabID = (KPrefabID)data;
		if (kprefabID != null)
		{
			foreach (Tag tag in kprefabID.Tags)
			{
				if (GameTags.OxygenSuitTaags.Contains(tag))
				{
					this.suitTank = kprefabID.GetComponent<SuitTank>();
					if (NameDisplayScreen.Instance != null)
					{
						NameDisplayScreen.Instance.SetSuitTankDisplayState(true, base.gameObject);
					}
					break;
				}
			}
		}
		base.GetComponent<Sensors>().UpdateSensors();
	}

	private void OnUnequippedItem(object data)
	{
		KPrefabID kprefabID = (KPrefabID)data;
		if (kprefabID != null && GameTags.AllSuitTags.Contains(kprefabID.PrefabTag))
		{
			foreach (Tag tag in kprefabID.Tags)
			{
				if (GameTags.AllSuitTags.Contains(tag))
				{
					if (this.suitTank != null && NameDisplayScreen.Instance != null)
					{
						NameDisplayScreen.Instance.SetSuitTankDisplayState(false, base.gameObject);
					}
					this.suitTank = null;
					break;
				}
			}
		}
		base.GetComponent<Sensors>().UpdateSensors();
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

	public bool IsUsingOxygenTank
	{
		get
		{
			return !this.IsBreathableElement && this.suitTank != null && !this.suitTank.IsEmpty();
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

	public float O2toCO2conversion = 0.5f;

	public float lowOxygenThreshold;

	public float noOxygenThreshold;

	public Vector2 mouthOffset;

	[Serialize]
	private float accumulatedCO2;

	[SerializeField]
	private float minCO2ToEmit = 0.3f;

	private bool hasAir = true;

	private Timer hasAirTimer = new Timer();

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpGet]
	private Facing facing;

	private Accumulator o2Accumulator;

	private Accumulator CO2Accumulator;

	private AmountInstance temperature;

	private SuitTank suitTank;

	private AttributeInstance airConsumptionRate;

	public CellOffset[] breathableCells;
}
