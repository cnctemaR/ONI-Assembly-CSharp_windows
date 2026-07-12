using System;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net.Security
{
	internal abstract class SafeFreeContextBufferChannelBinding : ChannelBinding
	{
		public override int Size
		{
			get
			{
				return this._size;
			}
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == new IntPtr(0) || this.handle == new IntPtr(-1);
			}
		}

		internal void Set(IntPtr value)
		{
			this.handle = value;
		}

		internal static SafeFreeContextBufferChannelBinding CreateEmptyHandle()
		{
			return new SafeFreeContextBufferChannelBinding_SECURITY();
		}

		public unsafe static int QueryContextChannelBinding(SafeDeleteContext phContext, global::Interop.SspiCli.ContextAttribute contextAttribute, SecPkgContext_Bindings* buffer, SafeFreeContextBufferChannelBinding refHandle)
		{
			int num = -2146893055;
			if (contextAttribute != global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_ENDPOINT_BINDINGS && contextAttribute != global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_UNIQUE_BINDINGS)
			{
				return num;
			}
			try
			{
				bool flag = false;
				phContext.DangerousAddRef(ref flag);
				num = global::Interop.SspiCli.QueryContextAttributesW(ref phContext._handle, contextAttribute, (void*)buffer);
			}
			finally
			{
				phContext.DangerousRelease();
			}
			if (num == 0 && refHandle != null)
			{
				refHandle.Set(buffer->Bindings);
				refHandle._size = buffer->BindingsLength;
			}
			if (num != 0 && refHandle != null)
			{
				refHandle.SetHandleAsInvalid();
			}
			return num;
		}

		public override string ToString()
		{
			if (this.IsInvalid)
			{
				return null;
			}
			byte[] array = new byte[this._size];
			Marshal.Copy(this.handle, array, 0, array.Length);
			return BitConverter.ToString(array).Replace('-', ' ');
		}

		private int _size;
	}
}
