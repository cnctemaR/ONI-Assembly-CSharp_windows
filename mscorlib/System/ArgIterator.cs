using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[StructLayout(LayoutKind.Auto)]
	public struct ArgIterator
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Setup(IntPtr argsp, IntPtr start);

		public ArgIterator(RuntimeArgumentHandle arglist)
		{
			this.sig = IntPtr.Zero;
			this.args = IntPtr.Zero;
			this.next_arg = (this.num_args = 0);
			this.Setup(arglist.args, IntPtr.Zero);
		}

		[CLSCompliant(false)]
		public unsafe ArgIterator(RuntimeArgumentHandle arglist, void* ptr)
		{
			this.sig = IntPtr.Zero;
			this.args = IntPtr.Zero;
			this.next_arg = (this.num_args = 0);
			this.Setup(arglist.args, (IntPtr)ptr);
		}

		public void End()
		{
			this.next_arg = this.num_args;
		}

		public override bool Equals(object o)
		{
			throw new NotSupportedException(Locale.GetText("ArgIterator does not support Equals."));
		}

		public override int GetHashCode()
		{
			return this.sig.GetHashCode();
		}

		[CLSCompliant(false)]
		public TypedReference GetNextArg()
		{
			if (this.num_args == this.next_arg)
			{
				throw new InvalidOperationException(Locale.GetText("Invalid iterator position."));
			}
			return this.IntGetNextArg();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TypedReference IntGetNextArg();

		[CLSCompliant(false)]
		public TypedReference GetNextArg(RuntimeTypeHandle rth)
		{
			if (this.num_args == this.next_arg)
			{
				throw new InvalidOperationException(Locale.GetText("Invalid iterator position."));
			}
			return this.IntGetNextArg(rth.Value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TypedReference IntGetNextArg(IntPtr rth);

		public RuntimeTypeHandle GetNextArgType()
		{
			if (this.num_args == this.next_arg)
			{
				throw new InvalidOperationException(Locale.GetText("Invalid iterator position."));
			}
			return new RuntimeTypeHandle(this.IntGetNextArgType());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr IntGetNextArgType();

		public int GetRemainingCount()
		{
			return this.num_args - this.next_arg;
		}

		private IntPtr sig;

		private IntPtr args;

		private int next_arg;

		private int num_args;
	}
}
