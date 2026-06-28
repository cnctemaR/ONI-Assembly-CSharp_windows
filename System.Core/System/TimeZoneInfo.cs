using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace System
{
	[Serializable]
	public sealed class TimeZoneInfo : ISerializable, IDeserializationCallback, IEquatable<TimeZoneInfo>
	{
		private TimeZoneInfo(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, TimeZoneInfo.AdjustmentRule[] adjustmentRules, bool disableDaylightSavingTime)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id == string.Empty)
			{
				throw new ArgumentException("id parameter is an empty string");
			}
			if (baseUtcOffset.Ticks % 600000000L != 0L)
			{
				throw new ArgumentException("baseUtcOffset parameter does not represent a whole number of minutes");
			}
			if (baseUtcOffset > new TimeSpan(14, 0, 0) || baseUtcOffset < new TimeSpan(-14, 0, 0))
			{
				throw new ArgumentOutOfRangeException("baseUtcOffset parameter is greater than 14 hours or less than -14 hours");
			}
			if (adjustmentRules != null && adjustmentRules.Length != 0)
			{
				TimeZoneInfo.AdjustmentRule adjustmentRule = null;
				foreach (TimeZoneInfo.AdjustmentRule adjustmentRule2 in adjustmentRules)
				{
					if (adjustmentRule2 == null)
					{
						throw new InvalidTimeZoneException("one or more elements in adjustmentRules are null");
					}
					if (baseUtcOffset + adjustmentRule2.DaylightDelta < new TimeSpan(-14, 0, 0) || baseUtcOffset + adjustmentRule2.DaylightDelta > new TimeSpan(14, 0, 0))
					{
						throw new InvalidTimeZoneException("Sum of baseUtcOffset and DaylightDelta of one or more object in adjustmentRules array is greater than 14 or less than -14 hours;");
					}
					if (adjustmentRule != null && adjustmentRule.DateStart > adjustmentRule2.DateStart)
					{
						throw new InvalidTimeZoneException("adjustment rules specified in adjustmentRules parameter are not in chronological order");
					}
					if (adjustmentRule != null && adjustmentRule.DateEnd > adjustmentRule2.DateStart)
					{
						throw new InvalidTimeZoneException("some adjustment rules in the adjustmentRules parameter overlap");
					}
					if (adjustmentRule != null && adjustmentRule.DateEnd == adjustmentRule2.DateStart)
					{
						throw new InvalidTimeZoneException("a date can have multiple adjustment rules applied to it");
					}
					adjustmentRule = adjustmentRule2;
				}
			}
			this.id = id;
			this.baseUtcOffset = baseUtcOffset;
			this.displayName = displayName ?? id;
			this.standardDisplayName = standardDisplayName ?? id;
			this.daylightDisplayName = daylightDisplayName;
			this.disableDaylightSavingTime = disableDaylightSavingTime;
			this.adjustmentRules = adjustmentRules;
		}

		public TimeSpan BaseUtcOffset
		{
			get
			{
				return this.baseUtcOffset;
			}
		}

		public string DaylightName
		{
			get
			{
				if (this.disableDaylightSavingTime)
				{
					return string.Empty;
				}
				return this.daylightDisplayName;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public static TimeZoneInfo Local
		{
			get
			{
				if (TimeZoneInfo.local == null)
				{
					try
					{
						TimeZoneInfo.local = TimeZoneInfo.FindSystemTimeZoneByFileName("Local", "/etc/localtime");
					}
					catch
					{
						try
						{
							TimeZoneInfo.local = TimeZoneInfo.FindSystemTimeZoneByFileName("Local", Path.Combine(TimeZoneInfo.TimeZoneDirectory, "localtime"));
						}
						catch
						{
							throw new TimeZoneNotFoundException();
						}
					}
				}
				return TimeZoneInfo.local;
			}
		}

		public string StandardName
		{
			get
			{
				return this.standardDisplayName;
			}
		}

		public bool SupportsDaylightSavingTime
		{
			get
			{
				return !this.disableDaylightSavingTime;
			}
		}

		public static TimeZoneInfo Utc
		{
			get
			{
				if (TimeZoneInfo.utc == null)
				{
					TimeZoneInfo.utc = TimeZoneInfo.CreateCustomTimeZone("UTC", new TimeSpan(0L), "UTC", "UTC");
				}
				return TimeZoneInfo.utc;
			}
		}

		private static string TimeZoneDirectory
		{
			get
			{
				if (TimeZoneInfo.timeZoneDirectory == null)
				{
					TimeZoneInfo.timeZoneDirectory = "/usr/share/zoneinfo";
				}
				return TimeZoneInfo.timeZoneDirectory;
			}
			set
			{
				TimeZoneInfo.ClearCachedData();
				TimeZoneInfo.timeZoneDirectory = value;
			}
		}

		public static void ClearCachedData()
		{
			TimeZoneInfo.local = null;
			TimeZoneInfo.utc = null;
			TimeZoneInfo.systemTimeZones = null;
		}

		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo destinationTimeZone)
		{
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, destinationTimeZone);
		}

		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
		{
			if (dateTime.Kind == DateTimeKind.Local && sourceTimeZone != TimeZoneInfo.Local)
			{
				throw new ArgumentException("Kind propery of dateTime is Local but the sourceTimeZone does not equal TimeZoneInfo.Local");
			}
			if (dateTime.Kind == DateTimeKind.Utc && sourceTimeZone != TimeZoneInfo.Utc)
			{
				throw new ArgumentException("Kind propery of dateTime is Utc but the sourceTimeZone does not equal TimeZoneInfo.Utc");
			}
			if (sourceTimeZone.IsInvalidTime(dateTime))
			{
				throw new ArgumentException("dateTime parameter is an invalid time");
			}
			if (sourceTimeZone == null)
			{
				throw new ArgumentNullException("sourceTimeZone");
			}
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			if (dateTime.Kind == DateTimeKind.Local && sourceTimeZone == TimeZoneInfo.Local && destinationTimeZone == TimeZoneInfo.Local)
			{
				return dateTime;
			}
			DateTime dateTime2 = TimeZoneInfo.ConvertTimeToUtc(dateTime);
			if (destinationTimeZone == TimeZoneInfo.Utc)
			{
				return dateTime2;
			}
			return TimeZoneInfo.ConvertTimeFromUtc(dateTime2, destinationTimeZone);
		}

		public static DateTimeOffset ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone)
		{
			throw new NotImplementedException();
		}

		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string destinationTimeZoneId)
		{
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string sourceTimeZoneId, string destinationTimeZoneId)
		{
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId), TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		public static DateTimeOffset ConvertTimeBySystemTimeZoneId(DateTimeOffset dateTimeOffset, string destinationTimeZoneId)
		{
			return TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		private DateTime ConvertTimeFromUtc(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Local)
			{
				throw new ArgumentException("Kind property of dateTime is Local");
			}
			if (this == TimeZoneInfo.Utc)
			{
				return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			}
			if (this == TimeZoneInfo.Local)
			{
				return DateTime.SpecifyKind(dateTime.ToLocalTime(), DateTimeKind.Unspecified);
			}
			TimeZoneInfo.AdjustmentRule applicableRule = this.GetApplicableRule(dateTime);
			if (this.IsDaylightSavingTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)))
			{
				return DateTime.SpecifyKind(dateTime + this.BaseUtcOffset + applicableRule.DaylightDelta, DateTimeKind.Unspecified);
			}
			return DateTime.SpecifyKind(dateTime + this.BaseUtcOffset, DateTimeKind.Unspecified);
		}

		public static DateTime ConvertTimeFromUtc(DateTime dateTime, TimeZoneInfo destinationTimeZone)
		{
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			return destinationTimeZone.ConvertTimeFromUtc(dateTime);
		}

		public static DateTime ConvertTimeToUtc(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime;
			}
			return DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
		}

		public static DateTime ConvertTimeToUtc(DateTime dateTime, TimeZoneInfo sourceTimeZone)
		{
			if (sourceTimeZone == null)
			{
				throw new ArgumentNullException("sourceTimeZone");
			}
			if (dateTime.Kind == DateTimeKind.Utc && sourceTimeZone != TimeZoneInfo.Utc)
			{
				throw new ArgumentException("Kind propery of dateTime is Utc but the sourceTimeZone does not equal TimeZoneInfo.Utc");
			}
			if (dateTime.Kind == DateTimeKind.Local && sourceTimeZone != TimeZoneInfo.Local)
			{
				throw new ArgumentException("Kind propery of dateTime is Local but the sourceTimeZone does not equal TimeZoneInfo.Local");
			}
			if (sourceTimeZone.IsInvalidTime(dateTime))
			{
				throw new ArgumentException("dateTime parameter is an invalid time");
			}
			if (dateTime.Kind == DateTimeKind.Utc && sourceTimeZone == TimeZoneInfo.Utc)
			{
				return dateTime;
			}
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime;
			}
			if (dateTime.Kind == DateTimeKind.Local)
			{
				return TimeZoneInfo.ConvertTimeToUtc(dateTime);
			}
			if (sourceTimeZone.IsAmbiguousTime(dateTime) || !sourceTimeZone.IsDaylightSavingTime(dateTime))
			{
				return DateTime.SpecifyKind(dateTime - sourceTimeZone.BaseUtcOffset, DateTimeKind.Utc);
			}
			TimeZoneInfo.AdjustmentRule applicableRule = sourceTimeZone.GetApplicableRule(dateTime);
			return DateTime.SpecifyKind(dateTime - sourceTimeZone.BaseUtcOffset - applicableRule.DaylightDelta, DateTimeKind.Utc);
		}

		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName)
		{
			return TimeZoneInfo.CreateCustomTimeZone(id, baseUtcOffset, displayName, standardDisplayName, null, null, true);
		}

		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, TimeZoneInfo.AdjustmentRule[] adjustmentRules)
		{
			return TimeZoneInfo.CreateCustomTimeZone(id, baseUtcOffset, displayName, standardDisplayName, daylightDisplayName, adjustmentRules, false);
		}

		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, TimeZoneInfo.AdjustmentRule[] adjustmentRules, bool disableDaylightSavingTime)
		{
			return new TimeZoneInfo(id, baseUtcOffset, displayName, standardDisplayName, daylightDisplayName, adjustmentRules, disableDaylightSavingTime);
		}

		public bool Equals(TimeZoneInfo other)
		{
			return other != null && other.Id == this.Id && this.HasSameRules(other);
		}

		public static TimeZoneInfo FindSystemTimeZoneById(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			string text = Path.Combine(TimeZoneInfo.TimeZoneDirectory, id);
			return TimeZoneInfo.FindSystemTimeZoneByFileName(id, text);
		}

		private static TimeZoneInfo FindSystemTimeZoneByFileName(string id, string filepath)
		{
			if (!File.Exists(filepath))
			{
				throw new TimeZoneNotFoundException();
			}
			byte[] array = new byte[16384];
			int num;
			using (FileStream fileStream = File.OpenRead(filepath))
			{
				num = fileStream.Read(array, 0, 16384);
			}
			if (!TimeZoneInfo.ValidTZFile(array, num))
			{
				throw new InvalidTimeZoneException("TZ file too big for the buffer");
			}
			TimeZoneInfo timeZoneInfo;
			try
			{
				timeZoneInfo = TimeZoneInfo.ParseTZBuffer(id, array, num);
			}
			catch (Exception ex)
			{
				throw new InvalidTimeZoneException(ex.Message);
			}
			return timeZoneInfo;
		}

		public static TimeZoneInfo FromSerializedString(string source)
		{
			throw new NotImplementedException();
		}

		public TimeZoneInfo.AdjustmentRule[] GetAdjustmentRules()
		{
			if (this.disableDaylightSavingTime)
			{
				return new TimeZoneInfo.AdjustmentRule[0];
			}
			return (TimeZoneInfo.AdjustmentRule[])this.adjustmentRules.Clone();
		}

		public TimeSpan[] GetAmbiguousTimeOffsets(DateTime dateTime)
		{
			if (!this.IsAmbiguousTime(dateTime))
			{
				throw new ArgumentException("dateTime is not an ambiguous time");
			}
			TimeZoneInfo.AdjustmentRule applicableRule = this.GetApplicableRule(dateTime);
			return new TimeSpan[]
			{
				this.baseUtcOffset,
				this.baseUtcOffset + applicableRule.DaylightDelta
			};
		}

		public TimeSpan[] GetAmbiguousTimeOffsets(DateTimeOffset dateTimeOffset)
		{
			if (!this.IsAmbiguousTime(dateTimeOffset))
			{
				throw new ArgumentException("dateTimeOffset is not an ambiguous time");
			}
			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			int num = this.Id.GetHashCode();
			foreach (TimeZoneInfo.AdjustmentRule adjustmentRule in this.GetAdjustmentRules())
			{
				num ^= adjustmentRule.GetHashCode();
			}
			return num;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public static ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
		{
			if (TimeZoneInfo.systemTimeZones == null)
			{
				TimeZoneInfo.systemTimeZones = new List<TimeZoneInfo>();
				string[] array = new string[]
				{
					"Africa", "America", "Antarctica", "Arctic", "Asia", "Atlantic", "Brazil", "Canada", "Chile", "Europe",
					"Indian", "Mexico", "Mideast", "Pacific", "US"
				};
				foreach (string text in array)
				{
					try
					{
						foreach (string text2 in Directory.GetFiles(Path.Combine(TimeZoneInfo.TimeZoneDirectory, text)))
						{
							try
							{
								string text3 = string.Format("{0}/{1}", text, Path.GetFileName(text2));
								TimeZoneInfo.systemTimeZones.Add(TimeZoneInfo.FindSystemTimeZoneById(text3));
							}
							catch (ArgumentNullException)
							{
							}
							catch (TimeZoneNotFoundException)
							{
							}
							catch (InvalidTimeZoneException)
							{
							}
							catch (Exception)
							{
								throw;
							}
						}
					}
					catch
					{
					}
				}
			}
			return new ReadOnlyCollection<TimeZoneInfo>(TimeZoneInfo.systemTimeZones);
		}

		public TimeSpan GetUtcOffset(DateTime dateTime)
		{
			if (this.IsDaylightSavingTime(dateTime))
			{
				TimeZoneInfo.AdjustmentRule applicableRule = this.GetApplicableRule(dateTime);
				return this.BaseUtcOffset + applicableRule.DaylightDelta;
			}
			return this.BaseUtcOffset;
		}

		public TimeSpan GetUtcOffset(DateTimeOffset dateTimeOffset)
		{
			throw new NotImplementedException();
		}

		public bool HasSameRules(TimeZoneInfo other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.adjustmentRules == null != (other.adjustmentRules == null))
			{
				return false;
			}
			if (this.adjustmentRules == null)
			{
				return true;
			}
			if (this.BaseUtcOffset != other.BaseUtcOffset)
			{
				return false;
			}
			if (this.adjustmentRules.Length != other.adjustmentRules.Length)
			{
				return false;
			}
			for (int i = 0; i < this.adjustmentRules.Length; i++)
			{
				if (!this.adjustmentRules[i].Equals(other.adjustmentRules[i]))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsAmbiguousTime(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Local && this.IsInvalidTime(dateTime))
			{
				throw new ArgumentException("Kind is Local and time is Invalid");
			}
			if (this == TimeZoneInfo.Utc)
			{
				return false;
			}
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				dateTime = this.ConvertTimeFromUtc(dateTime);
			}
			if (dateTime.Kind == DateTimeKind.Local && this != TimeZoneInfo.Local)
			{
				dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, this);
			}
			TimeZoneInfo.AdjustmentRule applicableRule = this.GetApplicableRule(dateTime);
			DateTime dateTime2 = TimeZoneInfo.TransitionPoint(applicableRule.DaylightTransitionEnd, dateTime.Year);
			return dateTime > dateTime2 - applicableRule.DaylightDelta && dateTime <= dateTime2;
		}

		public bool IsAmbiguousTime(DateTimeOffset dateTimeOffset)
		{
			throw new NotImplementedException();
		}

		public bool IsDaylightSavingTime(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Local && this.IsInvalidTime(dateTime))
			{
				throw new ArgumentException("dateTime is invalid and Kind is Local");
			}
			if (this == TimeZoneInfo.Utc)
			{
				return false;
			}
			if (!this.SupportsDaylightSavingTime)
			{
				return false;
			}
			if ((dateTime.Kind == DateTimeKind.Local || dateTime.Kind == DateTimeKind.Unspecified) && this == TimeZoneInfo.Local)
			{
				return dateTime.IsDaylightSavingTime();
			}
			if (dateTime.Kind == DateTimeKind.Local && this != TimeZoneInfo.Utc)
			{
				return this.IsDaylightSavingTime(DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc));
			}
			TimeZoneInfo.AdjustmentRule applicableRule = this.GetApplicableRule(dateTime.Date);
			if (applicableRule == null)
			{
				return false;
			}
			DateTime dateTime2 = TimeZoneInfo.TransitionPoint(applicableRule.DaylightTransitionStart, dateTime.Year);
			DateTime dateTime3 = TimeZoneInfo.TransitionPoint(applicableRule.DaylightTransitionEnd, dateTime.Year + ((applicableRule.DaylightTransitionStart.Month >= applicableRule.DaylightTransitionEnd.Month) ? 1 : 0));
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				dateTime2 -= this.BaseUtcOffset;
				dateTime3 -= this.BaseUtcOffset + applicableRule.DaylightDelta;
			}
			return dateTime >= dateTime2 && dateTime < dateTime3;
		}

		public bool IsDaylightSavingTime(DateTimeOffset dateTimeOffset)
		{
			throw new NotImplementedException();
		}

		public bool IsInvalidTime(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return false;
			}
			if (dateTime.Kind == DateTimeKind.Local && this != TimeZoneInfo.Local)
			{
				return false;
			}
			TimeZoneInfo.AdjustmentRule applicableRule = this.GetApplicableRule(dateTime);
			DateTime dateTime2 = TimeZoneInfo.TransitionPoint(applicableRule.DaylightTransitionStart, dateTime.Year);
			return dateTime >= dateTime2 && dateTime < dateTime2 + applicableRule.DaylightDelta;
		}

		public void OnDeserialization(object sender)
		{
			throw new NotImplementedException();
		}

		public string ToSerializedString()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.DisplayName;
		}

		private TimeZoneInfo.AdjustmentRule GetApplicableRule(DateTime dateTime)
		{
			DateTime dateTime2 = dateTime;
			if (dateTime.Kind == DateTimeKind.Local && this != TimeZoneInfo.Local)
			{
				dateTime2 = dateTime2.ToUniversalTime() + this.BaseUtcOffset;
			}
			if (dateTime.Kind == DateTimeKind.Utc && this != TimeZoneInfo.Utc)
			{
				dateTime2 += this.BaseUtcOffset;
			}
			foreach (TimeZoneInfo.AdjustmentRule adjustmentRule in this.adjustmentRules)
			{
				if (adjustmentRule.DateStart > dateTime2.Date)
				{
					return null;
				}
				if (!(adjustmentRule.DateEnd < dateTime2.Date))
				{
					return adjustmentRule;
				}
			}
			return null;
		}

		private static DateTime TransitionPoint(TimeZoneInfo.TransitionTime transition, int year)
		{
			if (transition.IsFixedDateRule)
			{
				return new DateTime(year, transition.Month, transition.Day) + transition.TimeOfDay.TimeOfDay;
			}
			DateTime dateTime = new DateTime(year, transition.Month, 1);
			DayOfWeek dayOfWeek = dateTime.DayOfWeek;
			int num = 1 + (transition.Week - 1) * 7 + (transition.DayOfWeek - dayOfWeek) % 7;
			if (num > DateTime.DaysInMonth(year, transition.Month))
			{
				num -= 7;
			}
			return new DateTime(year, transition.Month, num) + transition.TimeOfDay.TimeOfDay;
		}

		private static bool ValidTZFile(byte[] buffer, int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 4; i++)
			{
				stringBuilder.Append((char)buffer[i]);
			}
			return !(stringBuilder.ToString() != "TZif") && length < 16384;
		}

		private static int SwapInt32(int i)
		{
			return ((i >> 24) & 255) | ((i >> 8) & 65280) | ((i << 8) & 16711680) | (i << 24);
		}

		private static int ReadBigEndianInt32(byte[] buffer, int start)
		{
			int num = BitConverter.ToInt32(buffer, start);
			if (!BitConverter.IsLittleEndian)
			{
				return num;
			}
			return TimeZoneInfo.SwapInt32(num);
		}

		private static TimeZoneInfo ParseTZBuffer(string id, byte[] buffer, int length)
		{
			int num = TimeZoneInfo.ReadBigEndianInt32(buffer, 20);
			int num2 = TimeZoneInfo.ReadBigEndianInt32(buffer, 24);
			int num3 = TimeZoneInfo.ReadBigEndianInt32(buffer, 28);
			int num4 = TimeZoneInfo.ReadBigEndianInt32(buffer, 32);
			int num5 = TimeZoneInfo.ReadBigEndianInt32(buffer, 36);
			int num6 = TimeZoneInfo.ReadBigEndianInt32(buffer, 40);
			if (length < 44 + num4 * 5 + num5 * 6 + num6 + num3 * 8 + num2 + num)
			{
				throw new InvalidTimeZoneException();
			}
			Dictionary<int, string> dictionary = TimeZoneInfo.ParseAbbreviations(buffer, 44 + 4 * num4 + num4 + 6 * num5, num6);
			Dictionary<int, TimeZoneInfo.TimeType> dictionary2 = TimeZoneInfo.ParseTimesTypes(buffer, 44 + 4 * num4 + num4, num5, dictionary);
			List<KeyValuePair<DateTime, TimeZoneInfo.TimeType>> list = TimeZoneInfo.ParseTransitions(buffer, 44, num4, dictionary2);
			if (dictionary2.Count == 0)
			{
				throw new InvalidTimeZoneException();
			}
			if (dictionary2.Count == 1 && dictionary2[0].IsDst)
			{
				throw new InvalidTimeZoneException();
			}
			TimeSpan timeSpan = new TimeSpan(0L);
			TimeSpan timeSpan2 = new TimeSpan(0L);
			string text = null;
			string text2 = null;
			bool flag = false;
			DateTime dateTime = DateTime.MinValue;
			List<TimeZoneInfo.AdjustmentRule> list2 = new List<TimeZoneInfo.AdjustmentRule>();
			for (int i = 0; i < list.Count; i++)
			{
				KeyValuePair<DateTime, TimeZoneInfo.TimeType> keyValuePair = list[i];
				DateTime key = keyValuePair.Key;
				TimeZoneInfo.TimeType value = keyValuePair.Value;
				if (!value.IsDst)
				{
					if (text != value.Name || timeSpan.TotalSeconds != (double)value.Offset)
					{
						text = value.Name;
						text2 = null;
						timeSpan = new TimeSpan(0, 0, value.Offset);
						list2 = new List<TimeZoneInfo.AdjustmentRule>();
						flag = false;
					}
					if (flag)
					{
						dateTime += timeSpan;
						DateTime dateTime2 = key + timeSpan + timeSpan2;
						if (dateTime2.Date == new DateTime(dateTime2.Year, 1, 1) && dateTime2.Year > dateTime.Year)
						{
							dateTime2 -= new TimeSpan(24, 0, 0);
						}
						DateTime dateTime3;
						if (dateTime.Month < 7)
						{
							dateTime3 = new DateTime(dateTime.Year, 1, 1);
						}
						else
						{
							dateTime3 = new DateTime(dateTime.Year, 7, 1);
						}
						DateTime dateTime4;
						if (dateTime2.Month >= 7)
						{
							dateTime4 = new DateTime(dateTime2.Year, 12, 31);
						}
						else
						{
							dateTime4 = new DateTime(dateTime2.Year, 6, 30);
						}
						TimeZoneInfo.TransitionTime transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1) + dateTime.TimeOfDay, dateTime.Month, dateTime.Day);
						TimeZoneInfo.TransitionTime transitionTime2 = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1) + dateTime2.TimeOfDay, dateTime2.Month, dateTime2.Day);
						if (transitionTime != transitionTime2)
						{
							list2.Add(TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateTime3, dateTime4, timeSpan2, transitionTime, transitionTime2));
						}
					}
					flag = false;
				}
				else
				{
					if (text2 != value.Name || timeSpan2.TotalSeconds != (double)value.Offset - timeSpan.TotalSeconds)
					{
						text2 = value.Name;
						timeSpan2 = new TimeSpan(0, 0, value.Offset) - timeSpan;
					}
					dateTime = key;
					flag = true;
				}
			}
			if (list2.Count == 0)
			{
				TimeZoneInfo.TimeType timeType = dictionary2[0];
				if (text == null)
				{
					text = timeType.Name;
					timeSpan = new TimeSpan(0, 0, timeType.Offset);
				}
				return TimeZoneInfo.CreateCustomTimeZone(id, timeSpan, id, text);
			}
			return TimeZoneInfo.CreateCustomTimeZone(id, timeSpan, id, text, text2, TimeZoneInfo.ValidateRules(list2).ToArray());
		}

		private static List<TimeZoneInfo.AdjustmentRule> ValidateRules(List<TimeZoneInfo.AdjustmentRule> adjustmentRules)
		{
			TimeZoneInfo.AdjustmentRule adjustmentRule = null;
			foreach (TimeZoneInfo.AdjustmentRule adjustmentRule2 in adjustmentRules.ToArray())
			{
				if (adjustmentRule != null && adjustmentRule.DateEnd > adjustmentRule2.DateStart)
				{
					adjustmentRules.Remove(adjustmentRule2);
				}
				adjustmentRule = adjustmentRule2;
			}
			return adjustmentRules;
		}

		private static Dictionary<int, string> ParseAbbreviations(byte[] buffer, int index, int count)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				char c = (char)buffer[index + i];
				if (c != '\0')
				{
					stringBuilder.Append(c);
				}
				else
				{
					dictionary.Add(num, stringBuilder.ToString());
					for (int j = 1; j < stringBuilder.Length; j++)
					{
						dictionary.Add(num + j, stringBuilder.ToString(j, stringBuilder.Length - j));
					}
					num = i + 1;
					stringBuilder = new StringBuilder();
				}
			}
			return dictionary;
		}

		private static Dictionary<int, TimeZoneInfo.TimeType> ParseTimesTypes(byte[] buffer, int index, int count, Dictionary<int, string> abbreviations)
		{
			Dictionary<int, TimeZoneInfo.TimeType> dictionary = new Dictionary<int, TimeZoneInfo.TimeType>(count);
			for (int i = 0; i < count; i++)
			{
				int num = TimeZoneInfo.ReadBigEndianInt32(buffer, index + 6 * i);
				byte b = buffer[index + 6 * i + 4];
				byte b2 = buffer[index + 6 * i + 5];
				dictionary.Add(i, new TimeZoneInfo.TimeType(num, b != 0, abbreviations[(int)b2]));
			}
			return dictionary;
		}

		private static List<KeyValuePair<DateTime, TimeZoneInfo.TimeType>> ParseTransitions(byte[] buffer, int index, int count, Dictionary<int, TimeZoneInfo.TimeType> time_types)
		{
			List<KeyValuePair<DateTime, TimeZoneInfo.TimeType>> list = new List<KeyValuePair<DateTime, TimeZoneInfo.TimeType>>(count);
			for (int i = 0; i < count; i++)
			{
				int num = TimeZoneInfo.ReadBigEndianInt32(buffer, index + 4 * i);
				DateTime dateTime = TimeZoneInfo.DateTimeFromUnixTime((long)num);
				byte b = buffer[index + 4 * count + i];
				list.Add(new KeyValuePair<DateTime, TimeZoneInfo.TimeType>(dateTime, time_types[(int)b]));
			}
			return list;
		}

		private static DateTime DateTimeFromUnixTime(long unix_time)
		{
			DateTime dateTime = new DateTime(1970, 1, 1);
			return dateTime.AddSeconds((double)unix_time);
		}

		private const int BUFFER_SIZE = 16384;

		private TimeSpan baseUtcOffset;

		private string daylightDisplayName;

		private string displayName;

		private string id;

		private static TimeZoneInfo local;

		private string standardDisplayName;

		private bool disableDaylightSavingTime;

		private static TimeZoneInfo utc;

		private static string timeZoneDirectory;

		private TimeZoneInfo.AdjustmentRule[] adjustmentRules;

		private static List<TimeZoneInfo> systemTimeZones;

		[Serializable]
		public sealed class AdjustmentRule : ISerializable, IDeserializationCallback, IEquatable<TimeZoneInfo.AdjustmentRule>
		{
			private AdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd)
			{
				if (dateStart.Kind != DateTimeKind.Unspecified || dateEnd.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException("the Kind property of dateStart or dateEnd parameter does not equal DateTimeKind.Unspecified");
				}
				if (daylightTransitionStart == daylightTransitionEnd)
				{
					throw new ArgumentException("daylightTransitionStart parameter cannot equal daylightTransitionEnd parameter");
				}
				if (dateStart.Ticks % 864000000000L != 0L || dateEnd.Ticks % 864000000000L != 0L)
				{
					throw new ArgumentException("dateStart or dateEnd parameter includes a time of day value");
				}
				if (dateEnd < dateStart)
				{
					throw new ArgumentOutOfRangeException("dateEnd is earlier than dateStart");
				}
				if (daylightDelta > new TimeSpan(14, 0, 0) || daylightDelta < new TimeSpan(-14, 0, 0))
				{
					throw new ArgumentOutOfRangeException("daylightDelta is less than -14 or greater than 14 hours");
				}
				if (daylightDelta.Ticks % 10000000L != 0L)
				{
					throw new ArgumentOutOfRangeException("daylightDelta parameter does not represent a whole number of seconds");
				}
				this.dateStart = dateStart;
				this.dateEnd = dateEnd;
				this.daylightDelta = daylightDelta;
				this.daylightTransitionStart = daylightTransitionStart;
				this.daylightTransitionEnd = daylightTransitionEnd;
			}

			public DateTime DateEnd
			{
				get
				{
					return this.dateEnd;
				}
			}

			public DateTime DateStart
			{
				get
				{
					return this.dateStart;
				}
			}

			public TimeSpan DaylightDelta
			{
				get
				{
					return this.daylightDelta;
				}
			}

			public TimeZoneInfo.TransitionTime DaylightTransitionEnd
			{
				get
				{
					return this.daylightTransitionEnd;
				}
			}

			public TimeZoneInfo.TransitionTime DaylightTransitionStart
			{
				get
				{
					return this.daylightTransitionStart;
				}
			}

			public static TimeZoneInfo.AdjustmentRule CreateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd)
			{
				return new TimeZoneInfo.AdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd);
			}

			public bool Equals(TimeZoneInfo.AdjustmentRule other)
			{
				return this.dateStart == other.dateStart && this.dateEnd == other.dateEnd && this.daylightDelta == other.daylightDelta && this.daylightTransitionStart == other.daylightTransitionStart && this.daylightTransitionEnd == other.daylightTransitionEnd;
			}

			public override int GetHashCode()
			{
				return this.dateStart.GetHashCode() ^ this.dateEnd.GetHashCode() ^ this.daylightDelta.GetHashCode() ^ this.daylightTransitionStart.GetHashCode() ^ this.daylightTransitionEnd.GetHashCode();
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new NotImplementedException();
			}

			public void OnDeserialization(object sender)
			{
				throw new NotImplementedException();
			}

			private DateTime dateEnd;

			private DateTime dateStart;

			private TimeSpan daylightDelta;

			private TimeZoneInfo.TransitionTime daylightTransitionEnd;

			private TimeZoneInfo.TransitionTime daylightTransitionStart;
		}

		private struct TimeType
		{
			public TimeType(int offset, bool is_dst, string abbrev)
			{
				this.Offset = offset;
				this.IsDst = is_dst;
				this.Name = abbrev;
			}

			public override string ToString()
			{
				return string.Concat(new object[] { "offset: ", this.Offset, "s, is_dst: ", this.IsDst, ", zone name: ", this.Name });
			}

			public readonly int Offset;

			public readonly bool IsDst;

			public string Name;
		}

		[Serializable]
		public struct TransitionTime : ISerializable, IDeserializationCallback, IEquatable<TimeZoneInfo.TransitionTime>
		{
			private TransitionTime(DateTime timeOfDay, int month, int day)
			{
				this = new TimeZoneInfo.TransitionTime(timeOfDay, month);
				if (day < 1 || day > 31)
				{
					throw new ArgumentOutOfRangeException("day parameter is less than 1 or greater than 31");
				}
				this.day = day;
				this.isFixedDateRule = true;
			}

			private TransitionTime(DateTime timeOfDay, int month, int week, DayOfWeek dayOfWeek)
			{
				this = new TimeZoneInfo.TransitionTime(timeOfDay, month);
				if (week < 1 || week > 5)
				{
					throw new ArgumentOutOfRangeException("week parameter is less than 1 or greater than 5");
				}
				if (dayOfWeek != DayOfWeek.Sunday && dayOfWeek != DayOfWeek.Monday && dayOfWeek != DayOfWeek.Tuesday && dayOfWeek != DayOfWeek.Wednesday && dayOfWeek != DayOfWeek.Thursday && dayOfWeek != DayOfWeek.Friday && dayOfWeek != DayOfWeek.Saturday)
				{
					throw new ArgumentOutOfRangeException("dayOfWeek parameter is not a member od DayOfWeek enumeration");
				}
				this.week = week;
				this.dayOfWeek = dayOfWeek;
				this.isFixedDateRule = false;
			}

			private TransitionTime(DateTime timeOfDay, int month)
			{
				if (timeOfDay.Year != 1 || timeOfDay.Month != 1 || timeOfDay.Day != 1)
				{
					throw new ArgumentException("timeOfDay parameter has a non-default date component");
				}
				if (timeOfDay.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException("timeOfDay parameter Kind's property is not DateTimeKind.Unspecified");
				}
				if (timeOfDay.Ticks % 10000L != 0L)
				{
					throw new ArgumentException("timeOfDay parameter does not represent a whole number of milliseconds");
				}
				if (month < 1 || month > 12)
				{
					throw new ArgumentOutOfRangeException("month parameter is less than 1 or greater than 12");
				}
				this.timeOfDay = timeOfDay;
				this.month = month;
				this.week = -1;
				this.dayOfWeek = (DayOfWeek)(-1);
				this.day = -1;
				this.isFixedDateRule = false;
			}

			public DateTime TimeOfDay
			{
				get
				{
					return this.timeOfDay;
				}
			}

			public int Month
			{
				get
				{
					return this.month;
				}
			}

			public int Day
			{
				get
				{
					return this.day;
				}
			}

			public int Week
			{
				get
				{
					return this.week;
				}
			}

			public DayOfWeek DayOfWeek
			{
				get
				{
					return this.dayOfWeek;
				}
			}

			public bool IsFixedDateRule
			{
				get
				{
					return this.isFixedDateRule;
				}
			}

			public static TimeZoneInfo.TransitionTime CreateFixedDateRule(DateTime timeOfDay, int month, int day)
			{
				return new TimeZoneInfo.TransitionTime(timeOfDay, month, day);
			}

			public static TimeZoneInfo.TransitionTime CreateFloatingDateRule(DateTime timeOfDay, int month, int week, DayOfWeek dayOfWeek)
			{
				return new TimeZoneInfo.TransitionTime(timeOfDay, month, week, dayOfWeek);
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new NotImplementedException();
			}

			public override bool Equals(object other)
			{
				return other is TimeZoneInfo.TransitionTime && this == (TimeZoneInfo.TransitionTime)other;
			}

			public bool Equals(TimeZoneInfo.TransitionTime other)
			{
				return this == other;
			}

			public override int GetHashCode()
			{
				return this.day ^ (int)this.dayOfWeek ^ this.month ^ (int)this.timeOfDay.Ticks ^ this.week;
			}

			public void OnDeserialization(object sender)
			{
				throw new NotImplementedException();
			}

			public static bool operator ==(TimeZoneInfo.TransitionTime t1, TimeZoneInfo.TransitionTime t2)
			{
				return t1.day == t2.day && t1.dayOfWeek == t2.dayOfWeek && t1.isFixedDateRule == t2.isFixedDateRule && t1.month == t2.month && t1.timeOfDay == t2.timeOfDay && t1.week == t2.week;
			}

			public static bool operator !=(TimeZoneInfo.TransitionTime t1, TimeZoneInfo.TransitionTime t2)
			{
				return !(t1 == t2);
			}

			private DateTime timeOfDay;

			private int month;

			private int day;

			private int week;

			private DayOfWeek dayOfWeek;

			private bool isFixedDateRule;
		}
	}
}
