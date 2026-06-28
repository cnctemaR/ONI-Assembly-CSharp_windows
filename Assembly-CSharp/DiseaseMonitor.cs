using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;

public class DiseaseMonitor : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = true;
		default_state = this.healthy;
		this.healthy.DefaultState(this.healthy.healthy).EventTransition(GameHashes.DiseaseAdded, this.sick, (DiseaseMonitor.Instance smi) => smi.IsSick());
		this.healthy.pre.ToggleChore((DiseaseMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, new string[] { "idle_default" }, null), this.healthy.pre, false).GoTo(this.healthy.healthy);
		this.sick.DefaultState(this.sick.notify).EventTransition(GameHashes.DiseaseCured, this.sick.post, (DiseaseMonitor.Instance smi) => !smi.IsSick()).ToggleAnims("anim_idle_sick", 0f)
			.ToggleExpression(Db.Get().Expressions.Sick, null)
			.ToggleUrge(Db.Get().Urges.TakeMedicine)
			.ToggleUrge(Db.Get().Urges.RestDueToDisease);
		this.sick.notify.DefaultState(this.sick.notify.notify);
		this.sick.notify.notify.ToggleThought(Db.Get().Thoughts.GotInfected, null).ToggleChore((DiseaseMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, new string[] { "idle_pre", "idle_default" }, null), this.sick.notify.cooldown, false);
		this.sick.notify.cooldown.ScheduleGoTo(60f, this.sick.notify);
		this.sick.post.ToggleChore((DiseaseMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, new string[] { "idle_pst" }, null), this.healthy.pre, false);
	}

	public static Disease GetDisease(string disease_id)
	{
		Disease disease = null;
		if (disease_id != null)
		{
			global::Database.Diseases diseases = Db.Get().Diseases;
			for (int i = 0; i < diseases.Count; i++)
			{
				if (diseases[i].Id == disease_id)
				{
					disease = diseases[i];
					break;
				}
			}
		}
		return disease;
	}

	public DiseaseMonitor.HealthyStates healthy;

	public DiseaseMonitor.SickStates sick;

	public class HealthyStates : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget>.State pre;

		public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget>.State healthy;
	}

	public class NotifyStates : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget>.State notify;

		public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget>.State cooldown;
	}

	public class SickStates : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget>.State
	{
		public DiseaseMonitor.NotifyStates notify;

		public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget>.State post;
	}

	public new class Instance : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			base.gameObject.Subscribe(-283306403, new EventSystem.EventHandler(this.OnExposedToDisease));
			this.activeDiseases = master.GetComponent<MinionModifiers>().diseases;
		}

		public void StopListening()
		{
			base.master.Unsubscribe(-283306403, new EventSystem.EventHandler(this.OnExposedToDisease));
		}

		private void OnExposedToDisease(object data)
		{
			DiseaseExposureInfo diseaseExposureInfo = (DiseaseExposureInfo)data;
			Disease disease = DiseaseMonitor.GetDisease(diseaseExposureInfo.diseaseID);
			if (disease == null)
			{
				return;
			}
			if (disease.ShouldInfect(base.gameObject, diseaseExposureInfo.exposureCount))
			{
				this.activeDiseases.Infect(disease, diseaseExposureInfo);
			}
		}

		private string OnGetToolTip(List<Notification> notifications, object data)
		{
			return DUPLICANTS.STATUSITEMS.HASDISEASE.TOOLTIP;
		}

		public bool IsSick()
		{
			return this.activeDiseases.Count > 0;
		}

		private Klei.AI.Diseases activeDiseases;
	}
}
