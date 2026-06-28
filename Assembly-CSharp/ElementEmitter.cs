using System;
using UnityEngine;

public class ElementEmitter : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.SimRegister();
	}

	protected override void OnCleanUp()
	{
		this.SimUnregister();
		base.OnCleanUp();
	}

	public void SetEmitting(bool emitting)
	{
		this.simActive = emitting;
		this.dirty = true;
	}

	private void SimUpdate(float dt)
	{
		if (!Sim.IsValidHandle(this.simHandle))
		{
			return;
		}
		this.UpdateSimState();
	}

	private void UpdateSimState()
	{
		if (!this.dirty)
		{
			return;
		}
		this.dirty = false;
		int num = Grid.PosToCell(this.transform.position);
		int num2 = Grid.OffsetCell(num, (int)this.outputElement.outputElementOffset.x, (int)this.outputElement.outputElementOffset.y);
		if (this.simActive)
		{
			if (this.outputElement.elementHash != (SimHashes)0 && this.outputElement.massGenerationRate > 0f && this.emissionFrequency > 0f)
			{
				float num3 = ((this.outputElement.outputTemperature != 0f) ? this.outputElement.outputTemperature : base.GetComponent<PrimaryElement>().Temperature);
				SimMessages.ModifyElementEmitter(this.simHandle, num2, this.outputElement.elementHash, this.emissionFrequency, this.outputElement.massGenerationRate, num3);
			}
			if (this.showDescriptor)
			{
				this.statusHandle = base.GetComponent<KSelectable>().ReplaceStatusItem(this.statusHandle, Db.Get().BuildingStatusItems.ElementEmitterOutput, this);
			}
		}
		else
		{
			SimMessages.ModifyElementEmitter(this.simHandle, num2, SimHashes.Vacuum, 0f, 0f, 0f);
			if (this.showDescriptor)
			{
				this.statusHandle = base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle, false);
			}
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
		float num = this.outputElement.outputTemperature;
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
		if (base.isSpawned && this.simHandle == -1)
		{
			this.simHandle = -2;
			SimMessages.AddElementEmitter(Game.Instance.complexCallbackManager.Add(delegate(object data)
			{
				ElementEmitter.OnSimRegistered(this, data);
			}, "ElementEmitter").index);
		}
	}

	private void SimUnregister()
	{
		if (this.simHandle != -1)
		{
			if (Sim.IsValidHandle(this.simHandle))
			{
				SimMessages.RemoveElementEmitter(-1, this.simHandle);
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

	private bool simActive = true;

	private bool dirty = true;

	public bool showDescriptor = true;
}
