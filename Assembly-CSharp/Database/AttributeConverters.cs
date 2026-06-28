using System;
using Klei.AI;
using STRINGS;

namespace Database
{
	public class AttributeConverters : ResourceSet<AttributeConverter>
	{
		public AttributeConverters()
		{
			this.MovementSpeed = this.Create("MovementSpeed", "Movement Speed", DUPLICANTS.ATTRIBUTES.ATHLETICS.SPEEDMODIFIER, "Athletics", 0.1f, 0f, true);
			this.ConstructionSpeed = this.Create("ConstructionSpeed", "Construction Speed", DUPLICANTS.ATTRIBUTES.CONSTRUCTION.SPEEDMODIFIER, "Construction", 0.25f, 0f, true);
			this.DiggingSpeed = this.Create("DiggingSpeed", "Digging Speed", DUPLICANTS.ATTRIBUTES.DIGGING.SPEEDMODIFIER, "Digging", 0.25f, 0f, true);
			this.MachinerySpeed = this.Create("MachinerySpeed", "Machinery Speed", DUPLICANTS.ATTRIBUTES.MACHINERY.SPEEDMODIFIER, "Machinery", 0.1f, 0f, true);
			this.TemperatureInsulation = this.Create("TemperatureInsulation", "Temperature Insulation", DUPLICANTS.ATTRIBUTES.INSULATION.SPEEDMODIFIER, "Insulation", 0.1f, 0f, true);
			this.ResearchPoints = this.Create("ResearchPoints", "Research Points", DUPLICANTS.ATTRIBUTES.LEARNING.RESEARCHPOINTS, "Learning", 3f, 20f, false);
			this.TrainingSpeed = this.Create("TrainingSpeed", "Training Speed", DUPLICANTS.ATTRIBUTES.LEARNING.SPEEDMODIFIER, "Learning", 0.1f, 0f, true);
			this.CookingSpeed = this.Create("CookingSpeed", "Cooking Speed", DUPLICANTS.ATTRIBUTES.COOKING.SPEEDMODIFIER, "Cooking", 0.1f, 0f, true);
			this.DiseaseResistance = this.Create("DiseaseResistance", "Disease Resistance", DUPLICANTS.ATTRIBUTES.MEDICAL.SPEEDMODIFIER, "Medical", 0.1f, 0f, true);
			this.ArtSpeed = this.Create("ArtSpeed", "Art Speed", DUPLICANTS.ATTRIBUTES.ART.SPEEDMODIFIER, "Art", 0.1f, 0f, true);
			this.CarryAmount = this.Create("CarryAmount", "Carry Amount", DUPLICANTS.ATTRIBUTES.STRENGTH.SPEEDMODIFIER, "Strength", 15f, 0f, false);
			this.ToiletSpeed = this.Create("ToiletSpeed", "Toilet Speed", string.Empty, "ToiletEfficiency", 10f, 1f, false);
		}

		public AttributeConverter Create(string id, string name, string description, string attribute_name, float multiplier, float base_value, bool is_percent)
		{
			Klei.AI.Attribute attribute = Db.Get().Attributes.Get(attribute_name);
			AttributeConverter attributeConverter = new AttributeConverter(id, name, description, multiplier, base_value, attribute, is_percent);
			base.Add(attributeConverter);
			attribute.converters.Add(attributeConverter);
			return attributeConverter;
		}

		public AttributeConverter MovementSpeed;

		public AttributeConverter ConstructionSpeed;

		public AttributeConverter DiggingSpeed;

		public AttributeConverter MachinerySpeed;

		public AttributeConverter TemperatureInsulation;

		public AttributeConverter ResearchPoints;

		public AttributeConverter TrainingSpeed;

		public AttributeConverter CookingSpeed;

		public AttributeConverter DiseaseResistance;

		public AttributeConverter ArtSpeed;

		public AttributeConverter CarryAmount;

		public AttributeConverter ToiletSpeed;
	}
}
