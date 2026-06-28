using System;

namespace Database
{
	public class OwnableSlots : ResourceSet<OwnableSlot>
	{
		public OwnableSlots()
		{
			this.Bed = base.Add(new OwnableSlot("Bed", "Bed"));
			this.MessStation = base.Add(new OwnableSlot("MessStation", "Dining Area"));
			this.Clinic = base.Add(new OwnableSlot("Clinic", "Clinic"));
			this.RelaxationPoint = base.Add(new OwnableSlot("RelaxationPoint", "Relaxation Point"));
		}

		public OwnableSlot Bed;

		public OwnableSlot MessStation;

		public OwnableSlot Clinic;

		public OwnableSlot RelaxationPoint;
	}
}
