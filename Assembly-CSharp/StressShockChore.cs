using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class StressShockChore : Chore<StressShockChore.StatesInstance>
{
	private static bool CheckBlocked(int sourceCell, int destinationCell)
	{
		HashSet<int> hashSet = new HashSet<int>();
		Grid.CollectCellsInLine(sourceCell, destinationCell, hashSet);
		bool flag = false;
		foreach (int num in hashSet)
		{
			if (Grid.Solid[num])
			{
				flag = true;
				break;
			}
		}
		return flag;
	}

	public static void AddBatteryDrainModifier(StressShockChore.StatesInstance smi)
	{
		smi.SetDrainModifierActiveState(true);
	}

	public static void RemoveBatteryDrainModifier(StressShockChore.StatesInstance smi)
	{
		smi.SetDrainModifierActiveState(false);
	}

	public StressShockChore(ChoreType chore_type, IStateMachineTarget target, Notification notification, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.StressShock, target, target.GetComponent<ChoreProvider>(), false, on_complete, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StressShockChore.StatesInstance(this, target.gameObject, notification);
	}

	public class StatesInstance : GameStateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.GameInstance
	{
		public StatesInstance(StressShockChore master, GameObject shocker, Notification notification)
			: base(master)
		{
			base.sm.shocker.Set(shocker, base.smi, false);
			this.notification = notification;
		}

		public void SetDrainModifierActiveState(bool draining)
		{
			if (draining)
			{
				this.batteryMonitor.AddOrUpdateModifier(this.powerDrainModifier, true);
				return;
			}
			this.batteryMonitor.RemoveModifier(this.powerDrainModifier.id, true);
		}

		public void FindDestination()
		{
			int num = this.FindIdleCell();
			base.sm.targetMoveLocation.Set(num, base.smi, false);
			this.GoTo(base.sm.shocking.runAroundShockingStuff);
		}

		private int FindIdleCell()
		{
			Navigator component = base.smi.master.GetComponent<Navigator>();
			MinionPathFinderAbilities minionPathFinderAbilities = (MinionPathFinderAbilities)component.GetCurrentAbilities();
			minionPathFinderAbilities.SetIdleNavMaskEnabled(true);
			IdleCellQuery idleCellQuery = PathFinderQueries.idleCellQuery.Reset(base.GetComponent<MinionBrain>(), global::UnityEngine.Random.Range(90, 180));
			component.RunQuery(idleCellQuery);
			minionPathFinderAbilities.SetIdleNavMaskEnabled(false);
			return idleCellQuery.GetResultCell();
		}

		public void PickShockTarget(StressShockChore.StatesInstance smi)
		{
			int num = Grid.PosToCell(smi.master.gameObject);
			int num2 = (int)Grid.WorldIdx[num];
			List<GameObject> list = new List<GameObject>();
			float num3 = global::UnityEngine.Random.Range(0f, 2f);
			foreach (Health health in Components.Health.GetWorldItems(num2, false))
			{
				if (!health.IsNullOrDestroyed() && !(health.gameObject == smi.master.gameObject))
				{
					int num4 = Grid.PosToCell(health);
					float num5 = Vector2.Distance(Grid.CellToPos2D(num), Grid.CellToPos2D(num4));
					if (num5 <= (float)STRESS.SHOCKER.SHOCK_RADIUS && num5 > num3 && !StressShockChore.CheckBlocked(num, num4))
					{
						list.Add(health.gameObject);
					}
				}
			}
			Vector2I vector2I = Grid.CellToXY(num);
			List<ScenePartitionerEntry> list2 = new List<ScenePartitionerEntry>();
			GameScenePartitioner.Instance.GatherEntries(vector2I.x - STRESS.SHOCKER.SHOCK_RADIUS, vector2I.y - STRESS.SHOCKER.SHOCK_RADIUS, STRESS.SHOCKER.SHOCK_RADIUS * 2, STRESS.SHOCKER.SHOCK_RADIUS * 2, GameScenePartitioner.Instance.objectLayers[42], list2);
			foreach (ScenePartitionerEntry scenePartitionerEntry in list2)
			{
				if (!StressShockChore.CheckBlocked(num, Grid.PosToCell(new Vector2((float)scenePartitionerEntry.x, (float)scenePartitionerEntry.y))) && scenePartitionerEntry.obj as GameObject != null)
				{
					list.Add(scenePartitionerEntry.obj as GameObject);
				}
			}
			if (list.Count == 0)
			{
				Vector2I vector2I2 = Grid.CellToXY(num);
				List<ScenePartitionerEntry> list3 = new List<ScenePartitionerEntry>();
				GameScenePartitioner.Instance.GatherEntries(vector2I2.x - STRESS.SHOCKER.SHOCK_RADIUS, vector2I2.y - STRESS.SHOCKER.SHOCK_RADIUS, STRESS.SHOCKER.SHOCK_RADIUS * 2, STRESS.SHOCKER.SHOCK_RADIUS * 2, GameScenePartitioner.Instance.completeBuildings, list3);
				foreach (ScenePartitionerEntry scenePartitionerEntry2 in list3)
				{
					if (!StressShockChore.CheckBlocked(num, Grid.PosToCell(new Vector2((float)scenePartitionerEntry2.x, (float)scenePartitionerEntry2.y))))
					{
						BuildingComplete buildingComplete = scenePartitionerEntry2.obj as BuildingComplete;
						if (buildingComplete != null)
						{
							list.Add(buildingComplete.gameObject);
						}
					}
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			GameObject random = list.GetRandom<GameObject>();
			GameObject gameObject = random;
			float num6 = float.MaxValue;
			foreach (GameObject gameObject2 in list)
			{
				if (list.Count <= 1 || !(gameObject2 == base.sm.previousTarget.Get(smi)))
				{
					float num7 = Vector2.Distance(base.transform.position, gameObject2.transform.position);
					if (num7 < num6)
					{
						num6 = num7;
						gameObject = gameObject2;
					}
				}
			}
			if (random != null && gameObject != null && global::UnityEngine.Random.Range(0, 100) > 50)
			{
				base.sm.beamTarget.Set(gameObject, smi, false);
				return;
			}
			base.sm.beamTarget.Set(gameObject, smi, false);
		}

		public void MakeBeam()
		{
			GameObject gameObject = new GameObject("shockFX");
			gameObject.SetActive(false);
			KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
			base.sm.beamFX.Set(kbatchedAnimController, base.smi, false);
			kbatchedAnimController.SwapAnims(new KAnimFile[] { Assets.GetAnim("bionic_dupe_stress_beam_fx_kanim") });
			gameObject.SetActive(true);
			bool flag;
			Vector3 vector = base.GetComponent<KBatchedAnimController>().GetSymbolTransform("snapTo_hat", out flag).GetColumn(3);
			vector -= Vector3.up / 4f;
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
			gameObject.transform.position = vector;
			gameObject.transform.parent = base.transform;
			kbatchedAnimController.Play("lightning_beam_comp", KAnim.PlayMode.Loop, 1f, 0f);
			GameObject gameObject2 = new GameObject("impactFX");
			gameObject2.SetActive(false);
			KBatchedAnimController kbatchedAnimController2 = gameObject2.AddComponent<KBatchedAnimController>();
			base.sm.impactFX.Set(kbatchedAnimController2, base.smi, false);
			kbatchedAnimController2.SwapAnims(new KAnimFile[] { Assets.GetAnim("bionic_dupe_stress_beam_impact_fx_kanim") });
			gameObject2.SetActive(true);
			kbatchedAnimController2.Play("stress_beam_impact_fx", KAnim.PlayMode.Loop, 1f, 0f);
		}

		public void ClearBeam(int beamIdx)
		{
			base.sm.previousTarget.Set(base.sm.beamTarget.Get(base.smi), base.smi, false);
			base.sm.beamTarget.Set(null, base.smi, false);
			if (base.sm.beamFX.Get(base.smi) != null)
			{
				Util.KDestroyGameObject(base.sm.beamFX.Get(base.smi).gameObject);
				base.sm.beamFX.Set(null, base.smi, false);
			}
			if (base.sm.impactFX.Get(base.smi) != null)
			{
				Util.KDestroyGameObject(base.sm.impactFX.Get(base.smi).gameObject);
				base.sm.impactFX.Set(null, base.smi, false);
			}
		}

		public void AimBeam(Vector3 targetPosition, int beamIdx)
		{
			Vector3 vector = Vector3.Normalize(targetPosition - base.smi.sm.beamFX.Get(base.smi).transform.position);
			float num = MathUtil.AngleSigned(Vector3.up, vector, Vector3.forward) + 90f;
			base.smi.sm.beamFX.Get(base.smi).Rotation = num;
			float num2 = Vector2.Distance(base.smi.sm.beamTarget.Get(base.smi).transform.position, base.smi.sm.beamFX.Get(base.smi).transform.position) / 2.5f;
			base.smi.sm.beamFX.Get(base.smi).animWidth = num2;
			base.smi.sm.impactFX.Get(base.smi).transform.position = targetPosition;
		}

		public void ShowBeam(bool show)
		{
			base.smi.sm.impactFX.Get(base.smi).enabled = show;
			base.smi.sm.beamFX.Get(base.smi).enabled = show;
		}

		public Notification notification;

		[MySmiReq]
		public BionicBatteryMonitor.Instance batteryMonitor;

		public BionicBatteryMonitor.WattageModifier powerDrainModifier = new BionicBatteryMonitor.WattageModifier("StressShockChore", string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.STANDARD_ACTIVE_TEMPLATE, DUPLICANTS.TRAITS.STRESSSHOCKER.DRAIN_ATTRIBUTE, "<b>+</b>" + GameUtil.GetFormattedWattage(STRESS.SHOCKER.POWER_CONSUMPTION_RATE, GameUtil.WattageFormatterUnit.Automatic, true)), STRESS.SHOCKER.POWER_CONSUMPTION_RATE, STRESS.SHOCKER.POWER_CONSUMPTION_RATE);
	}

	public class States : GameStateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.shocking.findDestination;
			base.serializable = StateMachine.SerializeType.Never;
			base.Target(this.shocker);
			this.shocking.DefaultState(this.shocking.findDestination).ToggleAnims("anim_loco_stressshocker_kanim", 0f).Enter(delegate(StressShockChore.StatesInstance smi)
			{
				smi.MakeBeam();
			})
				.Exit(delegate(StressShockChore.StatesInstance smi)
				{
					smi.ClearBeam(0);
				});
			this.shocking.findDestination.Enter("FindDestination", delegate(StressShockChore.StatesInstance smi)
			{
				smi.ShowBeam(false);
				smi.FindDestination();
			});
			this.shocking.runAroundShockingStuff.MoveTo((StressShockChore.StatesInstance smi) => smi.sm.targetMoveLocation.Get(smi), this.shocking.findDestination, this.delay, false).ParamTransition<float>(this.powerConsumed, this.complete, (StressShockChore.StatesInstance smi, float p) => p >= STRESS.SHOCKER.MAX_POWER_USE).Toggle("BatteryDrain", new StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.State.Callback(StressShockChore.AddBatteryDrainModifier), new StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.State.Callback(StressShockChore.RemoveBatteryDrainModifier))
				.Enter(delegate(StressShockChore.StatesInstance smi)
				{
					smi.ShowBeam(true);
				})
				.Update(delegate(StressShockChore.StatesInstance smi, float dt)
				{
					smi.PickShockTarget(smi);
					float num = dt * STRESS.SHOCKER.POWER_CONSUMPTION_RATE;
					smi.sm.powerConsumed.Delta(-num, smi);
					smi.batteryMonitor.ConsumePower(-num);
					if (smi.sm.beamTarget.Get(smi) != null)
					{
						Health component = smi.sm.beamTarget.Get(smi).GetComponent<Health>();
						if (component != null)
						{
							component.Damage(dt * STRESS.SHOCKER.DAMAGE_RATE);
							return;
						}
						if (smi.sm.beamTarget.Get(smi).HasTag(GameTags.Wires))
						{
							BuildingHP component2 = smi.sm.beamTarget.Get(smi).GetComponent<BuildingHP>();
							if (component2 != null)
							{
								component2.DoDamage(Mathf.RoundToInt(dt * STRESS.SHOCKER.DAMAGE_RATE));
							}
						}
					}
				}, UpdateRate.SIM_200ms, false)
				.Update(delegate(StressShockChore.StatesInstance smi, float dt)
				{
					if (smi.sm.beamTarget.Get(smi) != null)
					{
						Vector3 vector = smi.sm.beamTarget.Get(smi).transform.position + Vector3.up / 2f;
						if (!StressShockChore.CheckBlocked(Grid.PosToCell(smi.sm.beamFX.Get(smi).transform.position), Grid.PosToCell(vector)))
						{
							smi.AimBeam(vector, 0);
						}
					}
				}, UpdateRate.RENDER_EVERY_TICK, false);
			this.delay.ScheduleGoTo(0.5f, this.shocking);
			this.complete.Enter(delegate(StressShockChore.StatesInstance smi)
			{
				smi.StopSM("complete");
			});
		}

		public StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.TargetParameter shocker;

		public StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.ObjectParameter<KBatchedAnimController[]> cosmeticBeamFXs;

		public StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.ObjectParameter<KBatchedAnimController> beamFX;

		public StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.ObjectParameter<KBatchedAnimController> impactFX;

		public StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.ObjectParameter<GameObject> beamTarget;

		public StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.ObjectParameter<GameObject> previousTarget;

		public StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.IntParameter targetMoveLocation;

		public StateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.FloatParameter powerConsumed;

		public StressShockChore.States.ShockStates shocking;

		public GameStateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.State delay;

		public GameStateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.State complete;

		public class ShockStates : GameStateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.State
		{
			public GameStateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.State findDestination;

			public GameStateMachine<StressShockChore.States, StressShockChore.StatesInstance, StressShockChore, object>.State runAroundShockingStuff;
		}
	}
}
