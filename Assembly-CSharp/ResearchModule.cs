using System;
using TUNING;
using UnityEngine;

public class ResearchModule : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		base.Subscribe<ResearchModule>(-1056989049, ResearchModule.OnLaunchDelegate);
		base.Subscribe<ResearchModule>(238242047, ResearchModule.OnLandDelegate);
	}

	public void OnLaunch(object data)
	{
	}

	public void OnLand(object data)
	{
		SpaceDestination.ResearchOpportunity researchOpportunity = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(base.GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>())).TryCompleteResearchOpportunity();
		if (researchOpportunity != null)
		{
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("ResearchDatabank"), base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore, null, 0);
			gameObject.SetActive(true);
			gameObject.GetComponent<PrimaryElement>().Mass = (float)researchOpportunity.dataValue;
			if (!string.IsNullOrEmpty(researchOpportunity.discoveredRareItem))
			{
				GameObject prefab = Assets.GetPrefab(researchOpportunity.discoveredRareItem);
				if (prefab == null)
				{
					KCrashReporter.Assert(false, "Missing prefab: " + researchOpportunity.discoveredRareItem);
				}
				else
				{
					GameUtil.KInstantiate(prefab, base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore, null, 0).SetActive(true);
				}
			}
		}
		GameObject gameObject2 = GameUtil.KInstantiate(Assets.GetPrefab("ResearchDatabank"), base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore, null, 0);
		gameObject2.SetActive(true);
		gameObject2.GetComponent<PrimaryElement>().Mass = (float)ROCKETRY.DESTINATION_RESEARCH.EVERGREEN;
	}

	private static readonly EventSystem.IntraObjectHandler<ResearchModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<ResearchModule>(delegate(ResearchModule component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ResearchModule> OnLandDelegate = new EventSystem.IntraObjectHandler<ResearchModule>(delegate(ResearchModule component, object data)
	{
		component.OnLand(data);
	});
}
