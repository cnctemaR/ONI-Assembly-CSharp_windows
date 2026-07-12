using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Mono.Util;

namespace Mono.Btls
{
	internal class MonoBtlsSslCtx : MonoBtlsObject
	{
		internal new MonoBtlsSslCtx.BoringSslCtxHandle Handle
		{
			get
			{
				return (MonoBtlsSslCtx.BoringSslCtxHandle)base.Handle;
			}
		}

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_ssl_ctx_new();

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_ssl_ctx_free(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_ssl_ctx_up_ref(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_ssl_ctx_initialize(IntPtr handle, IntPtr instance);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_ssl_ctx_set_debug_bio(IntPtr handle, IntPtr bio);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_ssl_ctx_set_cert_verify_callback(IntPtr handle, IntPtr func, int cert_required);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_ssl_ctx_set_cert_select_callback(IntPtr handle, IntPtr func);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_ssl_ctx_set_min_version(IntPtr handle, int version);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_ssl_ctx_set_max_version(IntPtr handle, int version);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_ssl_ctx_is_cipher_supported(IntPtr handle, short value);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_ssl_ctx_set_ciphers(IntPtr handle, int count, IntPtr data, int allow_unsupported);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_ssl_ctx_set_verify_param(IntPtr handle, IntPtr param);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_ssl_ctx_set_client_ca_list(IntPtr handle, int count, IntPtr sizes, IntPtr data);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_ssl_ctx_set_server_name_callback(IntPtr handle, IntPtr func);

		public MonoBtlsSslCtx()
			: this(new MonoBtlsSslCtx.BoringSslCtxHandle(MonoBtlsSslCtx.mono_btls_ssl_ctx_new()))
		{
		}

		internal MonoBtlsSslCtx(MonoBtlsSslCtx.BoringSslCtxHandle handle)
			: base(handle)
		{
			this.instance = GCHandle.Alloc(this);
			this.instancePtr = GCHandle.ToIntPtr(this.instance);
			MonoBtlsSslCtx.mono_btls_ssl_ctx_initialize(handle.DangerousGetHandle(), this.instancePtr);
			this.verifyFunc = new MonoBtlsSslCtx.NativeVerifyFunc(MonoBtlsSslCtx.NativeVerifyCallback);
			this.selectFunc = new MonoBtlsSslCtx.NativeSelectFunc(MonoBtlsSslCtx.NativeSelectCallback);
			this.serverNameFunc = new MonoBtlsSslCtx.NativeServerNameFunc(MonoBtlsSslCtx.NativeServerNameCallback);
			this.verifyFuncPtr = Marshal.GetFunctionPointerForDelegate<MonoBtlsSslCtx.NativeVerifyFunc>(this.verifyFunc);
			this.selectFuncPtr = Marshal.GetFunctionPointerForDelegate<MonoBtlsSslCtx.NativeSelectFunc>(this.selectFunc);
			this.serverNameFuncPtr = Marshal.GetFunctionPointerForDelegate<MonoBtlsSslCtx.NativeServerNameFunc>(this.serverNameFunc);
			this.store = new MonoBtlsX509Store(this.Handle);
		}

		internal MonoBtlsSslCtx Copy()
		{
			return new MonoBtlsSslCtx(new MonoBtlsSslCtx.BoringSslCtxHandle(MonoBtlsSslCtx.mono_btls_ssl_ctx_up_ref(this.Handle.DangerousGetHandle())));
		}

		public MonoBtlsX509Store CertificateStore
		{
			get
			{
				return this.store;
			}
		}

		private int VerifyCallback(bool preverify_ok, MonoBtlsX509StoreCtx ctx)
		{
			if (this.verifyCallback != null)
			{
				return this.verifyCallback(ctx);
			}
			return 0;
		}

		[MonoPInvokeCallback(typeof(MonoBtlsSslCtx.NativeVerifyFunc))]
		private static int NativeVerifyCallback(IntPtr instance, int preverify_ok, IntPtr store_ctx)
		{
			MonoBtlsSslCtx monoBtlsSslCtx = (MonoBtlsSslCtx)GCHandle.FromIntPtr(instance).Target;
			using (MonoBtlsX509StoreCtx monoBtlsX509StoreCtx = new MonoBtlsX509StoreCtx(preverify_ok, store_ctx))
			{
				try
				{
					return monoBtlsSslCtx.VerifyCallback(preverify_ok != 0, monoBtlsX509StoreCtx);
				}
				catch (Exception ex)
				{
					monoBtlsSslCtx.SetException(ex);
				}
			}
			return 0;
		}

		[MonoPInvokeCallback(typeof(MonoBtlsSslCtx.NativeSelectFunc))]
		private static int NativeSelectCallback(IntPtr instance, int count, IntPtr sizes, IntPtr data)
		{
			MonoBtlsSslCtx monoBtlsSslCtx = (MonoBtlsSslCtx)GCHandle.FromIntPtr(instance).Target;
			int num;
			try
			{
				string[] array = MonoBtlsSslCtx.CopyIssuers(count, sizes, data);
				if (monoBtlsSslCtx.selectCallback != null)
				{
					num = monoBtlsSslCtx.selectCallback(array);
				}
				else
				{
					num = 1;
				}
			}
			catch (Exception ex)
			{
				monoBtlsSslCtx.SetException(ex);
				num = 0;
			}
			return num;
		}

		private static string[] CopyIssuers(int count, IntPtr sizesPtr, IntPtr dataPtr)
		{
			if (count == 0 || sizesPtr == IntPtr.Zero || dataPtr == IntPtr.Zero)
			{
				return null;
			}
			int[] array = new int[count];
			Marshal.Copy(sizesPtr, array, 0, count);
			IntPtr[] array2 = new IntPtr[count];
			Marshal.Copy(dataPtr, array2, 0, count);
			string[] array3 = new string[count];
			for (int i = 0; i < count; i++)
			{
				byte[] array4 = new byte[array[i]];
				Marshal.Copy(array2[i], array4, 0, array4.Length);
				using (MonoBtlsX509Name monoBtlsX509Name = MonoBtlsX509Name.CreateFromData(array4, false))
				{
					array3[i] = MonoBtlsUtils.FormatName(monoBtlsX509Name, true, ", ", true);
				}
			}
			return array3;
		}

		public void SetDebugBio(MonoBtlsBio bio)
		{
			base.CheckThrow();
			MonoBtlsSslCtx.mono_btls_ssl_ctx_set_debug_bio(this.Handle.DangerousGetHandle(), bio.Handle.DangerousGetHandle());
		}

		public void SetVerifyCallback(MonoBtlsVerifyCallback callback, bool client_cert_required)
		{
			base.CheckThrow();
			this.verifyCallback = callback;
			MonoBtlsSslCtx.mono_btls_ssl_ctx_set_cert_verify_callback(this.Handle.DangerousGetHandle(), this.verifyFuncPtr, client_cert_required ? 1 : 0);
		}

		public void SetSelectCallback(MonoBtlsSelectCallback callback)
		{
			base.CheckThrow();
			this.selectCallback = callback;
			MonoBtlsSslCtx.mono_btls_ssl_ctx_set_cert_select_callback(this.Handle.DangerousGetHandle(), this.selectFuncPtr);
		}

		public void SetMinVersion(int version)
		{
			base.CheckThrow();
			MonoBtlsSslCtx.mono_btls_ssl_ctx_set_min_version(this.Handle.DangerousGetHandle(), version);
		}

		public void SetMaxVersion(int version)
		{
			base.CheckThrow();
			MonoBtlsSslCtx.mono_btls_ssl_ctx_set_max_version(this.Handle.DangerousGetHandle(), version);
		}

		public bool IsCipherSupported(short value)
		{
			base.CheckThrow();
			return MonoBtlsSslCtx.mono_btls_ssl_ctx_is_cipher_supported(this.Handle.DangerousGetHandle(), value) != 0;
		}

		public void SetCiphers(short[] ciphers, bool allow_unsupported)
		{
			base.CheckThrow();
			IntPtr intPtr = Marshal.AllocHGlobal(ciphers.Length * 2);
			try
			{
				Marshal.Copy(ciphers, 0, intPtr, ciphers.Length);
				int num = MonoBtlsSslCtx.mono_btls_ssl_ctx_set_ciphers(this.Handle.DangerousGetHandle(), ciphers.Length, intPtr, allow_unsupported ? 1 : 0);
				base.CheckError(num > 0, "SetCiphers");
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		public void SetVerifyParam(MonoBtlsX509VerifyParam param)
		{
			base.CheckThrow();
			int num = MonoBtlsSslCtx.mono_btls_ssl_ctx_set_verify_param(this.Handle.DangerousGetHandle(), param.Handle.DangerousGetHandle());
			base.CheckError(num, "SetVerifyParam");
		}

		public void SetClientCertificateIssuers(string[] acceptableIssuers)
		{
			base.CheckThrow();
			if (acceptableIssuers == null || acceptableIssuers.Length == 0)
			{
				return;
			}
			int num = acceptableIssuers.Length;
			new byte[num][];
			int[] array = new int[num];
			IntPtr[] array2 = new IntPtr[num];
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				for (int i = 0; i < num; i++)
				{
					byte[] rawData = new X500DistinguishedName(acceptableIssuers[i]).RawData;
					array[i] = rawData.Length;
					array2[i] = Marshal.AllocHGlobal(rawData.Length);
					Marshal.Copy(rawData, 0, array2[i], rawData.Length);
				}
				intPtr = Marshal.AllocHGlobal(num * 4);
				Marshal.Copy(array, 0, intPtr, num);
				intPtr2 = Marshal.AllocHGlobal(num * 8);
				Marshal.Copy(array2, 0, intPtr2, num);
				int num2 = MonoBtlsSslCtx.mono_btls_ssl_ctx_set_client_ca_list(this.Handle.DangerousGetHandle(), num, intPtr, intPtr2);
				base.CheckError(num2, "SetClientCertificateIssuers");
			}
			finally
			{
				for (int j = 0; j < num; j++)
				{
					if (array2[j] != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(array2[j]);
					}
				}
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr2);
				}
			}
		}

		public void SetServerNameCallback(MonoBtlsServerNameCallback callback)
		{
			base.CheckThrow();
			this.serverNameCallback = callback;
			MonoBtlsSslCtx.mono_btls_ssl_ctx_set_server_name_callback(this.Handle.DangerousGetHandle(), this.serverNameFuncPtr);
		}

		[MonoPInvokeCallback(typeof(MonoBtlsSslCtx.NativeServerNameFunc))]
		private static int NativeServerNameCallback(IntPtr instance)
		{
			MonoBtlsSslCtx monoBtlsSslCtx = (MonoBtlsSslCtx)GCHandle.FromIntPtr(instance).Target;
			int num;
			try
			{
				num = monoBtlsSslCtx.serverNameCallback();
			}
			catch (Exception ex)
			{
				monoBtlsSslCtx.SetException(ex);
				num = 0;
			}
			return num;
		}

		protected override void Close()
		{
			if (this.store != null)
			{
				this.store.Dispose();
				this.store = null;
			}
			if (this.instance.IsAllocated)
			{
				this.instance.Free();
			}
			base.Close();
		}

		private MonoBtlsSslCtx.NativeVerifyFunc verifyFunc;

		private MonoBtlsSslCtx.NativeSelectFunc selectFunc;

		private MonoBtlsSslCtx.NativeServerNameFunc serverNameFunc;

		private IntPtr verifyFuncPtr;

		private IntPtr selectFuncPtr;

		private IntPtr serverNameFuncPtr;

		private MonoBtlsVerifyCallback verifyCallback;

		private MonoBtlsSelectCallback selectCallback;

		private MonoBtlsServerNameCallback serverNameCallback;

		private MonoBtlsX509Store store;

		private GCHandle instance;

		private IntPtr instancePtr;

		internal class BoringSslCtxHandle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringSslCtxHandle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				MonoBtlsSslCtx.mono_btls_ssl_ctx_free(this.handle);
				return true;
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int NativeVerifyFunc(IntPtr instance, int preverify_ok, IntPtr ctx);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int NativeSelectFunc(IntPtr instance, int count, IntPtr sizes, IntPtr data);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int NativeServerNameFunc(IntPtr instance);
	}
}
