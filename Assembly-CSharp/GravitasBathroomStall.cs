using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class GravitasBathroomStall : GameStateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.start;
		this.root.DefaultState(this.start);
		this.start.PlayAnim("idle").Update(delegate(GravitasBathroomStall.Instance smi, float dt)
		{
			if (HijackedHeadquarters.Instance.PrinterceptorInstance != null && HijackedHeadquarters.IsOperational(HijackedHeadquarters.Instance.PrinterceptorInstance.GetSMI<HijackedHeadquarters.Instance>()))
			{
				smi.GoTo(this.branch);
			}
		}, UpdateRate.SIM_200ms, false);
		this.branch.ParamTransition<bool>(this.hasBeenActivated, this.blinking, GameStateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.IsFalse).ParamTransition<bool>(this.hasBeenActivated, this.activated, GameStateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.IsTrue);
		this.blinking.PlayAnim("code_ready", KAnim.PlayMode.Loop).EventHandlerTransition(GameHashes.BuildingActivated, this.activated, (GravitasBathroomStall.Instance smi, object data) => ((Boxed<bool>)data).value);
		this.activated.Enter(delegate(GravitasBathroomStall.Instance smi)
		{
			if (!smi.sm.hasShownPopup.Get(smi))
			{
				smi.ShowLoreUnlockedPopup();
			}
			else
			{
				smi.GoTo(this.complete);
			}
			smi.sm.hasBeenActivated.Set(true, smi, false);
		}).PlayAnim("activated");
		this.complete.PlayAnim("idle");
	}

	public GameStateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.State start;

	public GameStateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.State branch;

	public GameStateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.State blinking;

	public GameStateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.State activated;

	public GameStateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.State complete;

	public StateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.BoolParameter hasBeenActivated;

	public StateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.BoolParameter hasShownPopup;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<GravitasBathroomStall, GravitasBathroomStall.Instance, IStateMachineTarget, GravitasBathroomStall.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, GravitasBathroomStall.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			base.StartSM();
			base.GetComponent<Activatable>().activationCondition = () => HijackedHeadquarters.Instance.PrinterceptorInstance != null && HijackedHeadquarters.IsOperational(HijackedHeadquarters.Instance.PrinterceptorInstance.GetSMI<HijackedHeadquarters.Instance>());
			this.storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.HijackedHeadquarters.HashId);
			this.onBuildingSelectHandle = base.Subscribe(-1503271301, new Action<object>(this.OnBuildingSelect));
		}

		public override void StopSM(string reason)
		{
			if (this.onBuildingSelectHandle != -1)
			{
				base.Unsubscribe(ref this.onBuildingSelectHandle);
			}
			base.StopSM(reason);
		}

		private void OnBuildingSelect(object obj)
		{
			if (!((Boxed<bool>)obj).value)
			{
				return;
			}
			if (this.completeNotification != null)
			{
				this.completeNotification.customClickCallback(this.completeNotification.customClickData);
			}
		}

		public void ShowLoreUnlockedPopup()
		{
			EventInfoData eventInfoData = EventInfoDataHelper.GenerateStoryTraitData(CODEX.STORY_TRAITS.HIJACK_HEADQUARTERS.UNLOCK_POPUP.NAME, CODEX.STORY_TRAITS.HIJACK_HEADQUARTERS.UNLOCK_POPUP.DESCRIPTION, CODEX.STORY_TRAITS.HIJACK_HEADQUARTERS.UNLOCK_POPUP.BUTTON, "printerceptorcoderevealed_kanim", EventInfoDataHelper.PopupType.NORMAL, null, null, delegate
			{
				base.smi.sm.hasShownPopup.Set(true, base.smi, false);
				base.smi.master.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(GravitasBathroomStall.Instance.Sequence(base.smi));
				base.smi.GoTo(base.smi.sm.complete);
			});
			this.completeNotification = EventInfoScreen.CreateNotification(eventInfoData, null);
			base.gameObject.AddOrGet<Notifier>().Add(this.completeNotification, "");
			base.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.AttentionRequired, base.smi);
		}

		private static IEnumerator Sequence(GravitasBathroomStall.Instance smi)
		{
			StoryManager.Instance.GetStoryInstance(Db.Get().Stories.HijackedHeadquarters.HashId);
			smi.ClearEndNotification();
			if (HijackedHeadquarters.Instance.PrinterceptorInstance == null)
			{
				smi.RevealPrinterceptor();
			}
			CameraController.Instance.FadeOut(1f, 1f, null);
			yield return SequenceUtil.WaitForSecondsRealtime(1f);
			Vector3 vector = new Vector3(2f, 3f, 0f);
			GameUtil.FocusCamera(HijackedHeadquarters.Instance.PrinterceptorInstance.transform.position + vector, 10f, false, true);
			yield return SequenceUtil.WaitForSecondsRealtime(1f);
			if (SpeedControlScreen.Instance.IsPaused)
			{
				SpeedControlScreen.Instance.Unpause(false);
			}
			CameraController.Instance.FadeIn(0f, 1f, null);
			yield return SequenceUtil.WaitForSecondsRealtime(1f);
			HijackedHeadquarters.Instance.PrinterceptorInstance.GetSMI<HijackedHeadquarters.Instance>().UnlockPrinterceptor();
			yield break;
		}

		private void RevealPrinterceptor()
		{
			List<WorldGenSpawner.Spawnable> list = new List<WorldGenSpawner.Spawnable>();
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				list.AddRange(SaveGame.Instance.worldGenSpawner.GetSpawnablesWithTag("HijackedHeadquarters", worldContainer.id, false));
			}
			foreach (WorldGenSpawner.Spawnable spawnable in list)
			{
				int num;
				int num2;
				Grid.CellToXY(spawnable.cell, out num, out num2);
				GridVisibility.Reveal(num, num2, 10, 10f);
			}
		}

		public void ClearEndNotification()
		{
			base.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.AttentionRequired, false);
			if (this.completeNotification != null)
			{
				base.gameObject.AddOrGet<Notifier>().Remove(this.completeNotification);
			}
			this.completeNotification = null;
		}

		private StoryInstance storyInstance;

		private Notification completeNotification;

		private int onBuildingSelectHandle = -1;
	}
}
