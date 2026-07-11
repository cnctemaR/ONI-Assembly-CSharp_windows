using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class)]
	[Serializable]
	public class SynchronizationAttribute : ContextAttribute, IContributeClientContextSink, IContributeServerContextSink
	{
		public SynchronizationAttribute()
			: this(8, false)
		{
		}

		public SynchronizationAttribute(bool reEntrant)
			: this(8, reEntrant)
		{
		}

		public SynchronizationAttribute(int flag)
			: this(flag, false)
		{
		}

		public SynchronizationAttribute(int flag, bool reEntrant)
			: base("Synchronization")
		{
			if (flag != 1 && flag != 4 && flag != 8 && flag != 2)
			{
				throw new ArgumentException("flag");
			}
			this._bReEntrant = reEntrant;
			this._flavor = flag;
		}

		public virtual bool IsReEntrant
		{
			get
			{
				return this._bReEntrant;
			}
		}

		public virtual bool Locked
		{
			get
			{
				return this._locked;
			}
			set
			{
				if (value)
				{
					this._mutex.WaitOne();
					lock (this)
					{
						this._lockCount++;
						if (this._lockCount > 1)
						{
							this.ReleaseLock();
						}
						this._ownerThread = Thread.CurrentThread;
					}
				}
				else
				{
					lock (this)
					{
						while (this._lockCount > 0 && this._ownerThread == Thread.CurrentThread)
						{
							this._lockCount--;
							this._mutex.ReleaseMutex();
							this._ownerThread = null;
						}
					}
				}
			}
		}

		internal void AcquireLock()
		{
			this._mutex.WaitOne();
			lock (this)
			{
				this._ownerThread = Thread.CurrentThread;
				this._lockCount++;
			}
		}

		internal void ReleaseLock()
		{
			lock (this)
			{
				if (this._lockCount > 0 && this._ownerThread == Thread.CurrentThread)
				{
					this._lockCount--;
					this._mutex.ReleaseMutex();
					this._ownerThread = null;
				}
			}
		}

		[ComVisible(true)]
		public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
		{
			if (this._flavor != 1)
			{
				ctorMsg.ContextProperties.Add(this);
			}
		}

		public virtual IMessageSink GetClientContextSink(IMessageSink nextSink)
		{
			return new SynchronizedClientContextSink(nextSink, this);
		}

		public virtual IMessageSink GetServerContextSink(IMessageSink nextSink)
		{
			return new SynchronizedServerContextSink(nextSink, this);
		}

		[ComVisible(true)]
		public override bool IsContextOK(Context ctx, IConstructionCallMessage msg)
		{
			SynchronizationAttribute synchronizationAttribute = ctx.GetProperty("Synchronization") as SynchronizationAttribute;
			switch (this._flavor)
			{
			case 1:
				return synchronizationAttribute == null;
			case 2:
				return true;
			case 4:
				return synchronizationAttribute != null;
			case 8:
				return false;
			}
			return false;
		}

		internal static void ExitContext()
		{
			if (Thread.CurrentContext.IsDefaultContext)
			{
				return;
			}
			SynchronizationAttribute synchronizationAttribute = Thread.CurrentContext.GetProperty("Synchronization") as SynchronizationAttribute;
			if (synchronizationAttribute == null)
			{
				return;
			}
			synchronizationAttribute.Locked = false;
		}

		internal static void EnterContext()
		{
			if (Thread.CurrentContext.IsDefaultContext)
			{
				return;
			}
			SynchronizationAttribute synchronizationAttribute = Thread.CurrentContext.GetProperty("Synchronization") as SynchronizationAttribute;
			if (synchronizationAttribute == null)
			{
				return;
			}
			synchronizationAttribute.Locked = true;
		}

		public const int NOT_SUPPORTED = 1;

		public const int SUPPORTED = 2;

		public const int REQUIRED = 4;

		public const int REQUIRES_NEW = 8;

		private bool _bReEntrant;

		private int _flavor;

		[NonSerialized]
		private bool _locked;

		[NonSerialized]
		private int _lockCount;

		[NonSerialized]
		private Mutex _mutex = new Mutex(false);

		[NonSerialized]
		private Thread _ownerThread;
	}
}
