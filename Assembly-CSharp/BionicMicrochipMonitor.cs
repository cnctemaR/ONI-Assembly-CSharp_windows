using System;
using UnityEngine;

public class BionicMicrochipMonitor : GameStateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.idle.TagTransition(GameTags.BionicBedTime, this.production, false);
		this.production.TagTransition(GameTags.BionicBedTime, this.idle, true).Enter(new StateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.State.Callback(BionicMicrochipMonitor.CreateProgresesBar)).Exit(new StateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.State.Callback(BionicMicrochipMonitor.ClearProgressBar))
			.ToggleStatusItem(Db.Get().DuplicantStatusItems.BionicMicrochipGeneration, null)
			.DefaultState(this.production.charging);
		this.production.charging.ParamTransition<float>(this.Progress, this.production.produceOne, GameStateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.IsGTEOne).Update(new Action<BionicMicrochipMonitor.Instance, float>(BionicMicrochipMonitor.ProgressUpdate), UpdateRate.SIM_200ms, false);
		this.production.produceOne.Enter(new StateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.State.Callback(BionicMicrochipMonitor.CreateMicrochip)).Enter(new StateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.State.Callback(BionicMicrochipMonitor.ResetProgress)).GoTo(this.production.charging);
	}

	public static void ClearProgressBar(BionicMicrochipMonitor.Instance smi)
	{
		smi.ClearProgressBar();
	}

	public static void CreateProgresesBar(BionicMicrochipMonitor.Instance smi)
	{
		smi.CreateProgressBar();
	}

	public static void ResetProgress(BionicMicrochipMonitor.Instance smi)
	{
		smi.sm.Progress.Set(0f, smi, false);
	}

	public static void CreateMicrochip(BionicMicrochipMonitor.Instance smi)
	{
		smi.CreateMicrochip();
	}

	public static void ProgressUpdate(BionicMicrochipMonitor.Instance smi, float dt)
	{
		float num = dt / 150f;
		float progress = smi.Progress;
		smi.sm.Progress.Set(progress + num, smi, false);
	}

	public const float MICROCHIP_PRODUCTION_TIME = 150f;

	public GameStateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.State idle;

	public BionicMicrochipMonitor.ProductionStates production;

	public StateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.FloatParameter Progress;

	public class Def : StateMachine.BaseDef
	{
	}

	public class ProductionStates : GameStateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.State
	{
		public GameStateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.State charging;

		public GameStateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.State produceOne;
	}

	public new class Instance : GameStateMachine<BionicMicrochipMonitor, BionicMicrochipMonitor.Instance, IStateMachineTarget, BionicMicrochipMonitor.Def>.GameInstance
	{
		public float Progress
		{
			get
			{
				return base.sm.Progress.Get(this);
			}
		}

		public Instance(IStateMachineTarget master, BionicMicrochipMonitor.Def def)
			: base(master, def)
		{
		}

		public void CreateMicrochip()
		{
			Util.KInstantiate(Assets.GetPrefab(PowerStationToolsConfig.tag), Grid.CellToPos(Grid.PosToCell(base.smi.gameObject), CellAlignment.Top, Grid.SceneLayer.Ore)).SetActive(true);
		}

		public void CreateProgressBar()
		{
			this.progressBar = ProgressBar.CreateProgressBar(base.gameObject, () => this.Progress);
			base.smi.progressBar.SetVisibility(true);
			base.smi.progressBar.barColor = Color.green;
		}

		public void ClearProgressBar()
		{
			if (this.progressBar != null)
			{
				Util.KDestroyGameObject(base.smi.progressBar.gameObject);
				this.progressBar = null;
			}
		}

		public ProgressBar progressBar;
	}
}
