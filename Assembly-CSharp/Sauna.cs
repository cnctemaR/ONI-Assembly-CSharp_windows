using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class Sauna : StateMachineComponent<Sauna.StatesInstance>, IEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void AddRequirementDesc(List<Descriptor> descs, Tag tag, float mass)
	{
		string text = tag.ProperName();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
		descs.Add(descriptor);
	}

	List<Descriptor> IEffectDescriptor.GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, Descriptor.DescriptorType.Effect, false));
		Effect.AddModifierDescriptions(base.gameObject, list, this.specificEffect, true);
		Element element = ElementLoader.FindElementByHash(SimHashes.Steam);
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, element.name, GameUtil.GetFormattedMass(this.steamPerUseKG, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, element.name, GameUtil.GetFormattedMass(this.steamPerUseKG, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement, false));
		Element element2 = ElementLoader.FindElementByHash(SimHashes.Water);
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, element2.name, GameUtil.GetFormattedMass(this.steamPerUseKG, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, element2.name, GameUtil.GetFormattedMass(this.steamPerUseKG, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Effect, false));
		return list;
	}

	public string specificEffect;

	public string trackingEffect;

	public float steamPerUseKG;

	public float waterOutputTemp;

	public static readonly Operational.Flag sufficientSteam = new Operational.Flag("sufficientSteam", Operational.Flag.Type.Requirement);

	public class States : GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.inoperational;
			this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational, false).ToggleMainStatusItem(Db.Get().BuildingStatusItems.MissingRequirements, null);
			this.operational.TagTransition(GameTags.Operational, this.inoperational, true).ToggleMainStatusItem(Db.Get().BuildingStatusItems.GettingReady, null).EventTransition(GameHashes.OnStorageChange, this.ready, new StateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.Transition.ConditionCallback(this.IsReady));
			this.ready.TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<Sauna.StatesInstance, Chore>(this.CreateChore), this.inoperational)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working, null);
			this.ready.idle.WorkableStartTransition((Sauna.StatesInstance smi) => smi.master.GetComponent<SaunaWorkable>(), this.ready.working).EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.Not(new StateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.Transition.ConditionCallback(this.IsReady)));
			this.ready.working.WorkableCompleteTransition((Sauna.StatesInstance smi) => smi.master.GetComponent<SaunaWorkable>(), this.ready.idle).WorkableStopTransition((Sauna.StatesInstance smi) => smi.master.GetComponent<SaunaWorkable>(), this.ready.idle);
		}

		private Chore CreateChore(Sauna.StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<SaunaWorkable>();
			WorkChore<SaunaWorkable> workChore = new WorkChore<SaunaWorkable>(Db.Get().ChoreTypes.Relax, component, null, true, null, null, null, false, Db.Get().ScheduleBlockTypes.Recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
			workChore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
			return workChore;
		}

		private bool IsReady(Sauna.StatesInstance smi)
		{
			PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Steam);
			return primaryElement != null && primaryElement.Mass >= smi.master.steamPerUseKG;
		}

		private GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State inoperational;

		private GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State operational;

		private Sauna.States.ReadyStates ready;

		public class ReadyStates : GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State
		{
			public GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State idle;

			public GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State working;
		}
	}

	public class StatesInstance : GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.GameInstance
	{
		public StatesInstance(Sauna smi)
			: base(smi)
		{
		}
	}
}
