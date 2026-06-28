using System;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixProcess
	{
		internal UnixProcess(int pid)
		{
			this.pid = pid;
		}

		public int Id
		{
			get
			{
				return this.pid;
			}
		}

		public bool HasExited
		{
			get
			{
				int processStatus = this.GetProcessStatus();
				return Syscall.WIFEXITED(processStatus);
			}
		}

		private int GetProcessStatus()
		{
			int num2;
			int num = Syscall.waitpid(this.pid, out num2, WaitOptions.WNOHANG | WaitOptions.WUNTRACED);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return num;
		}

		public int ExitCode
		{
			get
			{
				if (!this.HasExited)
				{
					throw new InvalidOperationException(Locale.GetText("Process hasn't exited"));
				}
				int processStatus = this.GetProcessStatus();
				return Syscall.WEXITSTATUS(processStatus);
			}
		}

		public bool HasSignaled
		{
			get
			{
				int processStatus = this.GetProcessStatus();
				return Syscall.WIFSIGNALED(processStatus);
			}
		}

		public Signum TerminationSignal
		{
			get
			{
				if (!this.HasSignaled)
				{
					throw new InvalidOperationException(Locale.GetText("Process wasn't terminated by a signal"));
				}
				int processStatus = this.GetProcessStatus();
				return Syscall.WTERMSIG(processStatus);
			}
		}

		public bool HasStopped
		{
			get
			{
				int processStatus = this.GetProcessStatus();
				return Syscall.WIFSTOPPED(processStatus);
			}
		}

		public Signum StopSignal
		{
			get
			{
				if (!this.HasStopped)
				{
					throw new InvalidOperationException(Locale.GetText("Process isn't stopped"));
				}
				int processStatus = this.GetProcessStatus();
				return Syscall.WSTOPSIG(processStatus);
			}
		}

		public int ProcessGroupId
		{
			get
			{
				return Syscall.getpgid(this.pid);
			}
			set
			{
				int num = Syscall.setpgid(this.pid, value);
				UnixMarshal.ThrowExceptionForLastErrorIf(num);
			}
		}

		public int SessionId
		{
			get
			{
				int num = Syscall.getsid(this.pid);
				UnixMarshal.ThrowExceptionForLastErrorIf(num);
				return num;
			}
		}

		public static UnixProcess GetCurrentProcess()
		{
			return new UnixProcess(UnixProcess.GetCurrentProcessId());
		}

		public static int GetCurrentProcessId()
		{
			return Syscall.getpid();
		}

		public void Kill()
		{
			this.Signal(Signum.SIGKILL);
		}

		[CLSCompliant(false)]
		public void Signal(Signum signal)
		{
			int num = Syscall.kill(this.pid, signal);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public void WaitForExit()
		{
			int num;
			do
			{
				int num2;
				num = Syscall.waitpid(this.pid, out num2, (WaitOptions)0);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		private int pid;
	}
}
