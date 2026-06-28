using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class DiseaseInstance : ModifierInstance<Disease>
	{
		public DiseaseInstance(GameObject game_object, Disease disease)
			: base(game_object, disease)
		{
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			this.InitializeAndStart();
		}

		public DiseaseExposureInfo ExposureInfo
		{
			get
			{
				return this.exposureInfo;
			}
			set
			{
				this.exposureInfo = value;
				this.InitializeAndStart();
			}
		}

		private void InitializeAndStart()
		{
			Disease disease = this.modifier;
			Func<List<Notification>, object, string> func = delegate(List<Notification> notificationList, object data)
			{
				string text = string.Empty;
				for (int i = 0; i < notificationList.Count; i++)
				{
					Notification notification = notificationList[i];
					string text2 = (string)notification.tooltipData;
					text += string.Format(DUPLICANTS.DISEASES.NOTIFICATION_TOOLTIP, notification.NotifierName, disease.Name, text2);
					if (i < notificationList.Count - 1)
					{
						text += "\n";
					}
				}
				return text;
			};
			string name = disease.Name;
			string infectionSourceInfo = this.exposureInfo.infectionSourceInfo;
			this.notification = new Notification(name, NotificationType.Bad, null, func, infectionSourceInfo, true, 0f, null, null, null);
			this.statusItem = new StatusItem(disease.Id, disease.Name, string.Empty, DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP, false, StatusItem.IconType.Exclamation, NotificationType.Bad, SimViewMode.None, SimViewMode.None);
			this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveString);
			if (this.smi != null)
			{
				this.smi.StopSM("refresh");
			}
			this.smi = new DiseaseInstance.StatesInstance(this);
			this.smi.StartSM();
		}

		private string ResolveString(string str, object data)
		{
			if (this.smi == null)
			{
				Debug.LogWarning("Attempting to resolve string when smi is null");
				return str;
			}
			KSelectable component = this.gameObject.GetComponent<KSelectable>();
			string properName = component.GetProperName();
			str = str.Replace("{Infectee}", properName);
			str = str.Replace("{InfectionSource}", this.exposureInfo.infectionSourceInfo);
			str = str.Replace("{Duration}", GameUtil.GetFormattedCycles(this.GetInfectedTimeRemaining()));
			str = str.Replace("{Symptoms}", this.modifier.GetSymptoms());
			if (this.curesApplied.Count > 0)
			{
				string text = string.Empty;
				for (int i = 0; i < this.curesApplied.Count; i++)
				{
					text += this.curesApplied[i].name;
					if (i < this.curesApplied.Count - 1)
					{
						text += ", ";
					}
				}
				str = str.Replace("{Cures}", text);
			}
			else
			{
				str = str.Replace("{Cures}", DUPLICANTS.DISEASES.NOMEDICINETAKEN);
			}
			return str;
		}

		public float GetInfectedTimeRemaining()
		{
			float sicknessDuration = this.modifier.SicknessDuration;
			float num = sicknessDuration * (1f - this.smi.sm.percentRecovered.Get(this.smi));
			return num / this.cureSpeedMultiplier;
		}

		public void AddCureSpeedMultiplier(string cure, float multiplier)
		{
			this.curesApplied.Add(new DiseaseInstance.CureInfo
			{
				name = cure,
				multiplier = multiplier
			});
			this.cureSpeedMultiplier = Mathf.Max(multiplier, this.cureSpeedMultiplier);
		}

		public void RemoveCureSpeedMultiplier(string cure)
		{
			this.curesApplied.RemoveAll((DiseaseInstance.CureInfo c) => c.name == cure);
			this.cureSpeedMultiplier = 1f;
			for (int i = 0; i < this.curesApplied.Count; i++)
			{
				this.cureSpeedMultiplier = Mathf.Max(this.cureSpeedMultiplier, this.curesApplied[i].multiplier);
			}
		}

		public bool HasTakenPill(string pillName)
		{
			bool flag = false;
			for (int i = 0; i < this.curesApplied.Count; i++)
			{
				if (this.curesApplied[i].name == pillName)
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public void Cure()
		{
			this.smi.Cure();
		}

		public override void OnCleanUp()
		{
			this.smi.StopSM("DiseaseInstance.OnCleanUp");
			this.smi = null;
		}

		public StatusItem GetStatusItem()
		{
			return this.statusItem;
		}

		[Serialize]
		private DiseaseExposureInfo exposureInfo;

		[Serialize]
		private float cureSpeedMultiplier = 1f;

		[Serialize]
		private List<DiseaseInstance.CureInfo> curesApplied = new List<DiseaseInstance.CureInfo>();

		private DiseaseInstance.StatesInstance smi;

		private StatusItem statusItem;

		private Notification notification;

		private struct CureInfo
		{
			public string name;

			public float multiplier;
		}

		public class StatesInstance : GameStateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance>.GameInstance
		{
			public StatesInstance(DiseaseInstance master)
				: base(master)
			{
			}

			public void UpdatePercentCured()
			{
				float num = this.deltatime * base.master.cureSpeedMultiplier / base.master.modifier.SicknessDuration;
				base.sm.percentRecovered.Delta(num, base.smi);
			}

			public void Infect()
			{
				Disease modifier = base.master.modifier;
				this.instanceData = modifier.Infect(base.gameObject, base.master.exposureInfo);
				if (PopFXManager.Instance != null)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, string.Format(DUPLICANTS.DISEASES.INFECTED_POPUP, modifier.Name), base.gameObject.transform, 1.5f, true);
				}
			}

			public void Cure()
			{
				Disease modifier = base.master.modifier;
				base.gameObject.GetComponent<Modifiers>().diseases.Cure(modifier);
				modifier.Cure(base.gameObject, this.instanceData);
				if (PopFXManager.Instance != null)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, string.Format(DUPLICANTS.DISEASES.CURED_POPUP, modifier.Name), base.gameObject.transform, 1.5f, true);
				}
			}

			public DiseaseExposureInfo GetExposureInfo()
			{
				return base.master.ExposureInfo;
			}

			private object instanceData;
		}

		public class States : GameStateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = this.infected;
				base.serializable = true;
				this.infected.Enter("Infect", delegate(DiseaseInstance.StatesInstance smi)
				{
					smi.Infect();
				}).DoNotification((DiseaseInstance.StatesInstance smi) => smi.master.notification).Update("UpdatePercentCured", delegate(DiseaseInstance.StatesInstance smi)
				{
					smi.UpdatePercentCured();
				})
					.ParamTransition<float>(this.percentRecovered, null, (DiseaseInstance.StatesInstance smi, float p) => p > 1f)
					.Exit("StoreCuredStartTime", delegate(DiseaseInstance.StatesInstance smi)
					{
						smi.master.Cure();
					})
					.ToggleStatusItem((DiseaseInstance.StatesInstance smi) => smi.master.GetStatusItem(), (DiseaseInstance.StatesInstance smi) => smi.GetExposureInfo());
			}

			public StateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance>.FloatParameter percentRecovered;

			public GameStateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance>.State infected;
		}
	}
}
