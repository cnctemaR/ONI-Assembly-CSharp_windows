using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;

public class ReportManager : KMonoBehaviour
{
	public ReportManager()
	{
		Dictionary<ReportManager.ReportType, ReportManager.ReportGroup> dictionary = new Dictionary<ReportManager.ReportType, ReportManager.ReportGroup>();
		dictionary.Add(ReportManager.ReportType.OxygenCreated, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), true, 1, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NAME, UI.ENDOFDAYREPORT.OXYGEN_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.EnergyCreated, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedRoundedJoules), true, 1, UI.ENDOFDAYREPORT.ENERGY_USAGE.NAME, UI.ENDOFDAYREPORT.ENERGY_USAGE.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.ENERGY_USAGE.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.EnergyWasted, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedRoundedJoules), true, 1, UI.ENDOFDAYREPORT.ENERGY_WASTED.NAME, UI.ENDOFDAYREPORT.ENERGY_WASTED.POSITIVE_TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.CaloriesCreated, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedCalories(v, GameUtil.TimeSlice.None, true), true, 2, UI.ENDOFDAYREPORT.CALORIES_CREATED.NAME, UI.ENDOFDAYREPORT.CALORIES_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CALORIES_CREATED.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.StressDelta, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedPercent(v, GameUtil.TimeSlice.None), true, 2, UI.ENDOFDAYREPORT.STRESS_DELTA.NAME, UI.ENDOFDAYREPORT.STRESS_DELTA.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.STRESS_DELTA.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.TravelTime, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedTime), true, 2, UI.ENDOFDAYREPORT.TRAVEL_TIME.NAME, UI.ENDOFDAYREPORT.TRAVEL_TIME.POSITIVE_TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.IdleTime, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedTime), true, 2, UI.ENDOFDAYREPORT.IDLE_TIME.NAME, UI.ENDOFDAYREPORT.IDLE_TIME.POSITIVE_TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.TimeSpent, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedTime), true, 2, UI.ENDOFDAYREPORT.TIME_SPENT.NAME, UI.ENDOFDAYREPORT.TIME_SPENT.POSITIVE_TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.DiseaseStatus, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedDiseaseAmount((int)v), true, 2, UI.ENDOFDAYREPORT.DISEASE_STATUS.NAME, UI.ENDOFDAYREPORT.DISEASE_STATUS.TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.LevelUp, new ReportManager.ReportGroup(null, false, 3, UI.ENDOFDAYREPORT.LEVEL_UP.NAME, UI.ENDOFDAYREPORT.LEVEL_UP.TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.ToiletIncident, new ReportManager.ReportGroup(null, false, 3, UI.ENDOFDAYREPORT.TOILET_INCIDENT.NAME, UI.ENDOFDAYREPORT.TOILET_INCIDENT.TOOLTIP, string.Empty));
		dictionary.Add(ReportManager.ReportType.DiseaseAdded, new ReportManager.ReportGroup(null, false, 3, UI.ENDOFDAYREPORT.DISEASE_ADDED.NAME, UI.ENDOFDAYREPORT.DISEASE_ADDED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.DISEASE_ADDED.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.ContaminatedOxygenToilet, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), false, 4, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NAME, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NEGATIVE_TOOLTIP));
		dictionary.Add(ReportManager.ReportType.ContaminatedOxygenSublimation, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), false, 4, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NAME, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NEGATIVE_TOOLTIP));
		this.ReportGroups = dictionary;
		this.dailyReports = new List<ReportManager.DailyReport>();
		base..ctor();
	}

	public List<ReportManager.DailyReport> reports
	{
		get
		{
			return this.dailyReports;
		}
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
		base.Subscribe(Game.Instance.gameObject, -1917495436, new Action<object>(this.OnSaveGameReady));
	}

	protected override void OnCleanUp()
	{
		ReportManager.Instance = null;
	}

	private void OnSaveGameReady(object data)
	{
		base.Subscribe(GameClock.Instance.gameObject, -722330267, new Action<object>(this.OnNightTime));
		if (this.todaysReport == null)
		{
			this.todaysReport = new ReportManager.DailyReport(this);
			this.todaysReport.day = GameUtil.GetCurrentDay();
		}
	}

	public void ReportValue(ReportManager.ReportType reportType, float value, string note = null, string context = null)
	{
		this.TodaysReport.AddData(reportType, value, note, context);
	}

	private void OnNightTime(object data)
	{
		this.dailyReports.Add(this.todaysReport);
		int day = this.todaysReport.day;
		Notification notification = new Notification(string.Format(UI.ENDOFDAYREPORT.NOTIFICATION_TITLE, day), NotificationType.Good, HashedString.Invalid, (List<Notification> n, object d) => string.Format(UI.ENDOFDAYREPORT.NOTIFICATION_TOOLTIP, day), null, true, 0f, delegate(object d)
		{
			ManagementMenu.Instance.OpenReports(day);
		}, null, null);
		if (this.notifier == null)
		{
			Debug.LogError("Cant notify, null notifier", null);
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

	public delegate string FormattingFn(float v);

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
		ToiletIncident,
		ContaminatedOxygenFlatulence,
		ContaminatedOxygenToilet,
		ContaminatedOxygenSublimation,
		DiseaseStatus,
		TimeSpent
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

	[SerializationConfig(MemberSerialization.OptIn)]
	public class ReportEntry
	{
		public ReportEntry(ReportManager.ReportEntry entry)
		{
			this.gameHash = entry.gameHash;
			this.reportType = entry.reportType;
			this.context = entry.context;
			this.accumulate = entry.accumulate;
			this.accPositive = entry.accPositive;
			this.accNegative = entry.accNegative;
			this.posNotes = entry.posNotes;
			this.negNotes = entry.negNotes;
		}

		public ReportEntry(ReportManager.ReportType reportType, string context)
		{
			this.reportType = reportType;
			this.context = context;
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
			this.contextEntries = new List<ReportManager.ReportEntry>();
			this.posNotes = new Dictionary<string, float>();
			this.negNotes = new Dictionary<string, float>();
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			if (this.gameHash != -1)
			{
				this.reportType = (ReportManager.ReportType)this.gameHash;
				this.gameHash = -1;
			}
		}

		public void AddData(float value, string note = null, string dataContext = null)
		{
			this.AddActualData(value, note);
			if (dataContext != null)
			{
				ReportManager.ReportEntry reportEntry = null;
				for (int i = 0; i < this.contextEntries.Count; i++)
				{
					if (this.contextEntries[i].context == dataContext)
					{
						reportEntry = this.contextEntries[i];
						break;
					}
				}
				if (reportEntry == null)
				{
					reportEntry = new ReportManager.ReportEntry(this.reportType, dataContext);
					this.contextEntries.Add(reportEntry);
				}
				reportEntry.AddActualData(value, note);
			}
		}

		private void AddActualData(float value, string note = null)
		{
			this.accumulate += value;
			if (value > 0f)
			{
				this.accPositive += value;
				if (note != null && this.posNotes != null)
				{
					if (!this.posNotes.ContainsKey(note))
					{
						this.posNotes[note] = 0f;
					}
					Dictionary<string, float> dictionary;
					(dictionary = this.posNotes)[note] = dictionary[note] + value;
				}
			}
			else
			{
				this.accNegative += value;
				if (note != null && this.negNotes != null)
				{
					if (!this.negNotes.ContainsKey(note))
					{
						this.negNotes[note] = 0f;
					}
					Dictionary<string, float> dictionary;
					(dictionary = this.negNotes)[note] = dictionary[note] + value;
				}
			}
		}

		public bool HasContextEntries()
		{
			return this.contextEntries.Count > 0;
		}

		public List<ReportManager.ReportEntry> GetContextEntries()
		{
			return this.contextEntries;
		}

		[Serialize]
		public int gameHash = -1;

		[Serialize]
		public ReportManager.ReportType reportType;

		[Serialize]
		public string context;

		[Serialize]
		public float accumulate;

		[Serialize]
		public float accPositive;

		[Serialize]
		public float accNegative;

		[Serialize]
		public List<ReportManager.ReportEntry> contextEntries = new List<ReportManager.ReportEntry>();

		[Serialize]
		public Dictionary<string, float> posNotes = new Dictionary<string, float>();

		[Serialize]
		public Dictionary<string, float> negNotes = new Dictionary<string, float>();
	}

	public class DailyReport
	{
		public DailyReport(ReportManager manager)
		{
			foreach (KeyValuePair<ReportManager.ReportType, ReportManager.ReportGroup> keyValuePair in manager.ReportGroups)
			{
				this.reportEntries.Add(new ReportManager.ReportEntry(keyValuePair.Key, null));
			}
		}

		public ReportManager.ReportEntry GetEntry(ReportManager.ReportType reportType)
		{
			for (int i = 0; i < this.reportEntries.Count; i++)
			{
				ReportManager.ReportEntry reportEntry = this.reportEntries[i];
				if (reportEntry.reportType == reportType)
				{
					return reportEntry;
				}
			}
			ReportManager.ReportEntry reportEntry2 = new ReportManager.ReportEntry(reportType, null);
			this.reportEntries.Add(reportEntry2);
			return reportEntry2;
		}

		public void AddData(ReportManager.ReportType reportType, float value, string note = null, string context = null)
		{
			ReportManager.ReportEntry entry = this.GetEntry(reportType);
			entry.AddData(value, note, context);
		}

		[Serialize]
		public int day;

		[Serialize]
		public List<ReportManager.ReportEntry> reportEntries = new List<ReportManager.ReportEntry>();
	}
}
