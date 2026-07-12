using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class Personality : Resource
{
	public string description
	{
		get
		{
			return this.GetDescription();
		}
	}

	[Obsolete("Modders: Use constructor with isStartingMinion parameter")]
	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, string description)
		: this(name_string_key, name, Gender, PersonalityType, StressTrait, JoyTrait, StickerType, CongenitalTrait, headShape, mouth, neck, eyes, hair, body, 0, 0, 0, 0, 0, 0, description, true)
	{
	}

	[Obsolete("Modders: Added additional body part customization to duplicant personalities")]
	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, string description, bool isStartingMinion)
		: this(name_string_key, name, Gender, PersonalityType, StressTrait, JoyTrait, StickerType, CongenitalTrait, headShape, mouth, neck, eyes, hair, body, 0, 0, 0, 0, 0, 0, description, true)
	{
	}

	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, int belt, int cuff, int foot, int hand, int pelvis, int leg, string description, bool isStartingMinion)
		: base(name_string_key, name)
	{
		this.nameStringKey = name_string_key;
		this.genderStringKey = Gender;
		this.personalityType = PersonalityType;
		this.stresstrait = StressTrait;
		this.joyTrait = JoyTrait;
		this.stickerType = StickerType;
		this.congenitaltrait = CongenitalTrait;
		this.unformattedDescription = description;
		this.headShape = headShape;
		this.mouth = mouth;
		this.neck = neck;
		this.eyes = eyes;
		this.hair = hair;
		this.body = body;
		this.belt = belt;
		this.cuff = cuff;
		this.foot = foot;
		this.hand = hand;
		this.pelvis = pelvis;
		this.leg = leg;
		this.startingMinion = isStartingMinion;
		this.outfitIds = new Dictionary<ClothingOutfitUtility.OutfitType, string>();
	}

	public string GetDescription()
	{
		this.unformattedDescription = this.unformattedDescription.Replace("{0}", this.Name);
		return this.unformattedDescription;
	}

	public void SetAttribute(Klei.AI.Attribute attribute, int value)
	{
		Personality.StartingAttribute startingAttribute = new Personality.StartingAttribute(attribute, value);
		this.attributes.Add(startingAttribute);
	}

	public void AddTrait(Trait trait)
	{
		this.traits.Add(trait);
	}

	public void SetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType outfitType, Option<string> outfit)
	{
		Db.Get().Permits.ClothingOutfits.SetDuplicantPersonalityOutfit(this.Id, outfit, outfitType);
	}

	public void Internal_SetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType outfitType, Option<string> outfit)
	{
		if (outfit.HasValue)
		{
			this.outfitIds[outfitType] = outfit.Unwrap();
			return;
		}
		this.outfitIds.Remove(outfitType);
	}

	public string GetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType outfitType)
	{
		if (this.outfitIds.ContainsKey(outfitType))
		{
			return this.outfitIds[outfitType];
		}
		return null;
	}

	public Sprite GetMiniIcon()
	{
		if (string.IsNullOrWhiteSpace(this.nameStringKey))
		{
			return Assets.GetSprite("unknown");
		}
		string text;
		if (this.nameStringKey == "MIMA")
		{
			text = "Mi-Ma";
		}
		else
		{
			text = this.nameStringKey[0].ToString() + this.nameStringKey.Substring(1).ToLower();
		}
		return Assets.GetSprite("dreamIcon_" + text);
	}

	public List<Personality.StartingAttribute> attributes = new List<Personality.StartingAttribute>();

	public List<Trait> traits = new List<Trait>();

	public int headShape;

	public int mouth;

	public int neck;

	public int eyes;

	public int hair;

	public int body;

	public int belt;

	public int cuff;

	public int foot;

	public int hand;

	public int pelvis;

	public int leg;

	public Dictionary<ClothingOutfitUtility.OutfitType, string> outfitIds;

	public string nameStringKey;

	public string genderStringKey;

	public string personalityType;

	public string stresstrait;

	public string joyTrait;

	public string stickerType;

	public string congenitaltrait;

	public string unformattedDescription;

	public bool startingMinion;

	public class StartingAttribute
	{
		public StartingAttribute(Klei.AI.Attribute attribute, int value)
		{
			this.attribute = attribute;
			this.value = value;
		}

		public Klei.AI.Attribute attribute;

		public int value;
	}
}
