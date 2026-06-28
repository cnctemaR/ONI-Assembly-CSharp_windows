using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ReportManager : KMonoBehaviour
{
	public ReportManager()
	{
		Dictionary<ReportManager.ReportType, ReportManager.ReportGroup> dictionary = new Dictionary<ReportManager.ReportType, ReportManager.ReportGroup>();
		dictionary.Add(ReportManager.ReportType.OxygenCreated, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, true, "F1"), true, 1, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NAME, UI.ENDOFDAYREPORT.OXYGEN_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.EnergyCreated, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedJoules), true, 1, UI.ENDOFDAYREPORT.ENERGY_USAGE.NAME, UI.ENDOFDAYREPORT.ENERGY_USAGE.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.ENERGY_USAGE.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.EnergyWasted, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedJoules), true, 1, UI.ENDOFDAYREPORT.ENERGY_WASTED.NAME, UI.ENDOFDAYREPORT.ENERGY_WASTED.POSITIVE_TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.CaloriesCreated, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedCalories(v, GameUtil.TimeSlice.None, true), true, 2, UI.ENDOFDAYREPORT.CALORIES_CREATED.NAME, UI.ENDOFDAYREPORT.CALORIES_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CALORIES_CREATED.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.StressDelta, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedPercent(v, GameUtil.TimeSlice.None), true, 2, UI.ENDOFDAYREPORT.STRESS_DELTA.NAME, UI.ENDOFDAYREPORT.STRESS_DELTA.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.STRESS_DELTA.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.TravelTime, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedTime(v), true, 2, UI.ENDOFDAYREPORT.TRAVEL_TIME.NAME, UI.ENDOFDAYREPORT.TRAVEL_TIME.POSITIVE_TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.IdleTime, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedTime(v), true, 2, UI.ENDOFDAYREPORT.IDLE_TIME.NAME, UI.ENDOFDAYREPORT.IDLE_TIME.POSITIVE_TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.LevelUp, new ReportManager.ReportGroup(null, false, 3, UI.ENDOFDAYREPORT.LEVEL_UP.NAME, UI.ENDOFDAYREPORT.LEVEL_UP.TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.ToiletIncident, new ReportManager.ReportGroup(null, false, 3, UI.ENDOFDAYREPORT.TOILET_INCIDENT.NAME, UI.ENDOFDAYREPORT.TOILET_INCIDENT.TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.DiseaseAdded, new ReportManager.ReportGroup(null, false, 3, UI.ENDOFDAYREPORT.DISEASE_ADDED.NAME, UI.ENDOFDAYREPORT.DISEASE_ADDED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.DISEASE_ADDED.NEGATIVE_TOOLTIP));
		this.ReportGroups = dictionary;
		this.dailyReports = new List<ReportManager.DailyReport>();
		base..ctor();
	}

	public static ReportManager Instance { get; private set; }

	public ReportManager.DailyReport TodaysReport
	{
		get
		{
			return this.todaysReport;
		}
	}

	protected override void OnPrefabInit()
	{
		ReportManager.Instance = this;
		base.Subscribe(Game.Instance.gameObject, -1917495436, new EventSystem.EventHandler(this.OnSaveGameReady));
	}

	protected override void OnCleanUp()
	{
		ReportManager.Instance = null;
	}

	private void OnSaveGameReady(object data)
	{
		base.Subscribe(GameClock.Instance.gameObject, -722330267, new EventSystem.EventHandler(this.OnNightTime));
		if (this.todaysReport == null)
		{
			this.todaysReport = new ReportManager.DailyReport(this);
			this.todaysReport.day = GameUtil.GetCurrentDay();
		}
	}

	public void ReportValue(ReportManager.ReportType gameHash, float value, string note = null)
	{
		ReportManager.ReportEntry entry = this.TodaysReport.GetEntry(gameHash);
		Debug.Assert(entry != null);
		entry.accumulate += value;
		if (value > 0f)
		{
			entry.accPositive += value;
			if (note != null && entry.posNotes != null)
			{
				if (!entry.posNotes.ContainsKey(note))
				{
					entry.posNotes[note] = 0f;
				}
				Dictionary<string, float> posNotes;
				Dictionary<string, float> dictionary = (posNotes = entry.posNotes);
				float num = posNotes[note];
				dictionary[note] = num + value;
			}
		}
		else
		{
			entry.accNegative += value;
			if (note != null && entry.negNotes != null)
			{
				if (!entry.negNotes.ContainsKey(note))
				{
					entry.negNotes[note] = 0f;
				}
				Dictionary<string, float> negNotes;
				Dictionary<string, float> dictionary2 = (negNotes = entry.negNotes);
				float num = negNotes[note];
				dictionary2[note] = num + value;
			}
		}
	}

	private void OnNightTime(object data)
	{
		this.dailyReports.Add(this.todaysReport);
		int day = this.todaysReport.day;
		Notification.ClickCallback clickCallback = delegate(object d)
		{
			ManagementMenu.Instance.OpenReports(day);
		};
		Notification notification = new Notification(string.Format(UI.ENDOFDAYREPORT.NOTIFICATION_TITLE, day), NotificationType.Good, null, (List<Notification> n, object d) => string.Format(UI.ENDOFDAYREPORT.NOTIFICATION_TOOLTIP, day), null, true, 0f, clickCallback, null, null);
		if (this.notifier == null)
		{
			Debug.LogError("Cant notify, null notifier");
		}
		else
		{
			this.notifier.Add(notification, string.Empty);
		}
		this.todaysReport = new ReportManager.DailyReport(this);
		this.todaysReport.day = GameUtil.GetCurrentDay() + 1;
	}

	public ReportManager.DailyReport FindReport(int day)
	{
		foreach (ReportManager.DailyReport dailyReport in this.dailyReports)
		{
			if (dailyReport.day == day)
			{
				return dailyReport;
			}
		}
		if (this.todaysReport.day == day)
		{
			return this.todaysReport;
		}
		return null;
	}

	[MyCmpAdd]
	private Notifier notifier;

	public Dictionary<ReportManager.ReportType, ReportManager.ReportGroup> ReportGroups;

	[Serialize]
	private List<ReportManager.DailyReport> dailyReports;

	[Serialize]
	private ReportManager.DailyReport todaysReport;

	public enum ReportType
	{
		OxygenCreated,
		CaloriesCreated,
		StressDelta,
		EnergyCreated,
		EnergyWasted,
		LevelUp,
		TravelTime,
		IdleTime,
		DiseaseAdded,
		ToiletIncident
	}

	public struct ReportGroup
	{
		public ReportGroup(ReportManager.FormattingFn formatfn, bool reportIfZero, int group, string stringKey, string positiveTooltip, string negativeTooltip)
		{
			ReportManager.FormattingFn formattingFn;
			if (formatfn != null)
			{
				formattingFn = formatfn;
			}
			else
			{
				formattingFn = (float v) => v.ToString();
			}
			this.formatfn = formattingFn;
			this.stringKey = stringKey;
			this.positiveTooltip = positiveTooltip;
			this.negativeTooltip = negativeTooltip;
			this.reportIfZero = reportIfZero;
			this.group = group;
		}

		public ReportManager.FormattingFn formatfn;

		public string stringKey;

		public string positiveTooltip;

		public string negativeTooltip;

		public bool reportIfZero;

		public int group;
	}

	public class ReportEntry
	{
		public ReportEntry(ReportManager.ReportEntry entry)
		{
			this.gameHash = entry.gameHash;
			this.accumulate = entry.accumulate;
			this.accPositive = entry.accPositive;
			this.accNegative = entry.accNegative;
			this.posNotes = entry.posNotes;
			this.negNotes = entry.negNotes;
		}

		public ReportEntry(int gameHash)
		{
			this.gameHash = gameHash;
			this.accumulate = 0f;
			this.accPositive = 0f;
			this.accNegative = 0f;
			this.posNotes = new Dictionary<string, float>();
			this.negNotes = new Dictionary<string, float>();
		}

		public float Positive
		{
			get
			{
				return this.accPositive;
			}
		}

		public float Negative
		{
			get
			{
				return this.accNegative;
			}
		}

		public float Net
		{
			get
			{
				return this.accPositive + this.accNegative;
			}
		}

		[OnDeserializing]
		private void OnDeserialize()
		{
			this.posNotes = new Dictionary<string, float>();
			this.negNotes = new Dictionary<string, float>();
		}

		public int gameHash;

		public float accumulate;

		public float accPositive;

		public float accNegative;

		public Dictionary<string, float> posNotes = new Dictionary<string, float>();

		public Dictionary<string, float> negNotes = new Dictionary<string, float>();
	}

	public class DailyReport
	{
		public DailyReport(ReportManager manager)
		{
			foreach (KeyValuePair<ReportManager.ReportType, ReportManager.ReportGroup> keyValuePair in manager.ReportGroups)
			{
				this.reportEntries.Add(new ReportManager.ReportEntry((int)keyValuePair.Key));
			}
		}

		public ReportManager.ReportEntry GetEntry(ReportManager.ReportType hash)
		{
			for (int i = 0; i < this.reportEntries.Count; i++)
			{
				ReportManager.ReportEntry reportEntry = this.reportEntries[i];
				if (reportEntry.gameHash == (int)hash)
				{
					return reportEntry;
				}
			}
			return null;
		}

		[Serialize]
		public int day;

		[Serialize]
		public List<ReportManager.ReportEntry> reportEntries = new List<ReportManager.ReportEntry>();
	}

	public delegate string FormattingFn(float v);
}
