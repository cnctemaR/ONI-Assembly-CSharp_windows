using System;

namespace Database
{
	public class RoomTypeCategories : ResourceSet<RoomTypeCategory>
	{
		private RoomTypeCategory Add(string id, string name, string colorName)
		{
			RoomTypeCategory roomTypeCategory = new RoomTypeCategory(id, name, colorName);
			base.Add(roomTypeCategory);
			return roomTypeCategory;
		}

		public RoomTypeCategories(ResourceSet parent)
			: base("RoomTypeCategories", parent)
		{
			base.Initialize();
			this.None = this.Add("None", "", "roomNone");
			this.Food = this.Add("Food", "", "roomFood");
			this.Sleep = this.Add("Sleep", "", "roomSleep");
			this.Recreation = this.Add("Recreation", "", "roomRecreation");
			this.Bathroom = this.Add("Bathroom", "", "roomBathroom");
			this.Hospital = this.Add("Hospital", "", "roomHospital");
			this.Industrial = this.Add("Industrial", "", "roomIndustrial");
			this.Agricultural = this.Add("Agricultural", "", "roomAgricultural");
			this.Park = this.Add("Park", "", "roomPark");
		}

		public RoomTypeCategory None;

		public RoomTypeCategory Food;

		public RoomTypeCategory Sleep;

		public RoomTypeCategory Recreation;

		public RoomTypeCategory Bathroom;

		public RoomTypeCategory Hospital;

		public RoomTypeCategory Industrial;

		public RoomTypeCategory Agricultural;

		public RoomTypeCategory Park;
	}
}
