using System;
using STRINGS;
using UnityEngine;

public class CopyBuildingSettings : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = this.userMenu;
		string text = "action_mirror";
		string text2 = UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.NAME;
		global::System.Action action = new global::System.Action(this.ActivateCopyTool);
		global::Action action2 = global::Action.BuildingUtility1;
		string text3 = UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, action2, null, null, null, text3, true), 1f);
	}

	private void ActivateCopyTool()
	{
		CopySettingsTool.Instance.SetSourceObject(base.gameObject);
		PlayerController.Instance.ActivateTool(CopySettingsTool.Instance);
	}

	private void TriggerCopy()
	{
		if (this.leftHandle.IsValid)
		{
			this.leftHandle.ClearScheduler();
		}
		this.leftOffset = -1;
		this.leftHandle = UIScheduler.Instance.SchedulePeriodic("CopyLeft", 0.2f, new Action<object>(this.DoCopyLeft), null, null);
		this.DoCopyLeft(null);
		if (this.rightHandle.IsValid)
		{
			this.rightHandle.ClearScheduler();
		}
		this.rightOffset = 1;
		this.rightHandle = UIScheduler.Instance.SchedulePeriodic("CopyRight", 0.2f, new Action<object>(this.DoCopyRight), null, null);
		this.DoCopyRight(null);
	}

	private void DoCopyLeft(object obj)
	{
		int num = Grid.PosToCell(base.transform.position);
		if (this.ApplyCopy(num, this.leftOffset))
		{
			this.leftOffset--;
		}
		else
		{
			this.leftHandle.ClearScheduler();
		}
	}

	private void DoCopyRight(object obj)
	{
		int num = Grid.PosToCell(base.transform.position);
		if (this.ApplyCopy(num, this.rightOffset))
		{
			this.rightOffset++;
		}
		else
		{
			this.rightHandle.ClearScheduler();
		}
	}

	private bool ApplyCopy(int posCell, int offset)
	{
		int num = Grid.OffsetCell(posCell, offset, 0);
		GameObject gameObject = Grid.Objects[num, 1];
		bool flag;
		if (gameObject == null)
		{
			flag = false;
		}
		else
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (component == null)
			{
				flag = false;
			}
			else if (component.PrefabID() != this.id.PrefabID())
			{
				flag = false;
			}
			else
			{
				component.Trigger(-905833192, base.gameObject);
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.COPIED_SETTINGS, gameObject.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
				flag = true;
			}
		}
		return flag;
	}

	public static bool ApplyCopy(int targetCell, GameObject sourceGameObject)
	{
		GameObject gameObject = Grid.Objects[targetCell, 1];
		bool flag;
		if (gameObject == null)
		{
			flag = false;
		}
		else
		{
			KPrefabID component = sourceGameObject.GetComponent<KPrefabID>();
			if (component == null)
			{
				flag = false;
			}
			else
			{
				KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
				if (component2 == null)
				{
					flag = false;
				}
				else if (component2.PrefabID() != component.PrefabID())
				{
					flag = false;
				}
				else
				{
					component2.Trigger(-905833192, sourceGameObject);
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.COPIED_SETTINGS, gameObject.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
					flag = true;
				}
			}
		}
		return flag;
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	[MyCmpReq]
	private KPrefabID id;

	private SchedulerHandle leftHandle;

	private SchedulerHandle rightHandle;

	private int leftOffset;

	private int rightOffset;
}
