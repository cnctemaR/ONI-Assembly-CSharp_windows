using System;
using UnityEngine;

namespace Klei.AI
{
	public class Diseases : Modifications<Disease, DiseaseInstance>
	{
		public Diseases(GameObject go)
			: base(go, Db.Get().Diseases)
		{
		}

		public static bool CanCure(string diseaseCure, string diseaseName)
		{
			return diseaseCure == diseaseName || diseaseCure == "ALLDISEASES";
		}

		public void Infect(Disease disease, DiseaseExposureInfo exposure_info)
		{
			if (!base.Has(disease))
			{
				DiseaseInstance diseaseInstance = this.CreateInstance(disease);
				diseaseInstance.ExposureInfo = exposure_info;
			}
		}

		public override DiseaseInstance CreateInstance(Disease disease)
		{
			DiseaseInstance diseaseInstance = new DiseaseInstance(base.gameObject, disease);
			base.Add(diseaseInstance);
			base.Trigger(GameHashes.DiseaseAdded, diseaseInstance);
			ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, 1f, null);
			return diseaseInstance;
		}

		public bool IsInfected()
		{
			return base.Count > 0;
		}

		public void ApplyPill(Disease disease, string pillName, float multiplier)
		{
			DiseaseInstance diseaseInstance = null;
			foreach (DiseaseInstance diseaseInstance2 in this)
			{
				if (diseaseInstance2.modifier.Id == disease.Id)
				{
					diseaseInstance = diseaseInstance2;
					break;
				}
			}
			if (diseaseInstance != null)
			{
				diseaseInstance.AddCureSpeedMultiplier(pillName, multiplier);
			}
		}

		public void AddCure(string cure, float multiplier)
		{
			foreach (DiseaseInstance diseaseInstance in this)
			{
				DiseaseInstance diseaseInstance2 = diseaseInstance;
				diseaseInstance2.AddCureSpeedMultiplier(cure, multiplier);
			}
		}

		public void RemoveCure(string cure, float multiplier)
		{
			foreach (DiseaseInstance diseaseInstance in this)
			{
				DiseaseInstance diseaseInstance2 = diseaseInstance;
				diseaseInstance2.RemoveCureSpeedMultiplier(cure);
			}
		}

		public bool Cure(Disease disease)
		{
			DiseaseInstance diseaseInstance = null;
			foreach (DiseaseInstance diseaseInstance2 in this)
			{
				if (diseaseInstance2.modifier.Id == disease.Id)
				{
					diseaseInstance = diseaseInstance2;
					break;
				}
			}
			bool flag = false;
			if (diseaseInstance != null)
			{
				base.Remove(diseaseInstance);
				flag = true;
				base.Trigger(GameHashes.DiseaseCured, diseaseInstance);
				ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, -1f, null);
			}
			return flag;
		}
	}
}
