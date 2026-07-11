using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace System.Diagnostics
{
	internal static class TraceUtils
	{
		internal static object GetRuntimeObject(string className, Type baseType, string initializeData)
		{
			object obj = null;
			if (className.Length == 0)
			{
				throw new ConfigurationErrorsException(global::SR.GetString("switchType needs to be a valid class name. It can't be empty."));
			}
			Type type = Type.GetType(className);
			if (type == null)
			{
				throw new ConfigurationErrorsException(global::SR.GetString("Couldn't find type for class {0}.", new object[] { className }));
			}
			if (!baseType.IsAssignableFrom(type))
			{
				throw new ConfigurationErrorsException(global::SR.GetString("The specified type, '{0}' is not derived from the appropriate base type, '{1}'.", new object[] { className, baseType.FullName }));
			}
			Exception ex = null;
			try
			{
				if (string.IsNullOrEmpty(initializeData))
				{
					if (TraceUtils.IsOwnedTL(type))
					{
						throw new ConfigurationErrorsException(global::SR.GetString("initializeData needs to be valid for this TraceListener."));
					}
					ConstructorInfo constructor = type.GetConstructor(new Type[0]);
					if (constructor == null)
					{
						throw new ConfigurationErrorsException(global::SR.GetString("Couldn't find constructor for class {0}.", new object[] { className }));
					}
					obj = SecurityUtils.ConstructorInfoInvoke(constructor, new object[0]);
				}
				else
				{
					ConstructorInfo constructor2 = type.GetConstructor(new Type[] { typeof(string) });
					if (constructor2 != null)
					{
						if (TraceUtils.IsOwnedTextWriterTL(type) && initializeData[0] != Path.DirectorySeparatorChar && initializeData[0] != Path.AltDirectorySeparatorChar && !Path.IsPathRooted(initializeData))
						{
							string configFilePath = DiagnosticsConfiguration.ConfigFilePath;
							if (!string.IsNullOrEmpty(configFilePath))
							{
								string directoryName = Path.GetDirectoryName(configFilePath);
								if (directoryName != null)
								{
									initializeData = Path.Combine(directoryName, initializeData);
								}
							}
						}
						obj = SecurityUtils.ConstructorInfoInvoke(constructor2, new object[] { initializeData });
					}
					else
					{
						ConstructorInfo[] constructors = type.GetConstructors();
						if (constructors == null)
						{
							throw new ConfigurationErrorsException(global::SR.GetString("Couldn't find constructor for class {0}.", new object[] { className }));
						}
						for (int i = 0; i < constructors.Length; i++)
						{
							ParameterInfo[] parameters = constructors[i].GetParameters();
							if (parameters.Length == 1)
							{
								Type parameterType = parameters[0].ParameterType;
								try
								{
									object obj2 = TraceUtils.ConvertToBaseTypeOrEnum(initializeData, parameterType);
									obj = SecurityUtils.ConstructorInfoInvoke(constructors[i], new object[] { obj2 });
									break;
								}
								catch (TargetInvocationException ex2)
								{
									ex = ex2.InnerException;
								}
								catch (Exception ex)
								{
								}
							}
						}
					}
				}
			}
			catch (TargetInvocationException ex3)
			{
				ex = ex3.InnerException;
			}
			if (obj != null)
			{
				return obj;
			}
			if (ex != null)
			{
				throw new ConfigurationErrorsException(global::SR.GetString("Could not create {0}.", new object[] { className }), ex);
			}
			throw new ConfigurationErrorsException(global::SR.GetString("Could not create {0}.", new object[] { className }));
		}

		internal static bool IsOwnedTL(Type type)
		{
			return typeof(EventLogTraceListener) == type || TraceUtils.IsOwnedTextWriterTL(type);
		}

		internal static bool IsOwnedTextWriterTL(Type type)
		{
			return typeof(XmlWriterTraceListener) == type || typeof(DelimitedListTraceListener) == type || typeof(TextWriterTraceListener) == type;
		}

		private static object ConvertToBaseTypeOrEnum(string value, Type type)
		{
			if (type.IsEnum)
			{
				return Enum.Parse(type, value, false);
			}
			return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}

		internal static void VerifyAttributes(IDictionary attributes, string[] supportedAttributes, object parent)
		{
			foreach (object obj in attributes.Keys)
			{
				string text = (string)obj;
				bool flag = false;
				if (supportedAttributes != null)
				{
					for (int i = 0; i < supportedAttributes.Length; i++)
					{
						if (supportedAttributes[i] == text)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					throw new ConfigurationErrorsException(global::SR.GetString("'{0}' is not a valid configuration attribute for type '{1}'.", new object[]
					{
						text,
						parent.GetType().FullName
					}));
				}
			}
		}
	}
}
