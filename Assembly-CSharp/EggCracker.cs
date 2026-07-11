using System;
using UnityEngine;

public class EggCracker : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RefineryWorkable refineryWorkable = this.workable;
		refineryWorkable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(refineryWorkable.OnWorkableEventCB, new Action<Workable.WorkableEvent>(this.OnWorkableEvent));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		global::UnityEngine.Object.Destroy(this.tracker);
		this.tracker = null;
	}

	private void OnWorkableEvent(Workable.WorkableEvent e)
	{
		if (e == Workable.WorkableEvent.WorkStarted)
		{
			Refinery.MachineOrder currentMachineOrder = this.refinery.CurrentMachineOrder;
			if (currentMachineOrder != null)
			{
				ComplexRecipe.RecipeElement[] ingredients = currentMachineOrder.parentOrder.recipe.ingredients;
				if (ingredients.Length > 0)
				{
					ComplexRecipe.RecipeElement recipeElement = ingredients[0];
					this.display_egg = this.refinery.buildStorage.FindFirst(recipeElement.material);
					this.PositionActiveEgg();
				}
			}
		}
		else if (e == Workable.WorkableEvent.WorkCompleted)
		{
			if (this.display_egg)
			{
				KBatchedAnimController component = this.display_egg.GetComponent<KBatchedAnimController>();
				component.Play("hatching_pst", KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		else if (e == Workable.WorkableEvent.WorkStopped)
		{
			global::UnityEngine.Object.Destroy(this.tracker);
			this.tracker = null;
			this.display_egg = null;
		}
	}

	private void PositionActiveEgg()
	{
		if (!this.display_egg)
		{
			return;
		}
		KBatchedAnimController component = this.display_egg.GetComponent<KBatchedAnimController>();
		component.enabled = true;
		component.SetSceneLayer(Grid.SceneLayer.BuildingUse);
		KSelectable component2 = this.display_egg.GetComponent<KSelectable>();
		if (component2 != null)
		{
			component2.enabled = true;
		}
		this.tracker = this.display_egg.AddComponent<KBatchedAnimTracker>();
		this.tracker.symbol = "snapto_egg";
	}

	[MyCmpReq]
	private Refinery refinery;

	[MyCmpReq]
	private RefineryWorkable workable;

	private KBatchedAnimTracker tracker;

	private GameObject display_egg;
}
