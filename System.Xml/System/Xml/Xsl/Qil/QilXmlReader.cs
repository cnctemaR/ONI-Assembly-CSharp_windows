using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace System.Xml.Xsl.Qil
{
	internal sealed class QilXmlReader
	{
		static QilXmlReader()
		{
			foreach (MethodInfo methodInfo in typeof(QilFactory).GetMethods(BindingFlags.Instance | BindingFlags.Public))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				int num = 0;
				while (num < parameters.Length && !(parameters[num].ParameterType != typeof(QilNode)))
				{
					num++;
				}
				if (num == parameters.Length && (!QilXmlReader.nameToFactoryMethod.ContainsKey(methodInfo.Name) || QilXmlReader.nameToFactoryMethod[methodInfo.Name].GetParameters().Length < parameters.Length))
				{
					QilXmlReader.nameToFactoryMethod[methodInfo.Name] = methodInfo;
				}
			}
		}

		public QilXmlReader(XmlReader r)
		{
			this.r = r;
			this.f = new QilFactory();
		}

		public QilExpression Read()
		{
			this.stk = new Stack<QilList>();
			this.inFwdDecls = false;
			this.scope = new Dictionary<string, QilNode>();
			this.fwdDecls = new Dictionary<string, QilNode>();
			this.stk.Push(this.f.Sequence());
			while (this.r.Read())
			{
				XmlNodeType nodeType = this.r.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					switch (nodeType)
					{
					case XmlNodeType.EndElement:
						this.EndElement();
						break;
					}
				}
				else
				{
					bool isEmptyElement = this.r.IsEmptyElement;
					if (this.StartElement() && isEmptyElement)
					{
						this.EndElement();
					}
				}
			}
			return (QilExpression)this.stk.Peek()[0];
		}

		private bool StartElement()
		{
			QilXmlReader.ReaderAnnotation readerAnnotation = new QilXmlReader.ReaderAnnotation();
			string localName = this.r.LocalName;
			string localName2 = this.r.LocalName;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(localName2);
			QilNode qilNode;
			if (num <= 1748879461U)
			{
				if (num <= 394409068U)
				{
					if (num != 153024511U)
					{
						if (num != 259922736U)
						{
							if (num != 394409068U)
							{
								goto IL_0343;
							}
							if (!(localName2 == "LiteralQName"))
							{
								goto IL_0343;
							}
							qilNode = this.ParseName(this.r.GetAttribute("name"));
							goto IL_034F;
						}
						else if (!(localName2 == "For"))
						{
							goto IL_0343;
						}
					}
					else
					{
						if (!(localName2 == "LiteralInt64"))
						{
							goto IL_0343;
						}
						qilNode = this.f.LiteralInt64(long.Parse(this.ReadText(), CultureInfo.InvariantCulture));
						goto IL_034F;
					}
				}
				else if (num <= 459379415U)
				{
					if (num != 407419824U)
					{
						if (num != 459379415U)
						{
							goto IL_0343;
						}
						if (!(localName2 == "RefTo"))
						{
							goto IL_0343;
						}
					}
					else if (!(localName2 == "Parameter"))
					{
						goto IL_0343;
					}
				}
				else if (num != 1062209817U)
				{
					if (num != 1748879461U)
					{
						goto IL_0343;
					}
					if (!(localName2 == "LiteralString"))
					{
						goto IL_0343;
					}
					qilNode = this.f.LiteralString(this.ReadText());
					goto IL_034F;
				}
				else
				{
					if (!(localName2 == "LiteralDouble"))
					{
						goto IL_0343;
					}
					qilNode = this.f.LiteralDouble(double.Parse(this.ReadText(), CultureInfo.InvariantCulture));
					goto IL_034F;
				}
			}
			else if (num <= 2561120873U)
			{
				if (num != 2122509939U)
				{
					if (num != 2502332456U)
					{
						if (num != 2561120873U)
						{
							goto IL_0343;
						}
						if (!(localName2 == "Function"))
						{
							goto IL_0343;
						}
					}
					else
					{
						if (!(localName2 == "LiteralInt32"))
						{
							goto IL_0343;
						}
						qilNode = this.f.LiteralInt32(int.Parse(this.ReadText(), CultureInfo.InvariantCulture));
						goto IL_034F;
					}
				}
				else
				{
					if (!(localName2 == "ForwardDecls"))
					{
						goto IL_0343;
					}
					this.inFwdDecls = true;
					goto IL_0343;
				}
			}
			else if (num <= 2914621356U)
			{
				if (num != 2863549267U)
				{
					if (num != 2914621356U)
					{
						goto IL_0343;
					}
					if (!(localName2 == "LiteralType"))
					{
						goto IL_0343;
					}
					qilNode = this.f.LiteralType(this.ParseType(this.ReadText()));
					goto IL_034F;
				}
				else
				{
					if (!(localName2 == "LiteralDecimal"))
					{
						goto IL_0343;
					}
					qilNode = this.f.LiteralDecimal(decimal.Parse(this.ReadText(), CultureInfo.InvariantCulture));
					goto IL_034F;
				}
			}
			else if (num != 3002440602U)
			{
				if (num != 3961309433U)
				{
					goto IL_0343;
				}
				if (!(localName2 == "XsltInvokeEarlyBound"))
				{
					goto IL_0343;
				}
				readerAnnotation.ClrNamespace = this.r.GetAttribute("clrNamespace");
				goto IL_0343;
			}
			else if (!(localName2 == "Let"))
			{
				goto IL_0343;
			}
			readerAnnotation.Id = this.r.GetAttribute("id");
			readerAnnotation.Name = this.ParseName(this.r.GetAttribute("name"));
			IL_0343:
			qilNode = this.f.Sequence();
			IL_034F:
			readerAnnotation.XmlType = this.ParseType(this.r.GetAttribute("xmlType"));
			qilNode.SourceLine = this.ParseLineInfo(this.r.GetAttribute("lineInfo"));
			qilNode.Annotation = readerAnnotation;
			if (qilNode is QilList)
			{
				this.stk.Push((QilList)qilNode);
				return true;
			}
			this.stk.Peek().Add(qilNode);
			return false;
		}

		private void EndElement()
		{
			QilList qilList = this.stk.Pop();
			QilXmlReader.ReaderAnnotation readerAnnotation = (QilXmlReader.ReaderAnnotation)qilList.Annotation;
			string localName = this.r.LocalName;
			string text = this.r.LocalName;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			QilNode qilNode;
			if (num <= 1691271560U)
			{
				if (num <= 459379415U)
				{
					if (num <= 259922736U)
					{
						if (num != 73245973U)
						{
							if (num != 259922736U)
							{
								goto IL_06ED;
							}
							if (!(text == "For"))
							{
								goto IL_06ED;
							}
						}
						else
						{
							if (!(text == "FunctionList"))
							{
								goto IL_06ED;
							}
							qilNode = this.f.FunctionList(qilList);
							goto IL_0740;
						}
					}
					else if (num != 407419824U)
					{
						if (num != 459379415U)
						{
							goto IL_06ED;
						}
						if (!(text == "RefTo"))
						{
							goto IL_06ED;
						}
						string id = readerAnnotation.Id;
						this.stk.Peek().Add(this.scope[id]);
						return;
					}
					else if (!(text == "Parameter"))
					{
						goto IL_06ED;
					}
				}
				else if (num <= 1243829014U)
				{
					if (num != 1113995521U)
					{
						if (num != 1243829014U)
						{
							goto IL_06ED;
						}
						if (!(text == "GlobalVariableList"))
						{
							goto IL_06ED;
						}
						qilNode = this.f.GlobalVariableList(qilList);
						goto IL_0740;
					}
					else
					{
						if (!(text == "BranchList"))
						{
							goto IL_06ED;
						}
						qilNode = this.f.BranchList(qilList);
						goto IL_0740;
					}
				}
				else if (num != 1266645033U)
				{
					if (num != 1691271560U)
					{
						goto IL_06ED;
					}
					if (!(text == "Sequence"))
					{
						goto IL_06ED;
					}
					qilNode = this.f.Sequence(qilList);
					goto IL_0740;
				}
				else
				{
					if (!(text == "FormalParameterList"))
					{
						goto IL_06ED;
					}
					qilNode = this.f.FormalParameterList(qilList);
					goto IL_0740;
				}
			}
			else if (num <= 3002440602U)
			{
				if (num <= 2122509939U)
				{
					if (num != 1969361967U)
					{
						if (num != 2122509939U)
						{
							goto IL_06ED;
						}
						if (!(text == "ForwardDecls"))
						{
							goto IL_06ED;
						}
						this.inFwdDecls = false;
						return;
					}
					else
					{
						if (!(text == "QilExpression"))
						{
							goto IL_06ED;
						}
						QilExpression qilExpression = this.f.QilExpression(qilList[qilList.Count - 1]);
						for (int i = 0; i < qilList.Count - 1; i++)
						{
							QilNodeType nodeType = qilList[i].NodeType;
							switch (nodeType)
							{
							case QilNodeType.FunctionList:
								qilExpression.FunctionList = (QilList)qilList[i];
								break;
							case QilNodeType.GlobalVariableList:
								qilExpression.GlobalVariableList = (QilList)qilList[i];
								break;
							case QilNodeType.GlobalParameterList:
								qilExpression.GlobalParameterList = (QilList)qilList[i];
								break;
							default:
								if (nodeType - QilNodeType.True <= 1)
								{
									qilExpression.IsDebug = qilList[i].NodeType == QilNodeType.True;
								}
								break;
							}
						}
						qilNode = qilExpression;
						goto IL_0740;
					}
				}
				else if (num != 2561120873U)
				{
					if (num != 3002440602U)
					{
						goto IL_06ED;
					}
					if (!(text == "Let"))
					{
						goto IL_06ED;
					}
				}
				else if (!(text == "Function"))
				{
					goto IL_06ED;
				}
			}
			else if (num <= 3827601282U)
			{
				if (num != 3639411285U)
				{
					if (num != 3827601282U)
					{
						goto IL_06ED;
					}
					if (!(text == "ActualParameterList"))
					{
						goto IL_06ED;
					}
					qilNode = this.f.ActualParameterList(qilList);
					goto IL_0740;
				}
				else
				{
					if (!(text == "GlobalParameterList"))
					{
						goto IL_06ED;
					}
					qilNode = this.f.GlobalParameterList(qilList);
					goto IL_0740;
				}
			}
			else if (num != 3961309433U)
			{
				if (num != 4004652224U)
				{
					goto IL_06ED;
				}
				if (!(text == "SortKeyList"))
				{
					goto IL_06ED;
				}
				qilNode = this.f.SortKeyList(qilList);
				goto IL_0740;
			}
			else
			{
				if (!(text == "XsltInvokeEarlyBound"))
				{
					goto IL_06ED;
				}
				MethodInfo methodInfo = null;
				QilName qilName = (QilName)qilList[0];
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int j = 0; j < assemblies.Length; j++)
				{
					Type type = assemblies[j].GetType(readerAnnotation.ClrNamespace);
					if (type != null)
					{
						methodInfo = type.GetMethod(qilName.LocalName);
						break;
					}
				}
				qilNode = this.f.XsltInvokeEarlyBound(qilName, this.f.LiteralObject(methodInfo), qilList[1], readerAnnotation.XmlType);
				goto IL_0740;
			}
			string id2 = readerAnnotation.Id;
			QilName name = readerAnnotation.Name;
			text = this.r.LocalName;
			if (!(text == "Parameter"))
			{
				if (!(text == "Let"))
				{
					if (!(text == "For"))
					{
						if (this.inFwdDecls)
						{
							qilNode = this.f.Function(qilList[0], qilList[1], readerAnnotation.XmlType);
						}
						else
						{
							qilNode = this.f.Function(qilList[0], qilList[1], qilList[2], (readerAnnotation.XmlType != null) ? readerAnnotation.XmlType : qilList[1].XmlType);
						}
					}
					else
					{
						qilNode = this.f.For(qilList[0]);
					}
				}
				else if (this.inFwdDecls)
				{
					qilNode = this.f.Let(this.f.Unknown(readerAnnotation.XmlType));
				}
				else
				{
					qilNode = this.f.Let(qilList[0]);
				}
			}
			else if (this.inFwdDecls || qilList.Count == 0)
			{
				qilNode = this.f.Parameter(null, name, readerAnnotation.XmlType);
			}
			else
			{
				qilNode = this.f.Parameter(qilList[0], name, readerAnnotation.XmlType);
			}
			if (name != null)
			{
				((QilReference)qilNode).DebugName = name.ToString();
			}
			if (this.inFwdDecls)
			{
				this.fwdDecls[id2] = qilNode;
				this.scope[id2] = qilNode;
			}
			else if (this.fwdDecls.ContainsKey(id2))
			{
				qilNode = this.fwdDecls[id2];
				this.fwdDecls.Remove(id2);
				if (qilList.Count > 0)
				{
					qilNode[0] = qilList[0];
				}
				if (qilList.Count > 1)
				{
					qilNode[1] = qilList[1];
				}
			}
			else
			{
				this.scope[id2] = qilNode;
			}
			qilNode.Annotation = readerAnnotation;
			goto IL_0740;
			IL_06ED:
			MethodInfo methodInfo2 = QilXmlReader.nameToFactoryMethod[this.r.LocalName];
			object[] array = new object[qilList.Count];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = qilList[k];
			}
			qilNode = (QilNode)methodInfo2.Invoke(this.f, array);
			IL_0740:
			qilNode.SourceLine = qilList.SourceLine;
			this.stk.Peek().Add(qilNode);
		}

		private string ReadText()
		{
			string text = string.Empty;
			if (!this.r.IsEmptyElement)
			{
				while (this.r.Read())
				{
					XmlNodeType nodeType = this.r.NodeType;
					if (nodeType != XmlNodeType.Text && nodeType - XmlNodeType.Whitespace > 1)
					{
						break;
					}
					text += this.r.Value;
				}
			}
			return text;
		}

		private ISourceLineInfo ParseLineInfo(string s)
		{
			if (s != null && s.Length > 0)
			{
				Match match = QilXmlReader.lineInfoRegex.Match(s);
				return new SourceLineInfo("", int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture), int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture));
			}
			return null;
		}

		private XmlQueryType ParseType(string s)
		{
			if (s != null && s.Length > 0)
			{
				Match match = QilXmlReader.typeInfoRegex.Match(s);
				XmlQueryCardinality xmlQueryCardinality = new XmlQueryCardinality(match.Groups[1].Value);
				bool flag = bool.Parse(match.Groups[3].Value);
				string[] array = match.Groups[2].Value.Split(new char[] { '|' });
				XmlQueryType[] array2 = new XmlQueryType[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = XmlQueryTypeFactory.Type((XmlTypeCode)Enum.Parse(typeof(XmlTypeCode), array[i]), flag);
				}
				return XmlQueryTypeFactory.Product(XmlQueryTypeFactory.Choice(array2), xmlQueryCardinality);
			}
			return null;
		}

		private QilName ParseName(string name)
		{
			if (name != null && name.Length > 0)
			{
				int num = name.LastIndexOf('}');
				string text;
				if (num != -1 && name[0] == '{')
				{
					text = name.Substring(1, num - 1);
					name = name.Substring(num + 1);
				}
				else
				{
					text = string.Empty;
				}
				string text2;
				string text3;
				ValidateNames.ParseQNameThrow(name, out text2, out text3);
				return this.f.LiteralQName(text3, text, text2);
			}
			return null;
		}

		private static Regex lineInfoRegex = new Regex("\\[(\\d+),(\\d+) -- (\\d+),(\\d+)\\]");

		private static Regex typeInfoRegex = new Regex("(\\w+);([\\w|\\|]+);(\\w+)");

		private static Dictionary<string, MethodInfo> nameToFactoryMethod = new Dictionary<string, MethodInfo>();

		private QilFactory f;

		private XmlReader r;

		private Stack<QilList> stk;

		private bool inFwdDecls;

		private Dictionary<string, QilNode> scope;

		private Dictionary<string, QilNode> fwdDecls;

		private class ReaderAnnotation
		{
			public string Id;

			public QilName Name;

			public XmlQueryType XmlType;

			public string ClrNamespace;
		}
	}
}
