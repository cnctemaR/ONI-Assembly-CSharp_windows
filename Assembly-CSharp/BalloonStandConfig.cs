using System;
using Klei.AI;
using UnityEngine;

public class BalloonStandConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(BalloonStandConfig.ID, BalloonStandConfig.ID, false);
		KAnimFile[] array = new KAnimFile[] { Assets.GetAnim("anim_interacts_balloon_receiver_kanim") };
		GetBalloonWorkable getBalloonWorkable = gameObject.AddOrGet<GetBalloonWorkable>();
		getBalloonWorkable.workTime = 2f;
		getBalloonWorkable.workLayer = Grid.SceneLayer.BuildingFront;
		getBalloonWorkable.overrideAnims = array;
		getBalloonWorkable.synchronizeAnims = false;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		GetBalloonWorkable component = inst.GetComponent<GetBalloonWorkable>();
		WorkChore<GetBalloonWorkable> workChore = new WorkChore<GetBalloonWorkable>(Db.Get().ChoreTypes.JoyReaction, component, null, true, new Action<Chore>(this.MakeNewBalloonChore), null, null, true, Db.Get().ScheduleBlockTypes.Recreation, false, true, null, false, true, true, PriorityScreen.PriorityClass.high, 5, true, true);
		workChore.AddPrecondition(BalloonStandConfig.HasNoBalloon, workChore);
		workChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, null);
		component.GetBalloonArtist().NextBalloonOverride();
	}

	private void MakeNewBalloonChore(Chore chore)
	{
		GetBalloonWorkable component = chore.target.GetComponent<GetBalloonWorkable>();
		WorkChore<GetBalloonWorkable> workChore = new WorkChore<GetBalloonWorkable>(Db.Get().ChoreTypes.JoyReaction, component, null, true, new Action<Chore>(this.MakeNewBalloonChore), null, null, true, Db.Get().ScheduleBlockTypes.Recreation, false, true, null, false, true, true, PriorityScreen.PriorityClass.high, 5, true, true);
		workChore.AddPrecondition(BalloonStandConfig.HasNoBalloon, workChore);
		workChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, null);
		component.GetBalloonArtist().NextBalloonOverride();
	}

	public static readonly string ID = "BalloonStand";

	private static Chore.Precondition HasNoBalloon = new Chore.Precondition
	{
		id = "HasNoBalloon",
		description = "__ Duplicant doesn't have a balloon already",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumerState.consumer == null) && !context.consumerState.gameObject.GetComponent<Effects>().HasEffect("HasBalloon");
		}
	};
}
