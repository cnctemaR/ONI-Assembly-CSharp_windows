using System;
using System.Text;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	internal sealed class _AndroidJNIHelper
	{
		public static IntPtr CreateJavaProxy(IntPtr player, IntPtr delegateHandle, AndroidJavaProxy proxy)
		{
			return AndroidReflection.NewProxyInstance(player, delegateHandle, proxy.javaInterface.GetRawClass());
		}

		public static IntPtr CreateJavaRunnable(AndroidJavaRunnable jrunnable)
		{
			return AndroidJNIHelper.CreateJavaProxy(new AndroidJavaRunnableProxy(jrunnable));
		}

		[RequiredByNativeCode]
		public static IntPtr InvokeJavaProxyMethod(AndroidJavaProxy proxy, IntPtr jmethodName, IntPtr jargs)
		{
			IntPtr intPtr;
			try
			{
				intPtr = proxy.Invoke(AndroidJNI.GetStringChars(jmethodName), jargs);
			}
			catch (Exception ex)
			{
				intPtr = AndroidReflection.CreateInvocationError(ex, false);
			}
			return intPtr;
		}

		public static void CreateJNIArgArray(object[] args, Span<jvalue> ret)
		{
			int num = 0;
			foreach (object obj in args)
			{
				bool flag = obj == null;
				if (flag)
				{
					ret[num].l = IntPtr.Zero;
				}
				else
				{
					bool flag2 = AndroidReflection.IsPrimitive(obj.GetType());
					if (flag2)
					{
						bool flag3 = obj is int;
						if (flag3)
						{
							ret[num].i = (int)obj;
						}
						else
						{
							bool flag4 = obj is bool;
							if (flag4)
							{
								ret[num].z = (bool)obj;
							}
							else
							{
								bool flag5 = obj is byte;
								if (flag5)
								{
									Debug.LogWarning("Passing Byte arguments to Java methods is obsolete, pass SByte parameters instead");
									ret[num].b = (sbyte)((byte)obj);
								}
								else
								{
									bool flag6 = obj is sbyte;
									if (flag6)
									{
										ret[num].b = (sbyte)obj;
									}
									else
									{
										bool flag7 = obj is short;
										if (flag7)
										{
											ret[num].s = (short)obj;
										}
										else
										{
											bool flag8 = obj is long;
											if (flag8)
											{
												ret[num].j = (long)obj;
											}
											else
											{
												bool flag9 = obj is float;
												if (flag9)
												{
													ret[num].f = (float)obj;
												}
												else
												{
													bool flag10 = obj is double;
													if (flag10)
													{
														ret[num].d = (double)obj;
													}
													else
													{
														bool flag11 = obj is char;
														if (flag11)
														{
															ret[num].c = (char)obj;
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
					else
					{
						bool flag12 = obj is string;
						if (flag12)
						{
							ret[num].l = AndroidJNISafe.NewString((string)obj);
						}
						else
						{
							bool flag13 = obj is AndroidJavaClass;
							if (flag13)
							{
								ret[num].l = ((AndroidJavaClass)obj).GetRawClass();
							}
							else
							{
								bool flag14 = obj is AndroidJavaObject;
								if (flag14)
								{
									ret[num].l = ((AndroidJavaObject)obj).GetRawObject();
								}
								else
								{
									bool flag15 = obj is Array;
									if (flag15)
									{
										ret[num].l = _AndroidJNIHelper.ConvertToJNIArray((Array)obj);
									}
									else
									{
										bool flag16 = obj is AndroidJavaProxy;
										if (flag16)
										{
											ret[num].l = ((AndroidJavaProxy)obj).GetRawProxy();
										}
										else
										{
											bool flag17 = obj is AndroidJavaRunnable;
											if (!flag17)
											{
												string text = "JNI; Unknown argument type '";
												Type type = obj.GetType();
												throw new Exception(text + ((type != null) ? type.ToString() : null) + "'");
											}
											ret[num].l = AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable)obj);
										}
									}
								}
							}
						}
					}
				}
				num++;
			}
		}

		public static object UnboxArray(AndroidJavaObject obj)
		{
			bool flag = obj == null;
			object obj2;
			if (flag)
			{
				obj2 = null;
			}
			else
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("java/lang/reflect/Array");
				AndroidJavaObject androidJavaObject = obj.Call<AndroidJavaObject>("getClass", Array.Empty<object>());
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getComponentType", Array.Empty<object>());
				string text = androidJavaObject2.Call<string>("getName", Array.Empty<object>());
				int num = androidJavaClass.CallStatic<int>("getLength", new object[] { obj });
				bool flag2 = androidJavaObject2.Call<bool>("isPrimitive", Array.Empty<object>());
				Array array;
				if (flag2)
				{
					bool flag3 = "int" == text;
					if (flag3)
					{
						array = new int[num];
					}
					else
					{
						bool flag4 = "boolean" == text;
						if (flag4)
						{
							array = new bool[num];
						}
						else
						{
							bool flag5 = "byte" == text;
							if (flag5)
							{
								array = new sbyte[num];
							}
							else
							{
								bool flag6 = "short" == text;
								if (flag6)
								{
									array = new short[num];
								}
								else
								{
									bool flag7 = "long" == text;
									if (flag7)
									{
										array = new long[num];
									}
									else
									{
										bool flag8 = "float" == text;
										if (flag8)
										{
											array = new float[num];
										}
										else
										{
											bool flag9 = "double" == text;
											if (flag9)
											{
												array = new double[num];
											}
											else
											{
												bool flag10 = "char" == text;
												if (!flag10)
												{
													throw new Exception("JNI; Unknown argument type '" + text + "'");
												}
												array = new char[num];
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
					bool flag11 = "java.lang.String" == text;
					if (flag11)
					{
						array = new string[num];
					}
					else
					{
						bool flag12 = "java.lang.Class" == text;
						if (flag12)
						{
							array = new AndroidJavaClass[num];
						}
						else
						{
							array = new AndroidJavaObject[num];
						}
					}
				}
				for (int i = 0; i < num; i++)
				{
					array.SetValue(_AndroidJNIHelper.Unbox(androidJavaClass.CallStatic<AndroidJavaObject>("get", new object[] { obj, i })), i);
				}
				androidJavaClass.Dispose();
				obj2 = array;
			}
			return obj2;
		}

		public static object Unbox(AndroidJavaObject obj)
		{
			bool flag = obj == null;
			object obj2;
			if (flag)
			{
				obj2 = null;
			}
			else
			{
				using (AndroidJavaObject androidJavaObject = obj.Call<AndroidJavaObject>("getClass", Array.Empty<object>()))
				{
					string text = androidJavaObject.Call<string>("getName", Array.Empty<object>());
					bool flag2 = "java.lang.Integer" == text;
					if (flag2)
					{
						obj2 = obj.Call<int>("intValue", Array.Empty<object>());
					}
					else
					{
						bool flag3 = "java.lang.Boolean" == text;
						if (flag3)
						{
							obj2 = obj.Call<bool>("booleanValue", Array.Empty<object>());
						}
						else
						{
							bool flag4 = "java.lang.Byte" == text;
							if (flag4)
							{
								obj2 = obj.Call<sbyte>("byteValue", Array.Empty<object>());
							}
							else
							{
								bool flag5 = "java.lang.Short" == text;
								if (flag5)
								{
									obj2 = obj.Call<short>("shortValue", Array.Empty<object>());
								}
								else
								{
									bool flag6 = "java.lang.Long" == text;
									if (flag6)
									{
										obj2 = obj.Call<long>("longValue", Array.Empty<object>());
									}
									else
									{
										bool flag7 = "java.lang.Float" == text;
										if (flag7)
										{
											obj2 = obj.Call<float>("floatValue", Array.Empty<object>());
										}
										else
										{
											bool flag8 = "java.lang.Double" == text;
											if (flag8)
											{
												obj2 = obj.Call<double>("doubleValue", Array.Empty<object>());
											}
											else
											{
												bool flag9 = "java.lang.Character" == text;
												if (flag9)
												{
													obj2 = obj.Call<char>("charValue", Array.Empty<object>());
												}
												else
												{
													bool flag10 = "java.lang.String" == text;
													if (flag10)
													{
														obj2 = obj.Call<string>("toString", Array.Empty<object>());
													}
													else
													{
														bool flag11 = "java.lang.Class" == text;
														if (flag11)
														{
															obj2 = new AndroidJavaClass(obj.GetRawObject());
														}
														else
														{
															bool flag12 = androidJavaObject.Call<bool>("isArray", Array.Empty<object>());
															if (flag12)
															{
																obj2 = _AndroidJNIHelper.UnboxArray(obj);
															}
															else
															{
																obj2 = obj;
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
			return obj2;
		}

		public static AndroidJavaObject Box(object obj)
		{
			bool flag = obj == null;
			AndroidJavaObject androidJavaObject;
			if (flag)
			{
				androidJavaObject = null;
			}
			else
			{
				bool flag2 = AndroidReflection.IsPrimitive(obj.GetType());
				if (flag2)
				{
					bool flag3 = obj is int;
					if (flag3)
					{
						androidJavaObject = new AndroidJavaObject("java.lang.Integer", new object[] { (int)obj });
					}
					else
					{
						bool flag4 = obj is bool;
						if (flag4)
						{
							androidJavaObject = new AndroidJavaObject("java.lang.Boolean", new object[] { (bool)obj });
						}
						else
						{
							bool flag5 = obj is byte;
							if (flag5)
							{
								androidJavaObject = new AndroidJavaObject("java.lang.Byte", new object[] { (sbyte)obj });
							}
							else
							{
								bool flag6 = obj is sbyte;
								if (flag6)
								{
									androidJavaObject = new AndroidJavaObject("java.lang.Byte", new object[] { (sbyte)obj });
								}
								else
								{
									bool flag7 = obj is short;
									if (flag7)
									{
										androidJavaObject = new AndroidJavaObject("java.lang.Short", new object[] { (short)obj });
									}
									else
									{
										bool flag8 = obj is long;
										if (flag8)
										{
											androidJavaObject = new AndroidJavaObject("java.lang.Long", new object[] { (long)obj });
										}
										else
										{
											bool flag9 = obj is float;
											if (flag9)
											{
												androidJavaObject = new AndroidJavaObject("java.lang.Float", new object[] { (float)obj });
											}
											else
											{
												bool flag10 = obj is double;
												if (flag10)
												{
													androidJavaObject = new AndroidJavaObject("java.lang.Double", new object[] { (double)obj });
												}
												else
												{
													bool flag11 = obj is char;
													if (!flag11)
													{
														string text = "JNI; Unknown argument type '";
														Type type = obj.GetType();
														throw new Exception(text + ((type != null) ? type.ToString() : null) + "'");
													}
													androidJavaObject = new AndroidJavaObject("java.lang.Character", new object[] { (char)obj });
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
					bool flag12 = obj is string;
					if (flag12)
					{
						androidJavaObject = new AndroidJavaObject("java.lang.String", new object[] { (string)obj });
					}
					else
					{
						bool flag13 = obj is AndroidJavaClass;
						if (flag13)
						{
							androidJavaObject = new AndroidJavaObject(((AndroidJavaClass)obj).GetRawClass());
						}
						else
						{
							bool flag14 = obj is AndroidJavaObject;
							if (flag14)
							{
								androidJavaObject = (AndroidJavaObject)obj;
							}
							else
							{
								bool flag15 = obj is Array;
								if (flag15)
								{
									androidJavaObject = AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(_AndroidJNIHelper.ConvertToJNIArray((Array)obj));
								}
								else
								{
									bool flag16 = obj is AndroidJavaProxy;
									if (flag16)
									{
										androidJavaObject = ((AndroidJavaProxy)obj).GetProxyObject();
									}
									else
									{
										bool flag17 = obj is AndroidJavaRunnable;
										if (!flag17)
										{
											string text2 = "JNI; Unknown argument type '";
											Type type2 = obj.GetType();
											throw new Exception(text2 + ((type2 != null) ? type2.ToString() : null) + "'");
										}
										androidJavaObject = AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable)obj));
									}
								}
							}
						}
					}
				}
			}
			return androidJavaObject;
		}

		public static void DeleteJNIArgArray(object[] args, Span<jvalue> jniArgs)
		{
			bool flag = args == null;
			if (!flag)
			{
				int num = 0;
				foreach (object obj in args)
				{
					bool flag2 = obj is string || obj is AndroidJavaRunnable || obj is AndroidJavaProxy || obj is Array;
					if (flag2)
					{
						AndroidJNISafe.DeleteLocalRef(jniArgs[num].l);
					}
					num++;
				}
			}
		}

		public static IntPtr ConvertToJNIArray(Array array)
		{
			Type elementType = array.GetType().GetElementType();
			bool flag = AndroidReflection.IsPrimitive(elementType);
			IntPtr intPtr;
			if (flag)
			{
				bool flag2 = elementType == typeof(int);
				if (flag2)
				{
					intPtr = AndroidJNISafe.ToIntArray((int[])array);
				}
				else
				{
					bool flag3 = elementType == typeof(bool);
					if (flag3)
					{
						intPtr = AndroidJNISafe.ToBooleanArray((bool[])array);
					}
					else
					{
						bool flag4 = elementType == typeof(byte);
						if (flag4)
						{
							Debug.LogWarning("AndroidJNIHelper: converting Byte array is obsolete, use SByte array instead");
							intPtr = AndroidJNISafe.ToByteArray((byte[])array);
						}
						else
						{
							bool flag5 = elementType == typeof(sbyte);
							if (flag5)
							{
								intPtr = AndroidJNISafe.ToSByteArray((sbyte[])array);
							}
							else
							{
								bool flag6 = elementType == typeof(short);
								if (flag6)
								{
									intPtr = AndroidJNISafe.ToShortArray((short[])array);
								}
								else
								{
									bool flag7 = elementType == typeof(long);
									if (flag7)
									{
										intPtr = AndroidJNISafe.ToLongArray((long[])array);
									}
									else
									{
										bool flag8 = elementType == typeof(float);
										if (flag8)
										{
											intPtr = AndroidJNISafe.ToFloatArray((float[])array);
										}
										else
										{
											bool flag9 = elementType == typeof(double);
											if (flag9)
											{
												intPtr = AndroidJNISafe.ToDoubleArray((double[])array);
											}
											else
											{
												bool flag10 = elementType == typeof(char);
												if (flag10)
												{
													intPtr = AndroidJNISafe.ToCharArray((char[])array);
												}
												else
												{
													intPtr = IntPtr.Zero;
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
			else
			{
				bool flag11 = elementType == typeof(string);
				if (flag11)
				{
					IntPtr intPtr2 = IntPtr.Zero;
					bool flag12 = false;
					try
					{
						string[] array2 = (string[])array;
						int length = array.GetLength(0);
						int num = length;
						bool flag13 = num > _AndroidJNIHelper.FRAME_SIZE_FOR_ARRAYS;
						if (flag13)
						{
							num = _AndroidJNIHelper.FRAME_SIZE_FOR_ARRAYS;
						}
						IntPtr intPtr3 = AndroidJNISafe.FindClass("java/lang/String");
						IntPtr intPtr4 = AndroidJNI.NewObjectArray(length, intPtr3, IntPtr.Zero);
						AndroidJNISafe.DeleteLocalRef(intPtr3);
						bool flag14 = num > 0;
						if (flag14)
						{
							AndroidJNISafe.PushLocalFrame(num);
							flag12 = true;
						}
						for (int i = 0; i < length; i++)
						{
							bool flag15 = i % _AndroidJNIHelper.FRAME_SIZE_FOR_ARRAYS == 0;
							if (flag15)
							{
								AndroidJNI.PopLocalFrame(IntPtr.Zero);
								flag12 = false;
								AndroidJNISafe.PushLocalFrame(num);
								flag12 = true;
							}
							IntPtr intPtr5 = AndroidJNISafe.NewString(array2[i]);
							AndroidJNI.SetObjectArrayElement(intPtr4, i, intPtr5);
						}
						intPtr2 = intPtr4;
					}
					finally
					{
						bool flag16 = flag12;
						if (flag16)
						{
							AndroidJNI.PopLocalFrame(IntPtr.Zero);
						}
					}
					intPtr = intPtr2;
				}
				else
				{
					bool flag17 = elementType == typeof(AndroidJavaObject);
					if (flag17)
					{
						AndroidJavaObject[] array3 = (AndroidJavaObject[])array;
						int length2 = array.GetLength(0);
						IntPtr[] array4 = new IntPtr[length2];
						IntPtr intPtr6 = AndroidJNISafe.FindClass("java/lang/Object");
						IntPtr intPtr7 = IntPtr.Zero;
						for (int j = 0; j < length2; j++)
						{
							bool flag18 = array3[j] != null;
							if (flag18)
							{
								array4[j] = array3[j].GetRawObject();
								IntPtr rawClass = array3[j].GetRawClass();
								bool flag19 = intPtr7 == IntPtr.Zero;
								if (flag19)
								{
									intPtr7 = rawClass;
								}
								else
								{
									bool flag20 = intPtr7 != intPtr6 && !AndroidJNI.IsSameObject(intPtr7, rawClass);
									if (flag20)
									{
										intPtr7 = intPtr6;
									}
								}
							}
							else
							{
								array4[j] = IntPtr.Zero;
							}
						}
						IntPtr intPtr8 = AndroidJNISafe.ToObjectArray(array4, intPtr7);
						AndroidJNISafe.DeleteLocalRef(intPtr6);
						intPtr = intPtr8;
					}
					else
					{
						bool flag21 = AndroidReflection.IsAssignableFrom(typeof(AndroidJavaProxy), elementType);
						if (!flag21)
						{
							string text = "JNI; Unknown array type '";
							Type type = elementType;
							throw new Exception(text + ((type != null) ? type.ToString() : null) + "'");
						}
						AndroidJavaProxy[] array5 = (AndroidJavaProxy[])array;
						int length3 = array.GetLength(0);
						IntPtr[] array6 = new IntPtr[length3];
						IntPtr intPtr9 = AndroidJNISafe.FindClass("java/lang/Object");
						IntPtr intPtr10 = IntPtr.Zero;
						for (int k = 0; k < length3; k++)
						{
							bool flag22 = array5[k] != null;
							if (flag22)
							{
								array6[k] = array5[k].GetRawProxy();
								IntPtr rawClass2 = array5[k].javaInterface.GetRawClass();
								bool flag23 = intPtr10 == IntPtr.Zero;
								if (flag23)
								{
									intPtr10 = rawClass2;
								}
								else
								{
									bool flag24 = intPtr10 != intPtr9 && !AndroidJNI.IsSameObject(intPtr10, rawClass2);
									if (flag24)
									{
										intPtr10 = intPtr9;
									}
								}
							}
							else
							{
								array6[k] = IntPtr.Zero;
							}
						}
						IntPtr intPtr11 = AndroidJNISafe.ToObjectArray(array6, intPtr10);
						AndroidJNISafe.DeleteLocalRef(intPtr9);
						intPtr = intPtr11;
					}
				}
			}
			return intPtr;
		}

		public static ArrayType ConvertFromJNIArray<ArrayType>(IntPtr array)
		{
			Type elementType = typeof(ArrayType).GetElementType();
			bool flag = AndroidReflection.IsPrimitive(elementType);
			ArrayType arrayType;
			if (flag)
			{
				bool flag2 = elementType == typeof(int);
				if (flag2)
				{
					arrayType = (ArrayType)((object)AndroidJNISafe.FromIntArray(array));
				}
				else
				{
					bool flag3 = elementType == typeof(bool);
					if (flag3)
					{
						arrayType = (ArrayType)((object)AndroidJNISafe.FromBooleanArray(array));
					}
					else
					{
						bool flag4 = elementType == typeof(byte);
						if (flag4)
						{
							Debug.LogWarning("AndroidJNIHelper: converting from Byte array is obsolete, use SByte array instead");
							arrayType = (ArrayType)((object)AndroidJNISafe.FromByteArray(array));
						}
						else
						{
							bool flag5 = elementType == typeof(sbyte);
							if (flag5)
							{
								arrayType = (ArrayType)((object)AndroidJNISafe.FromSByteArray(array));
							}
							else
							{
								bool flag6 = elementType == typeof(short);
								if (flag6)
								{
									arrayType = (ArrayType)((object)AndroidJNISafe.FromShortArray(array));
								}
								else
								{
									bool flag7 = elementType == typeof(long);
									if (flag7)
									{
										arrayType = (ArrayType)((object)AndroidJNISafe.FromLongArray(array));
									}
									else
									{
										bool flag8 = elementType == typeof(float);
										if (flag8)
										{
											arrayType = (ArrayType)((object)AndroidJNISafe.FromFloatArray(array));
										}
										else
										{
											bool flag9 = elementType == typeof(double);
											if (flag9)
											{
												arrayType = (ArrayType)((object)AndroidJNISafe.FromDoubleArray(array));
											}
											else
											{
												bool flag10 = elementType == typeof(char);
												if (flag10)
												{
													arrayType = (ArrayType)((object)AndroidJNISafe.FromCharArray(array));
												}
												else
												{
													arrayType = default(ArrayType);
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
			else
			{
				bool flag11 = elementType == typeof(string);
				if (flag11)
				{
					int arrayLength = AndroidJNISafe.GetArrayLength(array);
					string[] array2 = new string[arrayLength];
					bool flag12 = arrayLength == 0;
					if (flag12)
					{
						arrayType = (ArrayType)((object)array2);
					}
					else
					{
						int num = ((arrayLength > _AndroidJNIHelper.FRAME_SIZE_FOR_ARRAYS) ? _AndroidJNIHelper.FRAME_SIZE_FOR_ARRAYS : arrayLength);
						AndroidJNISafe.PushLocalFrame(num);
						bool flag13 = true;
						try
						{
							for (int i = 0; i < arrayLength; i++)
							{
								bool flag14 = i % _AndroidJNIHelper.FRAME_SIZE_FOR_ARRAYS == 0;
								if (flag14)
								{
									AndroidJNI.PopLocalFrame(IntPtr.Zero);
									flag13 = false;
									AndroidJNISafe.PushLocalFrame(num);
									flag13 = true;
								}
								IntPtr objectArrayElement = AndroidJNI.GetObjectArrayElement(array, i);
								array2[i] = AndroidJNISafe.GetStringChars(objectArrayElement);
							}
						}
						finally
						{
							bool flag15 = flag13;
							if (flag15)
							{
								AndroidJNI.PopLocalFrame(IntPtr.Zero);
							}
						}
						arrayType = (ArrayType)((object)array2);
					}
				}
				else
				{
					bool flag16 = elementType == typeof(AndroidJavaObject);
					if (!flag16)
					{
						string text = "JNI: Unknown generic array type '";
						Type type = elementType;
						throw new Exception(text + ((type != null) ? type.ToString() : null) + "'");
					}
					int arrayLength2 = AndroidJNISafe.GetArrayLength(array);
					AndroidJavaObject[] array3 = new AndroidJavaObject[arrayLength2];
					bool flag17 = arrayLength2 == 0;
					if (flag17)
					{
						arrayType = (ArrayType)((object)array3);
					}
					else
					{
						int num2 = ((arrayLength2 > _AndroidJNIHelper.FRAME_SIZE_FOR_ARRAYS) ? _AndroidJNIHelper.FRAME_SIZE_FOR_ARRAYS : arrayLength2);
						AndroidJNISafe.PushLocalFrame(num2);
						bool flag18 = true;
						try
						{
							for (int j = 0; j < arrayLength2; j++)
							{
								bool flag19 = j % _AndroidJNIHelper.FRAME_SIZE_FOR_ARRAYS == 0;
								if (flag19)
								{
									AndroidJNI.PopLocalFrame(IntPtr.Zero);
									flag18 = false;
									AndroidJNISafe.PushLocalFrame(num2);
									flag18 = true;
								}
								IntPtr objectArrayElement2 = AndroidJNI.GetObjectArrayElement(array, j);
								array3[j] = new AndroidJavaObject(objectArrayElement2);
							}
						}
						finally
						{
							bool flag20 = flag18;
							if (flag20)
							{
								AndroidJNI.PopLocalFrame(IntPtr.Zero);
							}
						}
						arrayType = (ArrayType)((object)array3);
					}
				}
			}
			return arrayType;
		}

		public static IntPtr GetConstructorID(IntPtr jclass, object[] args)
		{
			return AndroidJNIHelper.GetConstructorID(jclass, _AndroidJNIHelper.GetSignature(args));
		}

		public static IntPtr GetMethodID(IntPtr jclass, string methodName, object[] args, bool isStatic)
		{
			return AndroidJNIHelper.GetMethodID(jclass, methodName, _AndroidJNIHelper.GetSignature(args), isStatic);
		}

		public static IntPtr GetMethodID<ReturnType>(IntPtr jclass, string methodName, object[] args, bool isStatic)
		{
			return AndroidJNIHelper.GetMethodID(jclass, methodName, _AndroidJNIHelper.GetSignature<ReturnType>(args), isStatic);
		}

		public static IntPtr GetFieldID<ReturnType>(IntPtr jclass, string fieldName, bool isStatic)
		{
			return AndroidJNIHelper.GetFieldID(jclass, fieldName, _AndroidJNIHelper.GetSignature(typeof(ReturnType)), isStatic);
		}

		public static IntPtr GetConstructorID(IntPtr jclass, string signature)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2;
			try
			{
				intPtr = AndroidReflection.GetConstructorMember(jclass, signature);
				intPtr2 = AndroidJNISafe.FromReflectedMethod(intPtr);
			}
			catch (Exception ex)
			{
				IntPtr methodID = AndroidJNISafe.GetMethodID(jclass, "<init>", signature);
				bool flag = methodID != IntPtr.Zero;
				if (!flag)
				{
					throw ex;
				}
				intPtr2 = methodID;
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(intPtr);
			}
			return intPtr2;
		}

		public static IntPtr GetMethodID(IntPtr jclass, string methodName, string signature, bool isStatic)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2;
			try
			{
				intPtr = AndroidReflection.GetMethodMember(jclass, methodName, signature, isStatic);
				intPtr2 = AndroidJNISafe.FromReflectedMethod(intPtr);
			}
			catch (Exception ex)
			{
				IntPtr methodIDFallback = _AndroidJNIHelper.GetMethodIDFallback(jclass, methodName, signature, isStatic);
				bool flag = methodIDFallback != IntPtr.Zero;
				if (!flag)
				{
					throw ex;
				}
				intPtr2 = methodIDFallback;
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(intPtr);
			}
			return intPtr2;
		}

		private static IntPtr GetMethodIDFallback(IntPtr jclass, string methodName, string signature, bool isStatic)
		{
			try
			{
				return isStatic ? AndroidJNISafe.GetStaticMethodID(jclass, methodName, signature) : AndroidJNISafe.GetMethodID(jclass, methodName, signature);
			}
			catch (Exception)
			{
			}
			return IntPtr.Zero;
		}

		public static IntPtr GetFieldID(IntPtr jclass, string fieldName, string signature, bool isStatic)
		{
			IntPtr intPtr = IntPtr.Zero;
			Exception ex = null;
			AndroidJNI.PushLocalFrame(10);
			try
			{
				IntPtr fieldMember = AndroidReflection.GetFieldMember(jclass, fieldName, signature, isStatic);
				bool flag = !isStatic;
				if (flag)
				{
					jclass = AndroidReflection.GetFieldClass(fieldMember);
				}
				signature = AndroidReflection.GetFieldSignature(fieldMember);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			IntPtr intPtr2;
			try
			{
				intPtr = (isStatic ? AndroidJNISafe.GetStaticFieldID(jclass, fieldName, signature) : AndroidJNISafe.GetFieldID(jclass, fieldName, signature));
				bool flag2 = intPtr == IntPtr.Zero;
				if (flag2)
				{
					bool flag3 = ex != null;
					if (flag3)
					{
						throw ex;
					}
					throw new Exception(string.Format("Field {0} or type signature {1} not found", fieldName, signature));
				}
				else
				{
					intPtr2 = intPtr;
				}
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
			return intPtr2;
		}

		public static string GetSignature(object obj)
		{
			bool flag = obj == null;
			string text;
			if (flag)
			{
				text = "Ljava/lang/Object;";
			}
			else
			{
				Type type = ((obj is Type) ? ((Type)obj) : obj.GetType());
				bool flag2 = AndroidReflection.IsPrimitive(type);
				if (flag2)
				{
					bool flag3 = type.Equals(typeof(int));
					if (flag3)
					{
						text = "I";
					}
					else
					{
						bool flag4 = type.Equals(typeof(bool));
						if (flag4)
						{
							text = "Z";
						}
						else
						{
							bool flag5 = type.Equals(typeof(byte));
							if (flag5)
							{
								Debug.LogWarning("AndroidJNIHelper.GetSignature: using Byte parameters is obsolete, use SByte parameters instead");
								text = "B";
							}
							else
							{
								bool flag6 = type.Equals(typeof(sbyte));
								if (flag6)
								{
									text = "B";
								}
								else
								{
									bool flag7 = type.Equals(typeof(short));
									if (flag7)
									{
										text = "S";
									}
									else
									{
										bool flag8 = type.Equals(typeof(long));
										if (flag8)
										{
											text = "J";
										}
										else
										{
											bool flag9 = type.Equals(typeof(float));
											if (flag9)
											{
												text = "F";
											}
											else
											{
												bool flag10 = type.Equals(typeof(double));
												if (flag10)
												{
													text = "D";
												}
												else
												{
													bool flag11 = type.Equals(typeof(char));
													if (flag11)
													{
														text = "C";
													}
													else
													{
														text = "";
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
				else
				{
					bool flag12 = type.Equals(typeof(string));
					if (flag12)
					{
						text = "Ljava/lang/String;";
					}
					else
					{
						bool flag13 = obj is AndroidJavaProxy;
						if (flag13)
						{
							using (AndroidJavaObject androidJavaObject = new AndroidJavaObject(((AndroidJavaProxy)obj).javaInterface.GetRawClass()))
							{
								return "L" + androidJavaObject.Call<string>("getName", Array.Empty<object>()) + ";";
							}
						}
						bool flag14 = obj == type && AndroidReflection.IsAssignableFrom(typeof(AndroidJavaProxy), type);
						if (flag14)
						{
							text = "";
						}
						else
						{
							bool flag15 = type.Equals(typeof(AndroidJavaRunnable));
							if (flag15)
							{
								text = "Ljava/lang/Runnable;";
							}
							else
							{
								bool flag16 = obj is AndroidJavaClass || (obj == type && AndroidReflection.IsAssignableFrom(typeof(AndroidJavaClass), type));
								if (flag16)
								{
									text = "Ljava/lang/Class;";
								}
								else
								{
									bool flag17 = obj is AndroidJavaObject;
									if (flag17)
									{
										AndroidJavaObject androidJavaObject2 = (AndroidJavaObject)obj;
										using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getClass", Array.Empty<object>()))
										{
											return "L" + androidJavaObject3.Call<string>("getName", Array.Empty<object>()) + ";";
										}
									}
									bool flag18 = obj == type && AndroidReflection.IsAssignableFrom(typeof(AndroidJavaObject), type);
									if (flag18)
									{
										text = "Ljava/lang/Object;";
									}
									else
									{
										bool flag19 = AndroidReflection.IsAssignableFrom(typeof(Array), type);
										if (!flag19)
										{
											string[] array = new string[6];
											array[0] = "JNI: Unknown signature for type '";
											int num = 1;
											Type type2 = type;
											array[num] = ((type2 != null) ? type2.ToString() : null);
											array[2] = "' (obj = ";
											array[3] = ((obj != null) ? obj.ToString() : null);
											array[4] = ") ";
											array[5] = ((type == obj) ? "equal" : "instance");
											throw new Exception(string.Concat(array));
										}
										bool flag20 = type.GetArrayRank() != 1;
										if (flag20)
										{
											throw new Exception("JNI: System.Array in n dimensions is not allowed");
										}
										StringBuilder stringBuilder = new StringBuilder();
										stringBuilder.Append('[');
										stringBuilder.Append(_AndroidJNIHelper.GetSignature(type.GetElementType()));
										text = ((stringBuilder.Length > 1) ? stringBuilder.ToString() : "");
									}
								}
							}
						}
					}
				}
			}
			return text;
		}

		public static string GetSignature(object[] args)
		{
			bool flag = args == null || args.Length == 0;
			string text;
			if (flag)
			{
				text = "()V";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append('(');
				foreach (object obj in args)
				{
					stringBuilder.Append(_AndroidJNIHelper.GetSignature(obj));
				}
				stringBuilder.Append(")V");
				text = stringBuilder.ToString();
			}
			return text;
		}

		public static string GetSignature<ReturnType>(object[] args)
		{
			bool flag = args == null || args.Length == 0;
			string text;
			if (flag)
			{
				text = "()" + _AndroidJNIHelper.GetSignature(typeof(ReturnType));
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append('(');
				foreach (object obj in args)
				{
					stringBuilder.Append(_AndroidJNIHelper.GetSignature(obj));
				}
				stringBuilder.Append(')');
				stringBuilder.Append(_AndroidJNIHelper.GetSignature(typeof(ReturnType)));
				text = stringBuilder.ToString();
			}
			return text;
		}

		private static int FRAME_SIZE_FOR_ARRAYS = 100;
	}
}
