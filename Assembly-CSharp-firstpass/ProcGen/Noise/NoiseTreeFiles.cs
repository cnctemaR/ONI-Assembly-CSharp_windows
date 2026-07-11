using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using LibNoiseDotNet.Graphics.Tools.Noise;
using UnityEngine;

namespace ProcGen.Noise
{
	public class NoiseTreeFiles : YamlIO<NoiseTreeFiles>
	{
		public NoiseTreeFiles()
		{
			this.trees = new Dictionary<string, Tree>();
			this.tree_files = new List<string>();
		}

		public static string GetPath()
		{
			return Path.Combine(Application.streamingAssetsPath, "worldgen/" + NoiseTreeFiles.NOISE_FILE + ".yaml");
		}

		public static string GetTreeFilePath(string filename)
		{
			return Path.Combine(Application.streamingAssetsPath, "worldgen/noise/" + filename + ".yaml");
		}

		public List<string> tree_files { get; set; }

		public void LoadAllTrees()
		{
			for (int i = 0; i < this.tree_files.Count; i++)
			{
				Tree tree = YamlIO<Tree>.LoadFile(NoiseTreeFiles.GetTreeFilePath(this.tree_files[i]), null);
				if (tree != null)
				{
					this.trees.Add(this.tree_files[i], tree);
				}
			}
		}

		public Tree LoadTree(string name, string path)
		{
			if (name != null && name.Length > 0)
			{
				if (!this.trees.ContainsKey(name))
				{
					Tree tree = YamlIO<Tree>.LoadFile(path + name + ".yaml", null);
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

		public Tree GetTree(string name, string path)
		{
			if (!this.trees.ContainsKey(name))
			{
				Tree tree = YamlIO<Tree>.LoadFile(path + "/" + name + ".yaml", null);
				if (tree == null)
				{
					return null;
				}
				this.trees.Add(name, tree);
			}
			return this.trees[name];
		}

		public Tree GetTree(string name)
		{
			if (!this.trees.ContainsKey(name))
			{
				return null;
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
