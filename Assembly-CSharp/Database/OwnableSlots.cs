using System;
using STRINGS;

namespace Database
{
	public class OwnableSlots : ResourceSet<OwnableSlot>
	{
		public OwnableSlots()
		{
			this.Bed = base.Add(new OwnableSlot("Bed", MISC.TAGS.BED));
			this.MessStation = base.Add(new OwnableSlot("MessStation", MISC.TAGS.MESSSTATION));
			this.Clinic = base.Add(new OwnableSlot("Clinic", MISC.TAGS.CLINIC));
			this.MedicalBed = base.Add(new OwnableSlot("MedicalBed", MISC.TAGS.CLINIC));
			this.MedicalBed.showInUI = false;
			this.GeneShuffler = base.Add(new OwnableSlot("GeneShuffler", MISC.TAGS.GENE_SHUFFLER));
			this.GeneShuffler.showInUI = false;
			this.Toilet = base.Add(new OwnableSlot("Toilet", MISC.TAGS.TOILET));
		}

		public OwnableSlot Bed;

		public OwnableSlot MessStation;

		public OwnableSlot Clinic;

		public OwnableSlot GeneShuffler;

		public OwnableSlot MedicalBed;

		public OwnableSlot Toilet;
	}
}
