using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.IlGen
{
	internal class XmlILModule
	{
		static XmlILModule()
		{
			XmlILModule.CreateModulePermissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
			XmlILModule.CreateModulePermissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.ControlEvidence));
			XmlILModule.AssemblyId = 0L;
			AssemblyName assemblyName = XmlILModule.CreateAssemblyName();
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			try
			{
				XmlILModule.CreateModulePermissionSet.Assert();
				assemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.Transparent, new object[0]));
				XmlILModule.LREModule = assemblyBuilder.DefineDynamicModule("System.Xml.Xsl.CompiledQuery", false);
			}
			finally
			{
				CodeAccessPermission.RevertAssert();
			}
		}

		public XmlILModule(TypeBuilder typeBldr)
		{
			this.typeBldr = typeBldr;
			this.emitSymbols = ((ModuleBuilder)this.typeBldr.Module).GetSymWriter() != null;
			this.useLRE = false;
			this.persistAsm = false;
			this.methods = new Hashtable();
			if (this.emitSymbols)
			{
				this.urlToSymWriter = new Hashtable();
			}
		}

		public bool EmitSymbols
		{
			get
			{
				return this.emitSymbols;
			}
		}

		public XmlILModule(bool useLRE, bool emitSymbols)
		{
			this.useLRE = useLRE;
			this.emitSymbols = emitSymbols;
			this.persistAsm = false;
			this.methods = new Hashtable();
			if (!useLRE)
			{
				AssemblyName assemblyName = XmlILModule.CreateAssemblyName();
				AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, this.persistAsm ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run);
				assemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.Transparent, new object[0]));
				if (emitSymbols)
				{
					this.urlToSymWriter = new Hashtable();
					DebuggableAttribute.DebuggingModes debuggingModes = DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints;
					assemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.Debuggable, new object[] { debuggingModes }));
				}
				ModuleBuilder moduleBuilder;
				if (this.persistAsm)
				{
					moduleBuilder = assemblyBuilder.DefineDynamicModule("System.Xml.Xsl.CompiledQuery", this.modFile + ".dll", emitSymbols);
				}
				else
				{
					moduleBuilder = assemblyBuilder.DefineDynamicModule("System.Xml.Xsl.CompiledQuery", emitSymbols);
				}
				this.typeBldr = moduleBuilder.DefineType("System.Xml.Xsl.CompiledQuery.Query", TypeAttributes.Public);
			}
		}

		public MethodInfo DefineMethod(string name, Type returnType, Type[] paramTypes, string[] paramNames, XmlILMethodAttributes xmlAttrs)
		{
			int num = 1;
			string text = name;
			bool flag = (xmlAttrs & XmlILMethodAttributes.Raw) > XmlILMethodAttributes.None;
			while (this.methods[name] != null)
			{
				num++;
				name = text + " (" + num.ToString() + ")";
			}
			if (!flag)
			{
				Type[] array = new Type[paramTypes.Length + 1];
				array[0] = typeof(XmlQueryRuntime);
				Array.Copy(paramTypes, 0, array, 1, paramTypes.Length);
				paramTypes = array;
			}
			MethodInfo methodInfo;
			if (!this.useLRE)
			{
				MethodBuilder methodBuilder = this.typeBldr.DefineMethod(name, MethodAttributes.Private | MethodAttributes.Static, returnType, paramTypes);
				if (this.emitSymbols && (xmlAttrs & XmlILMethodAttributes.NonUser) != XmlILMethodAttributes.None)
				{
					methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.StepThrough, new object[0]));
					methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.NonUserCode, new object[0]));
				}
				if (!flag)
				{
					methodBuilder.DefineParameter(1, ParameterAttributes.None, "{urn:schemas-microsoft-com:xslt-debug}runtime");
				}
				for (int i = 0; i < paramNames.Length; i++)
				{
					if (paramNames[i] != null && paramNames[i].Length != 0)
					{
						methodBuilder.DefineParameter(i + (flag ? 1 : 2), ParameterAttributes.None, paramNames[i]);
					}
				}
				methodInfo = methodBuilder;
			}
			else
			{
				DynamicMethod dynamicMethod = new DynamicMethod(name, returnType, paramTypes, XmlILModule.LREModule);
				dynamicMethod.InitLocals = true;
				if (!flag)
				{
					dynamicMethod.DefineParameter(1, ParameterAttributes.None, "{urn:schemas-microsoft-com:xslt-debug}runtime");
				}
				for (int j = 0; j < paramNames.Length; j++)
				{
					if (paramNames[j] != null && paramNames[j].Length != 0)
					{
						dynamicMethod.DefineParameter(j + (flag ? 1 : 2), ParameterAttributes.None, paramNames[j]);
					}
				}
				methodInfo = dynamicMethod;
			}
			this.methods[name] = methodInfo;
			return methodInfo;
		}

		public static ILGenerator DefineMethodBody(MethodBase methInfo)
		{
			DynamicMethod dynamicMethod = methInfo as DynamicMethod;
			if (dynamicMethod != null)
			{
				return dynamicMethod.GetILGenerator();
			}
			MethodBuilder methodBuilder = methInfo as MethodBuilder;
			if (methodBuilder != null)
			{
				return methodBuilder.GetILGenerator();
			}
			return ((ConstructorBuilder)methInfo).GetILGenerator();
		}

		public MethodInfo FindMethod(string name)
		{
			return (MethodInfo)this.methods[name];
		}

		public FieldInfo DefineInitializedData(string name, byte[] data)
		{
			return this.typeBldr.DefineInitializedData(name, data, FieldAttributes.Private | FieldAttributes.Static);
		}

		public FieldInfo DefineField(string fieldName, Type type)
		{
			return this.typeBldr.DefineField(fieldName, type, FieldAttributes.Private | FieldAttributes.Static);
		}

		public ConstructorInfo DefineTypeInitializer()
		{
			return this.typeBldr.DefineTypeInitializer();
		}

		public ISymbolDocumentWriter AddSourceDocument(string fileName)
		{
			ISymbolDocumentWriter symbolDocumentWriter = this.urlToSymWriter[fileName] as ISymbolDocumentWriter;
			if (symbolDocumentWriter == null)
			{
				symbolDocumentWriter = ((ModuleBuilder)this.typeBldr.Module).DefineDocument(fileName, XmlILModule.LanguageGuid, XmlILModule.VendorGuid, Guid.Empty);
				this.urlToSymWriter.Add(fileName, symbolDocumentWriter);
			}
			return symbolDocumentWriter;
		}

		public void BakeMethods()
		{
			if (!this.useLRE)
			{
				Type type = this.typeBldr.CreateType();
				if (this.persistAsm)
				{
					((AssemblyBuilder)this.typeBldr.Module.Assembly).Save(this.modFile + ".dll");
				}
				Hashtable hashtable = new Hashtable(this.methods.Count);
				foreach (object obj in this.methods.Keys)
				{
					string text = (string)obj;
					hashtable[text] = type.GetMethod(text, BindingFlags.Static | BindingFlags.NonPublic);
				}
				this.methods = hashtable;
				this.typeBldr = null;
				this.urlToSymWriter = null;
			}
		}

		public Delegate CreateDelegate(string name, Type typDelegate)
		{
			if (!this.useLRE)
			{
				return Delegate.CreateDelegate(typDelegate, (MethodInfo)this.methods[name]);
			}
			return ((DynamicMethod)this.methods[name]).CreateDelegate(typDelegate);
		}

		private static AssemblyName CreateAssemblyName()
		{
			Interlocked.Increment(ref XmlILModule.AssemblyId);
			return new AssemblyName
			{
				Name = "System.Xml.Xsl.CompiledQuery." + XmlILModule.AssemblyId.ToString()
			};
		}

		public static readonly PermissionSet CreateModulePermissionSet = new PermissionSet(PermissionState.None);

		private static long AssemblyId;

		private static ModuleBuilder LREModule;

		private TypeBuilder typeBldr;

		private Hashtable methods;

		private Hashtable urlToSymWriter;

		private string modFile;

		private bool persistAsm;

		private bool useLRE;

		private bool emitSymbols;

		private static readonly Guid LanguageGuid = new Guid(1177373246U, 45655, 19182, 151, 205, 89, 24, 199, 83, 23, 88);

		private static readonly Guid VendorGuid = new Guid(2571847108U, 59113, 4562, 144, 63, 0, 192, 79, 163, 2, 161);

		private const string RuntimeName = "{urn:schemas-microsoft-com:xslt-debug}runtime";
	}
}
