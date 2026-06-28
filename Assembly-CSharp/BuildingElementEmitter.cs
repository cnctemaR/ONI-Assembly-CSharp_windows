using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class BuildingElementEmitter : KMonoBehaviour, IEffectDescriptor
{
	public int DescriptionOrder { get; set; }

	public float AverageEmitRate
	{
		get
		{
			return this.accumulator.AvgFlowRate;
		}
	}

	public float MassChangeRate
	{
		get
		{
			return this.massChangeRate;
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
		this.Subscribe(824508782, new EventSystem.EventHandler(this.OnActiveChanged));
		this.OnActiveChanged(null);
	}

	protected override void OnCleanUp()
	{
		this.SimUnregister();
		base.OnCleanUp();
	}

	private void OnActiveChanged(object data)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		Operational component2 = base.GetComponent<Operational>();
		if (component2 != null && component2.IsActive)
		{
			this.statusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.EmittingElement, this);
			base.enabled = true;
			this.SimRegister();
		}
		else
		{
			component.RemoveStatusItem(this.statusHandle);
			base.enabled = false;
			this.SimUnregister();
		}
	}

	private unsafe void SimUpdate(float dt)
	{
		if (!Sim.IsValidHandle(this.simHandle))
		{
			return;
		}
		Sim.MassChangeInfo massChangeInfo = Game.Instance.simData.emittedMassEntries[this.simHandle];
		if (massChangeInfo.mass > 0f)
		{
			this.accumulator.Accumulate(massChangeInfo.mass);
			if (this.element == SimHashes.Oxygen)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, massChangeInfo.mass, null);
			}
		}
	}

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(this.element);
		string text = element.tag.ProperName();
		string keywordStyle = GameUtil.GetKeywordStyle(this.element);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, keywordStyle, text, GameUtil.GetFormattedMass(this.MassChangeRate, GameUtil.TimeSlice.PerSecond, true, "F1"))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, keywordStyle, text, GameUtil.GetFormattedMass(this.MassChangeRate, GameUtil.TimeSlice.PerSecond, true, "F1")));
		list.Add(descriptor);
		return list;
	}

	private void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1 && this.element != (SimHashes)0 && this.massChangeRate > 0f)
		{
			Vector3 vector = new Vector3(this.transform.position.x + this.modifierOffset.x, this.transform.position.y + this.modifierOffset.y, 0f);
			int num = Grid.PosToCell(vector);
			this.simHandle = -2;
			HandleVector<Action<object>>.Handle handle = Game.Instance.complexCallbackManager.Add(delegate(object data)
			{
				BuildingElementEmitter.OnSimRegistered(this, data);
			}, "BuildingElementEmitter");
			SimMessages.AddElementEmitter(num, this.element, 0.25f, this.massChangeRate, this.temperature, handle.index);
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

	[SerializeField]
	public float massChangeRate = 0.3f;

	[SerializeField]
	[Serialize]
	public float temperature = 293f;

	[HashedEnum]
	[SerializeField]
	public SimHashes element = SimHashes.Oxygen;

	private Accumulator accumulator;

	private Guid statusHandle;

	private int simHandle = -1;

	[NonSerialized]
	public Vector2 modifierOffset;
}
