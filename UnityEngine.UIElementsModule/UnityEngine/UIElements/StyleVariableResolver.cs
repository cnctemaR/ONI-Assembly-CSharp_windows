using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.StyleSheets.Syntax;

namespace UnityEngine.UIElements
{
	internal class StyleVariableResolver
	{
		private StyleSheet currentSheet
		{
			get
			{
				return this.m_CurrentContext.sheet;
			}
		}

		private StyleValueHandle[] currentHandles
		{
			get
			{
				return this.m_CurrentContext.handles;
			}
		}

		public List<StylePropertyValue> resolvedValues
		{
			get
			{
				return this.m_ResolvedValues;
			}
		}

		public StyleVariableContext variableContext { get; set; }

		public void Init(StyleProperty property, StyleSheet sheet, StyleValueHandle[] handles)
		{
			this.m_ResolvedValues.Clear();
			this.m_ContextStack.Clear();
			this.m_Property = property;
			this.PushContext(sheet, handles);
		}

		private void PushContext(StyleSheet sheet, StyleValueHandle[] handles)
		{
			this.m_CurrentContext = new StyleVariableResolver.ResolveContext
			{
				sheet = sheet,
				handles = handles
			};
			this.m_ContextStack.Push(this.m_CurrentContext);
		}

		private void PopContext()
		{
			this.m_ContextStack.Pop();
			this.m_CurrentContext = this.m_ContextStack.Peek();
		}

		public void AddValue(StyleValueHandle handle)
		{
			this.m_ResolvedValues.Add(new StylePropertyValue
			{
				sheet = this.currentSheet,
				handle = handle
			});
		}

		public bool ResolveVarFunction(ref int index)
		{
			this.m_ResolvedVarStack.Clear();
			int num;
			string text;
			StyleVariableResolver.ParseVarFunction(this.currentSheet, this.currentHandles, ref index, out num, out text);
			StyleVariableResolver.Result result = this.ResolveVarFunction(ref index, num, text);
			return result == StyleVariableResolver.Result.Valid;
		}

		private StyleVariableResolver.Result ResolveVarFunction(ref int index, int argc, string varName)
		{
			StyleVariableResolver.Result result = this.ResolveVariable(varName);
			bool flag = argc > 1;
			if (flag)
			{
				StyleValueHandle[] currentHandles = this.currentHandles;
				int num = index + 1;
				index = num;
				StyleValueHandle styleValueHandle = currentHandles[num];
				Debug.Assert(styleValueHandle.valueType == StyleValueType.CommaSeparator, string.Format("Unexpected value type {0} in var() fallback; expected CommaSeparator.", styleValueHandle.valueType));
				bool flag2 = styleValueHandle.valueType == StyleValueType.CommaSeparator && index + 1 < this.currentHandles.Length;
				if (flag2)
				{
					index++;
					result = this.ResolveFallback(ref index, result == StyleVariableResolver.Result.NotFound);
				}
			}
			return result;
		}

		public bool ValidateResolvedValues()
		{
			bool isCustomProperty = this.m_Property.isCustomProperty;
			bool flag;
			if (isCustomProperty)
			{
				flag = true;
			}
			else
			{
				string text;
				bool flag2 = !StylePropertyCache.TryGetSyntax(this.m_Property.name, out text);
				if (flag2)
				{
					Debug.LogAssertion("Unknown style property " + this.m_Property.name);
					flag = false;
				}
				else
				{
					Expression expression = StyleVariableResolver.s_SyntaxParser.Parse(text);
					flag = this.m_Matcher.Match(expression, this.m_ResolvedValues).success;
				}
			}
			return flag;
		}

		private StyleVariableResolver.Result ResolveVariable(string variableName)
		{
			StyleVariable styleVariable;
			bool flag = !this.variableContext.TryFindVariable(variableName, out styleVariable);
			StyleVariableResolver.Result result;
			if (flag)
			{
				result = StyleVariableResolver.Result.NotFound;
			}
			else
			{
				bool flag2 = this.m_ResolvedVarStack.Contains(styleVariable.name);
				if (flag2)
				{
					result = StyleVariableResolver.Result.NotFound;
				}
				else
				{
					this.m_ResolvedVarStack.Push(styleVariable.name);
					StyleVariableResolver.Result result2 = StyleVariableResolver.Result.Valid;
					int num = 0;
					while (num < styleVariable.handles.Length && result2 == StyleVariableResolver.Result.Valid)
					{
						bool flag3 = this.m_ResolvedValues.Count + 1 > 100;
						if (flag3)
						{
							return StyleVariableResolver.Result.Invalid;
						}
						StyleValueHandle styleValueHandle = styleVariable.handles[num];
						bool flag4 = styleValueHandle.IsVarFunction();
						if (flag4)
						{
							this.PushContext(styleVariable.sheet, styleVariable.handles);
							int num2;
							string text;
							StyleVariableResolver.ParseVarFunction(styleVariable.sheet, styleVariable.handles, ref num, out num2, out text);
							result2 = this.ResolveVarFunction(ref num, num2, text);
							this.PopContext();
						}
						else
						{
							this.m_ResolvedValues.Add(new StylePropertyValue
							{
								sheet = styleVariable.sheet,
								handle = styleValueHandle
							});
						}
						num++;
					}
					this.m_ResolvedVarStack.Pop();
					result = result2;
				}
			}
			return result;
		}

		private StyleVariableResolver.Result ResolveFallback(ref int index, bool appendValues)
		{
			StyleVariableResolver.Result result = StyleVariableResolver.Result.Valid;
			while (index < this.currentHandles.Length && result == StyleVariableResolver.Result.Valid)
			{
				StyleValueHandle styleValueHandle = this.currentHandles[index];
				bool flag = styleValueHandle.IsVarFunction();
				if (flag)
				{
					int num;
					string text;
					StyleVariableResolver.ParseVarFunction(this.currentSheet, this.currentHandles, ref index, out num, out text);
					if (appendValues)
					{
						StyleVariableResolver.Result result2 = this.ResolveVarFunction(ref index, num, text);
						bool flag2 = result2 > StyleVariableResolver.Result.Valid;
						if (flag2)
						{
							result = result2;
						}
					}
					else
					{
						bool flag3 = num > 1;
						if (flag3)
						{
							StyleValueHandle[] currentHandles = this.currentHandles;
							int num2 = index + 1;
							index = num2;
							StyleValueHandle styleValueHandle2 = currentHandles[num2];
							Debug.Assert(styleValueHandle2.valueType == StyleValueType.CommaSeparator);
							bool flag4 = styleValueHandle2.valueType == StyleValueType.CommaSeparator && index + 1 < this.currentHandles.Length;
							if (flag4)
							{
								index++;
								this.ResolveFallback(ref index, false);
							}
						}
					}
				}
				else if (appendValues)
				{
					this.m_ResolvedValues.Add(new StylePropertyValue
					{
						sheet = this.currentSheet,
						handle = styleValueHandle
					});
				}
				index++;
			}
			return result;
		}

		private static void ParseVarFunction(StyleSheet sheet, StyleValueHandle[] handles, ref int index, out int argCount, out string variableName)
		{
			int num = index + 1;
			index = num;
			argCount = (int)sheet.ReadFloat(handles[num]);
			num = index + 1;
			index = num;
			variableName = sheet.ReadVariable(handles[num]);
		}

		internal const int kMaxResolves = 100;

		private static StyleSyntaxParser s_SyntaxParser = new StyleSyntaxParser();

		private StylePropertyValueMatcher m_Matcher = new StylePropertyValueMatcher();

		private List<StylePropertyValue> m_ResolvedValues = new List<StylePropertyValue>();

		private Stack<string> m_ResolvedVarStack = new Stack<string>();

		private StyleProperty m_Property;

		private Stack<StyleVariableResolver.ResolveContext> m_ContextStack = new Stack<StyleVariableResolver.ResolveContext>();

		private StyleVariableResolver.ResolveContext m_CurrentContext;

		private enum Result
		{
			Valid,
			Invalid,
			NotFound
		}

		private struct ResolveContext
		{
			public StyleSheet sheet;

			public StyleValueHandle[] handles;
		}
	}
}
