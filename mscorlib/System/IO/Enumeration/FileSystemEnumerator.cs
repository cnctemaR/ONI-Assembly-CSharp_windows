using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.IO.Enumeration
{
	public abstract class FileSystemEnumerator<TResult> : CriticalFinalizerObject, IEnumerator<TResult>, IDisposable, IEnumerator
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool GetData()
		{
			Interop.NtDll.IO_STATUS_BLOCK io_STATUS_BLOCK;
			int num = Interop.NtDll.NtQueryDirectoryFile(this._directoryHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, out io_STATUS_BLOCK, this._buffer, (uint)this._bufferLength, Interop.NtDll.FILE_INFORMATION_CLASS.FileFullDirectoryInformation, Interop.BOOLEAN.FALSE, null, Interop.BOOLEAN.FALSE);
			uint num2 = (uint)num;
			if (num2 == 0U)
			{
				return true;
			}
			if (num2 == 2147483654U)
			{
				this.DirectoryFinished();
				return false;
			}
			int num3 = (int)Interop.NtDll.RtlNtStatusToDosError(num);
			if ((num3 == 5 && this._options.IgnoreInaccessible) || this.ContinueOnError(num3))
			{
				this.DirectoryFinished();
				return false;
			}
			throw Win32Marshal.GetExceptionForWin32Error(num3, this._currentPath);
		}

		private IntPtr CreateRelativeDirectoryHandle(ReadOnlySpan<char> relativePath, string fullPath)
		{
			ValueTuple<int, IntPtr> valueTuple = Interop.NtDll.CreateFile(relativePath, this._directoryHandle, Interop.NtDll.CreateDisposition.FILE_OPEN, Interop.NtDll.DesiredAccess.FILE_READ_DATA | Interop.NtDll.DesiredAccess.SYNCHRONIZE, FileShare.Read | FileShare.Write | FileShare.Delete, (FileAttributes)0, (Interop.NtDll.CreateOptions)16417U, Interop.NtDll.ObjectAttributes.OBJ_CASE_INSENSITIVE);
			int item = valueTuple.Item1;
			IntPtr item2 = valueTuple.Item2;
			if (item == 0)
			{
				return item2;
			}
			int num = (int)Interop.NtDll.RtlNtStatusToDosError(item);
			if (this.ContinueOnDirectoryError(num, true))
			{
				return IntPtr.Zero;
			}
			throw Win32Marshal.GetExceptionForWin32Error(num, fullPath);
		}

		public FileSystemEnumerator(string directory, EnumerationOptions options = null)
		{
			if (directory == null)
			{
				throw new ArgumentNullException("directory");
			}
			this._originalRootDirectory = directory;
			this._rootDirectory = PathInternal.TrimEndingDirectorySeparator(Path.GetFullPath(directory));
			this._options = options ?? EnumerationOptions.Default;
			using (default(DisableMediaInsertionPrompt))
			{
				this._directoryHandle = this.CreateDirectoryHandle(this._rootDirectory, false);
				if (this._directoryHandle == IntPtr.Zero)
				{
					this._lastEntryFound = true;
				}
			}
			this._currentPath = this._rootDirectory;
			int bufferSize = this._options.BufferSize;
			this._bufferLength = ((bufferSize <= 0) ? 4096 : Math.Max(1024, bufferSize));
			try
			{
				this._buffer = Marshal.AllocHGlobal(this._bufferLength);
			}
			catch
			{
				this.CloseDirectoryHandle();
				throw;
			}
		}

		private void CloseDirectoryHandle()
		{
			IntPtr intPtr = Interlocked.Exchange(ref this._directoryHandle, IntPtr.Zero);
			if (intPtr != IntPtr.Zero)
			{
				Interop.Kernel32.CloseHandle(intPtr);
			}
		}

		private IntPtr CreateDirectoryHandle(string path, bool ignoreNotFound = false)
		{
			IntPtr intPtr = Interop.Kernel32.CreateFile_IntPtr(path, 1, FileShare.Read | FileShare.Write | FileShare.Delete, FileMode.Open, 33554432);
			if (!(intPtr == IntPtr.Zero) && !(intPtr == (IntPtr)(-1)))
			{
				return intPtr;
			}
			int num = Marshal.GetLastWin32Error();
			if (this.ContinueOnDirectoryError(num, ignoreNotFound))
			{
				return IntPtr.Zero;
			}
			if (num == 2)
			{
				num = 3;
			}
			throw Win32Marshal.GetExceptionForWin32Error(num, path);
		}

		private bool ContinueOnDirectoryError(int error, bool ignoreNotFound)
		{
			return (ignoreNotFound && (error == 2 || error == 3 || error == 267)) || (error == 5 && this._options.IgnoreInaccessible) || this.ContinueOnError(error);
		}

		public unsafe bool MoveNext()
		{
			if (this._lastEntryFound)
			{
				return false;
			}
			FileSystemEntry fileSystemEntry = default(FileSystemEntry);
			object @lock = this._lock;
			bool flag2;
			lock (@lock)
			{
				if (this._lastEntryFound)
				{
					flag2 = false;
				}
				else
				{
					for (;;)
					{
						this.FindNextEntry();
						if (this._lastEntryFound)
						{
							break;
						}
						FileSystemEntry.Initialize(ref fileSystemEntry, this._entry, this._currentPath, this._rootDirectory, this._originalRootDirectory);
						if ((this._entry->FileAttributes & this._options.AttributesToSkip) == (FileAttributes)0)
						{
							if ((this._entry->FileAttributes & FileAttributes.Directory) != (FileAttributes)0)
							{
								if (this._entry->FileName.Length <= 2 && *this._entry->FileName[0] == 46 && (this._entry->FileName.Length != 2 || *this._entry->FileName[1] == 46))
								{
									if (!this._options.ReturnSpecialDirectories)
									{
										continue;
									}
								}
								else if (this._options.RecurseSubdirectories && this.ShouldRecurseIntoEntry(ref fileSystemEntry))
								{
									string text = Path.Join(this._currentPath, this._entry->FileName);
									IntPtr intPtr = this.CreateRelativeDirectoryHandle(this._entry->FileName, text);
									if (intPtr != IntPtr.Zero)
									{
										try
										{
											if (this._pending == null)
											{
												this._pending = new Queue<ValueTuple<IntPtr, string>>();
											}
											this._pending.Enqueue(new ValueTuple<IntPtr, string>(intPtr, text));
										}
										catch
										{
											Interop.Kernel32.CloseHandle(intPtr);
											throw;
										}
									}
								}
							}
							if (this.ShouldIncludeEntry(ref fileSystemEntry))
							{
								goto Block_15;
							}
						}
					}
					return false;
					Block_15:
					this._current = this.TransformEntry(ref fileSystemEntry);
					flag2 = true;
				}
			}
			return flag2;
		}

		private unsafe void FindNextEntry()
		{
			this._entry = Interop.NtDll.FILE_FULL_DIR_INFORMATION.GetNextInfo(this._entry);
			if (this._entry != null)
			{
				return;
			}
			if (this.GetData())
			{
				this._entry = (Interop.NtDll.FILE_FULL_DIR_INFORMATION*)(void*)this._buffer;
			}
		}

		private bool DequeueNextDirectory()
		{
			if (this._pending == null || this._pending.Count == 0)
			{
				return false;
			}
			ValueTuple<IntPtr, string> valueTuple = this._pending.Dequeue();
			this._directoryHandle = valueTuple.Item1;
			this._currentPath = valueTuple.Item2;
			return true;
		}

		private void InternalDispose(bool disposing)
		{
			if (this._lock != null)
			{
				object @lock = this._lock;
				lock (@lock)
				{
					this._lastEntryFound = true;
					this.CloseDirectoryHandle();
					if (this._pending != null)
					{
						while (this._pending.Count > 0)
						{
							Interop.Kernel32.CloseHandle(this._pending.Dequeue().Item1);
						}
						this._pending = null;
					}
					if (this._buffer != (IntPtr)0)
					{
						Marshal.FreeHGlobal(this._buffer);
					}
					this._buffer = 0;
				}
			}
			this.Dispose(disposing);
		}

		protected virtual bool ShouldIncludeEntry(ref FileSystemEntry entry)
		{
			return true;
		}

		protected virtual bool ShouldRecurseIntoEntry(ref FileSystemEntry entry)
		{
			return true;
		}

		protected abstract TResult TransformEntry(ref FileSystemEntry entry);

		protected virtual void OnDirectoryFinished(ReadOnlySpan<char> directory)
		{
		}

		protected virtual bool ContinueOnError(int error)
		{
			return false;
		}

		public TResult Current
		{
			get
			{
				return this._current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		private unsafe void DirectoryFinished()
		{
			this._entry = default(Interop.NtDll.FILE_FULL_DIR_INFORMATION*);
			this.CloseDirectoryHandle();
			this.OnDirectoryFinished(this._currentPath);
			if (!this.DequeueNextDirectory())
			{
				this._lastEntryFound = true;
				return;
			}
			this.FindNextEntry();
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
			this.InternalDispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		~FileSystemEnumerator()
		{
			this.InternalDispose(false);
		}

		private const int StandardBufferSize = 4096;

		private const int MinimumBufferSize = 1024;

		private readonly string _originalRootDirectory;

		private readonly string _rootDirectory;

		private readonly EnumerationOptions _options;

		private readonly object _lock = new object();

		private unsafe Interop.NtDll.FILE_FULL_DIR_INFORMATION* _entry;

		private TResult _current;

		private IntPtr _buffer;

		private int _bufferLength;

		private IntPtr _directoryHandle;

		private string _currentPath;

		private bool _lastEntryFound;

		[TupleElementNames(new string[] { "Handle", "Path" })]
		private Queue<ValueTuple<IntPtr, string>> _pending;
	}
}
