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
		RetiredColonyData retiredColonyData = new RetiredColonyData(SaveGame.Instance.BaseName, GameClock.Instance.GetCycle(), global::System.DateTime.Now.ToShortDateString(), list.ToArray(), array, array2);
		string text5 = JsonConvert.SerializeObject(retiredColonyData);
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
					Encoding utf = Encoding.UTF8;
					byte[] bytes = utf.GetBytes(text5);
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

	public static RetiredColonyData[] LoadRetiredColonies()
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
		foreach (string text2 in Directory.GetDirectories(text))
		{
			foreach (string text3 in Directory.GetFiles(text2))
			{
				if (text3.EndsWith(".json"))
				{
					for (int k = 0; k < RetireColonyUtility.attempt_encodings.Length; k++)
					{
						Encoding encoding = RetireColonyUtility.attempt_encodings[k];
						try
						{
							string text4 = File.ReadAllText(text3, encoding);
							RetiredColonyData retiredColonyData = JsonConvert.DeserializeObject<RetiredColonyData>(text4);
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
								text3,
								ex.ToString()
							});
						}
					}
				}
			}
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
					Texture2D texture2D = new Texture2D(640, 360);
					texture2D.filterMode = FilterMode.Point;
					texture2D.LoadImage(File.ReadAllBytes(text3));
					list.Add(Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2(640f, 360f)), new Vector2(0.5f, 0.5f)));
				}
			}
		}
		else
		{
			global::Debug.LogWarningFormat("LoadColonySlideshow path does not exist or is not directory [{0}]", new object[] { text2 });
		}
		return list.ToArray();
	}

	public static Sprite LoadColonyPreview(string colonyName)
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
			Texture2D texture2D = new Texture2D(640, 360);
			texture2D.LoadImage(File.ReadAllBytes(list[list.Count - 1]));
			return Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2(640f, 360f)), new Vector2(0.5f, 0.5f));
		}
		return null;
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
