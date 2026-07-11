using System;
using UnityEngine;

public class ElementSplitterComponents : KGameObjectComponentManager<ElementSplitter>
{
	public HandleVector<int>.Handle Add(GameObject go)
	{
		return base.Add(go, new ElementSplitter(go));
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle handle)
	{
		ElementSplitter data = base.GetData(handle);
		Pickupable component = data.primaryElement.GetComponent<Pickupable>();
		Func<float, Pickupable> func = (float amount) => ElementSplitterComponents.OnTake(handle, amount);
		component.OnTake = (Func<float, Pickupable>)Delegate.Combine(component.OnTake, func);
		Func<Pickupable, bool> func2 = delegate(Pickupable other)
		{
			HandleVector<int>.Handle handle2 = this.GetHandle(other.gameObject);
			return ElementSplitterComponents.CanFirstAbsorbSecond(handle, handle2);
		};
		component.CanAbsorb = (Func<Pickupable, bool>)Delegate.Combine(component.CanAbsorb, func2);
		component.absorbable = true;
		data.onTakeCB = func;
		data.canAbsorbCB = func2;
		base.SetData(handle, data);
	}

	protected override void OnSpawn(HandleVector<int>.Handle handle)
	{
	}

	protected override void OnCleanUp(HandleVector<int>.Handle handle)
	{
		ElementSplitter data = base.GetData(handle);
		if (data.primaryElement != null)
		{
			Pickupable component = data.primaryElement.GetComponent<Pickupable>();
			if (component != null)
			{
				Pickupable pickupable = component;
				pickupable.OnTake = (Func<float, Pickupable>)Delegate.Remove(pickupable.OnTake, data.onTakeCB);
				Pickupable pickupable2 = component;
				pickupable2.CanAbsorb = (Func<Pickupable, bool>)Delegate.Remove(pickupable2.CanAbsorb, data.canAbsorbCB);
			}
		}
	}

	private static bool CanFirstAbsorbSecond(HandleVector<int>.Handle first, HandleVector<int>.Handle second)
	{
		if (first == HandleVector<int>.InvalidHandle || second == HandleVector<int>.InvalidHandle)
		{
			return false;
		}
		ElementSplitter data = GameComps.ElementSplitters.GetData(first);
		ElementSplitter data2 = GameComps.ElementSplitters.GetData(second);
		return data.primaryElement.ElementID == data2.primaryElement.ElementID && data.primaryElement.Units + data2.primaryElement.Units < 25000f;
	}

	private static Pickupable OnTake(HandleVector<int>.Handle handle, float amount)
	{
		ElementSplitter data = GameComps.ElementSplitters.GetData(handle);
		Pickupable component = data.primaryElement.GetComponent<Pickupable>();
		Storage storage = component.storage;
		PrimaryElement component2 = component.GetComponent<PrimaryElement>();
		Pickupable component3 = component2.Element.substance.SpawnResource(component.transform.GetPosition(), amount, component2.Temperature, byte.MaxValue, 0, true, false, false).GetComponent<Pickupable>();
		component.TotalAmount -= amount;
		component3.Trigger(1335436905, component);
		ElementSplitterComponents.CopyRenderSettings(component.GetComponent<KBatchedAnimController>(), component3.GetComponent<KBatchedAnimController>());
		if (storage != null)
		{
			storage.Trigger(-1697596308, data.primaryElement.gameObject);
			storage.Trigger(-778359855, null);
		}
		return component3;
	}

	private static void CopyRenderSettings(KBatchedAnimController src, KBatchedAnimController dest)
	{
		if (src != null && dest != null)
		{
			dest.OverlayColour = src.OverlayColour;
		}
	}

	private const float MAX_STACK_SIZE = 25000f;
}
