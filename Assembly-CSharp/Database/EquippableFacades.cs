using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klei;

namespace Database
{
	public class EquippableFacades : ResourceSet<EquippableFacadeResource>
	{
		public EquippableFacades(ResourceSet parent)
			: base("EquippableFacades", parent)
		{
			base.Initialize();
			this.Load();
		}

		public void Load()
		{
			ListPool<YamlIO.Error, EquippableFacadeResource>.PooledList errors = ListPool<YamlIO.Error, EquippableFacadeResource>.Allocate();
			List<FileHandle> list = new List<FileHandle>();
			FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(new string[] { Db.GetPath("", "equippablefacades") })), "*.yaml", list);
			YamlIO.ErrorHandler <>9__0;
			foreach (FileHandle fileHandle in list)
			{
				YamlIO.ErrorHandler errorHandler;
				if ((errorHandler = <>9__0) == null)
				{
					errorHandler = (<>9__0 = delegate(YamlIO.Error error, bool force_log_as_warning)
					{
						errors.Add(error);
					});
				}
				EquippableFacadeInfo equippableFacadeInfo = YamlIO.LoadFile<EquippableFacadeInfo>(fileHandle, errorHandler, null);
				if (equippableFacadeInfo.defID != null)
				{
					foreach (EquippableFacadeInfo.equippable equippable in equippableFacadeInfo.equippables)
					{
						this.resources.Add(new EquippableFacadeResource(equippable.name, equippable.buildoverride, equippableFacadeInfo.defID, equippable.animfile));
					}
				}
			}
			this.resources = this.resources.Distinct<EquippableFacadeResource>().ToList<EquippableFacadeResource>();
		}
	}
}
