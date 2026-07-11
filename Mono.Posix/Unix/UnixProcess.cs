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
				return Syscall.WIFEXITED(this.GetProcessStatus());
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
				return Syscall.WEXITSTATUS(this.GetProcessStatus());
			}
		}

		public bool HasSignaled
		{
			get
			{
				return Syscall.WIFSIGNALED(this.GetProcessStatus());
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
				return Syscall.WTERMSIG(this.GetProcessStatus());
			}
		}

		public bool HasStopped
		{
			get
			{
				return Syscall.WIFSTOPPED(this.GetProcessStatus());
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
				return Syscall.WSTOPSIG(this.GetProcessStatus());
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
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.setpgid(this.pid, value));
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
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.kill(this.pid, signal));
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
