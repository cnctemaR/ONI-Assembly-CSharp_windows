using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class PeeChore : Chore<PeeChore.StatesInstance>
{
	public PeeChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Pee, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new PeeChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.GameInstance
	{
		public StatesInstance(PeeChore master, GameObject worker)
			: base(master)
		{
			this.bladder = Db.Get().Amounts.Bladder.Lookup(worker);
			this.bodyTemperature = Db.Get().Amounts.Temperature.Lookup(worker);
			base.sm.worker.Set(worker, base.smi, false);
		}

		public bool IsDonePeeing()
		{
			return this.bladder.value <= 0f;
		}

		public void SpawnDirtyWater(float dt)
		{
			int num = Grid.PosToCell(base.sm.worker.Get<KMonoBehaviour>(base.smi));
			byte index = Db.Get().Diseases.GetIndex("FoodPoisoning");
			float num2 = dt * -this.bladder.GetDelta() / this.bladder.GetMax();
			if (num2 > 0f)
			{
				float num3 = 2f * num2;
				Equippable equippable = base.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
				if (equippable != null)
				{
					equippable.GetComponent<Storage>().AddLiquid(SimHashes.DirtyWater, num3, this.bodyTemperature.value, index, Mathf.CeilToInt(100000f * num2), false, true);
					return;
				}
				SimMessages.AddRemoveSubstance(num, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, num3, this.bodyTemperature.value, index, Mathf.CeilToInt(100000f * num2), true, -1);
			}
		}

		public Notification stressfullyEmptyingBladder = new Notification(DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null, true, false, false);

		public AmountInstance bladder;

		private AmountInstance bodyTemperature;
	}

	public class States : GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.running;
			base.Target(this.worker);
			this.running.ToggleAnims("anim_expel_kanim", 0f, "").ToggleEffect("StressfulyEmptyingBladder").DoNotification((PeeChore.StatesInstance smi) => smi.stressfullyEmptyingBladder)
				.DoReport(ReportManager.ReportType.ToiletIncident, (PeeChore.StatesInstance smi) => 1f, (PeeChore.StatesInstance smi) => this.masterTarget.Get(smi).GetProperName())
				.DoTutorial(Tutorial.TutorialMessages.TM_Mopping)
				.Transition(null, (PeeChore.StatesInstance smi) => smi.IsDonePeeing(), UpdateRate.SIM_200ms)
				.Update("SpawnDirtyWater", delegate(PeeChore.StatesInstance smi, float dt)
				{
					smi.SpawnDirtyWater(dt);
				}, UpdateRate.SIM_200ms, false)
				.PlayAnim("working_loop", KAnim.PlayMode.Loop)
				.ToggleTag(GameTags.MakingMess)
				.Enter(delegate(PeeChore.StatesInstance smi)
				{
					if (Sim.IsRadiationEnabled() && smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).value > 0f)
					{
						smi.master.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads, null);
					}
				})
				.Exit(delegate(PeeChore.StatesInstance smi)
				{
					if (Sim.IsRadiationEnabled())
					{
						smi.master.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads, false);
						AmountInstance amountInstance = smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance.Id);
						RadiationMonitor.Instance smi2 = smi.master.gameObject.GetSMI<RadiationMonitor.Instance>();
						if (smi2 != null)
						{
							float num = Math.Min(amountInstance.value, 100f * smi2.difficultySettingMod);
							smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance.Id).ApplyDelta(-num);
							if (num >= 1f)
							{
								PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, Mathf.FloorToInt(num).ToString() + UI.UNITSUFFIXES.RADIATION.RADS, smi.master.transform, 1.5f, false);
							}
						}
					}
				});
		}

		public StateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.TargetParameter worker;

		public GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.State running;
	}
}
