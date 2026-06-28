using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpaceHeater : OperationalBuilding, ISaveLoadableJson
{
	public float TargetTemperature
	{
		get
		{
			return this.targetTemperature;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
	}

	protected override void OperationalUpdate(float dt)
	{
		this.lastStateUpdate += dt;
		if (this.lastStateUpdate > this.stateUpdateInterval)
		{
			this.lastStateUpdate = 0f;
			this.monitorCells.Clear();
			int num = Grid.PosToCell(this.transform.position);
			GameUtil.GetEmptyCells(num, 1, this.monitorCells);
			int count = this.monitorCells.Count;
			float num2 = 0f;
			for (int i = 0; i < count; i++)
			{
				num2 += Grid.Temperature[this.monitorCells[i]];
			}
			bool flag = num2 / (float)count < this.targetTemperature + this.temperatureWindow;
			this.Operational.SetActive(flag, false);
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		float delta = 10f;
		this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("iconTemperatureUp", "Temperature +" + delta.ToString("0"), delegate
		{
			this.targetTemperature += delta;
		}, global::Action.Increment, null, null, null, null, string.Empty));
		this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("iconTemperatureDown", "Temperature -" + delta.ToString("0"), delegate
		{
			this.targetTemperature -= delta;
		}, global::Action.Decrement, null, null, null, null, string.Empty));
	}

	[Serialize]
	public float targetTemperature = 308.15f;

	[MyCmpAdd]
	private UserMenu userMenu;

	private List<int> monitorCells = new List<int>();

	private float lastStateUpdate;

	private float stateUpdateInterval = 10f;

	private float temperatureWindow = 10f;
}
