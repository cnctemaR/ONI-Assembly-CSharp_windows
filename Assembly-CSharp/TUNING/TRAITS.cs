using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

namespace TUNING
{
	public class TRAITS
	{
		private static global::System.Action CreateDisabledTaskTrait(string id, string name, string desc, string disabled_chore_group, bool is_valid_starter_trait)
		{
			return delegate
			{
				ChoreGroup[] array = new ChoreGroup[] { Db.Get().ChoreGroups.Get(disabled_chore_group) };
				Db.Get().CreateTrait(id, name, desc, null, true, array, false, is_valid_starter_trait);
			};
		}

		private static global::System.Action CreateTrait(string id, string name, string desc, string attributeId, float delta, string[] chore_groups, bool positiveTrait = false)
		{
			return delegate
			{
				List<ChoreGroup> list = new List<ChoreGroup>();
				foreach (string text in chore_groups)
				{
					list.Add(Db.Get().ChoreGroups.Get(text));
				}
				Db.Get().CreateTrait(id, name, desc, null, true, list.ToArray(), positiveTrait, true).Add(new AttributeModifier(attributeId, delta, name, false, false, true));
			};
		}

		private static global::System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, string attributeId2, float delta2, bool positiveTrait = false)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				trait.Add(new AttributeModifier(attributeId, delta, name, false, false, true));
				trait.Add(new AttributeModifier(attributeId2, delta2, name, false, false, true));
			};
		}

		private static global::System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, bool positiveTrait = false, Action<GameObject> on_add = null)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				trait.Add(new AttributeModifier(attributeId, delta, name, false, false, true));
				trait.OnAddTrait = on_add;
			};
		}

		private static global::System.Action CreateEffectModifierTrait(string id, string name, string desc, string[] ignoredEffects, bool positiveTrait = false)
		{
			return delegate
			{
				Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true).AddIgnoredEffects(ignoredEffects);
			};
		}

		private static global::System.Action CreateNamedTrait(string id, string name, string desc, bool positiveTrait = false)
		{
			return delegate
			{
				Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
			};
		}

		private static global::System.Action CreateTrait(string id, string name, string desc, Action<GameObject> on_add, ChoreGroup[] disabled_chore_groups = null, bool positiveTrait = false, Func<string> extendedDescFn = null)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, disabled_chore_groups, positiveTrait, true);
				trait.OnAddTrait = on_add;
				if (extendedDescFn != null)
				{
					Trait trait2 = trait;
					trait2.ExtendedTooltip = (Func<string>)Delegate.Combine(trait2.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		private static global::System.Action CreateComponentTrait<T>(string id, string name, string desc, bool positiveTrait = false, Func<string> extendedDescFn = null) where T : KMonoBehaviour
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				trait.OnAddTrait = delegate(GameObject go)
				{
					go.FindOrAddUnityComponent<T>();
				};
				if (extendedDescFn != null)
				{
					Trait trait2 = trait;
					trait2.ExtendedTooltip = (Func<string>)Delegate.Combine(trait2.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		private static void OnAddStressVomiter(GameObject go)
		{
			Notification notification = new Notification(DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null);
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalVomiter", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_VOMITER.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_VOMITER.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022);
			Func<StatusItem> <>9__3;
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), delegate(ChoreProvider chore_provider)
			{
				ChoreType stressEmote = Db.Get().ChoreTypes.StressEmote;
				HashedString hashedString = "anim_interrupt_vomiter_kanim";
				HashedString[] array = new HashedString[] { "interrupt_vomiter" };
				KAnim.PlayMode playMode = KAnim.PlayMode.Once;
				Func<StatusItem> func;
				if ((func = <>9__3) == null)
				{
					func = (<>9__3 = () => tierOneBehaviourStatusItem);
				}
				return new StressEmoteChore(chore_provider, stressEmote, hashedString, array, playMode, func);
			}, (ChoreProvider chore_provider) => new VomitChore(Db.Get().ChoreTypes.StressVomit, chore_provider, Db.Get().DuplicantStatusItems.Vomiting, notification, null), "anim_loco_vomiter_kanim", 3f).StartSM();
		}

		private static void OnAddAggressive(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalAggresive", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_AGGRESIVE.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_AGGRESIVE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022);
			Func<StatusItem> <>9__2;
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), delegate(ChoreProvider chore_provider)
			{
				ChoreType stressEmote = Db.Get().ChoreTypes.StressEmote;
				HashedString hashedString = "anim_interrupt_destructive_kanim";
				HashedString[] array = new HashedString[] { "interrupt_destruct" };
				KAnim.PlayMode playMode = KAnim.PlayMode.Once;
				Func<StatusItem> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = () => tierOneBehaviourStatusItem);
				}
				return new StressEmoteChore(chore_provider, stressEmote, hashedString, array, playMode, func);
			}, (ChoreProvider chore_provider) => new AggressiveChore(chore_provider, null), "anim_loco_destructive_kanim", 3f).StartSM();
		}

		private static void OnAddUglyCrier(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalUglyCrier", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_UGLY_CRIER.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_UGLY_CRIER.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022);
			Func<StatusItem> <>9__2;
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), delegate(ChoreProvider chore_provider)
			{
				ChoreType stressEmote = Db.Get().ChoreTypes.StressEmote;
				HashedString hashedString = "anim_cry_kanim";
				HashedString[] array = new HashedString[] { "working_pre", "working_loop", "working_pst" };
				KAnim.PlayMode playMode = KAnim.PlayMode.Once;
				Func<StatusItem> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = () => tierOneBehaviourStatusItem);
				}
				return new StressEmoteChore(chore_provider, stressEmote, hashedString, array, playMode, func);
			}, (ChoreProvider chore_provider) => new UglyCryChore(Db.Get().ChoreTypes.UglyCry, chore_provider, null), "anim_loco_cry_kanim", 3f).StartSM();
		}

		private static void OnAddBingeEater(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalBingeEater", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_BINGE_EAT.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_BINGE_EAT.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022);
			Func<StatusItem> <>9__2;
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), delegate(ChoreProvider chore_provider)
			{
				ChoreType stressEmote = Db.Get().ChoreTypes.StressEmote;
				HashedString hashedString = "anim_interrupt_binge_eat_kanim";
				HashedString[] array = new HashedString[] { "interrupt_binge_eat" };
				KAnim.PlayMode playMode = KAnim.PlayMode.Once;
				Func<StatusItem> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = () => tierOneBehaviourStatusItem);
				}
				return new StressEmoteChore(chore_provider, stressEmote, hashedString, array, playMode, func);
			}, (ChoreProvider chore_provider) => new BingeEatChore(chore_provider, null), "anim_loco_binge_eat_kanim", 8f).StartSM();
		}

		private static void OnAddBalloonArtist(GameObject go)
		{
			new BalloonArtist.Instance(go.GetComponent<KMonoBehaviour>()).StartSM();
			new JoyBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), "anim_loco_happy_balloon_stickers_kanim", null, Db.Get().Expressions.Balloon).StartSM();
		}

		private static void OnAddSparkleStreaker(GameObject go)
		{
			new SparkleStreaker.Instance(go.GetComponent<KMonoBehaviour>()).StartSM();
			new JoyBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), "anim_loco_sparkle_kanim", null, Db.Get().Expressions.Sparkle).StartSM();
		}

		private static void OnAddStickerBomber(GameObject go)
		{
			new StickerBomber.Instance(go.GetComponent<KMonoBehaviour>()).StartSM();
			new JoyBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), "anim_loco_stickers", null, Db.Get().Expressions.Sticker).StartSM();
		}

		private static void OnAddSuperProductive(GameObject go)
		{
			new SuperProductive.Instance(go.GetComponent<KMonoBehaviour>()).StartSM();
			new JoyBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), "anim_loco_productive_kanim", "anim_loco_walk_productive_kanim", Db.Get().Expressions.Productive).StartSM();
		}

		public static float EARLYBIRD_MODIFIER = 2f;

		public static int EARLYBIRD_SCHEDULEBLOCK = 5;

		public static float NIGHTOWL_MODIFIER = 3f;

		public const float FLATULENCE_EMIT_MASS = 0.1f;

		public static float FLATULENCE_EMIT_INTERVAL_MIN = 10f;

		public static float FLATULENCE_EMIT_INTERVAL_MAX = 40f;

		public static float STINKY_EMIT_INTERVAL_MIN = 10f;

		public static float STINKY_EMIT_INTERVAL_MAX = 30f;

		public static float NARCOLEPSY_INTERVAL_MIN = 300f;

		public static float NARCOLEPSY_INTERVAL_MAX = 600f;

		public static float NARCOLEPSY_SLEEPDURATION_MIN = 15f;

		public static float NARCOLEPSY_SLEEPDURATION_MAX = 30f;

		public const float INTERRUPTED_SLEEP_STRESS_DELTA = 10f;

		public const float INTERRUPTED_SLEEP_ATHLETICS_DELTA = -2f;

		public static int NO_ATTRIBUTE_BONUS = 0;

		public static int GOOD_ATTRIBUTE_BONUS = 3;

		public static int GREAT_ATTRIBUTE_BONUS = 5;

		public static int BAD_ATTRIBUTE_PENALTY = -3;

		public static int HORRIBLE_ATTRIBUTE_PENALTY = -5;

		public static readonly List<global::System.Action> TRAIT_CREATORS = new List<global::System.Action>
		{
			TRAITS.CreateAttributeEffectTrait("None", DUPLICANTS.CONGENITALTRAITS.NONE.NAME, DUPLICANTS.CONGENITALTRAITS.NONE.DESC, "", (float)TRAITS.NO_ATTRIBUTE_BONUS, false, null),
			TRAITS.CreateComponentTrait<Stinky>("Stinky", DUPLICANTS.CONGENITALTRAITS.STINKY.NAME, DUPLICANTS.CONGENITALTRAITS.STINKY.DESC, false, null),
			TRAITS.CreateAttributeEffectTrait("Ellie", DUPLICANTS.CONGENITALTRAITS.ELLIE.NAME, DUPLICANTS.CONGENITALTRAITS.ELLIE.DESC, "AirConsumptionRate", -0.044999998f, "DecorExpectation", -5f, false),
			TRAITS.CreateDisabledTaskTrait("Joshua", DUPLICANTS.CONGENITALTRAITS.JOSHUA.NAME, DUPLICANTS.CONGENITALTRAITS.JOSHUA.DESC, "Combat", true),
			TRAITS.CreateComponentTrait<Stinky>("Liam", DUPLICANTS.CONGENITALTRAITS.LIAM.NAME, DUPLICANTS.CONGENITALTRAITS.LIAM.DESC, false, null),
			TRAITS.CreateDisabledTaskTrait("CantResearch", DUPLICANTS.TRAITS.CANTRESEARCH.NAME, DUPLICANTS.TRAITS.CANTRESEARCH.DESC, "Research", true),
			TRAITS.CreateDisabledTaskTrait("CantBuild", DUPLICANTS.TRAITS.CANTBUILD.NAME, DUPLICANTS.TRAITS.CANTBUILD.DESC, "Build", false),
			TRAITS.CreateDisabledTaskTrait("CantCook", DUPLICANTS.TRAITS.CANTCOOK.NAME, DUPLICANTS.TRAITS.CANTCOOK.DESC, "Cook", true),
			TRAITS.CreateDisabledTaskTrait("CantDig", DUPLICANTS.TRAITS.CANTDIG.NAME, DUPLICANTS.TRAITS.CANTDIG.DESC, "Dig", false),
			TRAITS.CreateDisabledTaskTrait("Hemophobia", DUPLICANTS.TRAITS.HEMOPHOBIA.NAME, DUPLICANTS.TRAITS.HEMOPHOBIA.DESC, "MedicalAid", true),
			TRAITS.CreateDisabledTaskTrait("ScaredyCat", DUPLICANTS.TRAITS.SCAREDYCAT.NAME, DUPLICANTS.TRAITS.SCAREDYCAT.DESC, "Combat", true),
			TRAITS.CreateNamedTrait("Allergies", DUPLICANTS.TRAITS.ALLERGIES.NAME, DUPLICANTS.TRAITS.ALLERGIES.DESC, false),
			TRAITS.CreateAttributeEffectTrait("MouthBreather", DUPLICANTS.TRAITS.MOUTHBREATHER.NAME, DUPLICANTS.TRAITS.MOUTHBREATHER.DESC, "AirConsumptionRate", 0.1f, false, null),
			TRAITS.CreateAttributeEffectTrait("CalorieBurner", DUPLICANTS.TRAITS.CALORIEBURNER.NAME, DUPLICANTS.TRAITS.CALORIEBURNER.DESC, "CaloriesDelta", -833.3333f, false, null),
			TRAITS.CreateAttributeEffectTrait("SmallBladder", DUPLICANTS.TRAITS.SMALLBLADDER.NAME, DUPLICANTS.TRAITS.SMALLBLADDER.DESC, "BladderDelta", 0.00027777778f, false, null),
			TRAITS.CreateAttributeEffectTrait("Anemic", DUPLICANTS.TRAITS.ANEMIC.NAME, DUPLICANTS.TRAITS.ANEMIC.DESC, "Athletics", (float)TRAITS.HORRIBLE_ATTRIBUTE_PENALTY, false, null),
			TRAITS.CreateAttributeEffectTrait("SlowLearner", DUPLICANTS.TRAITS.SLOWLEARNER.NAME, DUPLICANTS.TRAITS.SLOWLEARNER.DESC, "Learning", (float)TRAITS.BAD_ATTRIBUTE_PENALTY, false, null),
			TRAITS.CreateAttributeEffectTrait("NoodleArms", DUPLICANTS.TRAITS.NOODLEARMS.NAME, DUPLICANTS.TRAITS.NOODLEARMS.DESC, "Strength", (float)TRAITS.BAD_ATTRIBUTE_PENALTY, false, null),
			TRAITS.CreateAttributeEffectTrait("InteriorDecorator", DUPLICANTS.TRAITS.INTERIORDECORATOR.NAME, DUPLICANTS.TRAITS.INTERIORDECORATOR.DESC, "Art", (float)TRAITS.GOOD_ATTRIBUTE_BONUS, "DecorExpectation", -5f, true),
			TRAITS.CreateAttributeEffectTrait("SimpleTastes", DUPLICANTS.TRAITS.SIMPLETASTES.NAME, DUPLICANTS.TRAITS.SIMPLETASTES.DESC, "FoodExpectation", 1f, true, null),
			TRAITS.CreateAttributeEffectTrait("Foodie", DUPLICANTS.TRAITS.FOODIE.NAME, DUPLICANTS.TRAITS.FOODIE.DESC, "Cooking", (float)TRAITS.GOOD_ATTRIBUTE_BONUS, "FoodExpectation", -1f, true),
			TRAITS.CreateAttributeEffectTrait("Regeneration", DUPLICANTS.TRAITS.REGENERATION.NAME, DUPLICANTS.TRAITS.REGENERATION.DESC, "HitPointsDelta", 0.033333335f, false, null),
			TRAITS.CreateAttributeEffectTrait("DeeperDiversLungs", DUPLICANTS.TRAITS.DEEPERDIVERSLUNGS.NAME, DUPLICANTS.TRAITS.DEEPERDIVERSLUNGS.DESC, "AirConsumptionRate", -0.05f, false, null),
			TRAITS.CreateAttributeEffectTrait("SunnyDisposition", DUPLICANTS.TRAITS.SUNNYDISPOSITION.NAME, DUPLICANTS.TRAITS.SUNNYDISPOSITION.DESC, "StressDelta", -0.033333335f, false, delegate(GameObject go)
			{
				go.GetComponent<KBatchedAnimController>().AddAnimOverrides(Assets.GetAnim("anim_loco_happy_kanim"), 0f);
			}),
			TRAITS.CreateAttributeEffectTrait("RockCrusher", DUPLICANTS.TRAITS.ROCKCRUSHER.NAME, DUPLICANTS.TRAITS.ROCKCRUSHER.DESC, "Strength", 10f, false, null),
			TRAITS.CreateTrait("Uncultured", DUPLICANTS.TRAITS.UNCULTURED.NAME, DUPLICANTS.TRAITS.UNCULTURED.DESC, "DecorExpectation", 20f, new string[] { "Art" }, true),
			TRAITS.CreateNamedTrait("Archaeologist", DUPLICANTS.TRAITS.ARCHAEOLOGIST.NAME, DUPLICANTS.TRAITS.ARCHAEOLOGIST.DESC, false),
			TRAITS.CreateAttributeEffectTrait("WeakImmuneSystem", DUPLICANTS.TRAITS.WEAKIMMUNESYSTEM.NAME, DUPLICANTS.TRAITS.WEAKIMMUNESYSTEM.DESC, "GermResistance", -1f, false, null),
			TRAITS.CreateAttributeEffectTrait("IrritableBowel", DUPLICANTS.TRAITS.IRRITABLEBOWEL.NAME, DUPLICANTS.TRAITS.IRRITABLEBOWEL.DESC, "ToiletEfficiency", -0.5f, false, null),
			TRAITS.CreateComponentTrait<Flatulence>("Flatulence", DUPLICANTS.TRAITS.FLATULENCE.NAME, DUPLICANTS.TRAITS.FLATULENCE.DESC, false, null),
			TRAITS.CreateComponentTrait<Snorer>("Snorer", DUPLICANTS.TRAITS.SNORER.NAME, DUPLICANTS.TRAITS.SNORER.DESC, false, null),
			TRAITS.CreateComponentTrait<Narcolepsy>("Narcolepsy", DUPLICANTS.TRAITS.NARCOLEPSY.NAME, DUPLICANTS.TRAITS.NARCOLEPSY.DESC, false, null),
			TRAITS.CreateAttributeEffectTrait("Twinkletoes", DUPLICANTS.TRAITS.TWINKLETOES.NAME, DUPLICANTS.TRAITS.TWINKLETOES.DESC, "Athletics", (float)TRAITS.GOOD_ATTRIBUTE_BONUS, true, null),
			TRAITS.CreateAttributeEffectTrait("Greasemonkey", DUPLICANTS.TRAITS.GREASEMONKEY.NAME, DUPLICANTS.TRAITS.GREASEMONKEY.DESC, "Machinery", (float)TRAITS.GOOD_ATTRIBUTE_BONUS, true, null),
			TRAITS.CreateAttributeEffectTrait("MoleHands", DUPLICANTS.TRAITS.MOLEHANDS.NAME, DUPLICANTS.TRAITS.MOLEHANDS.DESC, "Digging", (float)TRAITS.GOOD_ATTRIBUTE_BONUS, true, null),
			TRAITS.CreateAttributeEffectTrait("FastLearner", DUPLICANTS.TRAITS.FASTLEARNER.NAME, DUPLICANTS.TRAITS.FASTLEARNER.DESC, "Learning", (float)TRAITS.GOOD_ATTRIBUTE_BONUS, true, null),
			TRAITS.CreateAttributeEffectTrait("DiversLung", DUPLICANTS.TRAITS.DIVERSLUNG.NAME, DUPLICANTS.TRAITS.DIVERSLUNG.DESC, "AirConsumptionRate", -0.025f, true, null),
			TRAITS.CreateAttributeEffectTrait("StrongArm", DUPLICANTS.TRAITS.STRONGARM.NAME, DUPLICANTS.TRAITS.STRONGARM.DESC, "Strength", (float)TRAITS.GOOD_ATTRIBUTE_BONUS, true, null),
			TRAITS.CreateNamedTrait("IronGut", DUPLICANTS.TRAITS.IRONGUT.NAME, DUPLICANTS.TRAITS.IRONGUT.DESC, true),
			TRAITS.CreateAttributeEffectTrait("StrongImmuneSystem", DUPLICANTS.TRAITS.STRONGIMMUNESYSTEM.NAME, DUPLICANTS.TRAITS.STRONGIMMUNESYSTEM.DESC, "GermResistance", 1f, true, null),
			TRAITS.CreateAttributeEffectTrait("BedsideManner", DUPLICANTS.TRAITS.BEDSIDEMANNER.NAME, DUPLICANTS.TRAITS.BEDSIDEMANNER.DESC, "Caring", (float)TRAITS.GOOD_ATTRIBUTE_BONUS, true, null),
			TRAITS.CreateTrait("Aggressive", DUPLICANTS.TRAITS.AGGRESSIVE.NAME, DUPLICANTS.TRAITS.AGGRESSIVE.DESC, new Action<GameObject>(TRAITS.OnAddAggressive), null, false, () => DUPLICANTS.TRAITS.AGGRESSIVE.NOREPAIR),
			TRAITS.CreateTrait("UglyCrier", DUPLICANTS.TRAITS.UGLYCRIER.NAME, DUPLICANTS.TRAITS.UGLYCRIER.DESC, new Action<GameObject>(TRAITS.OnAddUglyCrier), null, false, null),
			TRAITS.CreateTrait("BingeEater", DUPLICANTS.TRAITS.BINGEEATER.NAME, DUPLICANTS.TRAITS.BINGEEATER.DESC, new Action<GameObject>(TRAITS.OnAddBingeEater), null, false, null),
			TRAITS.CreateTrait("StressVomiter", DUPLICANTS.TRAITS.STRESSVOMITER.NAME, DUPLICANTS.TRAITS.STRESSVOMITER.DESC, new Action<GameObject>(TRAITS.OnAddStressVomiter), null, false, null),
			TRAITS.CreateTrait("BalloonArtist", DUPLICANTS.TRAITS.BALLOONARTIST.NAME, DUPLICANTS.TRAITS.BALLOONARTIST.DESC, new Action<GameObject>(TRAITS.OnAddBalloonArtist), null, false, null),
			TRAITS.CreateTrait("SparkleStreaker", DUPLICANTS.TRAITS.SPARKLESTREAKER.NAME, DUPLICANTS.TRAITS.SPARKLESTREAKER.DESC, new Action<GameObject>(TRAITS.OnAddSparkleStreaker), null, false, null),
			TRAITS.CreateTrait("StickerBomber", DUPLICANTS.TRAITS.STICKERBOMBER.NAME, DUPLICANTS.TRAITS.STICKERBOMBER.DESC, new Action<GameObject>(TRAITS.OnAddStickerBomber), null, false, null),
			TRAITS.CreateTrait("SuperProductive", DUPLICANTS.TRAITS.SUPERPRODUCTIVE.NAME, DUPLICANTS.TRAITS.SUPERPRODUCTIVE.DESC, new Action<GameObject>(TRAITS.OnAddSuperProductive), null, false, null),
			TRAITS.CreateComponentTrait<EarlyBird>("EarlyBird", DUPLICANTS.TRAITS.EARLYBIRD.NAME, DUPLICANTS.TRAITS.EARLYBIRD.DESC, true, () => string.Format(DUPLICANTS.TRAITS.EARLYBIRD.EXTENDED_DESC, GameUtil.AddPositiveSign(TRAITS.EARLYBIRD_MODIFIER.ToString(), true))),
			TRAITS.CreateComponentTrait<NightOwl>("NightOwl", DUPLICANTS.TRAITS.NIGHTOWL.NAME, DUPLICANTS.TRAITS.NIGHTOWL.DESC, true, () => string.Format(DUPLICANTS.TRAITS.NIGHTOWL.EXTENDED_DESC, GameUtil.AddPositiveSign(TRAITS.NIGHTOWL_MODIFIER.ToString(), true))),
			TRAITS.CreateComponentTrait<Claustrophobic>("Claustrophobic", DUPLICANTS.TRAITS.NEEDS.CLAUSTROPHOBIC.NAME, DUPLICANTS.TRAITS.NEEDS.CLAUSTROPHOBIC.DESC, false, null),
			TRAITS.CreateComponentTrait<PrefersWarmer>("PrefersWarmer", DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.NAME, DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.DESC, false, null),
			TRAITS.CreateComponentTrait<PrefersColder>("PrefersColder", DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.NAME, DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.DESC, false, null),
			TRAITS.CreateComponentTrait<SensitiveFeet>("SensitiveFeet", DUPLICANTS.TRAITS.NEEDS.SENSITIVEFEET.NAME, DUPLICANTS.TRAITS.NEEDS.SENSITIVEFEET.DESC, false, null),
			TRAITS.CreateComponentTrait<Fashionable>("Fashionable", DUPLICANTS.TRAITS.NEEDS.FASHIONABLE.NAME, DUPLICANTS.TRAITS.NEEDS.FASHIONABLE.DESC, false, null),
			TRAITS.CreateComponentTrait<Climacophobic>("Climacophobic", DUPLICANTS.TRAITS.NEEDS.CLIMACOPHOBIC.NAME, DUPLICANTS.TRAITS.NEEDS.CLIMACOPHOBIC.DESC, false, null),
			TRAITS.CreateComponentTrait<SolitarySleeper>("SolitarySleeper", DUPLICANTS.TRAITS.NEEDS.SOLITARYSLEEPER.NAME, DUPLICANTS.TRAITS.NEEDS.SOLITARYSLEEPER.DESC, false, null),
			TRAITS.CreateComponentTrait<Workaholic>("Workaholic", DUPLICANTS.TRAITS.NEEDS.WORKAHOLIC.NAME, DUPLICANTS.TRAITS.NEEDS.WORKAHOLIC.DESC, false, null)
		};

		public class JOY_REACTIONS
		{
			public static float MIN_MORALE_EXCESS = 8f;

			public static float MAX_MORALE_EXCESS = 20f;

			public static float MIN_REACTION_CHANCE = 2f;

			public static float MAX_REACTION_CHANCE = 5f;

			public static float JOY_REACTION_DURATION = 570f;

			public class SUPER_PRODUCTIVE
			{
				public static float INSTANT_SUCCESS_CHANCE = 10f;
			}

			public class BALLOON_ARTIST
			{
				public static float MINIMUM_BALLOON_MOVESPEED = 5f;

				public static int NUM_BALLOONS_TO_GIVE = 4;
			}

			public class STICKER_BOMBER
			{
				public static float TIME_PER_STICKER_BOMB = 150f;

				public static float STICKER_DURATION = 12000f;

				public static List<string> STICKER_ANIMS = new List<string>
				{
					"a", "b", "c", "d", "e", "f", "g", "h", "rocket", "paperplane",
					"plant", "plantpot", "mushroom", "mermaid", "spacepet", "spacepet2", "spacepet3", "spacepet4", "spacepet5", "unicorn"
				};
			}
		}
	}
}
