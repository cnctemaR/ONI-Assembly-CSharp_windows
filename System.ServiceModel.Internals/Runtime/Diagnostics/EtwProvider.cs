using System;
using System.Runtime.CompilerServices;
using System.Runtime.Interop;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Diagnostics
{
	internal sealed class EtwProvider : DiagnosticsEventProvider
	{
		[SecurityCritical]
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		internal EtwProvider(Guid id)
			: base(id)
		{
		}

		internal Action ControllerCallBack
		{
			get
			{
				return this.invokeControllerCallback;
			}
			set
			{
				this.invokeControllerCallback = value;
			}
		}

		internal bool IsEnd2EndActivityTracingEnabled
		{
			get
			{
				return this.end2EndActivityTracingEnabled;
			}
		}

		protected override void OnControllerCommand()
		{
			this.end2EndActivityTracingEnabled = false;
			if (this.invokeControllerCallback != null)
			{
				this.invokeControllerCallback();
			}
		}

		internal void SetEnd2EndActivityTracingEnabled(bool isEnd2EndActivityTracingEnabled)
		{
			this.end2EndActivityTracingEnabled = isEnd2EndActivityTracingEnabled;
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, string value2, string value3)
		{
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			fixed (string text = value2)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				bool flag;
				fixed (string text2 = value3)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					byte* ptr3 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 3)];
					UnsafeNativeMethods.EventData* ptr4 = (UnsafeNativeMethods.EventData*)ptr3;
					ptr4->DataPointer = &value1;
					ptr4->Size = (uint)sizeof(Guid);
					ptr4[1].DataPointer = ptr;
					ptr4[1].Size = (uint)((value2.Length + 1) * 2);
					ptr4[2].DataPointer = ptr2;
					ptr4[2].Size = (uint)((value3.Length + 1) * 2);
					flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 3, (IntPtr)((void*)ptr3));
					text = null;
				}
				return flag;
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteTransferEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid relatedActivityId, string value1, string value2)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				bool flag;
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					byte* ptr3 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 2)];
					UnsafeNativeMethods.EventData* ptr4 = (UnsafeNativeMethods.EventData*)ptr3;
					ptr4->DataPointer = ptr;
					ptr4->Size = (uint)((value1.Length + 1) * 2);
					ptr4[1].DataPointer = ptr2;
					ptr4[1].Size = (uint)((value2.Length + 1) * 2);
					flag = base.WriteTransferEvent(ref eventDescriptor, eventTraceActivity, relatedActivityId, 2, (IntPtr)((void*)ptr3));
					text = null;
				}
				return flag;
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				bool flag;
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					byte* ptr3 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 2)];
					UnsafeNativeMethods.EventData* ptr4 = (UnsafeNativeMethods.EventData*)ptr3;
					ptr4->DataPointer = ptr;
					ptr4->Size = (uint)((value1.Length + 1) * 2);
					ptr4[1].DataPointer = ptr2;
					ptr4[1].Size = (uint)((value2.Length + 1) * 2);
					flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 2, (IntPtr)((void*)ptr3));
					text = null;
				}
				return flag;
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					bool flag;
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						byte* ptr4 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 3)];
						UnsafeNativeMethods.EventData* ptr5 = (UnsafeNativeMethods.EventData*)ptr4;
						ptr5->DataPointer = ptr;
						ptr5->Size = (uint)((value1.Length + 1) * 2);
						ptr5[1].DataPointer = ptr2;
						ptr5[1].Size = (uint)((value2.Length + 1) * 2);
						ptr5[2].DataPointer = ptr3;
						ptr5[2].Size = (uint)((value3.Length + 1) * 2);
						flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 3, (IntPtr)((void*)ptr4));
						text = null;
						text2 = null;
					}
					return flag;
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						bool flag;
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							byte* ptr5 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 4)];
							UnsafeNativeMethods.EventData* ptr6 = (UnsafeNativeMethods.EventData*)ptr5;
							ptr6->DataPointer = ptr;
							ptr6->Size = (uint)((value1.Length + 1) * 2);
							ptr6[1].DataPointer = ptr2;
							ptr6[1].Size = (uint)((value2.Length + 1) * 2);
							ptr6[2].DataPointer = ptr3;
							ptr6[2].Size = (uint)((value3.Length + 1) * 2);
							ptr6[3].DataPointer = ptr4;
							ptr6[3].Size = (uint)((value4.Length + 1) * 2);
							flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 4, (IntPtr)((void*)ptr5));
							text = null;
							text2 = null;
							text3 = null;
						}
						return flag;
					}
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4, string value5)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							bool flag;
							fixed (string text5 = value5)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								byte* ptr6 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 5)];
								UnsafeNativeMethods.EventData* ptr7 = (UnsafeNativeMethods.EventData*)ptr6;
								ptr7->DataPointer = ptr;
								ptr7->Size = (uint)((value1.Length + 1) * 2);
								ptr7[1].DataPointer = ptr2;
								ptr7[1].Size = (uint)((value2.Length + 1) * 2);
								ptr7[2].DataPointer = ptr3;
								ptr7[2].Size = (uint)((value3.Length + 1) * 2);
								ptr7[3].DataPointer = ptr4;
								ptr7[3].Size = (uint)((value4.Length + 1) * 2);
								ptr7[4].DataPointer = ptr5;
								ptr7[4].Size = (uint)((value5.Length + 1) * 2);
								flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 5, (IntPtr)((void*)ptr6));
								text = null;
								text2 = null;
								text3 = null;
								text4 = null;
							}
							return flag;
						}
					}
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4, string value5, string value6)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value5)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								bool flag;
								fixed (string text6 = value6)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									byte* ptr7 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 6)];
									UnsafeNativeMethods.EventData* ptr8 = (UnsafeNativeMethods.EventData*)ptr7;
									ptr8->DataPointer = ptr;
									ptr8->Size = (uint)((value1.Length + 1) * 2);
									ptr8[1].DataPointer = ptr2;
									ptr8[1].Size = (uint)((value2.Length + 1) * 2);
									ptr8[2].DataPointer = ptr3;
									ptr8[2].Size = (uint)((value3.Length + 1) * 2);
									ptr8[3].DataPointer = ptr4;
									ptr8[3].Size = (uint)((value4.Length + 1) * 2);
									ptr8[4].DataPointer = ptr5;
									ptr8[4].Size = (uint)((value5.Length + 1) * 2);
									ptr8[5].DataPointer = ptr6;
									ptr8[5].Size = (uint)((value6.Length + 1) * 2);
									flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 6, (IntPtr)((void*)ptr7));
									text = null;
									text2 = null;
									text3 = null;
									text4 = null;
									text5 = null;
								}
								return flag;
							}
						}
					}
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4, string value5, string value6, string value7)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value5)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value6)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									bool flag;
									fixed (string text7 = value7)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										byte* ptr8 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 7)];
										UnsafeNativeMethods.EventData* ptr9 = (UnsafeNativeMethods.EventData*)ptr8;
										ptr9->DataPointer = ptr;
										ptr9->Size = (uint)((value1.Length + 1) * 2);
										ptr9[1].DataPointer = ptr2;
										ptr9[1].Size = (uint)((value2.Length + 1) * 2);
										ptr9[2].DataPointer = ptr3;
										ptr9[2].Size = (uint)((value3.Length + 1) * 2);
										ptr9[3].DataPointer = ptr4;
										ptr9[3].Size = (uint)((value4.Length + 1) * 2);
										ptr9[4].DataPointer = ptr5;
										ptr9[4].Size = (uint)((value5.Length + 1) * 2);
										ptr9[5].DataPointer = ptr6;
										ptr9[5].Size = (uint)((value6.Length + 1) * 2);
										ptr9[6].DataPointer = ptr7;
										ptr9[6].Size = (uint)((value7.Length + 1) * 2);
										flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 7, (IntPtr)((void*)ptr8));
										text = null;
										text2 = null;
										text3 = null;
										text4 = null;
										text5 = null;
										text6 = null;
									}
									return flag;
								}
							}
						}
					}
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4, string value5, string value6, string value7, string value8)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value5)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value6)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value7)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										bool flag;
										fixed (string text8 = value8)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											byte* ptr9 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 8)];
											UnsafeNativeMethods.EventData* ptr10 = (UnsafeNativeMethods.EventData*)ptr9;
											ptr10->DataPointer = ptr;
											ptr10->Size = (uint)((value1.Length + 1) * 2);
											ptr10[1].DataPointer = ptr2;
											ptr10[1].Size = (uint)((value2.Length + 1) * 2);
											ptr10[2].DataPointer = ptr3;
											ptr10[2].Size = (uint)((value3.Length + 1) * 2);
											ptr10[3].DataPointer = ptr4;
											ptr10[3].Size = (uint)((value4.Length + 1) * 2);
											ptr10[4].DataPointer = ptr5;
											ptr10[4].Size = (uint)((value5.Length + 1) * 2);
											ptr10[5].DataPointer = ptr6;
											ptr10[5].Size = (uint)((value6.Length + 1) * 2);
											ptr10[6].DataPointer = ptr7;
											ptr10[6].Size = (uint)((value7.Length + 1) * 2);
											ptr10[7].DataPointer = ptr8;
											ptr10[7].Size = (uint)((value8.Length + 1) * 2);
											flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 8, (IntPtr)((void*)ptr9));
											text = null;
											text2 = null;
											text3 = null;
											text4 = null;
											text5 = null;
											text6 = null;
											text7 = null;
										}
										return flag;
									}
								}
							}
						}
					}
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4, string value5, string value6, string value7, string value8, string value9)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value5)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value6)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value7)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value8)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											bool flag;
											fixed (string text9 = value9)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												byte* ptr10 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 9)];
												UnsafeNativeMethods.EventData* ptr11 = (UnsafeNativeMethods.EventData*)ptr10;
												ptr11->DataPointer = ptr;
												ptr11->Size = (uint)((value1.Length + 1) * 2);
												ptr11[1].DataPointer = ptr2;
												ptr11[1].Size = (uint)((value2.Length + 1) * 2);
												ptr11[2].DataPointer = ptr3;
												ptr11[2].Size = (uint)((value3.Length + 1) * 2);
												ptr11[3].DataPointer = ptr4;
												ptr11[3].Size = (uint)((value4.Length + 1) * 2);
												ptr11[4].DataPointer = ptr5;
												ptr11[4].Size = (uint)((value5.Length + 1) * 2);
												ptr11[5].DataPointer = ptr6;
												ptr11[5].Size = (uint)((value6.Length + 1) * 2);
												ptr11[6].DataPointer = ptr7;
												ptr11[6].Size = (uint)((value7.Length + 1) * 2);
												ptr11[7].DataPointer = ptr8;
												ptr11[7].Size = (uint)((value8.Length + 1) * 2);
												ptr11[8].DataPointer = ptr9;
												ptr11[8].Size = (uint)((value9.Length + 1) * 2);
												flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 9, (IntPtr)((void*)ptr10));
												text = null;
												text2 = null;
												text3 = null;
												text4 = null;
												text5 = null;
												text6 = null;
												text7 = null;
												text8 = null;
											}
											return flag;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value5)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value6)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value7)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value8)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text9 = value9)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												bool flag;
												fixed (string text10 = value10)
												{
													char* ptr10 = text10;
													if (ptr10 != null)
													{
														ptr10 += RuntimeHelpers.OffsetToStringData / 2;
													}
													byte* ptr11 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 10)];
													UnsafeNativeMethods.EventData* ptr12 = (UnsafeNativeMethods.EventData*)ptr11;
													ptr12->DataPointer = ptr;
													ptr12->Size = (uint)((value1.Length + 1) * 2);
													ptr12[1].DataPointer = ptr2;
													ptr12[1].Size = (uint)((value2.Length + 1) * 2);
													ptr12[2].DataPointer = ptr3;
													ptr12[2].Size = (uint)((value3.Length + 1) * 2);
													ptr12[3].DataPointer = ptr4;
													ptr12[3].Size = (uint)((value4.Length + 1) * 2);
													ptr12[4].DataPointer = ptr5;
													ptr12[4].Size = (uint)((value5.Length + 1) * 2);
													ptr12[5].DataPointer = ptr6;
													ptr12[5].Size = (uint)((value6.Length + 1) * 2);
													ptr12[6].DataPointer = ptr7;
													ptr12[6].Size = (uint)((value7.Length + 1) * 2);
													ptr12[7].DataPointer = ptr8;
													ptr12[7].Size = (uint)((value8.Length + 1) * 2);
													ptr12[8].DataPointer = ptr9;
													ptr12[8].Size = (uint)((value9.Length + 1) * 2);
													ptr12[9].DataPointer = ptr10;
													ptr12[9].Size = (uint)((value10.Length + 1) * 2);
													flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 10, (IntPtr)((void*)ptr11));
													text = null;
													text2 = null;
													text3 = null;
													text4 = null;
													text5 = null;
													text6 = null;
													text7 = null;
													text8 = null;
													text9 = null;
												}
												return flag;
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

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10, string value11)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			value11 = value11 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value5)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value6)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value7)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value8)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text9 = value9)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												fixed (string text10 = value10)
												{
													char* ptr10 = text10;
													if (ptr10 != null)
													{
														ptr10 += RuntimeHelpers.OffsetToStringData / 2;
													}
													bool flag;
													fixed (string text11 = value11)
													{
														char* ptr11 = text11;
														if (ptr11 != null)
														{
															ptr11 += RuntimeHelpers.OffsetToStringData / 2;
														}
														byte* ptr12 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 11)];
														UnsafeNativeMethods.EventData* ptr13 = (UnsafeNativeMethods.EventData*)ptr12;
														ptr13->DataPointer = ptr;
														ptr13->Size = (uint)((value1.Length + 1) * 2);
														ptr13[1].DataPointer = ptr2;
														ptr13[1].Size = (uint)((value2.Length + 1) * 2);
														ptr13[2].DataPointer = ptr3;
														ptr13[2].Size = (uint)((value3.Length + 1) * 2);
														ptr13[3].DataPointer = ptr4;
														ptr13[3].Size = (uint)((value4.Length + 1) * 2);
														ptr13[4].DataPointer = ptr5;
														ptr13[4].Size = (uint)((value5.Length + 1) * 2);
														ptr13[5].DataPointer = ptr6;
														ptr13[5].Size = (uint)((value6.Length + 1) * 2);
														ptr13[6].DataPointer = ptr7;
														ptr13[6].Size = (uint)((value7.Length + 1) * 2);
														ptr13[7].DataPointer = ptr8;
														ptr13[7].Size = (uint)((value8.Length + 1) * 2);
														ptr13[8].DataPointer = ptr9;
														ptr13[8].Size = (uint)((value9.Length + 1) * 2);
														ptr13[9].DataPointer = ptr10;
														ptr13[9].Size = (uint)((value10.Length + 1) * 2);
														ptr13[10].DataPointer = ptr11;
														ptr13[10].Size = (uint)((value11.Length + 1) * 2);
														flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 11, (IntPtr)((void*)ptr12));
														text = null;
														text2 = null;
														text3 = null;
														text4 = null;
														text5 = null;
														text6 = null;
														text7 = null;
														text8 = null;
														text9 = null;
														text10 = null;
													}
													return flag;
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

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10, string value11, string value12)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			value11 = value11 ?? string.Empty;
			value12 = value12 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value5)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value6)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value7)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value8)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text9 = value9)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												fixed (string text10 = value10)
												{
													char* ptr10 = text10;
													if (ptr10 != null)
													{
														ptr10 += RuntimeHelpers.OffsetToStringData / 2;
													}
													fixed (string text11 = value11)
													{
														char* ptr11 = text11;
														if (ptr11 != null)
														{
															ptr11 += RuntimeHelpers.OffsetToStringData / 2;
														}
														bool flag;
														fixed (string text12 = value12)
														{
															char* ptr12 = text12;
															if (ptr12 != null)
															{
																ptr12 += RuntimeHelpers.OffsetToStringData / 2;
															}
															byte* ptr13 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 12)];
															UnsafeNativeMethods.EventData* ptr14 = (UnsafeNativeMethods.EventData*)ptr13;
															ptr14->DataPointer = ptr;
															ptr14->Size = (uint)((value1.Length + 1) * 2);
															ptr14[1].DataPointer = ptr2;
															ptr14[1].Size = (uint)((value2.Length + 1) * 2);
															ptr14[2].DataPointer = ptr3;
															ptr14[2].Size = (uint)((value3.Length + 1) * 2);
															ptr14[3].DataPointer = ptr4;
															ptr14[3].Size = (uint)((value4.Length + 1) * 2);
															ptr14[4].DataPointer = ptr5;
															ptr14[4].Size = (uint)((value5.Length + 1) * 2);
															ptr14[5].DataPointer = ptr6;
															ptr14[5].Size = (uint)((value6.Length + 1) * 2);
															ptr14[6].DataPointer = ptr7;
															ptr14[6].Size = (uint)((value7.Length + 1) * 2);
															ptr14[7].DataPointer = ptr8;
															ptr14[7].Size = (uint)((value8.Length + 1) * 2);
															ptr14[8].DataPointer = ptr9;
															ptr14[8].Size = (uint)((value9.Length + 1) * 2);
															ptr14[9].DataPointer = ptr10;
															ptr14[9].Size = (uint)((value10.Length + 1) * 2);
															ptr14[10].DataPointer = ptr11;
															ptr14[10].Size = (uint)((value11.Length + 1) * 2);
															ptr14[11].DataPointer = ptr12;
															ptr14[11].Size = (uint)((value12.Length + 1) * 2);
															flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 12, (IntPtr)((void*)ptr13));
															text = null;
															text2 = null;
															text3 = null;
															text4 = null;
															text5 = null;
															text6 = null;
															text7 = null;
															text8 = null;
															text9 = null;
															text10 = null;
															text11 = null;
														}
														return flag;
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

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, string value2, string value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10, string value11, string value12, string value13)
		{
			value1 = value1 ?? string.Empty;
			value2 = value2 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			value11 = value11 ?? string.Empty;
			value12 = value12 ?? string.Empty;
			value13 = value13 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value2)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value3)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value4)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value5)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value6)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value7)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value8)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text9 = value9)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												fixed (string text10 = value10)
												{
													char* ptr10 = text10;
													if (ptr10 != null)
													{
														ptr10 += RuntimeHelpers.OffsetToStringData / 2;
													}
													fixed (string text11 = value11)
													{
														char* ptr11 = text11;
														if (ptr11 != null)
														{
															ptr11 += RuntimeHelpers.OffsetToStringData / 2;
														}
														fixed (string text12 = value12)
														{
															char* ptr12 = text12;
															if (ptr12 != null)
															{
																ptr12 += RuntimeHelpers.OffsetToStringData / 2;
															}
															bool flag;
															fixed (string text13 = value13)
															{
																char* ptr13 = text13;
																if (ptr13 != null)
																{
																	ptr13 += RuntimeHelpers.OffsetToStringData / 2;
																}
																byte* ptr14 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 13)];
																UnsafeNativeMethods.EventData* ptr15 = (UnsafeNativeMethods.EventData*)ptr14;
																ptr15->DataPointer = ptr;
																ptr15->Size = (uint)((value1.Length + 1) * 2);
																ptr15[1].DataPointer = ptr2;
																ptr15[1].Size = (uint)((value2.Length + 1) * 2);
																ptr15[2].DataPointer = ptr3;
																ptr15[2].Size = (uint)((value3.Length + 1) * 2);
																ptr15[3].DataPointer = ptr4;
																ptr15[3].Size = (uint)((value4.Length + 1) * 2);
																ptr15[4].DataPointer = ptr5;
																ptr15[4].Size = (uint)((value5.Length + 1) * 2);
																ptr15[5].DataPointer = ptr6;
																ptr15[5].Size = (uint)((value6.Length + 1) * 2);
																ptr15[6].DataPointer = ptr7;
																ptr15[6].Size = (uint)((value7.Length + 1) * 2);
																ptr15[7].DataPointer = ptr8;
																ptr15[7].Size = (uint)((value8.Length + 1) * 2);
																ptr15[8].DataPointer = ptr9;
																ptr15[8].Size = (uint)((value9.Length + 1) * 2);
																ptr15[9].DataPointer = ptr10;
																ptr15[9].Size = (uint)((value10.Length + 1) * 2);
																ptr15[10].DataPointer = ptr11;
																ptr15[10].Size = (uint)((value11.Length + 1) * 2);
																ptr15[11].DataPointer = ptr12;
																ptr15[11].Size = (uint)((value12.Length + 1) * 2);
																ptr15[12].DataPointer = ptr13;
																ptr15[12].Size = (uint)((value13.Length + 1) * 2);
																flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 13, (IntPtr)((void*)ptr14));
																text = null;
																text2 = null;
																text3 = null;
																text4 = null;
																text5 = null;
																text6 = null;
																text7 = null;
																text8 = null;
																text9 = null;
																text10 = null;
																text11 = null;
																text12 = null;
															}
															return flag;
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

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, int value1)
		{
			byte* ptr = stackalloc byte[(UIntPtr)sizeof(UnsafeNativeMethods.EventData)];
			UnsafeNativeMethods.EventData* ptr2 = (UnsafeNativeMethods.EventData*)ptr;
			ptr2->DataPointer = &value1;
			ptr2->Size = 4U;
			return base.WriteEvent(ref eventDescriptor, eventTraceActivity, 1, (IntPtr)((void*)ptr));
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, int value1, int value2)
		{
			byte* ptr = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 2)];
			UnsafeNativeMethods.EventData* ptr2 = (UnsafeNativeMethods.EventData*)ptr;
			ptr2->DataPointer = &value1;
			ptr2->Size = 4U;
			ptr2[1].DataPointer = &value2;
			ptr2[1].Size = 4U;
			return base.WriteEvent(ref eventDescriptor, eventTraceActivity, 2, (IntPtr)((void*)ptr));
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, int value1, int value2, int value3)
		{
			byte* ptr = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 3)];
			UnsafeNativeMethods.EventData* ptr2 = (UnsafeNativeMethods.EventData*)ptr;
			ptr2->DataPointer = &value1;
			ptr2->Size = 4U;
			ptr2[1].DataPointer = &value2;
			ptr2[1].Size = 4U;
			ptr2[2].DataPointer = &value3;
			ptr2[2].Size = 4U;
			return base.WriteEvent(ref eventDescriptor, eventTraceActivity, 3, (IntPtr)((void*)ptr));
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, long value1)
		{
			byte* ptr = stackalloc byte[(UIntPtr)sizeof(UnsafeNativeMethods.EventData)];
			UnsafeNativeMethods.EventData* ptr2 = (UnsafeNativeMethods.EventData*)ptr;
			ptr2->DataPointer = &value1;
			ptr2->Size = 8U;
			return base.WriteEvent(ref eventDescriptor, eventTraceActivity, 1, (IntPtr)((void*)ptr));
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, long value1, long value2)
		{
			byte* ptr = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 2)];
			UnsafeNativeMethods.EventData* ptr2 = (UnsafeNativeMethods.EventData*)ptr;
			ptr2->DataPointer = &value1;
			ptr2->Size = 8U;
			ptr2[1].DataPointer = &value2;
			ptr2[1].Size = 8U;
			return base.WriteEvent(ref eventDescriptor, eventTraceActivity, 2, (IntPtr)((void*)ptr));
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, long value1, long value2, long value3)
		{
			byte* ptr = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 3)];
			UnsafeNativeMethods.EventData* ptr2 = (UnsafeNativeMethods.EventData*)ptr;
			ptr2->DataPointer = &value1;
			ptr2->Size = 8U;
			ptr2[1].DataPointer = &value2;
			ptr2[1].Size = 8U;
			ptr2[2].DataPointer = &value3;
			ptr2[2].Size = 8U;
			return base.WriteEvent(ref eventDescriptor, eventTraceActivity, 3, (IntPtr)((void*)ptr));
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, long value2, long value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10, string value11, string value12, string value13, string value14, string value15)
		{
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			value11 = value11 ?? string.Empty;
			value12 = value12 ?? string.Empty;
			value13 = value13 ?? string.Empty;
			value14 = value14 ?? string.Empty;
			value15 = value15 ?? string.Empty;
			fixed (string text = value4)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value5)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value6)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value7)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value8)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value9)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value10)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value11)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text9 = value12)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												fixed (string text10 = value13)
												{
													char* ptr10 = text10;
													if (ptr10 != null)
													{
														ptr10 += RuntimeHelpers.OffsetToStringData / 2;
													}
													fixed (string text11 = value14)
													{
														char* ptr11 = text11;
														if (ptr11 != null)
														{
															ptr11 += RuntimeHelpers.OffsetToStringData / 2;
														}
														bool flag;
														fixed (string text12 = value15)
														{
															char* ptr12 = text12;
															if (ptr12 != null)
															{
																ptr12 += RuntimeHelpers.OffsetToStringData / 2;
															}
															byte* ptr13 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 15)];
															UnsafeNativeMethods.EventData* ptr14 = (UnsafeNativeMethods.EventData*)ptr13;
															ptr14->DataPointer = &value1;
															ptr14->Size = (uint)sizeof(Guid);
															ptr14[1].DataPointer = &value2;
															ptr14[1].Size = 8U;
															ptr14[2].DataPointer = &value3;
															ptr14[2].Size = 8U;
															ptr14[3].DataPointer = ptr;
															ptr14[3].Size = (uint)((value4.Length + 1) * 2);
															ptr14[4].DataPointer = ptr2;
															ptr14[4].Size = (uint)((value5.Length + 1) * 2);
															ptr14[5].DataPointer = ptr3;
															ptr14[5].Size = (uint)((value6.Length + 1) * 2);
															ptr14[6].DataPointer = ptr4;
															ptr14[6].Size = (uint)((value7.Length + 1) * 2);
															ptr14[7].DataPointer = ptr5;
															ptr14[7].Size = (uint)((value8.Length + 1) * 2);
															ptr14[8].DataPointer = ptr6;
															ptr14[8].Size = (uint)((value9.Length + 1) * 2);
															ptr14[9].DataPointer = ptr7;
															ptr14[9].Size = (uint)((value10.Length + 1) * 2);
															ptr14[10].DataPointer = ptr8;
															ptr14[10].Size = (uint)((value11.Length + 1) * 2);
															ptr14[11].DataPointer = ptr9;
															ptr14[11].Size = (uint)((value12.Length + 1) * 2);
															ptr14[12].DataPointer = ptr10;
															ptr14[12].Size = (uint)((value13.Length + 1) * 2);
															ptr14[13].DataPointer = ptr11;
															ptr14[13].Size = (uint)((value14.Length + 1) * 2);
															ptr14[14].DataPointer = ptr12;
															ptr14[14].Size = (uint)((value15.Length + 1) * 2);
															flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 15, (IntPtr)((void*)ptr13));
															text = null;
															text2 = null;
															text3 = null;
															text4 = null;
															text5 = null;
															text6 = null;
															text7 = null;
															text8 = null;
															text9 = null;
															text10 = null;
															text11 = null;
														}
														return flag;
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

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, long value2, long value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10, string value11, string value12, bool value13, string value14, string value15, string value16, string value17)
		{
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			value11 = value11 ?? string.Empty;
			value12 = value12 ?? string.Empty;
			value14 = value14 ?? string.Empty;
			value15 = value15 ?? string.Empty;
			value16 = value16 ?? string.Empty;
			value17 = value17 ?? string.Empty;
			fixed (string text = value4)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value5)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value6)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value7)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value8)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value9)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value10)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value11)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text9 = value12)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												fixed (string text10 = value14)
												{
													char* ptr10 = text10;
													if (ptr10 != null)
													{
														ptr10 += RuntimeHelpers.OffsetToStringData / 2;
													}
													fixed (string text11 = value15)
													{
														char* ptr11 = text11;
														if (ptr11 != null)
														{
															ptr11 += RuntimeHelpers.OffsetToStringData / 2;
														}
														fixed (string text12 = value16)
														{
															char* ptr12 = text12;
															if (ptr12 != null)
															{
																ptr12 += RuntimeHelpers.OffsetToStringData / 2;
															}
															bool flag;
															fixed (string text13 = value17)
															{
																char* ptr13 = text13;
																if (ptr13 != null)
																{
																	ptr13 += RuntimeHelpers.OffsetToStringData / 2;
																}
																byte* ptr14 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 17)];
																UnsafeNativeMethods.EventData* ptr15 = (UnsafeNativeMethods.EventData*)ptr14;
																ptr15->DataPointer = &value1;
																ptr15->Size = (uint)sizeof(Guid);
																ptr15[1].DataPointer = &value2;
																ptr15[1].Size = 8U;
																ptr15[2].DataPointer = &value3;
																ptr15[2].Size = 8U;
																ptr15[3].DataPointer = ptr;
																ptr15[3].Size = (uint)((value4.Length + 1) * 2);
																ptr15[4].DataPointer = ptr2;
																ptr15[4].Size = (uint)((value5.Length + 1) * 2);
																ptr15[5].DataPointer = ptr3;
																ptr15[5].Size = (uint)((value6.Length + 1) * 2);
																ptr15[6].DataPointer = ptr4;
																ptr15[6].Size = (uint)((value7.Length + 1) * 2);
																ptr15[7].DataPointer = ptr5;
																ptr15[7].Size = (uint)((value8.Length + 1) * 2);
																ptr15[8].DataPointer = ptr6;
																ptr15[8].Size = (uint)((value9.Length + 1) * 2);
																ptr15[9].DataPointer = ptr7;
																ptr15[9].Size = (uint)((value10.Length + 1) * 2);
																ptr15[10].DataPointer = ptr8;
																ptr15[10].Size = (uint)((value11.Length + 1) * 2);
																ptr15[11].DataPointer = ptr9;
																ptr15[11].Size = (uint)((value12.Length + 1) * 2);
																ptr15[12].DataPointer = &value13;
																ptr15[12].Size = 1U;
																ptr15[13].DataPointer = ptr10;
																ptr15[13].Size = (uint)((value14.Length + 1) * 2);
																ptr15[14].DataPointer = ptr11;
																ptr15[14].Size = (uint)((value15.Length + 1) * 2);
																ptr15[15].DataPointer = ptr12;
																ptr15[15].Size = (uint)((value16.Length + 1) * 2);
																ptr15[16].DataPointer = ptr13;
																ptr15[16].Size = (uint)((value17.Length + 1) * 2);
																flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 17, (IntPtr)((void*)ptr14));
																text = null;
																text2 = null;
																text3 = null;
																text4 = null;
																text5 = null;
																text6 = null;
																text7 = null;
																text8 = null;
																text9 = null;
																text10 = null;
																text11 = null;
																text12 = null;
															}
															return flag;
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

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, long value2, long value3, string value4, string value5, string value6, string value7, string value8, string value9)
		{
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			fixed (string text = value4)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value5)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value6)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value7)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value8)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								bool flag;
								fixed (string text6 = value9)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									byte* ptr7 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 9)];
									UnsafeNativeMethods.EventData* ptr8 = (UnsafeNativeMethods.EventData*)ptr7;
									ptr8->DataPointer = &value1;
									ptr8->Size = (uint)sizeof(Guid);
									ptr8[1].DataPointer = &value2;
									ptr8[1].Size = 8U;
									ptr8[2].DataPointer = &value3;
									ptr8[2].Size = 8U;
									ptr8[3].DataPointer = ptr;
									ptr8[3].Size = (uint)((value4.Length + 1) * 2);
									ptr8[4].DataPointer = ptr2;
									ptr8[4].Size = (uint)((value5.Length + 1) * 2);
									ptr8[5].DataPointer = ptr3;
									ptr8[5].Size = (uint)((value6.Length + 1) * 2);
									ptr8[6].DataPointer = ptr4;
									ptr8[6].Size = (uint)((value7.Length + 1) * 2);
									ptr8[7].DataPointer = ptr5;
									ptr8[7].Size = (uint)((value8.Length + 1) * 2);
									ptr8[8].DataPointer = ptr6;
									ptr8[8].Size = (uint)((value9.Length + 1) * 2);
									flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 9, (IntPtr)((void*)ptr7));
									text = null;
									text2 = null;
									text3 = null;
									text4 = null;
									text5 = null;
								}
								return flag;
							}
						}
					}
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, long value2, long value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10, string value11)
		{
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			value11 = value11 ?? string.Empty;
			fixed (string text = value4)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value5)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value6)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value7)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value8)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value9)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value10)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										bool flag;
										fixed (string text8 = value11)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											byte* ptr9 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 11)];
											UnsafeNativeMethods.EventData* ptr10 = (UnsafeNativeMethods.EventData*)ptr9;
											ptr10->DataPointer = &value1;
											ptr10->Size = (uint)sizeof(Guid);
											ptr10[1].DataPointer = &value2;
											ptr10[1].Size = 8U;
											ptr10[2].DataPointer = &value3;
											ptr10[2].Size = 8U;
											ptr10[3].DataPointer = ptr;
											ptr10[3].Size = (uint)((value4.Length + 1) * 2);
											ptr10[4].DataPointer = ptr2;
											ptr10[4].Size = (uint)((value5.Length + 1) * 2);
											ptr10[5].DataPointer = ptr3;
											ptr10[5].Size = (uint)((value6.Length + 1) * 2);
											ptr10[6].DataPointer = ptr4;
											ptr10[6].Size = (uint)((value7.Length + 1) * 2);
											ptr10[7].DataPointer = ptr5;
											ptr10[7].Size = (uint)((value8.Length + 1) * 2);
											ptr10[8].DataPointer = ptr6;
											ptr10[8].Size = (uint)((value9.Length + 1) * 2);
											ptr10[9].DataPointer = ptr7;
											ptr10[9].Size = (uint)((value10.Length + 1) * 2);
											ptr10[10].DataPointer = ptr8;
											ptr10[10].Size = (uint)((value11.Length + 1) * 2);
											flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 11, (IntPtr)((void*)ptr9));
											text = null;
											text2 = null;
											text3 = null;
											text4 = null;
											text5 = null;
											text6 = null;
											text7 = null;
										}
										return flag;
									}
								}
							}
						}
					}
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, long value2, long value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10, string value11, string value12, string value13)
		{
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			value11 = value11 ?? string.Empty;
			value12 = value12 ?? string.Empty;
			value13 = value13 ?? string.Empty;
			fixed (string text = value4)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value5)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value6)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value7)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value8)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value9)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value10)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value11)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text9 = value12)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												bool flag;
												fixed (string text10 = value13)
												{
													char* ptr10 = text10;
													if (ptr10 != null)
													{
														ptr10 += RuntimeHelpers.OffsetToStringData / 2;
													}
													byte* ptr11 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 13)];
													UnsafeNativeMethods.EventData* ptr12 = (UnsafeNativeMethods.EventData*)ptr11;
													ptr12->DataPointer = &value1;
													ptr12->Size = (uint)sizeof(Guid);
													ptr12[1].DataPointer = &value2;
													ptr12[1].Size = 8U;
													ptr12[2].DataPointer = &value3;
													ptr12[2].Size = 8U;
													ptr12[3].DataPointer = ptr;
													ptr12[3].Size = (uint)((value4.Length + 1) * 2);
													ptr12[4].DataPointer = ptr2;
													ptr12[4].Size = (uint)((value5.Length + 1) * 2);
													ptr12[5].DataPointer = ptr3;
													ptr12[5].Size = (uint)((value6.Length + 1) * 2);
													ptr12[6].DataPointer = ptr4;
													ptr12[6].Size = (uint)((value7.Length + 1) * 2);
													ptr12[7].DataPointer = ptr5;
													ptr12[7].Size = (uint)((value8.Length + 1) * 2);
													ptr12[8].DataPointer = ptr6;
													ptr12[8].Size = (uint)((value9.Length + 1) * 2);
													ptr12[9].DataPointer = ptr7;
													ptr12[9].Size = (uint)((value10.Length + 1) * 2);
													ptr12[10].DataPointer = ptr8;
													ptr12[10].Size = (uint)((value11.Length + 1) * 2);
													ptr12[11].DataPointer = ptr9;
													ptr12[11].Size = (uint)((value12.Length + 1) * 2);
													ptr12[12].DataPointer = ptr10;
													ptr12[12].Size = (uint)((value13.Length + 1) * 2);
													flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 13, (IntPtr)((void*)ptr11));
													text = null;
													text2 = null;
													text3 = null;
													text4 = null;
													text5 = null;
													text6 = null;
													text7 = null;
													text8 = null;
													text9 = null;
												}
												return flag;
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

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, long value2, long value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10, string value11, string value12, string value13, string value14)
		{
			value4 = value4 ?? string.Empty;
			value5 = value5 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			value11 = value11 ?? string.Empty;
			value12 = value12 ?? string.Empty;
			value13 = value13 ?? string.Empty;
			value14 = value14 ?? string.Empty;
			fixed (string text = value4)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value5)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value6)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value7)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value8)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value9)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value10)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value11)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											fixed (string text9 = value12)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												fixed (string text10 = value13)
												{
													char* ptr10 = text10;
													if (ptr10 != null)
													{
														ptr10 += RuntimeHelpers.OffsetToStringData / 2;
													}
													bool flag;
													fixed (string text11 = value14)
													{
														char* ptr11 = text11;
														if (ptr11 != null)
														{
															ptr11 += RuntimeHelpers.OffsetToStringData / 2;
														}
														byte* ptr12 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 14)];
														UnsafeNativeMethods.EventData* ptr13 = (UnsafeNativeMethods.EventData*)ptr12;
														ptr13->DataPointer = &value1;
														ptr13->Size = (uint)sizeof(Guid);
														ptr13[1].DataPointer = &value2;
														ptr13[1].Size = 8U;
														ptr13[2].DataPointer = &value3;
														ptr13[2].Size = 8U;
														ptr13[3].DataPointer = ptr;
														ptr13[3].Size = (uint)((value4.Length + 1) * 2);
														ptr13[4].DataPointer = ptr2;
														ptr13[4].Size = (uint)((value5.Length + 1) * 2);
														ptr13[5].DataPointer = ptr3;
														ptr13[5].Size = (uint)((value6.Length + 1) * 2);
														ptr13[6].DataPointer = ptr4;
														ptr13[6].Size = (uint)((value7.Length + 1) * 2);
														ptr13[7].DataPointer = ptr5;
														ptr13[7].Size = (uint)((value8.Length + 1) * 2);
														ptr13[8].DataPointer = ptr6;
														ptr13[8].Size = (uint)((value9.Length + 1) * 2);
														ptr13[9].DataPointer = ptr7;
														ptr13[9].Size = (uint)((value10.Length + 1) * 2);
														ptr13[10].DataPointer = ptr8;
														ptr13[10].Size = (uint)((value11.Length + 1) * 2);
														ptr13[11].DataPointer = ptr9;
														ptr13[11].Size = (uint)((value12.Length + 1) * 2);
														ptr13[12].DataPointer = ptr10;
														ptr13[12].Size = (uint)((value13.Length + 1) * 2);
														ptr13[13].DataPointer = ptr11;
														ptr13[13].Size = (uint)((value14.Length + 1) * 2);
														flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 14, (IntPtr)((void*)ptr12));
														text = null;
														text2 = null;
														text3 = null;
														text4 = null;
														text5 = null;
														text6 = null;
														text7 = null;
														text8 = null;
														text9 = null;
														text10 = null;
													}
													return flag;
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

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, long value2, long value3, string value4, Guid value5, string value6, string value7, string value8, string value9, string value10, string value11, string value12, string value13)
		{
			value4 = value4 ?? string.Empty;
			value6 = value6 ?? string.Empty;
			value7 = value7 ?? string.Empty;
			value8 = value8 ?? string.Empty;
			value9 = value9 ?? string.Empty;
			value10 = value10 ?? string.Empty;
			value11 = value11 ?? string.Empty;
			value12 = value12 ?? string.Empty;
			value13 = value13 ?? string.Empty;
			fixed (string text = value4)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value6)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text3 = value7)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text4 = value8)
						{
							char* ptr4 = text4;
							if (ptr4 != null)
							{
								ptr4 += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (string text5 = value9)
							{
								char* ptr5 = text5;
								if (ptr5 != null)
								{
									ptr5 += RuntimeHelpers.OffsetToStringData / 2;
								}
								fixed (string text6 = value10)
								{
									char* ptr6 = text6;
									if (ptr6 != null)
									{
										ptr6 += RuntimeHelpers.OffsetToStringData / 2;
									}
									fixed (string text7 = value11)
									{
										char* ptr7 = text7;
										if (ptr7 != null)
										{
											ptr7 += RuntimeHelpers.OffsetToStringData / 2;
										}
										fixed (string text8 = value12)
										{
											char* ptr8 = text8;
											if (ptr8 != null)
											{
												ptr8 += RuntimeHelpers.OffsetToStringData / 2;
											}
											bool flag;
											fixed (string text9 = value13)
											{
												char* ptr9 = text9;
												if (ptr9 != null)
												{
													ptr9 += RuntimeHelpers.OffsetToStringData / 2;
												}
												byte* ptr10 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 13)];
												UnsafeNativeMethods.EventData* ptr11 = (UnsafeNativeMethods.EventData*)ptr10;
												ptr11->DataPointer = &value1;
												ptr11->Size = (uint)sizeof(Guid);
												ptr11[1].DataPointer = &value2;
												ptr11[1].Size = 8U;
												ptr11[2].DataPointer = &value3;
												ptr11[2].Size = 8U;
												ptr11[3].DataPointer = ptr;
												ptr11[3].Size = (uint)((value4.Length + 1) * 2);
												ptr11[4].DataPointer = &value5;
												ptr11[4].Size = (uint)sizeof(Guid);
												ptr11[5].DataPointer = ptr2;
												ptr11[5].Size = (uint)((value6.Length + 1) * 2);
												ptr11[6].DataPointer = ptr3;
												ptr11[6].Size = (uint)((value7.Length + 1) * 2);
												ptr11[7].DataPointer = ptr4;
												ptr11[7].Size = (uint)((value8.Length + 1) * 2);
												ptr11[8].DataPointer = ptr5;
												ptr11[8].Size = (uint)((value9.Length + 1) * 2);
												ptr11[9].DataPointer = ptr6;
												ptr11[9].Size = (uint)((value10.Length + 1) * 2);
												ptr11[10].DataPointer = ptr7;
												ptr11[10].Size = (uint)((value11.Length + 1) * 2);
												ptr11[11].DataPointer = ptr8;
												ptr11[11].Size = (uint)((value12.Length + 1) * 2);
												ptr11[12].DataPointer = ptr9;
												ptr11[12].Size = (uint)((value13.Length + 1) * 2);
												flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 13, (IntPtr)((void*)ptr10));
												text = null;
												text2 = null;
												text3 = null;
												text4 = null;
												text5 = null;
												text6 = null;
												text7 = null;
												text8 = null;
											}
											return flag;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		[SecurityCritical]
		internal unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, long value2, string value3, string value4)
		{
			value1 = value1 ?? string.Empty;
			value3 = value3 ?? string.Empty;
			value4 = value4 ?? string.Empty;
			fixed (string text = value1)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = value3)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					bool flag;
					fixed (string text3 = value4)
					{
						char* ptr3 = text3;
						if (ptr3 != null)
						{
							ptr3 += RuntimeHelpers.OffsetToStringData / 2;
						}
						byte* ptr4 = stackalloc byte[(UIntPtr)(sizeof(UnsafeNativeMethods.EventData) * 4)];
						UnsafeNativeMethods.EventData* ptr5 = (UnsafeNativeMethods.EventData*)ptr4;
						ptr5->DataPointer = ptr;
						ptr5->Size = (uint)((value1.Length + 1) * 2);
						ptr5[1].DataPointer = &value2;
						ptr5[1].Size = 8U;
						ptr5[2].DataPointer = ptr2;
						ptr5[2].Size = (uint)((value3.Length + 1) * 2);
						ptr5[3].DataPointer = ptr3;
						ptr5[3].Size = (uint)((value4.Length + 1) * 2);
						flag = base.WriteEvent(ref eventDescriptor, eventTraceActivity, 4, (IntPtr)((void*)ptr4));
						text = null;
						text2 = null;
					}
					return flag;
				}
			}
		}

		private Action invokeControllerCallback;

		private bool end2EndActivityTracingEnabled;
	}
}
