using System;
using System.IO;
using System.Threading;

namespace Klei
{
	public static class FileUtil
	{
		public static event global::System.Action onErrorMessage;

		public static void ErrorDialog(FileUtil.ErrorType errorType, string errorSubject, string exceptionMessage, string exceptionStackTrace)
		{
			Debug.Log(string.Format("Error encountered during file access: {0} error: {1}", errorType, errorSubject));
			FileUtil.errorType = errorType;
			FileUtil.errorSubject = errorSubject;
			FileUtil.exceptionMessage = exceptionMessage;
			FileUtil.exceptionStackTrace = exceptionStackTrace;
			if (FileUtil.onErrorMessage != null)
			{
				FileUtil.onErrorMessage();
			}
		}

		public static T DoIOFunc<T>(Func<T> io_op, int retry_count = 0)
		{
			UnauthorizedAccessException ex = null;
			IOException ex2 = null;
			Exception ex3 = null;
			for (int i = 0; i <= retry_count; i++)
			{
				try
				{
					return io_op();
				}
				catch (UnauthorizedAccessException ex)
				{
				}
				catch (IOException ex2)
				{
				}
				catch (Exception ex3)
				{
				}
				Thread.Sleep(i * 100);
			}
			if (ex != null)
			{
				throw ex;
			}
			if (ex2 != null)
			{
				throw ex2;
			}
			if (ex3 != null)
			{
				throw ex3;
			}
			throw new Exception("Unreachable code path in FileUtil::DoIOFunc()");
		}

		public static void DoIOAction(global::System.Action io_op, int retry_count = 0)
		{
			UnauthorizedAccessException ex = null;
			IOException ex2 = null;
			Exception ex3 = null;
			for (int i = 0; i <= retry_count; i++)
			{
				try
				{
					io_op();
					return;
				}
				catch (UnauthorizedAccessException ex)
				{
				}
				catch (IOException ex2)
				{
				}
				catch (Exception ex3)
				{
				}
				Thread.Sleep(i * 100);
			}
			if (ex != null)
			{
				throw ex;
			}
			if (ex2 != null)
			{
				throw ex2;
			}
			if (ex3 != null)
			{
				throw ex3;
			}
			throw new Exception("Unreachable code path in FileUtil::DoIOAction()");
		}

		public static void DoIODialog(global::System.Action io_op, string io_subject, int retry_count = 0)
		{
			try
			{
				FileUtil.DoIOAction(io_op, retry_count);
			}
			catch (UnauthorizedAccessException ex)
			{
				DebugUtil.LogArgs(new object[] { "UnauthorizedAccessException during IO on ", io_subject, ", squelching. Stack trace was:\n", ex.Message, "\n", ex.StackTrace });
				FileUtil.ErrorDialog(FileUtil.ErrorType.UnauthorizedAccess, io_subject, ex.Message, ex.StackTrace);
			}
			catch (IOException ex2)
			{
				DebugUtil.LogArgs(new object[] { "IOException during IO on ", io_subject, ", squelching. Stack trace was:\n", ex2.Message, "\n", ex2.StackTrace });
				FileUtil.ErrorDialog(FileUtil.ErrorType.IOError, io_subject, ex2.Message, ex2.StackTrace);
			}
			catch
			{
				throw;
			}
		}

		public static T DoIODialog<T>(Func<T> io_op, string io_subject, T fail_result, int retry_count = 0)
		{
			try
			{
				return FileUtil.DoIOFunc<T>(io_op, retry_count);
			}
			catch (UnauthorizedAccessException ex)
			{
				DebugUtil.LogArgs(new object[] { "UnauthorizedAccessException during IO on ", io_subject, ", squelching. Stack trace was:\n", ex.Message, "\n", ex.StackTrace });
				FileUtil.ErrorDialog(FileUtil.ErrorType.IOError, io_subject, ex.Message, ex.StackTrace);
			}
			catch (IOException ex2)
			{
				DebugUtil.LogArgs(new object[] { "IOException during IO on ", io_subject, ", squelching. Stack trace was:\n", ex2.Message, "\n", ex2.StackTrace });
				FileUtil.ErrorDialog(FileUtil.ErrorType.IOError, io_subject, ex2.Message, ex2.StackTrace);
			}
			catch
			{
				throw;
			}
			return fail_result;
		}

		public static FileStream Create(string filename, int retry_count = 0)
		{
			return FileUtil.DoIODialog<FileStream>(() => File.Create(filename), filename, null, retry_count);
		}

		public static bool CreateDirectory(string path, int retry_count = 0)
		{
			return FileUtil.DoIODialog<bool>(delegate
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return true;
			}, path, false, retry_count);
		}

		public static bool DeleteDirectory(string path, int retry_count = 0)
		{
			return FileUtil.DoIODialog<bool>(delegate
			{
				if (!Directory.Exists(path))
				{
					return true;
				}
				Directory.Delete(path, true);
				return true;
			}, path, false, retry_count);
		}

		public static bool FileExists(string filename, int retry_count = 0)
		{
			return FileUtil.DoIODialog<bool>(() => File.Exists(filename), filename, false, retry_count);
		}

		private const FileUtil.Test TEST = FileUtil.Test.NoTesting;

		private const int DEFAULT_RETRY_COUNT = 0;

		private const int RETRY_MILLISECONDS = 100;

		public static FileUtil.ErrorType errorType;

		public static string errorSubject;

		public static string exceptionMessage;

		public static string exceptionStackTrace;

		private enum Test
		{
			NoTesting,
			RetryOnce
		}

		public enum ErrorType
		{
			None,
			UnauthorizedAccess,
			IOError
		}
	}
}
