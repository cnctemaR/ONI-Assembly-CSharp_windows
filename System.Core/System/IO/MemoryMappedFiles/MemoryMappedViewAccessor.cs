using System;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using Unity;

namespace System.IO.MemoryMappedFiles
{
	public sealed class MemoryMappedViewAccessor : UnmanagedMemoryAccessor
	{
		[SecurityCritical]
		internal MemoryMappedViewAccessor(MemoryMappedView view)
		{
			this.m_view = view;
			base.Initialize(this.m_view.ViewHandle, this.m_view.PointerOffset, this.m_view.Size, MemoryMappedFile.GetFileAccess(this.m_view.Access));
		}

		public SafeMemoryMappedViewHandle SafeMemoryMappedViewHandle
		{
			[SecurityCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				if (this.m_view == null)
				{
					return null;
				}
				return this.m_view.ViewHandle;
			}
		}

		public long PointerOffset
		{
			get
			{
				if (this.m_view == null)
				{
					throw new InvalidOperationException(global::SR.GetString("The underlying MemoryMappedView object is null."));
				}
				return this.m_view.PointerOffset;
			}
		}

		[SecuritySafeCritical]
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.m_view != null && !this.m_view.IsClosed)
				{
					this.Flush();
				}
			}
			finally
			{
				try
				{
					if (this.m_view != null)
					{
						this.m_view.Dispose();
					}
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		[SecurityCritical]
		public void Flush()
		{
			if (!base.IsOpen)
			{
				throw new ObjectDisposedException("MemoryMappedViewAccessor", global::SR.GetString("Cannot access a closed accessor."));
			}
			if (this.m_view != null)
			{
				this.m_view.Flush((IntPtr)base.Capacity);
			}
		}

		internal MemoryMappedViewAccessor()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private MemoryMappedView m_view;
	}
}
