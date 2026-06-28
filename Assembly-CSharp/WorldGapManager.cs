using System;
using System.Collections.Generic;
using Generated;
using Klei;
using STRINGS;
using UnityEngine;

public class WorldGapManager : KMonoBehaviour
{
	[SerializeField]
	public WorldGaps gaps { get; private set; }

	public static WorldGapManager Instance { get; private set; }

	public void SetVoidCells(List<int> voidCells)
	{
		if (!this.loaded)
		{
			this.gaps.voidCells.UnionWith(voidCells);
			this.cellUpdateOrder = new List<int>(voidCells);
			this.cellUpdateOrder.Shuffle<int>();
			this.currentCell = this.cellUpdateOrder.GetEnumerator();
		}
	}

	public void OnDeserialized()
	{
		this.loaded = true;
		this.cellUpdateOrder = new List<int>(this.gaps.voidCells);
		this.cellUpdateOrder.Shuffle<int>();
		this.currentCell = this.cellUpdateOrder.GetEnumerator();
	}

	protected override void OnPrefabInit()
	{
		this.warning = new Notification(MISC.NOTIFICATIONS.GASCLOUDWARNING.NAME, NotificationType.BadMinor, HashedString.Invalid, null, null, true, 0f, null, null, null);
		this.rampUp = new Notification(MISC.NOTIFICATIONS.GASCLOUDARRIVING.NAME, NotificationType.BadMinor, HashedString.Invalid, null, null, true, 0f, null, null, null);
		this.peak = new Notification(MISC.NOTIFICATIONS.GASCLOUDPEAK.NAME, NotificationType.BadMinor, HashedString.Invalid, null, null, true, 0f, null, null, null);
		this.rampDown = new Notification(MISC.NOTIFICATIONS.GASCLOUDDEPARTING.NAME, NotificationType.Good, HashedString.Invalid, null, null, true, 0f, null, null, null);
		this.over = new Notification(MISC.NOTIFICATIONS.GASCLOUDGONE.NAME, NotificationType.Good, HashedString.Invalid, null, null, true, 0f, null, null, null);
		WorldGapManager.Instance = this;
		if (this.gaps == null)
		{
			this.gaps = new WorldGaps();
			this.gaps.currentTime = 600f * (float)int.Parse(WorldGen.Settings.defaults.data["CloudDaysUntilFirst"] as string);
		}
		this.gaps.cyclePeriod = 600f * (float)int.Parse(WorldGen.Settings.defaults.data["CloudDaysBetween"] as string);
		this.gaps.notifyTimeApproaching = 600f * (float)int.Parse(WorldGen.Settings.defaults.data["CloudDaysForNotifyApproaching"] as string);
		this.cellUpdateOrder = new List<int>(this.gaps.voidCells);
		this.cellUpdateOrder.Shuffle<int>();
		this.currentCell = this.cellUpdateOrder.GetEnumerator();
	}

	public void SetGasClouds(List<Cloud> clouds)
	{
		this.gaps.SetGasClouds(clouds);
		if (this.gaps.state != WorldGaps.State.Disabled)
		{
			if (this.gaps.currentCloudIndex == -1)
			{
				this.gaps.Next();
			}
			this.currentElement = ElementLoader.FindElementByHash(this.gaps.currentCloud.element);
		}
	}

	private void SetValue(float percent)
	{
		this.currentPercent = percent;
		if (percent <= 0f)
		{
			this.SetElement(SimHashes.Void, 0f, 0f);
			return;
		}
		float num = this.gaps.currentCloud.externalMassKg * percent;
		this.currentMass = num;
		SimHashes element = this.gaps.currentCloud.element;
		if (this.currentElement == null || this.currentElement.id != this.gaps.currentCloud.element)
		{
			this.currentElement = ElementLoader.FindElementByHash(this.gaps.currentCloud.element);
		}
		float num2 = this.currentElement.defaultValues.temperature + (this.gaps.currentCloud.externalTemperatureK - this.currentElement.defaultValues.temperature) * percent;
		this.currentTemperature = num2;
		this.SetElement(element, num, num2);
	}

	private void SetElement(SimHashes element, float mass, float temperature)
	{
		this.cellsLastTick = 0;
		this.resetsLastTick = 0;
		int num = 0;
		while (num < this.cellsPerTick && num < this.cellUpdateOrder.Count)
		{
			if (!this.currentCell.MoveNext())
			{
				this.currentCell = this.cellUpdateOrder.GetEnumerator();
			}
			this.cellsLastTick++;
			SimMessages.ReplaceElement(this.currentCell.Current, element, CellEventLogger.Instance.WorldGapManager, mass, temperature, -1);
			num++;
		}
	}

	private void Reset()
	{
		this.numCompletes = 0;
		this.resetCells = 0;
	}

	private void Complete()
	{
		int num = 0;
		while (this.resetCells < this.cellUpdateOrder.Count && num < this.cellsPerTick)
		{
			SimMessages.ReplaceElement(this.cellUpdateOrder[this.resetCells], SimHashes.Void, CellEventLogger.Instance.WorldGapManager, 0f, -1f, -1);
			this.resetCells++;
			num++;
		}
		if (num != 0)
		{
			this.numCompletes++;
		}
	}

	private bool GapsVisible()
	{
		HashSet<int>.Enumerator enumerator = this.gaps.voidCells.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (Grid.Visible[enumerator.Current] == 255)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsVoid(int cell)
	{
		return this.ready && this.gaps.voidCells.Contains(cell);
	}

	private void SimUpdate(float dt)
	{
		this.gapCellCount = this.gaps.voidCells.Count;
		this.updateCellCount = this.cellUpdateOrder.Count;
		if (!this.ready)
		{
			return;
		}
		if (this.gaps.state == WorldGaps.State.Disabled)
		{
			this.gaps.currentTime = this.gaps.cyclePeriod;
			return;
		}
		if (this.notificationDelayForInvisible && this.GapsVisible())
		{
			this.notifier.Add(this.delayedNotification, string.Empty);
			this.notificationDelayForInvisible = false;
		}
		this.gaps.currentTime -= dt;
		switch (this.gaps.state)
		{
		case WorldGaps.State.EnabledRampUp:
			if (this.gaps.currentTime < 0f)
			{
				this.gaps.state = WorldGaps.State.EnabledFull;
				this.gaps.currentTime = this.gaps.currentCloud.onLength;
				this.notifier.Remove(this.rampUp);
				if (this.GapsVisible())
				{
					this.notifier.Add(this.peak, string.Empty);
					this.delayedNotification = null;
					this.notificationDelayForInvisible = false;
				}
				else
				{
					this.notificationDelayForInvisible = true;
					this.delayedNotification = this.peak;
				}
			}
			else
			{
				this.SetValue(1f - this.gaps.currentTime / this.gaps.currentCloud.rampTime);
			}
			break;
		case WorldGaps.State.EnabledFull:
			if (this.gaps.currentTime < 0f)
			{
				this.gaps.state = WorldGaps.State.EnabledRampDown;
				this.gaps.currentTime = this.gaps.currentCloud.rampTime;
				this.notifier.Remove(this.peak);
				if (this.GapsVisible())
				{
					this.delayedNotification = null;
					this.notificationDelayForInvisible = false;
					this.notifier.Add(this.rampDown, string.Empty);
				}
				else
				{
					this.delayedNotification = this.rampDown;
					this.notificationDelayForInvisible = true;
				}
			}
			else
			{
				this.SetValue(1f);
			}
			break;
		case WorldGaps.State.EnabledRampDown:
			if (this.gaps.currentTime < 0f)
			{
				this.gaps.state = WorldGaps.State.EnabledOff;
				this.gaps.currentTime = this.gaps.cyclePeriod;
				this.SetValue(0f);
				this.delayedNotification = null;
				this.notificationDelayForInvisible = false;
				this.notifier.Remove(this.rampDown);
				if (this.GapsVisible())
				{
					this.notifier.Add(this.over, string.Empty);
					this.delayedNotification = null;
					this.notificationDelayForInvisible = false;
				}
				this.hasNotified = true;
				this.Reset();
			}
			else
			{
				this.SetValue(this.gaps.currentTime / this.gaps.currentCloud.rampTime);
			}
			break;
		case WorldGaps.State.EnabledOff:
			this.Complete();
			if (!this.hasNotified && this.gaps.currentTime < this.gaps.notifyTimeApproaching)
			{
				this.hasNotified = true;
				if (this.GapsVisible())
				{
					this.notifier.Add(this.warning, string.Empty);
					this.delayedNotification = null;
					this.notificationDelayForInvisible = false;
				}
				else
				{
					this.notificationDelayForInvisible = true;
					this.delayedNotification = this.warning;
				}
			}
			else if (this.hasNotified && this.gaps.cyclePeriod - this.gaps.notifyTimeApproaching > this.gaps.currentTime)
			{
				this.notifier.Remove(this.over);
				this.hasNotified = false;
			}
			if (this.gaps.currentTime < 0f)
			{
				this.gaps.state = WorldGaps.State.EnabledRampUp;
				this.gaps.Next();
				this.gaps.currentTime = this.gaps.currentCloud.rampTime;
				this.gaps.currentCloud.element = this.gaps.currentCloud.element;
				this.currentElement = ElementLoader.FindElementByHash(this.gaps.currentCloud.element);
				this.notifier.Remove(this.warning);
				this.hasNotified = false;
				if (this.GapsVisible())
				{
					this.notifier.Add(this.rampUp, string.Empty);
					this.delayedNotification = null;
					this.notificationDelayForInvisible = false;
				}
				else
				{
					this.notificationDelayForInvisible = true;
					this.delayedNotification = this.rampUp;
				}
			}
			break;
		}
	}

	private bool hasNotified;

	private bool notificationDelayForInvisible;

	private Notification warning;

	private Notification rampUp;

	private Notification peak;

	private Notification rampDown;

	private Notification over;

	private Notification delayedNotification;

	private Element currentElement;

	[MyCmpReq]
	private Notifier notifier;

	public List<int> cellUpdateOrder;

	private bool loaded;

	private bool ready;

	public float currentPercent;

	public float currentMass;

	public float currentTemperature;

	public int cellsPerTick = 100;

	public int cellsLastTick;

	public int resetsLastTick;

	private List<int>.Enumerator currentCell;

	public int resetCells;

	public int numCompletes;

	public int gapCellCount;

	public int updateCellCount;
}
