using System;
using STRINGS;
using UnityEngine;

public class ConduitSleepStates : GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.connector.moveToSleepLocation;
		this.root.EventTransition(GameHashes.NewDay, (ConduitSleepStates.Instance smi) => GameClock.Instance, this.behaviourcomplete, null).Exit(new StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State.Callback(ConduitSleepStates.CleanUp));
		this.connector.moveToSleepLocation.ToggleStatusItem(CREATURES.STATUSITEMS.DROWSY.NAME, CREATURES.STATUSITEMS.DROWSY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).MoveTo(delegate(ConduitSleepStates.Instance smi)
		{
			ConduitSleepMonitor.Instance smi2 = smi.GetSMI<ConduitSleepMonitor.Instance>();
			return smi2.sm.targetSleepCell.Get(smi2);
		}, this.drowsy, this.behaviourcomplete, false);
		this.drowsy.ToggleStatusItem(CREATURES.STATUSITEMS.DROWSY.NAME, CREATURES.STATUSITEMS.DROWSY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Enter(delegate(ConduitSleepStates.Instance smi)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Ceiling);
		}).Enter(delegate(ConduitSleepStates.Instance smi)
		{
			if (GameClock.Instance.IsNighttime())
			{
				smi.GoTo(this.connector.sleep);
			}
		})
			.DefaultState(this.drowsy.loop);
		this.drowsy.loop.PlayAnim("drowsy_pre").QueueAnim("drowsy_loop", true, null).EventTransition(GameHashes.Nighttime, (ConduitSleepStates.Instance smi) => GameClock.Instance, this.drowsy.pst, (ConduitSleepStates.Instance smi) => GameClock.Instance.IsNighttime());
		this.drowsy.pst.PlayAnim("drowsy_pst").OnAnimQueueComplete(this.connector.sleep);
		this.connector.sleep.ToggleStatusItem(CREATURES.STATUSITEMS.SLEEPING.NAME, CREATURES.STATUSITEMS.SLEEPING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Enter(delegate(ConduitSleepStates.Instance smi)
		{
			if (!smi.staterpillar.IsConnectorBuildingSpawned())
			{
				smi.GoTo(this.behaviourcomplete);
				return;
			}
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Ceiling);
			smi.staterpillar.EnableConnector();
			if (smi.staterpillar.IsConnected())
			{
				smi.GoTo(this.connector.sleep.connected);
				return;
			}
			smi.GoTo(this.connector.sleep.noConnection);
		});
		this.connector.sleep.connected.Enter(delegate(ConduitSleepStates.Instance smi)
		{
			smi.animController.SetSceneLayer(ConduitSleepStates.GetSleepingLayer(smi));
		}).Exit(delegate(ConduitSleepStates.Instance smi)
		{
			smi.animController.SetSceneLayer(Grid.SceneLayer.Creatures);
		}).EventTransition(GameHashes.NewDay, (ConduitSleepStates.Instance smi) => GameClock.Instance, this.connector.connectedWake, null)
			.Transition(this.connector.sleep.noConnection, (ConduitSleepStates.Instance smi) => !smi.staterpillar.IsConnected(), UpdateRate.SIM_200ms)
			.PlayAnim("sleep_charging_pre")
			.QueueAnim("sleep_charging_loop", true, null)
			.Update(new Action<ConduitSleepStates.Instance, float>(ConduitSleepStates.UpdateGulpSymbol), UpdateRate.SIM_1000ms, false)
			.EventHandler(GameHashes.OnStorageChange, new GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.GameEvent.Callback(ConduitSleepStates.OnStorageChanged));
		this.connector.sleep.noConnection.PlayAnim("sleep_pre").QueueAnim("sleep_loop", true, null).ToggleStatusItem(new Func<ConduitSleepStates.Instance, StatusItem>(ConduitSleepStates.GetStatusItem), null)
			.EventTransition(GameHashes.NewDay, (ConduitSleepStates.Instance smi) => GameClock.Instance, this.connector.noConnectionWake, null)
			.Transition(this.connector.sleep.connected, (ConduitSleepStates.Instance smi) => smi.staterpillar.IsConnected(), UpdateRate.SIM_200ms);
		this.connector.connectedWake.QueueAnim("sleep_charging_pst", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.connector.noConnectionWake.QueueAnim("sleep_pst", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsConduitConnection, false);
	}

	private static Grid.SceneLayer GetSleepingLayer(ConduitSleepStates.Instance smi)
	{
		ObjectLayer conduitLayer = smi.staterpillar.conduitLayer;
		Grid.SceneLayer sceneLayer;
		if (conduitLayer != ObjectLayer.GasConduit)
		{
			if (conduitLayer != ObjectLayer.LiquidConduit)
			{
				if (conduitLayer == ObjectLayer.Wire)
				{
					sceneLayer = Grid.SceneLayer.SolidConduitBridges;
				}
				else
				{
					sceneLayer = Grid.SceneLayer.SolidConduitBridges;
				}
			}
			else
			{
				sceneLayer = Grid.SceneLayer.GasConduitBridges;
			}
		}
		else
		{
			sceneLayer = Grid.SceneLayer.Gas;
		}
		return sceneLayer;
	}

	private static StatusItem GetStatusItem(ConduitSleepStates.Instance smi)
	{
		ObjectLayer conduitLayer = smi.staterpillar.conduitLayer;
		StatusItem statusItem;
		if (conduitLayer != ObjectLayer.GasConduit)
		{
			if (conduitLayer != ObjectLayer.LiquidConduit)
			{
				if (conduitLayer == ObjectLayer.Wire)
				{
					statusItem = Db.Get().BuildingStatusItems.NoWireConnected;
				}
				else
				{
					statusItem = Db.Get().BuildingStatusItems.Normal;
				}
			}
			else
			{
				statusItem = Db.Get().BuildingStatusItems.NeedLiquidOut;
			}
		}
		else
		{
			statusItem = Db.Get().BuildingStatusItems.NeedGasOut;
		}
		return statusItem;
	}

	private static void OnStorageChanged(ConduitSleepStates.Instance smi, object obj)
	{
		GameObject gameObject = obj as GameObject;
		if (gameObject != null)
		{
			smi.amountDeposited += gameObject.GetComponent<PrimaryElement>().Mass;
		}
	}

	private static void UpdateGulpSymbol(ConduitSleepStates.Instance smi, float dt)
	{
		smi.SetGulpSymbolVisibility(smi.amountDeposited > 0f);
		smi.amountDeposited = 0f;
	}

	private static void CleanUp(ConduitSleepStates.Instance smi)
	{
		ConduitSleepMonitor.Instance smi2 = smi.GetSMI<ConduitSleepMonitor.Instance>();
		if (smi2 != null)
		{
			smi2.sm.targetSleepCell.Set(Grid.InvalidCell, smi2, false);
		}
		smi.staterpillar.DestroyOrphanedConnectorBuilding();
	}

	public ConduitSleepStates.DrowsyStates drowsy;

	public ConduitSleepStates.HasConnectorStates connector;

	public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
		public HashedString gulpSymbol = "gulp";
	}

	public new class Instance : GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.GameInstance
	{
		public Instance(Chore<ConduitSleepStates.Instance> chore, ConduitSleepStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsConduitConnection);
		}

		public void SetGulpSymbolVisibility(bool state)
		{
			string sound = GlobalAssets.GetSound("PlugSlug_Charging_Gulp_LP", false);
			if (this.gulpSymbolVisible != state)
			{
				this.gulpSymbolVisible = state;
				this.animController.SetSymbolVisiblity(base.def.gulpSymbol, state);
				if (state)
				{
					this.loopingSounds.StartSound(sound);
					return;
				}
				this.loopingSounds.StopSound(sound);
			}
		}

		[MyCmpReq]
		public KBatchedAnimController animController;

		[MyCmpReq]
		public Staterpillar staterpillar;

		[MyCmpAdd]
		private LoopingSounds loopingSounds;

		public bool gulpSymbolVisible;

		public float amountDeposited;
	}

	public class SleepStates : GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State
	{
		public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State connected;

		public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State noConnection;
	}

	public class DrowsyStates : GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State
	{
		public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State loop;

		public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State pst;
	}

	public class HasConnectorStates : GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State
	{
		public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State moveToSleepLocation;

		public ConduitSleepStates.SleepStates sleep;

		public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State noConnectionWake;

		public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State connectedWake;
	}
}
