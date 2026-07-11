using System;
using System.Text;

namespace System.Data.Common
{
	internal class MultipartIdentifier
	{
		internal static string[] ParseMultipartIdentifier(string name, string leftQuote, string rightQuote, string property, bool ThrowOnEmptyMultipartName)
		{
			return MultipartIdentifier.ParseMultipartIdentifier(name, leftQuote, rightQuote, '.', 4, true, property, ThrowOnEmptyMultipartName);
		}

		private static void IncrementStringCount(string name, string[] ary, ref int position, string property)
		{
			position++;
			int num = ary.Length;
			if (position >= num)
			{
				throw ADP.InvalidMultipartNameToManyParts(property, name, num);
			}
			ary[position] = string.Empty;
		}

		private static bool IsWhitespace(char ch)
		{
			return char.IsWhiteSpace(ch);
		}

		internal static string[] ParseMultipartIdentifier(string name, string leftQuote, string rightQuote, char separator, int limit, bool removequotes, string property, bool ThrowOnEmptyMultipartName)
		{
			if (limit <= 0)
			{
				throw ADP.InvalidMultipartNameToManyParts(property, name, limit);
			}
			if (-1 != leftQuote.IndexOf(separator) || -1 != rightQuote.IndexOf(separator) || leftQuote.Length != rightQuote.Length)
			{
				throw ADP.InvalidMultipartNameIncorrectUsageOfQuotes(property, name);
			}
			string[] array = new string[limit];
			int num = 0;
			MultipartIdentifier.MPIState mpistate = MultipartIdentifier.MPIState.MPI_Value;
			StringBuilder stringBuilder = new StringBuilder(name.Length);
			StringBuilder stringBuilder2 = null;
			char c = ' ';
			foreach (char c2 in name)
			{
				switch (mpistate)
				{
				case MultipartIdentifier.MPIState.MPI_Value:
					if (!MultipartIdentifier.IsWhitespace(c2))
					{
						int num2;
						if (c2 == separator)
						{
							array[num] = string.Empty;
							MultipartIdentifier.IncrementStringCount(name, array, ref num, property);
						}
						else if (-1 != (num2 = leftQuote.IndexOf(c2)))
						{
							c = rightQuote[num2];
							stringBuilder.Length = 0;
							if (!removequotes)
							{
								stringBuilder.Append(c2);
							}
							mpistate = MultipartIdentifier.MPIState.MPI_ParseQuote;
						}
						else
						{
							if (-1 != rightQuote.IndexOf(c2))
							{
								throw ADP.InvalidMultipartNameIncorrectUsageOfQuotes(property, name);
							}
							stringBuilder.Length = 0;
							stringBuilder.Append(c2);
							mpistate = MultipartIdentifier.MPIState.MPI_ParseNonQuote;
						}
					}
					break;
				case MultipartIdentifier.MPIState.MPI_ParseNonQuote:
					if (c2 == separator)
					{
						array[num] = stringBuilder.ToString();
						MultipartIdentifier.IncrementStringCount(name, array, ref num, property);
						mpistate = MultipartIdentifier.MPIState.MPI_Value;
					}
					else
					{
						if (-1 != rightQuote.IndexOf(c2))
						{
							throw ADP.InvalidMultipartNameIncorrectUsageOfQuotes(property, name);
						}
						if (-1 != leftQuote.IndexOf(c2))
						{
							throw ADP.InvalidMultipartNameIncorrectUsageOfQuotes(property, name);
						}
						if (MultipartIdentifier.IsWhitespace(c2))
						{
							array[num] = stringBuilder.ToString();
							if (stringBuilder2 == null)
							{
								stringBuilder2 = new StringBuilder();
							}
							stringBuilder2.Length = 0;
							stringBuilder2.Append(c2);
							mpistate = MultipartIdentifier.MPIState.MPI_LookForNextCharOrSeparator;
						}
						else
						{
							stringBuilder.Append(c2);
						}
					}
					break;
				case MultipartIdentifier.MPIState.MPI_LookForSeparator:
					if (!MultipartIdentifier.IsWhitespace(c2))
					{
						if (c2 != separator)
						{
							throw ADP.InvalidMultipartNameIncorrectUsageOfQuotes(property, name);
						}
						MultipartIdentifier.IncrementStringCount(name, array, ref num, property);
						mpistate = MultipartIdentifier.MPIState.MPI_Value;
					}
					break;
				case MultipartIdentifier.MPIState.MPI_LookForNextCharOrSeparator:
					if (!MultipartIdentifier.IsWhitespace(c2))
					{
						if (c2 == separator)
						{
							MultipartIdentifier.IncrementStringCount(name, array, ref num, property);
							mpistate = MultipartIdentifier.MPIState.MPI_Value;
						}
						else
						{
							stringBuilder.Append(stringBuilder2);
							stringBuilder.Append(c2);
							array[num] = stringBuilder.ToString();
							mpistate = MultipartIdentifier.MPIState.MPI_ParseNonQuote;
						}
					}
					else
					{
						stringBuilder2.Append(c2);
					}
					break;
				case MultipartIdentifier.MPIState.MPI_ParseQuote:
					if (c2 == c)
					{
						if (!removequotes)
						{
							stringBuilder.Append(c2);
						}
						mpistate = MultipartIdentifier.MPIState.MPI_RightQuote;
					}
					else
					{
						stringBuilder.Append(c2);
					}
					break;
				case MultipartIdentifier.MPIState.MPI_RightQuote:
					if (c2 == c)
					{
						stringBuilder.Append(c2);
						mpistate = MultipartIdentifier.MPIState.MPI_ParseQuote;
					}
					else if (c2 == separator)
					{
						array[num] = stringBuilder.ToString();
						MultipartIdentifier.IncrementStringCount(name, array, ref num, property);
						mpistate = MultipartIdentifier.MPIState.MPI_Value;
					}
					else
					{
						if (!MultipartIdentifier.IsWhitespace(c2))
						{
							throw ADP.InvalidMultipartNameIncorrectUsageOfQuotes(property, name);
						}
						array[num] = stringBuilder.ToString();
						mpistate = MultipartIdentifier.MPIState.MPI_LookForSeparator;
					}
					break;
				}
			}
			switch (mpistate)
			{
			case MultipartIdentifier.MPIState.MPI_Value:
			case MultipartIdentifier.MPIState.MPI_LookForSeparator:
			case MultipartIdentifier.MPIState.MPI_LookForNextCharOrSeparator:
				goto IL_02D4;
			case MultipartIdentifier.MPIState.MPI_ParseNonQuote:
			case MultipartIdentifier.MPIState.MPI_RightQuote:
				array[num] = stringBuilder.ToString();
				goto IL_02D4;
			}
			throw ADP.InvalidMultipartNameIncorrectUsageOfQuotes(property, name);
			IL_02D4:
			if (array[0] == null)
			{
				if (ThrowOnEmptyMultipartName)
				{
					throw ADP.InvalidMultipartName(property, name);
				}
			}
			else
			{
				int num3 = limit - num - 1;
				if (num3 > 0)
				{
					for (int j = limit - 1; j >= num3; j--)
					{
						array[j] = array[j - num3];
						array[j - num3] = null;
					}
				}
			}
			return array;
		}

		private const int MaxParts = 4;

		internal const int ServerIndex = 0;

		internal const int CatalogIndex = 1;

		internal const int SchemaIndex = 2;

		internal const int TableIndex = 3;

		private enum MPIState
		{
			MPI_Value,
			MPI_ParseNonQuote,
			MPI_LookForSeparator,
			MPI_LookForNextCharOrSeparator,
			MPI_ParseQuote,
			MPI_RightQuote
		}
	}
}
