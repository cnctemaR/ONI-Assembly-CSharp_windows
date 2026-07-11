using System;
using UnityEngine;

namespace Database
{
	public class RoomTypeCategories : ResourceSet<RoomTypeCategory>
	{
		private RoomTypeCategory Add(string id, string name, Color color)
		{
			RoomTypeCategory roomTypeCategory = new RoomTypeCategory(id, name, color);
			base.Add(roomTypeCategory);
			return roomTypeCategory;
		}

		public RoomTypeCategories(ResourceSet parent)
			: base("RoomTypeCategories", parent)
		{
			base.Initialize();
			this.None = this.Add("None", "", Color.grey);
			this.Food = this.Add("Food", "", new Color(1f, 0.8862745f, 0.5176471f));
			this.Sleep = this.Add("Sleep", "", new Color(0.6392157f, 1f, 0.5176471f));
			this.Recreation = this.Add("Recreation", "", new Color(0.25882354f, 0.6431373f, 0.95686275f));
			this.Bathroom = this.Add("Bathroom", "", new Color(0.5176471f, 1f, 0.95686275f));
			this.Hospital = this.Add("Hospital", "", new Color(1f, 0.5176471f, 0.5568628f));
			this.Industrial = this.Add("Industrial", "", new Color(0.95686275f, 0.77254903f, 0.25882354f));
			this.Agricultural = this.Add("Agricultural", "", new Color(0.8039216f, 0.9490196f, 0.28235295f));
			this.Park = this.Add("Park", "", new Color(0.6745098f, 1f, 0.7411765f));
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
