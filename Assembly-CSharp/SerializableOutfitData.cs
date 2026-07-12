using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

public static class SerializableOutfitData
{
	public static int GetVersionFrom(JObject jsonData)
	{
		int num;
		if (jsonData["Version"] == null)
		{
			num = 1;
		}
		else
		{
			num = jsonData.Value<int>("Version");
			jsonData.Remove("Version");
		}
		return num;
	}

	public static SerializableOutfitData.Version2 FromJson(JObject jsonData)
	{
		int versionFrom = SerializableOutfitData.GetVersionFrom(jsonData);
		if (versionFrom == 1)
		{
			return SerializableOutfitData.Version2.FromVersion1(SerializableOutfitData.Version1.FromJson(jsonData));
		}
		if (versionFrom != 2)
		{
			DebugUtil.DevAssert(false, string.Format("Version {0} of OutfitData is not supported", versionFrom), null);
			return new SerializableOutfitData.Version2();
		}
		return SerializableOutfitData.Version2.FromJson(jsonData);
	}

	public static JObject ToJson(SerializableOutfitData.Version2 data)
	{
		return SerializableOutfitData.Version2.ToJson(data);
	}

	public static string ToJsonString(JObject data)
	{
		string text;
		using (StringWriter stringWriter = new StringWriter())
		{
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				data.WriteTo(jsonTextWriter, Array.Empty<JsonConverter>());
				text = stringWriter.ToString();
			}
		}
		return text;
	}

	public static void ToJsonString(JObject data, TextWriter textWriter)
	{
		using (JsonTextWriter jsonTextWriter = new JsonTextWriter(textWriter))
		{
			data.WriteTo(jsonTextWriter, Array.Empty<JsonConverter>());
		}
	}

	public const string VERSION_KEY = "Version";

	public class Version2
	{
		public static SerializableOutfitData.Version2 FromVersion1(SerializableOutfitData.Version1 data)
		{
			Dictionary<string, SerializableOutfitData.Version2.CustomTemplateOutfitEntry> dictionary = new Dictionary<string, SerializableOutfitData.Version2.CustomTemplateOutfitEntry>();
			foreach (KeyValuePair<string, string[]> keyValuePair in data.CustomOutfits)
			{
				string text;
				string[] array;
				keyValuePair.Deconstruct<string, string[]>(out text, out array);
				string text2 = text;
				string[] array2 = array;
				dictionary.Add(text2, new SerializableOutfitData.Version2.CustomTemplateOutfitEntry
				{
					outfitType = "Clothing",
					itemIds = array2
				});
			}
			Dictionary<string, Dictionary<string, string>> dictionary2 = new Dictionary<string, Dictionary<string, string>>();
			foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> keyValuePair2 in data.DuplicantOutfits)
			{
				string text;
				Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary3;
				keyValuePair2.Deconstruct<string, Dictionary<ClothingOutfitUtility.OutfitType, string>>(out text, out dictionary3);
				string text3 = text;
				Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary4 = dictionary3;
				Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
				dictionary2[text3] = dictionary5;
				foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, string> keyValuePair3 in dictionary4)
				{
					ClothingOutfitUtility.OutfitType outfitType;
					keyValuePair3.Deconstruct<ClothingOutfitUtility.OutfitType, string>(out outfitType, out text);
					ClothingOutfitUtility.OutfitType outfitType2 = outfitType;
					string text4 = text;
					dictionary5.Add(Enum.GetName(typeof(ClothingOutfitUtility.OutfitType), outfitType2), text4);
				}
			}
			return new SerializableOutfitData.Version2
			{
				PersonalityIdToAssignedOutfits = dictionary2,
				OutfitIdToUserAuthoredTemplateOutfit = dictionary
			};
		}

		public static SerializableOutfitData.Version2 FromJson(JObject jsonData)
		{
			return jsonData.ToObject<SerializableOutfitData.Version2>(SerializableOutfitData.Version2.GetSerializer());
		}

		public static JObject ToJson(SerializableOutfitData.Version2 data)
		{
			JObject jobject = JObject.FromObject(data, SerializableOutfitData.Version2.GetSerializer());
			jobject.AddFirst(new JProperty("Version", 2));
			return jobject;
		}

		public static JsonSerializer GetSerializer()
		{
			if (SerializableOutfitData.Version2.s_serializer != null)
			{
				return SerializableOutfitData.Version2.s_serializer;
			}
			SerializableOutfitData.Version2.s_serializer = JsonSerializer.CreateDefault();
			SerializableOutfitData.Version2.s_serializer.Converters.Add(new StringEnumConverter());
			return SerializableOutfitData.Version2.s_serializer;
		}

		public Dictionary<string, Dictionary<string, string>> PersonalityIdToAssignedOutfits = new Dictionary<string, Dictionary<string, string>>();

		public Dictionary<string, SerializableOutfitData.Version2.CustomTemplateOutfitEntry> OutfitIdToUserAuthoredTemplateOutfit = new Dictionary<string, SerializableOutfitData.Version2.CustomTemplateOutfitEntry>();

		private static JsonSerializer s_serializer;

		public class CustomTemplateOutfitEntry
		{
			public string outfitType;

			public string[] itemIds;
		}
	}

	public class Version1
	{
		public static JObject ToJson(SerializableOutfitData.Version1 data)
		{
			return JObject.FromObject(data);
		}

		public static SerializableOutfitData.Version1 FromJson(JObject jsonData)
		{
			SerializableOutfitData.Version1 version = new SerializableOutfitData.Version1();
			SerializableOutfitData.Version1 version2;
			using (JsonReader jsonReader = jsonData.CreateReader())
			{
				string text = null;
				string text2 = "DuplicantOutfits";
				string text3 = "CustomOutfits";
				while (jsonReader.Read())
				{
					JsonToken jsonToken = jsonReader.TokenType;
					if (jsonToken == JsonToken.PropertyName)
					{
						text = jsonReader.Value.ToString();
					}
					if (jsonToken == JsonToken.StartObject && text == text2)
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
								string text4 = jsonReader.Value.ToString();
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
												string text5 = jsonReader.Value.ToString();
												if (outfitType != ClothingOutfitUtility.OutfitType.LENGTH)
												{
													if (!version.DuplicantOutfits.ContainsKey(text4))
													{
														version.DuplicantOutfits.Add(text4, new Dictionary<ClothingOutfitUtility.OutfitType, string>());
													}
													version.DuplicantOutfits[text4][outfitType] = text5;
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
					else if (text == text3)
					{
						string text6 = null;
						while (jsonReader.Read())
						{
							jsonToken = jsonReader.TokenType;
							if (jsonToken == JsonToken.EndObject)
							{
								break;
							}
							if (jsonToken == JsonToken.PropertyName)
							{
								text6 = jsonReader.Value.ToString();
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
									if (text6 != null)
									{
										version.CustomOutfits[text6] = array;
									}
								}
							}
						}
					}
				}
				version2 = version;
			}
			return version2;
		}

		public Dictionary<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> DuplicantOutfits = new Dictionary<string, Dictionary<ClothingOutfitUtility.OutfitType, string>>();

		public Dictionary<string, string[]> CustomOutfits = new Dictionary<string, string[]>();
	}
}
