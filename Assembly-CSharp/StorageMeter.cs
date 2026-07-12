using System;

public class StorageMeter : KMonoBehaviour
{
	public void SetInterpolateFunction(Func<float, int, float> func)
	{
		this.interpolateFunction = func;
		if (this.meter != null)
		{
			this.meter.interpolateFunction = this.interpolateFunction;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_frame", "meter_level" });
		this.meter.interpolateFunction = this.interpolateFunction;
		this.UpdateMeter(null);
		base.Subscribe(-1697596308, new Action<object>(this.UpdateMeter));
	}

	private void UpdateMeter(object data)
	{
		this.meter.SetPositionPercent(this.storage.MassStored() / this.storage.Capacity());
	}

	[MyCmpGet]
	private Storage storage;

	private MeterController meter;

	private Func<float, int, float> interpolateFunction = new Func<float, int, float>(MeterController.MinMaxStepLerp);
}
