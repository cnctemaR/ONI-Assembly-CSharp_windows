using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	public sealed class SoapDuration
	{
		public static string XsdType
		{
			get
			{
				return "duration";
			}
		}

		public static TimeSpan Parse(string value)
		{
			if (value.Length == 0)
			{
				throw new ArgumentException("Invalid format string for duration schema datatype.");
			}
			int num = 0;
			if (value[0] == '-')
			{
				num = 1;
			}
			bool flag = num == 1;
			if (value[num] != 'P')
			{
				throw new ArgumentException("Invalid format string for duration schema datatype.");
			}
			num++;
			int num2 = 0;
			int num3 = 0;
			bool flag2 = false;
			int num4 = 0;
			int num5 = 0;
			double num6 = 0.0;
			bool flag3 = false;
			int i = num;
			while (i < value.Length)
			{
				if (value[i] != 'T')
				{
					bool flag4 = true;
					int num7 = 0;
					while (i < value.Length)
					{
						if (!char.IsDigit(value[i]))
						{
							if (value[i] != '.')
							{
								break;
							}
							flag4 = false;
							num7++;
							if (num7 > 1)
							{
								flag3 = true;
								break;
							}
						}
						i++;
					}
					int num8 = -1;
					double num9 = -1.0;
					if (flag4)
					{
						num8 = int.Parse(value.Substring(num, i - num));
					}
					else
					{
						num9 = double.Parse(value.Substring(num, i - num), CultureInfo.InvariantCulture);
					}
					char c = value[i];
					if (c <= 'H')
					{
						if (c != 'D')
						{
							if (c != 'H')
							{
								goto IL_01F7;
							}
							num4 = num8;
							if (!flag2 || num2 > 4 || !flag4)
							{
								flag3 = true;
							}
							else
							{
								num2 = 5;
							}
						}
						else
						{
							num3 += num8;
							if (num2 > 2 || !flag4)
							{
								flag3 = true;
							}
							else
							{
								num2 = 3;
							}
						}
					}
					else if (c != 'M')
					{
						if (c != 'S')
						{
							if (c != 'Y')
							{
								goto IL_01F7;
							}
							num3 += num8 * 365;
							if (num2 > 0 || !flag4)
							{
								flag3 = true;
							}
							else
							{
								num2 = 1;
							}
						}
						else
						{
							if (flag4)
							{
								num6 = (double)num8;
							}
							else
							{
								num6 = num9;
							}
							if (!flag2 || num2 > 6)
							{
								flag3 = true;
							}
							else
							{
								num2 = 7;
							}
						}
					}
					else if (num2 < 2 && flag4)
					{
						num3 += 365 * (num8 / 12) + 30 * (num8 % 12);
						num2 = 2;
					}
					else if (flag2 && num2 < 6 && flag4)
					{
						num5 = num8;
						num2 = 6;
					}
					else
					{
						flag3 = true;
					}
					IL_01FA:
					if (!flag3)
					{
						i++;
						num = i;
						continue;
					}
					break;
					IL_01F7:
					flag3 = true;
					goto IL_01FA;
				}
				flag2 = true;
				num2 = 4;
				i++;
				num = i;
			}
			if (flag3)
			{
				throw new ArgumentException("Invalid format string for duration schema datatype.");
			}
			TimeSpan timeSpan = new TimeSpan(num3, num4, num5, 0) + TimeSpan.FromSeconds(num6);
			if (!flag)
			{
				return timeSpan;
			}
			return -timeSpan;
		}

		public static string ToString(TimeSpan timeSpan)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (timeSpan.Ticks < 0L)
			{
				stringBuilder.Append('-');
				timeSpan = timeSpan.Negate();
			}
			stringBuilder.Append('P');
			if (timeSpan.Days > 0)
			{
				stringBuilder.Append(timeSpan.Days).Append('D');
			}
			if (timeSpan.Days > 0 || timeSpan.Minutes > 0 || timeSpan.Seconds > 0 || timeSpan.Milliseconds > 0)
			{
				stringBuilder.Append('T');
				if (timeSpan.Hours > 0)
				{
					stringBuilder.Append(timeSpan.Hours).Append('H');
				}
				if (timeSpan.Minutes > 0)
				{
					stringBuilder.Append(timeSpan.Minutes).Append('M');
				}
				if (timeSpan.Seconds > 0 || timeSpan.Milliseconds > 0)
				{
					double num = (double)timeSpan.Seconds;
					if (timeSpan.Milliseconds > 0)
					{
						num += (double)timeSpan.Milliseconds / 1000.0;
					}
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", num));
					stringBuilder.Append('S');
				}
			}
			return stringBuilder.ToString();
		}
	}
}
