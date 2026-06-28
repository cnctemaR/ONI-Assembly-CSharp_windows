using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileHelpers.Events;

namespace FileHelpers
{
	public class BigFileSorter<T> where T : class, IComparable<T>
	{
		public BigFileSorter()
			: this(0)
		{
		}

		public BigFileSorter(int blockFileSizeInBytes)
			: this(null, blockFileSizeInBytes)
		{
		}

		public BigFileSorter(Encoding encoding)
			: this(encoding, 0)
		{
		}

		public BigFileSorter(Encoding encoding, int blockFileSizeInBytes)
			: this(null, encoding, blockFileSizeInBytes)
		{
		}

		internal BigFileSorter(Comparison<T> sorter, Encoding encoding, int blockFileSizeInBytes)
		{
			this.mSorter = sorter;
			if (blockFileSizeInBytes <= 0)
			{
				blockFileSizeInBytes = 10485760;
			}
			else if (blockFileSizeInBytes < 2097152)
			{
				blockFileSizeInBytes = 2097152;
			}
			else if (blockFileSizeInBytes > 52428800)
			{
				blockFileSizeInBytes = 52428800;
			}
			this.BlockFileSizeInBytes = blockFileSizeInBytes;
			if (encoding == null)
			{
				encoding = BigFileSorter<T>.DefaultEncoding;
			}
			this.Encoding = encoding;
			this.RunGcCollectForEachPart = true;
			this.DeleteTempFiles = true;
		}

		private static Encoding DefaultEncoding { get; set; } = Encoding.Default;

		public string TempDirectory { get; set; }

		public int BlockFileSizeInBytes { get; set; }

		public bool DeleteTempFiles { get; set; }

		public Encoding Encoding { get; set; }

		public bool RunGcCollectForEachPart { get; set; }

		public void Sort(string sourceFile, string destinationFile)
		{
			List<string> list = new List<string>();
			FileHelperAsyncEngine<T> fileHelperAsyncEngine = this.SplitAndSortParts(sourceFile, list);
			SortQueue<T>[] array = this.CreateQueues(list);
			this.MergeTheChunks(array, destinationFile, fileHelperAsyncEngine.HeaderText, fileHelperAsyncEngine.FooterText);
		}

		private SortQueue<T>[] CreateQueues(List<string> parts)
		{
			SortQueue<T>[] array = new SortQueue<T>[parts.Count];
			for (int i = 0; i < parts.Count; i++)
			{
				array[i] = new SortQueue<T>(this.Encoding, parts[i], this.DeleteTempFiles);
			}
			return array;
		}

		private FileHelperAsyncEngine<T> SplitAndSortParts(string file, List<string> res)
		{
			int num = 1;
			List<T> list = new List<T>();
			FileHelperAsyncEngine<T> fileHelperAsyncEngine2;
			try
			{
				long writtenBytes = 0L;
				long num2 = 0L;
				FileHelperAsyncEngine<T> fileHelperAsyncEngine = new FileHelperAsyncEngine<T>(this.Encoding);
				fileHelperAsyncEngine.Progress += delegate(object sender, ProgressEventArgs e)
				{
					writtenBytes = e.CurrentBytes;
				};
				using (fileHelperAsyncEngine.BeginReadFile(file, 204800))
				{
					foreach (T t in ((IEnumerable<T>)fileHelperAsyncEngine))
					{
						list.Add(t);
						if (writtenBytes - num2 > (long)this.BlockFileSizeInBytes)
						{
							this.WritePart(file, list, num, res);
							num++;
							num2 = writtenBytes;
						}
					}
				}
				fileHelperAsyncEngine2 = fileHelperAsyncEngine;
			}
			finally
			{
				if (list.Count > 0)
				{
					this.WritePart(file, list, num, res);
				}
			}
			return fileHelperAsyncEngine2;
		}

		private void WritePart(string file, List<T> lines, int partNumber, List<string> res)
		{
			string splitName = this.GetSplitName(file, partNumber);
			res.Add(splitName);
			if (this.mSorter != null)
			{
				lines.Sort(this.mSorter);
			}
			else
			{
				lines.Sort();
			}
			FileHelperEngine<T> fileHelperEngine = new FileHelperEngine<T>(this.Encoding)
			{
				Options = 
				{
					IgnoreFirstLines = 0,
					IgnoreLastLines = 0
				}
			};
			fileHelperEngine.WriteFile(splitName, lines);
			lines.Clear();
			if (this.RunGcCollectForEachPart)
			{
				GC.Collect();
			}
		}

		protected StreamWriter CreateStream(string filename, int bufferSize)
		{
			return new StreamWriter(filename, false, this.Encoding, Math.Min(52428800, bufferSize));
		}

		protected string GetSplitName(string file, int splitNum)
		{
			string text = this.TempDirectory;
			if (string.IsNullOrEmpty(text))
			{
				text = Path.GetDirectoryName(file);
			}
			return Path.Combine(text, Path.GetFileNameWithoutExtension(file) + ".part" + splitNum.ToString().PadLeft(4, '0'));
		}

		internal void MergeTheChunks(SortQueue<T>[] queues, string destinationFile, string headerText, string footerText)
		{
			try
			{
				using (FileHelperAsyncEngine<T> fileHelperAsyncEngine = new FileHelperAsyncEngine<T>(this.Encoding))
				{
					fileHelperAsyncEngine.HeaderText = headerText;
					fileHelperAsyncEngine.FooterText = footerText;
					fileHelperAsyncEngine.BeginWriteFile(destinationFile, 409600);
					for (;;)
					{
						int num = -1;
						T t = default(T);
						for (int i = 0; i < queues.Length; i++)
						{
							T t2 = queues[i].Current;
							if (t2 != null && (num < 0 || t2.CompareTo(t) < 0))
							{
								num = i;
								t = t2;
							}
						}
						if (num == -1)
						{
							break;
						}
						fileHelperAsyncEngine.WriteNext(t);
						queues[num].MoveNext();
					}
				}
			}
			finally
			{
				for (int j = 0; j < queues.Length; j++)
				{
					queues[j].Dispose();
				}
			}
		}

		private const int DefaultBlockSize = 10485760;

		private const int MaxBufferSize = 52428800;

		private const int MinBlockSize = 2097152;

		private readonly Comparison<T> mSorter;
	}
}
