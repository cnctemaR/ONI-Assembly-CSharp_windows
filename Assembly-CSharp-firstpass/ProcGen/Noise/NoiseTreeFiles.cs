using System;
using System.Collections.Generic;
using Klei;
using LibNoiseDotNet.Graphics.Tools.Noise;

namespace ProcGen.Noise
{
	public class NoiseTreeFiles
	{
		public static string GetDirectoryRel()
		{
			return "worldgen/noise/";
		}

		public static string GetPathRel()
		{
			return "worldgen/" + NoiseTreeFiles.NOISE_FILE + ".yaml";
		}

		public static string GetTreeFilePathRel(string filename)
		{
			return "worldgen/noise/" + filename + ".yaml";
		}

		public List<string> tree_files { get; set; }

		public void Clear()
		{
			this.tree_files.Clear();
			this.trees.Clear();
		}

		public NoiseTreeFiles()
		{
			this.trees = new Dictionary<string, Tree>();
			this.tree_files = new List<string>();
		}

		public Tree LoadTree(string name)
		{
			if (name != null && name.Length > 0)
			{
				if (!this.trees.ContainsKey(name))
				{
					Tree tree = YamlIO.LoadFile<Tree>(SettingsCache.RewriteWorldgenPathYaml(name), null, null);
					if (tree != null)
					{
						this.trees.Add(name, tree);
					}
				}
				return this.trees[name];
			}
			return null;
		}

		public float GetZoomForTree(string name)
		{
			if (!this.trees.ContainsKey(name))
			{
				return 1f;
			}
			return this.trees[name].settings.zoom;
		}

		public bool ShouldNormaliseTree(string name)
		{
			return this.trees.ContainsKey(name) && this.trees[name].settings.normalise;
		}

		public string[] GetTreeNames()
		{
			string[] array = new string[this.trees.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, Tree> keyValuePair in this.trees)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}

		public Tree GetTree(string name)
		{
			if (!this.trees.ContainsKey(name))
			{
				string text = SettingsCache.RewriteWorldgenPathYaml(name);
				Tree tree = YamlIO.LoadFile<Tree>(text, null, null);
				if (tree == null)
				{
					DebugUtil.LogArgs(new object[] { "NoiseArgs.GetTree failed to load " + name + " at " + text });
					return null;
				}
				this.trees.Add(name, tree);
			}
			return this.trees[name];
		}

		public IModule3D BuildTree(string name, int globalSeed)
		{
			if (!this.trees.ContainsKey(name))
			{
				return null;
			}
			return this.trees[name].BuildFinalModule(globalSeed);
		}

		public static string NOISE_FILE = "noise";

		private Dictionary<string, Tree> trees;
	}
}
