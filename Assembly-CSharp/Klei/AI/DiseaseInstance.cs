using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class DiseaseInstance : ModifierInstance<Disease>, ISaveLoadable
	{
		public DiseaseInstance(GameObject game_object, Disease disease)
			: base(game_object, disease)
		{
		}

		public float TotalCureSpeedMultiplier
		{
			get
			{
				AttributeInstance attributeInstance = Db.Get().Attributes.DiseaseCureSpeed.Lookup(this.smi.master.gameObject);
				AttributeInstance attributeInstance2 = this.modifier.cureSpeedBase.Lookup(this.smi.master.gameObject);
				float num = 1f;
				if (attributeInstance != null)
				{
					num *= attributeInstance.GetTotalValue();
				}
				if (attributeInstance2 != null)
				{
					num *= attributeInstance2.GetTotalValue();
				}
				return num;
			}
		}

		public bool IsDoctored
		{
			get
			{
				if (this.gameObject == null)
				{
					return false;
				}
				AttributeInstance attributeInstance = Db.Get().Attributes.DoctoredLevel.Lookup(this.gameObject);
				return attributeInstance != null && attributeInstance.GetTotalValue() > 0f;
			}
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
			this.notification = new Notification(name, (disease.severity > Disease.Severity.Minor) ? NotificationType.Bad : NotificationType.BadMinor, HashedString.Invalid, func, infectionSourceInfo, true, 0f, null, null, null);
			this.statusItem = new StatusItem(disease.Id, disease.Name, DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.TEMPLATE, string.Empty, (disease.severity > Disease.Severity.Minor) ? StatusItem.IconType.Exclamation : StatusItem.IconType.Info, (disease.severity > Disease.Severity.Minor) ? NotificationType.Bad : NotificationType.BadMinor, false, SimViewMode.None, 14334);
			this.statusItem.resolveTooltipCallback = new Func<string, object, string>(this.ResolveString);
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
				global::Debug.LogWarning("Attempting to resolve string when smi is null", null);
				return str;
			}
			KSelectable component = this.gameObject.GetComponent<KSelectable>();
			str = str.Replace("{Descriptor}", string.Format(DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.DESCRIPTOR, Strings.Get("STRINGS.DUPLICANTS.DISEASES.SEVERITY." + this.modifier.severity.ToString().ToUpper()), Strings.Get("STRINGS.DUPLICANTS.DISEASES.TYPE." + this.modifier.diseaseType.ToString().ToUpper())));
			str = str.Replace("{Infectee}", component.GetProperName());
			str = str.Replace("{InfectionSource}", string.Format(DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.INFECTION_SOURCE, this.exposureInfo.infectionSourceInfo));
			if (this.modifier.doctorRequired && !this.IsDoctored)
			{
				str = str.Replace("{Duration}", DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.DOCTOR_REQUIRED);
			}
			else
			{
				str = str.Replace("{Duration}", string.Format(DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.DURATION, GameUtil.GetFormattedCycles(this.GetInfectedTimeRemaining(), "F1")));
			}
			if (this.IsDoctored)
			{
				str = str.Replace("{Doctor}", DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.DOCTORED);
			}
			if (this.modifier.fatalityDuration > 0f)
			{
				str = str.Replace("{Fatality}", string.Format(DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.FATALITY, GameUtil.GetFormattedCycles(this.GetFatalityTimeRemaining(), "F1")));
			}
			List<Descriptor> symptoms = this.modifier.GetSymptoms();
			string text = string.Empty;
			foreach (Descriptor descriptor in symptoms)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				descriptor.IncreaseIndent();
				text += descriptor.IndentedText();
			}
			str = str.Replace("{Symptoms}", text);
			str = Regex.Replace(str, "{[^}]*}", string.Empty);
			return str;
		}

		public float GetInfectedTimeRemaining()
		{
			float sicknessDuration = this.modifier.SicknessDuration;
			float num = sicknessDuration * (1f - this.smi.sm.percentRecovered.Get(this.smi));
			return num / this.TotalCureSpeedMultiplier;
		}

		public float GetFatalityTimeRemaining()
		{
			float fatalityDuration = this.modifier.fatalityDuration;
			return fatalityDuration * (1f - this.smi.sm.percentDied.Get(this.smi));
		}

		public float GetPercentCured()
		{
			return (this.smi == null) ? 0f : this.smi.sm.percentRecovered.Get(this.smi);
		}

		public void SetPercentCured(float pct)
		{
			this.smi.sm.percentRecovered.Set(pct, this.smi);
		}

		public void Cure()
		{
			this.smi.Cure();
		}

		public override void OnCleanUp()
		{
			if (this.smi != null)
			{
				this.smi.StopSM("DiseaseInstance.OnCleanUp");
				this.smi = null;
			}
		}

		public StatusItem GetStatusItem()
		{
			return this.statusItem;
		}

		public List<Descriptor> GetDescriptors()
		{
			return this.modifier.GetDiseaseSourceDescriptors();
		}

		[Serialize]
		private DiseaseExposureInfo exposureInfo;

		private DiseaseInstance.StatesInstance smi;

		private StatusItem statusItem;

		private Notification notification;

		private struct CureInfo
		{
			public string name;

			public float multiplier;
		}

		public class StatesInstance : GameStateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance, object>.GameInstance
		{
			public StatesInstance(DiseaseInstance master)
				: base(master)
			{
			}

			public void UpdateProgress()
			{
				if (!base.master.modifier.doctorRequired || base.master.IsDoctored)
				{
					float num = this.deltatime * base.master.TotalCureSpeedMultiplier / base.master.modifier.SicknessDuration;
					base.sm.percentRecovered.Delta(num, base.smi);
				}
				if (base.master.modifier.fatalityDuration > 0f && !base.master.IsDoctored)
				{
					float num2 = this.deltatime / base.master.modifier.fatalityDuration;
					base.sm.percentDied.Delta(num2, base.smi);
				}
			}

			public void Infect()
			{
				Disease modifier = base.master.modifier;
				this.componentData = modifier.Infect(base.gameObject, base.master, base.master.exposureInfo);
				if (PopFXManager.Instance != null)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, string.Format(DUPLICANTS.DISEASES.INFECTED_POPUP, modifier.Name), base.gameObject.transform, 1.5f, true);
				}
			}

			public void Cure()
			{
				Disease modifier = base.master.modifier;
				base.gameObject.GetComponent<Modifiers>().diseases.Cure(modifier);
				modifier.Cure(base.gameObject, this.componentData);
				if (PopFXManager.Instance != null)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, string.Format(DUPLICANTS.DISEASES.CURED_POPUP, modifier.Name), base.gameObject.transform, 1.5f, true);
				}
			}

			public DiseaseExposureInfo GetExposureInfo()
			{
				return base.master.ExposureInfo;
			}

			private object[] componentData;
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
				}).DoNotification((DiseaseInstance.StatesInstance smi) => smi.master.notification).Update("UpdateProgress", delegate(DiseaseInstance.StatesInstance smi)
				{
					smi.UpdateProgress();
				})
					.ToggleStatusItem((DiseaseInstance.StatesInstance smi) => smi.master.GetStatusItem(), (DiseaseInstance.StatesInstance smi) => smi)
					.ParamTransition<float>(this.percentRecovered, this.cured, (DiseaseInstance.StatesInstance smi, float p) => p > 1f)
					.ParamTransition<float>(this.percentDied, this.fatality_pre, (DiseaseInstance.StatesInstance smi, float p) => p > 1f);
				this.cured.Enter("Cure", delegate(DiseaseInstance.StatesInstance smi)
				{
					smi.master.Cure();
				});
				this.fatality_pre.Enter("DeathByDisease", delegate(DiseaseInstance.StatesInstance smi)
				{
					DeathMonitor.Instance smi2 = smi.master.gameObject.GetSMI<DeathMonitor.Instance>();
					smi2.Kill(Db.Get().Deaths.FatalDisease);
				}).GoTo(this.fatality);
				this.fatality.DoNothing();
			}

			public StateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance, object>.FloatParameter percentRecovered;

			public StateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance, object>.FloatParameter percentDied;

			public GameStateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance, object>.State infected;

			public GameStateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance, object>.State cured;

			public GameStateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance, object>.State fatality_pre;

			public GameStateMachine<DiseaseInstance.States, DiseaseInstance.StatesInstance, DiseaseInstance, object>.State fatality;
		}
	}
}
