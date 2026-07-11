using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Interop;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace System.Runtime.Diagnostics
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	internal abstract class DiagnosticsEventProvider : IDisposable
	{
		[SecurityCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		protected DiagnosticsEventProvider(Guid providerGuid)
		{
			this.providerId = providerGuid;
			int platform = (int)Environment.OSVersion.Platform;
			if (platform == 4 || platform == 128)
			{
				return;
			}
			this.EtwRegister();
		}

		[SecurityCritical]
		private void EtwRegister()
		{
			this.etwCallback = new UnsafeNativeMethods.EtwEnableCallback(this.EtwEnableCallBack);
			uint num = UnsafeNativeMethods.EventRegister(ref this.providerId, this.etwCallback, null, ref this.traceRegistrationHandle);
			if (num != 0U)
			{
				throw new InvalidOperationException(InternalSR.EtwRegistrationFailed(num.ToString("x", CultureInfo.CurrentCulture)));
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[SecuritySafeCritical]
		protected virtual void Dispose(bool disposing)
		{
			if (this.isDisposed != 1 && Interlocked.Exchange(ref this.isDisposed, 1) == 0)
			{
				this.isProviderEnabled = false;
				this.Deregister();
			}
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		~DiagnosticsEventProvider()
		{
			this.Dispose(false);
		}

		[SecurityCritical]
		private void Deregister()
		{
			if (this.traceRegistrationHandle != 0L)
			{
				UnsafeNativeMethods.EventUnregister(this.traceRegistrationHandle);
				this.traceRegistrationHandle = 0L;
			}
		}

		[SecurityCritical]
		private unsafe void EtwEnableCallBack([In] ref Guid sourceId, [In] int isEnabled, [In] byte setLevel, [In] long anyKeyword, [In] long allKeyword, [In] void* filterData, [In] void* callbackContext)
		{
			this.isProviderEnabled = isEnabled != 0;
			this.currentTraceLevel = setLevel;
			this.anyKeywordMask = anyKeyword;
			this.allKeywordMask = allKeyword;
			this.OnControllerCommand();
		}

		protected abstract void OnControllerCommand();

		public bool IsEnabled()
		{
			return this.isProviderEnabled;
		}

		public bool IsEnabled(byte level, long keywords)
		{
			return this.isProviderEnabled && (level <= this.currentTraceLevel || this.currentTraceLevel == 0) && (keywords == 0L || ((keywords & this.anyKeywordMask) != 0L && (keywords & this.allKeywordMask) == this.allKeywordMask));
		}

		[SecurityCritical]
		public bool IsEventEnabled(ref EventDescriptor eventDescriptor)
		{
			return this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords) && UnsafeNativeMethods.EventEnabled(this.traceRegistrationHandle, ref eventDescriptor);
		}

		public static DiagnosticsEventProvider.WriteEventErrorCode GetLastWriteEventError()
		{
			return DiagnosticsEventProvider.errorCode;
		}

		private static void SetLastError(int error)
		{
			if (error != 8)
			{
				if (error == 234 || error == 534)
				{
					DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
					return;
				}
			}
			else
			{
				DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.NoFreeBuffers;
			}
		}

		[SecurityCritical]
		private unsafe static string EncodeObject(ref object data, UnsafeNativeMethods.EventData* dataDescriptor, byte* dataBuffer)
		{
			dataDescriptor->Reserved = 0;
			string text = data as string;
			if (text != null)
			{
				dataDescriptor->Size = (uint)((text.Length + 1) * 2);
				return text;
			}
			if (data is IntPtr)
			{
				dataDescriptor->Size = (uint)sizeof(IntPtr);
				*(IntPtr*)dataBuffer = (IntPtr)data;
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is int)
			{
				dataDescriptor->Size = 4U;
				*(int*)dataBuffer = (int)data;
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is long)
			{
				dataDescriptor->Size = 8U;
				*(long*)dataBuffer = (long)data;
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is uint)
			{
				dataDescriptor->Size = 4U;
				*(int*)dataBuffer = (int)((uint)data);
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is ulong)
			{
				dataDescriptor->Size = 8U;
				*(long*)dataBuffer = (long)((ulong)data);
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is char)
			{
				dataDescriptor->Size = 2U;
				*(short*)dataBuffer = (short)((char)data);
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is byte)
			{
				dataDescriptor->Size = 1U;
				*dataBuffer = (byte)data;
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is short)
			{
				dataDescriptor->Size = 2U;
				*(short*)dataBuffer = (short)data;
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is sbyte)
			{
				dataDescriptor->Size = 1U;
				*dataBuffer = (byte)((sbyte)data);
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is ushort)
			{
				dataDescriptor->Size = 2U;
				*(short*)dataBuffer = (short)((ushort)data);
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is float)
			{
				dataDescriptor->Size = 4U;
				*(float*)dataBuffer = (float)data;
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is double)
			{
				dataDescriptor->Size = 8U;
				*(double*)dataBuffer = (double)data;
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is bool)
			{
				dataDescriptor->Size = 1U;
				*dataBuffer = (((bool)data) ? 1 : 0);
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is Guid)
			{
				dataDescriptor->Size = (uint)sizeof(Guid);
				*(Guid*)dataBuffer = (Guid)data;
				dataDescriptor->DataPointer = dataBuffer;
			}
			else if (data is decimal)
			{
				dataDescriptor->Size = 16U;
				*(decimal*)dataBuffer = (decimal)data;
				dataDescriptor->DataPointer = dataBuffer;
			}
			else
			{
				if (!(data is bool))
				{
					text = data.ToString();
					dataDescriptor->Size = (uint)((text.Length + 1) * 2);
					return text;
				}
				dataDescriptor->Size = 1U;
				*dataBuffer = (((bool)data) ? 1 : 0);
				dataDescriptor->DataPointer = dataBuffer;
			}
			return null;
		}

		[SecurityCritical]
		public unsafe bool WriteMessageEvent(EventTraceActivity eventTraceActivity, string eventMessage, byte eventLevel, long eventKeywords)
		{
			if (eventMessage == null)
			{
				throw Fx.Exception.AsError(new ArgumentNullException("eventMessage"));
			}
			if (eventTraceActivity != null)
			{
				DiagnosticsEventProvider.SetActivityId(ref eventTraceActivity.ActivityId);
			}
			if (this.IsEnabled(eventLevel, eventKeywords))
			{
				if (eventMessage.Length > 32724)
				{
					DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
					return false;
				}
				int num;
				fixed (string text = eventMessage)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					num = (int)UnsafeNativeMethods.EventWriteString(this.traceRegistrationHandle, eventLevel, eventKeywords, ptr);
				}
				if (num != 0)
				{
					DiagnosticsEventProvider.SetLastError(num);
					return false;
				}
			}
			return true;
		}

		[SecurityCritical]
		public bool WriteMessageEvent(EventTraceActivity eventTraceActivity, string eventMessage)
		{
			return this.WriteMessageEvent(eventTraceActivity, eventMessage, 0, 0L);
		}

		[SecurityCritical]
		public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, params object[] eventPayload)
		{
			uint num = 0U;
			if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				int num2 = 0;
				if (eventTraceActivity != null)
				{
					DiagnosticsEventProvider.SetActivityId(ref eventTraceActivity.ActivityId);
				}
				if (eventPayload == null || eventPayload.Length == 0 || eventPayload.Length == 1)
				{
					string text = null;
					byte* ptr = stackalloc byte[(UIntPtr)16];
					UnsafeNativeMethods.EventData eventData;
					eventData.Size = 0U;
					if (eventPayload != null && eventPayload.Length != 0)
					{
						text = DiagnosticsEventProvider.EncodeObject(ref eventPayload[0], &eventData, ptr);
						num2 = 1;
					}
					if (eventData.Size > 65482U)
					{
						DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
						return false;
					}
					if (text != null)
					{
						fixed (string text2 = text)
						{
							char* ptr2 = text2;
							if (ptr2 != null)
							{
								ptr2 += RuntimeHelpers.OffsetToStringData / 2;
							}
							eventData.DataPointer = ptr2;
							num = UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, (uint)num2, &eventData);
						}
					}
					else if (num2 == 0)
					{
						num = UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, 0U, null);
					}
					else
					{
						num = UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, (uint)num2, &eventData);
					}
				}
				else
				{
					num2 = eventPayload.Length;
					if (num2 > 32)
					{
						throw Fx.Exception.AsError(new ArgumentOutOfRangeException("eventPayload", InternalSR.EtwMaxNumberArgumentsExceeded(32)));
					}
					uint num3 = 0U;
					int num4 = 0;
					int[] array = new int[8];
					string[] array2 = new string[8];
					UnsafeNativeMethods.EventData* ptr3;
					UnsafeNativeMethods.EventData* ptr4;
					checked
					{
						ptr3 = stackalloc UnsafeNativeMethods.EventData[unchecked((UIntPtr)num2) * (UIntPtr)sizeof(UnsafeNativeMethods.EventData)];
						ptr4 = ptr3;
					}
					byte* ptr5 = stackalloc byte[(UIntPtr)(16 * num2)];
					for (int i = 0; i < eventPayload.Length; i++)
					{
						if (eventPayload[i] != null)
						{
							string text3 = DiagnosticsEventProvider.EncodeObject(ref eventPayload[i], ptr4, ptr5);
							ptr5 += 16;
							num3 += ptr4->Size;
							ptr4++;
							if (text3 != null)
							{
								if (num4 >= 8)
								{
									throw Fx.Exception.AsError(new ArgumentOutOfRangeException("eventPayload", InternalSR.EtwAPIMaxStringCountExceeded(8)));
								}
								array2[num4] = text3;
								array[num4] = i;
								num4++;
							}
						}
					}
					if (num3 > 65482U)
					{
						DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
						return false;
					}
					fixed (string text2 = array2[0])
					{
						char* ptr6 = text2;
						if (ptr6 != null)
						{
							ptr6 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = array2[1])
						{
							char* ptr7 = text4;
							if (ptr7 != null)
							{
								ptr7 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = array2[2])
							{
								char* ptr8 = text5;
								if (ptr8 != null)
								{
									ptr8 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = array2[3])
								{
									char* ptr9 = text6;
									if (ptr9 != null)
									{
										ptr9 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = array2[4])
									{
										char* ptr10 = text7;
										if (ptr10 != null)
										{
											ptr10 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = array2[5])
										{
											char* ptr11 = text8;
											if (ptr11 != null)
											{
												ptr11 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text9 = array2[6])
											{
												char* ptr12 = text9;
												if (ptr12 != null)
												{
													ptr12 += RuntimeHelpers.OffsetToStringData / 2;
												}
												fixed (string text10 = array2[7])
												{
													char* ptr13 = text10;
													if (ptr13 != null)
													{
														ptr13 += RuntimeHelpers.OffsetToStringData / 2;
													}
													ptr4 = ptr3;
													if (array2[0] != null)
													{
														ptr4[array[0]].DataPointer = ptr6;
													}
													if (array2[1] != null)
													{
														ptr4[array[1]].DataPointer = ptr7;
													}
													if (array2[2] != null)
													{
														ptr4[array[2]].DataPointer = ptr8;
													}
													if (array2[3] != null)
													{
														ptr4[array[3]].DataPointer = ptr9;
													}
													if (array2[4] != null)
													{
														ptr4[array[4]].DataPointer = ptr10;
													}
													if (array2[5] != null)
													{
														ptr4[array[5]].DataPointer = ptr11;
													}
													if (array2[6] != null)
													{
														ptr4[array[6]].DataPointer = ptr12;
													}
													if (array2[7] != null)
													{
														ptr4[array[7]].DataPointer = ptr13;
													}
													num = UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, (uint)num2, ptr3);
													text2 = null;
													text4 = null;
													text5 = null;
													text6 = null;
													text7 = null;
													text8 = null;
													text9 = null;
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
			if (num != 0U)
			{
				DiagnosticsEventProvider.SetLastError((int)num);
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string data)
		{
			uint num = 0U;
			data = data ?? string.Empty;
			if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				if (data.Length > 32724)
				{
					DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
					return false;
				}
				if (eventTraceActivity != null)
				{
					DiagnosticsEventProvider.SetActivityId(ref eventTraceActivity.ActivityId);
				}
				UnsafeNativeMethods.EventData eventData;
				eventData.Size = (uint)((data.Length + 1) * 2);
				eventData.Reserved = 0;
				fixed (string text = data)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					eventData.DataPointer = ptr;
					num = UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, 1U, &eventData);
				}
			}
			if (num != 0U)
			{
				DiagnosticsEventProvider.SetLastError((int)num);
				return false;
			}
			return true;
		}

		[SecurityCritical]
		protected internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, int dataCount, IntPtr data)
		{
			if (eventTraceActivity != null)
			{
				DiagnosticsEventProvider.SetActivityId(ref eventTraceActivity.ActivityId);
			}
			uint num = UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, (uint)dataCount, (UnsafeNativeMethods.EventData*)(void*)data);
			if (num != 0U)
			{
				DiagnosticsEventProvider.SetLastError((int)num);
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public unsafe bool WriteTransferEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid relatedActivityId, params object[] eventPayload)
		{
			if (eventTraceActivity == null)
			{
				eventTraceActivity = EventTraceActivity.Empty;
			}
			uint num = 0U;
			if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				if (eventPayload != null && eventPayload.Length != 0)
				{
					int num2 = eventPayload.Length;
					if (num2 > 32)
					{
						throw Fx.Exception.AsError(new ArgumentOutOfRangeException("eventPayload", InternalSR.EtwMaxNumberArgumentsExceeded(32)));
					}
					uint num3 = 0U;
					int num4 = 0;
					int[] array = new int[8];
					string[] array2 = new string[8];
					UnsafeNativeMethods.EventData* ptr;
					UnsafeNativeMethods.EventData* ptr2;
					checked
					{
						ptr = stackalloc UnsafeNativeMethods.EventData[unchecked((UIntPtr)num2) * (UIntPtr)sizeof(UnsafeNativeMethods.EventData)];
						ptr2 = ptr;
					}
					byte* ptr3 = stackalloc byte[(UIntPtr)(16 * num2)];
					for (int i = 0; i < eventPayload.Length; i++)
					{
						if (eventPayload[i] != null)
						{
							string text = DiagnosticsEventProvider.EncodeObject(ref eventPayload[i], ptr2, ptr3);
							ptr3 += 16;
							num3 += ptr2->Size;
							ptr2++;
							if (text != null)
							{
								if (num4 >= 8)
								{
									throw Fx.Exception.AsError(new ArgumentOutOfRangeException("eventPayload", InternalSR.EtwAPIMaxStringCountExceeded(8)));
								}
								array2[num4] = text;
								array[num4] = i;
								num4++;
							}
						}
					}
					if (num3 > 65482U)
					{
						DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
						return false;
					}
					fixed (string text2 = array2[0])
					{
						char* ptr4 = text2;
						if (ptr4 != null)
						{
							ptr4 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text3 = array2[1])
						{
							char* ptr5 = text3;
							if (ptr5 != null)
							{
								ptr5 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text4 = array2[2])
							{
								char* ptr6 = text4;
								if (ptr6 != null)
								{
									ptr6 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text5 = array2[3])
								{
									char* ptr7 = text5;
									if (ptr7 != null)
									{
										ptr7 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text6 = array2[4])
									{
										char* ptr8 = text6;
										if (ptr8 != null)
										{
											ptr8 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text7 = array2[5])
										{
											char* ptr9 = text7;
											if (ptr9 != null)
											{
												ptr9 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text8 = array2[6])
											{
												char* ptr10 = text8;
												if (ptr10 != null)
												{
													ptr10 += RuntimeHelpers.OffsetToStringData / 2;
												}
												fixed (string text9 = array2[7])
												{
													char* ptr11 = text9;
													if (ptr11 != null)
													{
														ptr11 += RuntimeHelpers.OffsetToStringData / 2;
													}
													ptr2 = ptr;
													if (array2[0] != null)
													{
														ptr2[array[0]].DataPointer = ptr4;
													}
													if (array2[1] != null)
													{
														ptr2[array[1]].DataPointer = ptr5;
													}
													if (array2[2] != null)
													{
														ptr2[array[2]].DataPointer = ptr6;
													}
													if (array2[3] != null)
													{
														ptr2[array[3]].DataPointer = ptr7;
													}
													if (array2[4] != null)
													{
														ptr2[array[4]].DataPointer = ptr8;
													}
													if (array2[5] != null)
													{
														ptr2[array[5]].DataPointer = ptr9;
													}
													if (array2[6] != null)
													{
														ptr2[array[6]].DataPointer = ptr10;
													}
													if (array2[7] != null)
													{
														ptr2[array[7]].DataPointer = ptr11;
													}
													num = UnsafeNativeMethods.EventWriteTransfer(this.traceRegistrationHandle, ref eventDescriptor, ref eventTraceActivity.ActivityId, ref relatedActivityId, (uint)num2, ptr);
													text2 = null;
													text3 = null;
													text4 = null;
													text5 = null;
													text6 = null;
													text7 = null;
													text8 = null;
												}
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					num = UnsafeNativeMethods.EventWriteTransfer(this.traceRegistrationHandle, ref eventDescriptor, ref eventTraceActivity.ActivityId, ref relatedActivityId, 0U, null);
				}
			}
			if (num != 0U)
			{
				DiagnosticsEventProvider.SetLastError((int)num);
				return false;
			}
			return true;
		}

		[SecurityCritical]
		protected unsafe bool WriteTransferEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid relatedActivityId, int dataCount, IntPtr data)
		{
			if (eventTraceActivity == null)
			{
				throw Fx.Exception.ArgumentNull("eventTraceActivity");
			}
			uint num = UnsafeNativeMethods.EventWriteTransfer(this.traceRegistrationHandle, ref eventDescriptor, ref eventTraceActivity.ActivityId, ref relatedActivityId, (uint)dataCount, (UnsafeNativeMethods.EventData*)(void*)data);
			if (num != 0U)
			{
				DiagnosticsEventProvider.SetLastError((int)num);
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static void SetActivityId(ref Guid id)
		{
			UnsafeNativeMethods.EventActivityIdControl(2, ref id);
		}

		[SecurityCritical]
		private UnsafeNativeMethods.EtwEnableCallback etwCallback;

		private long traceRegistrationHandle;

		private byte currentTraceLevel;

		private long anyKeywordMask;

		private long allKeywordMask;

		private bool isProviderEnabled;

		private Guid providerId;

		private int isDisposed;

		[ThreadStatic]
		private static DiagnosticsEventProvider.WriteEventErrorCode errorCode;

		private const int basicTypeAllocationBufferSize = 16;

		private const int etwMaxNumberArguments = 32;

		private const int etwAPIMaxStringCount = 8;

		private const int maxEventDataDescriptors = 128;

		private const int traceEventMaximumSize = 65482;

		private const int traceEventMaximumStringSize = 32724;

		private const int WindowsVistaMajorNumber = 6;

		public enum WriteEventErrorCode
		{
			NoError,
			NoFreeBuffers,
			EventTooBig
		}
	}
}
