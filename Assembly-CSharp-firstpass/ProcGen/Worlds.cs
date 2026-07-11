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
			if (this.worldCache.ContainsKey(name))
			{
				return this.worldCache[name];
			}
			return null;
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
				WorkItemCollection<Worlds.LoadWorldfileWorkItem, object> workItemCollection = new WorkItemCollection<Worlds.LoadWorldfileWorkItem, object>();
				workItemCollection.Reset(null);
				foreach (string text in files)
				{
					workItemCollection.Add(new Worlds.LoadWorldfileWorkItem
					{
						path = text
					});
				}
				GlobalJobManager.Run(workItemCollection);
				for (int j = 0; j < workItemCollection.Count; j++)
				{
					Worlds.LoadWorldfileWorkItem workItem = workItemCollection.GetWorkItem(j);
					if (workItem.world != null)
					{
						this.worldCache[workItem.worldName] = workItem.world;
					}
					else
					{
						Debug.LogWarning("WorldGen: Attempting to load world: " + workItem.worldName + " failed", null);
					}
				}
			}
		}

		public Dictionary<string, World> worldCache = new Dictionary<string, World>();

		private struct LoadWorldfileWorkItem : IWorkItem<object>
		{
			public void Run(object shared_data)
			{
				int num = this.path.LastIndexOf("//");
				this.worldName = ((num != -1) ? this.path.Substring(num + 2, this.path.Length - 2 - num) : this.path);
				this.world = YamlIO<World>.LoadFile(this.path);
				if (this.world != null)
				{
					this.worldName = this.worldName.Replace(".yaml", string.Empty);
				}
				else
				{
					Debug.LogWarning("WorldGen: Attempting to load world: " + this.worldName + " failed", null);
				}
			}

			public string path;

			public string worldName;

			public World world;
		}
	}
}
