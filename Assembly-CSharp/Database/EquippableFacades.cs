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
			this.Add("clubshirt", "CustomClothing", "body_shirt_clubshirt_kanim", "shirt_clubshirt_kanim");
			this.Add("cummerbund", "CustomClothing", "body_shirt_cummerbund_kanim", "shirt_cummerbund_kanim");
			this.Add("decor_02", "CustomClothing", "body_shirt_decor02_kanim", "shirt_decor02_kanim");
			this.Add("decor_03", "CustomClothing", "body_shirt_decor03_kanim", "shirt_decor03_kanim");
			this.Add("decor_04", "CustomClothing", "body_shirt_decor04_kanim", "shirt_decor04_kanim");
			this.Add("decor_05", "CustomClothing", "body_shirt_decor05_kanim", "shirt_decor05_kanim");
			this.Add("gaudysweater", "CustomClothing", "body_shirt_gaudysweater_kanim", "shirt_gaudysweater_kanim");
			this.Add("limone", "CustomClothing", "body_suit_limone_kanim", "suit_limone_kanim");
			this.Add("mondrian", "CustomClothing", "body_shirt_mondrian_kanim", "shirt_mondrian_kanim");
			this.Add("overalls", "CustomClothing", "body_suit_overalls_kanim", "suit_overalls_kanim");
			this.Add("triangles", "CustomClothing", "body_shirt_triangles_kanim", "shirt_triangles_kanim");
			this.Add("workout", "CustomClothing", "body_suit_workout_kanim", "suit_workout_kanim");
			this.Load();
		}

		public void Load()
		{
			ListPool<YamlIO.Error, EquippableFacadeResource>.PooledList errors = ListPool<YamlIO.Error, EquippableFacadeResource>.Allocate();
			List<FileHandle> list = new List<FileHandle>();
			DirectoryInfo directoryInfo = new DirectoryInfo(FileSystem.Normalize(Path.Combine(new string[] { Db.GetPath("", "equippablefacades") })));
			if (directoryInfo.Exists)
			{
				YamlIO.ErrorHandler <>9__0;
				foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
				{
					list.Clear();
					FileSystem.GetFiles(directoryInfo2.FullName, "*.yaml", list);
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
						DebugUtil.DevAssert(string.Equals(directoryInfo2.Name, equippableFacadeInfo.defID, StringComparison.OrdinalIgnoreCase), "DefID mismatch!", null);
						if (equippableFacadeInfo.defID != null)
						{
							this.resources.Add(new EquippableFacadeResource(equippableFacadeInfo.name, equippableFacadeInfo.buildoverride, equippableFacadeInfo.defID, equippableFacadeInfo.animfile));
						}
					}
				}
			}
			this.resources = this.resources.Distinct<EquippableFacadeResource>().ToList<EquippableFacadeResource>();
			errors.Recycle();
		}

		public void Add(string id, string defID, string buildOverride, string animFile)
		{
			EquippableFacadeResource equippableFacadeResource = new EquippableFacadeResource(id, buildOverride, defID, animFile);
			this.resources.Add(equippableFacadeResource);
		}
	}
}
