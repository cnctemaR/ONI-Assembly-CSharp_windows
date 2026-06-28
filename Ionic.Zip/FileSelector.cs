using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace Ionic
{
	public class FileSelector
	{
		public FileSelector(string selectionCriteria)
			: this(selectionCriteria, true)
		{
		}

		public FileSelector(string selectionCriteria, bool traverseDirectoryReparsePoints)
		{
			if (!string.IsNullOrEmpty(selectionCriteria))
			{
				this._Criterion = FileSelector._ParseCriterion(selectionCriteria);
			}
			this.TraverseReparsePoints = traverseDirectoryReparsePoints;
		}

		public string SelectionCriteria
		{
			get
			{
				if (this._Criterion == null)
				{
					return null;
				}
				return this._Criterion.ToString();
			}
			set
			{
				if (value == null)
				{
					this._Criterion = null;
					return;
				}
				if (value.Trim() == "")
				{
					this._Criterion = null;
					return;
				}
				this._Criterion = FileSelector._ParseCriterion(value);
			}
		}

		public bool TraverseReparsePoints { get; set; }

		private static string NormalizeCriteriaExpression(string source)
		{
			string[][] array = new string[][]
			{
				new string[] { "([^']*)\\(\\(([^']+)", "$1( ($2" },
				new string[] { "(.)\\)\\)", "$1) )" },
				new string[] { "\\((\\S)", "( $1" },
				new string[] { "(\\S)\\)", "$1 )" },
				new string[] { "^\\)", " )" },
				new string[] { "(\\S)\\(", "$1 (" },
				new string[] { "\\)(\\S)", ") $1" },
				new string[] { "(=)('[^']*')", "$1 $2" },
				new string[] { "([^ !><])(>|<|!=|=)", "$1 $2" },
				new string[] { "(>|<|!=|=)([^ =])", "$1 $2" },
				new string[] { "/", "\\" }
			};
			string text = source;
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = FileSelector.RegexAssertions.PrecededByEvenNumberOfSingleQuotes + array[i][0] + FileSelector.RegexAssertions.FollowedByEvenNumberOfSingleQuotesAndLineEnd;
				text = Regex.Replace(text, text2, array[i][1]);
			}
			string text3 = "/" + FileSelector.RegexAssertions.FollowedByOddNumberOfSingleQuotesAndLineEnd;
			text = Regex.Replace(text, text3, "\\");
			text3 = " " + FileSelector.RegexAssertions.FollowedByOddNumberOfSingleQuotesAndLineEnd;
			return Regex.Replace(text, text3, "\u0006");
		}

		private static SelectionCriterion _ParseCriterion(string s)
		{
			if (s == null)
			{
				return null;
			}
			s = FileSelector.NormalizeCriteriaExpression(s);
			if (s.IndexOf(" ") == -1)
			{
				s = "name = " + s;
			}
			string[] array = s.Trim().Split(new char[] { ' ', '\t' });
			if (array.Length < 3)
			{
				throw new ArgumentException(s);
			}
			SelectionCriterion selectionCriterion = null;
			Stack<FileSelector.ParseState> stack = new Stack<FileSelector.ParseState>();
			Stack<SelectionCriterion> stack2 = new Stack<SelectionCriterion>();
			stack.Push(FileSelector.ParseState.Start);
			int i = 0;
			while (i < array.Length)
			{
				string text = array[i].ToLower();
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				FileSelector.ParseState parseState;
				if (num <= 1563699588U)
				{
					if (num <= 739023492U)
					{
						if (num <= 329706515U)
						{
							if (num != 254395046U)
							{
								if (num != 329706515U)
								{
									goto IL_08CA;
								}
								if (!(text == "ctime"))
								{
									goto IL_08CA;
								}
								goto IL_0449;
							}
							else
							{
								if (!(text == "and"))
								{
									goto IL_08CA;
								}
								goto IL_0310;
							}
						}
						else if (num != 597743964U)
						{
							if (num != 739023492U)
							{
								goto IL_08CA;
							}
							if (!(text == ")"))
							{
								goto IL_08CA;
							}
							parseState = stack.Pop();
							if (stack.Peek() != FileSelector.ParseState.OpenParen)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							stack.Pop();
							stack.Push(FileSelector.ParseState.CriterionDone);
						}
						else
						{
							if (!(text == "size"))
							{
								goto IL_08CA;
							}
							goto IL_0552;
						}
					}
					else if (num <= 1058081160U)
					{
						if (num != 755801111U)
						{
							if (num != 1058081160U)
							{
								goto IL_08CA;
							}
							if (!(text == "filename"))
							{
								goto IL_08CA;
							}
							goto IL_073C;
						}
						else
						{
							if (!(text == "("))
							{
								goto IL_08CA;
							}
							parseState = stack.Peek();
							if (parseState != FileSelector.ParseState.Start && parseState != FileSelector.ParseState.ConjunctionPending && parseState != FileSelector.ParseState.OpenParen)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							if (array.Length <= i + 4)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							stack.Push(FileSelector.ParseState.OpenParen);
						}
					}
					else if (num != 1361572173U)
					{
						if (num != 1563699588U)
						{
							goto IL_08CA;
						}
						if (!(text == "or"))
						{
							goto IL_08CA;
						}
						goto IL_0310;
					}
					else
					{
						if (!(text == "type"))
						{
							goto IL_08CA;
						}
						goto IL_080C;
					}
				}
				else if (num <= 2746858573U)
				{
					if (num <= 2211460629U)
					{
						if (num != 2166136261U)
						{
							if (num != 2211460629U)
							{
								goto IL_08CA;
							}
							if (!(text == "length"))
							{
								goto IL_08CA;
							}
							goto IL_0552;
						}
						else
						{
							if (text == null)
							{
								goto IL_08CA;
							}
							if (text.Length != 0)
							{
								goto IL_08CA;
							}
							stack.Push(FileSelector.ParseState.Whitespace);
						}
					}
					else if (num != 2369371622U)
					{
						if (num != 2746858573U)
						{
							goto IL_08CA;
						}
						if (!(text == "atime"))
						{
							goto IL_08CA;
						}
						goto IL_0449;
					}
					else
					{
						if (!(text == "name"))
						{
							goto IL_08CA;
						}
						goto IL_073C;
					}
				}
				else if (num <= 3429620606U)
				{
					if (num != 2888110417U)
					{
						if (num != 3429620606U)
						{
							goto IL_08CA;
						}
						if (!(text == "xor"))
						{
							goto IL_08CA;
						}
						goto IL_0310;
					}
					else
					{
						if (!(text == "mtime"))
						{
							goto IL_08CA;
						}
						goto IL_0449;
					}
				}
				else if (num != 3791641492U)
				{
					if (num != 4191246291U)
					{
						goto IL_08CA;
					}
					if (!(text == "attrs"))
					{
						goto IL_08CA;
					}
					goto IL_080C;
				}
				else
				{
					if (!(text == "attributes"))
					{
						goto IL_08CA;
					}
					goto IL_080C;
				}
				IL_08E3:
				parseState = stack.Peek();
				if (parseState == FileSelector.ParseState.CriterionDone)
				{
					stack.Pop();
					if (stack.Peek() == FileSelector.ParseState.ConjunctionPending)
					{
						while (stack.Peek() == FileSelector.ParseState.ConjunctionPending)
						{
							CompoundCriterion compoundCriterion = stack2.Pop() as CompoundCriterion;
							compoundCriterion.Right = selectionCriterion;
							selectionCriterion = compoundCriterion;
							stack.Pop();
							parseState = stack.Pop();
							if (parseState != FileSelector.ParseState.CriterionDone)
							{
								throw new ArgumentException("??");
							}
						}
					}
					else
					{
						stack.Push(FileSelector.ParseState.CriterionDone);
					}
				}
				if (parseState == FileSelector.ParseState.Whitespace)
				{
					stack.Pop();
				}
				i++;
				continue;
				IL_0310:
				parseState = stack.Peek();
				if (parseState != FileSelector.ParseState.CriterionDone)
				{
					throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
				}
				if (array.Length <= i + 3)
				{
					throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
				}
				LogicalConjunction logicalConjunction = (LogicalConjunction)Enum.Parse(typeof(LogicalConjunction), array[i].ToUpper(), true);
				selectionCriterion = new CompoundCriterion
				{
					Left = selectionCriterion,
					Right = null,
					Conjunction = logicalConjunction
				};
				stack.Push(parseState);
				stack.Push(FileSelector.ParseState.ConjunctionPending);
				stack2.Push(selectionCriterion);
				goto IL_08E3;
				IL_0449:
				if (array.Length <= i + 2)
				{
					throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
				}
				DateTime dateTime;
				try
				{
					dateTime = DateTime.ParseExact(array[i + 2], "yyyy-MM-dd-HH:mm:ss", null);
				}
				catch (FormatException)
				{
					try
					{
						dateTime = DateTime.ParseExact(array[i + 2], "yyyy/MM/dd-HH:mm:ss", null);
					}
					catch (FormatException)
					{
						try
						{
							dateTime = DateTime.ParseExact(array[i + 2], "yyyy/MM/dd", null);
						}
						catch (FormatException)
						{
							try
							{
								dateTime = DateTime.ParseExact(array[i + 2], "MM/dd/yyyy", null);
							}
							catch (FormatException)
							{
								dateTime = DateTime.ParseExact(array[i + 2], "yyyy-MM-dd", null);
							}
						}
					}
				}
				dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
				selectionCriterion = new TimeCriterion
				{
					Which = (WhichTime)Enum.Parse(typeof(WhichTime), array[i], true),
					Operator = (ComparisonOperator)EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]),
					Time = dateTime
				};
				i += 2;
				stack.Push(FileSelector.ParseState.CriterionDone);
				goto IL_08E3;
				IL_0552:
				if (array.Length <= i + 2)
				{
					throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
				}
				string text2 = array[i + 2];
				long num2;
				if (text2.ToUpper().EndsWith("K"))
				{
					num2 = long.Parse(text2.Substring(0, text2.Length - 1)) * 1024L;
				}
				else if (text2.ToUpper().EndsWith("KB"))
				{
					num2 = long.Parse(text2.Substring(0, text2.Length - 2)) * 1024L;
				}
				else if (text2.ToUpper().EndsWith("M"))
				{
					num2 = long.Parse(text2.Substring(0, text2.Length - 1)) * 1024L * 1024L;
				}
				else if (text2.ToUpper().EndsWith("MB"))
				{
					num2 = long.Parse(text2.Substring(0, text2.Length - 2)) * 1024L * 1024L;
				}
				else if (text2.ToUpper().EndsWith("G"))
				{
					num2 = long.Parse(text2.Substring(0, text2.Length - 1)) * 1024L * 1024L * 1024L;
				}
				else if (text2.ToUpper().EndsWith("GB"))
				{
					num2 = long.Parse(text2.Substring(0, text2.Length - 2)) * 1024L * 1024L * 1024L;
				}
				else
				{
					num2 = long.Parse(array[i + 2]);
				}
				selectionCriterion = new SizeCriterion
				{
					Size = num2,
					Operator = (ComparisonOperator)EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1])
				};
				i += 2;
				stack.Push(FileSelector.ParseState.CriterionDone);
				goto IL_08E3;
				IL_073C:
				if (array.Length <= i + 2)
				{
					throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
				}
				ComparisonOperator comparisonOperator = (ComparisonOperator)EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]);
				if (comparisonOperator != ComparisonOperator.NotEqualTo && comparisonOperator != ComparisonOperator.EqualTo)
				{
					throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
				}
				string text3 = array[i + 2];
				if (text3.StartsWith("'") && text3.EndsWith("'"))
				{
					text3 = text3.Substring(1, text3.Length - 2).Replace("\u0006", " ");
				}
				selectionCriterion = new NameCriterion
				{
					MatchingFileSpec = text3,
					Operator = comparisonOperator
				};
				i += 2;
				stack.Push(FileSelector.ParseState.CriterionDone);
				goto IL_08E3;
				IL_080C:
				if (array.Length <= i + 2)
				{
					throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
				}
				ComparisonOperator comparisonOperator2 = (ComparisonOperator)EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]);
				if (comparisonOperator2 != ComparisonOperator.NotEqualTo && comparisonOperator2 != ComparisonOperator.EqualTo)
				{
					throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
				}
				SelectionCriterion selectionCriterion2;
				if (!(text == "type"))
				{
					AttributesCriterion attributesCriterion = new AttributesCriterion();
					attributesCriterion.AttributeString = array[i + 2];
					selectionCriterion2 = attributesCriterion;
					attributesCriterion.Operator = comparisonOperator2;
				}
				else
				{
					TypeCriterion typeCriterion = new TypeCriterion();
					typeCriterion.AttributeString = array[i + 2];
					selectionCriterion2 = typeCriterion;
					typeCriterion.Operator = comparisonOperator2;
				}
				selectionCriterion = selectionCriterion2;
				i += 2;
				stack.Push(FileSelector.ParseState.CriterionDone);
				goto IL_08E3;
				IL_08CA:
				throw new ArgumentException("'" + array[i] + "'");
			}
			return selectionCriterion;
		}

		public override string ToString()
		{
			return "FileSelector(" + this._Criterion.ToString() + ")";
		}

		private bool Evaluate(string filename)
		{
			return this._Criterion.Evaluate(filename);
		}

		[Conditional("SelectorTrace")]
		private void SelectorTrace(string format, params object[] args)
		{
			if (this._Criterion != null && this._Criterion.Verbose)
			{
				Console.WriteLine(format, args);
			}
		}

		public ICollection<string> SelectFiles(string directory)
		{
			return this.SelectFiles(directory, false);
		}

		public ReadOnlyCollection<string> SelectFiles(string directory, bool recurseDirectories)
		{
			if (this._Criterion == null)
			{
				throw new ArgumentException("SelectionCriteria has not been set");
			}
			List<string> list = new List<string>();
			try
			{
				if (Directory.Exists(directory))
				{
					foreach (string text in Directory.GetFiles(directory))
					{
						if (this.Evaluate(text))
						{
							list.Add(text);
						}
					}
					if (recurseDirectories)
					{
						foreach (string text2 in Directory.GetDirectories(directory))
						{
							if (this.TraverseReparsePoints || (File.GetAttributes(text2) & FileAttributes.ReparsePoint) == (FileAttributes)0)
							{
								if (this.Evaluate(text2))
								{
									list.Add(text2);
								}
								list.AddRange(this.SelectFiles(text2, recurseDirectories));
							}
						}
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (IOException)
			{
			}
			return list.AsReadOnly();
		}

		private bool Evaluate(ZipEntry entry)
		{
			return this._Criterion.Evaluate(entry);
		}

		public ICollection<ZipEntry> SelectEntries(ZipFile zip)
		{
			if (zip == null)
			{
				throw new ArgumentNullException("zip");
			}
			List<ZipEntry> list = new List<ZipEntry>();
			foreach (ZipEntry zipEntry in zip)
			{
				if (this.Evaluate(zipEntry))
				{
					list.Add(zipEntry);
				}
			}
			return list;
		}

		public ICollection<ZipEntry> SelectEntries(ZipFile zip, string directoryPathInArchive)
		{
			if (zip == null)
			{
				throw new ArgumentNullException("zip");
			}
			List<ZipEntry> list = new List<ZipEntry>();
			string text = ((directoryPathInArchive == null) ? null : directoryPathInArchive.Replace("/", "\\"));
			if (text != null)
			{
				while (text.EndsWith("\\"))
				{
					text = text.Substring(0, text.Length - 1);
				}
			}
			foreach (ZipEntry zipEntry in zip)
			{
				if ((directoryPathInArchive == null || Path.GetDirectoryName(zipEntry.FileName) == directoryPathInArchive || Path.GetDirectoryName(zipEntry.FileName) == text) && this.Evaluate(zipEntry))
				{
					list.Add(zipEntry);
				}
			}
			return list;
		}

		internal SelectionCriterion _Criterion;

		private enum ParseState
		{
			Start,
			OpenParen,
			CriterionDone,
			ConjunctionPending,
			Whitespace
		}

		private static class RegexAssertions
		{
			public static readonly string PrecededByOddNumberOfSingleQuotes = "(?<=(?:[^']*'[^']*')*'[^']*)";

			public static readonly string FollowedByOddNumberOfSingleQuotesAndLineEnd = "(?=[^']*'(?:[^']*'[^']*')*[^']*$)";

			public static readonly string PrecededByEvenNumberOfSingleQuotes = "(?<=(?:[^']*'[^']*')*[^']*)";

			public static readonly string FollowedByEvenNumberOfSingleQuotesAndLineEnd = "(?=(?:[^']*'[^']*')*[^']*$)";
		}
	}
}
