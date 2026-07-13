using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Analytics
{
	[NativeHeader("Modules/UnityAnalytics/Public/Events/UserCustomEvent.h")]
	[StructLayout(LayoutKind.Sequential)]
	internal class CustomEventData : IDisposable
	{
		private CustomEventData()
		{
		}

		public CustomEventData(string name)
		{
			this.m_Ptr = CustomEventData.Internal_Create(this, name);
		}

		~CustomEventData()
		{
			this.Destroy();
		}

		private void Destroy()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				CustomEventData.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			this.Destroy();
			GC.SuppressFinalize(this);
		}

		internal unsafe static IntPtr Internal_Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] CustomEventData ced, string name)
		{
			IntPtr intPtr;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				intPtr = CustomEventData.Internal_Create_Injected(ced, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return intPtr;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Destroy(IntPtr ptr);

		public unsafe bool AddString(string key, string value)
		{
			bool flag;
			try
			{
				IntPtr intPtr = CustomEventData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = value.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				flag = CustomEventData.AddString_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return flag;
		}

		public unsafe bool AddInt32(string key, int value)
		{
			bool flag;
			try
			{
				IntPtr intPtr = CustomEventData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = CustomEventData.AddInt32_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public unsafe bool AddUInt32(string key, uint value)
		{
			bool flag;
			try
			{
				IntPtr intPtr = CustomEventData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = CustomEventData.AddUInt32_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public unsafe bool AddInt64(string key, long value)
		{
			bool flag;
			try
			{
				IntPtr intPtr = CustomEventData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = CustomEventData.AddInt64_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public unsafe bool AddUInt64(string key, ulong value)
		{
			bool flag;
			try
			{
				IntPtr intPtr = CustomEventData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = CustomEventData.AddUInt64_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public unsafe bool AddBool(string key, bool value)
		{
			bool flag;
			try
			{
				IntPtr intPtr = CustomEventData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = CustomEventData.AddBool_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public unsafe bool AddDouble(string key, double value)
		{
			bool flag;
			try
			{
				IntPtr intPtr = CustomEventData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = CustomEventData.AddDouble_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public bool AddDictionary(IDictionary<string, object> eventData)
		{
			foreach (KeyValuePair<string, object> keyValuePair in eventData)
			{
				string key = keyValuePair.Key;
				object value = keyValuePair.Value;
				bool flag = value == null;
				if (flag)
				{
					this.AddString(key, "null");
				}
				else
				{
					Type type = value.GetType();
					bool flag2 = type == typeof(string);
					if (flag2)
					{
						this.AddString(key, (string)value);
					}
					else
					{
						bool flag3 = type == typeof(char);
						if (flag3)
						{
							this.AddString(key, char.ToString((char)value));
						}
						else
						{
							bool flag4 = type == typeof(sbyte);
							if (flag4)
							{
								this.AddInt32(key, (int)((sbyte)value));
							}
							else
							{
								bool flag5 = type == typeof(byte);
								if (flag5)
								{
									this.AddInt32(key, (int)((byte)value));
								}
								else
								{
									bool flag6 = type == typeof(short);
									if (flag6)
									{
										this.AddInt32(key, (int)((short)value));
									}
									else
									{
										bool flag7 = type == typeof(ushort);
										if (flag7)
										{
											this.AddUInt32(key, (uint)((ushort)value));
										}
										else
										{
											bool flag8 = type == typeof(int);
											if (flag8)
											{
												this.AddInt32(key, (int)value);
											}
											else
											{
												bool flag9 = type == typeof(uint);
												if (flag9)
												{
													this.AddUInt32(keyValuePair.Key, (uint)value);
												}
												else
												{
													bool flag10 = type == typeof(long);
													if (flag10)
													{
														this.AddInt64(key, (long)value);
													}
													else
													{
														bool flag11 = type == typeof(ulong);
														if (flag11)
														{
															this.AddUInt64(key, (ulong)value);
														}
														else
														{
															bool flag12 = type == typeof(bool);
															if (flag12)
															{
																this.AddBool(key, (bool)value);
															}
															else
															{
																bool flag13 = type == typeof(float);
																if (flag13)
																{
																	this.AddDouble(key, (double)Convert.ToDecimal((float)value));
																}
																else
																{
																	bool flag14 = type == typeof(double);
																	if (flag14)
																	{
																		this.AddDouble(key, (double)value);
																	}
																	else
																	{
																		bool flag15 = type == typeof(decimal);
																		if (flag15)
																		{
																			this.AddDouble(key, (double)Convert.ToDecimal((decimal)value));
																		}
																		else
																		{
																			bool isValueType = type.IsValueType;
																			if (!isValueType)
																			{
																				throw new ArgumentException(string.Format("Invalid type: {0} passed", type));
																			}
																			this.AddString(key, value.ToString());
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create_Injected(CustomEventData ced, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddInt32_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddUInt32_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddInt64_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, long value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddUInt64_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, ulong value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddBool_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddDouble_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, double value);

		[NonSerialized]
		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(CustomEventData customEventData)
			{
				return customEventData.m_Ptr;
			}
		}
	}
}
