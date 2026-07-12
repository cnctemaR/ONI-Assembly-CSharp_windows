using System;
using Klei.AI;
using UnityEngine;

namespace Database
{
	public class Spice : Resource
	{
		public AttributeModifier StatBonus { get; private set; }

		public AttributeModifier FoodModifier { get; private set; }

		public AttributeModifier CalorieModifier { get; private set; }

		public Color PrimaryColor { get; private set; }

		public Color SecondaryColor { get; private set; }

		public string Image { get; private set; }

		public string[] DlcIds { get; private set; } = DlcManager.AVAILABLE_ALL_VERSIONS;

		public Spice(ResourceSet parent, string id, Spice.Ingredient[] ingredients, Color primaryColor, Color secondaryColor, AttributeModifier foodMod = null, AttributeModifier statBonus = null, string imageName = "unknown", string[] dlcID = null)
			: base(id, parent, null)
		{
			if (dlcID != null)
			{
				this.DlcIds = dlcID;
			}
			this.StatBonus = statBonus;
			this.FoodModifier = foodMod;
			this.Ingredients = ingredients;
			this.Image = imageName;
			this.PrimaryColor = primaryColor;
			this.SecondaryColor = secondaryColor;
			for (int i = 0; i < this.Ingredients.Length; i++)
			{
				this.TotalKG += this.Ingredients[i].AmountKG;
			}
		}

		public readonly Spice.Ingredient[] Ingredients;

		public readonly float TotalKG;

		public class Ingredient
		{
			public Tag[] IngredientSet;

			public float AmountKG;
		}
	}
}
