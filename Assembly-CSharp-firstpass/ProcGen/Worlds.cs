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
			World world;
			if (!name.IsNullOrWhiteSpace() && this.worldCache.TryGetValue(name, out world))
			{
				return world;
			}
			return null;
		}

		public List<string> GetNames()
		{
			return new List<string>(this.worldCache.Keys);
		}

		public static string GetWorldName(string path, string prefix)
		{
			return prefix + "worlds/" + Path.GetFileNameWithoutExtension(path);
		}

		public string GetIconFilename(string iconName)
		{
			if (!DlcManager.FeatureClusterSpaceEnabled())
			{
				return "Asteroid_sandstone";
			}
			return "asteroid_sandstone_start_kanim";
		}

		public void LoadReferencedWorlds(string path, string prefix, ISet<string> referencedWorlds, List<YamlIO.Error> errors)
		{
			this.UpdateWorldCache(path, prefix, referencedWorlds, errors);
		}

		private void UpdateWorldCache(string path, string prefix, ISet<string> referencedWorlds, List<YamlIO.Error> errors)
		{
			ListPool<FileHandle, Worlds>.PooledList pooledList = ListPool<FileHandle, Worlds>.Allocate();
			FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(path, "worlds/")), "*.yaml", pooledList);
			YamlIO.ErrorHandler <>9__0;
			foreach (FileHandle fileHandle in pooledList)
			{
				string text = fileHandle.full_path.Substring(path.Length);
				text = text.Remove(text.LastIndexOf(".yaml"));
				string text2 = prefix + text;
				if (referencedWorlds.Contains(text2))
				{
					string full_path = fileHandle.full_path;
					YamlIO.ErrorHandler errorHandler;
					if ((errorHandler = <>9__0) == null)
					{
						errorHandler = (<>9__0 = delegate(YamlIO.Error error, bool force_log_as_warning)
						{
							errors.Add(error);
						});
					}
					World world = YamlIO.LoadFile<World>(full_path, errorHandler, null);
					if (world == null)
					{
						DebugUtil.LogWarningArgs(new object[] { "Failed to load world: ", fileHandle.full_path });
					}
					else if (world.skip != World.Skip.Always && (world.skip != World.Skip.EditorOnly || Application.isEditor))
					{
						world.filePath = Worlds.GetWorldName(fileHandle.full_path, prefix);
						this.worldCache[world.filePath] = world;
					}
				}
			}
			pooledList.Recycle();
		}

		public void Validate()
		{
			foreach (KeyValuePair<string, World> keyValuePair in this.worldCache)
			{
				keyValuePair.Value.Validate();
			}
		}

		public Dictionary<string, World> worldCache = new Dictionary<string, World>();
	}
}
