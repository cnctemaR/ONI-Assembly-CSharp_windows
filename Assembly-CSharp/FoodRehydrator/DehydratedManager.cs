using System;
using Klei;
using UnityEngine;

namespace FoodRehydrator
{
	public class DehydratedManager : KMonoBehaviour
	{
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
			foreach (GameObject gameObject in this.packages.items)
			{
				this.PackagedFoodAdded(gameObject);
			}
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
			food.GetComponent<PrimaryElement>().AddDisease(diseaseInfo.idx, diseaseInfo.count, "rehydrating");
		}

		private void PackagedFoodAdded(GameObject package)
		{
			DehydratedFoodPackage component = package.GetComponent<DehydratedFoodPackage>();
			DebugUtil.DevAssert(component != null, "PackagedFoodAdded missing component", null);
			if (component != null)
			{
				component.StoredInRehydrator(base.gameObject);
			}
		}

		private void PackagedFoodRemoved(GameObject package)
		{
			DehydratedFoodPackage component = package.GetComponent<DehydratedFoodPackage>();
			global::Debug.Assert(component != null);
			if (component != null)
			{
				component.RemovedFromRehydrator();
			}
		}

		private void StorageChangeHandler(object obj)
		{
			GameObject gameObject = (GameObject)obj;
			if (gameObject.GetComponent<DehydratedFoodPackage>() != null)
			{
				if (this.packages.items.Contains(gameObject))
				{
					this.PackagedFoodAdded(gameObject);
				}
				else
				{
					this.PackagedFoodRemoved(gameObject);
				}
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

		private Storage packages;

		private Storage water;

		private MeterController packagesMeter;

		private static string HASH_FOOD = "food";

		private KBatchedAnimController foodKBAC;
	}
}
