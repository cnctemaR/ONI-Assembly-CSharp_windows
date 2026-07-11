using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Mono.Interop;

namespace System.Runtime.InteropServices
{
	public static class Marshal
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddRefInternal(IntPtr pUnk);

		public static int AddRef(IntPtr pUnk)
		{
			if (pUnk == IntPtr.Zero)
			{
				throw new ArgumentException("Value cannot be null.", "pUnk");
			}
			return Marshal.AddRefInternal(pUnk);
		}

		[MonoTODO]
		public static bool AreComObjectsAvailableForCleanup()
		{
			return false;
		}

		[MonoTODO]
		public static void CleanupUnusedObjectsInCurrentContext()
		{
			if (Environment.IsRunningOnWindows)
			{
				throw new PlatformNotSupportedException();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr AllocCoTaskMem(int cb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr AllocCoTaskMemSize(UIntPtr sizet);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr AllocHGlobal(IntPtr cb);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr AllocHGlobal(int cb)
		{
			return Marshal.AllocHGlobal((IntPtr)cb);
		}

		[MonoTODO]
		public static object BindToMoniker(string monikerName)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static void ChangeWrapperHandleStrength(object otp, bool fIsWeak)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void copy_to_unmanaged(Array source, int startIndex, IntPtr destination, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void copy_from_unmanaged(IntPtr source, int startIndex, Array destination, int length);

		public static void Copy(byte[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(char[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(short[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(int[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(long[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(float[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(double[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, byte[] destination, int startIndex, int length)
		{
			Marshal.copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, char[] destination, int startIndex, int length)
		{
			Marshal.copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, short[] destination, int startIndex, int length)
		{
			Marshal.copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, int[] destination, int startIndex, int length)
		{
			Marshal.copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, long[] destination, int startIndex, int length)
		{
			Marshal.copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, float[] destination, int startIndex, int length)
		{
			Marshal.copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, double[] destination, int startIndex, int length)
		{
			Marshal.copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, IntPtr[] destination, int startIndex, int length)
		{
			Marshal.copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static IntPtr CreateAggregatedObject(IntPtr pOuter, object o)
		{
			throw new NotImplementedException();
		}

		public static IntPtr CreateAggregatedObject<T>(IntPtr pOuter, T o)
		{
			return Marshal.CreateAggregatedObject(pOuter, o);
		}

		public static object CreateWrapperOfType(object o, Type t)
		{
			__ComObject _ComObject = o as __ComObject;
			if (_ComObject == null)
			{
				throw new ArgumentException("o must derive from __ComObject", "o");
			}
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			foreach (Type type in o.GetType().GetInterfaces())
			{
				if (type.IsImport && _ComObject.GetInterface(type) == IntPtr.Zero)
				{
					throw new InvalidCastException();
				}
			}
			return ComInteropProxy.GetProxy(_ComObject.IUnknown, t).GetTransparentProxy();
		}

		public static TWrapper CreateWrapperOfType<T, TWrapper>(T o)
		{
			return (TWrapper)((object)Marshal.CreateWrapperOfType(o, typeof(TWrapper)));
		}

		[ComVisible(true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DestroyStructure(IntPtr ptr, Type structuretype);

		public static void DestroyStructure<T>(IntPtr ptr)
		{
			Marshal.DestroyStructure(ptr, typeof(T));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FreeBSTR(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FreeCoTaskMem(IntPtr ptr);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FreeHGlobal(IntPtr hglobal);

		private static void ClearBSTR(IntPtr ptr)
		{
			int num = Marshal.ReadInt32(ptr, -4);
			for (int i = 0; i < num; i++)
			{
				Marshal.WriteByte(ptr, i, 0);
			}
		}

		public static void ZeroFreeBSTR(IntPtr s)
		{
			Marshal.ClearBSTR(s);
			Marshal.FreeBSTR(s);
		}

		private static void ClearAnsi(IntPtr ptr)
		{
			int num = 0;
			while (Marshal.ReadByte(ptr, num) != 0)
			{
				Marshal.WriteByte(ptr, num, 0);
				num++;
			}
		}

		private static void ClearUnicode(IntPtr ptr)
		{
			int num = 0;
			while (Marshal.ReadInt16(ptr, num) != 0)
			{
				Marshal.WriteInt16(ptr, num, 0);
				num += 2;
			}
		}

		public static void ZeroFreeCoTaskMemAnsi(IntPtr s)
		{
			Marshal.ClearAnsi(s);
			Marshal.FreeCoTaskMem(s);
		}

		public static void ZeroFreeCoTaskMemUnicode(IntPtr s)
		{
			Marshal.ClearUnicode(s);
			Marshal.FreeCoTaskMem(s);
		}

		public static void ZeroFreeCoTaskMemUTF8(IntPtr s)
		{
			Marshal.ClearAnsi(s);
			Marshal.FreeCoTaskMem(s);
		}

		public static void ZeroFreeGlobalAllocAnsi(IntPtr s)
		{
			Marshal.ClearAnsi(s);
			Marshal.FreeHGlobal(s);
		}

		public static void ZeroFreeGlobalAllocUnicode(IntPtr s)
		{
			Marshal.ClearUnicode(s);
			Marshal.FreeHGlobal(s);
		}

		public static Guid GenerateGuidForType(Type type)
		{
			return type.GUID;
		}

		public static string GenerateProgIdForType(Type type)
		{
			foreach (CustomAttributeData customAttributeData in CustomAttributeData.GetCustomAttributes(type))
			{
				if (customAttributeData.Constructor.DeclaringType.Name == "ProgIdAttribute")
				{
					IList<CustomAttributeTypedArgument> constructorArguments = customAttributeData.ConstructorArguments;
					string text = customAttributeData.ConstructorArguments[0].Value as string;
					if (text == null)
					{
						text = string.Empty;
					}
					return text;
				}
			}
			return type.FullName;
		}

		[MonoTODO]
		public static object GetActiveObject(string progID)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetCCW(object o, Type T);

		private static IntPtr GetComInterfaceForObjectInternal(object o, Type T)
		{
			if (Marshal.IsComObject(o))
			{
				return ((__ComObject)o).GetInterface(T);
			}
			return Marshal.GetCCW(o, T);
		}

		public static IntPtr GetComInterfaceForObject(object o, Type T)
		{
			IntPtr comInterfaceForObjectInternal = Marshal.GetComInterfaceForObjectInternal(o, T);
			Marshal.AddRef(comInterfaceForObjectInternal);
			return comInterfaceForObjectInternal;
		}

		[MonoTODO]
		public static IntPtr GetComInterfaceForObject(object o, Type T, CustomQueryInterfaceMode mode)
		{
			throw new NotImplementedException();
		}

		public static IntPtr GetComInterfaceForObject<T, TInterface>(T o)
		{
			return Marshal.GetComInterfaceForObject(o, typeof(T));
		}

		[MonoTODO]
		public static IntPtr GetComInterfaceForObjectInContext(object o, Type t)
		{
			throw new NotImplementedException();
		}

		[MonoNotSupported("MSDN states user code should never need to call this method.")]
		public static object GetComObjectData(object obj, object key)
		{
			throw new NotSupportedException("MSDN states user code should never need to call this method.");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetComSlotForMethodInfoInternal(MemberInfo m);

		public static int GetComSlotForMethodInfo(MemberInfo m)
		{
			if (m == null)
			{
				throw new ArgumentNullException("m");
			}
			if (!(m is MethodInfo))
			{
				throw new ArgumentException("The MemberInfo must be an interface method.", "m");
			}
			if (!m.DeclaringType.IsInterface)
			{
				throw new ArgumentException("The MemberInfo must be an interface method.", "m");
			}
			return Marshal.GetComSlotForMethodInfoInternal(m);
		}

		[MonoTODO]
		public static int GetEndComSlot(Type t)
		{
			throw new NotImplementedException();
		}

		[ComVisible(true)]
		[MonoTODO]
		public static IntPtr GetExceptionPointers()
		{
			throw new NotImplementedException();
		}

		public static IntPtr GetHINSTANCE(Module m)
		{
			if (m == null)
			{
				throw new ArgumentNullException("m");
			}
			return m.GetHINSTANCE();
		}

		public static int GetExceptionCode()
		{
			throw new PlatformNotSupportedException();
		}

		public static int GetHRForException(Exception e)
		{
			if (e == null)
			{
				return 0;
			}
			ManagedErrorInfo managedErrorInfo = new ManagedErrorInfo(e);
			Marshal.SetErrorInfo(0, managedErrorInfo);
			return e._HResult;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MonoTODO]
		public static int GetHRForLastWin32Error()
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetIDispatchForObjectInternal(object o);

		public static IntPtr GetIDispatchForObject(object o)
		{
			IntPtr idispatchForObjectInternal = Marshal.GetIDispatchForObjectInternal(o);
			Marshal.AddRef(idispatchForObjectInternal);
			return idispatchForObjectInternal;
		}

		[MonoTODO]
		public static IntPtr GetIDispatchForObjectInContext(object o)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static IntPtr GetITypeInfoForType(Type t)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static IntPtr GetIUnknownForObjectInContext(object o)
		{
			throw new NotImplementedException();
		}

		[Obsolete("This method has been deprecated")]
		[MonoTODO]
		public static IntPtr GetManagedThunkForUnmanagedMethodPtr(IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static MemberInfo GetMethodInfoForComSlot(Type t, int slot, ref ComMemberType memberType)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetIUnknownForObjectInternal(object o);

		public static IntPtr GetIUnknownForObject(object o)
		{
			IntPtr iunknownForObjectInternal = Marshal.GetIUnknownForObjectInternal(o);
			Marshal.AddRef(iunknownForObjectInternal);
			return iunknownForObjectInternal;
		}

		public static void GetNativeVariantForObject(object obj, IntPtr pDstNativeVariant)
		{
			Variant variant = default(Variant);
			variant.SetValue(obj);
			Marshal.StructureToPtr<Variant>(variant, pDstNativeVariant, false);
		}

		public static void GetNativeVariantForObject<T>(T obj, IntPtr pDstNativeVariant)
		{
			Marshal.GetNativeVariantForObject(obj, pDstNativeVariant);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetObjectForCCW(IntPtr pUnk);

		public static object GetObjectForIUnknown(IntPtr pUnk)
		{
			object obj = Marshal.GetObjectForCCW(pUnk);
			if (obj == null)
			{
				obj = ComInteropProxy.GetProxy(pUnk, typeof(__ComObject)).GetTransparentProxy();
			}
			return obj;
		}

		public static object GetObjectForNativeVariant(IntPtr pSrcNativeVariant)
		{
			return ((Variant)Marshal.PtrToStructure(pSrcNativeVariant, typeof(Variant))).GetValue();
		}

		public static T GetObjectForNativeVariant<T>(IntPtr pSrcNativeVariant)
		{
			return (T)((object)((Variant)Marshal.PtrToStructure(pSrcNativeVariant, typeof(Variant))).GetValue());
		}

		public static object[] GetObjectsForNativeVariants(IntPtr aSrcNativeVariant, int cVars)
		{
			if (cVars < 0)
			{
				throw new ArgumentOutOfRangeException("cVars", "cVars cannot be a negative number.");
			}
			object[] array = new object[cVars];
			for (int i = 0; i < cVars; i++)
			{
				array[i] = Marshal.GetObjectForNativeVariant((IntPtr)(aSrcNativeVariant.ToInt64() + (long)(i * Marshal.SizeOf(typeof(Variant)))));
			}
			return array;
		}

		public static T[] GetObjectsForNativeVariants<T>(IntPtr aSrcNativeVariant, int cVars)
		{
			if (cVars < 0)
			{
				throw new ArgumentOutOfRangeException("cVars", "cVars cannot be a negative number.");
			}
			T[] array = new T[cVars];
			for (int i = 0; i < cVars; i++)
			{
				array[i] = Marshal.GetObjectForNativeVariant<T>((IntPtr)(aSrcNativeVariant.ToInt64() + (long)(i * Marshal.SizeOf(typeof(Variant)))));
			}
			return array;
		}

		[MonoTODO]
		public static int GetStartComSlot(Type t)
		{
			throw new NotImplementedException();
		}

		[Obsolete("This method has been deprecated")]
		[MonoTODO]
		public static Thread GetThreadFromFiberCookie(int cookie)
		{
			throw new NotImplementedException();
		}

		public static object GetTypedObjectForIUnknown(IntPtr pUnk, Type t)
		{
			__ComObject _ComObject = (__ComObject)new ComInteropProxy(pUnk, t).GetTransparentProxy();
			foreach (Type type in t.GetInterfaces())
			{
				if ((type.Attributes & TypeAttributes.Import) == TypeAttributes.Import && _ComObject.GetInterface(type) == IntPtr.Zero)
				{
					return null;
				}
			}
			return _ComObject;
		}

		[MonoTODO]
		public static Type GetTypeForITypeInfo(IntPtr piTypeInfo)
		{
			throw new NotImplementedException();
		}

		[Obsolete]
		[MonoTODO]
		public static string GetTypeInfoName(UCOMITypeInfo pTI)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[Obsolete]
		public static Guid GetTypeLibGuid(UCOMITypeLib pTLB)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static Guid GetTypeLibGuid(ITypeLib typelib)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static Guid GetTypeLibGuidForAssembly(Assembly asm)
		{
			throw new NotImplementedException();
		}

		[Obsolete]
		[MonoTODO]
		public static int GetTypeLibLcid(UCOMITypeLib pTLB)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static int GetTypeLibLcid(ITypeLib typelib)
		{
			throw new NotImplementedException();
		}

		[Obsolete]
		[MonoTODO]
		public static string GetTypeLibName(UCOMITypeLib pTLB)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static string GetTypeLibName(ITypeLib typelib)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static void GetTypeLibVersionForAssembly(Assembly inputAssembly, out int majorVersion, out int minorVersion)
		{
			throw new NotImplementedException();
		}

		[Obsolete("This method has been deprecated")]
		[MonoTODO]
		public static IntPtr GetUnmanagedThunkForManagedMethodPtr(IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static bool IsTypeVisibleFromCom(Type t)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static int NumParamBytes(MethodInfo m)
		{
			throw new NotImplementedException();
		}

		public static Type GetTypeFromCLSID(Guid clsid)
		{
			throw new PlatformNotSupportedException();
		}

		public static string GetTypeInfoName(ITypeInfo typeInfo)
		{
			throw new PlatformNotSupportedException();
		}

		public static object GetUniqueObjectForIUnknown(IntPtr unknown)
		{
			throw new PlatformNotSupportedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsComObject(object o);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetLastWin32Error();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr OffsetOf(Type t, string fieldName);

		public static IntPtr OffsetOf<T>(string fieldName)
		{
			return Marshal.OffsetOf(typeof(T), fieldName);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Prelink(MethodInfo m);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PrelinkAll(Type c);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringAnsi(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringAnsi(IntPtr ptr, int len);

		public static string PtrToStringUTF8(IntPtr ptr)
		{
			return Marshal.PtrToStringAnsi(ptr);
		}

		public static string PtrToStringUTF8(IntPtr ptr, int byteLen)
		{
			return Marshal.PtrToStringAnsi(ptr, byteLen);
		}

		public static string PtrToStringAuto(IntPtr ptr)
		{
			if (Marshal.SystemDefaultCharSize != 2)
			{
				return Marshal.PtrToStringAnsi(ptr);
			}
			return Marshal.PtrToStringUni(ptr);
		}

		public static string PtrToStringAuto(IntPtr ptr, int len)
		{
			if (Marshal.SystemDefaultCharSize != 2)
			{
				return Marshal.PtrToStringAnsi(ptr, len);
			}
			return Marshal.PtrToStringUni(ptr, len);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringUni(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringUni(IntPtr ptr, int len);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringBSTR(IntPtr ptr);

		[ComVisible(true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PtrToStructure(IntPtr ptr, object structure);

		[ComVisible(true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object PtrToStructure(IntPtr ptr, Type structureType);

		public static void PtrToStructure<T>(IntPtr ptr, T structure)
		{
			Marshal.PtrToStructure(ptr, structure);
		}

		public static T PtrToStructure<T>(IntPtr ptr)
		{
			return (T)((object)Marshal.PtrToStructure(ptr, typeof(T)));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int QueryInterfaceInternal(IntPtr pUnk, ref Guid iid, out IntPtr ppv);

		public static int QueryInterface(IntPtr pUnk, ref Guid iid, out IntPtr ppv)
		{
			if (pUnk == IntPtr.Zero)
			{
				throw new ArgumentException("Value cannot be null.", "pUnk");
			}
			return Marshal.QueryInterfaceInternal(pUnk, ref iid, out ppv);
		}

		public unsafe static byte ReadByte(IntPtr ptr)
		{
			return *(byte*)(void*)ptr;
		}

		public unsafe static byte ReadByte(IntPtr ptr, int ofs)
		{
			return ((byte*)(void*)ptr)[ofs];
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static byte ReadByte([MarshalAs(UnmanagedType.AsAny)] [In] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		public unsafe static short ReadInt16(IntPtr ptr)
		{
			byte* ptr2 = (byte*)(void*)ptr;
			if ((ptr2 & 1U) == 0U)
			{
				return *(short*)ptr2;
			}
			short num;
			Buffer.Memcpy((byte*)(&num), (byte*)(void*)ptr, 2);
			return num;
		}

		public unsafe static short ReadInt16(IntPtr ptr, int ofs)
		{
			byte* ptr2 = (byte*)(void*)ptr + ofs;
			if ((ptr2 & 1U) == 0U)
			{
				return *(short*)ptr2;
			}
			short num;
			Buffer.Memcpy((byte*)(&num), ptr2, 2);
			return num;
		}

		[SuppressUnmanagedCodeSecurity]
		[MonoTODO]
		public static short ReadInt16([MarshalAs(UnmanagedType.AsAny)] [In] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public unsafe static int ReadInt32(IntPtr ptr)
		{
			byte* ptr2 = (byte*)(void*)ptr;
			if ((ptr2 & 3U) == 0U)
			{
				return *(int*)ptr2;
			}
			int num;
			Buffer.Memcpy((byte*)(&num), ptr2, 4);
			return num;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public unsafe static int ReadInt32(IntPtr ptr, int ofs)
		{
			byte* ptr2 = (byte*)(void*)ptr + ofs;
			if ((ptr2 & 3) == 0)
			{
				return *(int*)ptr2;
			}
			int num;
			Buffer.Memcpy((byte*)(&num), ptr2, 4);
			return num;
		}

		[SuppressUnmanagedCodeSecurity]
		[MonoTODO]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static int ReadInt32([MarshalAs(UnmanagedType.AsAny)] [In] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public unsafe static long ReadInt64(IntPtr ptr)
		{
			byte* ptr2 = (byte*)(void*)ptr;
			if ((ptr2 & 7U) == 0U)
			{
				return *(long*)(void*)ptr;
			}
			long num;
			Buffer.Memcpy((byte*)(&num), ptr2, 8);
			return num;
		}

		public unsafe static long ReadInt64(IntPtr ptr, int ofs)
		{
			byte* ptr2 = (byte*)(void*)ptr + ofs;
			if ((ptr2 & 7U) == 0U)
			{
				return *(long*)ptr2;
			}
			long num;
			Buffer.Memcpy((byte*)(&num), ptr2, 8);
			return num;
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static long ReadInt64([MarshalAs(UnmanagedType.AsAny)] [In] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr ReadIntPtr(IntPtr ptr)
		{
			if (IntPtr.Size == 4)
			{
				return (IntPtr)Marshal.ReadInt32(ptr);
			}
			return (IntPtr)Marshal.ReadInt64(ptr);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr ReadIntPtr(IntPtr ptr, int ofs)
		{
			if (IntPtr.Size == 4)
			{
				return (IntPtr)Marshal.ReadInt32(ptr, ofs);
			}
			return (IntPtr)Marshal.ReadInt64(ptr, ofs);
		}

		[MonoTODO]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr ReadIntPtr([MarshalAs(UnmanagedType.AsAny)] [In] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ReAllocCoTaskMem(IntPtr pv, int cb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ReAllocHGlobal(IntPtr pv, IntPtr cb);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ReleaseInternal(IntPtr pUnk);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static int Release(IntPtr pUnk)
		{
			if (pUnk == IntPtr.Zero)
			{
				throw new ArgumentException("Value cannot be null.", "pUnk");
			}
			return Marshal.ReleaseInternal(pUnk);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ReleaseComObjectInternal(object co);

		public static int ReleaseComObject(object o)
		{
			if (o == null)
			{
				throw new ArgumentException("Value cannot be null.", "o");
			}
			if (!Marshal.IsComObject(o))
			{
				throw new ArgumentException("Value must be a Com object.", "o");
			}
			return Marshal.ReleaseComObjectInternal(o);
		}

		[Obsolete]
		[MonoTODO]
		public static void ReleaseThreadCache()
		{
			throw new NotImplementedException();
		}

		[MonoNotSupported("MSDN states user code should never need to call this method.")]
		public static bool SetComObjectData(object obj, object key, object data)
		{
			throw new NotSupportedException("MSDN states user code should never need to call this method.");
		}

		[ComVisible(true)]
		public static int SizeOf(object structure)
		{
			return Marshal.SizeOf(structure.GetType());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int SizeOf(Type t);

		public static int SizeOf<T>()
		{
			return Marshal.SizeOf(typeof(T));
		}

		public static int SizeOf<T>(T structure)
		{
			return Marshal.SizeOf(structure.GetType());
		}

		internal static uint SizeOfType(Type type)
		{
			return (uint)Marshal.SizeOf(type);
		}

		internal static uint AlignedSizeOf<T>() where T : struct
		{
			uint num = Marshal.SizeOfType(typeof(T));
			if (num == 1U || num == 2U)
			{
				return num;
			}
			if (IntPtr.Size == 8 && num == 4U)
			{
				return num;
			}
			return (num + 3U) & 4294967292U;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr StringToBSTR(string s);

		public static IntPtr StringToCoTaskMemAnsi(string s)
		{
			return Marshal.StringToAllocatedMemoryUTF8(s);
		}

		public static IntPtr StringToCoTaskMemAuto(string s)
		{
			if (Marshal.SystemDefaultCharSize != 2)
			{
				return Marshal.StringToCoTaskMemAnsi(s);
			}
			return Marshal.StringToCoTaskMemUni(s);
		}

		public static IntPtr StringToCoTaskMemUni(string s)
		{
			int num = s.Length + 1;
			IntPtr intPtr = Marshal.AllocCoTaskMem(num * 2);
			char[] array = new char[num];
			s.CopyTo(0, array, 0, s.Length);
			array[s.Length] = '\0';
			Marshal.copy_to_unmanaged(array, 0, intPtr, num);
			return intPtr;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr StringToHGlobalAnsi(string s);

		public unsafe static IntPtr StringToAllocatedMemoryUTF8(string s)
		{
			if (s == null)
			{
				return IntPtr.Zero;
			}
			int num = (s.Length + 1) * 3;
			if (num < s.Length)
			{
				throw new ArgumentOutOfRangeException("s");
			}
			IntPtr intPtr = Marshal.AllocCoTaskMemSize(new UIntPtr((uint)(num + 1)));
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			byte* ptr = (byte*)(void*)intPtr;
			int bytesFromEncoding = s.GetBytesFromEncoding(ptr, num, Encoding.UTF8);
			ptr[bytesFromEncoding] = 0;
			return intPtr;
		}

		public static IntPtr StringToHGlobalAuto(string s)
		{
			if (Marshal.SystemDefaultCharSize != 2)
			{
				return Marshal.StringToHGlobalAnsi(s);
			}
			return Marshal.StringToHGlobalUni(s);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr StringToHGlobalUni(string s);

		public static IntPtr SecureStringToBSTR(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			byte[] buffer = s.GetBuffer();
			int length = s.Length;
			if (BitConverter.IsLittleEndian)
			{
				for (int i = 0; i < buffer.Length; i += 2)
				{
					byte b = buffer[i];
					buffer[i] = buffer[i + 1];
					buffer[i + 1] = b;
				}
			}
			return Marshal.BufferToBSTR(buffer, length);
		}

		public static IntPtr SecureStringToCoTaskMemAnsi(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			int length = s.Length;
			IntPtr intPtr = Marshal.AllocCoTaskMem(length + 1);
			byte[] array = new byte[length + 1];
			try
			{
				byte[] buffer = s.GetBuffer();
				int i = 0;
				int num = 0;
				while (i < length)
				{
					array[i] = buffer[num + 1];
					buffer[num] = 0;
					buffer[num + 1] = 0;
					i++;
					num += 2;
				}
				array[i] = 0;
				Marshal.copy_to_unmanaged(array, 0, intPtr, length + 1);
			}
			finally
			{
				int j = length;
				while (j > 0)
				{
					j--;
					array[j] = 0;
				}
			}
			return intPtr;
		}

		public static IntPtr SecureStringToCoTaskMemUnicode(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			int length = s.Length;
			IntPtr intPtr = Marshal.AllocCoTaskMem(length * 2 + 2);
			byte[] array = null;
			try
			{
				array = s.GetBuffer();
				for (int i = 0; i < length; i++)
				{
					Marshal.WriteInt16(intPtr, i * 2, (short)(((int)array[i * 2] << 8) | (int)array[i * 2 + 1]));
				}
				Marshal.WriteInt16(intPtr, array.Length, 0);
			}
			finally
			{
				if (array != null)
				{
					int j = array.Length;
					while (j > 0)
					{
						j--;
						array[j] = 0;
					}
				}
			}
			return intPtr;
		}

		public static IntPtr SecureStringToGlobalAllocAnsi(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return Marshal.SecureStringToCoTaskMemAnsi(s);
		}

		public static IntPtr SecureStringToGlobalAllocUnicode(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return Marshal.SecureStringToCoTaskMemUnicode(s);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[ComVisible(true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StructureToPtr(object structure, IntPtr ptr, bool fDeleteOld);

		public static void StructureToPtr<T>(T structure, IntPtr ptr, bool fDeleteOld)
		{
			Marshal.StructureToPtr(structure, ptr, fDeleteOld);
		}

		public static void ThrowExceptionForHR(int errorCode)
		{
			Exception exceptionForHR = Marshal.GetExceptionForHR(errorCode);
			if (exceptionForHR != null)
			{
				throw exceptionForHR;
			}
		}

		public static void ThrowExceptionForHR(int errorCode, IntPtr errorInfo)
		{
			Exception exceptionForHR = Marshal.GetExceptionForHR(errorCode, errorInfo);
			if (exceptionForHR != null)
			{
				throw exceptionForHR;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr BufferToBSTR(Array ptr, int slen);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr UnsafeAddrOfPinnedArrayElement(Array arr, int index);

		public static IntPtr UnsafeAddrOfPinnedArrayElement<T>(T[] arr, int index)
		{
			return Marshal.UnsafeAddrOfPinnedArrayElement(arr, index);
		}

		public unsafe static void WriteByte(IntPtr ptr, byte val)
		{
			*(byte*)(void*)ptr = val;
		}

		public unsafe static void WriteByte(IntPtr ptr, int ofs, byte val)
		{
			*(byte*)(void*)IntPtr.Add(ptr, ofs) = val;
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static void WriteByte([MarshalAs(UnmanagedType.AsAny)] [In] [Out] object ptr, int ofs, byte val)
		{
			throw new NotImplementedException();
		}

		public unsafe static void WriteInt16(IntPtr ptr, short val)
		{
			byte* ptr2 = (byte*)(void*)ptr;
			if ((ptr2 & 1U) == 0U)
			{
				*(short*)ptr2 = val;
				return;
			}
			Buffer.Memcpy(ptr2, (byte*)(&val), 2);
		}

		public unsafe static void WriteInt16(IntPtr ptr, int ofs, short val)
		{
			byte* ptr2 = (byte*)(void*)ptr + ofs;
			if ((ptr2 & 1U) == 0U)
			{
				*(short*)ptr2 = val;
				return;
			}
			Buffer.Memcpy(ptr2, (byte*)(&val), 2);
		}

		[SuppressUnmanagedCodeSecurity]
		[MonoTODO]
		public static void WriteInt16([MarshalAs(UnmanagedType.AsAny)] [In] [Out] object ptr, int ofs, short val)
		{
			throw new NotImplementedException();
		}

		public static void WriteInt16(IntPtr ptr, char val)
		{
			Marshal.WriteInt16(ptr, 0, (short)val);
		}

		public static void WriteInt16(IntPtr ptr, int ofs, char val)
		{
			Marshal.WriteInt16(ptr, ofs, (short)val);
		}

		[MonoTODO]
		public static void WriteInt16([In] [Out] object ptr, int ofs, char val)
		{
			throw new NotImplementedException();
		}

		public unsafe static void WriteInt32(IntPtr ptr, int val)
		{
			byte* ptr2 = (byte*)(void*)ptr;
			if ((ptr2 & 3U) == 0U)
			{
				*(int*)ptr2 = val;
				return;
			}
			Buffer.Memcpy(ptr2, (byte*)(&val), 4);
		}

		public unsafe static void WriteInt32(IntPtr ptr, int ofs, int val)
		{
			byte* ptr2 = (byte*)(void*)ptr + ofs;
			if ((ptr2 & 3U) == 0U)
			{
				*(int*)ptr2 = val;
				return;
			}
			Buffer.Memcpy(ptr2, (byte*)(&val), 4);
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static void WriteInt32([MarshalAs(UnmanagedType.AsAny)] [In] [Out] object ptr, int ofs, int val)
		{
			throw new NotImplementedException();
		}

		public unsafe static void WriteInt64(IntPtr ptr, long val)
		{
			byte* ptr2 = (byte*)(void*)ptr;
			if ((ptr2 & 7U) == 0U)
			{
				*(long*)ptr2 = val;
				return;
			}
			Buffer.Memcpy(ptr2, (byte*)(&val), 8);
		}

		public unsafe static void WriteInt64(IntPtr ptr, int ofs, long val)
		{
			byte* ptr2 = (byte*)(void*)ptr + ofs;
			if ((ptr2 & 7U) == 0U)
			{
				*(long*)ptr2 = val;
				return;
			}
			Buffer.Memcpy(ptr2, (byte*)(&val), 8);
		}

		[MonoTODO]
		[SuppressUnmanagedCodeSecurity]
		public static void WriteInt64([MarshalAs(UnmanagedType.AsAny)] [In] [Out] object ptr, int ofs, long val)
		{
			throw new NotImplementedException();
		}

		public static void WriteIntPtr(IntPtr ptr, IntPtr val)
		{
			if (IntPtr.Size == 4)
			{
				Marshal.WriteInt32(ptr, (int)val);
				return;
			}
			Marshal.WriteInt64(ptr, (long)val);
		}

		public static void WriteIntPtr(IntPtr ptr, int ofs, IntPtr val)
		{
			if (IntPtr.Size == 4)
			{
				Marshal.WriteInt32(ptr, ofs, (int)val);
				return;
			}
			Marshal.WriteInt64(ptr, ofs, (long)val);
		}

		[MonoTODO]
		public static void WriteIntPtr([MarshalAs(UnmanagedType.AsAny)] [In] [Out] object ptr, int ofs, IntPtr val)
		{
			throw new NotImplementedException();
		}

		private static Exception ConvertHrToException(int errorCode)
		{
			if (errorCode > -2147024362)
			{
				if (errorCode <= -2146232828)
				{
					if (errorCode <= -2146893792)
					{
						if (errorCode != -2147023895)
						{
							if (errorCode != -2146893792)
							{
								goto IL_03A9;
							}
							return new CryptographicException();
						}
					}
					else
					{
						if (errorCode == -2146234348)
						{
							return new AppDomainUnloadedException();
						}
						switch (errorCode)
						{
						case -2146233088:
							return new Exception();
						case -2146233087:
							return new SystemException();
						case -2146233086:
							return new ArgumentOutOfRangeException();
						case -2146233085:
							return new ArrayTypeMismatchException();
						case -2146233084:
							return new ContextMarshalException();
						case -2146233083:
						case -2146233074:
						case -2146233073:
						case -2146233061:
						case -2146233060:
						case -2146233059:
						case -2146233058:
						case -2146233057:
						case -2146233055:
						case -2146233053:
						case -2146233052:
						case -2146233051:
						case -2146233050:
						case -2146233046:
						case -2146233045:
						case -2146233044:
						case -2146233043:
						case -2146233042:
						case -2146233041:
						case -2146233040:
						case -2146233035:
						case -2146233034:
							goto IL_03A9;
						case -2146233082:
							return new ExecutionEngineException();
						case -2146233081:
							return new FieldAccessException();
						case -2146233080:
							return new IndexOutOfRangeException();
						case -2146233079:
							return new InvalidOperationException();
						case -2146233078:
							return new SecurityException();
						case -2146233077:
							return new RemotingException();
						case -2146233076:
							return new SerializationException();
						case -2146233075:
							return new VerificationException();
						case -2146233072:
							return new MethodAccessException();
						case -2146233071:
							return new MissingFieldException();
						case -2146233070:
							return new MissingMemberException();
						case -2146233069:
							return new MissingMethodException();
						case -2146233068:
							return new MulticastNotSupportedException();
						case -2146233067:
							return new NotSupportedException();
						case -2146233066:
							return new OverflowException();
						case -2146233065:
							return new RankException();
						case -2146233064:
							return new SynchronizationLockException();
						case -2146233063:
							return new ThreadInterruptedException();
						case -2146233062:
							return new MemberAccessException();
						case -2146233056:
							return new ThreadStateException();
						case -2146233054:
							return new TypeLoadException();
						case -2146233049:
							return new InvalidComObjectException();
						case -2146233048:
							return new NotFiniteNumberException();
						case -2146233047:
							return new DuplicateWaitObjectException();
						case -2146233039:
							return new InvalidOleVariantTypeException();
						case -2146233038:
							return new MissingManifestResourceException();
						case -2146233037:
							return new SafeArrayTypeMismatchException();
						case -2146233036:
							return new TypeInitializationException("", null);
						case -2146233033:
							return new FormatException();
						default:
							switch (errorCode)
							{
							case -2146232832:
								return new ApplicationException();
							case -2146232831:
								return new InvalidFilterCriteriaException();
							case -2146232830:
								return new ReflectionTypeLoadException(new Type[0], new Exception[0]);
							case -2146232829:
								return new TargetException();
							case -2146232828:
								return new TargetInvocationException(null);
							default:
								goto IL_03A9;
							}
							break;
						}
					}
				}
				else if (errorCode <= 3)
				{
					if (errorCode == -2146232800)
					{
						return new IOException();
					}
					if (errorCode == 2)
					{
						goto IL_02A6;
					}
					if (errorCode != 3)
					{
						goto IL_03A9;
					}
					goto IL_027C;
				}
				else
				{
					if (errorCode == 11)
					{
						goto IL_026A;
					}
					if (errorCode == 206)
					{
						goto IL_032A;
					}
					if (errorCode != 1001)
					{
						goto IL_03A9;
					}
				}
				return new StackOverflowException();
			}
			if (errorCode <= -2147024893)
			{
				if (errorCode <= -2147352562)
				{
					switch (errorCode)
					{
					case -2147467263:
						return new NotImplementedException();
					case -2147467262:
						return new InvalidCastException();
					case -2147467261:
						return new NullReferenceException();
					default:
						if (errorCode != -2147352562)
						{
							goto IL_03A9;
						}
						return new TargetParameterCountException();
					}
				}
				else
				{
					if (errorCode == -2147352558)
					{
						return new DivideByZeroException();
					}
					if (errorCode == -2147024894)
					{
						goto IL_02A6;
					}
					if (errorCode != -2147024893)
					{
						goto IL_03A9;
					}
					goto IL_027C;
				}
			}
			else if (errorCode <= -2147024858)
			{
				if (errorCode != -2147024885)
				{
					if (errorCode == -2147024882)
					{
						return new OutOfMemoryException();
					}
					if (errorCode != -2147024858)
					{
						goto IL_03A9;
					}
					return new EndOfStreamException();
				}
			}
			else
			{
				if (errorCode == -2147024809)
				{
					return new ArgumentException();
				}
				if (errorCode == -2147024690)
				{
					goto IL_032A;
				}
				if (errorCode != -2147024362)
				{
					goto IL_03A9;
				}
				return new ArithmeticException();
			}
			IL_026A:
			return new BadImageFormatException();
			IL_027C:
			return new DirectoryNotFoundException();
			IL_02A6:
			return new FileNotFoundException();
			IL_032A:
			return new PathTooLongException();
			IL_03A9:
			if (errorCode < 0)
			{
				return new COMException("", errorCode);
			}
			return null;
		}

		[DllImport("oleaut32.dll", CharSet = CharSet.Unicode, EntryPoint = "SetErrorInfo")]
		private static extern int _SetErrorInfo(int dwReserved, [MarshalAs(UnmanagedType.Interface)] IErrorInfo pIErrorInfo);

		[DllImport("oleaut32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetErrorInfo")]
		private static extern int _GetErrorInfo(int dwReserved, [MarshalAs(UnmanagedType.Interface)] out IErrorInfo ppIErrorInfo);

		internal static int SetErrorInfo(int dwReserved, IErrorInfo errorInfo)
		{
			int num = 0;
			errorInfo = null;
			if (Marshal.SetErrorInfoNotAvailable)
			{
				return -1;
			}
			try
			{
				num = Marshal._SetErrorInfo(dwReserved, errorInfo);
			}
			catch (Exception)
			{
				Marshal.SetErrorInfoNotAvailable = true;
			}
			return num;
		}

		internal static int GetErrorInfo(int dwReserved, out IErrorInfo errorInfo)
		{
			int num = 0;
			errorInfo = null;
			if (Marshal.GetErrorInfoNotAvailable)
			{
				return -1;
			}
			try
			{
				num = Marshal._GetErrorInfo(dwReserved, out errorInfo);
			}
			catch (Exception)
			{
				Marshal.GetErrorInfoNotAvailable = true;
			}
			return num;
		}

		public static Exception GetExceptionForHR(int errorCode)
		{
			return Marshal.GetExceptionForHR(errorCode, IntPtr.Zero);
		}

		public static Exception GetExceptionForHR(int errorCode, IntPtr errorInfo)
		{
			IErrorInfo errorInfo2 = null;
			if (errorInfo != (IntPtr)(-1))
			{
				if (errorInfo == IntPtr.Zero)
				{
					if (Marshal.GetErrorInfo(0, out errorInfo2) != 0)
					{
						errorInfo2 = null;
					}
				}
				else
				{
					errorInfo2 = Marshal.GetObjectForIUnknown(errorInfo) as IErrorInfo;
				}
			}
			if (errorInfo2 is ManagedErrorInfo && ((ManagedErrorInfo)errorInfo2).Exception._HResult == errorCode)
			{
				return ((ManagedErrorInfo)errorInfo2).Exception;
			}
			Exception ex = Marshal.ConvertHrToException(errorCode);
			if (errorInfo2 != null && ex != null)
			{
				uint num;
				errorInfo2.GetHelpContext(out num);
				string text;
				errorInfo2.GetSource(out text);
				ex.Source = text;
				errorInfo2.GetDescription(out text);
				ex.SetMessage(text);
				errorInfo2.GetHelpFile(out text);
				if (num == 0U)
				{
					ex.HelpLink = text;
				}
				else
				{
					ex.HelpLink = string.Format("{0}#{1}", text, num);
				}
			}
			return ex;
		}

		public static int FinalReleaseComObject(object o)
		{
			while (Marshal.ReleaseComObject(o) != 0)
			{
			}
			return 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Delegate GetDelegateForFunctionPointerInternal(IntPtr ptr, Type t);

		public static Delegate GetDelegateForFunctionPointer(IntPtr ptr, Type t)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			if (!t.IsSubclassOf(typeof(MulticastDelegate)) || t == typeof(MulticastDelegate))
			{
				throw new ArgumentException("Type is not a delegate", "t");
			}
			if (t.IsGenericType)
			{
				throw new ArgumentException("The specified Type must not be a generic type definition.");
			}
			if (ptr == IntPtr.Zero)
			{
				throw new ArgumentNullException("ptr");
			}
			return Marshal.GetDelegateForFunctionPointerInternal(ptr, t);
		}

		public static TDelegate GetDelegateForFunctionPointer<TDelegate>(IntPtr ptr)
		{
			return (TDelegate)((object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(TDelegate)));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetFunctionPointerForDelegateInternal(Delegate d);

		public static IntPtr GetFunctionPointerForDelegate(Delegate d)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			return Marshal.GetFunctionPointerForDelegateInternal(d);
		}

		public static IntPtr GetFunctionPointerForDelegate<TDelegate>(TDelegate d)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			return Marshal.GetFunctionPointerForDelegateInternal((Delegate)((object)d));
		}

		internal static void SetLastWin32Error(int error)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetRawIUnknownForComObjectNoAddRef(object o);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetHRForException_WinRT(Exception e);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object GetNativeActivationFactory(Type type);

		public static readonly int SystemMaxDBCSCharSize = 2;

		public static readonly int SystemDefaultCharSize = (Environment.IsRunningOnWindows ? 2 : 1);

		private static bool SetErrorInfoNotAvailable;

		private static bool GetErrorInfoNotAvailable;
	}
}
