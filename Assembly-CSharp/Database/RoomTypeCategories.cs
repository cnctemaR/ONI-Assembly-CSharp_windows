using System;
using STRINGS;

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
			this.None = this.Add("None", ROOMS.CATEGORY.NONE.NAME, "roomNone", "unknown");
			this.Food = this.Add("Food", ROOMS.CATEGORY.FOOD.NAME, "roomFood", "ui_room_food");
			this.Sleep = this.Add("Sleep", ROOMS.CATEGORY.SLEEP.NAME, "roomSleep", "ui_room_sleep");
			this.Recreation = this.Add("Recreation", ROOMS.CATEGORY.RECREATION.NAME, "roomRecreation", "ui_room_recreational");
			this.Bathroom = this.Add("Bathroom", ROOMS.CATEGORY.BATHROOM.NAME, "roomBathroom", "ui_room_bathroom");
			this.Hospital = this.Add("Hospital", ROOMS.CATEGORY.HOSPITAL.NAME, "roomHospital", "ui_room_hospital");
			this.Industrial = this.Add("Industrial", ROOMS.CATEGORY.INDUSTRIAL.NAME, "roomIndustrial", "ui_room_industrial");
			this.Agricultural = this.Add("Agricultural", ROOMS.CATEGORY.AGRICULTURAL.NAME, "roomAgricultural", "ui_room_agricultural");
			this.Park = this.Add("Park", ROOMS.CATEGORY.PARK.NAME, "roomPark", "ui_room_park");
			this.Science = this.Add("Science", ROOMS.CATEGORY.SCIENCE.NAME, "roomScience", "ui_room_science");
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

		public RoomTypeCategory Science;
	}
}
