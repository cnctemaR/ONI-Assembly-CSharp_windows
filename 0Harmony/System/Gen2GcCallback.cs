using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class Gen2GcCallback : CriticalFinalizerObject
	{
		private Gen2GcCallback(Func<bool> callback)
		{
			this._callback0 = callback;
		}

		private Gen2GcCallback(Func<object, bool> callback, object targetObj)
		{
			this._callback1 = callback;
			this._weakTargetObj = GCHandle.Alloc(targetObj, GCHandleType.Weak);
		}

		public static void Register(Func<bool> callback)
		{
			new Gen2GcCallback(callback);
		}

		public static void Register(Func<object, bool> callback, object targetObj)
		{
			new Gen2GcCallback(callback, targetObj);
		}

		protected override void Finalize()
		{
			try
			{
				if (this._weakTargetObj.IsAllocated)
				{
					object target = this._weakTargetObj.Target;
					if (target == null)
					{
						this._weakTargetObj.Free();
						return;
					}
					try
					{
						if (!this._callback1(target))
						{
							this._weakTargetObj.Free();
							return;
						}
						goto IL_005F;
					}
					catch
					{
						goto IL_005F;
					}
				}
				try
				{
					if (!this._callback0())
					{
						return;
					}
				}
				catch
				{
				}
				IL_005F:
				GC.ReRegisterForFinalize(this);
			}
			finally
			{
				base.Finalize();
			}
		}

		[Nullable(2)]
		private readonly Func<bool> _callback0;

		[Nullable(new byte[] { 2, 1 })]
		private readonly Func<object, bool> _callback1;

		private GCHandle _weakTargetObj;
	}
}
