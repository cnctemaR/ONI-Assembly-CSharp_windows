using System;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ClothingWearer")]
public class ClothingWearer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.decorProvider = base.GetComponent<DecorProvider>();
		if (this.decorModifier == null)
		{
			this.decorModifier = new AttributeModifier("Decor", 0f, DUPLICANTS.MODIFIERS.CLOTHING.NAME, false, false, false);
		}
		if (this.conductivityModifier == null)
		{
			AttributeInstance attributeInstance = base.gameObject.GetAttributes().Get("ThermalConductivityBarrier");
			this.conductivityModifier = new AttributeModifier("ThermalConductivityBarrier", ClothingWearer.ClothingInfo.BASIC_CLOTHING.conductivityMod, DUPLICANTS.MODIFIERS.CLOTHING.NAME, false, false, false);
			attributeInstance.Add(this.conductivityModifier);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.decorProvider.decor.Add(this.decorModifier);
		this.decorProvider.decorRadius.Add(new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, 3f, null, false, false, true));
		Traits component = base.GetComponent<Traits>();
		string text = UI.OVERLAYS.DECOR.CLOTHING;
		if (component != null)
		{
			if (component.HasTrait("DecorUp"))
			{
				text = UI.OVERLAYS.DECOR.CLOTHING_TRAIT_DECORUP;
			}
			else if (component.HasTrait("DecorDown"))
			{
				text = UI.OVERLAYS.DECOR.CLOTHING_TRAIT_DECORDOWN;
			}
		}
		this.decorProvider.overrideName = string.Format(text, base.gameObject.GetProperName());
		if (this.currentClothing == null)
		{
			this.ChangeToDefaultClothes();
			return;
		}
		this.ChangeClothes(this.currentClothing);
	}

	public void ChangeClothes(ClothingWearer.ClothingInfo clothingInfo)
	{
		this.decorProvider.baseRadius = 3f;
		this.currentClothing = clothingInfo;
		this.conductivityModifier.Description = clothingInfo.name;
		this.conductivityModifier.SetValue(this.currentClothing.conductivityMod);
		this.decorModifier.SetValue((float)this.currentClothing.decorMod);
	}

	public void ChangeToDefaultClothes()
	{
		this.ChangeClothes(new ClothingWearer.ClothingInfo(ClothingWearer.ClothingInfo.BASIC_CLOTHING.name, ClothingWearer.ClothingInfo.BASIC_CLOTHING.decorMod, ClothingWearer.ClothingInfo.BASIC_CLOTHING.conductivityMod, ClothingWearer.ClothingInfo.BASIC_CLOTHING.homeostasisEfficiencyMultiplier));
	}

	private DecorProvider decorProvider;

	private AttributeModifier decorModifier;

	private AttributeModifier conductivityModifier;

	[Serialize]
	public ClothingWearer.ClothingInfo currentClothing;

	public class ClothingInfo
	{
		public ClothingInfo(string _name, int _decor, float _temperature, float _homeostasisEfficiencyMultiplier)
		{
			this.name = _name;
			this.decorMod = _decor;
			this.conductivityMod = _temperature;
			this.homeostasisEfficiencyMultiplier = _homeostasisEfficiencyMultiplier;
		}

		[Serialize]
		public string name = "";

		[Serialize]
		public int decorMod;

		[Serialize]
		public float conductivityMod;

		[Serialize]
		public float homeostasisEfficiencyMultiplier;

		public static readonly ClothingWearer.ClothingInfo BASIC_CLOTHING = new ClothingWearer.ClothingInfo(EQUIPMENT.PREFABS.COOL_VEST.GENERICNAME, -5, 0.0025f, -1.25f);

		public static readonly ClothingWearer.ClothingInfo WARM_CLOTHING = new ClothingWearer.ClothingInfo(EQUIPMENT.PREFABS.WARM_VEST.NAME, -10, 0.01f, -1.25f);

		public static readonly ClothingWearer.ClothingInfo COOL_CLOTHING = new ClothingWearer.ClothingInfo(EQUIPMENT.PREFABS.COOL_VEST.NAME, -10, 0.0005f, 0f);

		public static readonly ClothingWearer.ClothingInfo FANCY_CLOTHING = new ClothingWearer.ClothingInfo(EQUIPMENT.PREFABS.FUNKY_VEST.NAME, 30, 0.0025f, -1.25f);

		public static readonly ClothingWearer.ClothingInfo CUSTOM_CLOTHING = new ClothingWearer.ClothingInfo(EQUIPMENT.PREFABS.CUSTOMCLOTHING.NAME, 40, 0.0025f, -1.25f);
	}
}
