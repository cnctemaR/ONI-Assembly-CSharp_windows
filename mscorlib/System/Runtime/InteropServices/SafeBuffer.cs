using System;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;

namespace System.Runtime.InteropServices
{
	public abstract class SafeBuffer : SafeHandleZeroOrMinusOneIsInvalid, IDisposable
	{
		protected SafeBuffer(bool ownsHandle)
			: base(ownsHandle)
		{
		}

		[CLSCompliant(false)]
		public unsafe void Initialize(ulong numBytes)
		{
			if (numBytes == 0UL)
			{
				throw new ArgumentOutOfRangeException("numBytes");
			}
			this.inited = true;
			this.byte_length = numBytes;
			this.last_byte = (byte*)(void*)this.handle + numBytes;
		}

		[CLSCompliant(false)]
		public void Initialize(uint numElements, uint sizeOfEachElement)
		{
			this.Initialize((ulong)(numElements * sizeOfEachElement));
		}

		[CLSCompliant(false)]
		public void Initialize<T>(uint numElements) where T : struct
		{
			this.Initialize(numElements, (uint)Marshal.SizeOf(typeof(T)));
		}

		[CLSCompliant(false)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public unsafe void AcquirePointer(ref byte* pointer)
		{
			if (!this.inited)
			{
				throw new InvalidOperationException();
			}
			bool flag = false;
			base.DangerousAddRef(ref flag);
			if (flag)
			{
				pointer = (void*)this.handle;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void ReleasePointer()
		{
			if (!this.inited)
			{
				throw new InvalidOperationException();
			}
			base.DangerousRelease();
		}

		[CLSCompliant(false)]
		public ulong ByteLength
		{
			get
			{
				return this.byte_length;
			}
		}

		[CLSCompliant(false)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public unsafe T Read<T>(ulong byteOffset) where T : struct
		{
			if (!this.inited)
			{
				throw new InvalidOperationException();
			}
			byte* ptr = (byte*)(void*)this.handle + byteOffset;
			if (ptr >= this.last_byte || ptr + Marshal.SizeOf(typeof(T)) != this.last_byte)
			{
				throw new ArgumentException("byteOffset");
			}
			return (T)((object)Marshal.PtrToStructure((IntPtr)((void*)ptr), typeof(T)));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[CLSCompliant(false)]
		public unsafe void ReadArray<T>(ulong byteOffset, T[] array, int index, int count) where T : struct
		{
			if (!this.inited)
			{
				throw new InvalidOperationException();
			}
			int num = Marshal.SizeOf(typeof(T)) * count;
			byte* ptr = (byte*)(void*)this.handle + byteOffset;
			if (ptr >= this.last_byte || ptr + num != this.last_byte)
			{
				throw new ArgumentException("byteOffset");
			}
			Marshal.copy_from_unmanaged((IntPtr)((void*)ptr), index, array, count);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[CLSCompliant(false)]
		public unsafe void Write<T>(ulong byteOffset, T value) where T : struct
		{
			if (!this.inited)
			{
				throw new InvalidOperationException();
			}
			byte* ptr = (byte*)(void*)this.handle + byteOffset;
			if (ptr >= this.last_byte || ptr + Marshal.SizeOf(typeof(T)) != this.last_byte)
			{
				throw new ArgumentException("byteOffset");
			}
			Marshal.StructureToPtr<T>(value, (IntPtr)((void*)ptr), false);
		}

		[CLSCompliant(false)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public unsafe void WriteArray<T>(ulong byteOffset, T[] array, int index, int count) where T : struct
		{
			if (!this.inited)
			{
				throw new InvalidOperationException();
			}
			byte* ptr = (byte*)(void*)this.handle + byteOffset;
			int num = Marshal.SizeOf(typeof(T)) * count;
			if (ptr >= this.last_byte || ptr + num != this.last_byte)
			{
				throw new ArgumentException("would overrite");
			}
			Marshal.copy_to_unmanaged(array, index, (IntPtr)((void*)ptr), count);
		}

		private ulong byte_length;

		private unsafe byte* last_byte;

		private bool inited;
	}
}
