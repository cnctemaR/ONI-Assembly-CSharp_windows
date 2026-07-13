using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Runtime
{
	[NullableContext(2)]
	[Nullable(0)]
	internal struct DependentHandle : IDisposable
	{
		public DependentHandle(object target, object dependent)
		{
			GCHandle gchandle = GCHandle.Alloc(target, GCHandleType.WeakTrackResurrection);
			this.dependentHandle = DependentHandle.AllocDepHolder(gchandle, dependent);
			GC.KeepAlive(target);
			this.allocated = true;
		}

		private static GCHandle AllocDepHolder(GCHandle targetHandle, object dependent)
		{
			return GCHandle.Alloc((dependent != null) ? new DependentHandle.DependentHolder(targetHandle, dependent) : null, GCHandleType.WeakTrackResurrection);
		}

		public bool IsAllocated
		{
			get
			{
				return this.allocated;
			}
		}

		public object Target
		{
			get
			{
				if (!this.allocated)
				{
					throw new InvalidOperationException();
				}
				return this.UnsafeGetTarget();
			}
			set
			{
				if (!this.allocated || value != null)
				{
					throw new InvalidOperationException();
				}
				this.UnsafeSetTargetToNull();
			}
		}

		public object Dependent
		{
			get
			{
				if (!this.allocated)
				{
					throw new InvalidOperationException();
				}
				DependentHandle.DependentHolder dependentHolder = this.UnsafeGetHolder();
				if (dependentHolder == null)
				{
					return null;
				}
				return dependentHolder.Dependent;
			}
			set
			{
				if (!this.allocated)
				{
					throw new InvalidOperationException();
				}
				this.UnsafeSetDependent(value);
			}
		}

		[TupleElementNames(new string[] { "Target", "Dependent" })]
		[Nullable(new byte[] { 0, 2, 2 })]
		public ValueTuple<object, object> TargetAndDependent
		{
			[return: TupleElementNames(new string[] { "Target", "Dependent" })]
			[return: Nullable(new byte[] { 0, 2, 2 })]
			get
			{
				if (!this.allocated)
				{
					throw new InvalidOperationException();
				}
				return new ValueTuple<object, object>(this.UnsafeGetTarget(), this.Dependent);
			}
		}

		private DependentHandle.DependentHolder UnsafeGetHolder()
		{
			return Unsafe.As<DependentHandle.DependentHolder>(this.dependentHandle.Target);
		}

		internal object UnsafeGetTarget()
		{
			DependentHandle.DependentHolder dependentHolder = this.UnsafeGetHolder();
			if (dependentHolder == null)
			{
				return null;
			}
			return dependentHolder.TargetHandle.Target;
		}

		internal object UnsafeGetTargetAndDependent(out object dependent)
		{
			dependent = null;
			DependentHandle.DependentHolder dependentHolder = this.UnsafeGetHolder();
			if (dependentHolder == null)
			{
				return null;
			}
			object target = dependentHolder.TargetHandle.Target;
			if (target == null)
			{
				return null;
			}
			dependent = dependentHolder.Dependent;
			return target;
		}

		internal void UnsafeSetTargetToNull()
		{
			this.Free();
		}

		internal void UnsafeSetDependent(object value)
		{
			DependentHandle.DependentHolder dependentHolder = this.UnsafeGetHolder();
			if (dependentHolder == null)
			{
				return;
			}
			if (!dependentHolder.TargetHandle.IsAllocated)
			{
				this.Free();
				return;
			}
			dependentHolder.Dependent = value;
		}

		private void FreeDependentHandle()
		{
			if (this.allocated)
			{
				DependentHandle.DependentHolder dependentHolder = this.UnsafeGetHolder();
				if (dependentHolder != null)
				{
					dependentHolder.TargetHandle.Free();
				}
				this.dependentHandle.Free();
			}
			this.allocated = false;
		}

		private void Free()
		{
			this.FreeDependentHandle();
		}

		public void Dispose()
		{
			this.Free();
			this.allocated = false;
		}

		private GCHandle dependentHandle;

		private volatile bool allocated;

		[Nullable(0)]
		private sealed class DependentHolder : CriticalFinalizerObject
		{
			public object Dependent
			{
				get
				{
					return GCHandle.FromIntPtr(this.dependent).Target;
				}
				set
				{
					IntPtr intPtr = GCHandle.ToIntPtr(GCHandle.Alloc(value, GCHandleType.Normal));
					IntPtr intPtr2;
					do
					{
						intPtr2 = this.dependent;
					}
					while (Interlocked.CompareExchange(ref this.dependent, intPtr, intPtr2) == intPtr2);
					GCHandle.FromIntPtr(intPtr2).Free();
				}
			}

			[NullableContext(1)]
			public DependentHolder(GCHandle targetHandle, object dependent)
			{
				this.TargetHandle = targetHandle;
				this.dependent = GCHandle.ToIntPtr(GCHandle.Alloc(dependent, GCHandleType.Normal));
			}

			~DependentHolder()
			{
				if (!AppDomain.CurrentDomain.IsFinalizingForUnload() && !Environment.HasShutdownStarted && this.TargetHandle.IsAllocated && this.TargetHandle.Target != null)
				{
					GC.ReRegisterForFinalize(this);
				}
				else
				{
					GCHandle.FromIntPtr(this.dependent).Free();
				}
			}

			public GCHandle TargetHandle;

			private IntPtr dependent;
		}
	}
}
