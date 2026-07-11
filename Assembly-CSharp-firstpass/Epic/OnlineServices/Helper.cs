using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices
{
	public static class Helper
	{
		public static int GetAllocationCount()
		{
			return Helper.s_Allocations.Count;
		}

		public static bool IsOperationComplete(Result result)
		{
			int num = Helper.EOS_EResult_IsOperationComplete(result);
			bool flag = false;
			Helper.TryMarshalGet(num, out flag);
			return flag;
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern int EOS_EResult_IsOperationComplete(Result result);

		public static string ToHexString(byte[] byteArray)
		{
			return string.Join("", byteArray.Select<byte, string>((byte b) => string.Format("{0:X2}", b)).ToArray<string>());
		}

		internal static bool TryMarshalGet<T>(T source, out T target)
		{
			target = source;
			return true;
		}

		internal static bool TryMarshalGet(IntPtr source, out IntPtr target)
		{
			target = source;
			return true;
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T target) where T : Handle
		{
			return Helper.TryConvert<T>(source, out target);
		}

		internal static bool TryMarshalGet(int source, out bool target)
		{
			return Helper.TryConvert(source, out target);
		}

		internal static bool TryMarshalGet(long source, out DateTimeOffset? target)
		{
			return Helper.TryConvert(source, out target);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T[] target, int arrayLength, bool isElementAllocated)
		{
			return Helper.TryFetch<T>(source, out target, arrayLength, isElementAllocated);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T[] target, uint arrayLength, bool isElementAllocated)
		{
			return Helper.TryFetch<T>(source, out target, (int)arrayLength, isElementAllocated);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T[] target, int arrayLength)
		{
			return Helper.TryMarshalGet<T>(source, out target, arrayLength, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T[] target, uint arrayLength)
		{
			return Helper.TryMarshalGet<T>(source, out target, arrayLength, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T? target) where T : struct
		{
			return Helper.TryFetch<T>(source, out target);
		}

		internal static bool TryMarshalGet(byte[] source, out string target)
		{
			return Helper.TryConvert(source, out target);
		}

		internal static bool TryMarshalGet(IntPtr source, out object target)
		{
			target = null;
			BoxedData boxedData;
			if (Helper.TryFetch<BoxedData>(source, out boxedData))
			{
				target = boxedData.Data;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet(IntPtr source, out string target)
		{
			return Helper.TryFetch(source, out target);
		}

		internal static bool TryMarshalGet<T, TEnum>(T source, out T target, TEnum currentEnum, TEnum comparisonEnum)
		{
			target = Helper.GetDefault<T>();
			if ((int)((object)currentEnum) == (int)((object)comparisonEnum))
			{
				target = source;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet<T, TEnum>(T source, out T? target, TEnum currentEnum, TEnum comparisonEnum) where T : struct
		{
			target = Helper.GetDefault<T?>();
			if ((int)((object)currentEnum) == (int)((object)comparisonEnum))
			{
				target = new T?(source);
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet<T, TEnum>(IntPtr source, out T target, TEnum currentEnum, TEnum comparisonEnum) where T : Handle
		{
			target = Helper.GetDefault<T>();
			return (int)((object)currentEnum) == (int)((object)comparisonEnum) && Helper.TryMarshalGet<T>(source, out target);
		}

		internal static bool TryMarshalGet<TEnum>(IntPtr source, out string target, TEnum currentEnum, TEnum comparisonEnum)
		{
			target = Helper.GetDefault<string>();
			return (int)((object)currentEnum) == (int)((object)comparisonEnum) && Helper.TryMarshalGet(source, out target);
		}

		internal static bool TryMarshalGet<TEnum>(int source, out bool? target, TEnum currentEnum, TEnum comparisonEnum)
		{
			target = Helper.GetDefault<bool?>();
			bool flag;
			if ((int)((object)currentEnum) == (int)((object)comparisonEnum) && Helper.TryConvert(source, out flag))
			{
				target = new bool?(flag);
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet<TInternal, TPublic>(IntPtr source, out TPublic target) where TInternal : struct where TPublic : class, new()
		{
			target = default(TPublic);
			TInternal tinternal;
			if (Helper.TryFetch<TInternal>(source, out tinternal))
			{
				target = Helper.CopyProperties<TPublic>(tinternal);
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet<TCallbackInfoInternal, TCallbackInfo>(IntPtr callbackInfoAddress, out TCallbackInfo callbackInfo, out IntPtr clientDataAddress) where TCallbackInfoInternal : struct, ICallbackInfo where TCallbackInfo : class, new()
		{
			callbackInfo = default(TCallbackInfo);
			clientDataAddress = IntPtr.Zero;
			TCallbackInfoInternal tcallbackInfoInternal;
			if (Helper.TryFetch<TCallbackInfoInternal>(callbackInfoAddress, out tcallbackInfoInternal))
			{
				callbackInfo = Helper.CopyProperties<TCallbackInfo>(tcallbackInfoInternal);
				clientDataAddress = tcallbackInfoInternal.ClientDataAddress;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalSet<T>(ref T target, T source)
		{
			target = source;
			return true;
		}

		internal static bool TryMarshalSet(ref IntPtr target, Handle source)
		{
			return Helper.TryConvert(source, out target);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T? source) where T : struct
		{
			return Helper.TryAllocate<T>(ref target, source);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, bool isElementAllocated)
		{
			return Helper.TryAllocate<T>(ref target, source, isElementAllocated);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source)
		{
			return Helper.TryMarshalSet<T>(ref target, source, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, out int arrayLength, bool isElementAllocated)
		{
			arrayLength = 0;
			if (Helper.TryMarshalSet<T>(ref target, source, isElementAllocated))
			{
				arrayLength = source.Length;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, out uint arrayLength, bool isElementAllocated)
		{
			arrayLength = 0U;
			int num = 0;
			if (Helper.TryMarshalSet<T>(ref target, source, out num, isElementAllocated))
			{
				arrayLength = (uint)num;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, out int arrayLength)
		{
			return Helper.TryMarshalSet<T>(ref target, source, out arrayLength, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, out uint arrayLength)
		{
			return Helper.TryMarshalSet<T>(ref target, source, out arrayLength, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalSet(ref long target, DateTimeOffset? source)
		{
			return Helper.TryConvert(source, out target);
		}

		internal static bool TryMarshalSet(ref int target, bool source)
		{
			return Helper.TryConvert(source, out target);
		}

		internal static bool TryMarshalSet(ref byte[] target, string source)
		{
			return Helper.TryConvert(source, out target);
		}

		internal static bool TryMarshalSet(ref byte[] target, string source, int length)
		{
			return Helper.TryConvert(source, out target, length);
		}

		internal static bool TryMarshalSet(ref IntPtr target, string source)
		{
			return Helper.TryAllocate(ref target, source);
		}

		internal static bool TryMarshalSet<T, TEnum>(ref T target, T source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable)
		{
			if (source != null)
			{
				Helper.TryMarshalDispose(ref disposable);
				if (Helper.TryMarshalSet<T>(ref target, source))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return false;
		}

		internal static bool TryMarshalSet<T, TEnum>(ref T target, T? source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable) where T : struct
		{
			if (source != null)
			{
				Helper.TryMarshalDispose(ref disposable);
				if (Helper.TryMarshalSet<T>(ref target, source.Value))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return true;
		}

		internal static bool TryMarshalSet<T, TEnum>(ref IntPtr target, T source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable) where T : Handle
		{
			if (source != null)
			{
				Helper.TryMarshalDispose(ref disposable);
				if (Helper.TryMarshalSet(ref target, source))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return true;
		}

		internal static bool TryMarshalSet<TEnum>(ref IntPtr target, string source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable)
		{
			if (source != null)
			{
				Helper.TryMarshalDispose(ref disposable);
				if (Helper.TryMarshalSet(ref target, source))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return true;
		}

		internal static bool TryMarshalSet<TEnum>(ref int target, bool? source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable)
		{
			if (source != null)
			{
				Helper.TryMarshalDispose(ref disposable);
				if (Helper.TryMarshalSet(ref target, source.Value))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return true;
		}

		internal static bool TryMarshalDispose(ref object value)
		{
			IDisposable disposable = value as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
				return true;
			}
			return false;
		}

		internal static bool TryMarshalDispose<T>(ref T value) where T : IDisposable
		{
			value.Dispose();
			return true;
		}

		internal static bool TryMarshalDispose(ref IntPtr value)
		{
			return Helper.TryRelease(ref value);
		}

		internal static bool TryMarshalDispose<TEnum>(ref IntPtr member, TEnum currentEnum, TEnum comparisonEnum)
		{
			return (int)((object)currentEnum) == (int)((object)comparisonEnum) && Helper.TryRelease(ref member);
		}

		internal static T GetDefault<T>()
		{
			return default(T);
		}

		internal static T CopyProperties<T>(object value) where T : new()
		{
			object obj = new T();
			IInitializable initializable = obj as IInitializable;
			if (initializable != null)
			{
				initializable.Initialize();
			}
			Helper.CopyProperties(value, obj);
			return (T)((object)obj);
		}

		internal static void AddCallback(ref IntPtr clientDataAddress, object clientData, Delegate publicDelegate, Delegate privateDelegate, params Delegate[] additionalDelegates)
		{
			Helper.TryAllocate<BoxedData>(ref clientDataAddress, new BoxedData(clientData));
			Helper.s_Callbacks.Add(clientDataAddress, new Helper.DelegateHolder(publicDelegate, privateDelegate, additionalDelegates));
		}

		internal static bool TryAssignNotificationIdToCallback(IntPtr clientDataAddress, ulong notificationId)
		{
			if (notificationId != 0UL)
			{
				Helper.DelegateHolder delegateHolder = null;
				if (Helper.s_Callbacks.TryGetValue(clientDataAddress, out delegateHolder))
				{
					delegateHolder.NotificationId = new ulong?(notificationId);
					return true;
				}
			}
			else
			{
				Helper.s_Callbacks.Remove(clientDataAddress);
				Helper.TryRelease(ref clientDataAddress);
			}
			return false;
		}

		internal static bool TryRemoveCallbackByNotificationId(ulong notificationId)
		{
			IEnumerable<KeyValuePair<IntPtr, Helper.DelegateHolder>> enumerable = Helper.s_Callbacks.Where<KeyValuePair<IntPtr, Helper.DelegateHolder>>(delegate(KeyValuePair<IntPtr, Helper.DelegateHolder> pair)
			{
				if (pair.Value.NotificationId != null)
				{
					ulong? notificationId2 = pair.Value.NotificationId;
					ulong notificationId3 = notificationId;
					return (notificationId2.GetValueOrDefault() == notificationId3) & (notificationId2 != null);
				}
				return false;
			});
			if (enumerable.Any<KeyValuePair<IntPtr, Helper.DelegateHolder>>())
			{
				IntPtr key = enumerable.First<KeyValuePair<IntPtr, Helper.DelegateHolder>>().Key;
				Helper.s_Callbacks.Remove(key);
				Helper.TryRelease(ref key);
				return true;
			}
			return false;
		}

		internal static bool TryGetAndRemoveCallback<TCallback, TCallbackInfoInternal, TCallbackInfo>(IntPtr callbackInfoAddress, out TCallback callback, out TCallbackInfo callbackInfo) where TCallback : class where TCallbackInfoInternal : struct, ICallbackInfo where TCallbackInfo : class, new()
		{
			callback = default(TCallback);
			callbackInfo = default(TCallbackInfo);
			IntPtr zero = IntPtr.Zero;
			return Helper.TryMarshalGet<TCallbackInfoInternal, TCallbackInfo>(callbackInfoAddress, out callbackInfo, out zero) && Helper.TryGetAndRemoveCallback<TCallback>(zero, callbackInfo, out callback);
		}

		internal static bool TryGetAdditionalCallback<TDelegate, TCallbackInfoInternal, TCallbackInfo>(IntPtr callbackInfoAddress, out TDelegate callback, out TCallbackInfo callbackInfo) where TDelegate : class where TCallbackInfoInternal : struct, ICallbackInfo where TCallbackInfo : class, new()
		{
			callback = default(TDelegate);
			callbackInfo = default(TCallbackInfo);
			IntPtr zero = IntPtr.Zero;
			return Helper.TryMarshalGet<TCallbackInfoInternal, TCallbackInfo>(callbackInfoAddress, out callbackInfo, out zero) && Helper.TryGetAdditionalCallback<TDelegate>(zero, out callback);
		}

		private static bool TryAllocate<T>(ref IntPtr target, T source)
		{
			Helper.TryRelease(ref target);
			if (target != IntPtr.Zero)
			{
				throw new ExternalAllocationException(target, source.GetType());
			}
			if (source == null)
			{
				return false;
			}
			target = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
			Marshal.StructureToPtr<T>(source, target, false);
			Helper.s_Allocations.Add(target, new Helper.Allocation(source));
			return true;
		}

		private static bool TryAllocate<T>(ref IntPtr target, T? source) where T : struct
		{
			Helper.TryRelease(ref target);
			if (target != IntPtr.Zero)
			{
				throw new ExternalAllocationException(target, source.GetType());
			}
			return source != null && Helper.TryAllocate<T>(ref target, source.Value);
		}

		private static bool TryAllocate(ref IntPtr target, string source)
		{
			Helper.TryRelease(ref target);
			if (target != IntPtr.Zero)
			{
				throw new ExternalAllocationException(target, source.GetType());
			}
			byte[] array;
			return source != null && Helper.TryConvert(source, out array) && Helper.TryAllocate<byte>(ref target, array, false);
		}

		private static bool TryAllocate<T>(ref IntPtr target, T[] source, bool isElementAllocated)
		{
			Helper.TryRelease(ref target);
			if (target != IntPtr.Zero)
			{
				throw new ExternalAllocationException(target, source.GetType());
			}
			if (source == null)
			{
				return false;
			}
			int num;
			if (isElementAllocated)
			{
				num = Marshal.SizeOf(typeof(IntPtr));
			}
			else
			{
				num = Marshal.SizeOf(typeof(T));
			}
			target = Marshal.AllocHGlobal(source.Length * num);
			Helper.s_Allocations.Add(target, new Helper.ArrayAllocation(source, isElementAllocated));
			for (int i = 0; i < source.Length; i++)
			{
				T t = (T)((object)source.GetValue(i));
				if (isElementAllocated)
				{
					IntPtr zero = IntPtr.Zero;
					if (typeof(T) == typeof(string))
					{
						Helper.TryAllocate(ref zero, (string)((object)t));
					}
					else if (typeof(T).BaseType == typeof(Handle))
					{
						Helper.TryConvert((Handle)((object)t), out zero);
					}
					else
					{
						Helper.TryAllocate<T>(ref zero, t);
					}
					IntPtr intPtr = new IntPtr(target.ToInt64() + (long)(i * num));
					Marshal.StructureToPtr<IntPtr>(zero, intPtr, false);
				}
				else
				{
					IntPtr intPtr2 = new IntPtr(target.ToInt64() + (long)(i * num));
					Marshal.StructureToPtr<T>(t, intPtr2, false);
				}
			}
			return true;
		}

		private static bool TryRelease(ref IntPtr target)
		{
			if (target == IntPtr.Zero)
			{
				return false;
			}
			Helper.Allocation allocation = null;
			if (!Helper.s_Allocations.TryGetValue(target, out allocation))
			{
				return false;
			}
			if (allocation is Helper.ArrayAllocation)
			{
				Helper.ArrayAllocation arrayAllocation = allocation as Helper.ArrayAllocation;
				int num;
				if (arrayAllocation.IsElementAllocated)
				{
					num = Marshal.SizeOf(typeof(IntPtr));
				}
				else
				{
					num = Marshal.SizeOf(arrayAllocation.Data.GetType().GetElementType());
				}
				Array array = arrayAllocation.Data as Array;
				for (int i = 0; i < array.Length; i++)
				{
					if (arrayAllocation.IsElementAllocated)
					{
						IntPtr intPtr = new IntPtr(target.ToInt64() + (long)(i * num));
						intPtr = Marshal.ReadIntPtr(intPtr);
						Helper.TryRelease(ref intPtr);
					}
					else
					{
						object value = array.GetValue(i);
						if (value is IDisposable)
						{
							IDisposable disposable = value as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
			}
			if (allocation.Data is IDisposable)
			{
				IDisposable disposable2 = allocation.Data as IDisposable;
				if (disposable2 != null)
				{
					disposable2.Dispose();
				}
			}
			Marshal.FreeHGlobal(target);
			Helper.s_Allocations.Remove(target);
			target = IntPtr.Zero;
			return true;
		}

		private static bool TryFetch<T>(IntPtr source, out T target)
		{
			target = Helper.GetDefault<T>();
			if (source == IntPtr.Zero)
			{
				return false;
			}
			if (!Helper.s_Allocations.ContainsKey(source))
			{
				target = (T)((object)Marshal.PtrToStructure(source, typeof(T)));
				return true;
			}
			Helper.Allocation allocation = Helper.s_Allocations[source];
			if (allocation.Data.GetType() == typeof(T))
			{
				target = (T)((object)allocation.Data);
				return true;
			}
			throw new TypeAllocationException(source, allocation.Data.GetType(), typeof(T));
		}

		private static bool TryFetch<T>(IntPtr source, out T? target) where T : struct
		{
			target = Helper.GetDefault<T?>();
			if (source == IntPtr.Zero)
			{
				return false;
			}
			if (!Helper.s_Allocations.ContainsKey(source))
			{
				target = (T?)Marshal.PtrToStructure(source, typeof(T));
				return true;
			}
			Helper.Allocation allocation = Helper.s_Allocations[source];
			if (allocation.Data.GetType() == typeof(T))
			{
				target = (T?)allocation.Data;
				return true;
			}
			throw new TypeAllocationException(source, allocation.Data.GetType(), typeof(T));
		}

		private static bool TryFetch<T>(IntPtr source, out T[] target, int arrayLength, bool isElementAllocated)
		{
			target = null;
			if (source == IntPtr.Zero)
			{
				return false;
			}
			if (!Helper.s_Allocations.ContainsKey(source))
			{
				int num;
				if (isElementAllocated)
				{
					num = Marshal.SizeOf(typeof(IntPtr));
				}
				else
				{
					num = Marshal.SizeOf(typeof(T));
				}
				List<T> list = new List<T>();
				for (int i = 0; i < arrayLength; i++)
				{
					IntPtr intPtr = new IntPtr(source.ToInt64() + (long)(i * num));
					if (isElementAllocated)
					{
						intPtr = Marshal.ReadIntPtr(intPtr);
					}
					T t;
					Helper.TryFetch<T>(intPtr, out t);
					list.Add(t);
				}
				target = list.ToArray();
				return true;
			}
			Helper.Allocation allocation = Helper.s_Allocations[source];
			if (!(allocation.Data.GetType() == typeof(T[])))
			{
				throw new TypeAllocationException(source, allocation.Data.GetType(), typeof(T[]));
			}
			Array array = (Array)allocation.Data;
			if (array.Length == arrayLength)
			{
				target = array as T[];
				return true;
			}
			throw new ArrayAllocationException(source, array.Length, arrayLength);
		}

		private static bool TryFetch(IntPtr source, out string target)
		{
			target = null;
			if (source == IntPtr.Zero)
			{
				return false;
			}
			int num = 0;
			while (Marshal.ReadByte(source, num) != 0)
			{
				num++;
			}
			byte[] array = new byte[num];
			Marshal.Copy(source, array, 0, num);
			target = Encoding.UTF8.GetString(array);
			return true;
		}

		private static bool TryConvert<THandle>(IntPtr source, out THandle target) where THandle : Handle
		{
			target = default(THandle);
			if (source != IntPtr.Zero)
			{
				target = Activator.CreateInstance(typeof(THandle), new object[] { source }) as THandle;
			}
			return true;
		}

		private static bool TryConvert(Handle source, out IntPtr target)
		{
			target = IntPtr.Zero;
			if (source != null)
			{
				target = source.InnerHandle;
			}
			return true;
		}

		private static bool TryConvert(byte[] source, out string target)
		{
			target = null;
			if (source == null)
			{
				return false;
			}
			int num = 0;
			int num2 = 0;
			while (num2 < source.Length && source[num2] != 0)
			{
				num++;
				num2++;
			}
			target = Encoding.UTF8.GetString(source.Take<byte>(num).ToArray<byte>());
			return true;
		}

		private static bool TryConvert(string source, out byte[] target, int length)
		{
			if (source == null)
			{
				source = "";
			}
			target = Encoding.UTF8.GetBytes(new string(source.Take<char>(length).ToArray<char>()).PadRight(length, '\0'));
			return true;
		}

		private static bool TryConvert(string source, out byte[] target)
		{
			return Helper.TryConvert(source, out target, source.Length + 1);
		}

		private static bool TryConvert(int source, out bool target)
		{
			target = source != 0;
			return true;
		}

		private static bool TryConvert(bool source, out int target)
		{
			target = (source ? 1 : 0);
			return true;
		}

		private static bool TryConvert(DateTimeOffset? source, out long target)
		{
			target = -1L;
			if (source != null)
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				long num = (source.Value.UtcDateTime - dateTime).Ticks / 10000000L;
				target = num;
			}
			return true;
		}

		private static bool TryConvert(long source, out DateTimeOffset? target)
		{
			target = null;
			if (source >= 0L)
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				long num = source * 10000000L;
				target = new DateTimeOffset?(new DateTimeOffset(dateTime.Ticks + num, TimeSpan.Zero));
			}
			return true;
		}

		private static void CopyProperties(object source, object target)
		{
			if (source == null || target == null)
			{
				return;
			}
			PropertyInfo[] properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
			PropertyInfo[] properties2 = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo sourceProperty = array[i];
				PropertyInfo propertyInfo = properties2.SingleOrDefault<PropertyInfo>((PropertyInfo property) => property.Name == sourceProperty.Name);
				if (!(propertyInfo == null) && !(propertyInfo.GetSetMethod(false) == null))
				{
					if (sourceProperty.PropertyType == propertyInfo.PropertyType)
					{
						propertyInfo.SetValue(target, sourceProperty.GetValue(source, null), null);
					}
					else if (propertyInfo.PropertyType.IsArray)
					{
						Array array2 = sourceProperty.GetValue(source, null) as Array;
						if (array2 != null)
						{
							Array array3 = Array.CreateInstance(propertyInfo.PropertyType.GetElementType(), array2.Length);
							for (int j = 0; j < array2.Length; j++)
							{
								object value = array2.GetValue(j);
								object obj = Activator.CreateInstance(propertyInfo.PropertyType.GetElementType());
								Helper.CopyProperties(value, obj);
								array3.SetValue(obj, j);
							}
							propertyInfo.SetValue(target, array3, null);
						}
						else
						{
							propertyInfo.SetValue(target, null, null);
						}
					}
					else
					{
						object obj2 = null;
						Type type = propertyInfo.PropertyType;
						Type underlyingType = Nullable.GetUnderlyingType(type);
						if (underlyingType != null)
						{
							type = underlyingType;
						}
						else
						{
							obj2 = Activator.CreateInstance(type);
						}
						object value2 = sourceProperty.GetValue(source, null);
						if (value2 != null)
						{
							obj2 = Activator.CreateInstance(type);
							Helper.CopyProperties(value2, obj2);
						}
						propertyInfo.SetValue(target, obj2, null);
					}
				}
			}
		}

		private static bool CanRemoveCallback(IntPtr clientDataAddress, object callbackInfo)
		{
			Helper.DelegateHolder delegateHolder = null;
			if (Helper.s_Callbacks.TryGetValue(clientDataAddress, out delegateHolder) && delegateHolder.NotificationId != null)
			{
				return false;
			}
			PropertyInfo propertyInfo = (from property in callbackInfo.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
				where property.PropertyType == typeof(Result)
				select property).FirstOrDefault<PropertyInfo>();
			return !(propertyInfo != null) || Helper.IsOperationComplete((Result)propertyInfo.GetValue(callbackInfo, null));
		}

		private static bool TryGetAndRemoveCallback<TCallback>(IntPtr clientDataAddress, object callbackInfo, out TCallback callback) where TCallback : class
		{
			callback = default(TCallback);
			if (clientDataAddress != IntPtr.Zero && Helper.s_Callbacks.ContainsKey(clientDataAddress))
			{
				callback = Helper.s_Callbacks[clientDataAddress].Public as TCallback;
				if (Helper.CanRemoveCallback(clientDataAddress, callbackInfo))
				{
					Helper.s_Callbacks.Remove(clientDataAddress);
					Helper.TryRelease(ref clientDataAddress);
				}
				return true;
			}
			return false;
		}

		private static bool TryGetAdditionalCallback<TCallback>(IntPtr clientDataAddress, out TCallback additionalCallback) where TCallback : class
		{
			additionalCallback = default(TCallback);
			if (clientDataAddress != IntPtr.Zero && Helper.s_Callbacks.ContainsKey(clientDataAddress))
			{
				additionalCallback = Helper.s_Callbacks[clientDataAddress].Additional.FirstOrDefault<Delegate>((Delegate delegat) => delegat.GetType() == typeof(TCallback)) as TCallback;
				if (additionalCallback != null)
				{
					return true;
				}
			}
			return false;
		}

		private static Dictionary<IntPtr, Helper.Allocation> s_Allocations = new Dictionary<IntPtr, Helper.Allocation>();

		private static Dictionary<IntPtr, Helper.DelegateHolder> s_Callbacks = new Dictionary<IntPtr, Helper.DelegateHolder>();

		private class Allocation
		{
			public object Data { get; private set; }

			public Allocation(object data)
			{
				this.Data = data;
			}
		}

		private class ArrayAllocation : Helper.Allocation
		{
			public bool IsElementAllocated { get; private set; }

			public ArrayAllocation(object data, bool isElementAllocated)
				: base(data)
			{
				this.IsElementAllocated = isElementAllocated;
			}
		}

		private class DelegateHolder
		{
			public Delegate Public { get; private set; }

			public Delegate Private { get; private set; }

			public Delegate[] Additional { get; private set; }

			public ulong? NotificationId { get; set; }

			public DelegateHolder(Delegate publicDelegate, Delegate privateDelegate, params Delegate[] additionalDelegates)
			{
				this.Public = publicDelegate;
				this.Private = privateDelegate;
				this.Additional = additionalDelegates;
			}
		}
	}
}
