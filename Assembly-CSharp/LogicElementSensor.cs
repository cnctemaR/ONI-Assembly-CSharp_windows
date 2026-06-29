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
		base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
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
		this.desiredElementIdx = ElementLoader.GetElementIndex(element_tag);
	}

	private bool wasOn;

	public Element.State desiredState = Element.State.Gas;

	private const int WINDOW_SIZE = 8;

	private bool[] samples = new bool[8];

	private int sampleIdx;

	private byte desiredElementIdx = byte.MaxValue;
}
