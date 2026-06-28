using System;
using Klei.AI;
using UnityEngine;

namespace Database
{
	public class Personalities : ResourceSet<Personality>
	{
		public Personalities(TextAsset file)
		{
			ResourceLoader<Personalities.PersonalityInfo> resourceLoader = new ResourceLoader<Personalities.PersonalityInfo>(file);
			foreach (Personalities.PersonalityInfo personalityInfo in resourceLoader)
			{
				Personality personality = new Personality(Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", personalityInfo.Name.ToUpper())), personalityInfo.IsFemale, personalityInfo.IsMale, personalityInfo.StressTrait, personalityInfo.CongenitalTrait, personalityInfo.HeadShape, personalityInfo.Mouth, personalityInfo.Neck, personalityInfo.Eyes, personalityInfo.Hair, personalityInfo.Body, Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", personalityInfo.Name.ToUpper())));
				base.Add(personality);
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
			}
			else
			{
				personality.SetAttribute(attribute, value);
			}
		}

		public class PersonalityInfo : Resource
		{
			public bool IsFemale;

			public bool IsMale;

			public int HeadShape;

			public int Mouth;

			public int Neck;

			public int Eyes;

			public int Hair;

			public int Body;

			public string StressTrait;

			public string CongenitalTrait;

			public string Design;
		}
	}
}
