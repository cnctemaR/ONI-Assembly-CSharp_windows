using System;
using UnityEngine;

public readonly struct JoyResponseOutfitTarget
{
	public JoyResponseOutfitTarget(JoyResponseOutfitTarget.Implementation impl)
	{
		this.impl = impl;
	}

	public Option<string> ReadFacadeId()
	{
		return this.impl.ReadFacadeId();
	}

	public void WriteFacadeId(Option<string> facadeId)
	{
		this.impl.WriteFacadeId(facadeId);
	}

	public string GetMinionName()
	{
		return this.impl.GetMinionName();
	}

	public Personality GetPersonality()
	{
		return this.impl.GetPersonality();
	}

	public static JoyResponseOutfitTarget FromMinion(GameObject minionInstance)
	{
		return new JoyResponseOutfitTarget(new JoyResponseOutfitTarget.MinionInstanceTarget(minionInstance));
	}

	public static JoyResponseOutfitTarget FromPersonality(Personality personality)
	{
		return new JoyResponseOutfitTarget(new JoyResponseOutfitTarget.PersonalityTarget(personality));
	}

	private readonly JoyResponseOutfitTarget.Implementation impl;

	public interface Implementation
	{
		Option<string> ReadFacadeId();

		void WriteFacadeId(Option<string> permitId);

		string GetMinionName();

		Personality GetPersonality();
	}

	public readonly struct MinionInstanceTarget : JoyResponseOutfitTarget.Implementation
	{
		public MinionInstanceTarget(GameObject minionInstance)
		{
			this.minionInstance = minionInstance;
			this.wearableAccessorizer = minionInstance.GetComponent<WearableAccessorizer>();
		}

		public string GetMinionName()
		{
			return this.minionInstance.GetProperName();
		}

		public Personality GetPersonality()
		{
			return Db.Get().Personalities.Get(this.minionInstance.GetComponent<MinionIdentity>().personalityResourceId);
		}

		public Option<string> ReadFacadeId()
		{
			return this.wearableAccessorizer.GetJoyResponseId();
		}

		public void WriteFacadeId(Option<string> permitId)
		{
			this.wearableAccessorizer.SetJoyResponseId(permitId);
		}

		public readonly GameObject minionInstance;

		public readonly WearableAccessorizer wearableAccessorizer;
	}

	public readonly struct PersonalityTarget : JoyResponseOutfitTarget.Implementation
	{
		public PersonalityTarget(Personality personality)
		{
			this.personality = personality;
		}

		public string GetMinionName()
		{
			return this.personality.Name;
		}

		public Personality GetPersonality()
		{
			return this.personality;
		}

		public Option<string> ReadFacadeId()
		{
			return this.personality.GetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType.JoyResponse);
		}

		public void WriteFacadeId(Option<string> facadeId)
		{
			this.personality.SetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType.JoyResponse, facadeId);
		}

		public readonly Personality personality;
	}
}
