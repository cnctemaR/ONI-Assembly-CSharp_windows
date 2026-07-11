using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Placeable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<Placeable>(493375141, Placeable.OnRefreshUserMenuDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.prefabId.AddTag(new Tag(this.prefabId.InstanceID.ToString()), false);
		if (this.targetCell != -1)
		{
			this.QueuePlacement(this.targetCell);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.preview != null)
		{
			this.preview.DeleteObject();
		}
		base.OnCleanUp();
	}

	public void QueuePlacement(int target)
	{
		this.targetCell = target;
		Vector3 vector = Grid.CellToPosCBC(this.targetCell, Grid.SceneLayer.Front);
		if (this.preview == null)
		{
			this.preview = GameUtil.KInstantiate(Assets.GetPrefab(this.previewTag), vector, Grid.SceneLayer.Front, null, 0);
			this.preview.SetActive(true);
		}
		else
		{
			this.preview.transform.SetPosition(vector);
		}
		if (this.chore != null)
		{
			this.chore.Cancel("new target");
		}
		this.chore = new FetchChore(Db.Get().ChoreTypes.Fetch, this.preview.GetComponent<Storage>(), 1f, new Tag[]
		{
			new Tag(this.prefabId.InstanceID.ToString())
		}, null, null, null, true, new Action<Chore>(this.OnChoreComplete), null, null, FetchOrder2.OperationalRequirement.None, 0);
	}

	private void OnChoreComplete(Chore completed_chore)
	{
		this.Place(this.targetCell);
	}

	public void Place(int target)
	{
		Vector3 vector = Grid.CellToPosCBC(target, Grid.SceneLayer.Front);
		GameUtil.KInstantiate(Assets.GetPrefab(this.spawnOnPlaceTag), vector, Grid.SceneLayer.Front, null, 0).SetActive(true);
		this.DeleteObject();
	}

	private void OpenPlaceTool()
	{
		PlaceTool.Instance.Activate(this, this.previewTag);
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo = ((this.targetCell == -1) ? new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.RELOCATE.NAME, new global::System.Action(this.OpenPlaceTool), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.RELOCATE.TOOLTIP, true) : new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.RELOCATE.NAME_OFF, new global::System.Action(this.CancelRelocation), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.RELOCATE.TOOLTIP_OFF, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	private void CancelRelocation()
	{
		if (this.preview != null)
		{
			this.preview.DeleteObject();
			this.preview = null;
		}
		this.targetCell = -1;
	}

	[MyCmpReq]
	private KPrefabID prefabId;

	[Serialize]
	private int targetCell = -1;

	public Tag previewTag;

	public Tag spawnOnPlaceTag;

	private GameObject preview;

	private FetchChore chore;

	private static readonly EventSystem.IntraObjectHandler<Placeable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Placeable>(delegate(Placeable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
