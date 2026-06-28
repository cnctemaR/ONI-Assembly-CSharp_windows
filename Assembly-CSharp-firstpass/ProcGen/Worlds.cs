using System;
using System.Collections.Generic;
using System.IO;
using Klei;

namespace ProcGen
{
	public class Worlds
	{
		public World GetWorld(string name)
		{
			World world;
			if (this.worldCache.ContainsKey(name))
			{
				world = this.worldCache[name];
			}
			else
			{
				world = null;
			}
			return world;
		}

		public List<string> GetNames()
		{
			return new List<string>(this.worldCache.Keys);
		}

		public void LoadFiles(string path)
		{
			this.worldCache.Clear();
			string[] files = Directory.GetFiles(path + "/worlds/", "*.yaml");
			if (files == null || files.Length == 0)
			{
				Debug.LogError("WorldGen: No world lookup table files will be loaded", null);
			}
			else
			{
				for (int i = 0; i < files.Length; i++)
				{
					int num = files[i].LastIndexOf("//");
					string text = ((num != -1) ? files[i].Substring(num + 2, files[i].Length - 2 - num) : files[i]);
					World world = YamlIO<World>.LoadFile(files[i]);
					if (world != null)
					{
						text = text.Replace(".yaml", "");
						this.worldCache[text] = world;
					}
					else
					{
						Debug.LogWarning("WorldGen: Attempting to load world: " + text + " failed", null);
					}
				}
			}
		}

		public Dictionary<string, World> worldCache = new Dictionary<string, World>();
	}
}
