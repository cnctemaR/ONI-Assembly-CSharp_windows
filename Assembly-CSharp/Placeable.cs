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
		this.prefabId.AddTag(new Tag(this.prefabId.InstanceID.ToString()));
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
		}, null, null, null, true, new Action<Chore>(this.OnChoreComplete), null, null, FetchOrder2.OperationalRequirement.None, 0, null);
	}

	private void OnChoreComplete(Chore completed_chore)
	{
		this.Place(this.targetCell);
	}

	public void Place(int target)
	{
		Vector3 vector = Grid.CellToPosCBC(target, Grid.SceneLayer.Front);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(this.spawnOnPlaceTag), vector, Grid.SceneLayer.Front, null, 0);
		gameObject.SetActive(true);
		this.DeleteObject();
	}

	private void OpenPlaceTool()
	{
		PlaceTool.Instance.Activate(this, this.previewTag);
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (this.targetCell == -1)
		{
			string text = "action_deconstruct";
			string text2 = UI.USERMENUACTIONS.RELOCATE.NAME;
			global::System.Action action = new global::System.Action(this.OpenPlaceTool);
			string text3 = UI.USERMENUACTIONS.RELOCATE.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_deconstruct";
			string text2 = UI.USERMENUACTIONS.RELOCATE.NAME_OFF;
			global::System.Action action = new global::System.Action(this.CancelRelocation);
			string text = UI.USERMENUACTIONS.RELOCATE.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
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
