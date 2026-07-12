using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Database;
using Newtonsoft.Json.Linq;
using STRINGS;

public static class ClothingOutfitUtility
{
	public static string GetName(this ClothingOutfitUtility.OutfitType self)
	{
		switch (self)
		{
		case ClothingOutfitUtility.OutfitType.Clothing:
			return UI.MINION_BROWSER_SCREEN.OUTFIT_TYPE_CLOTHING;
		case ClothingOutfitUtility.OutfitType.JoyResponse:
			return UI.MINION_BROWSER_SCREEN.OUTFIT_TYPE_JOY_RESPONSE;
		case ClothingOutfitUtility.OutfitType.AtmoSuit:
			return UI.MINION_BROWSER_SCREEN.OUTFIT_TYPE_ATMOSUIT;
		default:
			DebugUtil.DevAssert(false, string.Format("Couldn't find name for outfit type: {0}", self), null);
			return self.ToString();
		}
	}

	public static bool SaveClothingOutfitData()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = Path.Combine(Util.RootFolder(), Util.GetKleiItemUserDataFolderName());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = Path.Combine(text, ClothingOutfitUtility.OutfitFile_U47_to_Present);
		string text3 = SerializableOutfitData.ToJsonString(SerializableOutfitData.ToJson(CustomClothingOutfits.Instance.Internal_GetOutfitData()));
		bool flag = false;
		try
		{
			using (FileStream fileStream = File.Open(text2, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
			{
				flag = true;
				byte[] bytes = Encoding.UTF8.GetBytes(text3);
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}
		catch (Exception)
		{
			DebugUtil.DevAssert(false, "SaveClothingOutfitData failed", null);
		}
		return flag;
	}

	public static void LoadClothingOutfitData(ClothingOutfits dbClothingOutfits)
	{
		SerializableOutfitData.Version2 version = null;
		bool flag;
		try
		{
			string text = Path.Combine(Util.RootFolder(), Util.GetKleiItemUserDataFolderName(), ClothingOutfitUtility.OutfitFile_U47_to_Present);
			if (!File.Exists(text))
			{
				text = Path.Combine(Util.RootFolder(), Util.GetKleiItemUserDataFolderName(), ClothingOutfitUtility.OutfitFile_U44_to_U46);
				if (!File.Exists(text))
				{
					return;
				}
			}
			string text2;
			using (FileStream fileStream = File.Open(text, FileMode.Open))
			{
				using (StreamReader streamReader = new StreamReader(fileStream, new UTF8Encoding(false, true)))
				{
					text2 = streamReader.ReadToEnd();
				}
			}
			version = SerializableOutfitData.FromJson(JObject.Parse(text2));
			flag = true;
		}
		catch
		{
			flag = false;
			DebugUtil.DevAssert(false, "LoadClothingOutfitData failed", null);
		}
		if (!flag || version == null)
		{
			return;
		}
		foreach (KeyValuePair<string, SerializableOutfitData.Version2.CustomTemplateOutfitEntry> keyValuePair in version.OutfitIdToUserAuthoredTemplateOutfit)
		{
			string text3;
			SerializableOutfitData.Version2.CustomTemplateOutfitEntry customTemplateOutfitEntry;
			keyValuePair.Deconstruct<string, SerializableOutfitData.Version2.CustomTemplateOutfitEntry>(out text3, out customTemplateOutfitEntry);
			string text4 = text3;
			SerializableOutfitData.Version2.CustomTemplateOutfitEntry customTemplateOutfitEntry2 = customTemplateOutfitEntry;
			if (dbClothingOutfits.TryGet(text4) != null)
			{
				Debug.LogError(string.Format("User outfit data is trying to overwrite default {0}, outfitType is {1}", text4, customTemplateOutfitEntry2));
			}
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair2 in version.PersonalityIdToAssignedOutfits)
		{
			string text3;
			Dictionary<string, string> dictionary;
			keyValuePair2.Deconstruct<string, Dictionary<string, string>>(out text3, out dictionary);
			string text5 = text3;
			Dictionary<string, string> dictionary2 = dictionary;
			Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(text5);
			if (personalityFromNameStringKey.IsNullOrDestroyed())
			{
				DebugUtil.DevAssert(false, "<Loadings Outfit Error> Couldn't find personality \"" + text5 + "\" to apply outfit preferences", null);
			}
			else
			{
				foreach (KeyValuePair<string, string> keyValuePair3 in dictionary2)
				{
					string text6;
					keyValuePair3.Deconstruct<string, string>(out text3, out text6);
					string text7 = text3;
					string text8 = text6;
					ClothingOutfitUtility.OutfitType outfitType;
					if (Enum.TryParse<ClothingOutfitUtility.OutfitType>(text7, true, out outfitType))
					{
						personalityFromNameStringKey.Internal_SetSelectedTemplateOutfitId(outfitType, text8);
					}
				}
				if (text5 != personalityFromNameStringKey.Id)
				{
					list.Add(text5);
				}
			}
		}
		foreach (string text9 in list)
		{
			Personality personalityFromNameStringKey2 = Db.Get().Personalities.GetPersonalityFromNameStringKey(text9);
			if (!personalityFromNameStringKey2.IsNullOrDestroyed() && version.PersonalityIdToAssignedOutfits.ContainsKey(text9))
			{
				string id = personalityFromNameStringKey2.Id;
				Dictionary<string, string> dictionary3 = version.PersonalityIdToAssignedOutfits[text9];
				version.PersonalityIdToAssignedOutfits.Remove(text9);
				Dictionary<string, string> dictionary4;
				if (version.PersonalityIdToAssignedOutfits.TryGetValue(id, out dictionary4))
				{
					using (Dictionary<string, string>.Enumerator enumerator3 = dictionary3.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							KeyValuePair<string, string> keyValuePair4 = enumerator3.Current;
							string text3;
							string text6;
							keyValuePair4.Deconstruct<string, string>(out text6, out text3);
							string text10 = text6;
							string text11 = text3;
							if (!dictionary4.ContainsKey(text10))
							{
								dictionary4[text10] = text11;
							}
						}
						continue;
					}
				}
				version.PersonalityIdToAssignedOutfits.Add(id, dictionary3);
			}
		}
		CustomClothingOutfits.Instance.Internal_SetOutfitData(version);
	}

	public static readonly PermitCategory[] PERMIT_CATEGORIES_FOR_CLOTHING = new PermitCategory[]
	{
		PermitCategory.DupeTops,
		PermitCategory.DupeGloves,
		PermitCategory.DupeBottoms,
		PermitCategory.DupeShoes
	};

	public static readonly PermitCategory[] PERMIT_CATEGORIES_FOR_ATMO_SUITS = new PermitCategory[]
	{
		PermitCategory.AtmoSuitHelmet,
		PermitCategory.AtmoSuitBody,
		PermitCategory.AtmoSuitGloves,
		PermitCategory.AtmoSuitBelt,
		PermitCategory.AtmoSuitShoes
	};

	private static string OutfitFile_U44_to_U46 = "OutfitUserData.json";

	private static string OutfitFile_U47_to_Present = "OutfitUserData2.json";

	public enum OutfitType
	{
		Clothing,
		JoyResponse,
		AtmoSuit,
		LENGTH
	}
}
