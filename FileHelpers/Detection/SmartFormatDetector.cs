using System;
using System.Collections.Generic;
using System.Text;
using FileHelpers.Dynamic;

namespace FileHelpers.Detection
{
	public sealed class SmartFormatDetector
	{
		public SmartFormatDetector()
		{
			this.QuotedChar = '"';
		}

		public FormatHint FormatHint
		{
			get
			{
				return this.mFormatHint;
			}
			set
			{
				this.mFormatHint = value;
			}
		}

		public int MaxSampleLines
		{
			get
			{
				return this.mMaxSampleLines;
			}
			set
			{
				this.mMaxSampleLines = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this.mEncoding;
			}
			set
			{
				this.mEncoding = value;
			}
		}

		public bool FileHasHeaders { get; set; }

		public double FixedLengthDeviationTolerance
		{
			get
			{
				return this.mFixedLengthDeviationTolerance;
			}
			set
			{
				this.mFixedLengthDeviationTolerance = value;
			}
		}

		public RecordFormatInfo[] DetectFileFormat(string file)
		{
			return this.DetectFileFormat(new string[] { file });
		}

		public RecordFormatInfo[] DetectFileFormat(IEnumerable<string> files)
		{
			List<RecordFormatInfo> list = new List<RecordFormatInfo>();
			string[][] sampleLines = this.GetSampleLines(files, this.MaxSampleLines);
			switch (this.mFormatHint)
			{
			case FormatHint.Unknown:
				this.CreateMixedOptions(sampleLines, list);
				break;
			case FormatHint.FixedLength:
				this.CreateFixedLengthOptions(sampleLines, list);
				break;
			case FormatHint.Delimited:
				this.CreateDelimiterOptions(sampleLines, list, '\0');
				break;
			case FormatHint.DelimitedByTab:
				this.CreateDelimiterOptions(sampleLines, list, '\t');
				break;
			case FormatHint.DelimitedByComma:
				this.CreateDelimiterOptions(sampleLines, list, ',');
				break;
			case FormatHint.DelimitedBySemicolon:
				this.CreateDelimiterOptions(sampleLines, list, ';');
				break;
			default:
				throw new InvalidOperationException("Unsuported FormatHint value.");
			}
			foreach (RecordFormatInfo recordFormatInfo in list)
			{
				this.DetectOptionals(recordFormatInfo, sampleLines);
				this.DetectTypes(recordFormatInfo, sampleLines);
				this.DetectQuoted(recordFormatInfo, sampleLines);
			}
			list.Sort((RecordFormatInfo x, RecordFormatInfo y) => -1 * x.Confidence.CompareTo(y.Confidence));
			return list.ToArray();
		}

		private void DetectQuoted(RecordFormatInfo format, string[][] data)
		{
			FixedLengthClassBuilder fixedLengthClassBuilder = format.ClassBuilder as FixedLengthClassBuilder;
		}

		private void DetectTypes(RecordFormatInfo format, string[][] data)
		{
		}

		private void DetectOptionals(RecordFormatInfo option, string[][] data)
		{
		}

		private void CreateMixedOptions(string[][] data, List<RecordFormatInfo> res)
		{
			SmartFormatDetector.Indicators indicators = SmartFormatDetector.Indicators.CalculateAsFixedSize(data);
			if (indicators.Deviation / indicators.Avg <= this.FixedLengthDeviationTolerance * (double)Math.Min(1, this.NumberOfLines(data) / 15))
			{
				this.CreateFixedLengthOptions(data, res);
			}
			this.CreateDelimiterOptions(data, res, '\0');
		}

		private void CreateFixedLengthOptions(string[][] data, List<RecordFormatInfo> res)
		{
			RecordFormatInfo recordFormatInfo = new RecordFormatInfo();
			SmartFormatDetector.Indicators indicators = SmartFormatDetector.Indicators.CalculateAsFixedSize(data);
			recordFormatInfo.mConfidence = (int)(Math.Max(0.0, 1.0 - indicators.Deviation / indicators.Avg) * 100.0);
			FixedLengthClassBuilder fixedLengthClassBuilder = new FixedLengthClassBuilder("AutoDetectedClass");
			this.CreateFixedLengthFields(data, fixedLengthClassBuilder);
			recordFormatInfo.mClassBuilder = fixedLengthClassBuilder;
			res.Add(recordFormatInfo);
		}

		private void CreateFixedLengthFields(string[][] data, FixedLengthClassBuilder builder)
		{
			List<SmartFormatDetector.FixedColumnInfo> list = null;
			foreach (string[] array in data)
			{
				List<SmartFormatDetector.FixedColumnInfo> list2 = this.CreateFixedLengthCandidates(array);
				list = this.JoinFixedColCandidates(list, list2);
			}
			for (int j = 0; j < list.Count; j++)
			{
				SmartFormatDetector.FixedColumnInfo fixedColumnInfo = list[j];
				builder.AddField("Field" + j.ToString().PadLeft(4, '0'), fixedColumnInfo.Length, typeof(string));
			}
		}

		private List<SmartFormatDetector.FixedColumnInfo> CreateFixedLengthCandidates(string[] lines)
		{
			List<SmartFormatDetector.FixedColumnInfo> list = null;
			foreach (string text in lines)
			{
				List<SmartFormatDetector.FixedColumnInfo> list2 = new List<SmartFormatDetector.FixedColumnInfo>();
				int num = 0;
				SmartFormatDetector.FixedColumnInfo fixedColumnInfo = null;
				for (int j = 1; j < text.Length; j++)
				{
					if (char.IsWhiteSpace(text[j]))
					{
						num++;
					}
					else if (num > 2)
					{
						if (fixedColumnInfo == null)
						{
							fixedColumnInfo = new SmartFormatDetector.FixedColumnInfo
							{
								Start = 0,
								Length = j
							};
						}
						else
						{
							SmartFormatDetector.FixedColumnInfo fixedColumnInfo2 = fixedColumnInfo;
							fixedColumnInfo = new SmartFormatDetector.FixedColumnInfo
							{
								Start = fixedColumnInfo2.Start + fixedColumnInfo2.Length
							};
							fixedColumnInfo.Length = j - fixedColumnInfo.Start;
						}
						list2.Add(fixedColumnInfo);
						num = 0;
					}
				}
				if (fixedColumnInfo == null)
				{
					fixedColumnInfo = new SmartFormatDetector.FixedColumnInfo
					{
						Start = 0,
						Length = text.Length
					};
				}
				else
				{
					SmartFormatDetector.FixedColumnInfo fixedColumnInfo3 = fixedColumnInfo;
					fixedColumnInfo = new SmartFormatDetector.FixedColumnInfo
					{
						Start = fixedColumnInfo3.Start + fixedColumnInfo3.Length
					};
					fixedColumnInfo.Length = text.Length - fixedColumnInfo.Start;
				}
				list2.Add(fixedColumnInfo);
				list = this.JoinFixedColCandidates(list, list2);
			}
			return list;
		}

		private List<SmartFormatDetector.FixedColumnInfo> JoinFixedColCandidates(List<SmartFormatDetector.FixedColumnInfo> cand1, List<SmartFormatDetector.FixedColumnInfo> cand2)
		{
			if (cand1 == null)
			{
				return cand2;
			}
			if (cand2 == null)
			{
				return cand1;
			}
			return cand1;
		}

		private void CreateDelimiterOptions(string[][] sampleData, List<RecordFormatInfo> res, char delimiter = '\0')
		{
			List<DelimiterInfo> list = new List<DelimiterInfo>();
			if (delimiter == '\0')
			{
				list = this.GetDelimiters(sampleData);
			}
			else
			{
				list.Add(this.GetDelimiterInfo(sampleData, delimiter));
			}
			foreach (DelimiterInfo delimiterInfo in list)
			{
				RecordFormatInfo recordFormatInfo = new RecordFormatInfo
				{
					mConfidence = (int)((1.0 - delimiterInfo.Deviation) * 100.0)
				};
				this.AdjustConfidence(recordFormatInfo, delimiterInfo);
				DelimitedClassBuilder delimitedClassBuilder = new DelimitedClassBuilder("AutoDetectedClass", delimiterInfo.Delimiter.ToString())
				{
					IgnoreFirstLines = (this.FileHasHeaders ? 1 : 0)
				};
				string[] array = sampleData[0][0].Split(new char[] { delimiterInfo.Delimiter });
				for (int i = 0; i < delimiterInfo.Max + 1; i++)
				{
					string text = "Field " + (i + 1).ToString().PadLeft(3, '0');
					if (this.FileHasHeaders && i < array.Length)
					{
						text = array[i];
					}
					DelimitedFieldBuilder delimitedFieldBuilder = delimitedClassBuilder.AddField(StringHelper.ToValidIdentifier(text));
					if (i > delimiterInfo.Min)
					{
						delimitedFieldBuilder.FieldOptional = true;
					}
				}
				recordFormatInfo.mClassBuilder = delimitedClassBuilder;
				res.Add(recordFormatInfo);
			}
		}

		private void AdjustConfidence(RecordFormatInfo format, DelimiterInfo info)
		{
			char delimiter = info.Delimiter;
			if (delimiter > '"')
			{
				switch (delimiter)
				{
				case '&':
					break;
				case '\'':
					goto IL_0076;
				case '(':
				case ')':
				case '*':
				case '+':
					return;
				case ',':
					goto IL_00DA;
				case '-':
					format.mConfidence = (int)((double)format.Confidence * 0.7);
					return;
				case '.':
				case '/':
					format.mConfidence = (int)((double)format.Confidence * 0.4);
					return;
				default:
					switch (delimiter)
					{
					case ':':
					case '=':
					case '@':
						break;
					case ';':
						goto IL_00DA;
					case '<':
					case '>':
					case '?':
						return;
					default:
						if (delimiter != '|')
						{
							return;
						}
						goto IL_00DA;
					}
					break;
				}
				format.mConfidence = (int)((double)format.Confidence * 0.6);
				return;
			}
			if (delimiter == '\t')
			{
				goto IL_00DA;
			}
			if (delimiter != '"')
			{
				return;
			}
			IL_0076:
			format.mConfidence = (int)((double)format.Confidence * 0.2);
			return;
			IL_00DA:
			format.mConfidence = (int)Math.Min(100.0, (double)format.Confidence * 1.15);
		}

		private string[][] GetSampleLines(IEnumerable<string> files, int nroOfLines)
		{
			List<string[]> list = new List<string[]>();
			foreach (string text in files)
			{
				list.Add(CommonEngine.RawReadFirstLinesArray(text, nroOfLines, this.mEncoding));
			}
			return list.ToArray();
		}

		private int NumberOfLines(string[][] data)
		{
			int num = 0;
			foreach (string[] array in data)
			{
				num += array.Length;
			}
			return num;
		}

		private DelimiterInfo GetDelimiterInfo(string[][] data, char delimiter)
		{
			SmartFormatDetector.Indicators indicators = SmartFormatDetector.Indicators.CalculateByDelimiter(delimiter, data, new char?(this.QuotedChar));
			return new DelimiterInfo(delimiter, indicators.Avg, indicators.Max, indicators.Min, indicators.Deviation);
		}

		private List<DelimiterInfo> GetDelimiters(string[][] data)
		{
			Dictionary<char, int> dictionary = new Dictionary<char, int>();
			int num = 0;
			for (int i = 0; i < data.Length; i++)
			{
				for (int j = 0; j < data[i].Length; j++)
				{
					if (j != 0)
					{
						string text = data[i][j];
						if (!string.IsNullOrEmpty(text))
						{
							num++;
							foreach (char c in text)
							{
								if (!char.IsLetterOrDigit(c) && c != ' ')
								{
									int num2;
									if (dictionary.TryGetValue(c, out num2))
									{
										num2++;
										dictionary[c] = num2;
									}
									else
									{
										dictionary.Add(c, 1);
									}
								}
							}
						}
					}
				}
			}
			List<DelimiterInfo> list = new List<DelimiterInfo>();
			if (num == 0)
			{
				return list;
			}
			List<char> list2 = new List<char>(dictionary.Count);
			foreach (KeyValuePair<char, int> keyValuePair in dictionary)
			{
				if (keyValuePair.Value >= num)
				{
					list2.Add(keyValuePair.Key);
				}
			}
			foreach (char c2 in list2)
			{
				SmartFormatDetector.Indicators indicators = SmartFormatDetector.Indicators.CalculateByDelimiter(c2, data, new char?(this.QuotedChar));
				if (num < 15)
				{
					indicators.Deviation *= Math.Min(1.0, (double)num / 15.0);
				}
				if (indicators.Avg > 1.0 && indicators.Deviation < 0.25)
				{
					list.Add(new DelimiterInfo(c2, indicators.Avg, indicators.Max, indicators.Min, indicators.Deviation));
				}
			}
			return list;
		}

		private char QuotedChar { get; set; }

		private const int MinSampleData = 15;

		private const double MinDelimitedDeviation = 0.25;

		private FormatHint mFormatHint;

		private int mMaxSampleLines = 50;

		private Encoding mEncoding = Encoding.Default;

		private double mFixedLengthDeviationTolerance = 0.01;

		private class FixedColumnInfo
		{
			public int Start;

			public int Length;
		}

		private class Indicators
		{
			private static double CalculateDeviation(IList<int> values, double avg)
			{
				double num = 0.0;
				for (int i = 0; i < values.Count; i++)
				{
					num += Math.Pow((double)values[i] - avg, 2.0);
				}
				return Math.Sqrt(num / (double)values.Count);
			}

			private static int CountNumberOfDelimiters(string line, char delimiter)
			{
				int num = 0;
				foreach (char c in line)
				{
					if (c != ' ' && !char.IsLetterOrDigit(c))
					{
						num++;
					}
				}
				return num;
			}

			public static SmartFormatDetector.Indicators CalculateByDelimiter(char delimiter, string[][] data, char? quotedChar)
			{
				SmartFormatDetector.Indicators indicators = new SmartFormatDetector.Indicators();
				int num = 0;
				int num2 = 0;
				List<int> list = new List<int>(100);
				foreach (string[] array in data)
				{
					foreach (string text in array)
					{
						if (!string.IsNullOrEmpty(text))
						{
							num2++;
							int num3;
							if (quotedChar != null)
							{
								num3 = QuoteHelper.CountNumberOfDelimiters(text, delimiter, quotedChar.Value);
							}
							else
							{
								num3 = SmartFormatDetector.Indicators.CountNumberOfDelimiters(text, delimiter);
							}
							list.Add(num3);
							if (num3 > indicators.Max)
							{
								indicators.Max = num3;
							}
							if (num3 < indicators.Min)
							{
								indicators.Min = num3;
							}
							num += num3;
						}
					}
				}
				indicators.Avg = (double)num / (double)num2;
				indicators.Deviation = SmartFormatDetector.Indicators.CalculateDeviation(list, indicators.Avg);
				return indicators;
			}

			public static SmartFormatDetector.Indicators CalculateAsFixedSize(string[][] data)
			{
				SmartFormatDetector.Indicators indicators = new SmartFormatDetector.Indicators();
				double num = 0.0;
				int num2 = 0;
				List<int> list = new List<int>(100);
				foreach (string[] array in data)
				{
					foreach (string text in array)
					{
						if (!string.IsNullOrEmpty(text))
						{
							num2++;
							num += (double)text.Length;
							list.Add(text.Length);
							if (text.Length > indicators.Max)
							{
								indicators.Max = text.Length;
							}
							if (text.Length < indicators.Min)
							{
								indicators.Min = text.Length;
							}
						}
					}
				}
				indicators.Avg = num / (double)num2;
				indicators.Deviation = SmartFormatDetector.Indicators.CalculateDeviation(list, indicators.Avg);
				return indicators;
			}

			public int Max = int.MinValue;

			public int Min = int.MaxValue;

			public double Avg;

			public double Deviation;

			public int Lines;
		}
	}
}
