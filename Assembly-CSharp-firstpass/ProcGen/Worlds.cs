using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

namespace ProcGen
{
	public class Worlds
	{
		public bool HasWorld(string name)
		{
			return name != null && this.worldCache.ContainsKey(name);
		}

		public World GetWorldData(string name)
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

		public void LoadFiles(string path, List<YamlIO.Error> errors)
		{
			this.worldCache.Clear();
			this.UpdateWorldCache(path, errors);
		}

		private void UpdateWorldCache(string path, List<YamlIO.Error> errors)
		{
			ListPool<FileHandle, Worlds>.PooledList pooledList = ListPool<FileHandle, Worlds>.Allocate();
			FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(path, "worlds")), "*.yaml", pooledList);
			using (List<FileHandle>.Enumerator enumerator = pooledList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FileHandle world_file = enumerator.Current;
					World world = YamlIO.LoadFile<World>(world_file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
					{
						error.file = world_file;
						errors.Add(error);
					}, null);
					if (world == null)
					{
						DebugUtil.LogWarningArgs(new object[] { "Failed to load world: ", world_file.full_path });
					}
					else if (world.skip != World.Skip.Always && (world.skip != World.Skip.EditorOnly || Application.isEditor))
					{
						world.filePath = Worlds.GetWorldName(world_file.full_path);
						this.worldCache[world.filePath] = world;
					}
				}
			}
			pooledList.Recycle();
		}

		public Dictionary<string, World> worldCache = new Dictionary<string, World>();
	}
}
