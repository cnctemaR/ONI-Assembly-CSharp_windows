using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

namespace ProcGen
{
	public class Worlds
	{
		public static string GetName(string path, string addPrefix)
		{
			return FileSystem.Normalize(Path.Combine(addPrefix + "worlds", Path.GetFileNameWithoutExtension(path)));
		}

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

		public World GetWorldData(WorldPlacement placement, int seed)
		{
			return this.GetWorldData(placement.world);
		}

		public List<string> GetNames()
		{
			return new List<string>(this.worldCache.Keys);
		}

		public string GetWorldKey(World world)
		{
			foreach (KeyValuePair<string, World> keyValuePair in this.worldCache)
			{
				if (keyValuePair.Value == world)
				{
					return keyValuePair.Key;
				}
			}
			return null;
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

		public void LoadReferencedWorlds(ISet<string> referencedWorlds, List<YamlIO.Error> errors)
		{
			this.UpdateWorldCache(referencedWorlds, errors);
		}

		private void UpdateWorldCache(ISet<string> referencedWorlds, List<YamlIO.Error> errors)
		{
			YamlIO.ErrorHandler <>9__0;
			foreach (string text in referencedWorlds)
			{
				if (!this.worldCache.ContainsKey(text))
				{
					string text2 = SettingsCache.RewriteWorldgenPathYaml(text);
					string text3 = text2;
					YamlIO.ErrorHandler errorHandler;
					if ((errorHandler = <>9__0) == null)
					{
						errorHandler = (<>9__0 = delegate(YamlIO.Error error, bool force_log_as_warning)
						{
							errors.Add(error);
						});
					}
					World world = YamlIO.LoadFile<World>(text3, errorHandler, null);
					if (world == null)
					{
						DebugUtil.LogWarningArgs(new object[] { "Failed to load world: ", text2 });
					}
					else if (world.skip != World.Skip.Always && (world.skip != World.Skip.EditorOnly || Application.isEditor))
					{
						world.filePath = text;
						this.worldCache[world.filePath] = world;
					}
				}
			}
		}

		public void LoadFiles(string rootDirectory, string prefix, List<YamlIO.Error> errors)
		{
			ListPool<FileHandle, Worlds>.PooledList pooledList = ListPool<FileHandle, Worlds>.Allocate();
			FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(rootDirectory, "worlds")), "*.yaml", pooledList);
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
					else if (world.skip != World.Skip.Always && (world.skip != World.Skip.EditorOnly || Application.isEditor) && (world.requiredDlcIds == null || DlcManager.IsAllContentSubscribed(world.requiredDlcIds)) && (world.forbiddenDlcIds == null || !DlcManager.IsAnyContentSubscribed(world.forbiddenDlcIds)))
					{
						string name = Worlds.GetName(world_file.full_path, prefix);
						world.filePath = name;
						world.isModded = world_file.source.IsModded();
						this.worldCache[name] = world;
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

		public const string directory = "worlds";

		public Dictionary<string, World> worldCache = new Dictionary<string, World>();
	}
}
