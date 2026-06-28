using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class BuildingElementEmitter : KMonoBehaviour, IEffectDescriptor
{
	public float AverageEmitRate
	{
		get
		{
			return this.accumulator.AvgRate;
		}
	}

	public float EmitRate
	{
		get
		{
			return this.emitRate;
		}
	}

	public SimHashes Element
	{
		get
		{
			return this.element;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.accumulator = new Accumulator("Element", this, 3f);
		this.Subscribe(824508782, new Action<object>(this.OnActiveChanged));
		this.SimRegister();
	}

	protected override void OnCleanUp()
	{
		this.SimUnregister();
		base.OnCleanUp();
	}

	private void OnActiveChanged(object data)
	{
		this.simActive = (bool)data;
		this.dirty = true;
	}

	private unsafe void SimUpdate(float dt)
	{
		if (!Sim.IsValidHandle(this.simHandle))
		{
			return;
		}
		this.UpdateSimState();
		Sim.EmittedMassInfo emittedMassInfo = Game.Instance.simData.emittedMassEntries[this.simHandle];
		if (emittedMassInfo.mass > 0f)
		{
			this.accumulator.Accumulate(emittedMassInfo.mass);
			if (this.element == SimHashes.Oxygen)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, emittedMassInfo.mass, null);
			}
		}
	}

	private void UpdateSimState()
	{
		if (!this.dirty)
		{
			return;
		}
		this.dirty = false;
		if (this.simActive)
		{
			if (this.element != (SimHashes)0 && this.emitRate > 0f)
			{
				Vector3 vector = new Vector3(this.transform.position.x + this.modifierOffset.x, this.transform.position.y + this.modifierOffset.y, 0f);
				int num = Grid.PosToCell(vector);
				SimMessages.ModifyElementEmitter(this.simHandle, num, this.element, 0.25f, this.emitRate * 0.25f, this.temperature);
			}
			this.statusHandle = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.EmittingElement, this);
		}
		else
		{
			SimMessages.ModifyElementEmitter(this.simHandle, 0, SimHashes.Vacuum, 0f, 0f, 0f);
			this.statusHandle = base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle, this);
		}
	}

	private void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1)
		{
			this.simHandle = -2;
			SimMessages.AddElementEmitter(Game.Instance.complexCallbackManager.Add(delegate(object data)
			{
				BuildingElementEmitter.OnSimRegistered(this, data);
			}, "BuildingElementEmitter").index);
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

	private static void OnSimRegistered(BuildingElementEmitter instance, object data)
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

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(this.element);
		string text = element.tag.ProperName();
		string keywordStyle = GameUtil.GetKeywordStyle(this.element);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, keywordStyle, text, GameUtil.GetFormattedMass(this.EmitRate, GameUtil.TimeSlice.PerSecond, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, keywordStyle, text, GameUtil.GetFormattedMass(this.EmitRate, GameUtil.TimeSlice.PerSecond, true, "{0:0.#}")), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	[SerializeField]
	public float emitRate = 0.3f;

	[SerializeField]
	[Serialize]
	public float temperature = 293f;

	[SerializeField]
	[HashedEnum]
	public SimHashes element = SimHashes.Oxygen;

	[SerializeField]
	public Vector2 modifierOffset;

	private Accumulator accumulator;

	private int simHandle = -1;

	private bool simActive;

	private bool dirty = true;

	private Guid statusHandle;
}
