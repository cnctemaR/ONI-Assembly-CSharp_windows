using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicElementSensor : Switch, ISaveLoadable, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Filterable component = base.GetComponent<Filterable>();
		component.onFilterChanged += this.OnElementSelected;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += this.OnSwitchToggled;
		this.UpdateLogicCircuit();
		this.UpdateVisualState(true);
		this.wasOn = this.switchedOn;
		base.Subscribe<LogicElementSensor>(-592767678, LogicElementSensor.OnOperationalChangedDelegate);
	}

	public void Sim200ms(float dt)
	{
		int num = Grid.PosToCell(this);
		if (this.sampleIdx < 8)
		{
			this.samples[this.sampleIdx] = Grid.ElementIdx[num] == this.desiredElementIdx;
			this.sampleIdx++;
			return;
		}
		this.sampleIdx = 0;
		bool flag = true;
		foreach (bool flag2 in this.samples)
		{
			flag = flag2 && flag;
		}
		if (base.IsSwitchedOn != flag)
		{
			this.Toggle();
		}
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		this.UpdateLogicCircuit();
		this.UpdateVisualState(false);
	}

	private void UpdateLogicCircuit()
	{
		bool flag = this.switchedOn && base.GetComponent<Operational>().IsOperational;
		base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!flag) ? 0 : 1);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			this.wasOn = this.switchedOn;
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.Play((!this.switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue((!this.switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	private void OnElementSelected(Tag element_tag)
	{
		if (!element_tag.IsValid)
		{
			return;
		}
		Element element = ElementLoader.GetElement(element_tag);
		bool flag = true;
		if (element != null)
		{
			this.desiredElementIdx = (byte)ElementLoader.GetElementIndex(element.id);
			flag = element.id == SimHashes.Void || element.id == SimHashes.Vacuum;
		}
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, flag, null);
	}

	private void OnOperationalChanged(object data)
	{
		this.UpdateLogicCircuit();
		this.UpdateVisualState(false);
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem statusItem = ((!this.switchedOn) ? Db.Get().BuildingStatusItems.LogicSensorStatusInactive : Db.Get().BuildingStatusItems.LogicSensorStatusActive);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, statusItem, null);
	}

	private bool wasOn;

	public Element.State desiredState = Element.State.Gas;

	private const int WINDOW_SIZE = 8;

	private bool[] samples = new bool[8];

	private int sampleIdx;

	private byte desiredElementIdx = byte.MaxValue;

	private static readonly EventSystem.IntraObjectHandler<LogicElementSensor> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<LogicElementSensor>(delegate(LogicElementSensor component, object data)
	{
		component.OnOperationalChanged(data);
	});
}
