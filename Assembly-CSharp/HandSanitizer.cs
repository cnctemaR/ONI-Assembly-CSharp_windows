using System;
using System.Collections.Generic;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

public class HandSanitizer : StateMachineComponent<HandSanitizer.SMInstance>, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.FindOrAddComponent<Workable>();
	}

	private void RefreshMeters()
	{
		float num = 0f;
		PrimaryElement primaryElement = base.GetComponent<Storage>().FindPrimaryElement(this.consumedElement);
		if (primaryElement != null)
		{
			num = Mathf.Clamp01(primaryElement.Mass / ((float)this.maxUses * this.massConsumedPerUse));
		}
		float num2 = 0f;
		PrimaryElement primaryElement2 = base.GetComponent<Storage>().FindPrimaryElement(this.outputElement);
		if (primaryElement2 != null)
		{
			num2 = Mathf.Clamp01(primaryElement2.Mass / ((float)this.maxUses * this.massConsumedPerUse));
		}
		this.cleanMeter.SetPositionPercent(num);
		this.dirtyMeter.SetPositionPercent(num2);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.cleanMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_clean_target", "meter_clean", Meter.Offset.Infront, new string[] { "meter_clean_target" });
		this.dirtyMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_dirty_target", "meter_dirty", Meter.Offset.Infront, new string[] { "meter_dirty_target" });
		this.RefreshMeters();
		Components.HandSanitizers.Add(this);
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		DirectionControl component = base.GetComponent<DirectionControl>();
		component.onDirectionChanged = (Action<WorkableReactable.AllowedDirection>)Delegate.Combine(component.onDirectionChanged, new Action<WorkableReactable.AllowedDirection>(this.OnDirectionChanged));
		this.OnDirectionChanged(base.GetComponent<DirectionControl>().allowedDirection);
	}

	protected override void OnCleanUp()
	{
		Components.HandSanitizers.Remove(this);
		base.OnCleanUp();
	}

	private void OnDirectionChanged(WorkableReactable.AllowedDirection allowed_direction)
	{
		if (this.reactable != null)
		{
			this.reactable.allowedDirection = allowed_direction;
		}
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.REQUIRESELEMENT, ElementLoader.FindElementByHash(this.consumedElement).name), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESELEMENT, ElementLoader.FindElementByHash(this.consumedElement).name), Descriptor.DescriptorType.Requirement, false)
		};
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.DISEASECONSUMEDPERUSE, GameUtil.GetFormattedDiseaseAmount(this.diseaseRemovalCount)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DISEASECONSUMEDPERUSE, GameUtil.GetFormattedDiseaseAmount(this.diseaseRemovalCount)), Descriptor.DescriptorType.Effect, false)
		};
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(this.RequirementDescriptors(def));
		list.AddRange(this.EffectDescriptors(def));
		return list;
	}

	private void OnStorageChange(object data)
	{
		if (base.smi.OutputFull())
		{
			base.smi.DumpOutput();
		}
		this.RefreshMeters();
	}

	public float massConsumedPerUse = 1f;

	public SimHashes consumedElement = SimHashes.BleachStone;

	public int diseaseRemovalCount = 10000;

	public int maxUses = 10;

	public SimHashes outputElement = SimHashes.Vacuum;

	private WorkableReactable reactable;

	private MeterController cleanMeter;

	private MeterController dirtyMeter;

	[Serialize]
	public int maxPossiblyRemoved;

	private class WashHandsReactable : WorkableReactable
	{
		public WashHandsReactable(Workable workable, ChoreType chore_type, WorkableReactable.AllowedDirection allowed_direction = WorkableReactable.AllowedDirection.Any)
			: base(workable, chore_type, allowed_direction)
		{
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (base.InternalCanBegin(new_reactor, transition))
			{
				PrimaryElement component = new_reactor.GetComponent<PrimaryElement>();
				if (component != null)
				{
					return component.DiseaseIdx != byte.MaxValue;
				}
			}
			return false;
		}
	}

	public class SMInstance : GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.GameInstance
	{
		public SMInstance(HandSanitizer master)
			: base(master)
		{
		}

		public bool HasSufficientMass()
		{
			bool flag = false;
			PrimaryElement primaryElement = base.GetComponent<Storage>().FindPrimaryElement(base.master.consumedElement);
			if (primaryElement != null)
			{
				flag = primaryElement.Mass > 0f;
			}
			return flag;
		}

		public bool OutputFull()
		{
			PrimaryElement primaryElement = base.GetComponent<Storage>().FindPrimaryElement(base.master.outputElement);
			return primaryElement != null && primaryElement.Mass >= (float)base.master.maxUses * base.master.massConsumedPerUse;
		}

		public void OnCompleteWork(Worker worker)
		{
		}

		public void DumpOutput()
		{
			Storage component = base.master.GetComponent<Storage>();
			if (base.master.outputElement != SimHashes.Vacuum)
			{
				component.Drop(ElementLoader.FindElementByHash(base.master.outputElement).tag);
			}
		}
	}

	public class States : GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.notready;
			this.notoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.notready, false);
			this.notready.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, this.ready, (HandSanitizer.SMInstance smi) => smi.HasSufficientMass()).TagTransition(GameTags.Operational, this.notoperational, true);
			this.ready.DefaultState(this.ready.free).ToggleReactable((HandSanitizer.SMInstance smi) => smi.master.reactable = new HandSanitizer.WashHandsReactable(smi.master.GetComponent<HandSanitizer.Work>(), Db.Get().ChoreTypes.WashHands, smi.master.GetComponent<DirectionControl>().allowedDirection)).EventTransition(GameHashes.OnStorageChange, this.notready, (HandSanitizer.SMInstance smi) => !smi.HasSufficientMass())
				.TagTransition(GameTags.Operational, this.notoperational, true);
			this.ready.free.PlayAnim("on").WorkableStartTransition((HandSanitizer.SMInstance smi) => smi.GetComponent<HandSanitizer.Work>(), this.ready.occupied);
			this.ready.occupied.PlayAnim("working_pre").QueueAnim("working_loop", true, null).WorkableStopTransition((HandSanitizer.SMInstance smi) => smi.GetComponent<HandSanitizer.Work>(), this.ready);
		}

		public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State notready;

		public HandSanitizer.States.ReadyStates ready;

		public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State notoperational;

		public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State full;

		public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State empty;

		public class ReadyStates : GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State
		{
			public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State free;

			public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State occupied;
		}
	}

	public class Work : Workable, IGameObjectEffectDescriptor
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.resetProgressOnStop = true;
			this.shouldTransferDiseaseWithWorker = false;
			GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate(object obj)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
			}, null, null);
		}

		protected override void OnStartWork(Worker worker)
		{
			base.OnStartWork(worker);
			this.diseaseRemoved = 0;
		}

		public override void AwardExperience(float work_dt, MinionResume resume)
		{
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			base.OnWorkTick(worker, dt);
			HandSanitizer component = base.GetComponent<HandSanitizer>();
			Storage component2 = base.GetComponent<Storage>();
			PrimaryElement component3 = worker.GetComponent<PrimaryElement>();
			int num = Math.Min((int)(dt / this.workTime * (float)component.diseaseRemovalCount), component3.DiseaseCount);
			this.diseaseRemoved += num;
			SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
			invalid.idx = component3.DiseaseIdx;
			invalid.count = num;
			component3.ModifyDiseaseCount(-num, "HandSanitizer.OnWorkTick");
			component.maxPossiblyRemoved += num;
			float num2 = component.massConsumedPerUse * dt / this.workTime;
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
			float num3;
			component2.ConsumeAndGetDisease(ElementLoader.FindElementByHash(component.consumedElement).tag, num2, out diseaseInfo, out num3);
			if (component.outputElement != SimHashes.Vacuum)
			{
				diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(invalid, diseaseInfo);
				component2.AddLiquid(component.outputElement, num2, num3, diseaseInfo.idx, diseaseInfo.count, false, true);
			}
			return this.diseaseRemoved > component.diseaseRemovalCount;
		}

		protected override void OnCompleteWork(Worker worker)
		{
			base.OnCompleteWork(worker);
		}

		private int diseaseRemoved;
	}
}
