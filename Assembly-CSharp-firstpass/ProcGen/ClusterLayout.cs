using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Klei;
using UnityEngine;

namespace ProcGen
{
	[DebuggerDisplay("{name}")]
	[Serializable]
	public class ClusterLayout
	{
		public List<WorldPlacement> worldPlacements { get; set; }

		public List<SpaceMapPOIPlacement> poiPlacements { get; set; }

		public string name { get; set; }

		public string description { get; set; }

		public string welcomeMessage { get; set; }

		public ClusterLayout.ClusterAudioSettings clusterAudio { get; set; }

		public List<LoreCollectionOverride> clusterUnlocks { get; set; }

		[Obsolete("Use requiredDlcIds")]
		public string requiredDlcId { get; set; }

		[Obsolete("Use forbiddenDlcIds")]
		public string forbiddenDlcId { get; set; }

		public string dlcIdFrom { get; set; }

		public string[] requiredDlcIds { get; set; }

		public string[] forbiddenDlcIds { get; set; }

		public int difficulty { get; set; }

		public bool disableStoryTraits { get; set; }

		public int fixedCoordinate { get; private set; }

		public ClusterLayout.Skip skip { get; private set; }

		public ClusterLayout.ClusterCategory clusterCategory { get; set; }

		public int startWorldIndex { get; set; }

		public int width { get; set; }

		public int height { get; set; }

		public int numRings { get; set; }

		public int menuOrder { get; set; }

		public string coordinatePrefix { get; private set; }

		public List<string> clusterTags { get; private set; }

		public ClusterLayout()
		{
			this.numRings = 12;
			this.fixedCoordinate = -1;
			this.welcomeMessage = null;
			this.clusterAudio = new ClusterLayout.ClusterAudioSettings();
			this.clusterTags = new List<string>();
			this.clusterUnlocks = new List<LoreCollectionOverride>();
		}

		public static string GetName(string path, string addPrefix)
		{
			return FileSystem.Normalize(Path.Combine(addPrefix + "clusters", Path.GetFileNameWithoutExtension(path)));
		}

		public string GetStartWorld()
		{
			return this.worldPlacements[this.startWorldIndex].world;
		}

		public string GetCoordinatePrefix()
		{
			if (string.IsNullOrEmpty(this.coordinatePrefix))
			{
				string text = "";
				string[] array = Strings.Get(this.name).String.Split(' ', StringSplitOptions.None);
				int num = 5 - array.Length;
				bool flag = true;
				foreach (string text2 in array)
				{
					if (!flag)
					{
						text += "-";
					}
					string text3 = Regex.Replace(text2, "(a|e|i|o|u)", "");
					text += text3.Substring(0, Mathf.Min(num, text3.Length)).ToUpper();
					flag = false;
				}
				this.coordinatePrefix = text;
			}
			return this.coordinatePrefix;
		}

		public bool HasAnyTags(List<string> tags)
		{
			foreach (string text in tags)
			{
				if (this.clusterTags.Contains(text))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasAllTags(List<string> tags)
		{
			foreach (string text in tags)
			{
				if (!this.clusterTags.Contains(text))
				{
					return false;
				}
			}
			return true;
		}

		public const string directory = "clusters";

		public string filePath;

		[Serializable]
		public class ClusterAudioSettings
		{
			public string musicWelcome { get; set; }

			public string musicFirst { get; set; }

			public string stingerDay { get; set; }

			public string stingerNight { get; set; }

			public ClusterAudioSettings()
			{
				this.musicWelcome = "Music_WattsonMessage";
				this.musicFirst = null;
				this.stingerDay = "Stinger_Day";
				this.stingerNight = "Stinger_Loop_Night";
			}
		}

		public enum Skip
		{
			Never,
			Always = 99,
			EditorOnly
		}

		public enum ClusterCategory
		{
			Vanilla,
			SpacedOutVanillaStyle,
			SpacedOutStyle,
			Special
		}
	}
}
