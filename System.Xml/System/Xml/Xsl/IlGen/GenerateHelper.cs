using System;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.IlGen
{
	internal class GenerateHelper
	{
		public GenerateHelper(XmlILModule module, bool isDebug)
		{
			this.isDebug = isDebug;
			this.module = module;
			this.staticData = new StaticDataManager();
		}

		public void MethodBegin(MethodBase methInfo, ISourceLineInfo sourceInfo, bool initWriters)
		{
			this.methInfo = methInfo;
			this.ilgen = XmlILModule.DefineMethodBody(methInfo);
			this.lastSourceInfo = null;
			if (this.isDebug)
			{
				this.DebugStartScope();
				if (sourceInfo != null)
				{
					this.MarkSequencePoint(sourceInfo);
					this.Emit(OpCodes.Nop);
				}
			}
			else if (this.module.EmitSymbols && sourceInfo != null)
			{
				this.MarkSequencePoint(sourceInfo);
				this.lastSourceInfo = null;
			}
			this.initWriters = false;
			if (initWriters)
			{
				this.EnsureWriter();
				this.LoadQueryRuntime();
				this.Call(XmlILMethods.GetOutput);
				this.Emit(OpCodes.Stloc, this.locXOut);
			}
		}

		public void MethodEnd()
		{
			this.Emit(OpCodes.Ret);
			if (this.isDebug)
			{
				this.DebugEndScope();
			}
		}

		public void CallSyncToNavigator()
		{
			if (this.methSyncToNav == null)
			{
				this.methSyncToNav = this.module.FindMethod("SyncToNavigator");
			}
			this.Call(this.methSyncToNav);
		}

		public StaticDataManager StaticData
		{
			get
			{
				return this.staticData;
			}
		}

		public void LoadInteger(int intVal)
		{
			if (intVal >= -1 && intVal < 9)
			{
				OpCode opCode;
				switch (intVal)
				{
				case -1:
					opCode = OpCodes.Ldc_I4_M1;
					break;
				case 0:
					opCode = OpCodes.Ldc_I4_0;
					break;
				case 1:
					opCode = OpCodes.Ldc_I4_1;
					break;
				case 2:
					opCode = OpCodes.Ldc_I4_2;
					break;
				case 3:
					opCode = OpCodes.Ldc_I4_3;
					break;
				case 4:
					opCode = OpCodes.Ldc_I4_4;
					break;
				case 5:
					opCode = OpCodes.Ldc_I4_5;
					break;
				case 6:
					opCode = OpCodes.Ldc_I4_6;
					break;
				case 7:
					opCode = OpCodes.Ldc_I4_7;
					break;
				case 8:
					opCode = OpCodes.Ldc_I4_8;
					break;
				default:
					return;
				}
				this.Emit(opCode);
				return;
			}
			if (intVal >= -128 && intVal <= 127)
			{
				this.Emit(OpCodes.Ldc_I4_S, (sbyte)intVal);
				return;
			}
			this.Emit(OpCodes.Ldc_I4, intVal);
		}

		public void LoadBoolean(bool boolVal)
		{
			this.Emit(boolVal ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
		}

		public void LoadType(Type clrTyp)
		{
			this.Emit(OpCodes.Ldtoken, clrTyp);
			this.Call(XmlILMethods.GetTypeFromHandle);
		}

		public LocalBuilder DeclareLocal(string name, Type type)
		{
			return this.ilgen.DeclareLocal(type);
		}

		public void LoadQueryRuntime()
		{
			this.Emit(OpCodes.Ldarg_0);
		}

		public void LoadQueryContext()
		{
			this.Emit(OpCodes.Ldarg_0);
			this.Call(XmlILMethods.Context);
		}

		public void LoadXsltLibrary()
		{
			this.Emit(OpCodes.Ldarg_0);
			this.Call(XmlILMethods.XsltLib);
		}

		public void LoadQueryOutput()
		{
			this.Emit(OpCodes.Ldloc, this.locXOut);
		}

		public void LoadParameter(int paramPos)
		{
			switch (paramPos)
			{
			case 0:
				this.Emit(OpCodes.Ldarg_0);
				return;
			case 1:
				this.Emit(OpCodes.Ldarg_1);
				return;
			case 2:
				this.Emit(OpCodes.Ldarg_2);
				return;
			case 3:
				this.Emit(OpCodes.Ldarg_3);
				return;
			default:
				if (paramPos <= 255)
				{
					this.Emit(OpCodes.Ldarg_S, (byte)paramPos);
					return;
				}
				if (paramPos <= 65535)
				{
					this.Emit(OpCodes.Ldarg, paramPos);
					return;
				}
				throw new XslTransformException("Functions may not have more than 65535 parameters.");
			}
		}

		public void SetParameter(object paramId)
		{
			int num = (int)paramId;
			if (num <= 255)
			{
				this.Emit(OpCodes.Starg_S, (byte)num);
				return;
			}
			if (num <= 65535)
			{
				this.Emit(OpCodes.Starg, num);
				return;
			}
			throw new XslTransformException("Functions may not have more than 65535 parameters.");
		}

		public void BranchAndMark(Label lblBranch, Label lblMark)
		{
			if (!lblBranch.Equals(lblMark))
			{
				this.EmitUnconditionalBranch(OpCodes.Br, lblBranch);
			}
			this.MarkLabel(lblMark);
		}

		public void TestAndBranch(int i4, Label lblBranch, OpCode opcodeBranch)
		{
			if (i4 == 0)
			{
				if (opcodeBranch.Value == OpCodes.Beq.Value)
				{
					opcodeBranch = OpCodes.Brfalse;
					goto IL_0086;
				}
				if (opcodeBranch.Value == OpCodes.Beq_S.Value)
				{
					opcodeBranch = OpCodes.Brfalse_S;
					goto IL_0086;
				}
				if (opcodeBranch.Value == OpCodes.Bne_Un.Value)
				{
					opcodeBranch = OpCodes.Brtrue;
					goto IL_0086;
				}
				if (opcodeBranch.Value == OpCodes.Bne_Un_S.Value)
				{
					opcodeBranch = OpCodes.Brtrue_S;
					goto IL_0086;
				}
			}
			this.LoadInteger(i4);
			IL_0086:
			this.Emit(opcodeBranch, lblBranch);
		}

		public void ConvBranchToBool(Label lblBranch, bool isTrueBranch)
		{
			Label label = this.DefineLabel();
			this.Emit(isTrueBranch ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
			this.EmitUnconditionalBranch(OpCodes.Br_S, label);
			this.MarkLabel(lblBranch);
			this.Emit(isTrueBranch ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
			this.MarkLabel(label);
		}

		public void TailCall(MethodInfo meth)
		{
			this.Emit(OpCodes.Tailcall);
			this.Call(meth);
			this.Emit(OpCodes.Ret);
		}

		[Conditional("DEBUG")]
		private void TraceCall(OpCode opcode, MethodInfo meth)
		{
		}

		public void Call(MethodInfo meth)
		{
			OpCode opCode = ((meth.IsVirtual || meth.IsAbstract) ? OpCodes.Callvirt : OpCodes.Call);
			this.ilgen.Emit(opCode, meth);
			if (this.lastSourceInfo != null)
			{
				this.MarkSequencePoint(SourceLineInfo.NoSource);
			}
		}

		public void CallToken(MethodInfo meth)
		{
			MethodBuilder methodBuilder = this.methInfo as MethodBuilder;
			if (methodBuilder != null)
			{
				OpCode opCode = ((meth.IsVirtual || meth.IsAbstract) ? OpCodes.Callvirt : OpCodes.Call);
				this.ilgen.Emit(opCode, ((ModuleBuilder)methodBuilder.GetModule()).GetMethodToken(meth).Token);
				if (this.lastSourceInfo != null)
				{
					this.MarkSequencePoint(SourceLineInfo.NoSource);
					return;
				}
			}
			else
			{
				this.Call(meth);
			}
		}

		public void Construct(ConstructorInfo constr)
		{
			this.Emit(OpCodes.Newobj, constr);
		}

		public void CallConcatStrings(int cStrings)
		{
			switch (cStrings)
			{
			case 0:
				this.Emit(OpCodes.Ldstr, "");
				return;
			case 1:
				break;
			case 2:
				this.Call(XmlILMethods.StrCat2);
				return;
			case 3:
				this.Call(XmlILMethods.StrCat3);
				return;
			case 4:
				this.Call(XmlILMethods.StrCat4);
				break;
			default:
				return;
			}
		}

		public void TreatAs(Type clrTypeSrc, Type clrTypeDst)
		{
			if (clrTypeSrc == clrTypeDst)
			{
				return;
			}
			if (clrTypeSrc.IsValueType)
			{
				this.Emit(OpCodes.Box, clrTypeSrc);
				return;
			}
			if (clrTypeDst.IsValueType)
			{
				this.Emit(OpCodes.Unbox, clrTypeDst);
				this.Emit(OpCodes.Ldobj, clrTypeDst);
				return;
			}
			if (clrTypeDst != typeof(object))
			{
				this.Emit(OpCodes.Castclass, clrTypeDst);
			}
		}

		public void ConstructLiteralDecimal(decimal dec)
		{
			if (dec >= -2147483648m && dec <= 2147483647m && decimal.Truncate(dec) == dec)
			{
				this.LoadInteger((int)dec);
				this.Construct(XmlILConstructors.DecFromInt32);
				return;
			}
			int[] bits = decimal.GetBits(dec);
			this.LoadInteger(bits[0]);
			this.LoadInteger(bits[1]);
			this.LoadInteger(bits[2]);
			this.LoadBoolean(bits[3] < 0);
			this.LoadInteger(bits[3] >> 16);
			this.Construct(XmlILConstructors.DecFromParts);
		}

		public void ConstructLiteralQName(string localName, string namespaceName)
		{
			this.Emit(OpCodes.Ldstr, localName);
			this.Emit(OpCodes.Ldstr, namespaceName);
			this.Construct(XmlILConstructors.QName);
		}

		public void CallArithmeticOp(QilNodeType opType, XmlTypeCode code)
		{
			MethodInfo methodInfo = null;
			if (code <= XmlTypeCode.Double)
			{
				if (code == XmlTypeCode.Decimal)
				{
					switch (opType)
					{
					case QilNodeType.Negate:
						methodInfo = XmlILMethods.DecNeg;
						break;
					case QilNodeType.Add:
						methodInfo = XmlILMethods.DecAdd;
						break;
					case QilNodeType.Subtract:
						methodInfo = XmlILMethods.DecSub;
						break;
					case QilNodeType.Multiply:
						methodInfo = XmlILMethods.DecMul;
						break;
					case QilNodeType.Divide:
						methodInfo = XmlILMethods.DecDiv;
						break;
					case QilNodeType.Modulo:
						methodInfo = XmlILMethods.DecRem;
						break;
					}
					this.Call(methodInfo);
					return;
				}
				if (code - XmlTypeCode.Float > 1)
				{
					return;
				}
			}
			else if (code != XmlTypeCode.Integer && code != XmlTypeCode.Int)
			{
				return;
			}
			switch (opType)
			{
			case QilNodeType.Negate:
				this.Emit(OpCodes.Neg);
				return;
			case QilNodeType.Add:
				this.Emit(OpCodes.Add);
				return;
			case QilNodeType.Subtract:
				this.Emit(OpCodes.Sub);
				return;
			case QilNodeType.Multiply:
				this.Emit(OpCodes.Mul);
				return;
			case QilNodeType.Divide:
				this.Emit(OpCodes.Div);
				return;
			case QilNodeType.Modulo:
				this.Emit(OpCodes.Rem);
				return;
			default:
				return;
			}
		}

		public void CallCompareEquals(XmlTypeCode code)
		{
			MethodInfo methodInfo = null;
			if (code != XmlTypeCode.String)
			{
				if (code != XmlTypeCode.Decimal)
				{
					if (code == XmlTypeCode.QName)
					{
						methodInfo = XmlILMethods.QNameEq;
					}
				}
				else
				{
					methodInfo = XmlILMethods.DecEq;
				}
			}
			else
			{
				methodInfo = XmlILMethods.StrEq;
			}
			this.Call(methodInfo);
		}

		public void CallCompare(XmlTypeCode code)
		{
			MethodInfo methodInfo = null;
			if (code != XmlTypeCode.String)
			{
				if (code == XmlTypeCode.Decimal)
				{
					methodInfo = XmlILMethods.DecCmp;
				}
			}
			else
			{
				methodInfo = XmlILMethods.StrCmp;
			}
			this.Call(methodInfo);
		}

		public void CallStartRtfConstruction(string baseUri)
		{
			this.EnsureWriter();
			this.LoadQueryRuntime();
			this.Emit(OpCodes.Ldstr, baseUri);
			this.Emit(OpCodes.Ldloca, this.locXOut);
			this.Call(XmlILMethods.StartRtfConstr);
		}

		public void CallEndRtfConstruction()
		{
			this.LoadQueryRuntime();
			this.Emit(OpCodes.Ldloca, this.locXOut);
			this.Call(XmlILMethods.EndRtfConstr);
		}

		public void CallStartSequenceConstruction()
		{
			this.EnsureWriter();
			this.LoadQueryRuntime();
			this.Emit(OpCodes.Ldloca, this.locXOut);
			this.Call(XmlILMethods.StartSeqConstr);
		}

		public void CallEndSequenceConstruction()
		{
			this.LoadQueryRuntime();
			this.Emit(OpCodes.Ldloca, this.locXOut);
			this.Call(XmlILMethods.EndSeqConstr);
		}

		public void CallGetEarlyBoundObject(int idxObj, Type clrType)
		{
			this.LoadQueryRuntime();
			this.LoadInteger(idxObj);
			this.Call(XmlILMethods.GetEarly);
			this.TreatAs(typeof(object), clrType);
		}

		public void CallGetAtomizedName(int idxName)
		{
			this.LoadQueryRuntime();
			this.LoadInteger(idxName);
			this.Call(XmlILMethods.GetAtomizedName);
		}

		public void CallGetNameFilter(int idxFilter)
		{
			this.LoadQueryRuntime();
			this.LoadInteger(idxFilter);
			this.Call(XmlILMethods.GetNameFilter);
		}

		public void CallGetTypeFilter(XPathNodeType nodeType)
		{
			this.LoadQueryRuntime();
			this.LoadInteger((int)nodeType);
			this.Call(XmlILMethods.GetTypeFilter);
		}

		public void CallParseTagName(GenerateNameType nameType)
		{
			if (nameType == GenerateNameType.TagNameAndMappings)
			{
				this.Call(XmlILMethods.TagAndMappings);
				return;
			}
			this.Call(XmlILMethods.TagAndNamespace);
		}

		public void CallGetGlobalValue(int idxValue, Type clrType)
		{
			this.LoadQueryRuntime();
			this.LoadInteger(idxValue);
			this.Call(XmlILMethods.GetGlobalValue);
			this.TreatAs(typeof(object), clrType);
		}

		public void CallSetGlobalValue(Type clrType)
		{
			this.TreatAs(clrType, typeof(object));
			this.Call(XmlILMethods.SetGlobalValue);
		}

		public void CallGetCollation(int idxName)
		{
			this.LoadQueryRuntime();
			this.LoadInteger(idxName);
			this.Call(XmlILMethods.GetCollation);
		}

		private void EnsureWriter()
		{
			if (!this.initWriters)
			{
				this.locXOut = this.DeclareLocal("$$$xwrtChk", typeof(XmlQueryOutput));
				this.initWriters = true;
			}
		}

		public void CallGetParameter(string localName, string namespaceUri)
		{
			this.LoadQueryContext();
			this.Emit(OpCodes.Ldstr, localName);
			this.Emit(OpCodes.Ldstr, namespaceUri);
			this.Call(XmlILMethods.GetParam);
		}

		public void CallStartTree(XPathNodeType rootType)
		{
			this.LoadQueryOutput();
			this.LoadInteger((int)rootType);
			this.Call(XmlILMethods.StartTree);
		}

		public void CallEndTree()
		{
			this.LoadQueryOutput();
			this.Call(XmlILMethods.EndTree);
		}

		public void CallWriteStartRoot()
		{
			this.LoadQueryOutput();
			this.Call(XmlILMethods.StartRoot);
		}

		public void CallWriteEndRoot()
		{
			this.LoadQueryOutput();
			this.Call(XmlILMethods.EndRoot);
		}

		public void CallWriteStartElement(GenerateNameType nameType, bool callChk)
		{
			MethodInfo methodInfo = null;
			if (callChk)
			{
				switch (nameType)
				{
				case GenerateNameType.LiteralLocalName:
					methodInfo = XmlILMethods.StartElemLocName;
					break;
				case GenerateNameType.LiteralName:
					methodInfo = XmlILMethods.StartElemLitName;
					break;
				case GenerateNameType.CopiedName:
					methodInfo = XmlILMethods.StartElemCopyName;
					break;
				case GenerateNameType.TagNameAndMappings:
					methodInfo = XmlILMethods.StartElemMapName;
					break;
				case GenerateNameType.TagNameAndNamespace:
					methodInfo = XmlILMethods.StartElemNmspName;
					break;
				case GenerateNameType.QName:
					methodInfo = XmlILMethods.StartElemQName;
					break;
				}
			}
			else if (nameType != GenerateNameType.LiteralLocalName)
			{
				if (nameType == GenerateNameType.LiteralName)
				{
					methodInfo = XmlILMethods.StartElemLitNameUn;
				}
			}
			else
			{
				methodInfo = XmlILMethods.StartElemLocNameUn;
			}
			this.Call(methodInfo);
		}

		public void CallWriteEndElement(GenerateNameType nameType, bool callChk)
		{
			MethodInfo methodInfo = null;
			if (callChk)
			{
				methodInfo = XmlILMethods.EndElemStackName;
			}
			else if (nameType != GenerateNameType.LiteralLocalName)
			{
				if (nameType == GenerateNameType.LiteralName)
				{
					methodInfo = XmlILMethods.EndElemLitNameUn;
				}
			}
			else
			{
				methodInfo = XmlILMethods.EndElemLocNameUn;
			}
			this.Call(methodInfo);
		}

		public void CallStartElementContent()
		{
			this.LoadQueryOutput();
			this.Call(XmlILMethods.StartContentUn);
		}

		public void CallWriteStartAttribute(GenerateNameType nameType, bool callChk)
		{
			MethodInfo methodInfo = null;
			if (callChk)
			{
				switch (nameType)
				{
				case GenerateNameType.LiteralLocalName:
					methodInfo = XmlILMethods.StartAttrLocName;
					break;
				case GenerateNameType.LiteralName:
					methodInfo = XmlILMethods.StartAttrLitName;
					break;
				case GenerateNameType.CopiedName:
					methodInfo = XmlILMethods.StartAttrCopyName;
					break;
				case GenerateNameType.TagNameAndMappings:
					methodInfo = XmlILMethods.StartAttrMapName;
					break;
				case GenerateNameType.TagNameAndNamespace:
					methodInfo = XmlILMethods.StartAttrNmspName;
					break;
				case GenerateNameType.QName:
					methodInfo = XmlILMethods.StartAttrQName;
					break;
				}
			}
			else if (nameType != GenerateNameType.LiteralLocalName)
			{
				if (nameType == GenerateNameType.LiteralName)
				{
					methodInfo = XmlILMethods.StartAttrLitNameUn;
				}
			}
			else
			{
				methodInfo = XmlILMethods.StartAttrLocNameUn;
			}
			this.Call(methodInfo);
		}

		public void CallWriteEndAttribute(bool callChk)
		{
			this.LoadQueryOutput();
			if (callChk)
			{
				this.Call(XmlILMethods.EndAttr);
				return;
			}
			this.Call(XmlILMethods.EndAttrUn);
		}

		public void CallWriteNamespaceDecl(bool callChk)
		{
			if (callChk)
			{
				this.Call(XmlILMethods.NamespaceDecl);
				return;
			}
			this.Call(XmlILMethods.NamespaceDeclUn);
		}

		public void CallWriteString(bool disableOutputEscaping, bool callChk)
		{
			if (callChk)
			{
				if (disableOutputEscaping)
				{
					this.Call(XmlILMethods.NoEntText);
					return;
				}
				this.Call(XmlILMethods.Text);
				return;
			}
			else
			{
				if (disableOutputEscaping)
				{
					this.Call(XmlILMethods.NoEntTextUn);
					return;
				}
				this.Call(XmlILMethods.TextUn);
				return;
			}
		}

		public void CallWriteStartPI()
		{
			this.Call(XmlILMethods.StartPI);
		}

		public void CallWriteEndPI()
		{
			this.LoadQueryOutput();
			this.Call(XmlILMethods.EndPI);
		}

		public void CallWriteStartComment()
		{
			this.LoadQueryOutput();
			this.Call(XmlILMethods.StartComment);
		}

		public void CallWriteEndComment()
		{
			this.LoadQueryOutput();
			this.Call(XmlILMethods.EndComment);
		}

		public void CallCacheCount(Type itemStorageType)
		{
			XmlILStorageMethods xmlILStorageMethods = XmlILMethods.StorageMethods[itemStorageType];
			this.Call(xmlILStorageMethods.IListCount);
		}

		public void CallCacheItem(Type itemStorageType)
		{
			this.Call(XmlILMethods.StorageMethods[itemStorageType].IListItem);
		}

		public void CallValueAs(Type clrType)
		{
			MethodInfo valueAs = XmlILMethods.StorageMethods[clrType].ValueAs;
			if (valueAs == null)
			{
				this.LoadType(clrType);
				this.Emit(OpCodes.Ldnull);
				this.Call(XmlILMethods.ValueAsAny);
				this.TreatAs(typeof(object), clrType);
				return;
			}
			this.Call(valueAs);
		}

		public void AddSortKey(XmlQueryType keyType)
		{
			MethodInfo methodInfo = null;
			if (keyType == null)
			{
				methodInfo = XmlILMethods.SortKeyEmpty;
			}
			else
			{
				XmlTypeCode typeCode = keyType.TypeCode;
				if (typeCode <= XmlTypeCode.DateTime)
				{
					if (typeCode != XmlTypeCode.None)
					{
						switch (typeCode)
						{
						case XmlTypeCode.AnyAtomicType:
							return;
						case XmlTypeCode.String:
							methodInfo = XmlILMethods.SortKeyString;
							break;
						case XmlTypeCode.Boolean:
							methodInfo = XmlILMethods.SortKeyInt;
							break;
						case XmlTypeCode.Decimal:
							methodInfo = XmlILMethods.SortKeyDecimal;
							break;
						case XmlTypeCode.Double:
							methodInfo = XmlILMethods.SortKeyDouble;
							break;
						case XmlTypeCode.DateTime:
							methodInfo = XmlILMethods.SortKeyDateTime;
							break;
						}
					}
					else
					{
						this.Emit(OpCodes.Pop);
						methodInfo = XmlILMethods.SortKeyEmpty;
					}
				}
				else if (typeCode != XmlTypeCode.Integer)
				{
					if (typeCode == XmlTypeCode.Int)
					{
						methodInfo = XmlILMethods.SortKeyInt;
					}
				}
				else
				{
					methodInfo = XmlILMethods.SortKeyInteger;
				}
			}
			this.Call(methodInfo);
		}

		public void DebugStartScope()
		{
			this.ilgen.BeginScope();
		}

		public void DebugEndScope()
		{
			this.ilgen.EndScope();
		}

		public void DebugSequencePoint(ISourceLineInfo sourceInfo)
		{
			this.Emit(OpCodes.Nop);
			this.MarkSequencePoint(sourceInfo);
		}

		private string GetFileName(ISourceLineInfo sourceInfo)
		{
			string uri = sourceInfo.Uri;
			if (uri == this.lastUriString)
			{
				return this.lastFileName;
			}
			this.lastUriString = uri;
			this.lastFileName = SourceLineInfo.GetFileName(uri);
			return this.lastFileName;
		}

		private void MarkSequencePoint(ISourceLineInfo sourceInfo)
		{
			if (sourceInfo.IsNoSource && this.lastSourceInfo != null && this.lastSourceInfo.IsNoSource)
			{
				return;
			}
			string fileName = this.GetFileName(sourceInfo);
			ISymbolDocumentWriter symbolDocumentWriter = this.module.AddSourceDocument(fileName);
			this.ilgen.MarkSequencePoint(symbolDocumentWriter, sourceInfo.Start.Line, sourceInfo.Start.Pos, sourceInfo.End.Line, sourceInfo.End.Pos);
			this.lastSourceInfo = sourceInfo;
		}

		public Label DefineLabel()
		{
			return this.ilgen.DefineLabel();
		}

		public void MarkLabel(Label lbl)
		{
			if (this.lastSourceInfo != null && !this.lastSourceInfo.IsNoSource)
			{
				this.DebugSequencePoint(SourceLineInfo.NoSource);
			}
			this.ilgen.MarkLabel(lbl);
		}

		public void Emit(OpCode opcode)
		{
			this.ilgen.Emit(opcode);
		}

		public void Emit(OpCode opcode, byte byteVal)
		{
			this.ilgen.Emit(opcode, byteVal);
		}

		public void Emit(OpCode opcode, ConstructorInfo constrInfo)
		{
			this.ilgen.Emit(opcode, constrInfo);
		}

		public void Emit(OpCode opcode, double dblVal)
		{
			this.ilgen.Emit(opcode, dblVal);
		}

		public void Emit(OpCode opcode, float fltVal)
		{
			this.ilgen.Emit(opcode, fltVal);
		}

		public void Emit(OpCode opcode, FieldInfo fldInfo)
		{
			this.ilgen.Emit(opcode, fldInfo);
		}

		public void Emit(OpCode opcode, short shrtVal)
		{
			this.ilgen.Emit(opcode, shrtVal);
		}

		public void Emit(OpCode opcode, int intVal)
		{
			this.ilgen.Emit(opcode, intVal);
		}

		public void Emit(OpCode opcode, long longVal)
		{
			this.ilgen.Emit(opcode, longVal);
		}

		public void Emit(OpCode opcode, Label lblVal)
		{
			this.ilgen.Emit(opcode, lblVal);
		}

		public void Emit(OpCode opcode, Label[] arrLabels)
		{
			this.ilgen.Emit(opcode, arrLabels);
		}

		public void Emit(OpCode opcode, LocalBuilder locBldr)
		{
			this.ilgen.Emit(opcode, locBldr);
		}

		public void Emit(OpCode opcode, MethodInfo methInfo)
		{
			this.ilgen.Emit(opcode, methInfo);
		}

		public void Emit(OpCode opcode, sbyte sbyteVal)
		{
			this.ilgen.Emit(opcode, sbyteVal);
		}

		public void Emit(OpCode opcode, string strVal)
		{
			this.ilgen.Emit(opcode, strVal);
		}

		public void Emit(OpCode opcode, Type typVal)
		{
			this.ilgen.Emit(opcode, typVal);
		}

		public void EmitUnconditionalBranch(OpCode opcode, Label lblTarget)
		{
			if (!opcode.Equals(OpCodes.Br) && !opcode.Equals(OpCodes.Br_S))
			{
				this.Emit(OpCodes.Ldc_I4_1);
			}
			this.ilgen.Emit(opcode, lblTarget);
			if (this.lastSourceInfo != null && (opcode.Equals(OpCodes.Br) || opcode.Equals(OpCodes.Br_S)))
			{
				this.MarkSequencePoint(SourceLineInfo.NoSource);
			}
		}

		private MethodBase methInfo;

		private ILGenerator ilgen;

		private LocalBuilder locXOut;

		private XmlILModule module;

		private bool isDebug;

		private bool initWriters;

		private StaticDataManager staticData;

		private ISourceLineInfo lastSourceInfo;

		private MethodInfo methSyncToNav;

		private string lastUriString;

		private string lastFileName;
	}
}
