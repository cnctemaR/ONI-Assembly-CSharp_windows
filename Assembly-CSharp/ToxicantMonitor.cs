using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ToxicantMonitor : GameStateMachine<ToxicantMonitor, ToxicantMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = false;
		this.satisfied.EventTransition(GameHashes.EnteredToxicArea, this.toxicarea, (ToxicantMonitor.Instance smi) => smi.IsInToxicArea());
		this.toxicarea.EventTransition(GameHashes.ExitedToxicArea, this.satisfied, (ToxicantMonitor.Instance smi) => !smi.IsInToxicArea()).Enter("AddToxicityModifier", delegate(ToxicantMonitor.Instance smi)
		{
			smi.AddToxicityModifier();
		}).Update("UpdateInToxicArea", delegate(ToxicantMonitor.Instance smi)
		{
			smi.UpdateInToxicArea();
		})
			.ToggleSchedulePeriodic("Disease Exposure", 5f, delegate(ToxicantMonitor.Instance smi)
			{
				smi.ExposeToDiseases();
			})
			.Exit("RemoveToxicityModifier", delegate(ToxicantMonitor.Instance smi)
			{
				smi.RemoveToxicityModifier();
			});
	}

	private const float DiseaseExposureInterval = 5f;

	public GameStateMachine<ToxicantMonitor, ToxicantMonitor.Instance, IStateMachineTarget>.State satisfied;

	public GameStateMachine<ToxicantMonitor, ToxicantMonitor.Instance, IStateMachineTarget>.State toxicarea;

	public new class Instance : GameStateMachine<ToxicantMonitor, ToxicantMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.sensor = master.GetComponent<Sensors>().GetSensor<ToxicantSensor>();
			this.modifier = new AttributeModifier(Db.Get().Amounts.Toxicity.deltaAttribute.Id, 0f, DUPLICANTS.MODIFIERS.TOXICENVIRONMENT.NAME, false);
			if (ToxicantMonitor.Instance.infectiousSubstances.Count == 0)
			{
				ToxicantMonitor.Instance.infectiousSubstances[SimHashes.ContaminatedOxygen] = new Disease[]
				{
					Db.Get().Diseases.PutridOdour,
					Db.Get().Diseases.Spores
				};
				ToxicantMonitor.Instance.infectiousSubstances[SimHashes.DirtyWater] = new Disease[]
				{
					Db.Get().Diseases.PutridOdour,
					Db.Get().Diseases.Spores
				};
			}
		}

		public void AddToxicityModifier()
		{
			base.gameObject.GetAttributes().Add("Toxicant", this.modifier);
		}

		public void RemoveToxicityModifier()
		{
			base.gameObject.GetAttributes().Remove(this.modifier);
		}

		public void UpdateInToxicArea()
		{
			this.UpdateDirtiness();
		}

		public bool IsInToxicArea()
		{
			return this.sensor.IsInToxicArea();
		}

		public void ExposeToDiseases()
		{
			int num = Grid.CellAbove(Grid.PosToCell(base.master.transform.position));
			Element element = Grid.Element[num];
			Disease[] array = null;
			if (ToxicantMonitor.Instance.infectiousSubstances.TryGetValue(element.id, out array))
			{
				int num2 = global::UnityEngine.Random.Range(0, array.Length);
				Disease disease = array[num2];
				string infectionSourceInfo = ToxicantMonitor.Instance.GetInfectionSourceInfo();
				base.master.Trigger(-283306403, new DiseaseExposureInfo(disease.Id, 1f, infectionSourceInfo));
			}
		}

		private static string GetInfectionSourceInfo()
		{
			return DUPLICANTS.DISEASES.INFECTIONSOURCES.TOXIC_AREA;
		}

		private void UpdateToxicityModifier()
		{
			float toxicity = this.sensor.GetToxicity();
			float num = toxicity * 20f;
			this.modifier.Value = num;
		}

		private void UpdateDirtiness()
		{
			int num = Grid.PosToCell(base.master.transform.position);
			int num2 = Grid.CellAbove(num);
			Element element = Grid.Element[num];
			Element element2 = Grid.Element[num2];
			if ((element.IsLiquid && element.id != SimHashes.Water) || (element2.IsLiquid && element2.id != SimHashes.Water))
			{
				base.master.GetComponent<Effects>().Add("Unclean", true);
			}
			if (element2.id == SimHashes.DirtyWater)
			{
				base.master.GetComponent<Effects>().Add("DirtyHands", true);
				base.master.GetComponent<Effects>().Add("Unclean", true);
			}
		}

		private ToxicantSensor sensor;

		private AttributeModifier modifier;

		private static Dictionary<SimHashes, Disease[]> infectiousSubstances = new Dictionary<SimHashes, Disease[]>();

		private static Disease[] liquidExposures;

		private static Disease[] gasExposures;
	}
}
