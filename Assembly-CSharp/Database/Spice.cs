using System;
using Klei.AI;
using UnityEngine;

namespace Database
{
	public class Spice : Resource, IHasDlcRestrictions
	{
		public AttributeModifier StatBonus { get; private set; }

		public AttributeModifier FoodModifier { get; private set; }

		public AttributeModifier CalorieModifier { get; private set; }

		public Color PrimaryColor { get; private set; }

		public Color SecondaryColor { get; private set; }

		public string Image { get; private set; }

		public string[] requiredDlcIds { get; private set; }

		public Spice(ResourceSet parent, string id, Spice.Ingredient[] ingredients, Color primaryColor, Color secondaryColor, AttributeModifier foodMod = null, AttributeModifier statBonus = null, string imageName = "unknown", string[] dlcID = null)
			: base(id, parent, null)
		{
			this.requiredDlcIds = this.requiredDlcIds;
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

		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return null;
		}

		public readonly Spice.Ingredient[] Ingredients;

		public readonly float TotalKG;

		public class Ingredient : IConfigurableConsumerIngredient
		{
			public float GetAmount()
			{
				return this.AmountKG;
			}

			public Tag[] GetIDSets()
			{
				return this.IngredientSet;
			}

			public Tag[] IngredientSet;

			public float AmountKG;
		}
	}
}
