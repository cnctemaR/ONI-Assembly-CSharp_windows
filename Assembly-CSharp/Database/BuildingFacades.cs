using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klei;
using STRINGS;

namespace Database
{
	public class BuildingFacades : ResourceSet<BuildingFacadeResource>
	{
		public BuildingFacades(ResourceSet parent)
			: base("BuildingFacades", parent)
		{
			base.Initialize();
			this.Load();
			foreach (BuildingFacades.Info info in BuildingFacades.Infos)
			{
				this.Add(info.Id, info.Name, info.Description, info.Rarity, info.PrefabID, info.AnimFile, null);
			}
		}

		public void Load()
		{
			ListPool<YamlIO.Error, BuildingFacades>.PooledList pooledList = ListPool<YamlIO.Error, BuildingFacades>.Allocate();
			this.LoadBuildingFacades(pooledList);
			pooledList.Recycle();
		}

		private void LoadBuildingFacades(List<YamlIO.Error> errors)
		{
			List<FileHandle> list = new List<FileHandle>();
			DirectoryInfo directoryInfo = new DirectoryInfo(FileSystem.Normalize(Path.Combine(new string[] { Db.GetPath("", "buildingfacades") })));
			if (directoryInfo.Exists)
			{
				foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
				{
					list.Clear();
					FileSystem.GetFiles(directoryInfo2.FullName, "*.yaml", list);
					foreach (FileHandle fileHandle in list)
					{
						this.LoadBuildingFacade(directoryInfo2.Name, fileHandle, errors);
					}
				}
			}
			this.resources = this.resources.Distinct<BuildingFacadeResource>().ToList<BuildingFacadeResource>();
		}

		private void LoadBuildingFacade(string sub_dir, FileHandle file, List<YamlIO.Error> errors)
		{
			FacadeInfo facadeInfo = YamlIO.LoadFile<FacadeInfo>(file, delegate(YamlIO.Error error, bool force_log_as_warning)
			{
				errors.Add(error);
			}, null);
			DebugUtil.DevAssert(string.Equals(sub_dir, facadeInfo.prefabID, StringComparison.OrdinalIgnoreCase), "Mismatched prefab & facade!", null);
			KAnimFile kanimFile;
			if (!Assets.TryGetAnim(facadeInfo.animFile, out kanimFile))
			{
				Debug.LogWarning(string.Concat(new string[] { "Building facade on ", facadeInfo.prefabID, " could not be loaded due to missing ", facadeInfo.animFile, " on facade ID '", facadeInfo.id, "'" }));
				return;
			}
			bool flag = true;
			if (facadeInfo.workables != null)
			{
				foreach (FacadeInfo.workable workable in facadeInfo.workables)
				{
					KAnimFile kanimFile2;
					if (!Assets.TryGetAnim(workable.workableAnim, out kanimFile2))
					{
						Debug.LogWarning(string.Concat(new string[] { "Building facade on ", facadeInfo.prefabID, " could not be loaded due to missing workable anim '", workable.workableAnim, "' on facade ID '", facadeInfo.id, "'" }));
					}
				}
			}
			if (!flag)
			{
				return;
			}
			BuildingFacadeResource facade = new BuildingFacadeResource(facadeInfo.id, facadeInfo.name, facadeInfo.description, PermitRarity.Unknown, facadeInfo.prefabID, facadeInfo.animFile, facadeInfo.workables);
			if (this.resources.Exists((BuildingFacadeResource f) => f.Id == facade.Id))
			{
				Debug.LogWarning("Building facade on " + facadeInfo.prefabID + " already contains " + facade.Id);
			}
			this.resources.Add(facade);
		}

		public void Add(string id, LocString Name, LocString Desc, PermitRarity rarity, string prefabId, string animFile, Dictionary<string, string> workables = null)
		{
			BuildingFacadeResource buildingFacadeResource = new BuildingFacadeResource(id, Name, Desc, rarity, prefabId, animFile, workables);
			this.resources.Add(buildingFacadeResource);
		}

		public void PostProcess()
		{
			foreach (BuildingFacadeResource buildingFacadeResource in this.resources)
			{
				buildingFacadeResource.Init();
			}
		}

		public static BuildingFacades.Info[] Infos = new BuildingFacades.Info[]
		{
			new BuildingFacades.Info("FlowerVase_retro", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_SUNNY.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_SUNNY.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_kanim"),
			new BuildingFacades.Info("FlowerVase_retro_red", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BOLD.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BOLD.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_red_kanim"),
			new BuildingFacades.Info("FlowerVase_retro_white", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_ELEGANT.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_ELEGANT.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_white_kanim"),
			new BuildingFacades.Info("FlowerVase_retro_green", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BRIGHT.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BRIGHT.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_green_kanim"),
			new BuildingFacades.Info("FlowerVase_retro_blue", BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_DREAMY.NAME, BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_DREAMY.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_blue_kanim"),
			new BuildingFacades.Info("LuxuryBed_boat", BUILDINGS.PREFABS.LUXURYBED.FACADES.BOAT.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.BOAT.DESC, PermitRarity.Splendid, LuxuryBedConfig.ID, "elegantbed_boat_kanim"),
			new BuildingFacades.Info("LuxuryBed_bouncy", BUILDINGS.PREFABS.LUXURYBED.FACADES.BOUNCY_BED.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.BOUNCY_BED.DESC, PermitRarity.Splendid, LuxuryBedConfig.ID, "elegantbed_bouncy_kanim"),
			new BuildingFacades.Info("LuxuryBed_grandprix", BUILDINGS.PREFABS.LUXURYBED.FACADES.GRANDPRIX.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.GRANDPRIX.DESC, PermitRarity.Splendid, LuxuryBedConfig.ID, "elegantbed_grandprix_kanim"),
			new BuildingFacades.Info("LuxuryBed_rocket", BUILDINGS.PREFABS.LUXURYBED.FACADES.ROCKET_BED.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.ROCKET_BED.DESC, PermitRarity.Splendid, LuxuryBedConfig.ID, "elegantbed_rocket_kanim"),
			new BuildingFacades.Info("LuxuryBed_puft", BUILDINGS.PREFABS.LUXURYBED.FACADES.PUFT_BED.NAME, BUILDINGS.PREFABS.LUXURYBED.FACADES.PUFT_BED.DESC, PermitRarity.Loyalty, LuxuryBedConfig.ID, "elegantbed_puft_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_pink", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPINK.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPINK.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_pink_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_yellow", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELYELLOW.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELYELLOW.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_yellow_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_green", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELGREEN.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELGREEN.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_green_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_blue", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELBLUE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELBLUE.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_blue_kanim"),
			new BuildingFacades.Info("ExteriorWall_pastel_purple", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPURPLE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPURPLE.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_purple_kanim"),
			new BuildingFacades.Info("ExteriorWall_balm_lily", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BALM_LILY.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BALM_LILY.DESC, PermitRarity.Decent, "ExteriorWall", "walls_balm_lily_kanim"),
			new BuildingFacades.Info("ExteriorWall_clouds", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CLOUDS.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CLOUDS.DESC, PermitRarity.Decent, "ExteriorWall", "walls_clouds_kanim"),
			new BuildingFacades.Info("ExteriorWall_coffee", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.COFFEE.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.COFFEE.DESC, PermitRarity.Decent, "ExteriorWall", "walls_coffee_kanim"),
			new BuildingFacades.Info("ExteriorWall_mosaic", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.AQUATICMOSAIC.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.AQUATICMOSAIC.DESC, PermitRarity.Decent, "ExteriorWall", "walls_mosaic_kanim"),
			new BuildingFacades.Info("ExteriorWall_mushbar", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.MUSHBAR.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.MUSHBAR.DESC, PermitRarity.Decent, "ExteriorWall", "walls_mushbar_kanim"),
			new BuildingFacades.Info("ExteriorWall_plaid", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLAID.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLAID.DESC, PermitRarity.Decent, "ExteriorWall", "walls_plaid_kanim"),
			new BuildingFacades.Info("ExteriorWall_rain", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAIN.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAIN.DESC, PermitRarity.Decent, "ExteriorWall", "walls_rain_kanim"),
			new BuildingFacades.Info("ExteriorWall_rainbow", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAINBOW.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAINBOW.DESC, PermitRarity.Decent, "ExteriorWall", "walls_rainbow_kanim"),
			new BuildingFacades.Info("ExteriorWall_snow", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SNOW.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SNOW.DESC, PermitRarity.Decent, "ExteriorWall", "walls_snow_kanim"),
			new BuildingFacades.Info("ExteriorWall_sun", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SUN.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SUN.DESC, PermitRarity.Decent, "ExteriorWall", "walls_sun_kanim"),
			new BuildingFacades.Info("ExteriorWall_polka", BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.NAME, BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.DESC, PermitRarity.Decent, "ExteriorWall", "walls_polka_kanim")
		};

		public struct Info
		{
			public Info(string Id, string Name, string Description, PermitRarity rarity, string PrefabID, string AnimFile)
			{
				this.Id = Id;
				this.Name = Name;
				this.Description = Description;
				this.Rarity = rarity;
				this.PrefabID = PrefabID;
				this.AnimFile = AnimFile;
			}

			public string Id;

			public string Name;

			public string Description;

			public PermitRarity Rarity;

			public string PrefabID;

			public string AnimFile;
		}
	}
}
