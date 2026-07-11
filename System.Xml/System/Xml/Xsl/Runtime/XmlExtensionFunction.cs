using System;
using System.Globalization;
using System.Reflection;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlExtensionFunction
	{
		public XmlExtensionFunction()
		{
		}

		public XmlExtensionFunction(string name, string namespaceUri, MethodInfo meth)
		{
			this.name = name;
			this.namespaceUri = namespaceUri;
			this.Bind(meth);
		}

		public XmlExtensionFunction(string name, string namespaceUri, int numArgs, Type objectType, BindingFlags flags)
		{
			this.Init(name, namespaceUri, numArgs, objectType, flags);
		}

		public void Init(string name, string namespaceUri, int numArgs, Type objectType, BindingFlags flags)
		{
			this.name = name;
			this.namespaceUri = namespaceUri;
			this.numArgs = numArgs;
			this.objectType = objectType;
			this.flags = flags;
			this.meth = null;
			this.argClrTypes = null;
			this.retClrType = null;
			this.argXmlTypes = null;
			this.retXmlType = null;
			this.hashCode = namespaceUri.GetHashCode() ^ name.GetHashCode() ^ (int)((int)flags << 16) ^ numArgs;
		}

		public MethodInfo Method
		{
			get
			{
				return this.meth;
			}
		}

		public Type GetClrArgumentType(int index)
		{
			return this.argClrTypes[index];
		}

		public Type ClrReturnType
		{
			get
			{
				return this.retClrType;
			}
		}

		public XmlQueryType GetXmlArgumentType(int index)
		{
			return this.argXmlTypes[index];
		}

		public XmlQueryType XmlReturnType
		{
			get
			{
				return this.retXmlType;
			}
		}

		public bool CanBind()
		{
			MethodInfo[] methods = this.objectType.GetMethods(this.flags);
			StringComparison stringComparison = (((this.flags & BindingFlags.IgnoreCase) > BindingFlags.Default) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.Name.Equals(this.name, stringComparison) && (this.numArgs == -1 || methodInfo.GetParameters().Length == this.numArgs) && !methodInfo.IsGenericMethodDefinition)
				{
					return true;
				}
			}
			return false;
		}

		public void Bind()
		{
			MethodInfo[] methods = this.objectType.GetMethods(this.flags);
			MethodInfo methodInfo = null;
			StringComparison stringComparison = (((this.flags & BindingFlags.IgnoreCase) > BindingFlags.Default) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
			foreach (MethodInfo methodInfo2 in methods)
			{
				if (methodInfo2.Name.Equals(this.name, stringComparison) && (this.numArgs == -1 || methodInfo2.GetParameters().Length == this.numArgs))
				{
					if (methodInfo != null)
					{
						throw new XslTransformException("Ambiguous method call. Extension object '{0}' contains multiple '{1}' methods that have {2} parameter(s).", new string[]
						{
							this.namespaceUri,
							this.name,
							this.numArgs.ToString(CultureInfo.InvariantCulture)
						});
					}
					methodInfo = methodInfo2;
				}
			}
			if (methodInfo == null)
			{
				foreach (MethodInfo methodInfo3 in this.objectType.GetMethods(this.flags | BindingFlags.NonPublic))
				{
					if (methodInfo3.Name.Equals(this.name, stringComparison) && methodInfo3.GetParameters().Length == this.numArgs)
					{
						throw new XslTransformException("Method '{1}' of extension object '{0}' cannot be called because it is not public.", new string[] { this.namespaceUri, this.name });
					}
				}
				throw new XslTransformException("Extension object '{0}' does not contain a matching '{1}' method that has {2} parameter(s).", new string[]
				{
					this.namespaceUri,
					this.name,
					this.numArgs.ToString(CultureInfo.InvariantCulture)
				});
			}
			if (methodInfo.IsGenericMethodDefinition)
			{
				throw new XslTransformException("Method '{1}' of extension object '{0}' cannot be called because it is generic.", new string[] { this.namespaceUri, this.name });
			}
			this.Bind(methodInfo);
		}

		private void Bind(MethodInfo meth)
		{
			ParameterInfo[] parameters = meth.GetParameters();
			this.meth = meth;
			this.argClrTypes = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				this.argClrTypes[i] = this.GetClrType(parameters[i].ParameterType);
			}
			this.retClrType = this.GetClrType(this.meth.ReturnType);
			this.argXmlTypes = new XmlQueryType[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				this.argXmlTypes[i] = this.InferXmlType(this.argClrTypes[i]);
				if (this.namespaceUri.Length == 0)
				{
					if (this.argXmlTypes[i] == XmlQueryTypeFactory.NodeNotRtf)
					{
						this.argXmlTypes[i] = XmlQueryTypeFactory.Node;
					}
					else if (this.argXmlTypes[i] == XmlQueryTypeFactory.NodeSDod)
					{
						this.argXmlTypes[i] = XmlQueryTypeFactory.NodeS;
					}
				}
				else if (this.argXmlTypes[i] == XmlQueryTypeFactory.NodeSDod)
				{
					this.argXmlTypes[i] = XmlQueryTypeFactory.NodeNotRtfS;
				}
			}
			this.retXmlType = this.InferXmlType(this.retClrType);
		}

		public object Invoke(object extObj, object[] args)
		{
			object obj;
			try
			{
				obj = this.meth.Invoke(extObj, this.flags, null, args, CultureInfo.InvariantCulture);
			}
			catch (TargetInvocationException ex)
			{
				throw new XslTransformException(ex.InnerException, "An error occurred during a call to extension function '{0}'. See InnerException for a complete description of the error.", new string[] { this.name });
			}
			catch (Exception ex2)
			{
				if (!XmlException.IsCatchableException(ex2))
				{
					throw;
				}
				throw new XslTransformException(ex2, "An error occurred during a call to extension function '{0}'. See InnerException for a complete description of the error.", new string[] { this.name });
			}
			return obj;
		}

		public override bool Equals(object other)
		{
			XmlExtensionFunction xmlExtensionFunction = other as XmlExtensionFunction;
			return this.hashCode == xmlExtensionFunction.hashCode && this.name == xmlExtensionFunction.name && this.namespaceUri == xmlExtensionFunction.namespaceUri && this.numArgs == xmlExtensionFunction.numArgs && this.objectType == xmlExtensionFunction.objectType && this.flags == xmlExtensionFunction.flags;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		private Type GetClrType(Type clrType)
		{
			if (clrType.IsEnum)
			{
				return Enum.GetUnderlyingType(clrType);
			}
			if (clrType.IsByRef)
			{
				throw new XslTransformException("Method '{1}' of extension object '{0}' cannot be called because it has one or more ByRef parameters.", new string[] { this.namespaceUri, this.name });
			}
			return clrType;
		}

		private XmlQueryType InferXmlType(Type clrType)
		{
			return XsltConvert.InferXsltType(clrType);
		}

		private string namespaceUri;

		private string name;

		private int numArgs;

		private Type objectType;

		private BindingFlags flags;

		private int hashCode;

		private MethodInfo meth;

		private Type[] argClrTypes;

		private Type retClrType;

		private XmlQueryType[] argXmlTypes;

		private XmlQueryType retXmlType;
	}
}
