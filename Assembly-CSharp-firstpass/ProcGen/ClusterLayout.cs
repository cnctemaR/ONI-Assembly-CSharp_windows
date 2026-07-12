using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Klei;
using UnityEngine;

namespace ProcGen
{
	public class ClusterLayout
	{
		public List<WorldPlacement> worldPlacements { get; set; }

		public List<SpaceMapPOIPlacement> poiPlacements { get; set; }

		public string name { get; set; }

		public string description { get; set; }

		public string requiredDlcId { get; set; }

		public string forbiddenDlcId { get; set; }

		public int difficulty { get; set; }

		public ClusterLayout.Skip skip { get; private set; }

		public int clusterCategory { get; private set; }

		public int startWorldIndex { get; set; }

		public int width { get; set; }

		public int height { get; set; }

		public int numRings { get; set; }

		public int menuOrder { get; set; }

		public string coordinatePrefix { get; private set; }

		public ClusterLayout()
		{
			this.numRings = 12;
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
				string[] array = Strings.Get(this.name).String.Split(new char[] { ' ' });
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

		public const string directory = "clusters";

		public string filePath;

		public enum Skip
		{
			Never,
			Always = 99,
			EditorOnly
		}

		public enum ClusterCategory
		{
			vanilla,
			spacedOutVanillaStyle,
			spacedOutStyle
		}
	}
}
