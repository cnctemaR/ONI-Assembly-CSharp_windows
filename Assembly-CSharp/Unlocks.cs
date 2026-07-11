using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

public class Unlocks : KMonoBehaviour
{
	private static string UnlocksFilename
	{
		get
		{
			return Path.Combine(Util.RootFolder(), "unlocks.json");
		}
	}

	protected override void OnPrefabInit()
	{
		this.LoadUnlocks();
	}

	public bool IsUnlocked(string unlockID)
	{
		return !string.IsNullOrEmpty(unlockID) && (DebugHandler.InstantBuildMode || this.unlocked.Contains(unlockID));
	}

	public void Unlock(string unlockID)
	{
		if (string.IsNullOrEmpty(unlockID))
		{
			DebugUtil.DevAssert(false, "Unlock called with null or empty string");
			return;
		}
		if (!this.unlocked.Contains(unlockID))
		{
			this.unlocked.Add(unlockID);
			this.SaveUnlocks();
			Game.Instance.Trigger(1594320620, unlockID);
		}
	}

	private void SaveUnlocks()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = JsonConvert.SerializeObject(this.unlocked);
		bool flag = false;
		int num = 0;
		while (!flag && num < 5)
		{
			try
			{
				Thread.Sleep(num * 100);
				using (FileStream fileStream = File.Open(Unlocks.UnlocksFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					flag = true;
					ASCIIEncoding asciiencoding = new ASCIIEncoding();
					byte[] bytes = asciiencoding.GetBytes(text);
					fileStream.Write(bytes, 0, bytes.Length);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("Failed to save Unlocks attempt {0}: {1}", new object[]
				{
					num + 1,
					ex.ToString()
				});
			}
			num++;
		}
	}

	public void LoadUnlocks()
	{
		this.unlocked.Clear();
		if (!File.Exists(Unlocks.UnlocksFilename))
		{
			return;
		}
		string text = string.Empty;
		bool flag = false;
		int num = 0;
		while (!flag && num < 5)
		{
			try
			{
				Thread.Sleep(num * 100);
				using (FileStream fileStream = File.Open(Unlocks.UnlocksFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					flag = true;
					ASCIIEncoding asciiencoding = new ASCIIEncoding();
					byte[] array = new byte[fileStream.Length];
					if ((long)fileStream.Read(array, 0, array.Length) == fileStream.Length)
					{
						text += asciiencoding.GetString(array);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("Failed to load Unlocks attempt {0}: {1}", new object[]
				{
					num + 1,
					ex.ToString()
				});
			}
			num++;
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		try
		{
			string[] array2 = JsonConvert.DeserializeObject<string[]>(text);
			foreach (string text2 in array2)
			{
				if (!string.IsNullOrEmpty(text2))
				{
					if (!this.unlocked.Contains(text2))
					{
						this.unlocked.Add(text2);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Debug.LogErrorFormat("Error parsing unlocks file [{0}]: {1}", new object[]
			{
				Unlocks.UnlocksFilename,
				ex2.ToString()
			});
		}
	}

	public string UnlockNext(string collectionID)
	{
		foreach (string text in this.lockCollections[collectionID])
		{
			if (string.IsNullOrEmpty(text))
			{
				DebugUtil.DevAssertArgs(false, new object[] { "Found null/empty string in Unlocks collection: ", collectionID });
			}
			else if (!this.IsUnlocked(text))
			{
				this.Unlock(text);
				return text;
			}
		}
		return null;
	}

	private const int FILE_IO_RETRY_ATTEMPTS = 5;

	private List<string> unlocked = new List<string>();

	public Dictionary<string, string[]> lockCollections = new Dictionary<string, string[]>
	{
		{
			"emails",
			new string[]
			{
				"email_preliminarycalculations", "email_researchgiant", "email_thermodynamiclaws", "email_frankiesblog", "email_atomiconrecruitment", "email_thejanitor", "email_security2", "email_newemployee", "email_security3", "email_hollandsdog",
				"email_temporalbowupdate", "email_retemporalbowupdate", "email_pens", "email_pens2", "email_memorychip", "email_arthistoryrequest", "email_AIcontrol", "email_AIcontrol2", "email_friendlyemail", "email_AIcontrol3",
				"email_AIcontrol4"
			}
		},
		{
			"journals",
			new string[]
			{
				"journal_sunflowerseeds", "journal_debrief", "journal_employeeprocessing", "journal_B835_1", "journal_B835_2", "journal_B835_3", "journal_B835_4", "journal_B835_5", "journal_B835_6", "journal_cleanup",
				"journal_pipedream", "journal_A046_1", "journal_A046_2", "journal_A046_3", "journal_A046_4", "journal_spittingimage", "journal_elliesbirthday1", "journal_movedrats", "journal_B327_1", "journal_B327_2",
				"journal_B327_3", "journal_B327_4", "journal_elliesbirthday2", "journal_revisitednumbers", "journal_ants", "journal_B556_1", "journal_B556_2", "journal_B556_3", "journal_B556_4", "journal_timemusings",
				"journal_timesarrowthoughts", "journal_magazine", "journal_planetaryechoes1", "journal_planetaryechoes2", "journal_planetaryechoes3", "journal_planetaryechoes4", "journal_planetaryechoes5", "journal_notetojodi"
			}
		},
		{
			"researchnotes",
			new string[]
			{
				"notes_clonedrats", "notes_agriculture1", "notes_husbandry1", "notes_hibiscus3", "notes_husbandry2", "notes_agriculture2", "notes_geneticooze", "notes_agriculture3", "notes_husbandry3", "notes_memoryimplantation",
				"notes_husbandry4", "notes_agriculture4", "notes_neutronium", "notes_firstsuccess", "notes_neutroniumapplications"
			}
		},
		{
			"misc",
			new string[] { "misc_mailroometiquette", "misc_unattendedcultures", "misc_newsecurity", "misc_politerequest", "misc_casualfriday", "misc_bringyourkidtowork", "misc_dishbot" }
		}
	};
}
