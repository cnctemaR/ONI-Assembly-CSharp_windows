using System;
using UnityEngine;

public class FishingLure : KMonoBehaviour
{
	public Vector3 reelInLocation
	{
		get
		{
			return this.station.transform.position;
		}
	}

	public Catchable HookedObject
	{
		get
		{
			return this.hookedObject;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.anim.Play("hook", KAnim.PlayMode.Loop, 1f, 0f);
		this.moveTarget = base.transform.position;
	}

	public void SetupLine(FishingStation fishingStation, KAnimFile animFile)
	{
		this.station = fishingStation;
		this.setup = true;
	}

	private void TryCatch(Catchable catchable)
	{
		if (global::UnityEngine.Random.Range(0f, 100f) < this.HookChance * Time.deltaTime)
		{
			this.station.ForceCatch(catchable);
			this.hookedObject = catchable;
		}
	}

	public void CheckForCatchableFish(BodyOfWater body)
	{
		if (body == null)
		{
			return;
		}
		if (!this.isBeingWorked())
		{
			return;
		}
		Catchable[] catchables = body.GetCatchables();
		foreach (Catchable catchable in catchables)
		{
			if (Grid.PosToCell(catchable.gameObject) == Grid.PosToCell(base.gameObject))
			{
				this.TryCatch(catchable);
			}
		}
		if (catchables.Length == 0 && this.station.GetWorker() != null)
		{
			this.station.EmptyWater();
		}
	}

	private void Splash()
	{
		BodyOfWater.MakeSplash(base.transform.position);
	}

	private void Update()
	{
		if (!this.setup)
		{
			return;
		}
		this.timeSinceBodyTransition += Time.deltaTime;
		if (this.body == null)
		{
			this.body = WaterBodyProbe.Instance.GetBodyIfKnown(Grid.PosToCell(base.gameObject));
			if (this.body == null)
			{
				this.body = WaterBodyProbe.Instance.GetBodyIfKnown(Grid.CellBelow(Grid.PosToCell(base.gameObject)));
			}
			if (this.body != null)
			{
				if (this.timeSinceBodyTransition > 0.25f)
				{
					this.Splash();
				}
				this.timeSinceBodyTransition = 0f;
				if (!this.body.containedObjects.Contains(base.gameObject))
				{
					this.body.AddObjectToBody(base.gameObject);
				}
			}
		}
		else if (!this.body.waterCells.Contains(Grid.PosToCell(base.transform.position)))
		{
			this.body.RemoveObjectFromBody(base.gameObject);
			if (this.timeSinceBodyTransition > 0.25f)
			{
				this.Splash();
			}
			this.timeSinceBodyTransition = 0f;
			this.body = null;
		}
		if (this.body != null && this.hookedObject == null && this.station != null)
		{
			this.CheckForCatchableFish(this.body);
		}
		float num = Vector3.Distance(this.moveTarget, base.transform.position);
		base.transform.position += Vector3.Normalize(this.moveTarget - base.transform.position) * Mathf.Clamp(Time.deltaTime * this.moveSpeed, 0f, num);
		if (this.hookedObject != null)
		{
			this.hookedObject.transform.SetPosition(base.transform.position);
		}
		if (this.hookedObject != null && Vector3.Distance(base.transform.position, this.station.transform.position) < 1.5f)
		{
			this.moveTarget = base.transform.position;
			this.station.RemoveFromHook(this.hookedObject.gameObject);
			this.hookedObject = null;
		}
	}

	public void ReelIn()
	{
		this.moveTarget = this.reelInLocation;
	}

	public bool isBeingWorked()
	{
		return this.station && this.station.GetWorker() != null;
	}

	public FishingStation station;

	public BodyOfWater body;

	[MyCmpAdd]
	private KBatchedAnimController anim;

	private float moveSpeed = 2f;

	private float HookChance = 10f;

	private bool setup;

	private Catchable hookedObject;

	public Vector3 moveTarget;

	private Vector3 smallMoveTarget;

	private float timeSinceBodyTransition = 1f;
}
