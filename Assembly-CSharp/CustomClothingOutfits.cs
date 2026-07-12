using System;
using System.Collections.Generic;

public class CustomClothingOutfits
{
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

	public SerializableOutfitData.Version2 Internal_GetOutfitData()
	{
		return this.serializableOutfitData;
	}

	public void Internal_SetOutfitData(SerializableOutfitData.Version2 data)
	{
		this.serializableOutfitData = data;
	}

	public void Internal_EditOutfit(ClothingOutfitUtility.OutfitType outfit_type, string outfit_name, string[] outfit_items)
	{
		SerializableOutfitData.Version2.CustomTemplateOutfitEntry customTemplateOutfitEntry;
		if (!this.serializableOutfitData.OutfitIdToUserAuthoredTemplateOutfit.TryGetValue(outfit_name, out customTemplateOutfitEntry))
		{
			customTemplateOutfitEntry = new SerializableOutfitData.Version2.CustomTemplateOutfitEntry();
			customTemplateOutfitEntry.outfitType = outfit_type;
			customTemplateOutfitEntry.itemIds = outfit_items;
			this.serializableOutfitData.OutfitIdToUserAuthoredTemplateOutfit[outfit_name] = customTemplateOutfitEntry;
		}
		else
		{
			if (customTemplateOutfitEntry.outfitType != outfit_type)
			{
				throw new NotSupportedException(string.Format("Cannot edit outfit \"{0}\" of outfit type \"{1}\" to be an outfit of type \"{2}\"", outfit_name, customTemplateOutfitEntry.outfitType, outfit_type));
			}
			customTemplateOutfitEntry.itemIds = outfit_items;
		}
		ClothingOutfitUtility.SaveClothingOutfitData();
	}

	public void Internal_RenameOutfit(ClothingOutfitUtility.OutfitType outfit_type, string old_outfit_name, string new_outfit_name)
	{
		if (!this.serializableOutfitData.OutfitIdToUserAuthoredTemplateOutfit.ContainsKey(old_outfit_name))
		{
			throw new ArgumentException(string.Concat(new string[] { "Can't rename outfit \"", old_outfit_name, "\" to \"", new_outfit_name, "\": missing \"", old_outfit_name, "\" entry" }));
		}
		if (this.serializableOutfitData.OutfitIdToUserAuthoredTemplateOutfit.ContainsKey(new_outfit_name))
		{
			throw new ArgumentException(string.Concat(new string[] { "Can't rename outfit \"", old_outfit_name, "\" to \"", new_outfit_name, "\": entry \"", new_outfit_name, "\" already exists" }));
		}
		this.serializableOutfitData.OutfitIdToUserAuthoredTemplateOutfit.Add(new_outfit_name, this.serializableOutfitData.OutfitIdToUserAuthoredTemplateOutfit[old_outfit_name]);
		foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> keyValuePair in this.serializableOutfitData.PersonalityIdToAssignedOutfits)
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
							personalityFromNameStringKey.Internal_SetSelectedTemplateOutfitId(outfitType3, new_outfit_name);
						}
					}
				}
			}
		}
		this.serializableOutfitData.OutfitIdToUserAuthoredTemplateOutfit.Remove(old_outfit_name);
		ClothingOutfitUtility.SaveClothingOutfitData();
	}

	public void Internal_RemoveOutfit(ClothingOutfitUtility.OutfitType outfit_type, string outfit_name)
	{
		if (this.serializableOutfitData.OutfitIdToUserAuthoredTemplateOutfit.Remove(outfit_name))
		{
			foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> keyValuePair in this.serializableOutfitData.PersonalityIdToAssignedOutfits)
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
								personalityFromNameStringKey.Internal_SetSelectedTemplateOutfitId(outfitType3, Option.None);
							}
						}
					}
				}
			}
			ClothingOutfitUtility.SaveClothingOutfitData();
		}
	}

	public void Internal_SetDuplicantPersonalityOutfit(string personalityId, Option<string> outfit_id, ClothingOutfitUtility.OutfitType outfit_type)
	{
		Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary;
		if (outfit_id.HasValue)
		{
			if (!this.serializableOutfitData.PersonalityIdToAssignedOutfits.ContainsKey(personalityId))
			{
				this.serializableOutfitData.PersonalityIdToAssignedOutfits.Add(personalityId, new Dictionary<ClothingOutfitUtility.OutfitType, string>());
			}
			this.serializableOutfitData.PersonalityIdToAssignedOutfits[personalityId][outfit_type] = outfit_id.Value;
		}
		else if (this.serializableOutfitData.PersonalityIdToAssignedOutfits.TryGetValue(personalityId, out dictionary))
		{
			dictionary.Remove(outfit_type);
			if (dictionary.Count == 0)
			{
				this.serializableOutfitData.PersonalityIdToAssignedOutfits.Remove(personalityId);
			}
		}
		ClothingOutfitUtility.SaveClothingOutfitData();
	}

	private static CustomClothingOutfits _instance;

	private SerializableOutfitData.Version2 serializableOutfitData = new SerializableOutfitData.Version2();
}
