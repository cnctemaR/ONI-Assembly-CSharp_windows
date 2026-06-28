using System;
using ICSharpCode.SharpZipLib.Core;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class FastZipEvents
	{
		public bool OnDirectoryFailure(string directory, Exception e)
		{
			bool flag = false;
			DirectoryFailureHandler directoryFailure = this.DirectoryFailure;
			if (directoryFailure != null)
			{
				ScanFailureEventArgs e2 = new ScanFailureEventArgs(directory, e);
				directoryFailure(this, e2);
				flag = e2.ContinueRunning;
			}
			return flag;
		}

		public bool OnFileFailure(string file, Exception e)
		{
			FileFailureHandler fileFailure = this.FileFailure;
			bool flag = fileFailure != null;
			if (flag)
			{
				ScanFailureEventArgs e2 = new ScanFailureEventArgs(file, e);
				fileFailure(this, e2);
				flag = e2.ContinueRunning;
			}
			return flag;
		}

		public bool OnProcessFile(string file)
		{
			bool flag = true;
			ProcessFileHandler processFile = this.ProcessFile;
			if (processFile != null)
			{
				ScanEventArgs e = new ScanEventArgs(file);
				processFile(this, e);
				flag = e.ContinueRunning;
			}
			return flag;
		}

		public bool OnCompletedFile(string file)
		{
			bool flag = true;
			CompletedFileHandler completedFile = this.CompletedFile;
			if (completedFile != null)
			{
				ScanEventArgs e = new ScanEventArgs(file);
				completedFile(this, e);
				flag = e.ContinueRunning;
			}
			return flag;
		}

		public bool OnProcessDirectory(string directory, bool hasMatchingFiles)
		{
			bool flag = true;
			ProcessDirectoryHandler processDirectory = this.ProcessDirectory;
			if (processDirectory != null)
			{
				DirectoryEventArgs e = new DirectoryEventArgs(directory, hasMatchingFiles);
				processDirectory(this, e);
				flag = e.ContinueRunning;
			}
			return flag;
		}

		public TimeSpan ProgressInterval
		{
			get
			{
				return this.progressInterval_;
			}
			set
			{
				this.progressInterval_ = value;
			}
		}

		public ProcessDirectoryHandler ProcessDirectory;

		public ProcessFileHandler ProcessFile;

		public ProgressHandler Progress;

		public CompletedFileHandler CompletedFile;

		public DirectoryFailureHandler DirectoryFailure;

		public FileFailureHandler FileFailure;

		private TimeSpan progressInterval_ = TimeSpan.FromSeconds(3.0);
	}
}
