using System;

namespace Database
{
	public class RoomTypeCategories : ResourceSet<RoomTypeCategory>
	{
		private RoomTypeCategory Add(string id, string name, string colorName, string icon)
		{
			RoomTypeCategory roomTypeCategory = new RoomTypeCategory(id, name, colorName, icon);
			base.Add(roomTypeCategory);
			return roomTypeCategory;
		}

		public RoomTypeCategories(ResourceSet parent)
			: base("RoomTypeCategories", parent)
		{
			base.Initialize();
			this.None = this.Add("None", "", "roomNone", "unknown");
			this.Food = this.Add("Food", "", "roomFood", "ui_room_food");
			this.Sleep = this.Add("Sleep", "", "roomSleep", "ui_room_sleep");
			this.Recreation = this.Add("Recreation", "", "roomRecreation", "ui_room_recreational");
			this.Bathroom = this.Add("Bathroom", "", "roomBathroom", "ui_room_bathroom");
			this.Hospital = this.Add("Hospital", "", "roomHospital", "ui_room_hospital");
			this.Industrial = this.Add("Industrial", "", "roomIndustrial", "ui_room_industrial");
			this.Agricultural = this.Add("Agricultural", "", "roomAgricultural", "ui_room_agricultural");
			this.Park = this.Add("Park", "", "roomPark", "ui_room_park");
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
