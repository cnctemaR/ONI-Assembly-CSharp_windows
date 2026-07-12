using System;

public class DevPump : Filterable, ISim1000ms
{
	private Element element
	{
		get
		{
			if (base.SelectedTag.IsValid)
			{
				return ElementLoader.GetElement(base.SelectedTag);
			}
			return ElementLoader.FindElementByHash(SimHashes.Void);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.elementState == Filterable.ElementState.Liquid)
		{
			base.SelectedTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
			return;
		}
		if (this.elementState == Filterable.ElementState.Gas)
		{
			base.SelectedTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.filterElementState = this.elementState;
	}

	public void Sim1000ms(float dt)
	{
		float num = 10f - this.storage.GetAmountAvailable(this.element.tag);
		if (num <= 0f)
		{
			return;
		}
		if (this.element.IsLiquid)
		{
			this.storage.AddLiquid(this.element.id, num, this.element.defaultValues.temperature, byte.MaxValue, 0, false, true);
			return;
		}
		if (this.element.IsGas)
		{
			this.storage.AddGasChunk(this.element.id, num, this.element.defaultValues.temperature, byte.MaxValue, 0, false, true);
		}
	}

	public Filterable.ElementState elementState = Filterable.ElementState.Liquid;

	[MyCmpReq]
	private Storage storage;
}
