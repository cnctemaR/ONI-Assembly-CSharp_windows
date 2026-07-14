using System;
using System.Collections.Generic;
using UnityEngine;

public class ReefGenerator : GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.inoperational;
		this.inoperational.Enter(new StateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State.Callback(ReefGenerator.DisableWattage)).EnterTransition(this.inoperational.withoutGeyser, (ReefGenerator.Instance smi) => this.geyserTarget.IsNull(smi)).EnterTransition(this.inoperational.withGeyser, (ReefGenerator.Instance smi) => !this.geyserTarget.IsNull(smi));
		this.inoperational.withoutGeyser.PlayAnim("off").Update(delegate(ReefGenerator.Instance smi, float _)
		{
			smi.MonitorForLinkableGeyser();
		}, UpdateRate.SIM_200ms, false).UpdateTransition(this.inoperational.withGeyser, (ReefGenerator.Instance smi, float _) => !this.geyserTarget.IsNull(smi), UpdateRate.SIM_200ms, false);
		this.inoperational.withGeyser.Enter(new StateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State.Callback(ReefGenerator.LinkToGeyser)).Exit(new StateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State.Callback(ReefGenerator.UnlinkFromGeyser)).EnterTransition(this.operational.inhale, (ReefGenerator.Instance smi) => smi.IsOperationalIgnoringGeyser())
			.ParamTransition<GameObject>(this.geyserTarget, this.inoperational.withoutGeyser, GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.IsNull)
			.EventTransition(GameHashes.OperationalFlagChanged, this.operational.inhale, (ReefGenerator.Instance smi) => smi.IsOperationalIgnoringGeyser());
		this.operational.Enter(new StateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State.Callback(ReefGenerator.LinkToGeyser)).Exit(new StateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State.Callback(ReefGenerator.UnlinkFromGeyser)).ParamTransition<GameObject>(this.geyserTarget, this.inoperational.withoutGeyser, GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.IsNull)
			.Target(this.masterTarget)
			.EventTransition(GameHashes.OperationalFlagChanged, this.inoperational, (ReefGenerator.Instance smi) => !smi.IsOperationalIgnoringGeyser());
		this.operational.inhale.Target(this.geyserTarget).TagTransition(GameTags.GeyserExhaling, this.operational.exhale, false).Target(this.masterTarget)
			.PlayAnim("off")
			.Enter(new StateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State.Callback(ReefGenerator.DisableWattage))
			.ToggleStatusItem(Db.Get().BuildingStatusItems.ReefGeneratorIdle, null);
		this.operational.exhale.Target(this.geyserTarget).TagTransition(GameTags.GeyserExhaling, this.operational.inhale, true).Target(this.masterTarget)
			.Enter(new StateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State.Callback(ReefGenerator.EnableWattage))
			.ToggleStatusItem(Db.Get().BuildingStatusItems.Wattage, (ReefGenerator.Instance smi) => smi.GeneratorPower);
	}

	private static void EnableWattage(ReefGenerator.Instance smi)
	{
		smi.SetWattageState(true);
	}

	private static void DisableWattage(ReefGenerator.Instance smi)
	{
		smi.SetWattageState(false);
	}

	private static void LinkToGeyser(ReefGenerator.Instance smi)
	{
		smi.LinkToGeyser();
	}

	private static void UnlinkFromGeyser(ReefGenerator.Instance smi)
	{
		smi.UnlinkFromGeyser();
	}

	private const string OFF_ANIM = "off";

	private readonly ReefGenerator.InoperationalState inoperational;

	private readonly ReefGenerator.OperationalState operational;

	private readonly StateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.TargetParameter geyserTarget;

	private static readonly Operational.Flag reefGeyserEmittingFlag = new Operational.Flag("reefGeyserEmitting", Operational.Flag.Type.Requirement);

	public class Def : StateMachine.BaseDef
	{
	}

	private class OperationalState : GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State
	{
		public GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State inhale;

		public GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State exhale;
	}

	private class InoperationalState : GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State
	{
		public GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State withoutGeyser;

		public GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.State withGeyser;
	}

	public new class Instance : GameStateMachine<ReefGenerator, ReefGenerator.Instance, IStateMachineTarget, ReefGenerator.Def>.GameInstance
	{
		public ReefGeneratorPower GeneratorPower { get; private set; }

		public Instance(IStateMachineTarget master, ReefGenerator.Def def)
			: base(master, def)
		{
			this.operational = master.GetComponent<Operational>();
			this.myController = master.GetComponent<KAnimControllerBase>();
			this.GeneratorPower = master.GetComponent<ReefGeneratorPower>();
		}

		public void MonitorForLinkableGeyser()
		{
			if (!base.sm.geyserTarget.IsNull(base.smi))
			{
				return;
			}
			int num = Grid.PosToCell(base.master.transform.GetPosition());
			GameObject gameObject = Grid.Objects[num, 1];
			if (gameObject == null)
			{
				return;
			}
			base.sm.geyserTarget.Set(gameObject, base.smi, false);
		}

		public bool IsOperationalIgnoringGeyser()
		{
			if (base.sm.geyserTarget.IsNull(base.smi))
			{
				return false;
			}
			foreach (KeyValuePair<Operational.Flag, bool> keyValuePair in this.operational.Flags)
			{
				if (keyValuePair.Key != ReefGenerator.reefGeyserEmittingFlag && !keyValuePair.Value)
				{
					return false;
				}
			}
			return true;
		}

		public void SetWattageState(bool active)
		{
			this.operational.SetFlag(ReefGenerator.reefGeyserEmittingFlag, active);
		}

		public void LinkToGeyser()
		{
			GameObject gameObject = base.sm.geyserTarget.Get(base.smi);
			if (gameObject == null)
			{
				return;
			}
			this.geyserController = gameObject.GetComponent<KAnimControllerBase>();
			if (this.geyserController == null)
			{
				return;
			}
			this.animLink = new KAnimLink(this.geyserController, this.myController);
			KAnimSynchronizer synchronizer = this.geyserController.GetSynchronizer();
			synchronizer.Add(this.myController, new KAnimSynchronizer.TranslateAnimName(this.TranslateGeyserAnim));
			synchronizer.Sync(this.myController);
			synchronizer.IdleAnim = "off";
		}

		public void UnlinkFromGeyser()
		{
			if (this.animLink != null)
			{
				this.animLink.Unregister();
				this.animLink = null;
			}
			if (this.geyserController != null)
			{
				this.geyserController.GetSynchronizer().Remove(this.myController);
			}
			this.geyserController = null;
		}

		private string TranslateGeyserAnim(string masterAnimName)
		{
			if (this.IsOperationalIgnoringGeyser())
			{
				return masterAnimName;
			}
			foreach (string text in ReefGenerator.Instance.PHASE_PREFIXES)
			{
				if (masterAnimName.StartsWith(text))
				{
					string text2 = text;
					string text3 = "_inoperable";
					int length = text.Length;
					return text2 + text3 + masterAnimName.Substring(length, masterAnimName.Length - length);
				}
			}
			return masterAnimName;
		}

		private KAnimLink animLink;

		private KAnimControllerBase geyserController;

		private readonly Operational operational;

		private readonly KAnimControllerBase myController;

		private const string INOPERABLE_INSERT = "_inoperable";

		private static readonly string[] PHASE_PREFIXES = new string[] { "inhale", "exhale" };
	}
}
