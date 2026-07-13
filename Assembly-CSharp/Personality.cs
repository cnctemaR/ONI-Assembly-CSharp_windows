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
		: this(name_string_key, name, Gender, PersonalityType, StressTrait, JoyTrait, StickerType, CongenitalTrait, headShape, mouth, neck, eyes, hair, body, 0, 0, 0, 0, 0, 0, headShape, headShape, description, true, "", GameTags.Minions.Models.Standard, 0)
	{
	}

	[Obsolete("Modders: Added additional body part customization to duplicant personalities")]
	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, string description, bool isStartingMinion)
		: this(name_string_key, name, Gender, PersonalityType, StressTrait, JoyTrait, StickerType, CongenitalTrait, headShape, mouth, neck, eyes, hair, body, 0, 0, 0, 0, 0, 0, headShape, headShape, description, true, "", GameTags.Minions.Models.Standard, 0)
	{
	}

	[Obsolete("Modders: Added a custom gravestone image to duplicant personalities")]
	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, int belt, int cuff, int foot, int hand, int pelvis, int leg, string description, bool isStartingMinion)
		: this(name_string_key, name, Gender, PersonalityType, StressTrait, JoyTrait, StickerType, CongenitalTrait, headShape, mouth, neck, eyes, hair, body, 0, 0, 0, 0, 0, 0, headShape, headShape, description, isStartingMinion, "", GameTags.Minions.Models.Standard, 0)
	{
	}

	[Obsolete("Modders: Added 'model', 'arm_skin' and 'leg skin' to duplicant personalities")]
	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, int belt, int cuff, int foot, int hand, int pelvis, int leg, string description, bool isStartingMinion, string graveStone)
		: this(name_string_key, name, Gender, PersonalityType, StressTrait, JoyTrait, StickerType, CongenitalTrait, headShape, mouth, neck, eyes, hair, body, 0, 0, 0, 0, 0, 0, headShape, headShape, description, isStartingMinion, "", GameTags.Minions.Models.Standard, 0)
	{
	}

	[Obsolete("Modders: Added override_speech_mouth to duplicant personalities")]
	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, int belt, int cuff, int foot, int hand, int pelvis, int leg, int arm_skin, int leg_skin, string description, bool isStartingMinion, string graveStone, Tag model)
		: this(name_string_key, name, Gender, PersonalityType, StressTrait, JoyTrait, StickerType, CongenitalTrait, headShape, mouth, neck, eyes, hair, body, belt, cuff, foot, hand, pelvis, leg, arm_skin, leg_skin, description, isStartingMinion, graveStone, model, 0)
	{
	}

	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, int belt, int cuff, int foot, int hand, int pelvis, int leg, int arm_skin, int leg_skin, string description, bool isStartingMinion, string graveStone, Tag model, int SpeechMouth)
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
		this.arm_skin = arm_skin;
		this.leg_skin = leg_skin;
		this.startingMinion = isStartingMinion;
		this.graveStone = graveStone;
		this.model = model;
		this.speech_mouth = SpeechMouth;
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
		CustomClothingOutfits.Instance.Internal_SetDuplicantPersonalityOutfit(outfitType, this.Id, outfit);
	}

	public string GetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType outfitType)
	{
		string text;
		if (CustomClothingOutfits.Instance.Internal_TryGetDuplicantPersonalityOutfit(outfitType, this.Id, out text))
		{
			return text;
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

	public int leg_skin;

	public int arm_skin;

	public int speech_mouth;

	public string nameStringKey;

	public string genderStringKey;

	public string personalityType;

	public Tag model;

	public string stresstrait;

	public string joyTrait;

	public string stickerType;

	public string congenitaltrait;

	public string unformattedDescription;

	public string graveStone;

	public bool startingMinion;

	public string requiredDlcId;

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
