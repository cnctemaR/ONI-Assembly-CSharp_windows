using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

namespace ProcGen
{
	public class ClusterLayouts
	{
		public ClusterLayout GetClusterData(string name)
		{
			ClusterLayout clusterLayout;
			if (this.clusterCache.TryGetValue(name, out clusterLayout))
			{
				return clusterLayout;
			}
			return null;
		}

		public Dictionary<string, string> GetStartingBaseNames()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, ClusterLayout> keyValuePair in this.clusterCache)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value.worldPlacements[keyValuePair.Value.startWorldIndex].world);
			}
			return dictionary;
		}

		public List<string> GetNames()
		{
			return new List<string>(this.clusterCache.Keys);
		}

		public void LoadFiles(string path, string addPrefix, List<YamlIO.Error> errors)
		{
			this.UpdateClusterCache(path, addPrefix, errors);
		}

		private void UpdateClusterCache(string path, string addPrefix, List<YamlIO.Error> errors)
		{
			ListPool<FileHandle, Worlds>.PooledList pooledList = ListPool<FileHandle, Worlds>.Allocate();
			FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(path, "clusters")), "*.yaml", pooledList);
			using (List<FileHandle>.Enumerator enumerator = pooledList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FileHandle cluster_file = enumerator.Current;
					ClusterLayout clusterLayout = YamlIO.LoadFile<ClusterLayout>(cluster_file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
					{
						error.file = cluster_file;
						errors.Add(error);
					}, null);
					if (clusterLayout == null)
					{
						DebugUtil.LogWarningArgs(new object[] { "Failed to load cluster: ", cluster_file.full_path });
					}
					else
					{
						if (!clusterLayout.requiredDlcId.IsNullOrWhiteSpace())
						{
							clusterLayout.requiredDlcIds = new string[] { clusterLayout.requiredDlcId };
						}
						else if (clusterLayout.requiredDlcIds == null)
						{
							clusterLayout.requiredDlcIds = new string[] { DlcManager.IsExpansion1Active() ? "EXPANSION1_ID" : "" };
						}
						if (!clusterLayout.forbiddenDlcId.IsNullOrWhiteSpace())
						{
							clusterLayout.forbiddenDlcIds = new string[] { clusterLayout.forbiddenDlcId };
						}
						if (clusterLayout.requiredDlcIds != null)
						{
							for (int i = 0; i < clusterLayout.requiredDlcIds.Length; i++)
							{
								if (clusterLayout.requiredDlcIds[i] == "VANILLA_ID")
								{
									clusterLayout.requiredDlcIds[i] = "";
								}
							}
						}
						if (clusterLayout.skip != ClusterLayout.Skip.Always && (clusterLayout.skip != ClusterLayout.Skip.EditorOnly || Application.isEditor) && DlcManager.IsCorrectDlcSubscribed(clusterLayout))
						{
							string name = ClusterLayout.GetName(cluster_file.full_path, addPrefix);
							clusterLayout.filePath = name;
							this.clusterCache[name] = clusterLayout;
						}
					}
				}
			}
			pooledList.Recycle();
		}

		public World GetWorldData(string clusterID, int worldID)
		{
			WorldPlacement worldPlacement = this.GetClusterData(clusterID).worldPlacements[worldID];
			return SettingsCache.worlds.GetWorldData(worldPlacement.world);
		}

		public Dictionary<string, ClusterLayout> clusterCache = new Dictionary<string, ClusterLayout>();
	}
}
