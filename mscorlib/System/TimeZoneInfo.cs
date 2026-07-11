using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace System
{
	[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=b77a5c561934e089")]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public sealed class TimeZoneInfo : IEquatable<TimeZoneInfo>, ISerializable, IDeserializationCallback
	{
		internal static bool UtcOffsetOutOfRange(TimeSpan offset)
		{
			return offset.TotalHours < -14.0 || offset.TotalHours > 14.0;
		}

		private static void ValidateTimeZoneInfo(string id, TimeSpan baseUtcOffset, TimeZoneInfo.AdjustmentRule[] adjustmentRules, out bool adjustmentRulesSupportDst)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("The specified ID parameter '{0}' is not supported.", new object[] { id }), "id");
			}
			if (TimeZoneInfo.UtcOffsetOutOfRange(baseUtcOffset))
			{
				throw new ArgumentOutOfRangeException("baseUtcOffset", Environment.GetResourceString("The TimeSpan parameter must be within plus or minus 14.0 hours."));
			}
			if (baseUtcOffset.Ticks % 600000000L != 0L)
			{
				throw new ArgumentException(Environment.GetResourceString("The TimeSpan parameter cannot be specified more precisely than whole minutes."), "baseUtcOffset");
			}
			adjustmentRulesSupportDst = false;
			if (adjustmentRules != null && adjustmentRules.Length != 0)
			{
				adjustmentRulesSupportDst = true;
				TimeZoneInfo.AdjustmentRule adjustmentRule = null;
				for (int i = 0; i < adjustmentRules.Length; i++)
				{
					TimeZoneInfo.AdjustmentRule adjustmentRule2 = adjustmentRule;
					adjustmentRule = adjustmentRules[i];
					if (adjustmentRule == null)
					{
						throw new InvalidTimeZoneException(Environment.GetResourceString("The AdjustmentRule array cannot contain null elements."));
					}
					if (TimeZoneInfo.UtcOffsetOutOfRange(baseUtcOffset + adjustmentRule.DaylightDelta))
					{
						throw new InvalidTimeZoneException(Environment.GetResourceString("The sum of the BaseUtcOffset and DaylightDelta properties must within plus or minus 14.0 hours."));
					}
					if (adjustmentRule2 != null && adjustmentRule.DateStart <= adjustmentRule2.DateEnd)
					{
						throw new InvalidTimeZoneException(Environment.GetResourceString("The elements of the AdjustmentRule array must be in chronological order and must not overlap."));
					}
				}
			}
		}

		public static TimeZoneInfo FromSerializedString(string source)
		{
			StringBuilder stringBuilder = new StringBuilder(source);
			string text = TimeZoneInfo.DeserializeString(ref stringBuilder);
			int num = TimeZoneInfo.DeserializeInt(ref stringBuilder);
			string text2 = TimeZoneInfo.DeserializeString(ref stringBuilder);
			string text3 = TimeZoneInfo.DeserializeString(ref stringBuilder);
			string text4 = TimeZoneInfo.DeserializeString(ref stringBuilder);
			List<TimeZoneInfo.AdjustmentRule> list = null;
			while (stringBuilder[0] != ';')
			{
				if (list == null)
				{
					list = new List<TimeZoneInfo.AdjustmentRule>();
				}
				list.Add(TimeZoneInfo.DeserializeAdjustmentRule(ref stringBuilder));
			}
			TimeSpan timeSpan = TimeSpan.FromMinutes((double)num);
			return TimeZoneInfo.CreateCustomTimeZone(text, timeSpan, text2, text3, text4, (list != null) ? list.ToArray() : null);
		}

		public string ToSerializedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = (string.IsNullOrEmpty(this.DaylightName) ? this.StandardName : this.DaylightName);
			stringBuilder.AppendFormat("{0};{1};{2};{3};{4};", new object[]
			{
				TimeZoneInfo.EscapeForSerialization(this.Id),
				(int)this.BaseUtcOffset.TotalMinutes,
				TimeZoneInfo.EscapeForSerialization(this.DisplayName),
				TimeZoneInfo.EscapeForSerialization(this.StandardName),
				TimeZoneInfo.EscapeForSerialization(text)
			});
			if (this.SupportsDaylightSavingTime)
			{
				foreach (TimeZoneInfo.AdjustmentRule adjustmentRule in this.GetAdjustmentRules())
				{
					string text2 = adjustmentRule.DateStart.ToString("MM:dd:yyyy", CultureInfo.InvariantCulture);
					string text3 = adjustmentRule.DateEnd.ToString("MM:dd:yyyy", CultureInfo.InvariantCulture);
					int num = (int)adjustmentRule.DaylightDelta.TotalMinutes;
					string text4 = TimeZoneInfo.SerializeTransitionTime(adjustmentRule.DaylightTransitionStart);
					string text5 = TimeZoneInfo.SerializeTransitionTime(adjustmentRule.DaylightTransitionEnd);
					stringBuilder.AppendFormat("[{0};{1};{2};{3};{4};]", new object[] { text2, text3, num, text4, text5 });
				}
			}
			stringBuilder.Append(";");
			return stringBuilder.ToString();
		}

		private static TimeZoneInfo.AdjustmentRule DeserializeAdjustmentRule(ref StringBuilder input)
		{
			if (input[0] != '[')
			{
				throw new SerializationException();
			}
			input.Remove(0, 1);
			DateTime dateTime = TimeZoneInfo.DeserializeDate(ref input);
			DateTime dateTime2 = TimeZoneInfo.DeserializeDate(ref input);
			double num = (double)TimeZoneInfo.DeserializeInt(ref input);
			TimeZoneInfo.TransitionTime transitionTime = TimeZoneInfo.DeserializeTransitionTime(ref input);
			TimeZoneInfo.TransitionTime transitionTime2 = TimeZoneInfo.DeserializeTransitionTime(ref input);
			input.Remove(0, 1);
			TimeSpan timeSpan = TimeSpan.FromMinutes(num);
			return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateTime, dateTime2, timeSpan, transitionTime, transitionTime2);
		}

		private static TimeZoneInfo.TransitionTime DeserializeTransitionTime(ref StringBuilder input)
		{
			if (input[0] != '[' || (input[1] != '0' && input[1] != '1') || input[2] != ';')
			{
				throw new SerializationException();
			}
			int num = (int)input[1];
			input.Remove(0, 3);
			DateTime dateTime = TimeZoneInfo.DeserializeTime(ref input);
			int num2 = TimeZoneInfo.DeserializeInt(ref input);
			if (num == 48)
			{
				int num3 = TimeZoneInfo.DeserializeInt(ref input);
				int num4 = TimeZoneInfo.DeserializeInt(ref input);
				input.Remove(0, 2);
				return TimeZoneInfo.TransitionTime.CreateFloatingDateRule(dateTime, num2, num3, (DayOfWeek)num4);
			}
			int num5 = TimeZoneInfo.DeserializeInt(ref input);
			input.Remove(0, 2);
			return TimeZoneInfo.TransitionTime.CreateFixedDateRule(dateTime, num2, num5);
		}

		private static string DeserializeString(ref StringBuilder input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			int i;
			for (i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (flag)
				{
					flag = false;
					stringBuilder.Append(c);
				}
				else if (c == '\\')
				{
					flag = true;
				}
				else
				{
					if (c == ';')
					{
						break;
					}
					stringBuilder.Append(c);
				}
			}
			input.Remove(0, i + 1);
			return stringBuilder.ToString();
		}

		private static int DeserializeInt(ref StringBuilder input)
		{
			int num = 0;
			while (num++ < input.Length && input[num] != ';')
			{
			}
			int num2;
			if (!int.TryParse(input.ToString(0, num), NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
			{
				throw new SerializationException();
			}
			input.Remove(0, num + 1);
			return num2;
		}

		private static DateTime DeserializeDate(ref StringBuilder input)
		{
			char[] array = new char[11];
			input.CopyTo(0, array, 0, array.Length);
			DateTime dateTime;
			if (!DateTime.TryParseExact(new string(array), "MM:dd:yyyy;", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
			{
				throw new SerializationException();
			}
			input.Remove(0, array.Length);
			return dateTime;
		}

		private static DateTime DeserializeTime(ref StringBuilder input)
		{
			if (input[8] == ';')
			{
				char[] array = new char[9];
				input.CopyTo(0, array, 0, array.Length);
				DateTime dateTime;
				if (!DateTime.TryParseExact(new string(array), "HH:mm:ss;", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out dateTime))
				{
					throw new SerializationException();
				}
				input.Remove(0, array.Length);
				return dateTime;
			}
			else
			{
				if (input[12] != ';')
				{
					throw new SerializationException();
				}
				char[] array2 = new char[13];
				input.CopyTo(0, array2, 0, array2.Length);
				DateTime dateTime2;
				if (!DateTime.TryParseExact(new string(array2), "HH:mm:ss.fff;", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out dateTime2))
				{
					throw new SerializationException();
				}
				input.Remove(0, array2.Length);
				return dateTime2;
			}
		}

		private static string EscapeForSerialization(string unescaped)
		{
			return unescaped.Replace("\\", "\\\\").Replace(";", "\\;");
		}

		private static string SerializeTransitionTime(TimeZoneInfo.TransitionTime transition)
		{
			string text;
			if (transition.TimeOfDay.Millisecond > 0)
			{
				text = transition.TimeOfDay.ToString("HH:mm:ss.fff");
			}
			else
			{
				text = transition.TimeOfDay.ToString("HH:mm:ss");
			}
			if (transition.IsFixedDateRule)
			{
				return string.Format("[1;{0};{1};{2};]", text, transition.Month, transition.Day);
			}
			return string.Format("[0;{0};{1};{2};{3};]", new object[]
			{
				text,
				transition.Month,
				transition.Week,
				(int)transition.DayOfWeek
			});
		}

		private static TimeZoneInfo.AdjustmentRule CreateAdjustmentRule(int year, out long[] data, out string[] names, string standardNameCurrentYear, string daylightNameCurrentYear)
		{
			if (!CurrentSystemTimeZone.GetTimeZoneData(year, out data, out names))
			{
				return null;
			}
			DateTime dateTime = new DateTime(data[0]);
			DateTime dateTime2 = new DateTime(data[1]);
			TimeSpan timeSpan = new TimeSpan(data[3]);
			if (standardNameCurrentYear != names[0])
			{
				return null;
			}
			if (daylightNameCurrentYear != names[1])
			{
				return null;
			}
			TimeZoneInfo.TransitionTime transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1).Add(dateTime.TimeOfDay), dateTime.Month, dateTime.Day);
			TimeZoneInfo.TransitionTime transitionTime2 = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1).Add(dateTime2.TimeOfDay), dateTime2.Month, dateTime2.Day);
			return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(year, 1, 1), new DateTime(year, 12, DateTime.DaysInMonth(year, 12)), timeSpan, transitionTime, transitionTime2);
		}

		private static TimeZoneInfo CreateLocalUnity()
		{
			int year = DateTime.UtcNow.Year;
			long[] array;
			string[] array2;
			if (!CurrentSystemTimeZone.GetTimeZoneData(year, out array, out array2))
			{
				throw new NotSupportedException("Can't get timezone name.");
			}
			TimeSpan timeSpan = TimeSpan.FromTicks(array[2]);
			string text = "(GMT" + ((timeSpan >= TimeSpan.Zero) ? '+' : '-').ToString() + timeSpan.ToString("hh\\:mm") + ") Local Time";
			string text2 = array2[0];
			string text3 = array2[1];
			List<TimeZoneInfo.AdjustmentRule> list = new List<TimeZoneInfo.AdjustmentRule>();
			bool flag = array[3] <= 0L;
			if (!flag)
			{
				int num = 1971;
				int num2 = 2037;
				for (int i = year; i <= num2; i++)
				{
					TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.CreateAdjustmentRule(i, out array, out array2, text2, text3);
					if (adjustmentRule == null)
					{
						break;
					}
					list.Add(adjustmentRule);
				}
				for (int j = year - 1; j >= num; j--)
				{
					TimeZoneInfo.AdjustmentRule adjustmentRule2 = TimeZoneInfo.CreateAdjustmentRule(j, out array, out array2, text2, text3);
					if (adjustmentRule2 == null)
					{
						break;
					}
					list.Add(adjustmentRule2);
				}
				list.Sort((TimeZoneInfo.AdjustmentRule rule1, TimeZoneInfo.AdjustmentRule rule2) => rule1.DateStart.CompareTo(rule2.DateStart));
			}
			return TimeZoneInfo.CreateCustomTimeZone("Local", timeSpan, text, text2, text3, list.ToArray(), flag);
		}

		[DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
		internal static extern uint EnumDynamicTimeZoneInformation(uint dwIndex, out TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION lpTimeZoneInformation);

		[DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
		internal static extern uint GetDynamicTimeZoneInformation(out TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION pTimeZoneInformation);

		[DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
		internal static extern uint GetDynamicTimeZoneInformationEffectiveYears(ref TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION lpTimeZoneInformation, out uint FirstYear, out uint LastYear);

		[DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
		internal static extern bool GetTimeZoneInformationForYear(ushort wYear, ref TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION pdtzi, out TimeZoneInfo.TIME_ZONE_INFORMATION ptzi);

		internal static TimeZoneInfo.AdjustmentRule CreateAdjustmentRuleFromTimeZoneInformation(ref TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION timeZoneInformation, DateTime startDate, DateTime endDate, int defaultBaseUtcOffset)
		{
			if (timeZoneInformation.TZI.StandardDate.wMonth <= 0)
			{
				if (timeZoneInformation.TZI.Bias == defaultBaseUtcOffset)
				{
					return null;
				}
				return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(startDate, endDate, TimeSpan.Zero, TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue, 1, 1), TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue.AddMilliseconds(1.0), 1, 1), new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.TZI.Bias, 0));
			}
			else
			{
				TimeZoneInfo.TransitionTime transitionTime;
				if (!TimeZoneInfo.TransitionTimeFromTimeZoneInformation(timeZoneInformation, out transitionTime, true))
				{
					return null;
				}
				TimeZoneInfo.TransitionTime transitionTime2;
				if (!TimeZoneInfo.TransitionTimeFromTimeZoneInformation(timeZoneInformation, out transitionTime2, false))
				{
					return null;
				}
				if (transitionTime.Equals(transitionTime2))
				{
					return null;
				}
				return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(startDate, endDate, new TimeSpan(0, -timeZoneInformation.TZI.DaylightBias, 0), transitionTime, transitionTime2, new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.TZI.Bias, 0));
			}
		}

		private static bool TransitionTimeFromTimeZoneInformation(TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION timeZoneInformation, out TimeZoneInfo.TransitionTime transitionTime, bool readStartDate)
		{
			if (timeZoneInformation.TZI.StandardDate.wMonth <= 0)
			{
				transitionTime = default(TimeZoneInfo.TransitionTime);
				return false;
			}
			if (readStartDate)
			{
				if (timeZoneInformation.TZI.DaylightDate.wYear == 0)
				{
					transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.TZI.DaylightDate.wHour, (int)timeZoneInformation.TZI.DaylightDate.wMinute, (int)timeZoneInformation.TZI.DaylightDate.wSecond, (int)timeZoneInformation.TZI.DaylightDate.wMilliseconds), (int)timeZoneInformation.TZI.DaylightDate.wMonth, (int)timeZoneInformation.TZI.DaylightDate.wDay, (DayOfWeek)timeZoneInformation.TZI.DaylightDate.wDayOfWeek);
				}
				else
				{
					transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.TZI.DaylightDate.wHour, (int)timeZoneInformation.TZI.DaylightDate.wMinute, (int)timeZoneInformation.TZI.DaylightDate.wSecond, (int)timeZoneInformation.TZI.DaylightDate.wMilliseconds), (int)timeZoneInformation.TZI.DaylightDate.wMonth, (int)timeZoneInformation.TZI.DaylightDate.wDay);
				}
			}
			else if (timeZoneInformation.TZI.StandardDate.wYear == 0)
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.TZI.StandardDate.wHour, (int)timeZoneInformation.TZI.StandardDate.wMinute, (int)timeZoneInformation.TZI.StandardDate.wSecond, (int)timeZoneInformation.TZI.StandardDate.wMilliseconds), (int)timeZoneInformation.TZI.StandardDate.wMonth, (int)timeZoneInformation.TZI.StandardDate.wDay, (DayOfWeek)timeZoneInformation.TZI.StandardDate.wDayOfWeek);
			}
			else
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.TZI.StandardDate.wHour, (int)timeZoneInformation.TZI.StandardDate.wMinute, (int)timeZoneInformation.TZI.StandardDate.wSecond, (int)timeZoneInformation.TZI.StandardDate.wMilliseconds), (int)timeZoneInformation.TZI.StandardDate.wMonth, (int)timeZoneInformation.TZI.StandardDate.wDay);
			}
			return true;
		}

		internal static TimeZoneInfo TryCreateTimeZone(TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION timeZoneInformation)
		{
			uint num = 0U;
			uint num2 = 0U;
			TimeZoneInfo.AdjustmentRule[] array = null;
			int bias = timeZoneInformation.TZI.Bias;
			if (string.IsNullOrEmpty(timeZoneInformation.TimeZoneKeyName))
			{
				return null;
			}
			try
			{
				if (TimeZoneInfo.GetDynamicTimeZoneInformationEffectiveYears(ref timeZoneInformation, out num, out num2) != 0U)
				{
					num2 = (num = 0U);
				}
			}
			catch
			{
				num2 = (num = 0U);
			}
			if (num == num2)
			{
				TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(ref timeZoneInformation, DateTime.MinValue.Date, DateTime.MaxValue.Date, bias);
				if (adjustmentRule != null)
				{
					array = new TimeZoneInfo.AdjustmentRule[] { adjustmentRule };
				}
			}
			else
			{
				TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION dynamic_TIME_ZONE_INFORMATION = default(TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION);
				List<TimeZoneInfo.AdjustmentRule> list = new List<TimeZoneInfo.AdjustmentRule>();
				if (!TimeZoneInfo.GetTimeZoneInformationForYear((ushort)num, ref timeZoneInformation, out dynamic_TIME_ZONE_INFORMATION.TZI))
				{
					return null;
				}
				TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(ref dynamic_TIME_ZONE_INFORMATION, DateTime.MinValue.Date, new DateTime((int)num, 12, 31), bias);
				if (adjustmentRule != null)
				{
					list.Add(adjustmentRule);
				}
				for (uint num3 = num + 1U; num3 < num2; num3 += 1U)
				{
					if (!TimeZoneInfo.GetTimeZoneInformationForYear((ushort)num3, ref timeZoneInformation, out dynamic_TIME_ZONE_INFORMATION.TZI))
					{
						return null;
					}
					adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(ref dynamic_TIME_ZONE_INFORMATION, new DateTime((int)num3, 1, 1), new DateTime((int)num3, 12, 31), bias);
					if (adjustmentRule != null)
					{
						list.Add(adjustmentRule);
					}
				}
				if (!TimeZoneInfo.GetTimeZoneInformationForYear((ushort)num2, ref timeZoneInformation, out dynamic_TIME_ZONE_INFORMATION.TZI))
				{
					return null;
				}
				adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(ref dynamic_TIME_ZONE_INFORMATION, new DateTime((int)num2, 1, 1), DateTime.MaxValue.Date, bias);
				if (adjustmentRule != null)
				{
					list.Add(adjustmentRule);
				}
				if (list.Count > 0)
				{
					array = list.ToArray();
				}
			}
			return new TimeZoneInfo(timeZoneInformation.TimeZoneKeyName, new TimeSpan(0, -timeZoneInformation.TZI.Bias, 0), timeZoneInformation.TZI.StandardName, timeZoneInformation.TZI.StandardName, timeZoneInformation.TZI.DaylightName, array, false);
		}

		internal static TimeZoneInfo GetLocalTimeZoneInfoWinRTFallback()
		{
			TimeZoneInfo timeZoneInfo;
			try
			{
				TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION dynamic_TIME_ZONE_INFORMATION;
				if (TimeZoneInfo.GetDynamicTimeZoneInformation(out dynamic_TIME_ZONE_INFORMATION) == 4294967295U)
				{
					timeZoneInfo = TimeZoneInfo.Utc;
				}
				else
				{
					TimeZoneInfo timeZoneInfo2 = TimeZoneInfo.TryCreateTimeZone(dynamic_TIME_ZONE_INFORMATION);
					timeZoneInfo = ((timeZoneInfo2 != null) ? timeZoneInfo2 : TimeZoneInfo.Utc);
				}
			}
			catch
			{
				timeZoneInfo = TimeZoneInfo.Utc;
			}
			return timeZoneInfo;
		}

		internal static TimeZoneInfo FindSystemTimeZoneByIdWinRTFallback(string id)
		{
			foreach (TimeZoneInfo timeZoneInfo in TimeZoneInfo.GetSystemTimeZones())
			{
				if (string.Compare(id, timeZoneInfo.Id, StringComparison.Ordinal) == 0)
				{
					return timeZoneInfo;
				}
			}
			throw new TimeZoneNotFoundException();
		}

		internal static List<TimeZoneInfo> GetSystemTimeZonesWinRTFallback()
		{
			List<TimeZoneInfo> list = new List<TimeZoneInfo>();
			try
			{
				uint num = 0U;
				TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION dynamic_TIME_ZONE_INFORMATION;
				while (TimeZoneInfo.EnumDynamicTimeZoneInformation(num++, out dynamic_TIME_ZONE_INFORMATION) != 259U)
				{
					TimeZoneInfo timeZoneInfo = TimeZoneInfo.TryCreateTimeZone(dynamic_TIME_ZONE_INFORMATION);
					if (timeZoneInfo != null)
					{
						list.Add(timeZoneInfo);
					}
				}
			}
			catch
			{
			}
			if (list.Count == 0)
			{
				list.Add(TimeZoneInfo.Local);
			}
			return list;
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
				if (!this.supportsDaylightSavingTime)
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
				TimeZoneInfo timeZoneInfo = TimeZoneInfo.local;
				if (timeZoneInfo == null)
				{
					timeZoneInfo = TimeZoneInfo.CreateLocal();
					if (timeZoneInfo == null)
					{
						throw new TimeZoneNotFoundException();
					}
					if (Interlocked.CompareExchange<TimeZoneInfo>(ref TimeZoneInfo.local, timeZoneInfo, null) != null)
					{
						timeZoneInfo = TimeZoneInfo.local;
					}
				}
				return timeZoneInfo;
			}
		}

		[DllImport("libc")]
		private static extern int readlink(string path, byte[] buffer, int buflen);

		private static string readlink(string path)
		{
			if (TimeZoneInfo.readlinkNotFound)
			{
				return null;
			}
			byte[] array = new byte[512];
			int num;
			try
			{
				num = TimeZoneInfo.readlink(path, array, array.Length);
			}
			catch (DllNotFoundException)
			{
				TimeZoneInfo.readlinkNotFound = true;
				return null;
			}
			catch (EntryPointNotFoundException)
			{
				TimeZoneInfo.readlinkNotFound = true;
				return null;
			}
			if (num == -1)
			{
				return null;
			}
			char[] array2 = new char[512];
			int chars = Encoding.Default.GetChars(array, 0, num, array2, 0);
			return new string(array2, 0, chars);
		}

		private static bool TryGetNameFromPath(string path, out string name)
		{
			name = null;
			if (!File.Exists(path))
			{
				return false;
			}
			string text = TimeZoneInfo.readlink(path);
			if (text != null)
			{
				if (Path.IsPathRooted(text))
				{
					path = text;
				}
				else
				{
					path = Path.Combine(Path.GetDirectoryName(path), text);
				}
			}
			path = Path.GetFullPath(path);
			if (string.IsNullOrEmpty(TimeZoneInfo.TimeZoneDirectory))
			{
				return false;
			}
			string text2 = TimeZoneInfo.TimeZoneDirectory;
			if (text2[text2.Length - 1] != Path.DirectorySeparatorChar)
			{
				text2 += Path.DirectorySeparatorChar.ToString();
			}
			if (!path.StartsWith(text2, StringComparison.InvariantCulture))
			{
				return false;
			}
			name = path.Substring(text2.Length);
			if (name == "localtime")
			{
				name = "Local";
			}
			return true;
		}

		private static TimeZoneInfo CreateLocal()
		{
			if (TimeZoneInfo.IsWindows && TimeZoneInfo.LocalZoneKey != null)
			{
				string text = (string)TimeZoneInfo.LocalZoneKey.GetValue("TimeZoneKeyName");
				if (text == null)
				{
					text = (string)TimeZoneInfo.LocalZoneKey.GetValue("StandardName");
				}
				text = TimeZoneInfo.TrimSpecial(text);
				if (text != null)
				{
					return TimeZoneInfo.FindSystemTimeZoneById(text);
				}
			}
			else if (TimeZoneInfo.IsWindows)
			{
				return TimeZoneInfo.GetLocalTimeZoneInfoWinRTFallback();
			}
			TimeZoneInfo timeZoneInfo = null;
			try
			{
				timeZoneInfo = TimeZoneInfo.CreateLocalUnity();
			}
			catch
			{
				timeZoneInfo = null;
			}
			if (timeZoneInfo == null)
			{
				timeZoneInfo = TimeZoneInfo.Utc;
			}
			string environmentVariable = Environment.GetEnvironmentVariable("TZ");
			if (environmentVariable != null)
			{
				if (environmentVariable == string.Empty)
				{
					return timeZoneInfo;
				}
				try
				{
					return TimeZoneInfo.FindSystemTimeZoneByFileName(environmentVariable, Path.Combine(TimeZoneInfo.TimeZoneDirectory, environmentVariable));
				}
				catch
				{
					return timeZoneInfo;
				}
			}
			foreach (string text2 in new string[]
			{
				"/etc/localtime",
				Path.Combine(TimeZoneInfo.TimeZoneDirectory, "localtime")
			})
			{
				try
				{
					string text3 = null;
					if (!TimeZoneInfo.TryGetNameFromPath(text2, out text3))
					{
						text3 = "Local";
					}
					return TimeZoneInfo.FindSystemTimeZoneByFileName(text3, text2);
				}
				catch (TimeZoneNotFoundException)
				{
				}
			}
			return timeZoneInfo;
		}

		private static TimeZoneInfo FindSystemTimeZoneByIdCore(string id)
		{
			string text = Path.Combine(TimeZoneInfo.TimeZoneDirectory, id);
			return TimeZoneInfo.FindSystemTimeZoneByFileName(id, text);
		}

		private static void GetSystemTimeZonesCore(List<TimeZoneInfo> systemTimeZones)
		{
			if (TimeZoneInfo.TimeZoneKey != null)
			{
				string[] array = TimeZoneInfo.TimeZoneKey.GetSubKeyNames();
				int i = 0;
				while (i < array.Length)
				{
					string text = array[i];
					using (RegistryKey registryKey = TimeZoneInfo.TimeZoneKey.OpenSubKey(text))
					{
						if (registryKey == null || registryKey.GetValue("TZI") == null)
						{
							goto IL_0050;
						}
					}
					goto IL_0044;
					IL_0050:
					i++;
					continue;
					IL_0044:
					systemTimeZones.Add(TimeZoneInfo.FindSystemTimeZoneById(text));
					goto IL_0050;
				}
				return;
			}
			if (TimeZoneInfo.IsWindows)
			{
				systemTimeZones.AddRange(TimeZoneInfo.GetSystemTimeZonesWinRTFallback());
				return;
			}
			foreach (string text2 in new string[]
			{
				"Africa", "America", "Antarctica", "Arctic", "Asia", "Atlantic", "Australia", "Brazil", "Canada", "Chile",
				"Europe", "Indian", "Mexico", "Mideast", "Pacific", "US"
			})
			{
				try
				{
					foreach (string text3 in Directory.GetFiles(Path.Combine(TimeZoneInfo.TimeZoneDirectory, text2)))
					{
						try
						{
							string text4 = string.Format("{0}/{1}", text2, Path.GetFileName(text3));
							systemTimeZones.Add(TimeZoneInfo.FindSystemTimeZoneById(text4));
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
				return this.supportsDaylightSavingTime;
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

		private static bool IsWindows
		{
			get
			{
				int platform = (int)Environment.OSVersion.Platform;
				return platform != 4 && platform != 6 && platform != 128;
			}
		}

		private static string TrimSpecial(string str)
		{
			if (str == null)
			{
				return str;
			}
			int num = 0;
			while (num < str.Length && !char.IsLetterOrDigit(str[num]))
			{
				num++;
			}
			int num2 = str.Length - 1;
			while (num2 > num && !char.IsLetterOrDigit(str[num2]) && str[num2] != ')')
			{
				num2--;
			}
			return str.Substring(num, num2 - num + 1);
		}

		private static RegistryKey TimeZoneKey
		{
			get
			{
				if (TimeZoneInfo.timeZoneKey != null)
				{
					return TimeZoneInfo.timeZoneKey;
				}
				if (!TimeZoneInfo.IsWindows)
				{
					return null;
				}
				RegistryKey registryKey;
				try
				{
					registryKey = (TimeZoneInfo.timeZoneKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones", false));
				}
				catch
				{
					registryKey = null;
				}
				return registryKey;
			}
		}

		private static RegistryKey LocalZoneKey
		{
			get
			{
				if (TimeZoneInfo.localZoneKey != null)
				{
					return TimeZoneInfo.localZoneKey;
				}
				if (!TimeZoneInfo.IsWindows)
				{
					return null;
				}
				RegistryKey registryKey;
				try
				{
					registryKey = (TimeZoneInfo.localZoneKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation", false));
				}
				catch
				{
					registryKey = null;
				}
				return registryKey;
			}
		}

		private static bool TryAddTicks(DateTime date, long ticks, out DateTime result, DateTimeKind kind = DateTimeKind.Unspecified)
		{
			long num = date.Ticks + ticks;
			if (num < DateTime.MinValue.Ticks)
			{
				result = DateTime.SpecifyKind(DateTime.MinValue, kind);
				return false;
			}
			if (num > DateTime.MaxValue.Ticks)
			{
				result = DateTime.SpecifyKind(DateTime.MaxValue, kind);
				return false;
			}
			result = new DateTime(num, kind);
			return true;
		}

		public static void ClearCachedData()
		{
			TimeZoneInfo.local = null;
			TimeZoneInfo.utc = null;
			TimeZoneInfo.systemTimeZones = null;
		}

		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo destinationTimeZone)
		{
			return TimeZoneInfo.ConvertTime(dateTime, (dateTime.Kind == DateTimeKind.Utc) ? TimeZoneInfo.Utc : TimeZoneInfo.Local, destinationTimeZone);
		}

		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
		{
			if (sourceTimeZone == null)
			{
				throw new ArgumentNullException("sourceTimeZone");
			}
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			if (dateTime.Kind == DateTimeKind.Local && sourceTimeZone != TimeZoneInfo.Local)
			{
				throw new ArgumentException("Kind property of dateTime is Local but the sourceTimeZone does not equal TimeZoneInfo.Local");
			}
			if (dateTime.Kind == DateTimeKind.Utc && sourceTimeZone != TimeZoneInfo.Utc)
			{
				throw new ArgumentException("Kind property of dateTime is Utc but the sourceTimeZone does not equal TimeZoneInfo.Utc");
			}
			if (sourceTimeZone.IsInvalidTime(dateTime))
			{
				throw new ArgumentException("dateTime parameter is an invalid time");
			}
			if (dateTime.Kind == DateTimeKind.Local && sourceTimeZone == TimeZoneInfo.Local && destinationTimeZone == TimeZoneInfo.Local)
			{
				return dateTime;
			}
			DateTime dateTime2 = TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
			if (destinationTimeZone != TimeZoneInfo.Utc)
			{
				dateTime2 = TimeZoneInfo.ConvertTimeFromUtc(dateTime2, destinationTimeZone);
				if (dateTime.Kind == DateTimeKind.Unspecified)
				{
					return DateTime.SpecifyKind(dateTime2, DateTimeKind.Unspecified);
				}
			}
			return dateTime2;
		}

		public static DateTimeOffset ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone)
		{
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			DateTime utcDateTime = dateTimeOffset.UtcDateTime;
			bool flag;
			TimeSpan utcOffset = destinationTimeZone.GetUtcOffset(utcDateTime, out flag);
			return new DateTimeOffset(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Unspecified) + utcOffset, utcOffset);
		}

		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string destinationTimeZoneId)
		{
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string sourceTimeZoneId, string destinationTimeZoneId)
		{
			TimeZoneInfo timeZoneInfo;
			if (dateTime.Kind == DateTimeKind.Utc && sourceTimeZoneId == TimeZoneInfo.Utc.Id)
			{
				timeZoneInfo = TimeZoneInfo.Utc;
			}
			else
			{
				timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
			}
			return TimeZoneInfo.ConvertTime(dateTime, timeZoneInfo, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
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
			TimeSpan utcOffset = this.GetUtcOffset(dateTime);
			DateTimeKind dateTimeKind = ((this == TimeZoneInfo.Local) ? DateTimeKind.Local : DateTimeKind.Unspecified);
			DateTime dateTime2;
			if (!TimeZoneInfo.TryAddTicks(dateTime, utcOffset.Ticks, out dateTime2, dateTimeKind))
			{
				return DateTime.SpecifyKind(DateTime.MaxValue, dateTimeKind);
			}
			return dateTime2;
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
			return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local);
		}

		internal static DateTime ConvertTimeToUtc(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local, flags);
		}

		public static DateTime ConvertTimeToUtc(DateTime dateTime, TimeZoneInfo sourceTimeZone)
		{
			return TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone, TimeZoneInfoOptions.None);
		}

		private static DateTime ConvertTimeToUtc(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfoOptions flags)
		{
			if ((flags & TimeZoneInfoOptions.NoThrowOnInvalidTime) == (TimeZoneInfoOptions)0)
			{
				if (sourceTimeZone == null)
				{
					throw new ArgumentNullException("sourceTimeZone");
				}
				if (dateTime.Kind == DateTimeKind.Utc && sourceTimeZone != TimeZoneInfo.Utc)
				{
					throw new ArgumentException("Kind property of dateTime is Utc but the sourceTimeZone does not equal TimeZoneInfo.Utc");
				}
				if (dateTime.Kind == DateTimeKind.Local && sourceTimeZone != TimeZoneInfo.Local)
				{
					throw new ArgumentException("Kind property of dateTime is Local but the sourceTimeZone does not equal TimeZoneInfo.Local");
				}
				if (sourceTimeZone.IsInvalidTime(dateTime))
				{
					throw new ArgumentException("dateTime parameter is an invalid time");
				}
			}
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime;
			}
			bool flag;
			TimeSpan utcOffset = sourceTimeZone.GetUtcOffset(dateTime, out flag);
			DateTime dateTime2;
			TimeZoneInfo.TryAddTicks(dateTime, -utcOffset.Ticks, out dateTime2, DateTimeKind.Utc);
			return dateTime2;
		}

		internal static TimeSpan GetDateTimeNowUtcOffsetFromUtc(DateTime time, out bool isAmbiguousLocalDst)
		{
			bool flag;
			return TimeZoneInfo.GetUtcOffsetFromUtc(time, TimeZoneInfo.Local, out flag, out isAmbiguousLocalDst);
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

		public override bool Equals(object obj)
		{
			return this.Equals(obj as TimeZoneInfo);
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
			if (TimeZoneInfo.TimeZoneKey != null)
			{
				if (id == "Coordinated Universal Time")
				{
					id = "UTC";
				}
				RegistryKey registryKey = TimeZoneInfo.TimeZoneKey.OpenSubKey(id, false);
				if (registryKey == null)
				{
					throw new TimeZoneNotFoundException();
				}
				return TimeZoneInfo.FromRegistryKey(id, registryKey);
			}
			else
			{
				if (TimeZoneInfo.IsWindows)
				{
					return TimeZoneInfo.FindSystemTimeZoneByIdWinRTFallback(id);
				}
				if (id == "Local")
				{
					return TimeZoneInfo.Local;
				}
				return TimeZoneInfo.FindSystemTimeZoneByIdCore(id);
			}
		}

		private static TimeZoneInfo FindSystemTimeZoneByFileName(string id, string filepath)
		{
			FileStream fileStream = null;
			try
			{
				fileStream = File.OpenRead(filepath);
			}
			catch (Exception ex)
			{
				throw new TimeZoneNotFoundException("Couldn't read time zone file " + filepath, ex);
			}
			TimeZoneInfo timeZoneInfo;
			try
			{
				timeZoneInfo = TimeZoneInfo.BuildFromStream(id, fileStream);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Dispose();
				}
			}
			return timeZoneInfo;
		}

		private static TimeZoneInfo FromRegistryKey(string id, RegistryKey key)
		{
			byte[] array = (byte[])key.GetValue("TZI");
			if (array == null)
			{
				throw new InvalidTimeZoneException();
			}
			int num = BitConverter.ToInt32(array, 0);
			TimeSpan timeSpan = new TimeSpan(0, -num, 0);
			string text = (string)key.GetValue("Display");
			string text2 = (string)key.GetValue("Std");
			string text3 = (string)key.GetValue("Dlt");
			List<TimeZoneInfo.AdjustmentRule> list = new List<TimeZoneInfo.AdjustmentRule>();
			RegistryKey registryKey = key.OpenSubKey("Dynamic DST", false);
			if (registryKey != null)
			{
				int num2 = (int)registryKey.GetValue("FirstEntry");
				int num3 = (int)registryKey.GetValue("LastEntry");
				for (int i = num2; i <= num3; i++)
				{
					byte[] array2 = (byte[])registryKey.GetValue(i.ToString());
					if (array2 != null)
					{
						int num4 = ((i == num2) ? 1 : i);
						int num5 = ((i == num3) ? 9999 : i);
						TimeZoneInfo.ParseRegTzi(list, num4, num5, array2);
					}
				}
			}
			else
			{
				TimeZoneInfo.ParseRegTzi(list, 1, 9999, array);
			}
			return TimeZoneInfo.CreateCustomTimeZone(id, timeSpan, text, text2, text3, TimeZoneInfo.ValidateRules(list));
		}

		private static void ParseRegTzi(List<TimeZoneInfo.AdjustmentRule> adjustmentRules, int start_year, int end_year, byte[] buffer)
		{
			int num = BitConverter.ToInt32(buffer, 8);
			int num2 = (int)BitConverter.ToInt16(buffer, 12);
			int num3 = (int)BitConverter.ToInt16(buffer, 14);
			int num4 = (int)BitConverter.ToInt16(buffer, 16);
			int num5 = (int)BitConverter.ToInt16(buffer, 18);
			int num6 = (int)BitConverter.ToInt16(buffer, 20);
			int num7 = (int)BitConverter.ToInt16(buffer, 22);
			int num8 = (int)BitConverter.ToInt16(buffer, 24);
			int num9 = (int)BitConverter.ToInt16(buffer, 26);
			int num10 = (int)BitConverter.ToInt16(buffer, 28);
			int num11 = (int)BitConverter.ToInt16(buffer, 30);
			int num12 = (int)BitConverter.ToInt16(buffer, 32);
			int num13 = (int)BitConverter.ToInt16(buffer, 34);
			int num14 = (int)BitConverter.ToInt16(buffer, 36);
			int num15 = (int)BitConverter.ToInt16(buffer, 38);
			int num16 = (int)BitConverter.ToInt16(buffer, 40);
			int num17 = (int)BitConverter.ToInt16(buffer, 42);
			if (num3 == 0 || num11 == 0)
			{
				return;
			}
			DateTime dateTime = new DateTime(1, 1, 1, num14, num15, num16, num17);
			DateTime dateTime2 = new DateTime(start_year, 1, 1);
			TimeZoneInfo.TransitionTime transitionTime;
			if (num10 == 0)
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(dateTime, num11, num13, (DayOfWeek)num12);
			}
			else
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(dateTime, num11, num13);
			}
			DateTime dateTime3 = new DateTime(1, 1, 1, num6, num7, num8, num9);
			DateTime dateTime4 = new DateTime(end_year, 12, 31);
			TimeZoneInfo.TransitionTime transitionTime2;
			if (num2 == 0)
			{
				transitionTime2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(dateTime3, num3, num5, (DayOfWeek)num4);
			}
			else
			{
				transitionTime2 = TimeZoneInfo.TransitionTime.CreateFixedDateRule(dateTime3, num3, num5);
			}
			TimeSpan timeSpan = new TimeSpan(0, -num, 0);
			adjustmentRules.Add(TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateTime2, dateTime4, timeSpan, transitionTime, transitionTime2));
		}

		public TimeZoneInfo.AdjustmentRule[] GetAdjustmentRules()
		{
			if (!this.supportsDaylightSavingTime || this.adjustmentRules == null)
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
			if (applicableRule != null)
			{
				return new TimeSpan[]
				{
					this.baseUtcOffset,
					this.baseUtcOffset + applicableRule.DaylightDelta
				};
			}
			return new TimeSpan[] { this.baseUtcOffset, this.baseUtcOffset };
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

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Id", this.id);
			info.AddValue("DisplayName", this.displayName);
			info.AddValue("StandardName", this.standardDisplayName);
			info.AddValue("DaylightName", this.daylightDisplayName);
			info.AddValue("BaseUtcOffset", this.baseUtcOffset);
			info.AddValue("AdjustmentRules", this.adjustmentRules);
			info.AddValue("SupportsDaylightSavingTime", this.SupportsDaylightSavingTime);
		}

		public static ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
		{
			if (TimeZoneInfo.systemTimeZones == null)
			{
				List<TimeZoneInfo> list = new List<TimeZoneInfo>();
				TimeZoneInfo.GetSystemTimeZonesCore(list);
				Interlocked.CompareExchange<ReadOnlyCollection<TimeZoneInfo>>(ref TimeZoneInfo.systemTimeZones, new ReadOnlyCollection<TimeZoneInfo>(list), null);
			}
			return TimeZoneInfo.systemTimeZones;
		}

		public TimeSpan GetUtcOffset(DateTime dateTime)
		{
			bool flag;
			return this.GetUtcOffset(dateTime, out flag);
		}

		public TimeSpan GetUtcOffset(DateTimeOffset dateTimeOffset)
		{
			bool flag;
			return this.GetUtcOffset(dateTimeOffset.UtcDateTime, out flag);
		}

		private TimeSpan GetUtcOffset(DateTime dateTime, out bool isDST)
		{
			isDST = false;
			TimeZoneInfo timeZoneInfo = this;
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				timeZoneInfo = TimeZoneInfo.Utc;
			}
			if (dateTime.Kind == DateTimeKind.Local)
			{
				timeZoneInfo = TimeZoneInfo.Local;
			}
			bool flag;
			TimeSpan utcOffsetHelper = TimeZoneInfo.GetUtcOffsetHelper(dateTime, timeZoneInfo, out flag);
			if (timeZoneInfo == this)
			{
				isDST = flag;
				return utcOffsetHelper;
			}
			DateTime dateTime2;
			if (!TimeZoneInfo.TryAddTicks(dateTime, -utcOffsetHelper.Ticks, out dateTime2, DateTimeKind.Utc))
			{
				return this.BaseUtcOffset;
			}
			return TimeZoneInfo.GetUtcOffsetHelper(dateTime2, this, out isDST);
		}

		private static TimeSpan GetUtcOffsetHelper(DateTime dateTime, TimeZoneInfo tz, out bool isDST)
		{
			if (dateTime.Kind == DateTimeKind.Local && tz != TimeZoneInfo.Local)
			{
				throw new Exception();
			}
			isDST = false;
			if (tz == TimeZoneInfo.Utc)
			{
				return TimeSpan.Zero;
			}
			TimeSpan timeSpan;
			if (tz.TryGetTransitionOffset(dateTime, out timeSpan, out isDST))
			{
				return timeSpan;
			}
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				TimeZoneInfo.AdjustmentRule applicableRule = tz.GetApplicableRule(dateTime);
				if (applicableRule != null && tz.IsInDST(applicableRule, dateTime))
				{
					isDST = true;
					return tz.BaseUtcOffset + applicableRule.DaylightDelta;
				}
				return tz.BaseUtcOffset;
			}
			else
			{
				DateTime dateTime2;
				if (!TimeZoneInfo.TryAddTicks(dateTime, -tz.BaseUtcOffset.Ticks, out dateTime2, DateTimeKind.Utc))
				{
					return tz.BaseUtcOffset;
				}
				TimeZoneInfo.AdjustmentRule applicableRule2 = tz.GetApplicableRule(dateTime2);
				DateTime minValue = DateTime.MinValue;
				if (applicableRule2 != null && !TimeZoneInfo.TryAddTicks(dateTime2, -applicableRule2.DaylightDelta.Ticks, out minValue, DateTimeKind.Utc))
				{
					return tz.BaseUtcOffset;
				}
				if (applicableRule2 == null || !tz.IsInDST(applicableRule2, dateTime))
				{
					return tz.BaseUtcOffset;
				}
				isDST = true;
				if (tz.IsInDST(applicableRule2, minValue))
				{
					return tz.BaseUtcOffset + applicableRule2.DaylightDelta;
				}
				return tz.BaseUtcOffset;
			}
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
			if (applicableRule != null)
			{
				DateTime dateTime2 = TimeZoneInfo.TransitionPoint(applicableRule.DaylightTransitionEnd, dateTime.Year);
				if (dateTime > dateTime2 - applicableRule.DaylightDelta && dateTime <= dateTime2)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsAmbiguousTime(DateTimeOffset dateTimeOffset)
		{
			throw new NotImplementedException();
		}

		private bool IsInDST(TimeZoneInfo.AdjustmentRule rule, DateTime dateTime)
		{
			return this.IsInDSTForYear(rule, dateTime, dateTime.Year) || (dateTime.Year > 1 && this.IsInDSTForYear(rule, dateTime, dateTime.Year - 1));
		}

		private bool IsInDSTForYear(TimeZoneInfo.AdjustmentRule rule, DateTime dateTime, int year)
		{
			DateTime dateTime2 = TimeZoneInfo.TransitionPoint(rule.DaylightTransitionStart, year);
			DateTime dateTime3 = TimeZoneInfo.TransitionPoint(rule.DaylightTransitionEnd, year + ((rule.DaylightTransitionStart.Month < rule.DaylightTransitionEnd.Month) ? 0 : 1));
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				dateTime2 -= this.BaseUtcOffset;
				dateTime3 -= this.BaseUtcOffset + rule.DaylightDelta;
			}
			return dateTime >= dateTime2 && dateTime < dateTime3;
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
			bool flag;
			this.GetUtcOffset(dateTime, out flag);
			return flag;
		}

		internal bool IsDaylightSavingTime(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			return this.IsDaylightSavingTime(dateTime);
		}

		public bool IsDaylightSavingTime(DateTimeOffset dateTimeOffset)
		{
			return this.IsDaylightSavingTime(dateTimeOffset.DateTime);
		}

		internal DaylightTime GetDaylightChanges(int year)
		{
			DateTime dateTime = DateTime.MinValue;
			DateTime dateTime2 = DateTime.MinValue;
			TimeSpan timeSpan = default(TimeSpan);
			if (this.transitions != null)
			{
				dateTime2 = DateTime.MaxValue;
				for (int i = this.transitions.Count - 1; i >= 0; i--)
				{
					KeyValuePair<DateTime, TimeType> keyValuePair = this.transitions[i];
					DateTime key = keyValuePair.Key;
					TimeType value = keyValuePair.Value;
					if (key.Year <= year)
					{
						if (key.Year < year)
						{
							break;
						}
						if (value.IsDst)
						{
							timeSpan = new TimeSpan(0, 0, value.Offset) - this.BaseUtcOffset;
							dateTime = key;
						}
						else
						{
							dateTime2 = key;
						}
					}
				}
				if (!TimeZoneInfo.TryAddTicks(dateTime, this.BaseUtcOffset.Ticks, out dateTime, DateTimeKind.Unspecified))
				{
					dateTime = DateTime.MinValue;
				}
				if (!TimeZoneInfo.TryAddTicks(dateTime2, this.BaseUtcOffset.Ticks + timeSpan.Ticks, out dateTime2, DateTimeKind.Unspecified))
				{
					dateTime2 = DateTime.MinValue;
				}
			}
			else
			{
				TimeZoneInfo.AdjustmentRule adjustmentRule = null;
				TimeZoneInfo.AdjustmentRule adjustmentRule2 = null;
				foreach (TimeZoneInfo.AdjustmentRule adjustmentRule3 in this.GetAdjustmentRules())
				{
					if (adjustmentRule3.DateStart.Year <= year && adjustmentRule3.DateEnd.Year >= year)
					{
						if (adjustmentRule3.DateStart.Year <= year && (adjustmentRule == null || adjustmentRule3.DateStart.Year > adjustmentRule.DateStart.Year))
						{
							adjustmentRule = adjustmentRule3;
						}
						if (adjustmentRule3.DateEnd.Year >= year && (adjustmentRule2 == null || adjustmentRule3.DateEnd.Year < adjustmentRule2.DateEnd.Year))
						{
							adjustmentRule2 = adjustmentRule3;
						}
					}
				}
				if (adjustmentRule == null || adjustmentRule2 == null)
				{
					return new DaylightTime(default(DateTime), default(DateTime), default(TimeSpan));
				}
				dateTime = TimeZoneInfo.TransitionPoint(adjustmentRule.DaylightTransitionStart, year);
				dateTime2 = TimeZoneInfo.TransitionPoint(adjustmentRule2.DaylightTransitionEnd, year);
				timeSpan = adjustmentRule.DaylightDelta;
			}
			if (dateTime == DateTime.MinValue || dateTime2 == DateTime.MinValue)
			{
				return new DaylightTime(default(DateTime), default(DateTime), default(TimeSpan));
			}
			return new DaylightTime(dateTime, dateTime2, timeSpan);
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
			if (applicableRule != null)
			{
				DateTime dateTime2 = TimeZoneInfo.TransitionPoint(applicableRule.DaylightTransitionStart, dateTime.Year);
				if (dateTime >= dateTime2 && dateTime < dateTime2 + applicableRule.DaylightDelta)
				{
					return true;
				}
			}
			return false;
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			try
			{
				TimeZoneInfo.Validate(this.id, this.baseUtcOffset, this.adjustmentRules);
			}
			catch (ArgumentException ex)
			{
				throw new SerializationException("invalid serialization data", ex);
			}
		}

		private static void Validate(string id, TimeSpan baseUtcOffset, TimeZoneInfo.AdjustmentRule[] adjustmentRules)
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
		}

		public override string ToString()
		{
			return this.DisplayName;
		}

		private TimeZoneInfo(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.id = (string)info.GetValue("Id", typeof(string));
			this.displayName = (string)info.GetValue("DisplayName", typeof(string));
			this.standardDisplayName = (string)info.GetValue("StandardName", typeof(string));
			this.daylightDisplayName = (string)info.GetValue("DaylightName", typeof(string));
			this.baseUtcOffset = (TimeSpan)info.GetValue("BaseUtcOffset", typeof(TimeSpan));
			this.adjustmentRules = (TimeZoneInfo.AdjustmentRule[])info.GetValue("AdjustmentRules", typeof(TimeZoneInfo.AdjustmentRule[]));
			this.supportsDaylightSavingTime = (bool)info.GetValue("SupportsDaylightSavingTime", typeof(bool));
		}

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
			bool flag = !disableDaylightSavingTime;
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
			else
			{
				flag = false;
			}
			this.id = id;
			this.baseUtcOffset = baseUtcOffset;
			this.displayName = displayName ?? id;
			this.standardDisplayName = standardDisplayName ?? id;
			this.daylightDisplayName = daylightDisplayName;
			this.supportsDaylightSavingTime = flag;
			this.adjustmentRules = adjustmentRules;
		}

		private TimeZoneInfo.AdjustmentRule GetApplicableRule(DateTime dateTime)
		{
			DateTime dateTime2 = dateTime;
			if (dateTime.Kind == DateTimeKind.Local && this != TimeZoneInfo.Local)
			{
				if (!TimeZoneInfo.TryAddTicks(dateTime2.ToUniversalTime(), this.BaseUtcOffset.Ticks, out dateTime2, DateTimeKind.Unspecified))
				{
					return null;
				}
			}
			else if (dateTime.Kind == DateTimeKind.Utc && this != TimeZoneInfo.Utc && !TimeZoneInfo.TryAddTicks(dateTime2, this.BaseUtcOffset.Ticks, out dateTime2, DateTimeKind.Unspecified))
			{
				return null;
			}
			dateTime2 = dateTime2.Date;
			if (this.adjustmentRules != null)
			{
				foreach (TimeZoneInfo.AdjustmentRule adjustmentRule in this.adjustmentRules)
				{
					if (adjustmentRule.DateStart > dateTime2)
					{
						return null;
					}
					if (!(adjustmentRule.DateEnd < dateTime2))
					{
						return adjustmentRule;
					}
				}
			}
			return null;
		}

		private bool TryGetTransitionOffset(DateTime dateTime, out TimeSpan offset, out bool isDst)
		{
			offset = this.BaseUtcOffset;
			isDst = false;
			if (this.transitions == null)
			{
				return false;
			}
			DateTime dateTime2 = dateTime;
			if (dateTime.Kind == DateTimeKind.Local && this != TimeZoneInfo.Local && !TimeZoneInfo.TryAddTicks(dateTime2.ToUniversalTime(), this.BaseUtcOffset.Ticks, out dateTime2, DateTimeKind.Utc))
			{
				return false;
			}
			if (dateTime.Kind != DateTimeKind.Utc && !TimeZoneInfo.TryAddTicks(dateTime2, -this.BaseUtcOffset.Ticks, out dateTime2, DateTimeKind.Utc))
			{
				return false;
			}
			TimeZoneInfo.AdjustmentRule applicableRule = this.GetApplicableRule(dateTime2);
			if (applicableRule != null)
			{
				DateTime dateTime3 = TimeZoneInfo.TransitionPoint(applicableRule.DaylightTransitionStart, dateTime2.Year);
				DateTime dateTime4 = TimeZoneInfo.TransitionPoint(applicableRule.DaylightTransitionEnd, dateTime2.Year);
				if (dateTime2 >= dateTime3 && dateTime2 <= dateTime4)
				{
					offset = this.baseUtcOffset + applicableRule.DaylightDelta;
					isDst = true;
					return true;
				}
			}
			return false;
		}

		private static DateTime TransitionPoint(TimeZoneInfo.TransitionTime transition, int year)
		{
			if (transition.IsFixedDateRule)
			{
				return new DateTime(year, transition.Month, transition.Day) + transition.TimeOfDay.TimeOfDay;
			}
			DayOfWeek dayOfWeek = new DateTime(year, transition.Month, 1).DayOfWeek;
			int num = 1 + (transition.Week - 1) * 7 + (transition.DayOfWeek - dayOfWeek + 7) % 7;
			if (num > DateTime.DaysInMonth(year, transition.Month))
			{
				num -= 7;
			}
			if (num < 1)
			{
				num += 7;
			}
			return new DateTime(year, transition.Month, num) + transition.TimeOfDay.TimeOfDay;
		}

		private static TimeZoneInfo.AdjustmentRule[] ValidateRules(List<TimeZoneInfo.AdjustmentRule> adjustmentRules)
		{
			if (adjustmentRules == null || adjustmentRules.Count == 0)
			{
				return null;
			}
			TimeZoneInfo.AdjustmentRule adjustmentRule = null;
			foreach (TimeZoneInfo.AdjustmentRule adjustmentRule2 in adjustmentRules.ToArray())
			{
				if (adjustmentRule != null && adjustmentRule.DateEnd > adjustmentRule2.DateStart)
				{
					adjustmentRules.Remove(adjustmentRule2);
				}
				adjustmentRule = adjustmentRule2;
			}
			return adjustmentRules.ToArray();
		}

		private static TimeZoneInfo BuildFromStream(string id, Stream stream)
		{
			byte[] array = new byte[16384];
			int num = stream.Read(array, 0, 16384);
			if (!TimeZoneInfo.ValidTZFile(array, num))
			{
				throw new InvalidTimeZoneException("TZ file too big for the buffer");
			}
			TimeZoneInfo timeZoneInfo;
			try
			{
				timeZoneInfo = TimeZoneInfo.ParseTZBuffer(id, array, num);
			}
			catch (InvalidTimeZoneException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new InvalidTimeZoneException("Time zone information file contains invalid data", ex);
			}
			return timeZoneInfo;
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
			return ((i >> 24) & 255) | ((i >> 8) & 65280) | ((i << 8) & 16711680) | ((i & 255) << 24);
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
			Dictionary<int, TimeType> dictionary2 = TimeZoneInfo.ParseTimesTypes(buffer, 44 + 4 * num4 + num4, num5, dictionary);
			List<KeyValuePair<DateTime, TimeType>> list = TimeZoneInfo.ParseTransitions(buffer, 44, num4, dictionary2);
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
			bool flag2 = false;
			for (int i = 0; i < list.Count; i++)
			{
				KeyValuePair<DateTime, TimeType> keyValuePair = list[i];
				DateTime key = keyValuePair.Key;
				TimeType value = keyValuePair.Value;
				if (!value.IsDst)
				{
					if (text != value.Name)
					{
						text = value.Name;
					}
					if (timeSpan.TotalSeconds != (double)value.Offset)
					{
						timeSpan = new TimeSpan(0, 0, value.Offset);
						if (list2.Count > 0)
						{
							flag2 = true;
						}
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
						if (dateTime.AddYears(1) < dateTime2)
						{
							flag2 = true;
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
					if (text2 != value.Name)
					{
						text2 = value.Name;
					}
					if (timeSpan2.TotalSeconds != (double)value.Offset - timeSpan.TotalSeconds)
					{
						timeSpan2 = new TimeSpan(0, 0, value.Offset) - timeSpan;
						if (timeSpan2.Ticks % 600000000L != 0L)
						{
							timeSpan2 = TimeSpan.FromMinutes((double)((long)(timeSpan2.TotalMinutes + 0.5)));
						}
					}
					dateTime = key;
					flag = true;
				}
			}
			TimeZoneInfo timeZoneInfo;
			if (list2.Count == 0 && !flag2)
			{
				if (text == null)
				{
					TimeType timeType = dictionary2[0];
					text = timeType.Name;
					timeSpan = new TimeSpan(0, 0, timeType.Offset);
				}
				timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(id, timeSpan, id, text);
			}
			else
			{
				timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(id, timeSpan, id, text, text2, TimeZoneInfo.ValidateRules(list2));
			}
			if (flag2 && list.Count > 0)
			{
				timeZoneInfo.transitions = list;
			}
			timeZoneInfo.supportsDaylightSavingTime = list2.Count > 0;
			return timeZoneInfo;
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
					for (int j = 1; j <= stringBuilder.Length; j++)
					{
						dictionary.Add(num + j, stringBuilder.ToString(j, stringBuilder.Length - j));
					}
					num = i + 1;
					stringBuilder = new StringBuilder();
				}
			}
			return dictionary;
		}

		private static Dictionary<int, TimeType> ParseTimesTypes(byte[] buffer, int index, int count, Dictionary<int, string> abbreviations)
		{
			Dictionary<int, TimeType> dictionary = new Dictionary<int, TimeType>(count);
			for (int i = 0; i < count; i++)
			{
				int num = TimeZoneInfo.ReadBigEndianInt32(buffer, index + 6 * i);
				num = num / 60 * 60;
				byte b = buffer[index + 6 * i + 4];
				byte b2 = buffer[index + 6 * i + 5];
				dictionary.Add(i, new TimeType(num, b > 0, abbreviations[(int)b2]));
			}
			return dictionary;
		}

		private static List<KeyValuePair<DateTime, TimeType>> ParseTransitions(byte[] buffer, int index, int count, Dictionary<int, TimeType> time_types)
		{
			List<KeyValuePair<DateTime, TimeType>> list = new List<KeyValuePair<DateTime, TimeType>>(count);
			for (int i = 0; i < count; i++)
			{
				DateTime dateTime = TimeZoneInfo.DateTimeFromUnixTime((long)TimeZoneInfo.ReadBigEndianInt32(buffer, index + 4 * i));
				byte b = buffer[index + 4 * count + i];
				list.Add(new KeyValuePair<DateTime, TimeType>(dateTime, time_types[(int)b]));
			}
			return list;
		}

		private static DateTime DateTimeFromUnixTime(long unix_time)
		{
			DateTime dateTime = new DateTime(1970, 1, 1);
			return dateTime.AddSeconds((double)unix_time);
		}

		internal static TimeSpan GetLocalUtcOffset(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			bool flag;
			return TimeZoneInfo.Local.GetUtcOffset(dateTime, out flag);
		}

		internal TimeSpan GetUtcOffset(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			bool flag;
			return this.GetUtcOffset(dateTime, out flag);
		}

		internal static TimeSpan GetUtcOffsetFromUtc(DateTime time, TimeZoneInfo zone, out bool isDaylightSavings, out bool isAmbiguousLocalDst)
		{
			isDaylightSavings = false;
			isAmbiguousLocalDst = false;
			TimeSpan timeSpan = zone.BaseUtcOffset;
			if (zone.IsAmbiguousTime(time))
			{
				isAmbiguousLocalDst = true;
			}
			return zone.GetUtcOffset(time, out isDaylightSavings);
		}

		internal const uint TIME_ZONE_ID_INVALID = 4294967295U;

		internal const uint ERROR_NO_MORE_ITEMS = 259U;

		private TimeSpan baseUtcOffset;

		private string daylightDisplayName;

		private string displayName;

		private string id;

		private static TimeZoneInfo local;

		private List<KeyValuePair<DateTime, TimeType>> transitions;

		private static bool readlinkNotFound;

		private string standardDisplayName;

		private bool supportsDaylightSavingTime;

		private static TimeZoneInfo utc;

		private static string timeZoneDirectory;

		private TimeZoneInfo.AdjustmentRule[] adjustmentRules;

		private static RegistryKey timeZoneKey;

		private static RegistryKey localZoneKey;

		private static ReadOnlyCollection<TimeZoneInfo> systemTimeZones;

		private const int BUFFER_SIZE = 16384;

		[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=b77a5c561934e089")]
		[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
		[Serializable]
		public sealed class AdjustmentRule : IEquatable<TimeZoneInfo.AdjustmentRule>, ISerializable, IDeserializationCallback
		{
			public DateTime DateStart
			{
				get
				{
					return this.m_dateStart;
				}
			}

			public DateTime DateEnd
			{
				get
				{
					return this.m_dateEnd;
				}
			}

			public TimeSpan DaylightDelta
			{
				get
				{
					return this.m_daylightDelta;
				}
			}

			public TimeZoneInfo.TransitionTime DaylightTransitionStart
			{
				get
				{
					return this.m_daylightTransitionStart;
				}
			}

			public TimeZoneInfo.TransitionTime DaylightTransitionEnd
			{
				get
				{
					return this.m_daylightTransitionEnd;
				}
			}

			internal TimeSpan BaseUtcOffsetDelta
			{
				get
				{
					return this.m_baseUtcOffsetDelta;
				}
			}

			internal bool HasDaylightSaving
			{
				get
				{
					return this.DaylightDelta != TimeSpan.Zero || this.DaylightTransitionStart.TimeOfDay != DateTime.MinValue || this.DaylightTransitionEnd.TimeOfDay != DateTime.MinValue.AddMilliseconds(1.0);
				}
			}

			public bool Equals(TimeZoneInfo.AdjustmentRule other)
			{
				return other != null && this.m_dateStart == other.m_dateStart && this.m_dateEnd == other.m_dateEnd && this.m_daylightDelta == other.m_daylightDelta && this.m_baseUtcOffsetDelta == other.m_baseUtcOffsetDelta && this.m_daylightTransitionEnd.Equals(other.m_daylightTransitionEnd) && this.m_daylightTransitionStart.Equals(other.m_daylightTransitionStart);
			}

			public override int GetHashCode()
			{
				return this.m_dateStart.GetHashCode();
			}

			private AdjustmentRule()
			{
			}

			public static TimeZoneInfo.AdjustmentRule CreateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd)
			{
				TimeZoneInfo.AdjustmentRule.ValidateAdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd);
				return new TimeZoneInfo.AdjustmentRule
				{
					m_dateStart = dateStart,
					m_dateEnd = dateEnd,
					m_daylightDelta = daylightDelta,
					m_daylightTransitionStart = daylightTransitionStart,
					m_daylightTransitionEnd = daylightTransitionEnd,
					m_baseUtcOffsetDelta = TimeSpan.Zero
				};
			}

			internal static TimeZoneInfo.AdjustmentRule CreateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd, TimeSpan baseUtcOffsetDelta)
			{
				TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd);
				adjustmentRule.m_baseUtcOffsetDelta = baseUtcOffsetDelta;
				return adjustmentRule;
			}

			internal bool IsStartDateMarkerForBeginningOfYear()
			{
				return this.DaylightTransitionStart.Month == 1 && this.DaylightTransitionStart.Day == 1 && this.DaylightTransitionStart.TimeOfDay.Hour == 0 && this.DaylightTransitionStart.TimeOfDay.Minute == 0 && this.DaylightTransitionStart.TimeOfDay.Second == 0 && this.m_dateStart.Year == this.m_dateEnd.Year;
			}

			internal bool IsEndDateMarkerForEndOfYear()
			{
				return this.DaylightTransitionEnd.Month == 1 && this.DaylightTransitionEnd.Day == 1 && this.DaylightTransitionEnd.TimeOfDay.Hour == 0 && this.DaylightTransitionEnd.TimeOfDay.Minute == 0 && this.DaylightTransitionEnd.TimeOfDay.Second == 0 && this.m_dateStart.Year == this.m_dateEnd.Year;
			}

			private static void ValidateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd)
			{
				if (dateStart.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException(Environment.GetResourceString("The supplied DateTime must have the Kind property set to DateTimeKind.Unspecified."), "dateStart");
				}
				if (dateEnd.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException(Environment.GetResourceString("The supplied DateTime must have the Kind property set to DateTimeKind.Unspecified."), "dateEnd");
				}
				if (daylightTransitionStart.Equals(daylightTransitionEnd))
				{
					throw new ArgumentException(Environment.GetResourceString("The DaylightTransitionStart property must not equal the DaylightTransitionEnd property."), "daylightTransitionEnd");
				}
				if (dateStart > dateEnd)
				{
					throw new ArgumentException(Environment.GetResourceString("The DateStart property must come before the DateEnd property."), "dateStart");
				}
				if (TimeZoneInfo.UtcOffsetOutOfRange(daylightDelta))
				{
					throw new ArgumentOutOfRangeException("daylightDelta", daylightDelta, Environment.GetResourceString("The TimeSpan parameter must be within plus or minus 14.0 hours."));
				}
				if (daylightDelta.Ticks % 600000000L != 0L)
				{
					throw new ArgumentException(Environment.GetResourceString("The TimeSpan parameter cannot be specified more precisely than whole minutes."), "daylightDelta");
				}
				if (dateStart.TimeOfDay != TimeSpan.Zero)
				{
					throw new ArgumentException(Environment.GetResourceString("The supplied DateTime includes a TimeOfDay setting.   This is not supported."), "dateStart");
				}
				if (dateEnd.TimeOfDay != TimeSpan.Zero)
				{
					throw new ArgumentException(Environment.GetResourceString("The supplied DateTime includes a TimeOfDay setting.   This is not supported."), "dateEnd");
				}
			}

			void IDeserializationCallback.OnDeserialization(object sender)
			{
				try
				{
					TimeZoneInfo.AdjustmentRule.ValidateAdjustmentRule(this.m_dateStart, this.m_dateEnd, this.m_daylightDelta, this.m_daylightTransitionStart, this.m_daylightTransitionEnd);
				}
				catch (ArgumentException ex)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."), ex);
				}
			}

			[SecurityCritical]
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("DateStart", this.m_dateStart);
				info.AddValue("DateEnd", this.m_dateEnd);
				info.AddValue("DaylightDelta", this.m_daylightDelta);
				info.AddValue("DaylightTransitionStart", this.m_daylightTransitionStart);
				info.AddValue("DaylightTransitionEnd", this.m_daylightTransitionEnd);
				info.AddValue("BaseUtcOffsetDelta", this.m_baseUtcOffsetDelta);
			}

			private AdjustmentRule(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				this.m_dateStart = (DateTime)info.GetValue("DateStart", typeof(DateTime));
				this.m_dateEnd = (DateTime)info.GetValue("DateEnd", typeof(DateTime));
				this.m_daylightDelta = (TimeSpan)info.GetValue("DaylightDelta", typeof(TimeSpan));
				this.m_daylightTransitionStart = (TimeZoneInfo.TransitionTime)info.GetValue("DaylightTransitionStart", typeof(TimeZoneInfo.TransitionTime));
				this.m_daylightTransitionEnd = (TimeZoneInfo.TransitionTime)info.GetValue("DaylightTransitionEnd", typeof(TimeZoneInfo.TransitionTime));
				object valueNoThrow = info.GetValueNoThrow("BaseUtcOffsetDelta", typeof(TimeSpan));
				if (valueNoThrow != null)
				{
					this.m_baseUtcOffsetDelta = (TimeSpan)valueNoThrow;
				}
			}

			private DateTime m_dateStart;

			private DateTime m_dateEnd;

			private TimeSpan m_daylightDelta;

			private TimeZoneInfo.TransitionTime m_daylightTransitionStart;

			private TimeZoneInfo.TransitionTime m_daylightTransitionEnd;

			private TimeSpan m_baseUtcOffsetDelta;
		}

		[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=b77a5c561934e089")]
		[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
		[Serializable]
		public struct TransitionTime : IEquatable<TimeZoneInfo.TransitionTime>, ISerializable, IDeserializationCallback
		{
			public DateTime TimeOfDay
			{
				get
				{
					return this.m_timeOfDay;
				}
			}

			public int Month
			{
				get
				{
					return (int)this.m_month;
				}
			}

			public int Week
			{
				get
				{
					return (int)this.m_week;
				}
			}

			public int Day
			{
				get
				{
					return (int)this.m_day;
				}
			}

			public DayOfWeek DayOfWeek
			{
				get
				{
					return this.m_dayOfWeek;
				}
			}

			public bool IsFixedDateRule
			{
				get
				{
					return this.m_isFixedDateRule;
				}
			}

			public override bool Equals(object obj)
			{
				return obj is TimeZoneInfo.TransitionTime && this.Equals((TimeZoneInfo.TransitionTime)obj);
			}

			public static bool operator ==(TimeZoneInfo.TransitionTime t1, TimeZoneInfo.TransitionTime t2)
			{
				return t1.Equals(t2);
			}

			public static bool operator !=(TimeZoneInfo.TransitionTime t1, TimeZoneInfo.TransitionTime t2)
			{
				return !t1.Equals(t2);
			}

			public bool Equals(TimeZoneInfo.TransitionTime other)
			{
				bool flag = this.m_isFixedDateRule == other.m_isFixedDateRule && this.m_timeOfDay == other.m_timeOfDay && this.m_month == other.m_month;
				if (flag)
				{
					if (other.m_isFixedDateRule)
					{
						flag = this.m_day == other.m_day;
					}
					else
					{
						flag = this.m_week == other.m_week && this.m_dayOfWeek == other.m_dayOfWeek;
					}
				}
				return flag;
			}

			public override int GetHashCode()
			{
				return (int)this.m_month ^ ((int)this.m_week << 8);
			}

			public static TimeZoneInfo.TransitionTime CreateFixedDateRule(DateTime timeOfDay, int month, int day)
			{
				return TimeZoneInfo.TransitionTime.CreateTransitionTime(timeOfDay, month, 1, day, DayOfWeek.Sunday, true);
			}

			public static TimeZoneInfo.TransitionTime CreateFloatingDateRule(DateTime timeOfDay, int month, int week, DayOfWeek dayOfWeek)
			{
				return TimeZoneInfo.TransitionTime.CreateTransitionTime(timeOfDay, month, week, 1, dayOfWeek, false);
			}

			private static TimeZoneInfo.TransitionTime CreateTransitionTime(DateTime timeOfDay, int month, int week, int day, DayOfWeek dayOfWeek, bool isFixedDateRule)
			{
				TimeZoneInfo.TransitionTime.ValidateTransitionTime(timeOfDay, month, week, day, dayOfWeek);
				return new TimeZoneInfo.TransitionTime
				{
					m_isFixedDateRule = isFixedDateRule,
					m_timeOfDay = timeOfDay,
					m_dayOfWeek = dayOfWeek,
					m_day = (byte)day,
					m_week = (byte)week,
					m_month = (byte)month
				};
			}

			private static void ValidateTransitionTime(DateTime timeOfDay, int month, int week, int day, DayOfWeek dayOfWeek)
			{
				if (timeOfDay.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException(Environment.GetResourceString("The supplied DateTime must have the Kind property set to DateTimeKind.Unspecified."), "timeOfDay");
				}
				if (month < 1 || month > 12)
				{
					throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("The Month parameter must be in the range 1 through 12."));
				}
				if (day < 1 || day > 31)
				{
					throw new ArgumentOutOfRangeException("day", Environment.GetResourceString("The Day parameter must be in the range 1 through 31."));
				}
				if (week < 1 || week > 5)
				{
					throw new ArgumentOutOfRangeException("week", Environment.GetResourceString("The Week parameter must be in the range 1 through 5."));
				}
				if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
				{
					throw new ArgumentOutOfRangeException("dayOfWeek", Environment.GetResourceString("The DayOfWeek enumeration must be in the range 0 through 6."));
				}
				if (timeOfDay.Year != 1 || timeOfDay.Month != 1 || timeOfDay.Day != 1 || timeOfDay.Ticks % 10000L != 0L)
				{
					throw new ArgumentException(Environment.GetResourceString("The supplied DateTime must have the Year, Month, and Day properties set to 1.  The time cannot be specified more precisely than whole milliseconds."), "timeOfDay");
				}
			}

			void IDeserializationCallback.OnDeserialization(object sender)
			{
				try
				{
					TimeZoneInfo.TransitionTime.ValidateTransitionTime(this.m_timeOfDay, (int)this.m_month, (int)this.m_week, (int)this.m_day, this.m_dayOfWeek);
				}
				catch (ArgumentException ex)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."), ex);
				}
			}

			[SecurityCritical]
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("TimeOfDay", this.m_timeOfDay);
				info.AddValue("Month", this.m_month);
				info.AddValue("Week", this.m_week);
				info.AddValue("Day", this.m_day);
				info.AddValue("DayOfWeek", this.m_dayOfWeek);
				info.AddValue("IsFixedDateRule", this.m_isFixedDateRule);
			}

			private TransitionTime(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				this.m_timeOfDay = (DateTime)info.GetValue("TimeOfDay", typeof(DateTime));
				this.m_month = (byte)info.GetValue("Month", typeof(byte));
				this.m_week = (byte)info.GetValue("Week", typeof(byte));
				this.m_day = (byte)info.GetValue("Day", typeof(byte));
				this.m_dayOfWeek = (DayOfWeek)info.GetValue("DayOfWeek", typeof(DayOfWeek));
				this.m_isFixedDateRule = (bool)info.GetValue("IsFixedDateRule", typeof(bool));
			}

			private DateTime m_timeOfDay;

			private byte m_month;

			private byte m_week;

			private byte m_day;

			private DayOfWeek m_dayOfWeek;

			private bool m_isFixedDateRule;
		}

		private sealed class StringSerializer
		{
			public static string GetSerializedString(TimeZoneInfo zone)
			{
				StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.Id));
				stringBuilder.Append(';');
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.BaseUtcOffset.TotalMinutes.ToString(CultureInfo.InvariantCulture)));
				stringBuilder.Append(';');
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.DisplayName));
				stringBuilder.Append(';');
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.StandardName));
				stringBuilder.Append(';');
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.DaylightName));
				stringBuilder.Append(';');
				TimeZoneInfo.AdjustmentRule[] adjustmentRules = zone.GetAdjustmentRules();
				if (adjustmentRules != null && adjustmentRules.Length != 0)
				{
					foreach (TimeZoneInfo.AdjustmentRule adjustmentRule in adjustmentRules)
					{
						stringBuilder.Append('[');
						stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(adjustmentRule.DateStart.ToString("MM:dd:yyyy", DateTimeFormatInfo.InvariantInfo)));
						stringBuilder.Append(';');
						stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(adjustmentRule.DateEnd.ToString("MM:dd:yyyy", DateTimeFormatInfo.InvariantInfo)));
						stringBuilder.Append(';');
						stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(adjustmentRule.DaylightDelta.TotalMinutes.ToString(CultureInfo.InvariantCulture)));
						stringBuilder.Append(';');
						TimeZoneInfo.StringSerializer.SerializeTransitionTime(adjustmentRule.DaylightTransitionStart, stringBuilder);
						stringBuilder.Append(';');
						TimeZoneInfo.StringSerializer.SerializeTransitionTime(adjustmentRule.DaylightTransitionEnd, stringBuilder);
						stringBuilder.Append(';');
						if (adjustmentRule.BaseUtcOffsetDelta != TimeSpan.Zero)
						{
							stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(adjustmentRule.BaseUtcOffsetDelta.TotalMinutes.ToString(CultureInfo.InvariantCulture)));
							stringBuilder.Append(';');
						}
						stringBuilder.Append(']');
					}
				}
				stringBuilder.Append(';');
				return StringBuilderCache.GetStringAndRelease(stringBuilder);
			}

			public static TimeZoneInfo GetDeserializedTimeZoneInfo(string source)
			{
				TimeZoneInfo.StringSerializer stringSerializer = new TimeZoneInfo.StringSerializer(source);
				string nextStringValue = stringSerializer.GetNextStringValue(false);
				TimeSpan nextTimeSpanValue = stringSerializer.GetNextTimeSpanValue(false);
				string nextStringValue2 = stringSerializer.GetNextStringValue(false);
				string nextStringValue3 = stringSerializer.GetNextStringValue(false);
				string nextStringValue4 = stringSerializer.GetNextStringValue(false);
				TimeZoneInfo.AdjustmentRule[] nextAdjustmentRuleArrayValue = stringSerializer.GetNextAdjustmentRuleArrayValue(false);
				TimeZoneInfo timeZoneInfo;
				try
				{
					timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(nextStringValue, nextTimeSpanValue, nextStringValue2, nextStringValue3, nextStringValue4, nextAdjustmentRuleArrayValue);
				}
				catch (ArgumentException ex)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."), ex);
				}
				catch (InvalidTimeZoneException ex2)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."), ex2);
				}
				return timeZoneInfo;
			}

			private StringSerializer(string str)
			{
				this.m_serializedText = str;
				this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
			}

			private static string SerializeSubstitute(string text)
			{
				text = text.Replace("\\", "\\\\");
				text = text.Replace("[", "\\[");
				text = text.Replace("]", "\\]");
				return text.Replace(";", "\\;");
			}

			private static void SerializeTransitionTime(TimeZoneInfo.TransitionTime time, StringBuilder serializedText)
			{
				serializedText.Append('[');
				serializedText.Append((time.IsFixedDateRule ? 1 : 0).ToString(CultureInfo.InvariantCulture));
				serializedText.Append(';');
				if (time.IsFixedDateRule)
				{
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.TimeOfDay.ToString("HH:mm:ss.FFF", DateTimeFormatInfo.InvariantInfo)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.Month.ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.Day.ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
				}
				else
				{
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.TimeOfDay.ToString("HH:mm:ss.FFF", DateTimeFormatInfo.InvariantInfo)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.Month.ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.Week.ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(((int)time.DayOfWeek).ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
				}
				serializedText.Append(']');
			}

			private static void VerifyIsEscapableCharacter(char c)
			{
				if (c != '\\' && c != ';' && c != '[' && c != ']')
				{
					throw new SerializationException(Environment.GetResourceString("The serialized data contained an invalid escape sequence '\\\\{0}'.", new object[] { c }));
				}
			}

			private void SkipVersionNextDataFields(int depth)
			{
				if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				TimeZoneInfo.StringSerializer.State state = TimeZoneInfo.StringSerializer.State.NotEscaped;
				for (int i = this.m_currentTokenStartIndex; i < this.m_serializedText.Length; i++)
				{
					if (state == TimeZoneInfo.StringSerializer.State.Escaped)
					{
						TimeZoneInfo.StringSerializer.VerifyIsEscapableCharacter(this.m_serializedText[i]);
						state = TimeZoneInfo.StringSerializer.State.NotEscaped;
					}
					else if (state == TimeZoneInfo.StringSerializer.State.NotEscaped)
					{
						char c = this.m_serializedText[i];
						if (c == '\0')
						{
							throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
						}
						switch (c)
						{
						case '[':
							depth++;
							break;
						case '\\':
							state = TimeZoneInfo.StringSerializer.State.Escaped;
							break;
						case ']':
							depth--;
							if (depth == 0)
							{
								this.m_currentTokenStartIndex = i + 1;
								if (this.m_currentTokenStartIndex >= this.m_serializedText.Length)
								{
									this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
									return;
								}
								this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
								return;
							}
							break;
						}
					}
				}
				throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
			}

			private string GetNextStringValue(bool canEndWithoutSeparator)
			{
				if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine)
				{
					if (canEndWithoutSeparator)
					{
						return null;
					}
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				else
				{
					if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
					}
					TimeZoneInfo.StringSerializer.State state = TimeZoneInfo.StringSerializer.State.NotEscaped;
					StringBuilder stringBuilder = StringBuilderCache.Acquire(64);
					for (int i = this.m_currentTokenStartIndex; i < this.m_serializedText.Length; i++)
					{
						if (state == TimeZoneInfo.StringSerializer.State.Escaped)
						{
							TimeZoneInfo.StringSerializer.VerifyIsEscapableCharacter(this.m_serializedText[i]);
							stringBuilder.Append(this.m_serializedText[i]);
							state = TimeZoneInfo.StringSerializer.State.NotEscaped;
						}
						else if (state == TimeZoneInfo.StringSerializer.State.NotEscaped)
						{
							char c = this.m_serializedText[i];
							if (c == '\0')
							{
								throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
							}
							if (c == ';')
							{
								this.m_currentTokenStartIndex = i + 1;
								if (this.m_currentTokenStartIndex >= this.m_serializedText.Length)
								{
									this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
								}
								else
								{
									this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
								}
								return StringBuilderCache.GetStringAndRelease(stringBuilder);
							}
							switch (c)
							{
							case '[':
								throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
							case '\\':
								state = TimeZoneInfo.StringSerializer.State.Escaped;
								break;
							case ']':
								if (canEndWithoutSeparator)
								{
									this.m_currentTokenStartIndex = i;
									this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
									return stringBuilder.ToString();
								}
								throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
							default:
								stringBuilder.Append(this.m_serializedText[i]);
								break;
							}
						}
					}
					if (state == TimeZoneInfo.StringSerializer.State.Escaped)
					{
						throw new SerializationException(Environment.GetResourceString("The serialized data contained an invalid escape sequence '\\\\{0}'.", new object[] { string.Empty }));
					}
					if (!canEndWithoutSeparator)
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
					}
					this.m_currentTokenStartIndex = this.m_serializedText.Length;
					this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
					return StringBuilderCache.GetStringAndRelease(stringBuilder);
				}
			}

			private DateTime GetNextDateTimeValue(bool canEndWithoutSeparator, string format)
			{
				DateTime dateTime;
				if (!DateTime.TryParseExact(this.GetNextStringValue(canEndWithoutSeparator), format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dateTime))
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				return dateTime;
			}

			private TimeSpan GetNextTimeSpanValue(bool canEndWithoutSeparator)
			{
				int nextInt32Value = this.GetNextInt32Value(canEndWithoutSeparator);
				TimeSpan timeSpan;
				try
				{
					timeSpan = new TimeSpan(0, nextInt32Value, 0);
				}
				catch (ArgumentOutOfRangeException ex)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."), ex);
				}
				return timeSpan;
			}

			private int GetNextInt32Value(bool canEndWithoutSeparator)
			{
				int num;
				if (!int.TryParse(this.GetNextStringValue(canEndWithoutSeparator), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out num))
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				return num;
			}

			private TimeZoneInfo.AdjustmentRule[] GetNextAdjustmentRuleArrayValue(bool canEndWithoutSeparator)
			{
				List<TimeZoneInfo.AdjustmentRule> list = new List<TimeZoneInfo.AdjustmentRule>(1);
				int num = 0;
				for (TimeZoneInfo.AdjustmentRule adjustmentRule = this.GetNextAdjustmentRuleValue(true); adjustmentRule != null; adjustmentRule = this.GetNextAdjustmentRuleValue(true))
				{
					list.Add(adjustmentRule);
					num++;
				}
				if (!canEndWithoutSeparator)
				{
					if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine)
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
					}
					if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
					}
				}
				if (num == 0)
				{
					return null;
				}
				return list.ToArray();
			}

			private TimeZoneInfo.AdjustmentRule GetNextAdjustmentRuleValue(bool canEndWithoutSeparator)
			{
				if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine)
				{
					if (canEndWithoutSeparator)
					{
						return null;
					}
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				else
				{
					if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
					}
					if (this.m_serializedText[this.m_currentTokenStartIndex] == ';')
					{
						return null;
					}
					if (this.m_serializedText[this.m_currentTokenStartIndex] != '[')
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
					}
					this.m_currentTokenStartIndex++;
					DateTime nextDateTimeValue = this.GetNextDateTimeValue(false, "MM:dd:yyyy");
					DateTime nextDateTimeValue2 = this.GetNextDateTimeValue(false, "MM:dd:yyyy");
					TimeSpan nextTimeSpanValue = this.GetNextTimeSpanValue(false);
					TimeZoneInfo.TransitionTime nextTransitionTimeValue = this.GetNextTransitionTimeValue(false);
					TimeZoneInfo.TransitionTime nextTransitionTimeValue2 = this.GetNextTransitionTimeValue(false);
					TimeSpan timeSpan = TimeSpan.Zero;
					if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
					}
					if ((this.m_serializedText[this.m_currentTokenStartIndex] >= '0' && this.m_serializedText[this.m_currentTokenStartIndex] <= '9') || this.m_serializedText[this.m_currentTokenStartIndex] == '-' || this.m_serializedText[this.m_currentTokenStartIndex] == '+')
					{
						timeSpan = this.GetNextTimeSpanValue(false);
					}
					if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
					}
					if (this.m_serializedText[this.m_currentTokenStartIndex] != ']')
					{
						this.SkipVersionNextDataFields(1);
					}
					else
					{
						this.m_currentTokenStartIndex++;
					}
					TimeZoneInfo.AdjustmentRule adjustmentRule;
					try
					{
						adjustmentRule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(nextDateTimeValue, nextDateTimeValue2, nextTimeSpanValue, nextTransitionTimeValue, nextTransitionTimeValue2, timeSpan);
					}
					catch (ArgumentException ex)
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."), ex);
					}
					if (this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
					}
					else
					{
						this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
					}
					return adjustmentRule;
				}
			}

			private TimeZoneInfo.TransitionTime GetNextTransitionTimeValue(bool canEndWithoutSeparator)
			{
				if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine || (this.m_currentTokenStartIndex < this.m_serializedText.Length && this.m_serializedText[this.m_currentTokenStartIndex] == ']'))
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				if (this.m_serializedText[this.m_currentTokenStartIndex] != '[')
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				this.m_currentTokenStartIndex++;
				int nextInt32Value = this.GetNextInt32Value(false);
				if (nextInt32Value != 0 && nextInt32Value != 1)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				DateTime nextDateTimeValue = this.GetNextDateTimeValue(false, "HH:mm:ss.FFF");
				nextDateTimeValue = new DateTime(1, 1, 1, nextDateTimeValue.Hour, nextDateTimeValue.Minute, nextDateTimeValue.Second, nextDateTimeValue.Millisecond);
				int nextInt32Value2 = this.GetNextInt32Value(false);
				TimeZoneInfo.TransitionTime transitionTime;
				if (nextInt32Value == 1)
				{
					int nextInt32Value3 = this.GetNextInt32Value(false);
					try
					{
						transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(nextDateTimeValue, nextInt32Value2, nextInt32Value3);
						goto IL_015B;
					}
					catch (ArgumentException ex)
					{
						throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."), ex);
					}
				}
				int nextInt32Value4 = this.GetNextInt32Value(false);
				int nextInt32Value5 = this.GetNextInt32Value(false);
				try
				{
					transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(nextDateTimeValue, nextInt32Value2, nextInt32Value4, (DayOfWeek)nextInt32Value5);
				}
				catch (ArgumentException ex2)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."), ex2);
				}
				IL_015B:
				if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				if (this.m_serializedText[this.m_currentTokenStartIndex] != ']')
				{
					this.SkipVersionNextDataFields(1);
				}
				else
				{
					this.m_currentTokenStartIndex++;
				}
				bool flag = false;
				if (this.m_currentTokenStartIndex < this.m_serializedText.Length && this.m_serializedText[this.m_currentTokenStartIndex] == ';')
				{
					this.m_currentTokenStartIndex++;
					flag = true;
				}
				if (!flag && !canEndWithoutSeparator)
				{
					throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."));
				}
				if (this.m_currentTokenStartIndex >= this.m_serializedText.Length)
				{
					this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
				}
				else
				{
					this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
				}
				return transitionTime;
			}

			private string m_serializedText;

			private int m_currentTokenStartIndex;

			private TimeZoneInfo.StringSerializer.State m_state;

			private const int initialCapacityForString = 64;

			private const char esc = '\\';

			private const char sep = ';';

			private const char lhs = '[';

			private const char rhs = ']';

			private const string escString = "\\";

			private const string sepString = ";";

			private const string lhsString = "[";

			private const string rhsString = "]";

			private const string escapedEsc = "\\\\";

			private const string escapedSep = "\\;";

			private const string escapedLhs = "\\[";

			private const string escapedRhs = "\\]";

			private const string dateTimeFormat = "MM:dd:yyyy";

			private const string timeOfDayFormat = "HH:mm:ss.FFF";

			private enum State
			{
				Escaped,
				NotEscaped,
				StartOfToken,
				EndOfLine
			}
		}

		private class TimeZoneInfoComparer : IComparer<TimeZoneInfo>
		{
			int IComparer<TimeZoneInfo>.Compare(TimeZoneInfo x, TimeZoneInfo y)
			{
				int num = x.BaseUtcOffset.CompareTo(y.BaseUtcOffset);
				if (num != 0)
				{
					return num;
				}
				return string.Compare(x.DisplayName, y.DisplayName, StringComparison.Ordinal);
			}
		}

		private enum TimeZoneData
		{
			DaylightSavingStartIdx,
			DaylightSavingEndIdx,
			UtcOffsetIdx,
			AdditionalDaylightOffsetIdx
		}

		private enum TimeZoneNames
		{
			StandardNameIdx,
			DaylightNameIdx
		}

		internal struct SYSTEMTIME
		{
			internal ushort wYear;

			internal ushort wMonth;

			internal ushort wDayOfWeek;

			internal ushort wDay;

			internal ushort wHour;

			internal ushort wMinute;

			internal ushort wSecond;

			internal ushort wMilliseconds;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct TIME_ZONE_INFORMATION
		{
			internal int Bias;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			internal string StandardName;

			internal TimeZoneInfo.SYSTEMTIME StandardDate;

			internal int StandardBias;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			internal string DaylightName;

			internal TimeZoneInfo.SYSTEMTIME DaylightDate;

			internal int DaylightBias;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct DYNAMIC_TIME_ZONE_INFORMATION
		{
			internal TimeZoneInfo.TIME_ZONE_INFORMATION TZI;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			internal string TimeZoneKeyName;

			internal byte DynamicDaylightTimeDisabled;
		}
	}
}
