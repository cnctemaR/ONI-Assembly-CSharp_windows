using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.Bindings;

namespace UnityEngine.TestTools
{
	[NativeType(CodegenOptions.Custom, "ManagedCoveredMethodStats", Header = "Runtime/Scripting/ScriptingCoverage.bindings.h")]
	public struct CoveredMethodStats
	{
		private string GetTypeDisplayName(Type t)
		{
			bool flag = t == typeof(int);
			string text;
			if (flag)
			{
				text = "int";
			}
			else
			{
				bool flag2 = t == typeof(bool);
				if (flag2)
				{
					text = "bool";
				}
				else
				{
					bool flag3 = t == typeof(float);
					if (flag3)
					{
						text = "float";
					}
					else
					{
						bool flag4 = t == typeof(double);
						if (flag4)
						{
							text = "double";
						}
						else
						{
							bool flag5 = t == typeof(void);
							if (flag5)
							{
								text = "void";
							}
							else
							{
								bool flag6 = t == typeof(string);
								if (flag6)
								{
									text = "string";
								}
								else
								{
									bool flag7 = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
									if (flag7)
									{
										text = "System.Collections.Generic.List<" + this.GetTypeDisplayName(t.GetGenericArguments()[0]) + ">";
									}
									else
									{
										bool flag8 = t.IsArray && t.GetArrayRank() == 1;
										if (flag8)
										{
											text = this.GetTypeDisplayName(t.GetElementType()) + "[]";
										}
										else
										{
											text = t.FullName;
										}
									}
								}
							}
						}
					}
				}
			}
			return text;
		}

		public override string ToString()
		{
			bool flag = this.method == null;
			string text;
			if (flag)
			{
				text = "<no method>";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.GetTypeDisplayName(this.method.DeclaringType));
				stringBuilder.Append(".");
				stringBuilder.Append(this.method.Name);
				stringBuilder.Append("(");
				bool flag2 = false;
				foreach (ParameterInfo parameterInfo in this.method.GetParameters())
				{
					bool flag3 = flag2;
					if (flag3)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(this.GetTypeDisplayName(parameterInfo.ParameterType));
					stringBuilder.Append(" ");
					stringBuilder.Append(parameterInfo.Name);
					flag2 = true;
				}
				stringBuilder.Append(")");
				text = stringBuilder.ToString();
			}
			return text;
		}

		public MethodBase method;

		public int totalSequencePoints;

		public int uncoveredSequencePoints;
	}
}
