using System;
using System.Collections;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicSwitch : Switch
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateVisualization();
		this.UpdateLogicCircuit();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	protected override void Toggle()
	{
		base.Toggle();
		this.UpdateVisualization();
		this.UpdateLogicCircuit();
	}

	private void UpdateVisualization()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.Play(this.switchedOn ? "on_pre" : "on_pst", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue(this.switchedOn ? "on" : "off", KAnim.PlayMode.Once, 1f, 0f);
	}

	private void UpdateLogicCircuit()
	{
		base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem statusItem = (this.switchedOn ? Db.Get().BuildingStatusItems.LogicSwitchStatusActive : Db.Get().BuildingStatusItems.LogicSwitchStatusInactive);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, statusItem, null);
	}

	public void SetFirstFrameCallback(global::System.Action ffCb)
	{
		this.firstFrameCallback = ffCb;
		base.StartCoroutine(this.RunCallback());
	}

	private IEnumerator RunCallback()
	{
		yield return null;
		if (this.firstFrameCallback != null)
		{
			this.firstFrameCallback();
			this.firstFrameCallback = null;
		}
		yield return null;
		yield break;
	}

	public static readonly HashedString PORT_ID = "LogicSwitch";

	private global::System.Action firstFrameCallback;
}
