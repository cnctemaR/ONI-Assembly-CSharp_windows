using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GermExposureMonitor : GameStateMachine<GermExposureMonitor, GermExposureMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		base.serializable = false;
		this.root.EventHandler(GameHashes.EatCompleteEater, delegate(GermExposureMonitor.Instance smi, object obj)
		{
			smi.OnEatComplete(obj);
		}).EventHandler(GameHashes.SicknessAdded, delegate(GermExposureMonitor.Instance smi, object data)
		{
			smi.OnSicknessAdded(data);
		}).EventHandler(GameHashes.SicknessCured, delegate(GermExposureMonitor.Instance smi, object data)
		{
			smi.OnSicknessCured(data);
		})
			.EventHandler(GameHashes.SleepFinished, delegate(GermExposureMonitor.Instance smi)
			{
				smi.OnSleepFinished();
			});
	}

	public static float GetContractionChance(float rating)
	{
		return 0.5f - 0.5f * (float)Math.Tanh(0.5 * (double)rating);
	}

	private const int MIN_GERM_EXPOSURE_THRESHOLD = 100;

	public static GermExposureMonitor.ExposureType[] exposureTypes = new GermExposureMonitor.ExposureType[]
	{
		new GermExposureMonitor.ExposureType
		{
			germ_id = "FoodPoisoning",
			sickness_id = "FoodSickness",
			excluded_traits = new List<string> { "IronGut" },
			base_resistance = 1
		},
		new GermExposureMonitor.ExposureType
		{
			germ_id = "SlimeLung",
			sickness_id = "SlimeSickness",
			base_resistance = 2
		},
		new GermExposureMonitor.ExposureType
		{
			germ_id = "ZombieSpores",
			sickness_id = "ZombieSickness",
			exposure_threshold = 1,
			base_resistance = -1
		},
		new GermExposureMonitor.ExposureType
		{
			germ_id = "PollenGerms",
			sickness_id = "Allergies",
			exposure_threshold = 1,
			infect_immediately = true,
			required_traits = new List<string> { "Allergies" },
			excluded_effects = new List<string> { "HistamineSuppression" }
		},
		new GermExposureMonitor.ExposureType
		{
			germ_id = "PollenGerms",
			infection_effect = "SmelledFlowers",
			exposure_threshold = 1,
			infect_immediately = true,
			excluded_traits = new List<string> { "Allergies" }
		}
	};

	public enum ExposureState
	{
		None,
		Exposed,
		Contracted,
		Sick
	}

	public class ExposureType
	{
		public string germ_id;

		public string sickness_id;

		public string infection_effect;

		public int exposure_threshold = 100;

		public bool infect_immediately;

		public List<string> required_traits;

		public List<string> excluded_traits;

		public List<string> excluded_effects;

		public int base_resistance;
	}

	public class ExposureStatusData
	{
		public GermExposureMonitor.ExposureType exposure_type;

		public GermExposureMonitor.Instance owner;
	}

	public new class Instance : GameStateMachine<GermExposureMonitor, GermExposureMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.sicknesses = master.GetComponent<MinionModifiers>().sicknesses;
			this.primaryElement = master.GetComponent<PrimaryElement>();
			this.traits = master.GetComponent<Traits>();
			this.lastDiseaseSources = new Dictionary<HashedString, GermExposureMonitor.Instance.DiseaseSourceInfo>();
			GameClock.Instance.Subscribe(-722330267, new Action<object>(this.OnNightTime));
			this.modifiers = base.gameObject.GetComponent<Modifiers>();
			OxygenBreather component = base.GetComponent<OxygenBreather>();
			component.onSimConsume = (Action<Sim.MassConsumedCallback>)Delegate.Combine(component.onSimConsume, new Action<Sim.MassConsumedCallback>(this.OnAirConsumed));
		}

		public override void StartSM()
		{
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			GameClock.Instance.Unsubscribe(-722330267, new Action<object>(this.OnNightTime));
			base.StopSM(reason);
		}

		public void OnEatComplete(object obj)
		{
			Edible edible = (Edible)obj;
			HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(edible.gameObject);
			if (handle != HandleVector<int>.InvalidHandle)
			{
				DiseaseHeader header = GameComps.DiseaseContainers.GetHeader(handle);
				if (header.diseaseIdx != 255)
				{
					Disease disease = Db.Get().Diseases[(int)header.diseaseIdx];
					float num = edible.unitsConsumed / (edible.unitsConsumed + edible.Units);
					int num2 = Mathf.CeilToInt((float)header.diseaseCount * num);
					GameComps.DiseaseContainers.ModifyDiseaseCount(handle, -num2);
					KPrefabID component = edible.GetComponent<KPrefabID>();
					this.InjectDisease(disease, num2, component.PrefabID(), Sickness.InfectionVector.Digestion);
				}
			}
		}

		public void OnAirConsumed(Sim.MassConsumedCallback mass_cb_info)
		{
			if (mass_cb_info.diseaseIdx != 255)
			{
				Disease disease = Db.Get().Diseases[(int)mass_cb_info.diseaseIdx];
				this.InjectDisease(disease, mass_cb_info.diseaseCount, ElementLoader.elements[(int)mass_cb_info.elemIdx].tag, Sickness.InfectionVector.Inhalation);
			}
		}

		public void TryInjectDisease(byte disease_idx, int count, Tag source, Sickness.InfectionVector vector)
		{
			if (disease_idx != 255)
			{
				Disease disease = Db.Get().Diseases[(int)disease_idx];
				this.InjectDisease(disease, count, source, vector);
			}
		}

		public void InjectDisease(Disease disease, int count, Tag source, Sickness.InfectionVector vector)
		{
			foreach (GermExposureMonitor.ExposureType exposureType in GermExposureMonitor.exposureTypes)
			{
				if (disease.id == exposureType.germ_id && count > exposureType.exposure_threshold)
				{
					if (this.IsExposureValidForTraits(exposureType))
					{
						Sickness sickness = ((exposureType.sickness_id == null) ? null : Db.Get().Sicknesses.Get(exposureType.sickness_id));
						if ((sickness == null || sickness.infectionVectors.Contains(vector)) && this.GetExposureState(exposureType.germ_id) == GermExposureMonitor.ExposureState.None)
						{
							AttributeInstance attributeInstance = Db.Get().Attributes.GermResistance.Lookup(base.gameObject);
							float totalValue = attributeInstance.GetTotalValue();
							float num = (float)exposureType.base_resistance + totalValue;
							float contractionChance = GermExposureMonitor.GetContractionChance(num);
							if (contractionChance > 0f)
							{
								this.lastDiseaseSources[disease.id] = new GermExposureMonitor.Instance.DiseaseSourceInfo(source, vector, contractionChance);
								if (exposureType.infect_immediately)
								{
									this.InfectImmediately(exposureType);
								}
								else
								{
									this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.Exposed);
									float num2 = Mathf.Clamp01(contractionChance);
									GermExposureTracker.Instance.AddExposure(exposureType, num2);
								}
							}
						}
					}
				}
			}
			this.RefreshStatusItems();
		}

		public GermExposureMonitor.ExposureState GetExposureState(string germ_id)
		{
			GermExposureMonitor.ExposureState exposureState;
			bool flag = this.exposureStates.TryGetValue(germ_id, out exposureState);
			return exposureState;
		}

		public void SetExposureState(string germ_id, GermExposureMonitor.ExposureState exposure_state)
		{
			this.exposureStates[germ_id] = exposure_state;
			this.RefreshStatusItems();
		}

		public void ContractGerms(string germ_id)
		{
			GermExposureMonitor.ExposureState exposureState = this.GetExposureState(germ_id);
			DebugUtil.DevAssert(exposureState == GermExposureMonitor.ExposureState.Exposed, "Duplicant is contracting a sickness but was never exposed to it!");
			this.SetExposureState(germ_id, GermExposureMonitor.ExposureState.Contracted);
		}

		public void OnSicknessAdded(object sickness_instance_data)
		{
			SicknessInstance sicknessInstance = (SicknessInstance)sickness_instance_data;
			foreach (GermExposureMonitor.ExposureType exposureType in GermExposureMonitor.exposureTypes)
			{
				if (exposureType.sickness_id == sicknessInstance.Sickness.Id)
				{
					this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.Sick);
				}
			}
		}

		public void OnSicknessCured(object sickness_instance_data)
		{
			SicknessInstance sicknessInstance = (SicknessInstance)sickness_instance_data;
			foreach (GermExposureMonitor.ExposureType exposureType in GermExposureMonitor.exposureTypes)
			{
				if (exposureType.sickness_id == sicknessInstance.Sickness.Id)
				{
					this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.None);
				}
			}
		}

		private bool IsExposureValidForTraits(GermExposureMonitor.ExposureType exposure_type)
		{
			if (exposure_type.required_traits != null && exposure_type.required_traits.Count > 0)
			{
				foreach (string text in exposure_type.required_traits)
				{
					if (!this.traits.HasTrait(text))
					{
						return false;
					}
				}
			}
			if (exposure_type.excluded_traits != null && exposure_type.excluded_traits.Count > 0)
			{
				foreach (string text2 in exposure_type.excluded_traits)
				{
					if (this.traits.HasTrait(text2))
					{
						return false;
					}
				}
			}
			if (exposure_type.excluded_effects != null && exposure_type.excluded_effects.Count > 0)
			{
				Effects component = base.master.GetComponent<Effects>();
				foreach (string text3 in exposure_type.excluded_effects)
				{
					if (component.HasEffect(text3))
					{
						return false;
					}
				}
			}
			return true;
		}

		private void RefreshStatusItems()
		{
			foreach (GermExposureMonitor.ExposureType exposureType in GermExposureMonitor.exposureTypes)
			{
				Guid guid;
				this.statusItemHandles.TryGetValue(exposureType.germ_id, out guid);
				GermExposureMonitor.ExposureState exposureState = this.GetExposureState(exposureType.germ_id);
				if (guid == Guid.Empty && (exposureState == GermExposureMonitor.ExposureState.Exposed || exposureState == GermExposureMonitor.ExposureState.Contracted))
				{
					KSelectable component = base.GetComponent<KSelectable>();
					guid = component.AddStatusItem(Db.Get().DuplicantStatusItems.ExposedToGerms, new GermExposureMonitor.ExposureStatusData
					{
						exposure_type = exposureType,
						owner = this
					});
				}
				else if (guid != Guid.Empty && exposureState != GermExposureMonitor.ExposureState.Exposed && exposureState != GermExposureMonitor.ExposureState.Contracted)
				{
					KSelectable component2 = base.GetComponent<KSelectable>();
					guid = component2.RemoveStatusItem(guid, false);
				}
				this.statusItemHandles[exposureType.germ_id] = guid;
			}
		}

		private void OnNightTime(object data)
		{
			this.UpdateReports();
		}

		private void UpdateReports()
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseStatus, (float)this.primaryElement.DiseaseCount, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.GERMS, "{0}", base.master.name), base.master.gameObject.GetProperName());
		}

		public void InfectImmediately(GermExposureMonitor.ExposureType exposure_type)
		{
			if (exposure_type.infection_effect != null)
			{
				Effects component = base.master.GetComponent<Effects>();
				component.Add(exposure_type.infection_effect, true);
			}
			if (exposure_type.sickness_id != null)
			{
				string lastDiseaseSource = this.GetLastDiseaseSource(exposure_type.germ_id);
				SicknessExposureInfo sicknessExposureInfo = new SicknessExposureInfo(exposure_type.sickness_id, lastDiseaseSource);
				this.sicknesses.Infect(sicknessExposureInfo);
			}
		}

		public void OnSleepFinished()
		{
			foreach (GermExposureMonitor.ExposureType exposureType in GermExposureMonitor.exposureTypes)
			{
				if (!exposureType.infect_immediately)
				{
					GermExposureMonitor.ExposureState exposureState = this.GetExposureState(exposureType.germ_id);
					if (exposureState == GermExposureMonitor.ExposureState.Exposed)
					{
						this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.None);
					}
					if (exposureState == GermExposureMonitor.ExposureState.Contracted)
					{
						this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.Sick);
						string lastDiseaseSource = this.GetLastDiseaseSource(exposureType.germ_id);
						SicknessExposureInfo sicknessExposureInfo = new SicknessExposureInfo(exposureType.sickness_id, lastDiseaseSource);
						this.sicknesses.Infect(sicknessExposureInfo);
					}
				}
			}
		}

		public string GetLastDiseaseSource(string id)
		{
			GermExposureMonitor.Instance.DiseaseSourceInfo diseaseSourceInfo;
			string text;
			if (this.lastDiseaseSources.TryGetValue(id, out diseaseSourceInfo))
			{
				switch (diseaseSourceInfo.vector)
				{
				case Sickness.InfectionVector.Contact:
					text = DUPLICANTS.DISEASES.INFECTIONSOURCES.SKIN;
					break;
				case Sickness.InfectionVector.Digestion:
					text = string.Format(DUPLICANTS.DISEASES.INFECTIONSOURCES.FOOD, diseaseSourceInfo.sourceObject.ProperName());
					break;
				case Sickness.InfectionVector.Inhalation:
					text = string.Format(DUPLICANTS.DISEASES.INFECTIONSOURCES.AIR, diseaseSourceInfo.sourceObject.ProperName());
					break;
				default:
					text = DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
					break;
				}
			}
			else
			{
				text = DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
			}
			return text;
		}

		public float GetExposureWeight(string id)
		{
			GermExposureMonitor.Instance.DiseaseSourceInfo diseaseSourceInfo;
			if (this.lastDiseaseSources.TryGetValue(id, out diseaseSourceInfo))
			{
				return diseaseSourceInfo.factor;
			}
			return 0f;
		}

		[Serialize]
		public Dictionary<HashedString, GermExposureMonitor.Instance.DiseaseSourceInfo> lastDiseaseSources;

		private Sicknesses sicknesses;

		private PrimaryElement primaryElement;

		private Modifiers modifiers;

		private Traits traits;

		[Serialize]
		private Dictionary<string, GermExposureMonitor.ExposureState> exposureStates = new Dictionary<string, GermExposureMonitor.ExposureState>();

		private Dictionary<string, Guid> statusItemHandles = new Dictionary<string, Guid>();

		public class DiseaseSourceInfo
		{
			public DiseaseSourceInfo(Tag sourceObject, Sickness.InfectionVector vector, float factor)
			{
				this.sourceObject = sourceObject;
				this.vector = vector;
				this.factor = factor;
			}

			public Tag sourceObject;

			public Sickness.InfectionVector vector;

			public float factor;
		}
	}
}
