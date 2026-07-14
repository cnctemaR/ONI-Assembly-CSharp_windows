using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class TemperatureOverlayInfraredVisualizerBase : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.updateCBHandle = Game.Instance.Subscribe(-880408538, TemperatureOverlayInfraredVisualizerBase.OverlayInfraredUpdateDispatcher, this);
		this.clearCBHandle = Game.Instance.Subscribe(972756592, TemperatureOverlayInfraredVisualizerBase.OverlayIfraredClearDispatcher, this);
		base.OnPrefabInit();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.Unsubscribe(ref this.updateCBHandle);
		Game.Instance.Unsubscribe(ref this.clearCBHandle);
		base.OnCleanUp();
	}

	private void OnTemperatureOverlayInfraredUpdate(object obj)
	{
		Infrared.TemperatureOverlayInfraredData temperatureOverlayInfraredData = (Infrared.TemperatureOverlayInfraredData)obj;
		Vector3 position = base.transform.GetPosition();
		if (temperatureOverlayInfraredData.bounds.Min <= position && position <= temperatureOverlayInfraredData.bounds.Max)
		{
			this.TemperatureOverlayInfraredUpdate(temperatureOverlayInfraredData);
		}
	}

	protected abstract void TemperatureOverlayInfraredUpdate(Infrared.TemperatureOverlayInfraredData data);

	protected abstract void TemperatureOverlayInfraredClear();

	private int updateCBHandle = -1;

	private int clearCBHandle = -1;

	private static Action<object, object> OverlayInfraredUpdateDispatcher = delegate(object context, object data)
	{
		Unsafe.As<TemperatureOverlayInfraredVisualizerBase>(context).OnTemperatureOverlayInfraredUpdate(data);
	};

	private static Action<object, object> OverlayIfraredClearDispatcher = delegate(object context, object data)
	{
		Unsafe.As<TemperatureOverlayInfraredVisualizerBase>(context).TemperatureOverlayInfraredClear();
	};
}
