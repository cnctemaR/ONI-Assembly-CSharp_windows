using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FileHelpers
{
	[DebuggerDisplay("FileDiffEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
	public sealed class FileDiffEngine<T> : EngineBase where T : class, IComparableRecord<T>
	{
		public FileDiffEngine()
			: base(typeof(T))
		{
		}

		public T[] OnlyNewRecords(string sourceFile, string newFile)
		{
			FileHelperEngine<T> fileHelperEngine = this.CreateEngineAndClearErrors();
			T[] array = fileHelperEngine.ReadFile(sourceFile);
			base.ErrorManager.AddErrors(fileHelperEngine.ErrorManager);
			T[] array2 = fileHelperEngine.ReadFile(newFile);
			base.ErrorManager.AddErrors(fileHelperEngine.ErrorManager);
			List<T> list = new List<T>();
			FileDiffEngine<T>.ApplyDiffOnlyIn1(array2, array, list);
			return list.ToArray();
		}

		public T[] OnlyMissingRecords(string sourceFile, string newFile)
		{
			FileHelperEngine<T> fileHelperEngine = this.CreateEngineAndClearErrors();
			T[] array = fileHelperEngine.ReadFile(sourceFile);
			base.ErrorManager.AddErrors(fileHelperEngine.ErrorManager);
			T[] array2 = fileHelperEngine.ReadFile(newFile);
			base.ErrorManager.AddErrors(fileHelperEngine.ErrorManager);
			List<T> list = new List<T>();
			FileDiffEngine<T>.ApplyDiffOnlyIn1(array, array2, list);
			return list.ToArray();
		}

		private FileHelperEngine<T> CreateEngineAndClearErrors()
		{
			FileHelperEngine<T> fileHelperEngine = new FileHelperEngine<T>
			{
				Encoding = base.Encoding
			};
			base.ErrorManager.ClearErrors();
			fileHelperEngine.ErrorManager.ErrorMode = base.ErrorManager.ErrorMode;
			return fileHelperEngine;
		}

		public T[] OnlyDuplicatedRecords(string file1, string file2)
		{
			FileHelperEngine<T> fileHelperEngine = this.CreateEngineAndClearErrors();
			T[] array = fileHelperEngine.ReadFile(file1);
			base.ErrorManager.AddErrors(fileHelperEngine.ErrorManager);
			T[] array2 = fileHelperEngine.ReadFile(file2);
			base.ErrorManager.AddErrors(fileHelperEngine.ErrorManager);
			List<T> list = new List<T>();
			FileDiffEngine<T>.ApplyDiffInBoth(array2, array, list);
			return list.ToArray();
		}

		private static void ApplyDiffInBoth(T[] col1, T[] col2, List<T> arr)
		{
			FileDiffEngine<T>.ApplyDiff(col1, col2, arr, true);
		}

		private static void ApplyDiffOnlyIn1(T[] col1, T[] col2, List<T> arr)
		{
			FileDiffEngine<T>.ApplyDiff(col1, col2, arr, false);
		}

		private static void ApplyDiff(T[] col1, T[] col2, List<T> arr, bool addIfIn1)
		{
			for (int i = 0; i < col1.Length; i++)
			{
				bool flag = false;
				T t = col1[i];
				for (int j = i; j < col2.Length; j++)
				{
					if (t.IsEqualRecord(col2[j]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					for (int k = 0; k < Math.Min(i, col2.Length); k++)
					{
						if (t.IsEqualRecord(col2[k]))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag == addIfIn1)
				{
					arr.Add(t);
				}
			}
		}

		public T[] OnlyNoDuplicatedRecords(string file1, string file2)
		{
			FileHelperEngine<T> fileHelperEngine = this.CreateEngineAndClearErrors();
			T[] array = fileHelperEngine.ReadFile(file1);
			base.ErrorManager.AddErrors(fileHelperEngine.ErrorManager);
			T[] array2 = fileHelperEngine.ReadFile(file2);
			base.ErrorManager.AddErrors(fileHelperEngine.ErrorManager);
			List<T> list = new List<T>();
			FileDiffEngine<T>.ApplyDiffOnlyIn1(array2, array, list);
			FileDiffEngine<T>.ApplyDiffOnlyIn1(array, array2, list);
			return list.ToArray();
		}

		public T[] WriteNewRecords(string sourceFile, string newFile, string destFile)
		{
			FileHelperEngine<T> fileHelperEngine = this.CreateEngineAndClearErrors();
			T[] array = this.OnlyNewRecords(sourceFile, newFile);
			fileHelperEngine.WriteFile(destFile, array);
			base.ErrorManager.AddErrors(fileHelperEngine.ErrorManager);
			return array;
		}
	}
}
