using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;

public class ReportManager : KMonoBehaviour
{
	public ReportManager()
	{
		Dictionary<ReportManager.ReportType, ReportManager.ReportGroup> dictionary = new Dictionary<ReportManager.ReportType, ReportManager.ReportGroup>();
		dictionary.Add(ReportManager.ReportType.CaloriesCreated, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedCalories(v, GameUtil.TimeSlice.None, true), true, 1, UI.ENDOFDAYREPORT.CALORIES_CREATED.NAME, UI.ENDOFDAYREPORT.CALORIES_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CALORIES_CREATED.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.StressDelta, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedPercent(v, GameUtil.TimeSlice.None), true, 1, UI.ENDOFDAYREPORT.STRESS_DELTA.NAME, UI.ENDOFDAYREPORT.STRESS_DELTA.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.STRESS_DELTA.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.DiseaseAdded, new ReportManager.ReportGroup(null, false, 1, UI.ENDOFDAYREPORT.DISEASE_ADDED.NAME, UI.ENDOFDAYREPORT.DISEASE_ADDED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.DISEASE_ADDED.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.DiseaseStatus, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedDiseaseAmount((int)v), true, 1, UI.ENDOFDAYREPORT.DISEASE_STATUS.NAME, UI.ENDOFDAYREPORT.DISEASE_STATUS.TOOLTIP, string.Empty, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.TimeSpent, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedTime), true, 1, UI.ENDOFDAYREPORT.TIME_SPENT.NAME, UI.ENDOFDAYREPORT.TIME_SPENT.POSITIVE_TOOLTIP, string.Empty, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.TravelTime, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedTime), true, 1, UI.ENDOFDAYREPORT.TRAVEL_TIME.NAME, UI.ENDOFDAYREPORT.TRAVEL_TIME.POSITIVE_TOOLTIP, string.Empty, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.IdleTime, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedTime), true, 2, UI.ENDOFDAYREPORT.IDLE_TIME.NAME, UI.ENDOFDAYREPORT.IDLE_TIME.POSITIVE_TOOLTIP, string.Empty, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.OxygenCreated, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), true, 2, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NAME, UI.ENDOFDAYREPORT.OXYGEN_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.EnergyCreated, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedRoundedJoules), true, 2, UI.ENDOFDAYREPORT.ENERGY_USAGE.NAME, UI.ENDOFDAYREPORT.ENERGY_USAGE.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.ENERGY_USAGE.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.EnergyWasted, new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedRoundedJoules), true, 2, UI.ENDOFDAYREPORT.ENERGY_WASTED.NAME, UI.ENDOFDAYREPORT.ENERGY_WASTED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.ENERGY_WASTED.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.LevelUp, new ReportManager.ReportGroup(null, false, 2, UI.ENDOFDAYREPORT.LEVEL_UP.NAME, UI.ENDOFDAYREPORT.LEVEL_UP.TOOLTIP, string.Empty, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.ToiletIncident, new ReportManager.ReportGroup(null, false, 2, UI.ENDOFDAYREPORT.TOILET_INCIDENT.NAME, UI.ENDOFDAYREPORT.TOILET_INCIDENT.TOOLTIP, string.Empty, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.ContaminatedOxygenToilet, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), false, 2, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NAME, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
		dictionary.Add(ReportManager.ReportType.ContaminatedOxygenSublimation, new ReportManager.ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), false, 2, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NAME, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending));
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
		this.noteStorage = new ReportManager.NoteStorage();
	}

	protected override void OnCleanUp()
	{
		ReportManager.Instance = null;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		this.noteStorage.Serialize(binaryWriter);
		this.noteStorageBytes = memoryStream.GetBuffer();
	}

	[OnSerialized]
	private void OnSerialized()
	{
		this.noteStorageBytes = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.noteStorageBytes != null)
		{
			this.noteStorage.Deserialize(new BinaryReader(new MemoryStream(this.noteStorageBytes)));
			this.noteStorageBytes = null;
		}
	}

	private void OnSaveGameReady(object data)
	{
		base.Subscribe(GameClock.Instance.gameObject, -722330267, new Action<object>(this.OnNightTime));
		if (this.todaysReport == null)
		{
			this.todaysReport = new ReportManager.DailyReport(this);
			this.todaysReport.day = GameUtil.GetCurrentCycle();
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
		this.todaysReport.day = GameUtil.GetCurrentCycle() + 1;
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

	private ReportManager.NoteStorage noteStorage;

	public Dictionary<ReportManager.ReportType, ReportManager.ReportGroup> ReportGroups;

	[Serialize]
	private List<ReportManager.DailyReport> dailyReports;

	[Serialize]
	private ReportManager.DailyReport todaysReport;

	[Serialize]
	private byte[] noteStorageBytes;

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
		public ReportGroup(ReportManager.FormattingFn formatfn, bool reportIfZero, int group, string stringKey, string positiveTooltip, string negativeTooltip, ReportManager.ReportEntry.Order pos_note_order = ReportManager.ReportEntry.Order.Unordered, ReportManager.ReportEntry.Order neg_note_order = ReportManager.ReportEntry.Order.Unordered)
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
			this.posNoteOrder = pos_note_order;
			this.negNoteOrder = neg_note_order;
		}

		public ReportManager.FormattingFn formatfn;

		public string stringKey;

		public string positiveTooltip;

		public string negativeTooltip;

		public bool reportIfZero;

		public int group;

		public ReportManager.ReportEntry.Order posNoteOrder;

		public ReportManager.ReportEntry.Order negNoteOrder;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public class ReportEntry
	{
		public ReportEntry(ReportManager.ReportType reportType, int note_storage_id, string context)
		{
			this.reportType = reportType;
			this.context = context;
			this.accumulate = 0f;
			this.accPositive = 0f;
			this.accNegative = 0f;
			this.noteStorageId = note_storage_id;
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
		}

		public void IterateNotes(Action<ReportManager.ReportEntry.Note> callback)
		{
			ReportManager.Instance.noteStorage.IterateNotes(this.noteStorageId, callback);
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

		public void AddData(ReportManager.NoteStorage note_storage, float value, string note = null, string dataContext = null)
		{
			this.AddActualData(note_storage, value, note);
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
					reportEntry = new ReportManager.ReportEntry(this.reportType, note_storage.GetNewNoteId(), dataContext);
					this.contextEntries.Add(reportEntry);
				}
				reportEntry.AddActualData(note_storage, value, note);
			}
		}

		private void AddActualData(ReportManager.NoteStorage note_storage, float value, string note = null)
		{
			this.accumulate += value;
			if (value > 0f)
			{
				this.accPositive += value;
			}
			else
			{
				this.accNegative += value;
			}
			if (note != null)
			{
				note_storage.Add(this.noteStorageId, value, note);
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
		public int noteStorageId;

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

		public struct Note
		{
			public Note(float value, string note)
			{
				this.value = value;
				this.note = note;
			}

			public float value;

			public string note;
		}

		public enum Order
		{
			Unordered,
			Ascending,
			Descending
		}
	}

	public class DailyReport
	{
		public DailyReport(ReportManager manager)
		{
			foreach (KeyValuePair<ReportManager.ReportType, ReportManager.ReportGroup> keyValuePair in manager.ReportGroups)
			{
				this.reportEntries.Add(new ReportManager.ReportEntry(keyValuePair.Key, this.noteStorage.GetNewNoteId(), null));
			}
		}

		private ReportManager.NoteStorage noteStorage
		{
			get
			{
				return ReportManager.Instance.noteStorage;
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
			ReportManager.ReportEntry reportEntry2 = new ReportManager.ReportEntry(reportType, this.noteStorage.GetNewNoteId(), null);
			this.reportEntries.Add(reportEntry2);
			return reportEntry2;
		}

		public void AddData(ReportManager.ReportType reportType, float value, string note = null, string context = null)
		{
			ReportManager.ReportEntry entry = this.GetEntry(reportType);
			entry.AddData(this.noteStorage, value, note, context);
		}

		[Serialize]
		public int day;

		[Serialize]
		public List<ReportManager.ReportEntry> reportEntries = new List<ReportManager.ReportEntry>();
	}

	public class NoteStorage
	{
		public NoteStorage()
		{
			this.noteEntries = new ReportManager.NoteStorage.NoteEntries();
			this.stringTable = new ReportManager.NoteStorage.StringTable();
		}

		public void Add(int report_entry_id, float value, string note)
		{
			int num = this.stringTable.AddString(note);
			this.noteEntries.Add(report_entry_id, value, num);
		}

		public int GetNewNoteId()
		{
			return ++this.nextNoteId;
		}

		public void IterateNotes(int report_entry_id, Action<ReportManager.ReportEntry.Note> callback)
		{
			this.noteEntries.IterateNotes(this.stringTable, report_entry_id, callback);
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(5);
			writer.Write(this.nextNoteId);
			this.stringTable.Serialize(writer);
			this.noteEntries.Serialize(writer);
		}

		public void Deserialize(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num != 5)
			{
				return;
			}
			this.nextNoteId = reader.ReadInt32();
			this.stringTable.Deserialize(reader);
			this.noteEntries.Deserialize(reader);
		}

		private const int SERIALIZATION_VERSION = 5;

		private int nextNoteId;

		private ReportManager.NoteStorage.NoteEntries noteEntries;

		private ReportManager.NoteStorage.StringTable stringTable;

		private class StringTable
		{
			public int AddString(string str)
			{
				HashedString hashedString = new HashedString(str);
				this.strings[hashedString.HashValue] = str;
				return hashedString.HashValue;
			}

			public string GetStringByHash(int hash)
			{
				string empty = string.Empty;
				this.strings.TryGetValue(hash, out empty);
				return empty;
			}

			public void Serialize(BinaryWriter writer)
			{
				writer.Write(this.strings.Count);
				foreach (KeyValuePair<int, string> keyValuePair in this.strings)
				{
					writer.Write(keyValuePair.Value);
				}
			}

			public void Deserialize(BinaryReader reader)
			{
				int num = reader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					string text = reader.ReadString();
					this.AddString(text);
				}
			}

			private Dictionary<int, string> strings = new Dictionary<int, string>();
		}

		private class NoteEntries
		{
			public void Add(int report_entry_id, float value, int note_id)
			{
				int i = this.ReportEntryIdToStorageBlockIdx(report_entry_id);
				while (i >= this.storageBlocks.Count)
				{
					int num = 32;
					this.storageBlocks.Add(new ReportManager.NoteStorage.NoteEntries.NoteStorageBlock(num));
				}
				ReportManager.NoteStorage.NoteEntries.NoteStorageBlock noteStorageBlock = this.storageBlocks[i];
				noteStorageBlock.Add(report_entry_id, value, note_id);
				this.storageBlocks[i] = noteStorageBlock;
			}

			public void Serialize(BinaryWriter writer)
			{
				writer.Write(this.storageBlocks.Count);
				foreach (ReportManager.NoteStorage.NoteEntries.NoteStorageBlock noteStorageBlock in this.storageBlocks)
				{
					noteStorageBlock.Serialize(writer);
				}
			}

			public void Deserialize(BinaryReader reader)
			{
				int num = reader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					ReportManager.NoteStorage.NoteEntries.NoteStorageBlock noteStorageBlock = default(ReportManager.NoteStorage.NoteEntries.NoteStorageBlock);
					noteStorageBlock.Deserialize(reader);
					this.storageBlocks.Add(noteStorageBlock);
				}
			}

			private int ReportEntryIdToStorageBlockIdx(int report_entry_id)
			{
				return report_entry_id / 100;
			}

			public void IterateNotes(ReportManager.NoteStorage.StringTable string_table, int report_entry_id, Action<ReportManager.ReportEntry.Note> callback)
			{
				int num = this.ReportEntryIdToStorageBlockIdx(report_entry_id);
				if (num < this.storageBlocks.Count)
				{
					this.storageBlocks[num].IterateNotes(string_table, report_entry_id, callback);
				}
			}

			private const int REPORT_IDS_PER_BLOCK = 100;

			private List<ReportManager.NoteStorage.NoteEntries.NoteStorageBlock> storageBlocks = new List<ReportManager.NoteStorage.NoteEntries.NoteStorageBlock>();

			[StructLayout(LayoutKind.Explicit)]
			private struct NoteEntry
			{
				public NoteEntry(int report_entry_id, int note_hash, float value)
				{
					this.reportEntryId = report_entry_id;
					this.noteHash = note_hash;
					this.value = value;
				}

				public bool Matches(int report_entry_id, int note_hash, float value)
				{
					return report_entry_id == this.reportEntryId && note_hash == this.noteHash && value > 0f == this.value > 0f;
				}

				[FieldOffset(0)]
				public int reportEntryId;

				[FieldOffset(4)]
				public int noteHash;

				[FieldOffset(8)]
				public float value;
			}

			private struct NoteStorageBlock
			{
				public NoteStorageBlock(int capacity)
				{
					this.entries = new StructByteArray<ReportManager.NoteStorage.NoteEntries.NoteEntry>(capacity);
					this.entryCount = 0;
				}

				public void Add(int report_entry_id, float value, int note_id)
				{
					bool flag = false;
					for (int i = 0; i < this.entryCount; i++)
					{
						ReportManager.NoteStorage.NoteEntries.NoteEntry noteEntry = this.entries.structs[i];
						if (noteEntry.Matches(report_entry_id, note_id, value))
						{
							noteEntry.value += value;
							this.entries.structs[i] = noteEntry;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						if (this.entries.SizeInStructs <= this.entryCount)
						{
							this.entries.Resize(this.entries.SizeInStructs * 2);
						}
						this.entries.structs[this.entryCount++] = new ReportManager.NoteStorage.NoteEntries.NoteEntry(report_entry_id, note_id, value);
					}
				}

				public void IterateNotes(ReportManager.NoteStorage.StringTable string_table, int report_entry_id, Action<ReportManager.ReportEntry.Note> callback)
				{
					for (int i = 0; i < this.entryCount; i++)
					{
						ReportManager.NoteStorage.NoteEntries.NoteEntry noteEntry = this.entries.structs[i];
						if (noteEntry.reportEntryId == report_entry_id)
						{
							string stringByHash = string_table.GetStringByHash(noteEntry.noteHash);
							ReportManager.ReportEntry.Note note = new ReportManager.ReportEntry.Note(noteEntry.value, stringByHash);
							callback(note);
						}
					}
				}

				public void Serialize(BinaryWriter writer)
				{
					writer.Write(this.entryCount);
					writer.Write(this.entries.bytes, 0, this.entries.StructSizeInBytes * this.entryCount);
				}

				public void Deserialize(BinaryReader reader)
				{
					this.entryCount = reader.ReadInt32();
					this.entries.bytes = reader.ReadBytes(this.entries.StructSizeInBytes * this.entryCount);
				}

				private int entryCount;

				private StructByteArray<ReportManager.NoteStorage.NoteEntries.NoteEntry> entries;
			}
		}
	}
}
