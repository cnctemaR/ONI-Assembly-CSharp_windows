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
			if (this.m_Ptr != IntPtr.Zero)
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr Internal_Create(CustomEventData ced, string name);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Destroy(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool AddString(string key, string value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool AddInt32(string key, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool AddUInt32(string key, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool AddInt64(string key, long value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool AddUInt64(string key, ulong value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool AddBool(string key, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool AddDouble(string key, double value);

		public bool AddDictionary(IDictionary<string, object> eventData)
		{
			foreach (KeyValuePair<string, object> keyValuePair in eventData)
			{
				string key = keyValuePair.Key;
				object value = keyValuePair.Value;
				if (value == null)
				{
					this.AddString(key, "null");
				}
				else
				{
					Type type = value.GetType();
					if (type == typeof(string))
					{
						this.AddString(key, (string)value);
					}
					else if (type == typeof(char))
					{
						this.AddString(key, char.ToString((char)value));
					}
					else if (type == typeof(sbyte))
					{
						this.AddInt32(key, (int)((sbyte)value));
					}
					else if (type == typeof(byte))
					{
						this.AddInt32(key, (int)((byte)value));
					}
					else if (type == typeof(short))
					{
						this.AddInt32(key, (int)((short)value));
					}
					else if (type == typeof(ushort))
					{
						this.AddUInt32(key, (uint)((ushort)value));
					}
					else if (type == typeof(int))
					{
						this.AddInt32(key, (int)value);
					}
					else if (type == typeof(uint))
					{
						this.AddUInt32(keyValuePair.Key, (uint)value);
					}
					else if (type == typeof(long))
					{
						this.AddInt64(key, (long)value);
					}
					else if (type == typeof(ulong))
					{
						this.AddUInt64(key, (ulong)value);
					}
					else if (type == typeof(bool))
					{
						this.AddBool(key, (bool)value);
					}
					else if (type == typeof(float))
					{
						this.AddDouble(key, (double)Convert.ToDecimal((float)value));
					}
					else if (type == typeof(double))
					{
						this.AddDouble(key, (double)value);
					}
					else if (type == typeof(decimal))
					{
						this.AddDouble(key, (double)Convert.ToDecimal((decimal)value));
					}
					else
					{
						if (!type.IsValueType)
						{
							throw new ArgumentException(string.Format("Invalid type: {0} passed", type));
						}
						this.AddString(key, value.ToString());
					}
				}
			}
			return true;
		}

		[NonSerialized]
		internal IntPtr m_Ptr;
	}
}
