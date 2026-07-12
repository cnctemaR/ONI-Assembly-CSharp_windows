using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.IO.Enumeration
{
	public class FileSystemEnumerable<TResult> : IEnumerable<TResult>, IEnumerable
	{
		public FileSystemEnumerable(string directory, FileSystemEnumerable<TResult>.FindTransform transform, EnumerationOptions options = null)
		{
			if (directory == null)
			{
				throw new ArgumentNullException("directory");
			}
			this._directory = directory;
			if (transform == null)
			{
				throw new ArgumentNullException("transform");
			}
			this._transform = transform;
			this._options = options ?? EnumerationOptions.Default;
			this._enumerator = new FileSystemEnumerable<TResult>.DelegateEnumerator(this);
		}

		public FileSystemEnumerable<TResult>.FindPredicate ShouldIncludePredicate { get; set; }

		public FileSystemEnumerable<TResult>.FindPredicate ShouldRecursePredicate { get; set; }

		public IEnumerator<TResult> GetEnumerator()
		{
			return Interlocked.Exchange<FileSystemEnumerable<TResult>.DelegateEnumerator>(ref this._enumerator, null) ?? new FileSystemEnumerable<TResult>.DelegateEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private FileSystemEnumerable<TResult>.DelegateEnumerator _enumerator;

		private readonly FileSystemEnumerable<TResult>.FindTransform _transform;

		private readonly EnumerationOptions _options;

		private readonly string _directory;

		public delegate bool FindPredicate(ref FileSystemEntry entry);

		public delegate TResult FindTransform(ref FileSystemEntry entry);

		private sealed class DelegateEnumerator : FileSystemEnumerator<TResult>
		{
			public DelegateEnumerator(FileSystemEnumerable<TResult> enumerable)
				: base(enumerable._directory, enumerable._options)
			{
				this._enumerable = enumerable;
			}

			protected override TResult TransformEntry(ref FileSystemEntry entry)
			{
				return this._enumerable._transform(ref entry);
			}

			protected override bool ShouldRecurseIntoEntry(ref FileSystemEntry entry)
			{
				FileSystemEnumerable<TResult>.FindPredicate shouldRecursePredicate = this._enumerable.ShouldRecursePredicate;
				return shouldRecursePredicate == null || shouldRecursePredicate(ref entry);
			}

			protected override bool ShouldIncludeEntry(ref FileSystemEntry entry)
			{
				FileSystemEnumerable<TResult>.FindPredicate shouldIncludePredicate = this._enumerable.ShouldIncludePredicate;
				return shouldIncludePredicate == null || shouldIncludePredicate(ref entry);
			}

			private readonly FileSystemEnumerable<TResult> _enumerable;
		}
	}
}
