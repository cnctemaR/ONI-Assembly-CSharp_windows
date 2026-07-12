using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net.Security
{
	internal abstract class SafeDeleteContext : SafeHandle
	{
		protected SafeDeleteContext()
			: base(IntPtr.Zero, true)
		{
			this._handle = default(global::Interop.SspiCli.CredHandle);
		}

		public override bool IsInvalid
		{
			get
			{
				return base.IsClosed || this._handle.IsZero;
			}
		}

		public override string ToString()
		{
			return this._handle.ToString();
		}

		internal unsafe static int InitializeSecurityContext(ref SafeFreeCredentials inCredentials, ref SafeDeleteContext refContext, string targetName, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, SecurityBuffer inSecBuffer, SecurityBuffer[] inSecBuffers, SecurityBuffer outSecBuffer, ref global::Interop.SspiCli.ContextFlags outFlags)
		{
			if (outSecBuffer == null)
			{
				NetEventSource.Fail(null, "outSecBuffer != null", "InitializeSecurityContext");
			}
			if (inSecBuffer != null && inSecBuffers != null)
			{
				NetEventSource.Fail(null, "inSecBuffer == null || inSecBuffers == null", "InitializeSecurityContext");
			}
			if (inCredentials == null)
			{
				throw new ArgumentNullException("inCredentials");
			}
			global::Interop.SspiCli.SecBufferDesc secBufferDesc = default(global::Interop.SspiCli.SecBufferDesc);
			bool flag = false;
			if (inSecBuffer != null)
			{
				secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(1);
				flag = true;
			}
			else if (inSecBuffers != null)
			{
				secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(inSecBuffers.Length);
				flag = true;
			}
			global::Interop.SspiCli.SecBufferDesc secBufferDesc2 = new global::Interop.SspiCli.SecBufferDesc(1);
			bool flag2 = (inFlags & global::Interop.SspiCli.ContextFlags.AllocateMemory) != global::Interop.SspiCli.ContextFlags.Zero;
			int num = -1;
			global::Interop.SspiCli.CredHandle credHandle = default(global::Interop.SspiCli.CredHandle);
			if (refContext != null)
			{
				credHandle = refContext._handle;
			}
			GCHandle[] array = null;
			GCHandle gchandle = default(GCHandle);
			SafeFreeContextBuffer safeFreeContextBuffer = null;
			try
			{
				gchandle = GCHandle.Alloc(outSecBuffer.token, GCHandleType.Pinned);
				global::Interop.SspiCli.SecBuffer[] array2 = new global::Interop.SspiCli.SecBuffer[flag ? secBufferDesc.cBuffers : 1];
				try
				{
					global::Interop.SspiCli.SecBuffer[] array3;
					void* ptr;
					if ((array3 = array2) == null || array3.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = (void*)(&array3[0]);
					}
					if (flag)
					{
						secBufferDesc.pBuffers = ptr;
						array = new GCHandle[secBufferDesc.cBuffers];
						for (int i = 0; i < secBufferDesc.cBuffers; i++)
						{
							SecurityBuffer securityBuffer = ((inSecBuffer != null) ? inSecBuffer : inSecBuffers[i]);
							if (securityBuffer != null)
							{
								array2[i].cbBuffer = securityBuffer.size;
								array2[i].BufferType = securityBuffer.type;
								if (securityBuffer.unmanagedToken != null)
								{
									array2[i].pvBuffer = securityBuffer.unmanagedToken.DangerousGetHandle();
								}
								else if (securityBuffer.token == null || securityBuffer.token.Length == 0)
								{
									array2[i].pvBuffer = IntPtr.Zero;
								}
								else
								{
									array[i] = GCHandle.Alloc(securityBuffer.token, GCHandleType.Pinned);
									array2[i].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement<byte>(securityBuffer.token, securityBuffer.offset);
								}
							}
						}
					}
					global::Interop.SspiCli.SecBuffer secBuffer = default(global::Interop.SspiCli.SecBuffer);
					secBufferDesc2.pBuffers = (void*)(&secBuffer);
					secBuffer.cbBuffer = outSecBuffer.size;
					secBuffer.BufferType = outSecBuffer.type;
					if (outSecBuffer.token == null || outSecBuffer.token.Length == 0)
					{
						secBuffer.pvBuffer = IntPtr.Zero;
					}
					else
					{
						secBuffer.pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement<byte>(outSecBuffer.token, outSecBuffer.offset);
					}
					if (flag2)
					{
						safeFreeContextBuffer = SafeFreeContextBuffer.CreateEmptyHandle();
					}
					if (refContext == null || refContext.IsInvalid)
					{
						refContext = new SafeDeleteContext_SECURITY();
					}
					if (targetName == null || targetName.Length == 0)
					{
						targetName = " ";
					}
					string ascii = SafeDeleteContext.s_idnMapping.GetAscii(targetName);
					try
					{
						fixed (string text = ascii)
						{
							char* ptr2 = text;
							if (ptr2 != null)
							{
								ptr2 += RuntimeHelpers.OffsetToStringData / 2;
							}
							num = SafeDeleteContext.MustRunInitializeSecurityContext(ref inCredentials, credHandle.IsZero ? null : ((void*)(&credHandle)), (byte*)((targetName == " ") ? null : ptr2), inFlags, endianness, flag ? (&secBufferDesc) : null, refContext, ref secBufferDesc2, ref outFlags, safeFreeContextBuffer);
						}
					}
					finally
					{
						string text = null;
					}
					if (NetEventSource.IsEnabled)
					{
						NetEventSource.Info(null, "Marshalling OUT buffer", "InitializeSecurityContext");
					}
					outSecBuffer.size = secBuffer.cbBuffer;
					outSecBuffer.type = secBuffer.BufferType;
					if (outSecBuffer.size > 0)
					{
						outSecBuffer.token = new byte[outSecBuffer.size];
						Marshal.Copy(secBuffer.pvBuffer, outSecBuffer.token, 0, outSecBuffer.size);
					}
					else
					{
						outSecBuffer.token = null;
					}
				}
				finally
				{
					global::Interop.SspiCli.SecBuffer[] array3 = null;
				}
			}
			finally
			{
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].IsAllocated)
						{
							array[j].Free();
						}
					}
				}
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
				if (safeFreeContextBuffer != null)
				{
					safeFreeContextBuffer.Dispose();
				}
			}
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Exit(null, FormattableStringFactory.Create("errorCode:0x{0:x8}, refContext:{1}", new object[] { num, refContext }), "InitializeSecurityContext");
			}
			return num;
		}

		private unsafe static int MustRunInitializeSecurityContext(ref SafeFreeCredentials inCredentials, void* inContextPtr, byte* targetName, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, global::Interop.SspiCli.SecBufferDesc* inputBuffer, SafeDeleteContext outContext, ref global::Interop.SspiCli.SecBufferDesc outputBuffer, ref global::Interop.SspiCli.ContextFlags attributes, SafeFreeContextBuffer handleTemplate)
		{
			int num = -2146893055;
			try
			{
				bool flag = false;
				inCredentials.DangerousAddRef(ref flag);
				outContext.DangerousAddRef(ref flag);
				global::Interop.SspiCli.CredHandle handle = inCredentials._handle;
				long num2;
				num = global::Interop.SspiCli.InitializeSecurityContextW(ref handle, inContextPtr, targetName, inFlags, 0, endianness, inputBuffer, 0, ref outContext._handle, ref outputBuffer, ref attributes, out num2);
			}
			finally
			{
				if (outContext._EffectiveCredential != inCredentials && ((long)num & (long)((ulong)(-2147483648))) == 0L)
				{
					if (outContext._EffectiveCredential != null)
					{
						outContext._EffectiveCredential.DangerousRelease();
					}
					outContext._EffectiveCredential = inCredentials;
				}
				else
				{
					inCredentials.DangerousRelease();
				}
				outContext.DangerousRelease();
			}
			if (handleTemplate != null)
			{
				handleTemplate.Set(((global::Interop.SspiCli.SecBuffer*)outputBuffer.pBuffers)->pvBuffer);
				if (handleTemplate.IsInvalid)
				{
					handleTemplate.SetHandleAsInvalid();
				}
			}
			if (inContextPtr == null && ((long)num & (long)((ulong)(-2147483648))) != 0L)
			{
				outContext._handle.SetToInvalid();
			}
			return num;
		}

		internal unsafe static int AcceptSecurityContext(ref SafeFreeCredentials inCredentials, ref SafeDeleteContext refContext, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, SecurityBuffer inSecBuffer, SecurityBuffer[] inSecBuffers, SecurityBuffer outSecBuffer, ref global::Interop.SspiCli.ContextFlags outFlags)
		{
			if (outSecBuffer == null)
			{
				NetEventSource.Fail(null, "outSecBuffer != null", "AcceptSecurityContext");
			}
			if (inSecBuffer != null && inSecBuffers != null)
			{
				NetEventSource.Fail(null, "inSecBuffer == null || inSecBuffers == null", "AcceptSecurityContext");
			}
			if (inCredentials == null)
			{
				throw new ArgumentNullException("inCredentials");
			}
			global::Interop.SspiCli.SecBufferDesc secBufferDesc = default(global::Interop.SspiCli.SecBufferDesc);
			bool flag = false;
			if (inSecBuffer != null)
			{
				secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(1);
				flag = true;
			}
			else if (inSecBuffers != null)
			{
				secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(inSecBuffers.Length);
				flag = true;
			}
			global::Interop.SspiCli.SecBufferDesc secBufferDesc2 = new global::Interop.SspiCli.SecBufferDesc(1);
			bool flag2 = (inFlags & global::Interop.SspiCli.ContextFlags.AllocateMemory) != global::Interop.SspiCli.ContextFlags.Zero;
			int num = -1;
			global::Interop.SspiCli.CredHandle credHandle = default(global::Interop.SspiCli.CredHandle);
			if (refContext != null)
			{
				credHandle = refContext._handle;
			}
			GCHandle[] array = null;
			GCHandle gchandle = default(GCHandle);
			SafeFreeContextBuffer safeFreeContextBuffer = null;
			try
			{
				gchandle = GCHandle.Alloc(outSecBuffer.token, GCHandleType.Pinned);
				global::Interop.SspiCli.SecBuffer[] array2 = new global::Interop.SspiCli.SecBuffer[flag ? secBufferDesc.cBuffers : 1];
				try
				{
					global::Interop.SspiCli.SecBuffer[] array3;
					void* ptr;
					if ((array3 = array2) == null || array3.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = (void*)(&array3[0]);
					}
					if (flag)
					{
						secBufferDesc.pBuffers = ptr;
						array = new GCHandle[secBufferDesc.cBuffers];
						for (int i = 0; i < secBufferDesc.cBuffers; i++)
						{
							SecurityBuffer securityBuffer = ((inSecBuffer != null) ? inSecBuffer : inSecBuffers[i]);
							if (securityBuffer != null)
							{
								array2[i].cbBuffer = securityBuffer.size;
								array2[i].BufferType = securityBuffer.type;
								if (securityBuffer.unmanagedToken != null)
								{
									array2[i].pvBuffer = securityBuffer.unmanagedToken.DangerousGetHandle();
								}
								else if (securityBuffer.token == null || securityBuffer.token.Length == 0)
								{
									array2[i].pvBuffer = IntPtr.Zero;
								}
								else
								{
									array[i] = GCHandle.Alloc(securityBuffer.token, GCHandleType.Pinned);
									array2[i].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement<byte>(securityBuffer.token, securityBuffer.offset);
								}
							}
						}
					}
					global::Interop.SspiCli.SecBuffer[] array4 = new global::Interop.SspiCli.SecBuffer[1];
					try
					{
						fixed (global::Interop.SspiCli.SecBuffer* ptr2 = &array4[0])
						{
							void* ptr3 = (void*)ptr2;
							secBufferDesc2.pBuffers = ptr3;
							array4[0].cbBuffer = outSecBuffer.size;
							array4[0].BufferType = outSecBuffer.type;
							if (outSecBuffer.token == null || outSecBuffer.token.Length == 0)
							{
								array4[0].pvBuffer = IntPtr.Zero;
							}
							else
							{
								array4[0].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement<byte>(outSecBuffer.token, outSecBuffer.offset);
							}
							if (flag2)
							{
								safeFreeContextBuffer = SafeFreeContextBuffer.CreateEmptyHandle();
							}
							if (refContext == null || refContext.IsInvalid)
							{
								refContext = new SafeDeleteContext_SECURITY();
							}
							num = SafeDeleteContext.MustRunAcceptSecurityContext_SECURITY(ref inCredentials, credHandle.IsZero ? null : ((void*)(&credHandle)), flag ? (&secBufferDesc) : null, inFlags, endianness, refContext, ref secBufferDesc2, ref outFlags, safeFreeContextBuffer);
							if (NetEventSource.IsEnabled)
							{
								NetEventSource.Info(null, "Marshaling OUT buffer", "AcceptSecurityContext");
							}
							outSecBuffer.size = array4[0].cbBuffer;
							outSecBuffer.type = array4[0].BufferType;
							if (outSecBuffer.size > 0)
							{
								outSecBuffer.token = new byte[outSecBuffer.size];
								Marshal.Copy(array4[0].pvBuffer, outSecBuffer.token, 0, outSecBuffer.size);
							}
							else
							{
								outSecBuffer.token = null;
							}
						}
					}
					finally
					{
						global::Interop.SspiCli.SecBuffer* ptr2 = null;
					}
				}
				finally
				{
					global::Interop.SspiCli.SecBuffer[] array3 = null;
				}
			}
			finally
			{
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].IsAllocated)
						{
							array[j].Free();
						}
					}
				}
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
				if (safeFreeContextBuffer != null)
				{
					safeFreeContextBuffer.Dispose();
				}
			}
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Exit(null, FormattableStringFactory.Create("errorCode:0x{0:x8}, refContext:{1}", new object[] { num, refContext }), "AcceptSecurityContext");
			}
			return num;
		}

		private unsafe static int MustRunAcceptSecurityContext_SECURITY(ref SafeFreeCredentials inCredentials, void* inContextPtr, global::Interop.SspiCli.SecBufferDesc* inputBuffer, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, SafeDeleteContext outContext, ref global::Interop.SspiCli.SecBufferDesc outputBuffer, ref global::Interop.SspiCli.ContextFlags outFlags, SafeFreeContextBuffer handleTemplate)
		{
			int num = -2146893055;
			try
			{
				bool flag = false;
				inCredentials.DangerousAddRef(ref flag);
				outContext.DangerousAddRef(ref flag);
				global::Interop.SspiCli.CredHandle handle = inCredentials._handle;
				long num2;
				num = global::Interop.SspiCli.AcceptSecurityContext(ref handle, inContextPtr, inputBuffer, inFlags, endianness, ref outContext._handle, ref outputBuffer, ref outFlags, out num2);
			}
			finally
			{
				if (outContext._EffectiveCredential != inCredentials && ((long)num & (long)((ulong)(-2147483648))) == 0L)
				{
					if (outContext._EffectiveCredential != null)
					{
						outContext._EffectiveCredential.DangerousRelease();
					}
					outContext._EffectiveCredential = inCredentials;
				}
				else
				{
					inCredentials.DangerousRelease();
				}
				outContext.DangerousRelease();
			}
			if (handleTemplate != null)
			{
				handleTemplate.Set(((global::Interop.SspiCli.SecBuffer*)outputBuffer.pBuffers)->pvBuffer);
				if (handleTemplate.IsInvalid)
				{
					handleTemplate.SetHandleAsInvalid();
				}
			}
			if (inContextPtr == null && ((long)num & (long)((ulong)(-2147483648))) != 0L)
			{
				outContext._handle.SetToInvalid();
			}
			return num;
		}

		internal unsafe static int CompleteAuthToken(ref SafeDeleteContext refContext, SecurityBuffer[] inSecBuffers)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(null, "SafeDeleteContext::CompleteAuthToken", "CompleteAuthToken");
				NetEventSource.Info(null, FormattableStringFactory.Create("    refContext       = {0}", new object[] { refContext }), "CompleteAuthToken");
				NetEventSource.Info(null, FormattableStringFactory.Create("    inSecBuffers[]   = {0}", new object[] { inSecBuffers }), "CompleteAuthToken");
			}
			if (inSecBuffers == null)
			{
				NetEventSource.Fail(null, "inSecBuffers == null", "CompleteAuthToken");
			}
			global::Interop.SspiCli.SecBufferDesc secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(inSecBuffers.Length);
			int num = -2146893055;
			GCHandle[] array = null;
			global::Interop.SspiCli.SecBuffer[] array2 = new global::Interop.SspiCli.SecBuffer[secBufferDesc.cBuffers];
			global::Interop.SspiCli.SecBuffer[] array3;
			void* ptr;
			if ((array3 = array2) == null || array3.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = (void*)(&array3[0]);
			}
			secBufferDesc.pBuffers = ptr;
			array = new GCHandle[secBufferDesc.cBuffers];
			for (int i = 0; i < secBufferDesc.cBuffers; i++)
			{
				SecurityBuffer securityBuffer = inSecBuffers[i];
				if (securityBuffer != null)
				{
					array2[i].cbBuffer = securityBuffer.size;
					array2[i].BufferType = securityBuffer.type;
					if (securityBuffer.unmanagedToken != null)
					{
						array2[i].pvBuffer = securityBuffer.unmanagedToken.DangerousGetHandle();
					}
					else if (securityBuffer.token == null || securityBuffer.token.Length == 0)
					{
						array2[i].pvBuffer = IntPtr.Zero;
					}
					else
					{
						array[i] = GCHandle.Alloc(securityBuffer.token, GCHandleType.Pinned);
						array2[i].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement<byte>(securityBuffer.token, securityBuffer.offset);
					}
				}
			}
			global::Interop.SspiCli.CredHandle credHandle = default(global::Interop.SspiCli.CredHandle);
			if (refContext != null)
			{
				credHandle = refContext._handle;
			}
			try
			{
				if (refContext == null || refContext.IsInvalid)
				{
					refContext = new SafeDeleteContext_SECURITY();
				}
				try
				{
					bool flag = false;
					refContext.DangerousAddRef(ref flag);
					num = global::Interop.SspiCli.CompleteAuthToken(credHandle.IsZero ? null : ((void*)(&credHandle)), ref secBufferDesc);
				}
				finally
				{
					refContext.DangerousRelease();
				}
			}
			finally
			{
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].IsAllocated)
						{
							array[j].Free();
						}
					}
				}
			}
			array3 = null;
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Exit(null, FormattableStringFactory.Create("unmanaged CompleteAuthToken() errorCode:0x{0:x8} refContext:{1}", new object[] { num, refContext }), "CompleteAuthToken");
			}
			return num;
		}

		internal unsafe static int ApplyControlToken(ref SafeDeleteContext refContext, SecurityBuffer[] inSecBuffers)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(null, null, "ApplyControlToken");
				NetEventSource.Info(null, FormattableStringFactory.Create("    refContext       = {0}", new object[] { refContext }), "ApplyControlToken");
				NetEventSource.Info(null, FormattableStringFactory.Create("    inSecBuffers[]   = length:{0}", new object[] { inSecBuffers.Length }), "ApplyControlToken");
			}
			if (inSecBuffers == null)
			{
				NetEventSource.Fail(null, "inSecBuffers == null", "ApplyControlToken");
			}
			global::Interop.SspiCli.SecBufferDesc secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(inSecBuffers.Length);
			int num = -2146893055;
			GCHandle[] array = null;
			global::Interop.SspiCli.SecBuffer[] array2 = new global::Interop.SspiCli.SecBuffer[secBufferDesc.cBuffers];
			global::Interop.SspiCli.SecBuffer[] array3;
			void* ptr;
			if ((array3 = array2) == null || array3.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = (void*)(&array3[0]);
			}
			secBufferDesc.pBuffers = ptr;
			array = new GCHandle[secBufferDesc.cBuffers];
			for (int i = 0; i < secBufferDesc.cBuffers; i++)
			{
				SecurityBuffer securityBuffer = inSecBuffers[i];
				if (securityBuffer != null)
				{
					array2[i].cbBuffer = securityBuffer.size;
					array2[i].BufferType = securityBuffer.type;
					if (securityBuffer.unmanagedToken != null)
					{
						array2[i].pvBuffer = securityBuffer.unmanagedToken.DangerousGetHandle();
					}
					else if (securityBuffer.token == null || securityBuffer.token.Length == 0)
					{
						array2[i].pvBuffer = IntPtr.Zero;
					}
					else
					{
						array[i] = GCHandle.Alloc(securityBuffer.token, GCHandleType.Pinned);
						array2[i].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement<byte>(securityBuffer.token, securityBuffer.offset);
					}
				}
			}
			global::Interop.SspiCli.CredHandle credHandle = default(global::Interop.SspiCli.CredHandle);
			if (refContext != null)
			{
				credHandle = refContext._handle;
			}
			try
			{
				if (refContext == null || refContext.IsInvalid)
				{
					refContext = new SafeDeleteContext_SECURITY();
				}
				try
				{
					bool flag = false;
					refContext.DangerousAddRef(ref flag);
					num = global::Interop.SspiCli.ApplyControlToken(credHandle.IsZero ? null : ((void*)(&credHandle)), ref secBufferDesc);
				}
				finally
				{
					refContext.DangerousRelease();
				}
			}
			finally
			{
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].IsAllocated)
						{
							array[j].Free();
						}
					}
				}
			}
			array3 = null;
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Exit(null, FormattableStringFactory.Create("unmanaged ApplyControlToken() errorCode:0x{0:x8} refContext: {1}", new object[] { num, refContext }), "ApplyControlToken");
			}
			return num;
		}

		internal global::Interop.SspiCli.CredHandle _handle;

		private const string dummyStr = " ";

		private static readonly byte[] s_dummyBytes = new byte[1];

		private static readonly IdnMapping s_idnMapping = new IdnMapping();

		protected SafeFreeCredentials _EffectiveCredential;
	}
}
