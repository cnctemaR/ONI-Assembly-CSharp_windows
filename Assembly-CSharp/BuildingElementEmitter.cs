using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class BuildingElementEmitter : KMonoBehaviour, IEffectDescriptor, IElementEmitter, ISim200ms
{
	public float AverageEmitRate
	{
		get
		{
			return Game.Instance.accumulators.GetAverageRate(this.accumulator);
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
		this.accumulator = Game.Instance.accumulators.Add("Element", this);
		base.Subscribe(824508782, new Action<object>(this.OnActiveChanged));
		this.SimRegister();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(this.accumulator);
		this.SimUnregister();
		base.OnCleanUp();
	}

	private void OnActiveChanged(object data)
	{
		this.simActive = (bool)data;
		this.dirty = true;
	}

	public void Sim200ms(float dt)
	{
		this.UnsafeUpdate(dt);
	}

	private unsafe void UnsafeUpdate(float dt)
	{
		if (!Sim.IsValidHandle(this.simHandle))
		{
			return;
		}
		this.UpdateSimState();
		Sim.EmittedMassInfo emittedMassInfo = Game.Instance.simData.emittedMassEntries[this.simHandle];
		if (emittedMassInfo.mass > 0f)
		{
			Game.Instance.accumulators.Accumulate(this.accumulator, emittedMassInfo.mass);
			if (this.element == SimHashes.Oxygen)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, emittedMassInfo.mass, base.gameObject.GetProperName(), null);
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
				Vector3 vector = new Vector3(base.transform.GetPosition().x + this.modifierOffset.x, base.transform.GetPosition().y + this.modifierOffset.y, 0f);
				int num = Grid.PosToCell(vector);
				SimMessages.ModifyElementEmitter(this.simHandle, num, (int)this.emitRange, this.element, 0.2f, this.emitRate * 0.2f, this.temperature, float.MaxValue, this.emitDiseaseIdx, this.emitDiseaseCount);
			}
			this.statusHandle = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.EmittingElement, this);
		}
		else
		{
			SimMessages.ModifyElementEmitter(this.simHandle, 0, 0, SimHashes.Vacuum, 0f, 0f, 0f, 0f, byte.MaxValue, 0);
			this.statusHandle = base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle, this);
		}
	}

	private void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1)
		{
			this.simHandle = -2;
			SimMessages.AddElementEmitter(float.MaxValue, Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(delegate(object data)
			{
				BuildingElementEmitter.OnSimRegistered(this, data);
			}, "BuildingElementEmitter")).index, -1, -1);
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
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, text, GameUtil.GetFormattedMass(this.EmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, text, GameUtil.GetFormattedMass(this.EmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Effect);
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

	[SerializeField]
	public byte emitRange = 1;

	[SerializeField]
	public byte emitDiseaseIdx = byte.MaxValue;

	[SerializeField]
	public int emitDiseaseCount;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private int simHandle = -1;

	private bool simActive;

	private bool dirty = true;

	private Guid statusHandle;
}
