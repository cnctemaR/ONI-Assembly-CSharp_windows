using System;
using Klei.AI;
using STRINGS;

namespace Database
{
	public class AttributeConverters : ResourceSet<AttributeConverter>
	{
		public AttributeConverters()
		{
			ToPercentAttributeFormatter toPercentAttributeFormatter = new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None);
			this.MovementSpeed = this.Create("MovementSpeed", "Movement Speed", DUPLICANTS.ATTRIBUTES.ATHLETICS.SPEEDMODIFIER, "Athletics", 0.1f, 0f, toPercentAttributeFormatter);
			this.ConstructionSpeed = this.Create("ConstructionSpeed", "Construction Speed", DUPLICANTS.ATTRIBUTES.CONSTRUCTION.SPEEDMODIFIER, "Construction", 0.25f, 0f, toPercentAttributeFormatter);
			this.DiggingSpeed = this.Create("DiggingSpeed", "Digging Speed", DUPLICANTS.ATTRIBUTES.DIGGING.SPEEDMODIFIER, "Digging", 0.25f, 0f, toPercentAttributeFormatter);
			this.MachinerySpeed = this.Create("MachinerySpeed", "Machinery Speed", DUPLICANTS.ATTRIBUTES.MACHINERY.SPEEDMODIFIER, "Machinery", 0.1f, 0f, toPercentAttributeFormatter);
			this.TemperatureInsulation = this.Create("TemperatureInsulation", "Temperature Insulation", DUPLICANTS.ATTRIBUTES.INSULATION.SPEEDMODIFIER, "Insulation", 0.1f, 0f, toPercentAttributeFormatter);
			this.ResearchSpeed = this.Create("ResearchSpeed", "Research Speed", DUPLICANTS.ATTRIBUTES.LEARNING.RESEARCHSPEED, "Learning", 0.1f, 0f, toPercentAttributeFormatter);
			this.TrainingSpeed = this.Create("TrainingSpeed", "Training Speed", DUPLICANTS.ATTRIBUTES.LEARNING.SPEEDMODIFIER, "Learning", 0.1f, 0f, toPercentAttributeFormatter);
			this.CookingSpeed = this.Create("CookingSpeed", "Cooking Speed", DUPLICANTS.ATTRIBUTES.COOKING.SPEEDMODIFIER, "Cooking", 0.1f, 0f, toPercentAttributeFormatter);
			this.ArtSpeed = this.Create("ArtSpeed", "Art Speed", DUPLICANTS.ATTRIBUTES.ART.SPEEDMODIFIER, "Art", 0.1f, 0f, toPercentAttributeFormatter);
			this.CarryAmount = this.Create("CarryAmount", "Carry Amount", DUPLICANTS.ATTRIBUTES.STRENGTH.SPEEDMODIFIER, "Strength", 15f, 0f, null);
			this.ToiletSpeed = this.Create("ToiletSpeed", "Toilet Speed", string.Empty, "ToiletEfficiency", 1f, -1f, toPercentAttributeFormatter);
			this.ImmuneLevelBoost = this.Create("ImmuneLevelBoost", "Immune Level Boost", DUPLICANTS.ATTRIBUTES.IMMUNITY.BOOST_MODIFIER, "Immunity", 0.0016666667f, 0f, new ToPercentAttributeFormatter(100f, GameUtil.TimeSlice.PerCycle));
			this.HealingSpeed = this.Create("HealingSpeed", "Healing Speed", DUPLICANTS.ATTRIBUTES.MEDICAL.SPEEDMODIFIER, "Medical", 0.2f, 0f, toPercentAttributeFormatter);
		}

		public AttributeConverter Create(string id, string name, string description, string attribute_name, float multiplier, float base_value, IAttributeFormatter formatter)
		{
			Klei.AI.Attribute attribute = Db.Get().Attributes.Get(attribute_name);
			AttributeConverter attributeConverter = new AttributeConverter(id, name, description, multiplier, base_value, attribute, formatter);
			base.Add(attributeConverter);
			attribute.converters.Add(attributeConverter);
			return attributeConverter;
		}

		public AttributeConverter MovementSpeed;

		public AttributeConverter ConstructionSpeed;

		public AttributeConverter DiggingSpeed;

		public AttributeConverter MachinerySpeed;

		public AttributeConverter TemperatureInsulation;

		public AttributeConverter ResearchSpeed;

		public AttributeConverter TrainingSpeed;

		public AttributeConverter CookingSpeed;

		public AttributeConverter ArtSpeed;

		public AttributeConverter CarryAmount;

		public AttributeConverter ToiletSpeed;

		public AttributeConverter ImmuneLevelBoost;

		public AttributeConverter HealingSpeed;
	}
}
