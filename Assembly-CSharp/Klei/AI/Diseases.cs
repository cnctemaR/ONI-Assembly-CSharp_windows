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

		public void Infect(DiseaseExposureInfo exposure_info)
		{
			Disease disease = Db.Get().Diseases.Get(exposure_info.diseaseID);
			if (!base.Has(disease))
			{
				DiseaseInstance diseaseInstance = this.CreateInstance(disease);
				diseaseInstance.ExposureInfo = exposure_info;
			}
		}

		public override DiseaseInstance CreateInstance(Disease disease)
		{
			DiseaseInstance diseaseInstance = new DiseaseInstance(base.gameObject, disease);
			this.Add(diseaseInstance);
			base.Trigger(GameHashes.DiseaseAdded, diseaseInstance);
			ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, 1f, base.gameObject.GetProperName(), null);
			return diseaseInstance;
		}

		public bool IsInfected()
		{
			return base.Count > 0;
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
				this.Remove(diseaseInstance);
				flag = true;
				base.Trigger(GameHashes.DiseaseCured, diseaseInstance);
				ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, -1f, base.gameObject.GetProperName(), null);
			}
			return flag;
		}
	}
}
