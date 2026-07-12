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
			customTemplateOutfitEntry.outfitType = Enum.GetName(typeof(ClothingOutfitUtility.OutfitType), outfit_type);
			customTemplateOutfitEntry.itemIds = outfit_items;
			this.serializableOutfitData.OutfitIdToUserAuthoredTemplateOutfit[outfit_name] = customTemplateOutfitEntry;
		}
		else
		{
			ClothingOutfitUtility.OutfitType outfitType;
			if (!Enum.TryParse<ClothingOutfitUtility.OutfitType>(customTemplateOutfitEntry.outfitType, true, out outfitType))
			{
				throw new NotSupportedException(string.Concat(new string[] { "Cannot edit outfit \"", outfit_name, "\" of unknown outfit type \"", customTemplateOutfitEntry.outfitType, "\"" }));
			}
			if (outfitType != outfit_type)
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
		foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in this.serializableOutfitData.PersonalityIdToAssignedOutfits)
		{
			string text;
			Dictionary<string, string> dictionary;
			keyValuePair.Deconstruct<string, Dictionary<string, string>>(out text, out dictionary);
			string text2 = text;
			Dictionary<string, string> dictionary2 = dictionary;
			if (dictionary2 != null)
			{
				using (ListPool<string, CustomClothingOutfits>.PooledList pooledList = PoolsFor<CustomClothingOutfits>.AllocateList<string>())
				{
					foreach (KeyValuePair<string, string> keyValuePair2 in dictionary2)
					{
						string text3;
						keyValuePair2.Deconstruct<string, string>(out text, out text3);
						string text4 = text;
						if (text3 == old_outfit_name)
						{
							pooledList.Add(text4);
						}
					}
					foreach (string text5 in pooledList)
					{
						dictionary2[text5] = new_outfit_name;
						Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(text2);
						ClothingOutfitUtility.OutfitType outfitType;
						if (personalityFromNameStringKey.IsNullOrDestroyed())
						{
							DebugUtil.DevAssert(false, string.Concat(new string[] { "<Renaming Outfit Error> Couldn't find personality \"", text2, "\" to switch their outfit preference from \"", old_outfit_name, "\" to \"", new_outfit_name, "\"" }), null);
						}
						else if (Enum.TryParse<ClothingOutfitUtility.OutfitType>(text5, true, out outfitType))
						{
							personalityFromNameStringKey.Internal_SetSelectedTemplateOutfitId(outfitType, new_outfit_name);
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
			foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in this.serializableOutfitData.PersonalityIdToAssignedOutfits)
			{
				string text;
				Dictionary<string, string> dictionary;
				keyValuePair.Deconstruct<string, Dictionary<string, string>>(out text, out dictionary);
				string text2 = text;
				Dictionary<string, string> dictionary2 = dictionary;
				if (dictionary2 != null)
				{
					using (ListPool<string, CustomClothingOutfits>.PooledList pooledList = PoolsFor<CustomClothingOutfits>.AllocateList<string>())
					{
						foreach (KeyValuePair<string, string> keyValuePair2 in dictionary2)
						{
							string text3;
							keyValuePair2.Deconstruct<string, string>(out text, out text3);
							string text4 = text;
							if (text3 == outfit_name)
							{
								pooledList.Add(text4);
							}
						}
						foreach (string text5 in pooledList)
						{
							dictionary2.Remove(text5);
							Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(text2);
							ClothingOutfitUtility.OutfitType outfitType;
							if (personalityFromNameStringKey.IsNullOrDestroyed())
							{
								DebugUtil.DevAssert(false, "<Deleting Outfit Error> Couldn't find personality \"" + text2 + "\" to clear their outfit preference", null);
							}
							else if (Enum.TryParse<ClothingOutfitUtility.OutfitType>(text5, true, out outfitType))
							{
								personalityFromNameStringKey.Internal_SetSelectedTemplateOutfitId(outfitType, Option.None);
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
		string name = Enum.GetName(typeof(ClothingOutfitUtility.OutfitType), outfit_type);
		Dictionary<string, string> dictionary;
		if (outfit_id.HasValue)
		{
			if (!this.serializableOutfitData.PersonalityIdToAssignedOutfits.ContainsKey(personalityId))
			{
				this.serializableOutfitData.PersonalityIdToAssignedOutfits.Add(personalityId, new Dictionary<string, string>());
			}
			this.serializableOutfitData.PersonalityIdToAssignedOutfits[personalityId][name] = outfit_id.Value;
		}
		else if (this.serializableOutfitData.PersonalityIdToAssignedOutfits.TryGetValue(personalityId, out dictionary))
		{
			dictionary.Remove(name);
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
