using System;
using TUNING;
using UnityEngine;

public class EggCracker : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.refinery.choreType = Db.Get().ChoreTypes.Cook;
		this.refinery.fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		this.workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
		this.workable.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		this.workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
		this.workable.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		ComplexFabricatorWorkable complexFabricatorWorkable = this.workable;
		complexFabricatorWorkable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(complexFabricatorWorkable.OnWorkableEventCB, new Action<Workable.WorkableEvent>(this.OnWorkableEvent));
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
			ComplexRecipe currentWorkingOrder = this.refinery.CurrentWorkingOrder;
			if (currentWorkingOrder != null)
			{
				ComplexRecipe.RecipeElement[] ingredients = currentWorkingOrder.ingredients;
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
	private ComplexFabricator refinery;

	[MyCmpReq]
	private ComplexFabricatorWorkable workable;

	private KBatchedAnimTracker tracker;

	private GameObject display_egg;
}
