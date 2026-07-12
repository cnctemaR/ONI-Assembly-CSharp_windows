using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiStoragePtr
	{
		public unsafe readonly ImGuiStorage* NativePtr { get; }

		public unsafe ImGuiStoragePtr(ImGuiStorage* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiStoragePtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiStorage*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiStoragePtr(ImGuiStorage* nativePtr)
		{
			return new ImGuiStoragePtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiStorage*(ImGuiStoragePtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiStoragePtr(IntPtr nativePtr)
		{
			return new ImGuiStoragePtr(nativePtr);
		}

		public unsafe ImPtrVector<ImGuiStoragePairPtr> Data
		{
			get
			{
				return new ImPtrVector<ImGuiStoragePairPtr>(this.NativePtr->Data, Unsafe.SizeOf<ImGuiStoragePair>());
			}
		}

		public void BuildSortByKey()
		{
			ImGuiNative.ImGuiStorage_BuildSortByKey(this.NativePtr);
		}

		public void Clear()
		{
			ImGuiNative.ImGuiStorage_Clear(this.NativePtr);
		}

		public bool GetBool(uint key)
		{
			byte b = 0;
			return ImGuiNative.ImGuiStorage_GetBool(this.NativePtr, key, b) > 0;
		}

		public bool GetBool(uint key, bool default_val)
		{
			byte b = (default_val ? 1 : 0);
			return ImGuiNative.ImGuiStorage_GetBool(this.NativePtr, key, b) > 0;
		}

		public unsafe byte* GetBoolRef(uint key)
		{
			byte b = 0;
			return ImGuiNative.ImGuiStorage_GetBoolRef(this.NativePtr, key, b);
		}

		public unsafe byte* GetBoolRef(uint key, bool default_val)
		{
			byte b = (default_val ? 1 : 0);
			return ImGuiNative.ImGuiStorage_GetBoolRef(this.NativePtr, key, b);
		}

		public float GetFloat(uint key)
		{
			float num = 0f;
			return ImGuiNative.ImGuiStorage_GetFloat(this.NativePtr, key, num);
		}

		public float GetFloat(uint key, float default_val)
		{
			return ImGuiNative.ImGuiStorage_GetFloat(this.NativePtr, key, default_val);
		}

		public unsafe float* GetFloatRef(uint key)
		{
			float num = 0f;
			return ImGuiNative.ImGuiStorage_GetFloatRef(this.NativePtr, key, num);
		}

		public unsafe float* GetFloatRef(uint key, float default_val)
		{
			return ImGuiNative.ImGuiStorage_GetFloatRef(this.NativePtr, key, default_val);
		}

		public int GetInt(uint key)
		{
			int num = 0;
			return ImGuiNative.ImGuiStorage_GetInt(this.NativePtr, key, num);
		}

		public int GetInt(uint key, int default_val)
		{
			return ImGuiNative.ImGuiStorage_GetInt(this.NativePtr, key, default_val);
		}

		public unsafe int* GetIntRef(uint key)
		{
			int num = 0;
			return ImGuiNative.ImGuiStorage_GetIntRef(this.NativePtr, key, num);
		}

		public unsafe int* GetIntRef(uint key, int default_val)
		{
			return ImGuiNative.ImGuiStorage_GetIntRef(this.NativePtr, key, default_val);
		}

		public IntPtr GetVoidPtr(uint key)
		{
			return (IntPtr)ImGuiNative.ImGuiStorage_GetVoidPtr(this.NativePtr, key);
		}

		public unsafe void** GetVoidPtrRef(uint key)
		{
			void* ptr = null;
			return ImGuiNative.ImGuiStorage_GetVoidPtrRef(this.NativePtr, key, ptr);
		}

		public unsafe void** GetVoidPtrRef(uint key, IntPtr default_val)
		{
			void* ptr = default_val.ToPointer();
			return ImGuiNative.ImGuiStorage_GetVoidPtrRef(this.NativePtr, key, ptr);
		}

		public void SetAllInt(int val)
		{
			ImGuiNative.ImGuiStorage_SetAllInt(this.NativePtr, val);
		}

		public void SetBool(uint key, bool val)
		{
			byte b = (val ? 1 : 0);
			ImGuiNative.ImGuiStorage_SetBool(this.NativePtr, key, b);
		}

		public void SetFloat(uint key, float val)
		{
			ImGuiNative.ImGuiStorage_SetFloat(this.NativePtr, key, val);
		}

		public void SetInt(uint key, int val)
		{
			ImGuiNative.ImGuiStorage_SetInt(this.NativePtr, key, val);
		}

		public unsafe void SetVoidPtr(uint key, IntPtr val)
		{
			void* ptr = val.ToPointer();
			ImGuiNative.ImGuiStorage_SetVoidPtr(this.NativePtr, key, ptr);
		}
	}
}
