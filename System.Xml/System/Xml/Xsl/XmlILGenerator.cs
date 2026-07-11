using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using System.Xml.Xsl.IlGen;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl
{
	internal class XmlILGenerator
	{
		public XmlILCommand Generate(QilExpression query, TypeBuilder typeBldr)
		{
			this.qil = query;
			bool flag = !this.qil.IsDebug && typeBldr == null;
			bool isDebug = this.qil.IsDebug;
			this.optVisitor = new XmlILOptimizerVisitor(this.qil, !this.qil.IsDebug);
			this.qil = this.optVisitor.Optimize();
			XmlILModule.CreateModulePermissionSet.Assert();
			if (typeBldr != null)
			{
				this.module = new XmlILModule(typeBldr);
			}
			else
			{
				this.module = new XmlILModule(flag, isDebug);
			}
			this.helper = new GenerateHelper(this.module, this.qil.IsDebug);
			this.CreateHelperFunctions();
			MethodInfo methodInfo = this.module.DefineMethod("Execute", typeof(void), new Type[0], new string[0], XmlILMethodAttributes.NonUser);
			XmlILMethodAttributes xmlILMethodAttributes = ((this.qil.Root.SourceLine == null) ? XmlILMethodAttributes.NonUser : XmlILMethodAttributes.None);
			MethodInfo methodInfo2 = this.module.DefineMethod("Root", typeof(void), new Type[0], new string[0], xmlILMethodAttributes);
			foreach (EarlyBoundInfo earlyBoundInfo in this.qil.EarlyBoundTypes)
			{
				this.helper.StaticData.DeclareEarlyBound(earlyBoundInfo.NamespaceUri, earlyBoundInfo.EarlyBoundType);
			}
			this.CreateFunctionMetadata(this.qil.FunctionList);
			this.CreateGlobalValueMetadata(this.qil.GlobalVariableList);
			this.CreateGlobalValueMetadata(this.qil.GlobalParameterList);
			this.GenerateExecuteFunction(methodInfo, methodInfo2);
			this.xmlIlVisitor = new XmlILVisitor();
			this.xmlIlVisitor.Visit(this.qil, this.helper, methodInfo2);
			XmlQueryStaticData xmlQueryStaticData = new XmlQueryStaticData(this.qil.DefaultWriterSettings, this.qil.WhitespaceRules, this.helper.StaticData);
			if (typeBldr != null)
			{
				this.CreateTypeInitializer(xmlQueryStaticData);
				this.module.BakeMethods();
				return null;
			}
			this.module.BakeMethods();
			return new XmlILCommand((ExecuteDelegate)this.module.CreateDelegate("Execute", typeof(ExecuteDelegate)), xmlQueryStaticData);
		}

		private void CreateFunctionMetadata(IList<QilNode> funcList)
		{
			foreach (QilNode qilNode in funcList)
			{
				QilFunction qilFunction = (QilFunction)qilNode;
				Type[] array = new Type[qilFunction.Arguments.Count];
				string[] array2 = new string[qilFunction.Arguments.Count];
				for (int i = 0; i < qilFunction.Arguments.Count; i++)
				{
					QilParameter qilParameter = (QilParameter)qilFunction.Arguments[i];
					array[i] = XmlILTypeHelper.GetStorageType(qilParameter.XmlType);
					if (qilParameter.DebugName != null)
					{
						array2[i] = qilParameter.DebugName;
					}
				}
				Type type;
				if (XmlILConstructInfo.Read(qilFunction).PushToWriterLast)
				{
					type = typeof(void);
				}
				else
				{
					type = XmlILTypeHelper.GetStorageType(qilFunction.XmlType);
				}
				XmlILMethodAttributes xmlILMethodAttributes = ((qilFunction.SourceLine == null) ? XmlILMethodAttributes.NonUser : XmlILMethodAttributes.None);
				MethodInfo methodInfo = this.module.DefineMethod(qilFunction.DebugName, type, array, array2, xmlILMethodAttributes);
				for (int j = 0; j < qilFunction.Arguments.Count; j++)
				{
					XmlILAnnotation.Write(qilFunction.Arguments[j]).ArgumentPosition = j;
				}
				XmlILAnnotation.Write(qilFunction).FunctionBinding = methodInfo;
			}
		}

		private void CreateGlobalValueMetadata(IList<QilNode> globalList)
		{
			foreach (QilNode qilNode in globalList)
			{
				QilReference qilReference = (QilReference)qilNode;
				Type storageType = XmlILTypeHelper.GetStorageType(qilReference.XmlType);
				XmlILMethodAttributes xmlILMethodAttributes = ((qilReference.SourceLine == null) ? XmlILMethodAttributes.NonUser : XmlILMethodAttributes.None);
				MethodInfo methodInfo = this.module.DefineMethod(qilReference.DebugName.ToString(), storageType, new Type[0], new string[0], xmlILMethodAttributes);
				XmlILAnnotation.Write(qilReference).FunctionBinding = methodInfo;
			}
		}

		private MethodInfo GenerateExecuteFunction(MethodInfo methExec, MethodInfo methRoot)
		{
			this.helper.MethodBegin(methExec, null, false);
			this.EvaluateGlobalValues(this.qil.GlobalVariableList);
			this.EvaluateGlobalValues(this.qil.GlobalParameterList);
			this.helper.LoadQueryRuntime();
			this.helper.Call(methRoot);
			this.helper.MethodEnd();
			return methExec;
		}

		private void CreateHelperFunctions()
		{
			MethodInfo methodInfo = this.module.DefineMethod("SyncToNavigator", typeof(XPathNavigator), new Type[]
			{
				typeof(XPathNavigator),
				typeof(XPathNavigator)
			}, new string[2], (XmlILMethodAttributes)3);
			this.helper.MethodBegin(methodInfo, null, false);
			Label label = this.helper.DefineLabel();
			this.helper.Emit(OpCodes.Ldarg_0);
			this.helper.Emit(OpCodes.Brfalse, label);
			this.helper.Emit(OpCodes.Ldarg_0);
			this.helper.Emit(OpCodes.Ldarg_1);
			this.helper.Call(XmlILMethods.NavMoveTo);
			this.helper.Emit(OpCodes.Brfalse, label);
			this.helper.Emit(OpCodes.Ldarg_0);
			this.helper.Emit(OpCodes.Ret);
			this.helper.MarkLabel(label);
			this.helper.Emit(OpCodes.Ldarg_1);
			this.helper.Call(XmlILMethods.NavClone);
			this.helper.MethodEnd();
		}

		private void EvaluateGlobalValues(IList<QilNode> iterList)
		{
			foreach (QilNode qilNode in iterList)
			{
				QilIterator qilIterator = (QilIterator)qilNode;
				if (this.qil.IsDebug || OptimizerPatterns.Read(qilIterator).MatchesPattern(OptimizerPatternName.MaybeSideEffects))
				{
					MethodInfo functionBinding = XmlILAnnotation.Write(qilIterator).FunctionBinding;
					this.helper.LoadQueryRuntime();
					this.helper.Call(functionBinding);
					this.helper.Emit(OpCodes.Pop);
				}
			}
		}

		public void CreateTypeInitializer(XmlQueryStaticData staticData)
		{
			byte[] array;
			Type[] array2;
			staticData.GetObjectData(out array, out array2);
			FieldInfo fieldInfo = this.module.DefineInitializedData("__staticData", array);
			FieldInfo fieldInfo2 = this.module.DefineField("staticData", typeof(object));
			FieldInfo fieldInfo3 = this.module.DefineField("ebTypes", typeof(Type[]));
			ConstructorInfo constructorInfo = this.module.DefineTypeInitializer();
			this.helper.MethodBegin(constructorInfo, null, false);
			this.helper.LoadInteger(array.Length);
			this.helper.Emit(OpCodes.Newarr, typeof(byte));
			this.helper.Emit(OpCodes.Dup);
			this.helper.Emit(OpCodes.Ldtoken, fieldInfo);
			this.helper.Call(XmlILMethods.InitializeArray);
			this.helper.Emit(OpCodes.Stsfld, fieldInfo2);
			if (array2 != null)
			{
				LocalBuilder localBuilder = this.helper.DeclareLocal("$$$types", typeof(Type[]));
				this.helper.LoadInteger(array2.Length);
				this.helper.Emit(OpCodes.Newarr, typeof(Type));
				this.helper.Emit(OpCodes.Stloc, localBuilder);
				for (int i = 0; i < array2.Length; i++)
				{
					this.helper.Emit(OpCodes.Ldloc, localBuilder);
					this.helper.LoadInteger(i);
					this.helper.LoadType(array2[i]);
					this.helper.Emit(OpCodes.Stelem_Ref);
				}
				this.helper.Emit(OpCodes.Ldloc, localBuilder);
				this.helper.Emit(OpCodes.Stsfld, fieldInfo3);
			}
			this.helper.MethodEnd();
		}

		private QilExpression qil;

		private GenerateHelper helper;

		private XmlILOptimizerVisitor optVisitor;

		private XmlILVisitor xmlIlVisitor;

		private XmlILModule module;
	}
}
