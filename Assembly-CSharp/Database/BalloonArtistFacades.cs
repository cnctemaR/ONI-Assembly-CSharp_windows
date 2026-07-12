using System;
using STRINGS;

namespace Database
{
	public class BalloonArtistFacades : ResourceSet<BalloonArtistFacadeResource>
	{
		public BalloonArtistFacades(ResourceSet parent)
			: base("BalloonArtistFacades", parent)
		{
			foreach (BalloonArtistFacades.Info info in BalloonArtistFacades.Infos_All)
			{
				this.Add(info.id, info.name, info.desc, info.rarity, info.animFile, info.balloonFacadeType);
			}
		}

		public void Add(string id, string name, string desc, PermitRarity rarity, string animFile, BalloonArtistFacadeType balloonFacadeType)
		{
			BalloonArtistFacadeResource balloonArtistFacadeResource = new BalloonArtistFacadeResource(id, name, desc, rarity, animFile, balloonFacadeType);
			this.resources.Add(balloonArtistFacadeResource);
		}

		public static BalloonArtistFacades.Info[] Infos_Skins = new BalloonArtistFacades.Info[]
		{
			new BalloonArtistFacades.Info("BalloonRedFireEngineLongSparkles", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_FIREENGINE_LONG_SPARKLES.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_FIREENGINE_LONG_SPARKLES.DESC, PermitRarity.Common, "balloon_red_fireengine_long_sparkles_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonYellowLongSparkles", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_YELLOW_LONG_SPARKLES.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_YELLOW_LONG_SPARKLES.DESC, PermitRarity.Common, "balloon_yellow_long_sparkles_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonBlueLongSparkles", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BLUE_LONG_SPARKLES.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BLUE_LONG_SPARKLES.DESC, PermitRarity.Common, "balloon_blue_long_sparkles_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonGreenLongSparkles", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_GREEN_LONG_SPARKLES.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_GREEN_LONG_SPARKLES.DESC, PermitRarity.Common, "balloon_green_long_sparkles_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonPinkLongSparkles", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_PINK_LONG_SPARKLES.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_PINK_LONG_SPARKLES.DESC, PermitRarity.Common, "balloon_pink_long_sparkles_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonPurpleLongSparkles", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_PURPLE_LONG_SPARKLES.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_PURPLE_LONG_SPARKLES.DESC, PermitRarity.Common, "balloon_purple_long_sparkles_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonBabyPacuEgg", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_PACU_EGG.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_PACU_EGG.DESC, PermitRarity.Splendid, "balloon_babypacu_egg_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonBabyGlossyDreckoEgg", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_GLOSSY_DRECKO_EGG.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_GLOSSY_DRECKO_EGG.DESC, PermitRarity.Splendid, "balloon_babyglossydrecko_egg_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonBabyHatchEgg", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_HATCH_EGG.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_HATCH_EGG.DESC, PermitRarity.Splendid, "balloon_babyhatch_egg_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonBabyPokeshellEgg", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_POKESHELL_EGG.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_POKESHELL_EGG.DESC, PermitRarity.Splendid, "balloon_babypokeshell_egg_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonBabyPuftEgg", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_PUFT_EGG.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_PUFT_EGG.DESC, PermitRarity.Splendid, "balloon_babypuft_egg_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonBabyShovoleEgg", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_SHOVOLE_EGG.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_SHOVOLE_EGG.DESC, PermitRarity.Splendid, "balloon_babyshovole_egg_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonBabyPipEgg", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_PIP_EGG.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.BALLOON_BABY_PIP_EGG.DESC, PermitRarity.Splendid, "balloon_babypip_egg_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonCandyBlueberry", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_BLUEBERRY.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_BLUEBERRY.DESC, PermitRarity.Decent, "balloon_candy_blueberry_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonCandyGrape", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_GRAPE.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_GRAPE.DESC, PermitRarity.Decent, "balloon_candy_grape_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonCandyLemon", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_LEMON.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_LEMON.DESC, PermitRarity.Decent, "balloon_candy_lemon_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonCandyLime", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_LIME.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_LIME.DESC, PermitRarity.Decent, "balloon_candy_lime_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonCandyOrange", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_ORANGE.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_ORANGE.DESC, PermitRarity.Decent, "balloon_candy_orange_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonCandyStrawberry", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_STRAWBERRY.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_STRAWBERRY.DESC, PermitRarity.Decent, "balloon_candy_strawberry_kanim", BalloonArtistFacadeType.ThreeSet),
			new BalloonArtistFacades.Info("BalloonCandyWatermelon", EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_WATERMELON.NAME, EQUIPMENT.PREFABS.EQUIPPABLEBALLOON.FACADES.CANDY_WATERMELON.DESC, PermitRarity.Decent, "balloon_candy_watermelon_kanim", BalloonArtistFacadeType.ThreeSet)
		};

		public static BalloonArtistFacades.Info[] Infos_All = BalloonArtistFacades.Infos_Skins;

		public struct Info
		{
			public Info(string id, string name, string desc, PermitRarity rarity, string animFile, BalloonArtistFacadeType balloonFacadeType)
			{
				this.id = id;
				this.name = name;
				this.desc = desc;
				this.rarity = rarity;
				this.animFile = animFile;
				this.balloonFacadeType = balloonFacadeType;
			}

			public string id;

			public string name;

			public string desc;

			public PermitRarity rarity;

			public string animFile;

			public BalloonArtistFacadeType balloonFacadeType;
		}
	}
}
