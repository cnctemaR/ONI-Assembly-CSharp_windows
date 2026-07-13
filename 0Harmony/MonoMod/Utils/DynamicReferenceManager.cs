using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class DynamicReferenceManager
	{
		[NullableContext(0)]
		private static DataScope<DynamicReferenceCell> AllocReferenceCore([Nullable(1)] DynamicReferenceManager.Cell cell, out DynamicReferenceCell cellRef)
		{
			cellRef = default(DynamicReferenceCell);
			bool flag = false;
			try
			{
				DynamicReferenceManager.writeLock.Enter(ref flag);
				DynamicReferenceManager.Cell[] array = DynamicReferenceManager.cells;
				int num = DynamicReferenceManager.firstEmptyCell;
				if (num >= array.Length)
				{
					DynamicReferenceManager.Cell[] array2 = new DynamicReferenceManager.Cell[array.Length * 2];
					Array.Copy(array, array2, array.Length);
					array = (DynamicReferenceManager.cells = array2);
				}
				int num2 = num++;
				while (num < array.Length && array[num] != null)
				{
					num++;
				}
				DynamicReferenceManager.firstEmptyCell = num;
				Volatile.Write<DynamicReferenceManager.Cell>(ref array[num2], cell);
				cellRef = new DynamicReferenceCell(num2, cell.GetHashCode());
			}
			finally
			{
				if (flag)
				{
					DynamicReferenceManager.writeLock.Exit();
				}
			}
			return new DataScope<DynamicReferenceCell>(DynamicReferenceManager.ScopeHandler.Instance, cellRef);
		}

		[NullableContext(0)]
		private static DataScope<DynamicReferenceCell> AllocReferenceClass([Nullable(2)] object value, out DynamicReferenceCell cellRef)
		{
			return DynamicReferenceManager.AllocReferenceCore(new DynamicReferenceManager.RefCell
			{
				Value = value
			}, out cellRef);
		}

		[NullableContext(0)]
		private static DataScope<DynamicReferenceCell> AllocReferenceStruct<[Nullable(2)] T>([Nullable(1)] in T value, out DynamicReferenceCell cellRef)
		{
			return DynamicReferenceManager.AllocReferenceCore(new DynamicReferenceManager.ValueCell<T>
			{
				Value = value
			}, out cellRef);
		}

		[NullableContext(2)]
		[MethodImpl((MethodImplOptions)512)]
		[return: Nullable(0)]
		public unsafe static DataScope<DynamicReferenceCell> AllocReference<T>(in T value, out DynamicReferenceCell cellRef)
		{
			if (!typeof(T).IsValueType)
			{
				return DynamicReferenceManager.AllocReferenceClass(*Unsafe.As<T, object>(Unsafe.AsRef<T>(in value)), out cellRef);
			}
			return DynamicReferenceManager.AllocReferenceStruct<T>(in value, out cellRef);
		}

		private static DynamicReferenceManager.Cell GetCell(DynamicReferenceCell cellRef)
		{
			DynamicReferenceManager.Cell cell = Volatile.Read<DynamicReferenceManager.Cell>(ref DynamicReferenceManager.cells[cellRef.Index]);
			if (cell == null || cell.GetHashCode() != cellRef.Hash)
			{
				throw new ArgumentException("Referenced cell no longer exists", "cellRef");
			}
			return cell;
		}

		[NullableContext(2)]
		public static object GetValue(DynamicReferenceCell cellRef)
		{
			DynamicReferenceManager.Cell cell = DynamicReferenceManager.GetCell(cellRef);
			ulong num = (ulong)cell.Type;
			if (num == 0UL)
			{
				return Unsafe.As<DynamicReferenceManager.RefCell>(cell).Value;
			}
			if (num != 1UL)
			{
				throw new InvalidOperationException("Cell is not of valid type");
			}
			return Unsafe.As<DynamicReferenceManager.ValueCellBase>(cell).BoxValue();
		}

		[NullableContext(2)]
		[MethodImpl((MethodImplOptions)512)]
		private static ref T GetValueRef<T>(DynamicReferenceCell cellRef)
		{
			DynamicReferenceManager.Cell cell = DynamicReferenceManager.GetCell(cellRef);
			ulong num = (ulong)cell.Type;
			if (num == 0UL)
			{
				Helpers.Assert(!typeof(T).IsValueType, null, "!typeof(T).IsValueType");
				DynamicReferenceManager.RefCell refCell = Unsafe.As<DynamicReferenceManager.RefCell>(cell);
				object value = refCell.Value;
				bool flag = value == null || value is T;
				Helpers.Assert(flag, null, "c.Value is null or T");
				return Unsafe.As<object, T>(ref refCell.Value);
			}
			if (num != 1UL)
			{
				throw new InvalidOperationException("Cell is not of valid type");
			}
			Helpers.Assert(typeof(T).IsValueType, null, "typeof(T).IsValueType");
			return ref ((DynamicReferenceManager.ValueCell<T>)cell).Value;
		}

		[NullableContext(2)]
		[MethodImpl((MethodImplOptions)512)]
		private static ref T GetValueRefUnsafe<T>(DynamicReferenceCell cellRef)
		{
			DynamicReferenceManager.Cell cell = DynamicReferenceManager.GetCell(cellRef);
			if (default(T) == null)
			{
				return Unsafe.As<object, T>(ref Unsafe.As<DynamicReferenceManager.RefCell>(cell).Value);
			}
			return ref Unsafe.As<DynamicReferenceManager.ValueCell<T>>(cell).Value;
		}

		[NullableContext(2)]
		public unsafe static T GetValue<T>(DynamicReferenceCell cellRef)
		{
			return *DynamicReferenceManager.GetValueRef<T>(cellRef);
		}

		[NullableContext(2)]
		internal static object GetValue(int index, int hash)
		{
			return DynamicReferenceManager.GetValue(new DynamicReferenceCell(index, hash));
		}

		[NullableContext(2)]
		internal static T GetValueT<T>(int index, int hash)
		{
			return DynamicReferenceManager.GetValue<T>(new DynamicReferenceCell(index, hash));
		}

		[NullableContext(2)]
		internal unsafe static T GetValueTUnsafe<T>(int index, int hash)
		{
			return *DynamicReferenceManager.GetValueRefUnsafe<T>(new DynamicReferenceCell(index, hash));
		}

		[NullableContext(2)]
		public unsafe static void SetValue<T>(DynamicReferenceCell cellRef, in T value)
		{
			*DynamicReferenceManager.GetValueRef<T>(cellRef) = value;
		}

		public static void EmitLoadReference(this ILProcessor il, DynamicReferenceCell cellRef)
		{
			Helpers.ThrowIfArgumentNull<ILProcessor>(il, "il");
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Index);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Hash);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, il.Body.Method.Module.ImportReference(DynamicReferenceManager.Self_GetValue_ii));
		}

		public static void EmitLoadReference(this ILCursor il, DynamicReferenceCell cellRef)
		{
			Helpers.ThrowIfArgumentNull<ILCursor>(il, "il");
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Index);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Hash);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, il.Body.Method.Module.ImportReference(DynamicReferenceManager.Self_GetValue_ii));
		}

		public static void EmitLoadReference(this ILGenerator il, DynamicReferenceCell cellRef)
		{
			Helpers.ThrowIfArgumentNull<ILGenerator>(il, "il");
			il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, cellRef.Index);
			il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, cellRef.Hash);
			il.Emit(global::System.Reflection.Emit.OpCodes.Call, DynamicReferenceManager.Self_GetValue_ii);
		}

		public static void EmitLoadTypedReference(this ILProcessor il, DynamicReferenceCell cellRef, Type type)
		{
			Helpers.ThrowIfArgumentNull<ILProcessor>(il, "il");
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Index);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Hash);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, il.Body.Method.Module.ImportReference(DynamicReferenceManager.Self_GetValueT_ii.MakeGenericMethod(new Type[] { type })));
		}

		public static void EmitLoadTypedReference(this ILCursor il, DynamicReferenceCell cellRef, Type type)
		{
			Helpers.ThrowIfArgumentNull<ILCursor>(il, "il");
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Index);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Hash);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, il.Body.Method.Module.ImportReference(DynamicReferenceManager.Self_GetValueT_ii.MakeGenericMethod(new Type[] { type })));
		}

		public static void EmitLoadTypedReference(this ILGenerator il, DynamicReferenceCell cellRef, Type type)
		{
			Helpers.ThrowIfArgumentNull<ILGenerator>(il, "il");
			il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, cellRef.Index);
			il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, cellRef.Hash);
			il.Emit(global::System.Reflection.Emit.OpCodes.Call, DynamicReferenceManager.Self_GetValueT_ii.MakeGenericMethod(new Type[] { type }));
		}

		internal static void EmitLoadTypedReferenceUnsafe(this ILProcessor il, DynamicReferenceCell cellRef, Type type)
		{
			Helpers.ThrowIfArgumentNull<ILProcessor>(il, "il");
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Index);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Hash);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, il.Body.Method.Module.ImportReference(DynamicReferenceManager.Self_GetValueTUnsafe_ii.MakeGenericMethod(new Type[] { type })));
		}

		internal static void EmitLoadTypedReferenceUnsafe(this ILCursor il, DynamicReferenceCell cellRef, Type type)
		{
			Helpers.ThrowIfArgumentNull<ILCursor>(il, "il");
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Index);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, cellRef.Hash);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, il.Body.Method.Module.ImportReference(DynamicReferenceManager.Self_GetValueTUnsafe_ii.MakeGenericMethod(new Type[] { type })));
		}

		internal static void EmitLoadTypedReferenceUnsafe(this ILGenerator il, DynamicReferenceCell cellRef, Type type)
		{
			Helpers.ThrowIfArgumentNull<ILGenerator>(il, "il");
			il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, cellRef.Index);
			il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, cellRef.Hash);
			il.Emit(global::System.Reflection.Emit.OpCodes.Call, DynamicReferenceManager.Self_GetValueTUnsafe_ii.MakeGenericMethod(new Type[] { type }));
		}

		[NullableContext(0)]
		public static DataScope<DynamicReferenceCell> EmitNewReference([Nullable(1)] this ILProcessor il, [Nullable(2)] object value, out DynamicReferenceCell cellRef)
		{
			DataScope<DynamicReferenceCell> dataScope = DynamicReferenceManager.AllocReference<object>(in value, out cellRef);
			il.EmitLoadReference(cellRef);
			return dataScope;
		}

		[NullableContext(0)]
		public static DataScope<DynamicReferenceCell> EmitNewReference([Nullable(1)] this ILCursor il, [Nullable(2)] object value, out DynamicReferenceCell cellRef)
		{
			DataScope<DynamicReferenceCell> dataScope = DynamicReferenceManager.AllocReference<object>(in value, out cellRef);
			il.EmitLoadReference(cellRef);
			return dataScope;
		}

		[NullableContext(0)]
		public static DataScope<DynamicReferenceCell> EmitNewReference([Nullable(1)] this ILGenerator il, [Nullable(2)] object value, out DynamicReferenceCell cellRef)
		{
			DataScope<DynamicReferenceCell> dataScope = DynamicReferenceManager.AllocReference<object>(in value, out cellRef);
			il.EmitLoadReference(cellRef);
			return dataScope;
		}

		[NullableContext(2)]
		[return: Nullable(0)]
		public static DataScope<DynamicReferenceCell> EmitNewTypedReference<T>([Nullable(1)] this ILProcessor il, T value, out DynamicReferenceCell cellRef)
		{
			DataScope<DynamicReferenceCell> dataScope = DynamicReferenceManager.AllocReference<T>(in value, out cellRef);
			il.EmitLoadTypedReferenceUnsafe(cellRef, typeof(T));
			return dataScope;
		}

		[NullableContext(2)]
		[return: Nullable(0)]
		public static DataScope<DynamicReferenceCell> EmitNewTypedReference<T>([Nullable(1)] this ILCursor il, T value, out DynamicReferenceCell cellRef)
		{
			DataScope<DynamicReferenceCell> dataScope = DynamicReferenceManager.AllocReference<T>(in value, out cellRef);
			il.EmitLoadTypedReferenceUnsafe(cellRef, typeof(T));
			return dataScope;
		}

		[NullableContext(2)]
		[return: Nullable(0)]
		public static DataScope<DynamicReferenceCell> EmitNewTypedReference<T>([Nullable(1)] this ILGenerator il, T value, out DynamicReferenceCell cellRef)
		{
			DataScope<DynamicReferenceCell> dataScope = DynamicReferenceManager.AllocReference<T>(in value, out cellRef);
			il.EmitLoadTypedReferenceUnsafe(cellRef, typeof(T));
			return dataScope;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static DynamicReferenceManager()
		{
			MethodInfo method = typeof(DynamicReferenceManager).GetMethod("GetValue", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[]
			{
				typeof(int),
				typeof(int)
			}, null);
			if (method == null)
			{
				throw new InvalidOperationException("GetValue doesn't exist?!?!?!?");
			}
			DynamicReferenceManager.Self_GetValue_ii = method;
			MethodInfo method2 = typeof(DynamicReferenceManager).GetMethod("GetValueT", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[]
			{
				typeof(int),
				typeof(int)
			}, null);
			if (method2 == null)
			{
				throw new InvalidOperationException("GetValueT doesn't exist?!?!?!?");
			}
			DynamicReferenceManager.Self_GetValueT_ii = method2;
			MethodInfo method3 = typeof(DynamicReferenceManager).GetMethod("GetValueTUnsafe", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[]
			{
				typeof(int),
				typeof(int)
			}, null);
			if (method3 == null)
			{
				throw new InvalidOperationException("GetValueTUnsafe doesn't exist?!?!?!?");
			}
			DynamicReferenceManager.Self_GetValueTUnsafe_ii = method3;
		}

		[NativeInteger]
		private const UIntPtr RefValueCell = 0;

		[NativeInteger]
		private const UIntPtr ValueTypeCell = 1;

		private static SpinLock writeLock = new SpinLock(false);

		[Nullable(new byte[] { 1, 2 })]
		private static volatile DynamicReferenceManager.Cell[] cells = new DynamicReferenceManager.Cell[16];

		private static volatile int firstEmptyCell;

		private static readonly MethodInfo Self_GetValue_ii;

		private static readonly MethodInfo Self_GetValueT_ii;

		private static readonly MethodInfo Self_GetValueTUnsafe_ii;

		[NullableContext(0)]
		private abstract class Cell
		{
			protected Cell([NativeInteger] UIntPtr type)
			{
				this.Type = type;
			}

			[NativeInteger]
			public readonly UIntPtr Type;
		}

		[NullableContext(0)]
		private class RefCell : DynamicReferenceManager.Cell
		{
			public RefCell()
				: base((UIntPtr)((IntPtr)0))
			{
			}

			[Nullable(2)]
			public object Value;
		}

		[NullableContext(0)]
		private abstract class ValueCellBase : DynamicReferenceManager.Cell
		{
			public ValueCellBase()
				: base((UIntPtr)((IntPtr)1))
			{
			}

			[NullableContext(2)]
			public abstract object BoxValue();
		}

		[NullableContext(2)]
		[Nullable(0)]
		private class ValueCell<T> : DynamicReferenceManager.ValueCellBase
		{
			public override object BoxValue()
			{
				return this.Value;
			}

			public T Value;
		}

		[NullableContext(0)]
		private sealed class ScopeHandler : ScopeHandlerBase<DynamicReferenceCell>
		{
			public override void EndScope(DynamicReferenceCell data)
			{
				bool flag = false;
				try
				{
					DynamicReferenceManager.writeLock.Enter(ref flag);
					DynamicReferenceManager.Cell[] cells = DynamicReferenceManager.cells;
					DynamicReferenceManager.Cell cell = Volatile.Read<DynamicReferenceManager.Cell>(ref cells[data.Index]);
					if (cell != null && cell.GetHashCode() == data.Hash)
					{
						Volatile.Write<DynamicReferenceManager.Cell>(ref cells[data.Index], null);
						DynamicReferenceManager.firstEmptyCell = Math.Min(DynamicReferenceManager.firstEmptyCell, data.Index);
					}
				}
				finally
				{
					if (flag)
					{
						DynamicReferenceManager.writeLock.Exit();
					}
				}
			}

			[Nullable(1)]
			public static readonly DynamicReferenceManager.ScopeHandler Instance = new DynamicReferenceManager.ScopeHandler();
		}
	}
}
