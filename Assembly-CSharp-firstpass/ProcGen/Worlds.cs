using System;
using System.Collections.Generic;
using System.IO;
using Klei;

namespace ProcGen
{
	public class Worlds
	{
		public bool HasWorld(string name)
		{
			return this.worldCache.ContainsKey(name);
		}

		public Worlds.Data GetWorldData(string name)
		{
			return this.worldCache[name];
		}

		public List<string> GetNames()
		{
			return new List<string>(this.worldCache.Keys);
		}

		public static string GetWorldName(string path)
		{
			return "worlds/" + Path.GetFileNameWithoutExtension(path);
		}

		public void LoadFiles(string path, IFileSystem filesystem)
		{
			this.worldCache.Clear();
			this.UpdateWorldCache(path, filesystem);
		}

		private void UpdateWorldCache(string path, IFileSystem filesystem)
		{
			List<string> list = new List<string>();
			FSUtil.GetFiles(filesystem, FSUtil.Normalize(Path.Combine(path, "worlds")), "*.yaml", list);
			foreach (string text in list)
			{
				World world = YamlIO<World>.LoadFile(text, null);
				string worldName = Worlds.GetWorldName(text);
				this.worldCache[worldName] = new Worlds.Data
				{
					world = world
				};
			}
		}

		public Dictionary<string, Worlds.Data> worldCache = new Dictionary<string, Worlds.Data>();

		public struct Data
		{
			public World world;
		}
	}
}
