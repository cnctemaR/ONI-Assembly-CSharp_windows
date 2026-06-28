using System;
using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;

namespace System.IO.Pipes
{
	internal abstract class UnixNamedPipe : IPipe
	{
		public abstract SafePipeHandle Handle { get; }

		public void WaitForPipeDrain()
		{
			throw new NotImplementedException();
		}

		public void EnsureTargetFile(string name)
		{
			if (!File.Exists(name))
			{
				int num = Syscall.mknod(name, FilePermissions.S_ISUID | FilePermissions.S_ISGID | FilePermissions.S_ISVTX | FilePermissions.S_IRUSR | FilePermissions.S_IWUSR | FilePermissions.S_IXUSR | FilePermissions.S_IRGRP | FilePermissions.S_IWGRP | FilePermissions.S_IXGRP | FilePermissions.S_IROTH | FilePermissions.S_IWOTH | FilePermissions.S_IXOTH | FilePermissions.S_IFIFO, 0UL);
				if (num != 0)
				{
					throw new IOException(string.Format("Error on creating named pipe: error code {0}", num));
				}
			}
		}

		protected void ValidateOptions(PipeOptions options, PipeTransmissionMode mode)
		{
			if ((options & PipeOptions.WriteThrough) != PipeOptions.None)
			{
				throw new NotImplementedException("WriteThrough is not supported");
			}
			if ((mode & PipeTransmissionMode.Message) != PipeTransmissionMode.Byte)
			{
				throw new NotImplementedException("Message transmission mode is not supported");
			}
			if ((options & PipeOptions.Asynchronous) != PipeOptions.None)
			{
				throw new NotImplementedException("Asynchronous pipe mode is not supported");
			}
		}

		protected string RightsToAccess(PipeAccessRights rights)
		{
			string text;
			if ((rights & PipeAccessRights.ReadData) != (PipeAccessRights)0)
			{
				if ((rights & PipeAccessRights.WriteData) != (PipeAccessRights)0)
				{
					text = "r+";
				}
				else
				{
					text = "r";
				}
			}
			else
			{
				if ((rights & PipeAccessRights.WriteData) == (PipeAccessRights)0)
				{
					throw new InvalidOperationException("The pipe must be opened to either read or write");
				}
				text = "w";
			}
			return text;
		}

		protected FileAccess RightsToFileAccess(PipeAccessRights rights)
		{
			if ((rights & PipeAccessRights.ReadData) != (PipeAccessRights)0)
			{
				if ((rights & PipeAccessRights.WriteData) != (PipeAccessRights)0)
				{
					return FileAccess.ReadWrite;
				}
				return FileAccess.Read;
			}
			else
			{
				if ((rights & PipeAccessRights.WriteData) != (PipeAccessRights)0)
				{
					return FileAccess.Write;
				}
				throw new InvalidOperationException("The pipe must be opened to either read or write");
			}
		}
	}
}
