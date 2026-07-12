using System;
using System.Collections.Generic;

public class CustomClothingOutfits
{
	public OutfitData OutfitData
	{
		get
		{
			return this.outfitData;
		}
	}

	public static CustomClothingOutfits Instance
	{
		get
		{
			if (CustomClothingOutfits._instance == null)
			{
				CustomClothingOutfits._instance = new CustomClothingOutfits();
			}
			return CustomClothingOutfits._instance;
		}
	}

	public void EditOutfit(string outfit_name, string[] outfit_items)
	{
		this.outfitData.CustomOutfits[outfit_name] = outfit_items;
		ClothingOutfitUtility.SaveClothingOutfitData();
	}

	public void RenameOutfit(string old_outfit_name, string new_outfit_name)
	{
		if (!this.outfitData.CustomOutfits.ContainsKey(old_outfit_name))
		{
			throw new ArgumentException(string.Concat(new string[] { "Can't rename outfit \"", old_outfit_name, "\" to \"", new_outfit_name, "\": missing \"", old_outfit_name, "\" entry" }));
		}
		if (this.outfitData.CustomOutfits.ContainsKey(new_outfit_name))
		{
			throw new ArgumentException(string.Concat(new string[] { "Can't rename outfit \"", old_outfit_name, "\" to \"", new_outfit_name, "\": entry \"", new_outfit_name, "\" already exists" }));
		}
		this.outfitData.CustomOutfits.Add(new_outfit_name, this.outfitData.CustomOutfits[old_outfit_name]);
		foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> keyValuePair in this.outfitData.DuplicantOutfits)
		{
			string text;
			Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary;
			keyValuePair.Deconstruct<string, Dictionary<ClothingOutfitUtility.OutfitType, string>>(out text, out dictionary);
			string text2 = text;
			Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary2 = dictionary;
			if (dictionary2 != null)
			{
				using (ListPool<ClothingOutfitUtility.OutfitType, CustomClothingOutfits>.PooledList pooledList = PoolsFor<CustomClothingOutfits>.AllocateList<ClothingOutfitUtility.OutfitType>())
				{
					foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, string> keyValuePair2 in dictionary2)
					{
						ClothingOutfitUtility.OutfitType outfitType;
						keyValuePair2.Deconstruct<ClothingOutfitUtility.OutfitType, string>(out outfitType, out text);
						ClothingOutfitUtility.OutfitType outfitType2 = outfitType;
						if (text == old_outfit_name)
						{
							pooledList.Add(outfitType2);
						}
					}
					foreach (ClothingOutfitUtility.OutfitType outfitType3 in pooledList)
					{
						dictionary2[outfitType3] = new_outfit_name;
						Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(text2);
						if (personalityFromNameStringKey.IsNullOrDestroyed())
						{
							DebugUtil.DevAssert(false, string.Concat(new string[] { "<Renaming Outfit Error> Couldn't find personality \"", text2, "\" to switch their outfit preference from \"", old_outfit_name, "\" to \"", new_outfit_name, "\"" }), null);
						}
						else
						{
							personalityFromNameStringKey.SetOutfit(outfitType3, new_outfit_name);
						}
					}
				}
			}
		}
		this.outfitData.CustomOutfits.Remove(old_outfit_name);
		ClothingOutfitUtility.SaveClothingOutfitData();
	}

	public void RemoveOutfit(string outfit_name)
	{
		if (this.outfitData.CustomOutfits.Remove(outfit_name))
		{
			foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> keyValuePair in this.outfitData.DuplicantOutfits)
			{
				string text;
				Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary;
				keyValuePair.Deconstruct<string, Dictionary<ClothingOutfitUtility.OutfitType, string>>(out text, out dictionary);
				string text2 = text;
				Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary2 = dictionary;
				if (dictionary2 != null)
				{
					using (ListPool<ClothingOutfitUtility.OutfitType, CustomClothingOutfits>.PooledList pooledList = PoolsFor<CustomClothingOutfits>.AllocateList<ClothingOutfitUtility.OutfitType>())
					{
						foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, string> keyValuePair2 in dictionary2)
						{
							ClothingOutfitUtility.OutfitType outfitType;
							keyValuePair2.Deconstruct<ClothingOutfitUtility.OutfitType, string>(out outfitType, out text);
							ClothingOutfitUtility.OutfitType outfitType2 = outfitType;
							if (text == outfit_name)
							{
								pooledList.Add(outfitType2);
							}
						}
						foreach (ClothingOutfitUtility.OutfitType outfitType3 in pooledList)
						{
							dictionary2.Remove(outfitType3);
							Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(text2);
							if (personalityFromNameStringKey.IsNullOrDestroyed())
							{
								DebugUtil.DevAssert(false, "<Deleting Outfit Error> Couldn't find personality \"" + text2 + "\" to clear their outfit preference", null);
							}
							else
							{
								personalityFromNameStringKey.SetOutfit(outfitType3, Option.None);
							}
						}
					}
				}
			}
			ClothingOutfitUtility.SaveClothingOutfitData();
		}
	}

	public void SetDuplicantPerosonalityOutfit(string personalityId, Option<string> outfit_id, ClothingOutfitUtility.OutfitType outfit_type)
	{
		Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary;
		if (outfit_id.HasValue)
		{
			if (!this.outfitData.DuplicantOutfits.ContainsKey(personalityId))
			{
				this.outfitData.DuplicantOutfits.Add(personalityId, new Dictionary<ClothingOutfitUtility.OutfitType, string>());
			}
			this.outfitData.DuplicantOutfits[personalityId][outfit_type] = outfit_id.Value;
		}
		else if (this.outfitData.DuplicantOutfits.TryGetValue(personalityId, out dictionary))
		{
			dictionary.Remove(outfit_type);
			if (dictionary.Count == 0)
			{
				this.outfitData.DuplicantOutfits.Remove(personalityId);
			}
		}
		ClothingOutfitUtility.SaveClothingOutfitData();
	}

	private OutfitData outfitData = new OutfitData();

	private static CustomClothingOutfits _instance;
}
