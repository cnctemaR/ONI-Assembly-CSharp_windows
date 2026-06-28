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
			this.RelaxationPoint = base.Add(new OwnableSlot("RelaxationPoint", MISC.TAGS.RELAXATION_POINT));
		}

		public OwnableSlot Bed;

		public OwnableSlot MessStation;

		public OwnableSlot Clinic;

		public OwnableSlot RelaxationPoint;
	}
}
