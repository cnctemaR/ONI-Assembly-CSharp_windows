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
		if (self == ClothingOutfitUtility.OutfitType.Clothing)
		{
			return UI.MINION_BROWSER_SCREEN.OUTFIT_TYPE_CLOTHING;
		}
		if (self != ClothingOutfitUtility.OutfitType.JoyResponse)
		{
			DebugUtil.DevAssert(false, string.Format("Couldn't find name for outfit type: {0}", self), null);
			return self.ToString();
		}
		return UI.MINION_BROWSER_SCREEN.OUTFIT_TYPE_JOY_RESPONSE;
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
		string text2 = Path.Combine(text, ClothingOutfitUtility.outfitfile);
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
			Debug.LogWarningFormat("SaveClothingOutfitData failed", Array.Empty<object>());
		}
		return flag;
	}

	public static void LoadClothingOutfitData(ClothingOutfits dbClothingOutfits)
	{
		string text = Path.Combine(Util.RootFolder(), Util.GetKleiItemUserDataFolderName(), ClothingOutfitUtility.outfitfile);
		if (!File.Exists(text))
		{
			return;
		}
		string text2;
		using (FileStream fileStream = File.Open(text, FileMode.Open))
		{
			using (StreamReader streamReader = new StreamReader(fileStream, new UTF8Encoding(false, true)))
			{
				text2 = streamReader.ReadToEnd();
			}
		}
		SerializableOutfitData.Version2 version = SerializableOutfitData.FromJson(JObject.Parse(text2));
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
		foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> keyValuePair2 in version.PersonalityIdToAssignedOutfits)
		{
			string text3;
			Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary;
			keyValuePair2.Deconstruct<string, Dictionary<ClothingOutfitUtility.OutfitType, string>>(out text3, out dictionary);
			string text5 = text3;
			Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary2 = dictionary;
			Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(text5);
			if (personalityFromNameStringKey.IsNullOrDestroyed())
			{
				DebugUtil.DevAssert(false, "<Loadings Outfit Error> Couldn't find personality \"" + text5 + "\" to apply outfit preferences", null);
			}
			else
			{
				foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, string> keyValuePair3 in dictionary2)
				{
					ClothingOutfitUtility.OutfitType outfitType;
					keyValuePair3.Deconstruct<ClothingOutfitUtility.OutfitType, string>(out outfitType, out text3);
					ClothingOutfitUtility.OutfitType outfitType2 = outfitType;
					string text6 = text3;
					personalityFromNameStringKey.Internal_SetSelectedTemplateOutfitId(outfitType2, text6);
				}
				if (text5 != personalityFromNameStringKey.Id)
				{
					list.Add(text5);
				}
			}
		}
		foreach (string text7 in list)
		{
			Personality personalityFromNameStringKey2 = Db.Get().Personalities.GetPersonalityFromNameStringKey(text7);
			if (!personalityFromNameStringKey2.IsNullOrDestroyed() && version.PersonalityIdToAssignedOutfits.ContainsKey(text7))
			{
				string id = personalityFromNameStringKey2.Id;
				Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary3 = version.PersonalityIdToAssignedOutfits[text7];
				version.PersonalityIdToAssignedOutfits.Remove(text7);
				Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary4;
				if (version.PersonalityIdToAssignedOutfits.TryGetValue(id, out dictionary4))
				{
					using (Dictionary<ClothingOutfitUtility.OutfitType, string>.Enumerator enumerator3 = dictionary3.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							KeyValuePair<ClothingOutfitUtility.OutfitType, string> keyValuePair4 = enumerator3.Current;
							string text3;
							ClothingOutfitUtility.OutfitType outfitType;
							keyValuePair4.Deconstruct<ClothingOutfitUtility.OutfitType, string>(out outfitType, out text3);
							ClothingOutfitUtility.OutfitType outfitType3 = outfitType;
							string text8 = text3;
							if (!dictionary4.ContainsKey(outfitType3))
							{
								dictionary4[outfitType3] = text8;
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

	private static string outfitfile = "OutfitUserData.json";

	public enum OutfitType
	{
		Clothing,
		JoyResponse,
		LENGTH
	}
}
