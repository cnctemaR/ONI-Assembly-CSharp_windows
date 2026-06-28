using System;
using STRINGS;
using UnityEngine;

public class ScheduleScreen : KScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.schedule = ScheduleManager.Instance.GetSchedule();
		this.entries = new ScheduleScreenColumnEntry[this.schedule.GetBlocks().Length, Db.Get().ScheduleBlockTypes.Count];
		ScheduleScreenToggle component = global::UnityEngine.Object.Instantiate<ScheduleScreenToggle>(this.togglePrefab).GetComponent<ScheduleScreenToggle>();
		component.GetComponent<RectTransform>().SetParent(this.togglesRoot);
		component.toggle.onClick += delegate
		{
			this.isTogglingOn = true;
			this.isToggleSet = true;
		};
		component.text.text = UI.SCHEDULESCREEN.ALLOWED;
		ScheduleScreenToggle component2 = global::UnityEngine.Object.Instantiate<ScheduleScreenToggle>(this.togglePrefab).GetComponent<ScheduleScreenToggle>();
		component2.GetComponent<RectTransform>().SetParent(this.togglesRoot);
		component2.toggle.onClick += delegate
		{
			this.isTogglingOn = false;
			this.isToggleSet = true;
		};
		component2.text.text = UI.SCHEDULESCREEN.DENIED;
		for (int i = 0; i < Db.Get().ScheduleBlockTypes.Count; i++)
		{
			ScheduleBlockType block_type = Db.Get().ScheduleBlockTypes[i];
			RectTransform component3 = global::UnityEngine.Object.Instantiate<ScheduleScreenColumn>(this.columnPrefab).GetComponent<RectTransform>();
			component3.SetParent(this.scheduleRoot);
			component3.GetComponent<ScheduleScreenColumn>().header.text = block_type.Name;
			for (int j = 0; j < this.schedule.GetBlocks().Length; j++)
			{
				RectTransform component4 = global::UnityEngine.Object.Instantiate<ScheduleScreenColumnEntry>(this.columnEntryPrefab).GetComponent<RectTransform>();
				component4.SetParent(component3);
				this.entries[j, i] = component4.GetComponent<ScheduleScreenColumnEntry>();
				int i_iter = i;
				int j_iter = j;
				ScheduleScreenColumnEntry scheduleScreenColumnEntry = this.entries[j, i];
				scheduleScreenColumnEntry.onLeftClick = (global::System.Action)Delegate.Combine(scheduleScreenColumnEntry.onLeftClick, new global::System.Action(delegate
				{
					if (this.isToggleSet)
					{
						this.SetColumnEntry(j_iter, i_iter, block_type, this.isTogglingOn);
					}
				}));
				this.SetColumnEntry(j, i, block_type, this.schedule.GetBlocks()[j].Contains(block_type));
			}
		}
		this.columnPrefab.gameObject.SetActive(false);
		this.columnEntryPrefab.gameObject.SetActive(false);
		this.togglePrefab.gameObject.SetActive(false);
		this.bodyText.text = UI.SCHEDULESCREEN.SELECTHELP;
	}

	private void SetColumnEntry(int i, int j, ScheduleBlockType block_type, bool is_allowed)
	{
		if (is_allowed)
		{
			this.entries[i, j].image.color = this.allowedColor;
			this.schedule.Add(i, block_type);
		}
		else
		{
			this.entries[i, j].image.color = this.deniedColor;
			this.schedule.Remove(i, block_type);
		}
	}

	[SerializeField]
	private RectTransform scheduleRoot;

	[SerializeField]
	private RectTransform togglesRoot;

	[SerializeField]
	private ScheduleScreenToggle togglePrefab;

	[SerializeField]
	private ScheduleScreenColumn columnPrefab;

	[SerializeField]
	private ScheduleScreenColumnEntry columnEntryPrefab;

	[SerializeField]
	private LocText bodyText;

	[SerializeField]
	private Color allowedColor;

	[SerializeField]
	private Color deniedColor;

	private Schedule schedule;

	private ScheduleScreenColumnEntry[,] entries;

	private bool isTogglingOn;

	private bool isToggleSet;
}
