using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Database;
using Newtonsoft.Json;
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
		string text3 = JsonConvert.SerializeObject(CustomClothingOutfits.Instance.OutfitData);
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
		if (File.Exists(text))
		{
			using (FileStream fileStream = File.Open(text, FileMode.Open))
			{
				using (StreamReader streamReader = new StreamReader(fileStream, new UTF8Encoding(false, true)))
				{
					using (JsonReader jsonReader = new JsonTextReader(streamReader))
					{
						string text2 = null;
						string text3 = "DuplicantOutfits";
						string text4 = "CustomOutfits";
						while (jsonReader.Read())
						{
							JsonToken jsonToken = jsonReader.TokenType;
							if (jsonToken == JsonToken.PropertyName)
							{
								text2 = jsonReader.Value.ToString();
							}
							if (jsonToken == JsonToken.StartObject && text2 == text3)
							{
								ClothingOutfitUtility.OutfitType outfitType = ClothingOutfitUtility.OutfitType.LENGTH;
								while (jsonReader.Read())
								{
									jsonToken = jsonReader.TokenType;
									if (jsonToken == JsonToken.EndObject)
									{
										break;
									}
									if (jsonToken == JsonToken.PropertyName)
									{
										string text5 = jsonReader.Value.ToString();
										while (jsonReader.Read())
										{
											jsonToken = jsonReader.TokenType;
											if (jsonToken == JsonToken.EndObject)
											{
												break;
											}
											if (jsonToken == JsonToken.PropertyName)
											{
												Enum.TryParse<ClothingOutfitUtility.OutfitType>(jsonReader.Value.ToString(), out outfitType);
												while (jsonReader.Read())
												{
													jsonToken = jsonReader.TokenType;
													if (jsonToken == JsonToken.String)
													{
														string text6 = jsonReader.Value.ToString();
														if (outfitType != ClothingOutfitUtility.OutfitType.LENGTH)
														{
															if (!CustomClothingOutfits.Instance.OutfitData.DuplicantOutfits.ContainsKey(text5))
															{
																CustomClothingOutfits.Instance.OutfitData.DuplicantOutfits.Add(text5, new Dictionary<ClothingOutfitUtility.OutfitType, string>());
															}
															CustomClothingOutfits.Instance.OutfitData.DuplicantOutfits[text5][outfitType] = text6;
															break;
														}
														break;
													}
												}
											}
										}
									}
								}
							}
							else if (text2 == text4)
							{
								string text7 = null;
								while (jsonReader.Read())
								{
									jsonToken = jsonReader.TokenType;
									if (jsonToken == JsonToken.EndObject)
									{
										break;
									}
									if (jsonToken == JsonToken.PropertyName)
									{
										text7 = jsonReader.Value.ToString();
									}
									if (jsonToken == JsonToken.StartArray)
									{
										JArray jarray = JArray.Load(jsonReader);
										if (jarray != null)
										{
											string[] array = new string[jarray.Count];
											for (int i = 0; i < jarray.Count; i++)
											{
												array[i] = jarray[i].ToString();
											}
											if (text7 != null)
											{
												CustomClothingOutfits.Instance.OutfitData.CustomOutfits[text7] = array;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			foreach (KeyValuePair<string, string[]> keyValuePair in CustomClothingOutfits.Instance.OutfitData.CustomOutfits)
			{
				if (dbClothingOutfits.TryGet(keyValuePair.Key) != null)
				{
					Debug.LogError("User outfit data is trying to overwrite default " + keyValuePair.Key);
				}
			}
			foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> keyValuePair2 in CustomClothingOutfits.Instance.OutfitData.DuplicantOutfits)
			{
				Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(keyValuePair2.Key);
				if (personalityFromNameStringKey.IsNullOrDestroyed())
				{
					DebugUtil.DevAssert(false, "<Loadings Outfit Error> Couldn't find personality \"" + keyValuePair2.Key + "\" to apply outfit preferences", null);
				}
				else
				{
					foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, string> keyValuePair3 in keyValuePair2.Value)
					{
						personalityFromNameStringKey.Internal_SetOutfit(keyValuePair3.Key, keyValuePair3.Value);
					}
				}
			}
		}
	}

	private static string outfitfile = "OutfitUserData.json";

	public enum OutfitType
	{
		Clothing,
		JoyResponse,
		LENGTH
	}
}
