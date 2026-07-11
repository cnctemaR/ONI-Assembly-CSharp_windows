using System;
using Klei.AI;
using UnityEngine;

public class BalloonStandConfig : IEntityConfig
{
	public BalloonStandConfig()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "HasNoBalloon";
		precondition.description = "Duplicant doesn't have a balloon already";
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumerState.consumer == null) && !context.consumerState.gameObject.GetComponent<Effects>().HasEffect("HasBalloon");
		};
		this.HasNoBalloon = precondition;
		base..ctor();
	}

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
		ChoreType joyReaction = Db.Get().ChoreTypes.JoyReaction;
		GetBalloonWorkable getBalloonWorkable = component;
		Action<Chore> action = new Action<Chore>(this.MakeNewBalloonChore);
		ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
		WorkChore<GetBalloonWorkable> workChore = new WorkChore<GetBalloonWorkable>(joyReaction, getBalloonWorkable, null, true, action, null, null, true, recreation, false, true, null, false, true, true, PriorityScreen.PriorityClass.high, 5, true, true);
		workChore.AddPrecondition(this.HasNoBalloon, workChore);
	}

	private void MakeNewBalloonChore(Chore chore)
	{
		GetBalloonWorkable component = chore.target.GetComponent<GetBalloonWorkable>();
		ChoreType joyReaction = Db.Get().ChoreTypes.JoyReaction;
		GetBalloonWorkable getBalloonWorkable = component;
		Action<Chore> action = new Action<Chore>(this.MakeNewBalloonChore);
		ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
		WorkChore<GetBalloonWorkable> workChore = new WorkChore<GetBalloonWorkable>(joyReaction, getBalloonWorkable, null, true, action, null, null, true, recreation, false, true, null, false, true, true, PriorityScreen.PriorityClass.high, 5, true, true);
		workChore.AddPrecondition(this.HasNoBalloon, workChore);
	}

	public static readonly string ID = "BalloonStand";

	private Chore.Precondition HasNoBalloon;
}
