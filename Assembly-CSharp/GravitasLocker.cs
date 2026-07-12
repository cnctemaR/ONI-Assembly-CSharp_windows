using System;
using UnityEngine;

public class GravitasLocker : GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.close;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.close.ParamTransition<bool>(this.IsOpen, this.open, GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.IsTrue).DefaultState(this.close.idle);
		this.close.idle.PlayAnim("on").ParamTransition<bool>(this.WorkOrderGiven, this.close.work, GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.IsTrue);
		this.close.work.DefaultState(this.close.work.waitingForDupe);
		this.close.work.waitingForDupe.Enter(new StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State.Callback(GravitasLocker.StartlWorkChore_OpenLocker)).Exit(new StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State.Callback(GravitasLocker.StopWorkChore)).WorkableCompleteTransition((GravitasLocker.Instance smi) => smi.GetWorkable(), this.close.work.complete)
			.ParamTransition<bool>(this.WorkOrderGiven, this.close, GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.IsFalse);
		this.close.work.complete.Enter(delegate(GravitasLocker.Instance smi)
		{
			this.WorkOrderGiven.Set(false, smi, false);
		}).Enter(new StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State.Callback(GravitasLocker.Open)).TriggerOnEnter(GameHashes.UIRefresh, null);
		this.open.ParamTransition<bool>(this.IsOpen, this.close, GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.IsFalse).DefaultState(this.open.opening);
		this.open.opening.PlayAnim("working").OnAnimQueueComplete(this.open.idle);
		this.open.idle.PlayAnim("empty").Enter(new StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State.Callback(GravitasLocker.SpawnLoot)).ParamTransition<bool>(this.WorkOrderGiven, this.open.work, GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.IsTrue);
		this.open.work.DefaultState(this.open.work.waitingForDupe);
		this.open.work.waitingForDupe.Enter(new StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State.Callback(GravitasLocker.StartWorkChore_CloseLocker)).Exit(new StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State.Callback(GravitasLocker.StopWorkChore)).WorkableCompleteTransition((GravitasLocker.Instance smi) => smi.GetWorkable(), this.open.work.complete)
			.ParamTransition<bool>(this.WorkOrderGiven, this.open.idle, GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.IsFalse);
		this.open.work.complete.Enter(delegate(GravitasLocker.Instance smi)
		{
			this.WorkOrderGiven.Set(false, smi, false);
		}).Enter(new StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State.Callback(GravitasLocker.Close)).TriggerOnEnter(GameHashes.UIRefresh, null);
	}

	public static void Open(GravitasLocker.Instance smi)
	{
		smi.Open();
	}

	public static void Close(GravitasLocker.Instance smi)
	{
		smi.Close();
	}

	public static void SpawnLoot(GravitasLocker.Instance smi)
	{
		smi.SpawnLoot();
	}

	public static void StartWorkChore_CloseLocker(GravitasLocker.Instance smi)
	{
		smi.CreateWorkChore_CloseLocker();
	}

	public static void StartlWorkChore_OpenLocker(GravitasLocker.Instance smi)
	{
		smi.CreateWorkChore_OpenLocker();
	}

	public static void StopWorkChore(GravitasLocker.Instance smi)
	{
		smi.StopWorkChore();
	}

	public const float CLOSE_WORKTIME = 1f;

	public const float OPEN_WORKTIME = 1.5f;

	public const string CLOSED_ANIM_NAME = "on";

	public const string OPENING_ANIM_NAME = "working";

	public const string OPENED = "empty";

	private StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.BoolParameter IsOpen;

	private StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.BoolParameter WasEmptied;

	private StateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.BoolParameter WorkOrderGiven;

	public GravitasLocker.CloseStates close;

	public GravitasLocker.OpenStates open;

	public class Def : StateMachine.BaseDef
	{
		public bool CanBeClosed;

		public string SideScreen_OpenButtonText;

		public string SideScreen_OpenButtonTooltip;

		public string SideScreen_CancelOpenButtonText;

		public string SideScreen_CancelOpenButtonTooltip;

		public string SideScreen_CloseButtonText;

		public string SideScreen_CloseButtonTooltip;

		public string SideScreen_CancelCloseButtonText;

		public string SideScreen_CancelCloseButtonTooltip;

		public string OPEN_INTERACT_ANIM_NAME = "anim_interacts_clothingfactory_kanim";

		public string CLOSE_INTERACT_ANIM_NAME = "anim_interacts_clothingfactory_kanim";

		public string[] ObjectsToSpawn = new string[0];

		public string[] LootSymbols = new string[0];
	}

	public class WorkStates : GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State
	{
		public GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State waitingForDupe;

		public GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State complete;
	}

	public class CloseStates : GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State
	{
		public GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State idle;

		public GravitasLocker.WorkStates work;
	}

	public class OpenStates : GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State
	{
		public GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State opening;

		public GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.State idle;

		public GravitasLocker.WorkStates work;
	}

	public new class Instance : GameStateMachine<GravitasLocker, GravitasLocker.Instance, IStateMachineTarget, GravitasLocker.Def>.GameInstance, ISidescreenButtonControl
	{
		public bool WorkOrderGiven
		{
			get
			{
				return base.smi.sm.WorkOrderGiven.Get(base.smi);
			}
		}

		public bool IsOpen
		{
			get
			{
				return base.smi.sm.IsOpen.Get(base.smi);
			}
		}

		public bool HasContents
		{
			get
			{
				return !base.smi.sm.WasEmptied.Get(base.smi) && base.def.ObjectsToSpawn.Length != 0;
			}
		}

		public Workable GetWorkable()
		{
			return this.workable;
		}

		public void Open()
		{
			base.smi.sm.IsOpen.Set(true, base.smi, false);
		}

		public void Close()
		{
			base.smi.sm.IsOpen.Set(false, base.smi, false);
		}

		public Instance(IStateMachineTarget master, GravitasLocker.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			this.DefineDropSpawnPositions();
			base.StartSM();
			this.UpdateContentPreviewSymbols();
		}

		public void DefineDropSpawnPositions()
		{
			if (this.dropSpawnPositions == null && base.def.LootSymbols.Length != 0)
			{
				this.dropSpawnPositions = new Vector3[base.def.LootSymbols.Length];
				for (int i = 0; i < this.dropSpawnPositions.Length; i++)
				{
					bool flag;
					Vector3 vector = this.animController.GetSymbolTransform(base.def.LootSymbols[i], out flag).GetColumn(3);
					vector.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
					this.dropSpawnPositions[i] = (flag ? vector : base.gameObject.transform.GetPosition());
				}
			}
		}

		public void CreateWorkChore_CloseLocker()
		{
			if (this.chore == null)
			{
				this.workable.SetWorkTime(1f);
				this.chore = new WorkChore<Workable>(Db.Get().ChoreTypes.Repair, this.workable, null, true, null, null, null, true, null, false, true, Assets.GetAnim(base.def.CLOSE_INTERACT_ANIM_NAME), false, true, true, PriorityScreen.PriorityClass.high, 5, false, true);
			}
		}

		public void CreateWorkChore_OpenLocker()
		{
			if (this.chore == null)
			{
				this.workable.SetWorkTime(1.5f);
				this.chore = new WorkChore<Workable>(Db.Get().ChoreTypes.EmptyStorage, this.workable, null, true, null, null, null, true, null, false, true, Assets.GetAnim(base.def.OPEN_INTERACT_ANIM_NAME), false, true, true, PriorityScreen.PriorityClass.high, 5, false, true);
			}
		}

		public void StopWorkChore()
		{
			if (this.chore != null)
			{
				this.chore.Cancel("Canceled by user");
				this.chore = null;
			}
		}

		public void SpawnLoot()
		{
			if (this.HasContents)
			{
				for (int i = 0; i < base.def.ObjectsToSpawn.Length; i++)
				{
					string text = base.def.ObjectsToSpawn[i];
					GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 0, 0, text, Grid.SceneLayer.Ore);
					gameObject.SetActive(true);
					if (this.dropSpawnPositions != null && i < this.dropSpawnPositions.Length)
					{
						gameObject.transform.position = this.dropSpawnPositions[i];
					}
				}
				base.smi.sm.WasEmptied.Set(true, base.smi, false);
				this.UpdateContentPreviewSymbols();
			}
		}

		public void UpdateContentPreviewSymbols()
		{
			for (int i = 0; i < base.def.LootSymbols.Length; i++)
			{
				this.animController.SetSymbolVisiblity(base.def.LootSymbols[i], false);
			}
			if (this.HasContents)
			{
				for (int j = 0; j < Mathf.Min(base.def.LootSymbols.Length, base.def.ObjectsToSpawn.Length); j++)
				{
					KAnim.Build.Symbol symbolByIndex = Assets.GetPrefab(base.def.ObjectsToSpawn[j]).GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbolByIndex(0U);
					SymbolOverrideController component = base.gameObject.GetComponent<SymbolOverrideController>();
					string text = base.def.LootSymbols[j];
					component.AddSymbolOverride(text, symbolByIndex, 0);
					this.animController.SetSymbolVisiblity(text, true);
				}
			}
		}

		public string SidescreenButtonText
		{
			get
			{
				if (!this.IsOpen)
				{
					if (!this.WorkOrderGiven)
					{
						return base.def.SideScreen_OpenButtonText;
					}
					return base.def.SideScreen_CancelOpenButtonText;
				}
				else
				{
					if (!this.WorkOrderGiven)
					{
						return base.def.SideScreen_CloseButtonText;
					}
					return base.def.SideScreen_CancelCloseButtonText;
				}
			}
		}

		public string SidescreenButtonTooltip
		{
			get
			{
				if (!this.IsOpen)
				{
					if (!this.WorkOrderGiven)
					{
						return base.def.SideScreen_OpenButtonTooltip;
					}
					return base.def.SideScreen_CancelOpenButtonTooltip;
				}
				else
				{
					if (!this.WorkOrderGiven)
					{
						return base.def.SideScreen_CloseButtonTooltip;
					}
					return base.def.SideScreen_CancelCloseButtonTooltip;
				}
			}
		}

		public bool SidescreenEnabled()
		{
			return !this.IsOpen || base.def.CanBeClosed;
		}

		public bool SidescreenButtonInteractable()
		{
			return !this.IsOpen || base.def.CanBeClosed;
		}

		public int HorizontalGroupID()
		{
			return 0;
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}

		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
			throw new NotImplementedException();
		}

		public void OnSidescreenButtonPressed()
		{
			base.smi.sm.WorkOrderGiven.Set(!base.smi.sm.WorkOrderGiven.Get(base.smi), base.smi, false);
		}

		[MyCmpGet]
		private Workable workable;

		[MyCmpGet]
		private KBatchedAnimController animController;

		private Chore chore;

		private Vector3[] dropSpawnPositions;
	}
}
