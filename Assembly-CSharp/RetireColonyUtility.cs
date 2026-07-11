using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;

public static class RetireColonyUtility
{
	public static bool SaveColonySummaryData()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = RetireColonyUtility.StripInvalidCharacters(SaveGame.Instance.BaseName);
		string text3 = Path.Combine(text, text2);
		if (!Directory.Exists(text3))
		{
			Directory.CreateDirectory(text3);
		}
		string text4 = Path.Combine(text3, text2 + ".json");
		string text5 = JsonConvert.SerializeObject(RetireColonyUtility.GetCurrentColonyRetiredColonyData());
		bool flag = false;
		int num = 0;
		while (!flag && num < 5)
		{
			try
			{
				Thread.Sleep(num * 100);
				using (FileStream fileStream = File.Open(text4, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					flag = true;
					byte[] bytes = Encoding.UTF8.GetBytes(text5);
					fileStream.Write(bytes, 0, bytes.Length);
				}
			}
			catch (Exception ex)
			{
				global::Debug.LogWarningFormat("SaveColonySummaryData failed attempt {0}: {1}", new object[]
				{
					num + 1,
					ex.ToString()
				});
			}
			num++;
		}
		return flag;
	}

	public static RetiredColonyData GetCurrentColonyRetiredColonyData()
	{
		MinionAssignablesProxy[] array = new MinionAssignablesProxy[Components.MinionAssignablesProxy.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Components.MinionAssignablesProxy[i];
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, ColonyAchievementStatus> keyValuePair in SaveGame.Instance.GetComponent<ColonyAchievementTracker>().achievements)
		{
			if (keyValuePair.Value.success)
			{
				list.Add(keyValuePair.Key);
			}
		}
		BuildingComplete[] array2 = new BuildingComplete[Components.BuildingCompletes.Count];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = Components.BuildingCompletes[j];
		}
		return new RetiredColonyData(SaveGame.Instance.BaseName, GameClock.Instance.GetCycle(), global::System.DateTime.Now.ToShortDateString(), list.ToArray(), array, array2);
	}

	private static RetiredColonyData LoadRetiredColony(string file, bool skipStats, Encoding enc)
	{
		RetiredColonyData retiredColonyData = new RetiredColonyData();
		using (FileStream fileStream = File.Open(file, FileMode.Open))
		{
			using (StreamReader streamReader = new StreamReader(fileStream, enc))
			{
				using (JsonReader jsonReader = new JsonTextReader(streamReader))
				{
					string text = string.Empty;
					List<string> list = new List<string>();
					List<global::Tuple<string, int>> list2 = new List<global::Tuple<string, int>>();
					List<RetiredColonyData.RetiredDuplicantData> list3 = new List<RetiredColonyData.RetiredDuplicantData>();
					List<RetiredColonyData.RetiredColonyStatistic> list4 = new List<RetiredColonyData.RetiredColonyStatistic>();
					while (jsonReader.Read())
					{
						JsonToken jsonToken = jsonReader.TokenType;
						if (jsonToken == JsonToken.PropertyName)
						{
							text = jsonReader.Value.ToString();
						}
						if (jsonToken == JsonToken.String && text == "colonyName")
						{
							retiredColonyData.colonyName = jsonReader.Value.ToString();
						}
						if (jsonToken == JsonToken.String && text == "date")
						{
							retiredColonyData.date = jsonReader.Value.ToString();
						}
						if (jsonToken == JsonToken.Integer && text == "cycleCount")
						{
							retiredColonyData.cycleCount = int.Parse(jsonReader.Value.ToString());
						}
						if (jsonToken == JsonToken.String && text == "achievements")
						{
							list.Add(jsonReader.Value.ToString());
						}
						if (jsonToken == JsonToken.StartObject && text == "Duplicants")
						{
							string text2 = null;
							RetiredColonyData.RetiredDuplicantData retiredDuplicantData = new RetiredColonyData.RetiredDuplicantData();
							retiredDuplicantData.accessories = new Dictionary<string, string>();
							while (jsonReader.Read())
							{
								jsonToken = jsonReader.TokenType;
								if (jsonToken == JsonToken.EndObject)
								{
									break;
								}
								if (jsonToken == JsonToken.PropertyName)
								{
									text2 = jsonReader.Value.ToString();
								}
								if (text2 == "name" && jsonToken == JsonToken.String)
								{
									retiredDuplicantData.name = jsonReader.Value.ToString();
								}
								if (text2 == "age" && jsonToken == JsonToken.Integer)
								{
									retiredDuplicantData.age = int.Parse(jsonReader.Value.ToString());
								}
								if (text2 == "skillPointsGained" && jsonToken == JsonToken.Integer)
								{
									retiredDuplicantData.skillPointsGained = int.Parse(jsonReader.Value.ToString());
								}
								if (text2 == "accessories")
								{
									string text3 = null;
									while (jsonReader.Read())
									{
										jsonToken = jsonReader.TokenType;
										if (jsonToken == JsonToken.EndObject)
										{
											break;
										}
										if (jsonToken == JsonToken.PropertyName)
										{
											text3 = jsonReader.Value.ToString();
										}
										if (text3 != null && jsonReader.Value != null && jsonToken == JsonToken.String)
										{
											string text4 = jsonReader.Value.ToString();
											retiredDuplicantData.accessories.Add(text3, text4);
										}
									}
								}
							}
							list3.Add(retiredDuplicantData);
						}
						if (jsonToken == JsonToken.StartObject && text == "buildings")
						{
							string text5 = null;
							string text6 = null;
							int num = 0;
							while (jsonReader.Read())
							{
								jsonToken = jsonReader.TokenType;
								if (jsonToken == JsonToken.EndObject)
								{
									break;
								}
								if (jsonToken == JsonToken.PropertyName)
								{
									text5 = jsonReader.Value.ToString();
								}
								if (text5 == "first" && jsonToken == JsonToken.String)
								{
									text6 = jsonReader.Value.ToString();
								}
								if (text5 == "second" && jsonToken == JsonToken.Integer)
								{
									num = int.Parse(jsonReader.Value.ToString());
								}
							}
							global::Tuple<string, int> tuple = new global::Tuple<string, int>(text6, num);
							list2.Add(tuple);
						}
						if (jsonToken == JsonToken.StartObject && text == "Stats")
						{
							if (skipStats)
							{
								break;
							}
							string text7 = null;
							RetiredColonyData.RetiredColonyStatistic retiredColonyStatistic = new RetiredColonyData.RetiredColonyStatistic();
							List<global::Tuple<float, float>> list5 = new List<global::Tuple<float, float>>();
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
								if (text7 == "id" && jsonToken == JsonToken.String)
								{
									retiredColonyStatistic.id = jsonReader.Value.ToString();
								}
								if (text7 == "name" && jsonToken == JsonToken.String)
								{
									retiredColonyStatistic.name = jsonReader.Value.ToString();
								}
								if (text7 == "nameX" && jsonToken == JsonToken.String)
								{
									retiredColonyStatistic.nameX = jsonReader.Value.ToString();
								}
								if (text7 == "nameY" && jsonToken == JsonToken.String)
								{
									retiredColonyStatistic.nameY = jsonReader.Value.ToString();
								}
								if (text7 == "value" && jsonToken == JsonToken.StartObject)
								{
									string text8 = null;
									float num2 = 0f;
									float num3 = 0f;
									while (jsonReader.Read())
									{
										jsonToken = jsonReader.TokenType;
										if (jsonToken == JsonToken.EndObject)
										{
											break;
										}
										if (jsonToken == JsonToken.PropertyName)
										{
											text8 = jsonReader.Value.ToString();
										}
										if (text8 == "first" && (jsonToken == JsonToken.Float || jsonToken == JsonToken.Integer))
										{
											num2 = float.Parse(jsonReader.Value.ToString());
										}
										if (text8 == "second" && (jsonToken == JsonToken.Float || jsonToken == JsonToken.Integer))
										{
											num3 = float.Parse(jsonReader.Value.ToString());
										}
									}
									global::Tuple<float, float> tuple2 = new global::Tuple<float, float>(num2, num3);
									list5.Add(tuple2);
								}
							}
							retiredColonyStatistic.value = list5.ToArray();
							list4.Add(retiredColonyStatistic);
						}
					}
					retiredColonyData.Duplicants = list3.ToArray();
					retiredColonyData.Stats = list4.ToArray();
					retiredColonyData.achievements = list.ToArray();
					retiredColonyData.buildings = list2;
				}
			}
		}
		return retiredColonyData;
	}

	public static RetiredColonyData[] LoadRetiredColonies(bool skipStats = false)
	{
		List<RetiredColonyData> list = new List<RetiredColonyData>();
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		text = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		string[] directories = Directory.GetDirectories(text);
		for (int i = 0; i < directories.Length; i++)
		{
			foreach (string text2 in Directory.GetFiles(directories[i]))
			{
				if (text2.EndsWith(".json"))
				{
					for (int k = 0; k < RetireColonyUtility.attempt_encodings.Length; k++)
					{
						Encoding encoding = RetireColonyUtility.attempt_encodings[k];
						try
						{
							RetiredColonyData retiredColonyData = RetireColonyUtility.LoadRetiredColony(text2, skipStats, encoding);
							if (retiredColonyData != null)
							{
								if (retiredColonyData.colonyName == null)
								{
									throw new Exception("data.colonyName was null");
								}
								list.Add(retiredColonyData);
							}
							break;
						}
						catch (Exception ex)
						{
							global::Debug.LogWarningFormat("LoadRetiredColonies failed load {0} [{1}]: {2}", new object[]
							{
								encoding,
								text2,
								ex.ToString()
							});
						}
					}
				}
			}
		}
		return list.ToArray();
	}

	public static string[] LoadColonySlideshowFiles(string colonyName)
	{
		string text = RetireColonyUtility.StripInvalidCharacters(colonyName);
		string text2 = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), text);
		List<string> list = new List<string>();
		if (Directory.Exists(text2))
		{
			foreach (string text3 in Directory.GetFiles(text2))
			{
				if (text3.EndsWith(".png"))
				{
					list.Add(text3);
				}
			}
		}
		else
		{
			global::Debug.LogWarningFormat("LoadColonySlideshow path does not exist or is not directory [{0}]", new object[] { text2 });
		}
		return list.ToArray();
	}

	public static Sprite[] LoadColonySlideshow(string colonyName)
	{
		string text = RetireColonyUtility.StripInvalidCharacters(colonyName);
		string text2 = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), text);
		List<Sprite> list = new List<Sprite>();
		if (Directory.Exists(text2))
		{
			foreach (string text3 in Directory.GetFiles(text2))
			{
				if (text3.EndsWith(".png"))
				{
					Texture2D texture2D = new Texture2D(512, 768);
					texture2D.filterMode = FilterMode.Point;
					texture2D.LoadImage(File.ReadAllBytes(text3));
					list.Add(Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2((float)texture2D.width, (float)texture2D.height)), new Vector2(0.5f, 0.5f), 100f, 0U, SpriteMeshType.FullRect));
				}
			}
		}
		else
		{
			global::Debug.LogWarningFormat("LoadColonySlideshow path does not exist or is not directory [{0}]", new object[] { text2 });
		}
		return list.ToArray();
	}

	public static Sprite LoadRetiredColonyPreview(string colonyName)
	{
		string text = RetireColonyUtility.StripInvalidCharacters(colonyName);
		string text2 = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), text);
		List<string> list = new List<string>();
		if (Directory.Exists(text2))
		{
			foreach (string text3 in Directory.GetFiles(text2))
			{
				if (text3.EndsWith(".png"))
				{
					list.Add(text3);
				}
			}
		}
		else
		{
			global::Debug.LogWarningFormat("LoadColonyPreview path does not exist or is not directory [{0}]", new object[] { text2 });
		}
		if (list.Count > 0)
		{
			Texture2D texture2D = new Texture2D(512, 768);
			texture2D.LoadImage(File.ReadAllBytes(list[list.Count - 1]));
			return Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2((float)texture2D.width, (float)texture2D.height)), new Vector2(0.5f, 0.5f), 100f, 0U, SpriteMeshType.FullRect);
		}
		return null;
	}

	public static Sprite LoadColonyPreview(string savePath, string colonyName, bool fallbackToTimelapse = false)
	{
		string text = Path.ChangeExtension(savePath, ".png");
		if (File.Exists(text))
		{
			try
			{
				Texture2D texture2D = new Texture2D(512, 768);
				texture2D.LoadImage(File.ReadAllBytes(text));
				return Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2((float)texture2D.width, (float)texture2D.height)), new Vector2(0.5f, 0.5f), 100f, 0U, SpriteMeshType.FullRect);
			}
			catch (Exception ex)
			{
				global::Debug.Log("failed to load preview image!? " + ex);
			}
		}
		if (!fallbackToTimelapse)
		{
			return null;
		}
		return RetireColonyUtility.LoadRetiredColonyPreview(colonyName);
	}

	public static string StripInvalidCharacters(string source)
	{
		foreach (char c in RetireColonyUtility.invalidCharacters)
		{
			source = source.Replace(c, '_');
		}
		source = source.Trim();
		return source;
	}

	private const int FILE_IO_RETRY_ATTEMPTS = 5;

	private static char[] invalidCharacters = "<>:\"\\/|?*.".ToCharArray();

	private static Encoding[] attempt_encodings = new Encoding[]
	{
		new UTF8Encoding(false, true),
		new UnicodeEncoding(false, true, true),
		Encoding.ASCII
	};
}
