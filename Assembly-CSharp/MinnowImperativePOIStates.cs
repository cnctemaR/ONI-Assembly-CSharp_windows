using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MinnowImperativePOIStates : GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.off;
		this.off.PlayAnim("empty_water").ParamTransition<bool>(this.hasShownCompletedPopup, this.off_poi_completed, GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.IsTrue).ParamTransition<bool>(this.isCompleted, this.poi_completed_pending, GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.IsTrue)
			.Transition(this.on.working, (MinnowImperativePOIStates.Instance smi) => MinnowImperativePOIStates.HasLiquid(smi) && smi.sm.hasClickedSideScreen.Get(smi), UpdateRate.SIM_200ms)
			.Transition(this.on.enter, (MinnowImperativePOIStates.Instance smi) => MinnowImperativePOIStates.HasLiquid(smi) && MinnowImperativePOIStates.IsVisibleOnCamera(smi), UpdateRate.SIM_200ms)
			.ToggleStatusItem(Db.Get().MiscStatusItems.MinnowPOIDehydratedStatus, null);
		this.on.DoNothing().Transition(this.disabling, (MinnowImperativePOIStates.Instance smi) => !MinnowImperativePOIStates.HasLiquid(smi), UpdateRate.SIM_1000ms);
		this.disabling.PlayAnim("exit").OnAnimQueueComplete(this.off);
		this.on.enter.PlayAnim("enter").OnAnimQueueComplete(this.on.waiting);
		this.on.waiting.PlayAnim("on", KAnim.PlayMode.Loop).ParamTransition<bool>(this.hasClickedSideScreen, this.on.working, GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.IsTrue);
		this.on.working.PlayAnim("on", KAnim.PlayMode.Loop).ToggleComponent<ManualDeliveryKG>(false).Enter(delegate(MinnowImperativePOIStates.Instance smi)
		{
			smi.GetComponent<ManualDeliveryKG>().Pause(false, "Delivery enabled");
		})
			.ParamTransition<bool>(this.hasClickedSideScreen, this.on.waiting, GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.IsFalse)
			.Transition(this.poi_completed_pending, new StateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.Transition.ConditionCallback(MinnowImperativePOIStates.HasEnoughMass), UpdateRate.SIM_1000ms)
			.Exit(delegate(MinnowImperativePOIStates.Instance smi)
			{
				smi.GetComponent<ManualDeliveryKG>().Pause(true, "Delivery disabled");
			});
		this.poi_completed_pending.ParamTransition<bool>(this.hasShownCompletedPopup, this.poi_completed_acknowledged, GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.IsTrue).PlayAnim("on", KAnim.PlayMode.Loop).Enter(delegate(MinnowImperativePOIStates.Instance smi)
		{
			this.isCompleted.Set(true, smi, false);
			smi.ClearUserPriority();
			smi.ShowCompletedNotification();
		});
		this.poi_completed_acknowledged.PlayAnim(delegate(MinnowImperativePOIStates.Instance smi)
		{
			if (!MinnowImperativePOIStates.Instance.AllPOIsCompleted())
			{
				return "exit";
			}
			return "victory";
		}, KAnim.PlayMode.Once).Enter(delegate(MinnowImperativePOIStates.Instance smi)
		{
			smi.ClearUserPriority();
			if (MinnowImperativePOIStates.Instance.AllPOIsCompleted())
			{
				MusicManager.instance.PlaySong("Stinger_NewDuplicant", false);
			}
		}).OnAnimQueueComplete(this.off_poi_completed)
			.Toggle("Toggle selectable", new StateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State.Callback(MinnowImperativePOIStates.MakeItUnselectable), new StateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State.Callback(MinnowImperativePOIStates.MakeItSelectable));
		this.off_poi_completed.PlayAnim("off").Toggle("Toggle selectable", new StateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State.Callback(MinnowImperativePOIStates.MakeItUnselectable), new StateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State.Callback(MinnowImperativePOIStates.MakeItSelectable)).Enter(delegate(MinnowImperativePOIStates.Instance smi)
		{
			smi.ClearUserPriority();
		});
	}

	private static void MakeItUnselectable(MinnowImperativePOIStates.Instance smi)
	{
		smi.SetSelectable(false);
	}

	private static void MakeItSelectable(MinnowImperativePOIStates.Instance smi)
	{
		smi.SetSelectable(true);
	}

	private static bool IsVisibleOnCamera(MinnowImperativePOIStates.Instance smi)
	{
		return CameraController.Instance != null && CameraController.Instance.IsVisiblePos(smi.transform.GetPosition());
	}

	private static bool HasEnoughMass(MinnowImperativePOIStates.Instance smi)
	{
		return smi.GetComponent<Storage>().GetMassAvailable(smi.def.requestedTag) >= smi.def.requiredMass;
	}

	private static bool HasLiquid(MinnowImperativePOIStates.Instance smi)
	{
		int num = Grid.CellAbove(Grid.PosToCell(smi.transform.GetPosition()));
		return Grid.IsValidCell(num) && Grid.Element[num].IsLiquid;
	}

	public static int GetPOIStartedCount()
	{
		int num = 0;
		foreach (MinnowImperativePOIStates.Instance instance in Components.MinnowImperativePOIs.Items)
		{
			if (instance.sm.hasShownQuestPopup.Get(instance))
			{
				num++;
			}
		}
		return num;
	}

	public static int GetPOICompletedCount()
	{
		int num = 0;
		foreach (MinnowImperativePOIStates.Instance instance in Components.MinnowImperativePOIs.Items)
		{
			if (instance.sm.isCompleted.Get(instance))
			{
				num++;
			}
		}
		return num;
	}

	private static void UnlockWinAchievement(MinnowImperativePOIStates.Instance smi)
	{
		SaveGame.Instance.ColonyAchievementTracker.allMinnowQuestsCompleted = true;
	}

	private static void SpawnReward(MinnowImperativePOIStates.Instance smi)
	{
		Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(smi.gameObject), Grid.SceneLayer.Ore);
		switch (smi.def.minnowPOIIdentity)
		{
		case MinnowImperativePOIStates.MinnowPOIIdentity.POI_A:
			Util.KInstantiate(Assets.GetPrefab("PlasticGasket"), vector).SetActive(true);
			Util.KInstantiate(Assets.GetPrefab("OxyCoralSeed"), vector + Vector3.right).SetActive(true);
			Util.KInstantiate(Assets.GetPrefab("OxyCoralSeed"), vector + Vector3.left).SetActive(true);
			return;
		case MinnowImperativePOIStates.MinnowPOIIdentity.POI_B:
			Util.KInstantiate(Assets.GetPrefab("PlasticGasket"), vector).SetActive(true);
			Util.KInstantiate(Assets.GetPrefab(DewPalmConfig.SEED_ID), vector + Vector3.right).SetActive(true);
			Util.KInstantiate(Assets.GetPrefab(DewPalmConfig.SEED_ID), vector + Vector3.left).SetActive(true);
			return;
		case MinnowImperativePOIStates.MinnowPOIIdentity.POI_C:
			Util.KInstantiate(Assets.GetPrefab("PlasticGasket"), vector).SetActive(true);
			return;
		default:
			return;
		}
	}

	public const int TOTALPOICOUNT = 3;

	private const int STARTING_ATTRIBUTE_LEVEL = 4;

	private const int STARTING_SKILL_POINTS = 3;

	public GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State off;

	public MinnowImperativePOIStates.OnStates on;

	public GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State disabling;

	public GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State poi_completed_pending;

	public GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State poi_completed_acknowledged;

	public GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State off_poi_completed;

	public StateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.BoolParameter hasShownQuestPopup;

	public StateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.BoolParameter hasShownCompletedPopup;

	public StateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.BoolParameter isCompleted;

	public StateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.BoolParameter hasClickedSideScreen;

	public const int GASKET_REWARD_COUNT = 1;

	public enum MinnowPOIIdentity
	{
		POI_A,
		POI_B,
		POI_C
	}

	public class OnStates : GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State
	{
		public GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State enter;

		public GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State waiting;

		public GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.State working;
	}

	public class Def : StateMachine.BaseDef
	{
		public Tag requestedTag;

		public float requiredMass;

		public MinnowImperativePOIStates.MinnowPOIIdentity minnowPOIIdentity;
	}

	public new class Instance : GameStateMachine<MinnowImperativePOIStates, MinnowImperativePOIStates.Instance, IStateMachineTarget, MinnowImperativePOIStates.Def>.GameInstance, ISidescreenButtonControl
	{
		public string SidescreenTitle
		{
			get
			{
				return global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_A.UI_HEADER;
			}
		}

		public bool HasUserEverClicked
		{
			get
			{
				return base.sm.hasShownQuestPopup.Get(this);
			}
		}

		public bool WasCompletedAndAcknowledged
		{
			get
			{
				return base.smi.sm.isCompleted.Get(base.smi) && base.smi.sm.hasShownCompletedPopup.Get(base.smi);
			}
		}

		public Instance(IStateMachineTarget master, MinnowImperativePOIStates.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			base.StartSM();
			Components.MinnowImperativePOIs.Add(this);
			this.onSelectHandle = base.Subscribe(-1503271301, new Action<object>(this.OnObjectSelected));
		}

		public override void StopSM(string reason)
		{
			if (this.onSelectHandle != -1)
			{
				base.Unsubscribe(ref this.onSelectHandle);
			}
			this.ClearCompletedNotification();
			Components.MinnowImperativePOIs.Remove(this);
			base.StopSM(reason);
		}

		public void SetSelectable(bool selectable)
		{
			KSelectable component = base.gameObject.GetComponent<KSelectable>();
			if (component != null)
			{
				component.IsSelectable = selectable;
			}
		}

		private bool IsInPopupEligibleState()
		{
			return base.smi.IsInsideState(base.smi.sm.on);
		}

		private void OnObjectSelected(object data)
		{
			if (!((Boxed<bool>)data).value)
			{
				return;
			}
			if (this.completedNotification != null)
			{
				Notification.ClickCallback customClickCallback = this.completedNotification.customClickCallback;
				if (customClickCallback == null)
				{
					return;
				}
				customClickCallback(this.completedNotification);
				return;
			}
			else
			{
				if (!this.IsInPopupEligibleState())
				{
					return;
				}
				if (!base.smi.sm.hasShownQuestPopup.Get(base.smi))
				{
					this.ShowQuestPopup();
				}
				return;
			}
		}

		private string GetStartPopupTitle(MinnowImperativePOIStates.MinnowPOIIdentity identity)
		{
			string text = "";
			switch (identity)
			{
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_A:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_START_POI_A_TITLE;
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_B:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_START_POI_B_TITLE;
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_C:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_START_POI_C_TITLE;
				break;
			}
			text = text.Replace("{0}", (MinnowImperativePOIStates.GetPOIStartedCount() + 1).ToString());
			return text.Replace("{1}", 3.ToString());
		}

		private string GetStartPopupDescription(MinnowImperativePOIStates.MinnowPOIIdentity identity)
		{
			string text = "";
			switch (identity)
			{
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_A:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_START_POI_A_DESCRIPTION;
				text = text.Replace("{AMOUNT2}", GameUtil.GetFormattedMass(200f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{AMOUNT1}", "1");
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_B:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_START_POI_B_DESCRIPTION;
				text = text.Replace("{AMOUNT1}", GameUtil.GetFormattedCaloriesForItem(MinnowImperativePOIBConfig.RequiredDeliveryTag, 10f, GameUtil.TimeSlice.None, true)).Replace("{AMOUNT2}", "1");
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_C:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_START_POI_C_DESCRIPTION;
				text = text.Replace("{AMOUNT1}", GameUtil.GetFormattedMass(10f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{AMOUNT2}", "1");
				break;
			}
			return text;
		}

		private string GetCompletePopupTitle(MinnowImperativePOIStates.MinnowPOIIdentity identity)
		{
			string text = "";
			switch (identity)
			{
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_A:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_COMPLETE_POI_A_TITLE;
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_B:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_COMPLETE_POI_B_TITLE;
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_C:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_COMPLETE_POI_C_TITLE;
				break;
			}
			return text;
		}

		private string GetCompletePopupDescription(MinnowImperativePOIStates.MinnowPOIIdentity identity)
		{
			string text = "";
			switch (identity)
			{
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_A:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_COMPLETE_POI_A_DESCRIPTION;
				text = text.Replace("{AMOUNT1}", GameUtil.GetFormattedMass(200f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{AMOUNT2}", "1");
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_B:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_COMPLETE_POI_B_DESCRIPTION;
				text = text.Replace("{AMOUNT1}", GameUtil.GetFormattedMass(10f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{AMOUNT2}", "1");
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_C:
				text = COLONY_ACHIEVEMENTS.FINDING_MINNOW.POPUPS.POPUP_COMPLETE_POI_C_DESCRIPTION;
				text = text.Replace("{AMOUNT1}", GameUtil.GetFormattedMass(10f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")).Replace("{AMOUNT2}", "1");
				break;
			}
			return text;
		}

		private string GetStartPopupImage(MinnowImperativePOIStates.MinnowPOIIdentity identity)
		{
			string text = "";
			switch (identity)
			{
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_A:
				text = "MinnowDiscoveryA_kanim";
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_B:
				text = "MinnowDiscoveryB_kanim";
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_C:
				text = "MinnowDiscoveryC_kanim";
				break;
			}
			return text;
		}

		private string GetCompletedPopupImage(MinnowImperativePOIStates.MinnowPOIIdentity identity)
		{
			string text = "";
			switch (identity)
			{
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_A:
				text = "MinnowCompleteA_kanim";
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_B:
				text = "MinnowCompleteB_kanim";
				break;
			case MinnowImperativePOIStates.MinnowPOIIdentity.POI_C:
				text = "MinnowCompleteC_kanim";
				break;
			}
			return text;
		}

		public void ShowCompletedNotification()
		{
			EventInfoData eventInfoData = EventInfoDataHelper.GenerateStoryTraitData(this.GetCompletePopupTitle(base.smi.def.minnowPOIIdentity), this.GetCompletePopupDescription(base.smi.def.minnowPOIIdentity), UI.TOOLTIPS.CLOSETOOLTIP, this.GetCompletedPopupImage(base.smi.def.minnowPOIIdentity), MinnowImperativePOIStates.Instance.AllPOIsCompleted() ? EventInfoDataHelper.PopupType.COMPLETE : EventInfoDataHelper.PopupType.NORMAL, null, null, new global::System.Action(this.OnCompletionPopupAcknowledged));
			this.completedNotification = EventInfoScreen.CreateNotification(eventInfoData, null);
			base.gameObject.AddOrGet<Notifier>().Add(this.completedNotification, "");
		}

		public void ClearUserPriority()
		{
			Prioritizable component = base.smi.GetComponent<Prioritizable>();
			if (component != null)
			{
				component.SetMasterPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, 5));
			}
		}

		private void OnCompletionPopupAcknowledged()
		{
			this.ClearCompletedNotification();
			MinnowImperativePOIStates.SpawnReward(base.smi);
			int poicompletedCount = MinnowImperativePOIStates.GetPOICompletedCount();
			SaveGame.Instance.ColonyAchievementTracker.minnowQuestsCompleted = Mathf.Max(SaveGame.Instance.ColonyAchievementTracker.minnowQuestsCompleted, poicompletedCount);
			bool flag = poicompletedCount >= 3;
			Game.Instance.StartCoroutine(this.CompletionCameraSequence(flag));
		}

		private IEnumerator CompletionCameraSequence(bool wasLastPOI)
		{
			Vector3 cameraStartPos = CameraController.Instance.transform.position;
			Vector3 position = base.transform.GetPosition();
			if (!SpeedControlScreen.Instance.IsPaused)
			{
				SpeedControlScreen.Instance.Pause(false, false);
			}
			RootMenu.Instance.canTogglePauseScreen = false;
			CameraController.Instance.DisableUserCameraControl = true;
			CameraController.Instance.SetWorldInteractive(false);
			ManagementMenu.Instance.CloseAll();
			StoryMessageScreen.HideInterface(true);
			OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID, false);
			CameraController.Instance.SetOverrideZoomSpeed(10f);
			CameraController.Instance.SetTargetPos(position, 8f, false);
			yield return SequenceUtil.WaitForSecondsRealtime(0.5f);
			base.smi.sm.hasShownCompletedPopup.Set(true, base.smi, false);
			if (SpeedControlScreen.Instance.IsPaused)
			{
				SpeedControlScreen.Instance.Unpause(false);
			}
			SpeedControlScreen.Instance.SetSpeed(0);
			yield return SequenceUtil.WaitForSecondsRealtime(2.5f);
			if (!wasLastPOI)
			{
				Vector3 nextPOIPos;
				if (MinnowImperativePOIStates.Instance.FindNextUncompletedPOIPosition(out nextPOIPos))
				{
					int num;
					int num2;
					Grid.CellToXY(Grid.PosToCell(nextPOIPos), out num, out num2);
					GridVisibility.Reveal(num, num2, 16, 16f);
					yield return null;
					CameraController.Instance.SetOverrideZoomSpeed(2f);
					CameraController.Instance.SetTargetPos(nextPOIPos, 8f, false);
					yield return SequenceUtil.WaitForSecondsRealtime(2.5f);
				}
				nextPOIPos = default(Vector3);
			}
			else if (!MinnowImperativePOIStates.Instance.MinnowAlreadyExists())
			{
				MinnowImperativePOIStates.UnlockWinAchievement(base.smi);
				this.SpawnMinnow();
				base.smi.GoTo(base.smi.sm.off_poi_completed);
			}
			CameraController.Instance.SetOverrideZoomSpeed(1f);
			CameraController.Instance.SetWorldInteractive(true);
			CameraController.Instance.DisableUserCameraControl = false;
			RootMenu.Instance.canTogglePauseScreen = true;
			StoryMessageScreen.HideInterface(false);
			NotificationScreen_TemporaryActions.Instance.CreateCameraReturnActionButton(cameraStartPos);
			yield break;
		}

		private static bool FindNextUncompletedPOIPosition(out Vector3 position)
		{
			int i = 0;
			while (i < 3)
			{
				MinnowImperativePOIStates.MinnowPOIIdentity minnowPOIIdentity = (MinnowImperativePOIStates.MinnowPOIIdentity)i;
				foreach (MinnowImperativePOIStates.Instance instance in Components.MinnowImperativePOIs.Items)
				{
					if (instance.def.minnowPOIIdentity == minnowPOIIdentity && !instance.sm.isCompleted.Get(instance))
					{
						position = instance.transform.GetPosition();
						return true;
					}
				}
				string text = MinnowImperativePOIStates.Instance.MinnowPOIPrefabIDs[i];
				List<WorldGenSpawner.Spawnable> spawnablesWithTag = SaveGame.Instance.worldGenSpawner.GetSpawnablesWithTag(false, new Tag[]
				{
					new Tag(text)
				});
				if (spawnablesWithTag.Count > 0)
				{
					position = Grid.CellToPosCCC(spawnablesWithTag[0].cell, Grid.SceneLayer.Creatures);
					return true;
				}
				i++;
				continue;
			}
			position = Vector3.zero;
			return false;
		}

		public void ClearCompletedNotification()
		{
			if (this.completedNotification != null)
			{
				base.gameObject.AddOrGet<Notifier>().Remove(this.completedNotification);
				this.completedNotification = null;
			}
		}

		public static bool AllPOIsCompleted()
		{
			return MinnowImperativePOIStates.GetPOICompletedCount() >= 3;
		}

		private static bool MinnowAlreadyExists()
		{
			using (List<MinionIdentity>.Enumerator enumerator = Components.LiveMinionIdentities.Items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.personalityResourceId == "MINNOW")
					{
						return true;
					}
				}
			}
			return false;
		}

		private void SpawnMinnow()
		{
			MinionStartingStats minionStartingStats = new MinionStartingStats(Db.Get().Personalities.Get("MINNOW"), null, "AncientKnowledge", false);
			string[] all_ATTRIBUTES = DUPLICANTSTATS.ALL_ATTRIBUTES;
			for (int i = 0; i < all_ATTRIBUTES.Length; i++)
			{
				Dictionary<string, int> startingLevels = minionStartingStats.StartingLevels;
				string text = all_ATTRIBUTES[i];
				startingLevels[text] += 4;
			}
			GameObject prefab = Assets.GetPrefab(BaseMinionConfig.GetMinionIDForModel(minionStartingStats.personality.model));
			GameObject gameObject = Util.KInstantiate(prefab, null, null);
			gameObject.name = prefab.name;
			Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
			Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(base.gameObject), Grid.SceneLayer.Move);
			gameObject.transform.SetLocalPosition(vector);
			gameObject.SetActive(true);
			MinionResume component = gameObject.GetComponent<MinionResume>();
			for (int j = 0; j < 3; j++)
			{
				component.ForceAddSkillPoint();
			}
			minionStartingStats.Apply(gameObject);
			gameObject.GetComponent<MinionIdentity>().arrivalTime = (float)(-1 * global::UnityEngine.Random.Range(2050, 2180));
			gameObject.GetMyWorld().SetDupeVisited();
		}

		private void ShowQuestPopup()
		{
			EventInfoScreen.ShowPopup(EventInfoDataHelper.GenerateStoryTraitData(this.GetStartPopupTitle(base.smi.def.minnowPOIIdentity), this.GetStartPopupDescription(base.smi.def.minnowPOIIdentity), UI.TOOLTIPS.CLOSETOOLTIP, this.GetStartPopupImage(base.smi.def.minnowPOIIdentity), EventInfoDataHelper.PopupType.BEGIN, null, null, delegate
			{
				base.smi.sm.hasShownQuestPopup.Set(true, base.smi, false);
				KSelectable component = base.gameObject.GetComponent<KSelectable>();
				SelectTool.Instance.Select(null, false);
				SelectTool.Instance.Select(component, false);
			}));
		}

		public string SidescreenButtonText
		{
			get
			{
				ManualDeliveryKG component = base.gameObject.GetComponent<ManualDeliveryKG>();
				if (component != null && component.enabled)
				{
					return global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_A.UI_BUTTON_DISABLE;
				}
				return global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_A.UI_BUTTON_ENABLE;
			}
		}

		public string SidescreenButtonTooltip
		{
			get
			{
				ManualDeliveryKG component = base.gameObject.GetComponent<ManualDeliveryKG>();
				if (component != null && component.enabled)
				{
					return global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_A.UI_BUTTON_DISABLE_TOOLTIP;
				}
				return global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_A.UI_BUTTON_ENABLE_TOOLTIP;
			}
		}

		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
		}

		public bool SidescreenEnabled()
		{
			return this.IsInPopupEligibleState() && base.smi.sm.hasShownQuestPopup.Get(base.smi);
		}

		public bool SidescreenButtonInteractable()
		{
			return true;
		}

		public void OnSidescreenButtonPressed()
		{
			bool flag = base.smi.sm.hasClickedSideScreen.Get(base.smi);
			base.smi.sm.hasClickedSideScreen.Set(!flag, base.smi, false);
		}

		public int HorizontalGroupID()
		{
			return -1;
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}

		private int onSelectHandle = -1;

		private Notification completedNotification;

		private static readonly string[] MinnowPOIPrefabIDs = new string[] { "MinnowImperativePOIA", "MinnowImperativePOIB", "MinnowImperativePOIC" };
	}
}
