using System;
using UnityEngine;

public class ElementEmitter : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
	}

	protected override void OnCmpDisable()
	{
		this.SimUnregister();
		base.OnCmpDisable();
	}

	protected override void OnCleanUp()
	{
		this.SimUnregister();
		base.OnCleanUp();
	}

	public void SetEmitting(bool t)
	{
		if (t)
		{
			this.SimRegister();
		}
		else
		{
			this.SimUnregister();
		}
	}

	public void ForceEmit(float mass, float temperature = -1f)
	{
		Vector3 vector = new Vector3(0f, 0.5f, 0f);
		if (mass <= 0f)
		{
			return;
		}
		Element element = ElementLoader.FindElementByHash(this.outputElement.elementHash);
		float num = element.defaultValues.temperature;
		if (temperature > 0f)
		{
			num = temperature;
		}
		if (element.IsGas || element.IsLiquid)
		{
			int num2 = Grid.PosToCell(this.transform.position);
			SimMessages.AddRemoveSubstance(num2, this.outputElement.elementHash, CellEventLogger.Instance.ElementConsumerSimUpdate, mass, num, -1);
		}
		else if (element.IsSolid)
		{
			element.substance.SpawnResource(this.transform.position + vector, mass, num, false, true);
		}
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ElementLoader.FindElementByHash(this.outputElement.elementHash).name, base.gameObject.transform, 1.5f, false);
	}

	private void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1 && this.outputElement.elementHash != (SimHashes)0 && this.outputElement.outputMass > 0f && this.emissionFrequency > 0f)
		{
			int num = Grid.PosToCell(this.transform.position);
			int num2 = Grid.OffsetCell(num, (int)this.outputElement.outputElementOffset.x, (int)this.outputElement.outputElementOffset.y);
			this.statusHandle = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ElementEmitterOutput, this);
			this.simHandle = -2;
			HandleVector<Action<object>>.Handle handle = Game.Instance.complexCallbackManager.Add(delegate(object data)
			{
				ElementEmitter.OnSimRegistered(this, data);
			}, "ElementEmitter");
			float num3 = ((this.outputElement.outputTemperature != 0f) ? this.outputElement.outputTemperature : base.GetComponent<PrimaryElement>().Temperature);
			SimMessages.AddElementEmitter(num2, this.outputElement.elementHash, this.emissionFrequency, this.outputElement.outputMass, num3, handle.index);
		}
	}

	private void SimUnregister()
	{
		if (this.simHandle != -1)
		{
			if (Sim.IsValidHandle(this.simHandle))
			{
				SimMessages.RemoveElementEmitter(-1, this.simHandle);
				base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle);
				this.statusHandle = Guid.Empty;
			}
			this.simHandle = -1;
		}
	}

	private static void OnSimRegistered(ElementEmitter instance, object data)
	{
		int num = (int)data;
		if (instance != null)
		{
			instance.simHandle = num;
		}
		else
		{
			SimMessages.RemoveElementEmitter(-1, num);
		}
	}

	[SerializeField]
	public ElementConverter.OutputElement outputElement;

	[SerializeField]
	public float emissionFrequency = 1f;

	private int simHandle = -1;

	private Guid statusHandle = Guid.Empty;
}
