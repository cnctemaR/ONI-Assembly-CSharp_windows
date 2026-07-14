using System;
using STRINGS;
using UnityEngine;

public class VentBubbleStates : GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.position_above_floor;
		this.position_above_floor.MoveTo(new Func<VentBubbleStates.Instance, int>(VentBubbleStates.GetCellAboveFloor), this.full, this.full, false);
		this.full.Enter(new StateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State.Callback(VentBubbleStates.DrainStomachToStorage)).Enter(new StateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State.Callback(VentBubbleStates.EnableBreathingLocation)).PlayAnim("full_pre", KAnim.PlayMode.Once)
			.QueueAnim("full_loop", true, null)
			.ToggleMainStatusItem(VentBubbleStates.InflatedStatus, null)
			.ScheduleGoTo(30f, this.venting)
			.WorkableStartTransition((VentBubbleStates.Instance smi) => this.GetWorkable(smi), this.dupe_venting);
		this.dupe_venting.ToggleMainStatusItem(VentBubbleStates.SharingAirStatus, null).Update(delegate(VentBubbleStates.Instance smi, float dt)
		{
			smi.EmitBubble(smi.def.emitMass * 0.25f, 0.75f);
		}, UpdateRate.SIM_1000ms, false).WorkableStopTransition((VentBubbleStates.Instance smi) => this.GetWorkable(smi), this.dupe_venting_pst)
			.Exit(new StateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State.Callback(VentBubbleStates.DisableBreathingLocation));
		this.dupe_venting_pst.Enter(delegate(VentBubbleStates.Instance smi)
		{
			if (smi.GetComponent<Storage>().IsEmpty())
			{
				smi.GoTo(this.empty);
				return;
			}
			smi.GoTo(this.full);
		});
		this.venting.PlayAnim("deflate_loop", KAnim.PlayMode.Loop).Enter(new StateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State.Callback(VentBubbleStates.DisableBreathingLocation)).ToggleMainStatusItem(VentBubbleStates.VentingStatus, null)
			.Update(new Action<VentBubbleStates.Instance, float>(VentBubbleStates.EmitBubble), UpdateRate.SIM_1000ms, false)
			.Transition(this.venting_pst, GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.Not(new StateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.Transition.ConditionCallback(VentBubbleStates.HasStoredElement)), UpdateRate.SIM_1000ms);
		this.venting_pst.PlayAnim("full_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.empty);
		this.empty.BehaviourComplete(GameTags.Creatures.Poop, false);
	}

	private static int GetCellAboveFloor(VentBubbleStates.Instance smi)
	{
		int num = Grid.PosToCell(smi.transform.GetPosition());
		if (!Grid.IsValidCell(num))
		{
			return Grid.InvalidCell;
		}
		int num2;
		int num3;
		Grid.CellToXY(num, out num2, out num3);
		for (int i = 0; i <= 8; i++)
		{
			for (int j = -1; j <= 1; j += 2)
			{
				if (i != 0 || j != -1)
				{
					int num4 = Grid.XYToCell(num2 + i * j, num3);
					if (Grid.IsValidCell(num4) && !Grid.Solid[num4])
					{
						int num5 = num4;
						int num6 = 0;
						while (Grid.IsValidCell(num5) && !Grid.Solid[num5] && num6 <= 32)
						{
							num5 = Grid.CellBelow(num5);
							num6++;
						}
						if (Grid.IsValidCell(num5) && Grid.Solid[num5])
						{
							if (num6 >= 3)
							{
								return VentBubbleStates.AvoidLiquidSurface(num4);
							}
							int num7 = num5;
							bool flag = false;
							for (int k = 0; k < 3; k++)
							{
								num7 = Grid.CellAbove(num7);
								if (!Grid.IsValidCell(num7) || Grid.Solid[num7])
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								return VentBubbleStates.AvoidLiquidSurface(num7);
							}
						}
					}
				}
			}
		}
		return num;
	}

	private static int AvoidLiquidSurface(int cell)
	{
		int num = Grid.CellAbove(cell);
		if (Grid.IsValidCell(num) && Grid.Element[cell].IsLiquid && !Grid.Element[num].IsLiquid)
		{
			int num2 = Grid.CellBelow(cell);
			if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
			{
				return num2;
			}
		}
		return cell;
	}

	private static bool HasStoredElement(VentBubbleStates.Instance smi)
	{
		return smi.HasStoredElement();
	}

	private static void EmitBubble(VentBubbleStates.Instance smi, float dt)
	{
		smi.EmitBubble(smi.def.emitMass, 0f);
	}

	private static void DrainStomachToStorage(VentBubbleStates.Instance smi)
	{
		smi.DrainStomachToStorage();
	}

	private Workable GetWorkable(VentBubbleStates.Instance smi)
	{
		UnderwaterBreathingLocationWorkable underwaterBreathingLocationWorkable = smi.Get<UnderwaterBreathingLocationWorkable>();
		underwaterBreathingLocationWorkable.overrideAnims = smi.def.dupebreathingAnimFiles;
		underwaterBreathingLocationWorkable.workAnims = smi.def.dupebreathingAnims;
		underwaterBreathingLocationWorkable.workingPstComplete = smi.def.dupebreathingPst;
		underwaterBreathingLocationWorkable.workingPstFailed = smi.def.dupebreathingPst;
		underwaterBreathingLocationWorkable.synchronizeAnims = true;
		underwaterBreathingLocationWorkable.workLayer = Grid.SceneLayer.Move;
		underwaterBreathingLocationWorkable.SetWorkerStatusItem(VentBubbleStates.DupeConsumingAirStatus);
		return smi.Get<UnderwaterBreathingLocationWorkable>();
	}

	private static void EnableBreathingLocation(VentBubbleStates.Instance smi)
	{
		UnderwaterBreathingLocation component = smi.GetComponent<UnderwaterBreathingLocation>();
		if (component != null)
		{
			component.MarkCells();
		}
	}

	private static void DisableBreathingLocation(VentBubbleStates.Instance smi)
	{
		UnderwaterBreathingLocation component = smi.GetComponent<UnderwaterBreathingLocation>();
		if (component != null)
		{
			component.UnmarkCells();
		}
	}

	private const float INFLATED_DURATION = 30f;

	private const int INFLATE_POSITION_MIN_TILES_ABOVE_FLOOR = 3;

	public static StatusItem InflatedStatus = new StatusItem("InflatedStatus", CREATURES.STATUSITEMS.PUFFER_INFLATED.NAME, CREATURES.STATUSITEMS.PUFFER_INFLATED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022, true, null);

	public static StatusItem VentingStatus = new StatusItem("VentingStatus", CREATURES.STATUSITEMS.PUFFER_VENTING.NAME, CREATURES.STATUSITEMS.PUFFER_VENTING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022, true, null);

	public static StatusItem SharingAirStatus = new StatusItem("SharingAirStatus", CREATURES.STATUSITEMS.PUFFER_SHARING_AIR.NAME, CREATURES.STATUSITEMS.PUFFER_SHARING_AIR.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022, true, null);

	public static StatusItem DupeConsumingAirStatus = new StatusItem("SharingAirStatus", DUPLICANTS.STATUSITEMS.PUFFER_SHARING_AIR.NAME, DUPLICANTS.STATUSITEMS.PUFFER_SHARING_AIR.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022, true, null);

	public GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State position_above_floor;

	public GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State full;

	public GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State venting;

	public GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State venting_pst;

	public GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State empty;

	public GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State dupe_venting;

	public GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.State dupe_venting_pst;

	public class Def : StateMachine.BaseDef
	{
		public SimHashes element;

		public float emitMass = 1f;

		public KAnimFile[] dupebreathingAnimFiles;

		public HashedString[] dupebreathingAnims;

		public HashedString[] dupebreathingPst;
	}

	public new class Instance : GameStateMachine<VentBubbleStates, VentBubbleStates.Instance, IStateMachineTarget, VentBubbleStates.Def>.GameInstance
	{
		public Instance(Chore<VentBubbleStates.Instance> chore, VentBubbleStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(VentBubbleStates.Instance.ShouldInflate, null);
			this.elementTag = ElementLoader.FindElementByHash(def.element).tag;
		}

		protected override void OnCleanUp()
		{
			VentBubbleStates.DisableBreathingLocation(this);
			base.OnCleanUp();
		}

		public void DrainStomachToStorage()
		{
			if (base.gameObject.GetSMI<CreatureCalorieMonitor.Instance>() == null)
			{
				return;
			}
			base.gameObject.Trigger(-667597687, new PoopData(false, this.storage, null, null));
		}

		public bool HasStoredElement()
		{
			GameObject gameObject = this.storage.FindFirst(this.elementTag);
			return !(gameObject == null) && gameObject.GetComponent<PrimaryElement>().Mass > 0f;
		}

		public void EmitBubble(float min_mass, float y_offset = 0f)
		{
			GameObject gameObject = this.storage.FindFirst(this.elementTag);
			if (gameObject == null)
			{
				return;
			}
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			float mass = component.Mass;
			if (mass <= 0f)
			{
				return;
			}
			float num = Mathf.Min(mass, min_mass);
			component.Mass -= num;
			Vector3 position = base.master.transform.GetPosition();
			position.y += 0.75f;
			Facing component2 = base.GetComponent<Facing>();
			if (component2 != null)
			{
				position.x += (component2.GetFacing() ? (-0.45f) : 0.45f);
			}
			position.y += y_offset;
			BubbleManager.instance.SpawnBubble(base.def.element, position, num, component.Temperature, BubbleManager.Disease.None, null);
		}

		[MyCmpGet]
		private Storage storage;

		private Tag elementTag;

		private static Chore.Precondition ShouldInflate = new Chore.Precondition
		{
			id = "ShouldInflate",
			description = "__ Blowter has no oxygen",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return !(context.consumerState.consumer == null) && (context.consumerState.gameObject.GetComponent<Storage>().MassStored() > 0f || context.consumerState.consumer.RunBehaviourPrecondition(GameTags.Creatures.Poop));
			}
		};
	}
}
