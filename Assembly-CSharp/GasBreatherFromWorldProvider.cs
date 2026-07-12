using System;

public class GasBreatherFromWorldProvider : OxygenBreather.IGasProvider
{
	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
		this.suffocationMonitor = new SuffocationMonitor.Instance(oxygen_breather);
		this.suffocationMonitor.StartSM();
		this.safeCellMonitor = new SafeCellMonitor.Instance(oxygen_breather);
		this.safeCellMonitor.StartSM();
		this.oxygenBreather = oxygen_breather;
		this.nav = this.oxygenBreather.GetComponent<Navigator>();
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
		this.suffocationMonitor.StopSM("Removed gas provider");
		this.safeCellMonitor.StopSM("Removed gas provider");
	}

	public bool ShouldEmitCO2()
	{
		return this.nav.CurrentNavType != NavType.Tube;
	}

	public bool ShouldStoreCO2()
	{
		return false;
	}

	public bool IsLowOxygen()
	{
		return this.oxygenBreather.IsLowOxygenAtMouthCell();
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float gas_consumed)
	{
		if (this.nav.CurrentNavType != NavType.Tube)
		{
			SimHashes getBreathableElement = oxygen_breather.GetBreathableElement;
			if (getBreathableElement == SimHashes.Vacuum)
			{
				return false;
			}
			HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(new Action<Sim.MassConsumedCallback, object>(GasBreatherFromWorldProvider.OnSimConsumeCallback), this, "GasBreatherFromWorldProvider");
			SimMessages.ConsumeMass(oxygen_breather.mouthCell, getBreathableElement, gas_consumed, 3, handle.index);
		}
		return true;
	}

	private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		((GasBreatherFromWorldProvider)data).OnSimConsume(mass_cb_info);
	}

	private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
	{
		if (this.oxygenBreather == null || this.oxygenBreather.GetComponent<KPrefabID>().HasTag(GameTags.Dead))
		{
			return;
		}
		if (ElementLoader.elements[(int)mass_cb_info.elemIdx].id == SimHashes.ContaminatedOxygen)
		{
			this.oxygenBreather.Trigger(-935848905, mass_cb_info);
		}
		Game.Instance.accumulators.Accumulate(this.oxygenBreather.O2Accumulator, mass_cb_info.mass);
		float num = -mass_cb_info.mass;
		ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num, this.oxygenBreather.GetProperName(), null);
		this.oxygenBreather.Consume(mass_cb_info);
	}

	private SuffocationMonitor.Instance suffocationMonitor;

	private SafeCellMonitor.Instance safeCellMonitor;

	private OxygenBreather oxygenBreather;

	private Navigator nav;
}
