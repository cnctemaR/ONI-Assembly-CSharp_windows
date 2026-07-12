using System;
using System.Collections.Generic;
using Klei.AI;

namespace Database
{
	public class Personalities : ResourceSet<Personality>
	{
		public Personalities()
		{
			foreach (Personalities.PersonalityInfo personalityInfo in AsyncLoadManager<IGlobalAsyncLoader>.AsyncLoader<Personalities.PersonalityLoader>.Get().entries)
			{
				if (string.IsNullOrEmpty(personalityInfo.RequiredDlcId) || DlcManager.IsContentSubscribed(personalityInfo.RequiredDlcId))
				{
					base.Add(new Personality(personalityInfo.Name.ToUpper(), Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", personalityInfo.Name.ToUpper())), personalityInfo.Gender.ToUpper(), personalityInfo.PersonalityType, personalityInfo.StressTrait, personalityInfo.JoyTrait, personalityInfo.StickerType, personalityInfo.CongenitalTrait, personalityInfo.HeadShape, personalityInfo.Mouth, personalityInfo.Neck, personalityInfo.Eyes, personalityInfo.Hair, personalityInfo.Body, personalityInfo.Belt, personalityInfo.Cuff, personalityInfo.Foot, personalityInfo.Hand, personalityInfo.Pelvis, personalityInfo.Leg, Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", personalityInfo.Name.ToUpper())), personalityInfo.ValidStarter, personalityInfo.Grave)
					{
						requiredDlcId = personalityInfo.RequiredDlcId
					});
				}
			}
		}

		private void AddTrait(Personality personality, string trait_name)
		{
			Trait trait = Db.Get().traits.TryGet(trait_name);
			if (trait != null)
			{
				personality.AddTrait(trait);
			}
		}

		private void SetAttribute(Personality personality, string attribute_name, int value)
		{
			Klei.AI.Attribute attribute = Db.Get().Attributes.TryGet(attribute_name);
			if (attribute == null)
			{
				Debug.LogWarning("Attribute does not exist: " + attribute_name);
				return;
			}
			personality.SetAttribute(attribute, value);
		}

		public List<Personality> GetStartingPersonalities()
		{
			return this.resources.FindAll((Personality x) => x.startingMinion);
		}

		public List<Personality> GetAll(bool onlyEnabledMinions, bool onlyStartingMinions)
		{
			return this.resources.FindAll((Personality personality) => (!onlyStartingMinions || personality.startingMinion) && (!onlyEnabledMinions || !personality.Disabled) && (!(SaveLoader.Instance != null) || !DlcManager.IsDlcId(personality.requiredDlcId) || SaveLoader.Instance.GameInfo.dlcIds.Contains(personality.requiredDlcId)));
		}

		public Personality GetRandom(bool onlyEnabledMinions, bool onlyStartingMinions)
		{
			return this.GetAll(onlyEnabledMinions, onlyStartingMinions).GetRandom<Personality>();
		}

		public Personality GetPersonalityFromNameStringKey(string name_string_key)
		{
			foreach (Personality personality in Db.Get().Personalities.resources)
			{
				if (personality.nameStringKey.Equals(name_string_key, StringComparison.CurrentCultureIgnoreCase))
				{
					return personality;
				}
			}
			return null;
		}

		public class PersonalityLoader : AsyncCsvLoader<Personalities.PersonalityLoader, Personalities.PersonalityInfo>
		{
			public PersonalityLoader()
				: base(Assets.instance.personalitiesFile)
			{
			}

			public override void Run()
			{
				base.Run();
			}
		}

		public class PersonalityInfo : Resource
		{
			public int HeadShape;

			public int Mouth;

			public int Neck;

			public int Eyes;

			public int Hair;

			public int Body;

			public int Belt;

			public int Cuff;

			public int Foot;

			public int Hand;

			public int Pelvis;

			public int Leg;

			public string Gender;

			public string PersonalityType;

			public string StressTrait;

			public string JoyTrait;

			public string StickerType;

			public string CongenitalTrait;

			public string Design;

			public bool ValidStarter;

			public string Grave;

			public string RequiredDlcId;
		}
	}
}
