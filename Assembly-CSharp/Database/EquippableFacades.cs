using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klei;
using STRINGS;

namespace Database
{
	public class EquippableFacades : ResourceSet<EquippableFacadeResource>
	{
		public EquippableFacades(ResourceSet parent)
			: base("EquippableFacades", parent)
		{
			base.Initialize();
			foreach (EquippableFacades.Info info in EquippableFacades.Infos_All)
			{
				this.Add(info.id, info.name, info.defID, info.buildOverride, info.animFile);
			}
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
							string text = ((equippableFacadeInfo.name != null) ? Strings.Get(equippableFacadeInfo.name) : "NAME NOT POPULATED (DbEquippableFacades)");
							this.resources.Add(new EquippableFacadeResource(equippableFacadeInfo.id, text, equippableFacadeInfo.buildoverride, equippableFacadeInfo.defID, equippableFacadeInfo.animfile));
							InventoryOrganization.subcategoryIdToPermitIdsMap["YAML"].Add(equippableFacadeInfo.id);
						}
					}
				}
			}
			this.resources = this.resources.Distinct<EquippableFacadeResource>().ToList<EquippableFacadeResource>();
			errors.Recycle();
		}

		public void Add(string id, string name, string defID, string buildOverride, string animFile)
		{
			EquippableFacadeResource equippableFacadeResource = new EquippableFacadeResource(id, name, buildOverride, defID, animFile);
			this.resources.Add(equippableFacadeResource);
		}

		public static EquippableFacades.Info[] Infos_Default = new EquippableFacades.Info[]
		{
			new EquippableFacades.Info("clubshirt", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.CLUBSHIRT, "CustomClothing", "body_shirt_clubshirt_kanim", "shirt_clubshirt_kanim"),
			new EquippableFacades.Info("cummerbund", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.CUMMERBUND, "CustomClothing", "body_shirt_cummerbund_kanim", "shirt_cummerbund_kanim"),
			new EquippableFacades.Info("decor_02", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_02, "CustomClothing", "body_shirt_decor02_kanim", "shirt_decor02_kanim"),
			new EquippableFacades.Info("decor_03", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_03, "CustomClothing", "body_shirt_decor03_kanim", "shirt_decor03_kanim"),
			new EquippableFacades.Info("decor_04", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_04, "CustomClothing", "body_shirt_decor04_kanim", "shirt_decor04_kanim"),
			new EquippableFacades.Info("decor_05", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_05, "CustomClothing", "body_shirt_decor05_kanim", "shirt_decor05_kanim"),
			new EquippableFacades.Info("gaudysweater", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.GAUDYSWEATER, "CustomClothing", "body_shirt_gaudysweater_kanim", "shirt_gaudysweater_kanim"),
			new EquippableFacades.Info("limone", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.LIMONE, "CustomClothing", "body_suit_limone_kanim", "suit_limone_kanim"),
			new EquippableFacades.Info("mondrian", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.MONDRIAN, "CustomClothing", "body_shirt_mondrian_kanim", "shirt_mondrian_kanim"),
			new EquippableFacades.Info("overalls", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.OVERALLS, "CustomClothing", "body_suit_overalls_kanim", "suit_overalls_kanim"),
			new EquippableFacades.Info("triangles", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.TRIANGLES, "CustomClothing", "body_shirt_triangles_kanim", "shirt_triangles_kanim"),
			new EquippableFacades.Info("workout", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.WORKOUT, "CustomClothing", "body_suit_workout_kanim", "suit_workout_kanim")
		};

		public static EquippableFacades.Info[] Infos_Skins = new EquippableFacades.Info[0];

		public static EquippableFacades.Info[] Infos_All = EquippableFacades.Infos_Default;

		public struct Info
		{
			public Info(string id, string name, string defID, string buildOverride, string animFile)
			{
				this.id = id;
				this.name = name;
				this.defID = defID;
				this.buildOverride = buildOverride;
				this.animFile = animFile;
			}

			public string id;

			public string name;

			public string buildOverride;

			public string defID;

			public string animFile;
		}
	}
}
