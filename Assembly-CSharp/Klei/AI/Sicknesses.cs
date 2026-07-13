using System;
using UnityEngine;

namespace Klei.AI
{
	public class Sicknesses : Modifications<Sickness, SicknessInstance>
	{
		public Sicknesses(GameObject go)
			: base(go, Db.Get().Sicknesses)
		{
		}

		public void Infect(SicknessExposureInfo exposure_info)
		{
			Sickness sickness = Db.Get().Sicknesses.Get(exposure_info.sicknessID);
			if (!base.Has(sickness))
			{
				this.CreateInstance(sickness).ExposureInfo = exposure_info;
			}
		}

		public override SicknessInstance CreateInstance(Sickness sickness)
		{
			SicknessInstance sicknessInstance = new SicknessInstance(base.gameObject, sickness);
			this.Add(sicknessInstance);
			base.Trigger(GameHashes.SicknessAdded, sicknessInstance);
			ReportManager.Instance.ReportValueWithGameObjectContext(ReportManager.ReportType.DiseaseAdded, 1f, base.gameObject, null);
			return sicknessInstance;
		}

		public bool IsInfected()
		{
			return base.Count > 0;
		}

		public bool Cure(Sickness sickness)
		{
			return this.Cure(sickness.Id);
		}

		public bool Cure(string sickness_id)
		{
			SicknessInstance sicknessInstance = null;
			foreach (SicknessInstance sicknessInstance2 in this)
			{
				if (sicknessInstance2.modifier.Id == sickness_id)
				{
					sicknessInstance = sicknessInstance2;
					break;
				}
			}
			if (sicknessInstance != null)
			{
				this.Remove(sicknessInstance);
				base.Trigger(GameHashes.SicknessCured, sicknessInstance);
				ReportManager.Instance.ReportValueWithGameObjectContext(ReportManager.ReportType.DiseaseAdded, -1f, base.gameObject, null);
				return true;
			}
			return false;
		}
	}
}
