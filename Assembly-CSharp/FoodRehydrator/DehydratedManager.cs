using System;
using System.Collections.Generic;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace FoodRehydrator
{
	public class DehydratedManager : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			base.Subscribe<DehydratedManager>(-905833192, DehydratedManager.OnCopySettingsDelegate);
		}

		public Tag ChosenContent
		{
			get
			{
				return this.chosenContent;
			}
			set
			{
				if (this.chosenContent != value)
				{
					base.GetComponent<ManualDeliveryKG>().RequestedItemTag = value;
					this.chosenContent = value;
					this.packages.DropUnlessHasTag(this.chosenContent);
					if (this.chosenContent != GameTags.Dehydrated)
					{
						AccessabilityManager component = base.GetComponent<AccessabilityManager>();
						if (component != null)
						{
							component.CancelActiveWorkable();
						}
					}
				}
			}
		}

		public void SetFabricatedFoodSymbol(Tag material)
		{
			this.foodKBAC.gameObject.SetActive(true);
			GameObject prefab = Assets.GetPrefab(material);
			this.foodKBAC.SwapAnims(prefab.GetComponent<KBatchedAnimController>().AnimFiles);
			this.foodKBAC.Play("object", KAnim.PlayMode.Loop, 1f, 0f);
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();
			Storage[] components = base.GetComponents<Storage>();
			global::Debug.Assert(components.Length == 2);
			this.packages = components[0];
			this.water = components[1];
			this.packagesMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, new string[] { "meter_target" });
			base.Subscribe(-1697596308, new Action<object>(this.StorageChangeHandler));
			this.SetupFoodSymbol();
			this.packagesMeter.SetPositionPercent((float)this.packages.items.Count / 5f);
		}

		public void ConsumeResourcesForRehydration(GameObject package, GameObject food)
		{
			global::Debug.Assert(this.packages.items.Contains(package));
			this.packages.ConsumeIgnoringDisease(package);
			float num;
			SimUtil.DiseaseInfo diseaseInfo;
			float num2;
			this.water.ConsumeAndGetDisease(FoodRehydratorConfig.REHYDRATION_TAG, 1f, out num, out diseaseInfo, out num2);
			PrimaryElement component = food.GetComponent<PrimaryElement>();
			if (component != null)
			{
				component.AddDisease(diseaseInfo.idx, diseaseInfo.count, "rehydrating");
				component.SetMassTemperature(component.Mass, component.Temperature * 0.125f + num2 * 0.875f);
			}
		}

		private void StorageChangeHandler(object obj)
		{
			if (((GameObject)obj).GetComponent<DehydratedFoodPackage>() != null)
			{
				this.packagesMeter.SetPositionPercent((float)this.packages.items.Count / 5f);
			}
		}

		private void SetupFoodSymbol()
		{
			GameObject gameObject = Util.NewGameObject(base.gameObject, "food_symbol");
			gameObject.SetActive(false);
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			bool flag;
			Vector3 vector = component.GetSymbolTransform(DehydratedManager.HASH_FOOD, out flag).GetColumn(3);
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse);
			gameObject.transform.SetPosition(vector);
			this.foodKBAC = gameObject.AddComponent<KBatchedAnimController>();
			this.foodKBAC.AnimFiles = new KAnimFile[] { Assets.GetAnim("mushbar_kanim") };
			this.foodKBAC.initialAnim = "object";
			component.SetSymbolVisiblity(DehydratedManager.HASH_FOOD, false);
			this.foodKBAC.sceneLayer = Grid.SceneLayer.BuildingUse;
			KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
			kbatchedAnimTracker.symbol = new HashedString("food");
			kbatchedAnimTracker.offset = Vector3.zero;
		}

		public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions()
		{
			HashSet<Tag> discoveredResourcesFromTag = DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(GameTags.Dehydrated);
			FewOptionSideScreen.IFewOptionSideScreen.Option[] array = new FewOptionSideScreen.IFewOptionSideScreen.Option[1 + discoveredResourcesFromTag.Count];
			array[0] = new FewOptionSideScreen.IFewOptionSideScreen.Option(GameTags.Dehydrated, UI.UISIDESCREENS.FILTERSIDESCREEN.DRIEDFOOD, Def.GetUISprite("icon_category_food", "ui", false), "");
			int num = 1;
			foreach (Tag tag in discoveredResourcesFromTag)
			{
				array[num] = new FewOptionSideScreen.IFewOptionSideScreen.Option(tag, tag.ProperName(), Def.GetUISprite(tag, "ui", false), "");
				num++;
			}
			return array;
		}

		public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
		{
			this.ChosenContent = option.tag;
		}

		public Tag GetSelectedOption()
		{
			return this.chosenContent;
		}

		protected void OnCopySettings(object data)
		{
			GameObject gameObject = data as GameObject;
			if (gameObject != null)
			{
				DehydratedManager component = gameObject.GetComponent<DehydratedManager>();
				if (component != null)
				{
					this.ChosenContent = component.ChosenContent;
				}
			}
		}

		[MyCmpAdd]
		private CopyBuildingSettings copyBuildingSettings;

		private Storage packages;

		private Storage water;

		private MeterController packagesMeter;

		private static string HASH_FOOD = "food";

		private KBatchedAnimController foodKBAC;

		private static readonly EventSystem.IntraObjectHandler<DehydratedManager> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<DehydratedManager>(delegate(DehydratedManager component, object data)
		{
			component.OnCopySettings(data);
		});

		[Serialize]
		private Tag chosenContent = GameTags.Dehydrated;
	}
}
