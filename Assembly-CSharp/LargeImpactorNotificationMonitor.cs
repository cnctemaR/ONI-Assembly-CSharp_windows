using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LargeImpactorNotificationMonitor : GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.undiscovered;
		this.undiscovered.ParamTransition<bool>(this.HasBeenDiscovered, this.discovered, GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.IsTrue).EventHandler(GameHashes.DiscoveredSpace, (LargeImpactorNotificationMonitor.Instance smi) => Game.Instance, new GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.GameEvent.Callback(LargeImpactorNotificationMonitor.OnDuplicantReachedSpace)).EventHandler(GameHashes.DLCPOICompleted, (LargeImpactorNotificationMonitor.Instance smi) => Game.Instance, new GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.GameEvent.Callback(LargeImpactorNotificationMonitor.OnPOIActivated));
		this.discovered.DefaultState(this.discovered.sequence);
		this.discovered.sequence.ParamTransition<bool>(this.SequenceCompleted, this.discovered.notification, GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.IsTrue).Enter(new StateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State.Callback(LargeImpactorNotificationMonitor.RevealSurface)).Enter(new StateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State.Callback(LargeImpactorNotificationMonitor.PlaySequence))
			.EventHandler(GameHashes.SequenceCompleted, new StateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State.Callback(LargeImpactorNotificationMonitor.CompleteSequence));
		this.discovered.notification.DefaultState(this.discovered.notification.delayEntry);
		this.discovered.notification.delayEntry.ScheduleGoTo(3f, this.discovered.notification.running);
		this.discovered.notification.running.Enter(new StateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State.Callback(LargeImpactorNotificationMonitor.PlayNotificationEnterSound)).Enter(new StateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State.Callback(LargeImpactorNotificationMonitor.SetLandingZoneVisualizationToActive)).ScheduleAction("Toggle off the visualization after a delay", 2f, new Action<LargeImpactorNotificationMonitor.Instance>(LargeImpactorNotificationMonitor.FoldTheVisualization))
			.ToggleNotification((LargeImpactorNotificationMonitor.Instance smi) => smi.notification);
	}

	public static void CompleteSequence(LargeImpactorNotificationMonitor.Instance smi)
	{
		smi.sm.SequenceCompleted.Set(true, smi, false);
	}

	public static void Discover(LargeImpactorNotificationMonitor.Instance smi)
	{
		smi.sm.HasBeenDiscovered.Set(true, smi, false);
	}

	public static void RevealSurface(LargeImpactorNotificationMonitor.Instance smi)
	{
		smi.RevealSurface();
	}

	public static void PlayNotificationEnterSound(LargeImpactorNotificationMonitor.Instance smi)
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound("Notification_Imperative", false));
	}

	public static void SetLandingZoneVisualizationToActive(LargeImpactorNotificationMonitor.Instance smi)
	{
		smi.GetComponent<LargeImpactorVisualizer>().Active = true;
	}

	public static void FoldTheVisualization(LargeImpactorNotificationMonitor.Instance smi)
	{
		LargeImpactorVisualizer component = smi.GetComponent<LargeImpactorVisualizer>();
		if (!component.Folded)
		{
			component.SetFoldedState(true);
		}
	}

	public static void OnPOIActivated(LargeImpactorNotificationMonitor.Instance smi, object obj)
	{
		if (((GameObject)obj).PrefabID() == "POIDlc4TechUnlock")
		{
			LargeImpactorNotificationMonitor.Discover(smi);
		}
	}

	public static void OnDuplicantReachedSpace(LargeImpactorNotificationMonitor.Instance smi, object obj)
	{
		int myWorldId = ((GameObject)obj).GetMyWorldId();
		int myWorldId2 = smi.gameObject.GetMyWorldId();
		if (myWorldId == myWorldId2)
		{
			LargeImpactorNotificationMonitor.Discover(smi);
		}
	}

	public static void PlaySequence(LargeImpactorNotificationMonitor.Instance smi)
	{
		smi.PlaySequence();
	}

	public const string NOTIFICATION_PREFAB_ID = "LargeImpactNotification";

	public GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State undiscovered;

	public LargeImpactorNotificationMonitor.DiscoveredStates discovered;

	public StateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.BoolParameter HasBeenDiscovered;

	public StateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.BoolParameter SequenceCompleted;

	public class Def : StateMachine.BaseDef
	{
	}

	public class NotificationStates : GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State
	{
		public GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State delayEntry;

		public GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State running;
	}

	public class DiscoveredStates : GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State
	{
		public GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.State sequence;

		public LargeImpactorNotificationMonitor.NotificationStates notification;
	}

	public new class Instance : GameStateMachine<LargeImpactorNotificationMonitor, LargeImpactorNotificationMonitor.Instance, IStateMachineTarget, LargeImpactorNotificationMonitor.Def>.GameInstance
	{
		public bool HasRevealSequencePlayed
		{
			get
			{
				return base.sm.SequenceCompleted.Get(this);
			}
		}

		public Notification notification { get; private set; }

		public Instance(IStateMachineTarget master, LargeImpactorNotificationMonitor.Def def)
			: base(master, def)
		{
			this.notifier = base.gameObject.AddOrGet<Notifier>();
			LargeImpactorStatus.Instance smi = base.smi.GetSMI<LargeImpactorStatus.Instance>();
			string text = MISC.NOTIFICATIONS.INCOMINGPREHISTORICASTEROIDNOTIFICATION.NAME;
			NotificationType notificationType = NotificationType.Custom;
			object obj = smi;
			this.notification = new Notification(text, notificationType, new Func<List<Notification>, object, string>(this.ResolveNotificationTooltip), obj, false, 0f, null, null, null, true, false, false);
			this.notification.customNotificationID = "LargeImpactNotification";
		}

		private string ResolveNotificationTooltip(List<Notification> not, object data)
		{
			LargeImpactorStatus.Instance instance = (LargeImpactorStatus.Instance)data;
			return GameUtil.SafeStringFormat(MISC.NOTIFICATIONS.INCOMINGPREHISTORICASTEROIDNOTIFICATION.TOOLTIP, new object[]
			{
				GameUtil.GetFormattedInt((float)instance.Health, GameUtil.TimeSlice.None),
				GameUtil.GetFormattedInt((float)instance.def.MAX_HEALTH, GameUtil.TimeSlice.None),
				GameUtil.GetFormattedCycles(instance.TimeRemainingBeforeCollision, "F1", false)
			});
		}

		public void RevealSurface()
		{
			GameplayEventInstance gameplayEventInstance = GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.Id, -1);
			if (gameplayEventInstance != null)
			{
				WorldContainer world = ClusterManager.Instance.GetWorld(gameplayEventInstance.worldId);
				if (world != null && !world.IsSurfaceRevealed)
				{
					world.RevealSurface();
				}
			}
		}

		public void SetNotificationVisibility(bool visible)
		{
			if (visible)
			{
				this.notifier.Add(this.notification, "");
				return;
			}
			this.notifier.Remove(this.notification);
		}

		public void PlaySequence()
		{
			this.AbortSequenceCoroutine();
			this.CreateReticleForSequence();
			GameplayEventInstance gameplayEventInstance = GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.Id, -1);
			if (gameplayEventInstance != null)
			{
				WorldContainer world = ClusterManager.Instance.GetWorld(gameplayEventInstance.worldId);
				this.sequenceCoroutine = LargeImpactorRevealSequence.Start(this.notifier, this.sequenceReticle, world);
			}
		}

		private void CreateReticleForSequence()
		{
			this.DeleteReticleObject();
			this.sequenceReticle = Util.KInstantiateUI<LargeImpactorSequenceUIReticle>(ScreenPrefabs.Instance.largeImpactorSequenceReticlePrefab.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, true);
			LargeImpactorStatus.Instance smi = base.gameObject.GetSMI<LargeImpactorStatus.Instance>();
			this.sequenceReticle.SetTarget(smi);
		}

		private void DeleteReticleObject()
		{
			if (this.sequenceReticle != null)
			{
				this.sequenceReticle.gameObject.DeleteObject();
			}
		}

		private void AbortSequenceCoroutine()
		{
			if (this.sequenceCoroutine != null)
			{
				this.notifier.StopCoroutine(this.sequenceCoroutine);
				this.sequenceCoroutine = null;
			}
		}

		protected override void OnCleanUp()
		{
			this.AbortSequenceCoroutine();
			this.DeleteReticleObject();
			base.OnCleanUp();
		}

		private Notifier notifier;

		private Coroutine sequenceCoroutine;

		private LargeImpactorSequenceUIReticle sequenceReticle;
	}
}
