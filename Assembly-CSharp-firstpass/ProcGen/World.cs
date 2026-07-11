using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ProcGen
{
	[Serializable]
	public class World
	{
		public string name { get; private set; }

		public string description { get; private set; }

		public string coordinatePrefix { get; private set; }

		public string spriteName { get; private set; }

		public int difficulty { get; private set; }

		public int tier { get; private set; }

		public bool disableWorldTraits { get; private set; }

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

		public World.Skip skip { get; private set; }

		public bool noStart { get; private set; }

		public Vector2I worldsize { get; private set; }

		public DefaultSettings defaultsOverrides { get; private set; }

		public World.LayoutMethod layoutMethod { get; private set; }

		public List<WeightedName> subworldFiles { get; private set; }

		public List<World.AllowedCellsFilter> unknownCellsAllowedSubworlds { get; private set; }

		public string startSubworldName { get; private set; }

		public string startingBaseTemplate { get; set; }

		public MinMax startingBasePositionHorizontal { get; private set; }

		public MinMax startingBasePositionVertical { get; private set; }

		public Dictionary<string, int> globalFeatureTemplates { get; private set; }

		public Dictionary<string, int> globalFeatures { get; private set; }

		public World()
		{
			this.subworldFiles = new List<WeightedName>();
			this.unknownCellsAllowedSubworlds = new List<World.AllowedCellsFilter>();
			this.startingBasePositionHorizontal = new MinMax(0.5f, 0.5f);
			this.startingBasePositionVertical = new MinMax(0.5f, 0.5f);
			this.globalFeatureTemplates = new Dictionary<string, int>();
			this.globalFeatures = new Dictionary<string, int>();
		}

		public void ModStartLocation(MinMax hMod, MinMax vMod)
		{
			MinMax startingBasePositionHorizontal = this.startingBasePositionHorizontal;
			MinMax startingBasePositionVertical = this.startingBasePositionVertical;
			startingBasePositionHorizontal.Mod(hMod);
			startingBasePositionVertical.Mod(vMod);
			this.startingBasePositionHorizontal = startingBasePositionHorizontal;
			this.startingBasePositionVertical = startingBasePositionVertical;
		}

		public string filePath;

		public enum Skip
		{
			Never,
			False = 0,
			Always = 99,
			True = 99,
			EditorOnly
		}

		public enum LayoutMethod
		{
			Default,
			VoronoiTree = 0,
			PowerTree
		}

		[Serializable]
		public class AllowedCellsFilter
		{
			public AllowedCellsFilter()
			{
				this.temperatureRanges = new List<Temperature.Range>();
				this.zoneTypes = new List<SubWorld.ZoneType>();
				this.subworldNames = new List<string>();
			}

			public World.AllowedCellsFilter.TagCommand tagcommand { get; private set; }

			public string tag { get; private set; }

			public int minDistance { get; private set; }

			public int maxDistance { get; private set; }

			public int distCmp { get; private set; }

			public World.AllowedCellsFilter.Command command { get; private set; }

			public List<Temperature.Range> temperatureRanges { get; private set; }

			public List<SubWorld.ZoneType> zoneTypes { get; private set; }

			public List<string> subworldNames { get; private set; }

			public enum TagCommand
			{
				Default,
				AtTag,
				DistanceFromTag
			}

			public enum Command
			{
				Clear,
				Replace,
				UnionWith,
				IntersectWith,
				ExceptWith,
				SymmetricExceptWith
			}
		}
	}
}
