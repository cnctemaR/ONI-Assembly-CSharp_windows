using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public static class Expectations
{
	public static Expectation PrivateRoom = new Expectation("PrivateRoom", UI.ROLES_SCREEN.EXPECTATIONS.PRIVATE_ROOM.NAME, UI.ROLES_SCREEN.EXPECTATIONS.PRIVATE_ROOM.DESCRIPTION, delegate(MinionResume identity)
	{
	}, delegate(MinionResume identity)
	{
	});

	private static AttributeModifier Attribute_Modifier_FoodQuality_Minor = new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 1f, DUPLICANTS.NEEDS.FOOD_QUALITY.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation FoodQuality_Minor = new AttributeModifierExpectation("FoodQuality_Minor", UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.MINOR.NAME, UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.MINOR.DESCRIPTION, Expectations.Attribute_Modifier_FoodQuality_Minor, Assets.GetSprite("icon_category_food"));

	private static AttributeModifier Attribute_Modifier_FoodQuality_Medium = new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 2f, DUPLICANTS.NEEDS.FOOD_QUALITY.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation FoodQuality_Medium = new AttributeModifierExpectation("FoodQuality_Medium", UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.MEDIUM.NAME, UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.MEDIUM.DESCRIPTION, Expectations.Attribute_Modifier_FoodQuality_Medium, Assets.GetSprite("icon_category_food"));

	private static AttributeModifier Attribute_Modifier_FoodQuality_High = new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 3f, DUPLICANTS.NEEDS.FOOD_QUALITY.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation FoodQuality_High = new AttributeModifierExpectation("FoodQuality_High", UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.HIGH.NAME, UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.HIGH.DESCRIPTION, Expectations.Attribute_Modifier_FoodQuality_High, Assets.GetSprite("icon_category_food"));

	private static AttributeModifier Attribute_Modifier_FoodQuality_VeryHigh = new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 4f, DUPLICANTS.NEEDS.FOOD_QUALITY.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation FoodQuality_VeryHigh = new AttributeModifierExpectation("FoodQuality_VeryHigh", UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.VERY_HIGH.NAME, UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.VERY_HIGH.DESCRIPTION, Expectations.Attribute_Modifier_FoodQuality_VeryHigh, Assets.GetSprite("icon_category_food"));

	private static AttributeModifier Attribute_Modifier_FoodQuality_Exceptional = new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 5f, DUPLICANTS.NEEDS.FOOD_QUALITY.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation FoodQuality_Exceptional = new AttributeModifierExpectation("FoodQuality_Exceptional", UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.EXCEPTIONAL.NAME, UI.ROLES_SCREEN.EXPECTATIONS.FOOD_QUALITY.EXCEPTIONAL.DESCRIPTION, Expectations.Attribute_Modifier_FoodQuality_Exceptional, Assets.GetSprite("icon_category_food"));

	private static AttributeModifier Attribute_Modifier_Decor_Minor = new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 25f, DUPLICANTS.NEEDS.DECOR.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation Decor_Minor = new AttributeModifierExpectation("Decor_Minor", UI.ROLES_SCREEN.EXPECTATIONS.DECOR.MINOR.NAME, UI.ROLES_SCREEN.EXPECTATIONS.DECOR.MINOR.DESCRIPTION, Expectations.Attribute_Modifier_Decor_Minor, Assets.GetSprite("icon_category_decor"));

	private static AttributeModifier Attribute_Modifier_Decor_Medium = new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 50f, DUPLICANTS.NEEDS.DECOR.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation Decor_Medium = new AttributeModifierExpectation("Decor_Medium", UI.ROLES_SCREEN.EXPECTATIONS.DECOR.MEDIUM.NAME, UI.ROLES_SCREEN.EXPECTATIONS.DECOR.MEDIUM.DESCRIPTION, Expectations.Attribute_Modifier_Decor_Medium, Assets.GetSprite("icon_category_decor"));

	private static AttributeModifier Attribute_Modifier_Decor_High = new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 75f, DUPLICANTS.NEEDS.DECOR.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation Decor_High = new AttributeModifierExpectation("Decor_High", UI.ROLES_SCREEN.EXPECTATIONS.DECOR.HIGH.NAME, UI.ROLES_SCREEN.EXPECTATIONS.DECOR.HIGH.DESCRIPTION, Expectations.Attribute_Modifier_Decor_High, Assets.GetSprite("icon_category_decor"));

	private static AttributeModifier Attribute_Modifier_Decor_VeryHigh = new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 100f, DUPLICANTS.NEEDS.DECOR.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation Decor_VeryHigh = new AttributeModifierExpectation("Decor_VeryHigh", UI.ROLES_SCREEN.EXPECTATIONS.DECOR.VERY_HIGH.NAME, UI.ROLES_SCREEN.EXPECTATIONS.DECOR.VERY_HIGH.DESCRIPTION, Expectations.Attribute_Modifier_Decor_VeryHigh, Assets.GetSprite("icon_category_decor"));

	private static AttributeModifier Attribute_Modifier_Decor_Unreasonable = new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 125f, DUPLICANTS.NEEDS.DECOR.EXPECTATION_MOD_NAME, false, false, true);

	public static AttributeModifierExpectation Decor_Unreasonable = new AttributeModifierExpectation("Decor_Unreasonable", UI.ROLES_SCREEN.EXPECTATIONS.DECOR.UNREASONABLE.NAME, UI.ROLES_SCREEN.EXPECTATIONS.DECOR.UNREASONABLE.DESCRIPTION, Expectations.Attribute_Modifier_Decor_Unreasonable, Assets.GetSprite("icon_category_decor"));

	public static List<Expectation[]> ExpectationsByTier = new List<Expectation[]>
	{
		new Expectation[0],
		new Expectation[] { Expectations.Decor_Minor },
		new Expectation[]
		{
			Expectations.FoodQuality_Minor,
			Expectations.Decor_Minor
		},
		new Expectation[]
		{
			Expectations.FoodQuality_Minor,
			Expectations.Decor_Medium
		},
		new Expectation[]
		{
			Expectations.FoodQuality_Medium,
			Expectations.Decor_Medium
		},
		new Expectation[]
		{
			Expectations.FoodQuality_Medium,
			Expectations.Decor_High
		},
		new Expectation[]
		{
			Expectations.FoodQuality_High,
			Expectations.Decor_High
		},
		new Expectation[]
		{
			Expectations.FoodQuality_High,
			Expectations.Decor_VeryHigh
		},
		new Expectation[]
		{
			Expectations.FoodQuality_VeryHigh,
			Expectations.Decor_VeryHigh
		},
		new Expectation[]
		{
			Expectations.FoodQuality_VeryHigh,
			Expectations.Decor_Unreasonable
		}
	};
}
