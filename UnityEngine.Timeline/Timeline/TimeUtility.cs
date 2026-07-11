using System;
using System.Text.RegularExpressions;

namespace UnityEngine.Timeline
{
	internal static class TimeUtility
	{
		private static void ValidateFrameRate(double frameRate)
		{
			if (frameRate <= TimeUtility.kTimeEpsilon)
			{
				throw new ArgumentException("frame rate cannot be 0 or negative");
			}
		}

		public static int ToFrames(double time, double frameRate)
		{
			TimeUtility.ValidateFrameRate(frameRate);
			time = Math.Min(Math.Max(time, -TimeUtility.k_MaxTimelineDurationInSeconds), TimeUtility.k_MaxTimelineDurationInSeconds);
			double num = TimeUtility.kTimeEpsilon * time;
			int num2;
			if (time < 0.0)
			{
				num2 = (int)Math.Ceiling(time * frameRate + num);
			}
			else
			{
				num2 = (int)Math.Floor(time * frameRate + num);
			}
			return num2;
		}

		public static double ToExactFrames(double time, double frameRate)
		{
			TimeUtility.ValidateFrameRate(frameRate);
			return time * frameRate;
		}

		public static double FromFrames(int frames, double frameRate)
		{
			TimeUtility.ValidateFrameRate(frameRate);
			return (double)frames / frameRate;
		}

		public static double FromFrames(double frames, double frameRate)
		{
			TimeUtility.ValidateFrameRate(frameRate);
			return frames / frameRate;
		}

		public static bool OnFrameBoundary(double time, double frameRate)
		{
			return TimeUtility.OnFrameBoundary(time, frameRate, Math.Max(time, 1.0) * frameRate * TimeUtility.kTimeEpsilon);
		}

		public static bool OnFrameBoundary(double time, double frameRate, double epsilon)
		{
			TimeUtility.ValidateFrameRate(frameRate);
			double num = TimeUtility.ToExactFrames(time, frameRate);
			double num2 = Math.Round(num);
			return Math.Abs(num - num2) < epsilon;
		}

		public static double RoundToFrame(double time, double frameRate)
		{
			TimeUtility.ValidateFrameRate(frameRate);
			double num = (double)((int)Math.Floor(time * frameRate)) / frameRate;
			double num2 = (double)((int)Math.Ceiling(time * frameRate)) / frameRate;
			return (Math.Abs(time - num) >= Math.Abs(time - num2)) ? num2 : num;
		}

		public static string TimeAsFrames(double timeValue, double frameRate, string format = "F2")
		{
			string text;
			if (TimeUtility.OnFrameBoundary(timeValue, frameRate))
			{
				text = TimeUtility.ToFrames(timeValue, frameRate).ToString();
			}
			else
			{
				text = TimeUtility.ToExactFrames(timeValue, frameRate).ToString(format);
			}
			return text;
		}

		public static string TimeAsTimeCode(double timeValue, double frameRate, string format = "F2")
		{
			TimeUtility.ValidateFrameRate(frameRate);
			int num = (int)Math.Abs(timeValue);
			int num2 = num / 3600;
			int num3 = num % 3600 / 60;
			int num4 = num % 60;
			string text = ((timeValue >= 0.0) ? string.Empty : "-");
			string text2;
			if (num2 > 0)
			{
				text2 = string.Concat(new object[]
				{
					num2,
					":",
					num3.ToString("D2"),
					":",
					num4.ToString("D2")
				});
			}
			else if (num3 > 0)
			{
				text2 = num3 + ":" + num4.ToString("D2");
			}
			else
			{
				text2 = num4.ToString();
			}
			int num5 = (int)Math.Floor(Math.Log10(frameRate) + 1.0);
			string text3 = (TimeUtility.ToFrames(timeValue, frameRate) - TimeUtility.ToFrames((double)num, frameRate)).ToString().PadLeft(num5, '0');
			if (!TimeUtility.OnFrameBoundary(timeValue, frameRate))
			{
				string text4 = TimeUtility.ToExactFrames(timeValue, frameRate).ToString(format);
				int num6 = text4.IndexOf('.');
				if (num6 >= 0)
				{
					text3 = text3 + " [" + text4.Substring(num6) + "]";
				}
			}
			return text + text2 + ":" + text3;
		}

		public static double ParseTimeCode(string timeCode, double frameRate, double defaultValue)
		{
			timeCode = TimeUtility.RemoveChar(timeCode, (char c) => char.IsWhiteSpace(c));
			string[] array = timeCode.Split(new char[] { ':' });
			double num;
			if (array.Length == 0 || array.Length > 4)
			{
				num = defaultValue;
			}
			else
			{
				int num2 = 0;
				int num3 = 0;
				double num4 = 0.0;
				double num5 = 0.0;
				try
				{
					string text = array[array.Length - 1];
					if (Regex.Match(text, "^\\d+\\.\\d+$").Success)
					{
						num4 = double.Parse(text);
						if (array.Length > 3)
						{
							return defaultValue;
						}
						if (array.Length > 1)
						{
							num3 = int.Parse(array[array.Length - 2]);
						}
						if (array.Length > 2)
						{
							num2 = int.Parse(array[array.Length - 3]);
						}
					}
					else
					{
						if (Regex.Match(text, "^\\d+\\[\\.\\d+\\]$").Success)
						{
							string text2 = TimeUtility.RemoveChar(text, (char c) => c == '[' || c == ']');
							num5 = double.Parse(text2);
						}
						else
						{
							if (!Regex.Match(text, "^\\d*$").Success)
							{
								return defaultValue;
							}
							num5 = (double)int.Parse(text);
						}
						if (array.Length > 1)
						{
							num4 = (double)int.Parse(array[array.Length - 2]);
						}
						if (array.Length > 2)
						{
							num3 = int.Parse(array[array.Length - 3]);
						}
						if (array.Length > 3)
						{
							num2 = int.Parse(array[array.Length - 4]);
						}
					}
				}
				catch (FormatException)
				{
					return defaultValue;
				}
				num = num5 / frameRate + num4 + (double)(num3 * 60) + (double)(num2 * 3600);
			}
			return num;
		}

		private static string RemoveChar(string str, Func<char, bool> charToRemoveFunc)
		{
			int length = str.Length;
			char[] array = str.ToCharArray();
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				if (!charToRemoveFunc(array[i]))
				{
					array[num++] = array[i];
				}
			}
			return new string(array, 0, num);
		}

		public static readonly double kTimeEpsilon = 1E-14;

		public static readonly double kFrameRateEpsilon = 1E-06;

		public static readonly double k_MaxTimelineDurationInSeconds = 9000000.0;
	}
}
