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
		base.Subscribe<BuildingElementEmitter>(824508782, BuildingElementEmitter.OnActiveChangedDelegate);
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
		this.simActive = ((Operational)data).IsActive;
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
		int handleIndex = Sim.GetHandleIndex(this.simHandle);
		Sim.EmittedMassInfo emittedMassInfo = Game.Instance.simData.emittedMassEntries[handleIndex];
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
				int num = Grid.PosToCell(new Vector3(base.transform.GetPosition().x + this.modifierOffset.x, base.transform.GetPosition().y + this.modifierOffset.y, 0f));
				SimMessages.ModifyElementEmitter(this.simHandle, num, (int)this.emitRange, this.element, 0.2f, this.emitRate * 0.2f, this.temperature, float.MaxValue, this.emitDiseaseIdx, this.emitDiseaseCount);
			}
			this.statusHandle = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.EmittingElement, this);
			return;
		}
		SimMessages.ModifyElementEmitter(this.simHandle, 0, 0, SimHashes.Vacuum, 0f, 0f, 0f, 0f, byte.MaxValue, 0);
		this.statusHandle = base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle, this);
	}

	private void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1)
		{
			this.simHandle = -2;
			SimMessages.AddElementEmitter(float.MaxValue, Game.Instance.simComponentCallbackManager.Add(new Action<int, object>(BuildingElementEmitter.OnSimRegisteredCallback), this, "BuildingElementEmitter").index, -1, -1);
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

	private static void OnSimRegisteredCallback(int handle, object data)
	{
		((BuildingElementEmitter)data).OnSimRegistered(handle);
	}

	private void OnSimRegistered(int handle)
	{
		if (this != null)
		{
			this.simHandle = handle;
			return;
		}
		SimMessages.RemoveElementEmitter(-1, handle);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		string text = ElementLoader.FindElementByHash(this.element).tag.ProperName();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_FIXEDTEMP, text, GameUtil.GetFormattedMass(this.EmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(this.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_FIXEDTEMP, text, GameUtil.GetFormattedMass(this.EmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(this.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Effect);
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

	private static readonly EventSystem.IntraObjectHandler<BuildingElementEmitter> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<BuildingElementEmitter>(delegate(BuildingElementEmitter component, object data)
	{
		component.OnActiveChanged(data);
	});
}
