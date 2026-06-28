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
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
		this.suffocationMonitor.StopSM("Removed gas provider");
		this.safeCellMonitor.StopSM("Removed gas provider");
	}

	public bool ShouldEmitCO2()
	{
		return true;
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float gas_consumed)
	{
		SimHashes getBreathableElement = oxygen_breather.GetBreathableElement;
		bool flag;
		if (getBreathableElement == SimHashes.Vacuum)
		{
			flag = false;
		}
		else
		{
			HandleVector<Game.ComplexCallbackInfo>.Handle handle = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(new Action<object>(this.OnSimConsume)));
			SimMessages.ConsumeMass(oxygen_breather.mouthCell, getBreathableElement, gas_consumed, 3, handle.index);
			flag = true;
		}
		return flag;
	}

	private void OnSimConsume(object obj)
	{
		if (!(this.oxygenBreather == null))
		{
			Sim.MassConsumptionCallback massConsumptionCallback = (Sim.MassConsumptionCallback)obj;
			this.oxygenBreather.o2Accumulator.Accumulate(massConsumptionCallback.mass);
			ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, -massConsumptionCallback.mass, this.oxygenBreather.GetProperName(), null);
			this.oxygenBreather.Trigger(240573938, massConsumptionCallback);
		}
	}

	private SuffocationMonitor.Instance suffocationMonitor;

	private SafeCellMonitor.Instance safeCellMonitor;

	private OxygenBreather oxygenBreather;
}
