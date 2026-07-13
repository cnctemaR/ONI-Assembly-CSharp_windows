using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mono.Cecil;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal class InlineSignature : ICallSiteGenerator
	{
		public bool HasThis { get; set; }

		public bool ExplicitThis { get; set; }

		public CallingConvention CallingConvention { get; set; } = CallingConvention.Winapi;

		public List<object> Parameters { get; set; } = new List<object>();

		public object ReturnType { get; set; } = typeof(void);

		public override string ToString()
		{
			Type type = this.ReturnType as Type;
			string text;
			if (type == null)
			{
				object returnType = this.ReturnType;
				text = ((returnType != null) ? returnType.ToString() : null);
			}
			else
			{
				text = type.FullDescription();
			}
			return text + " (" + this.Parameters.Join<object>(delegate(object p)
			{
				Type type2 = p as Type;
				if (type2 != null)
				{
					return type2.FullDescription();
				}
				if (p == null)
				{
					return null;
				}
				return p.ToString();
			}, ", ") + ")";
		}

		internal static TypeReference GetTypeReference(ModuleDefinition module, object param)
		{
			Type type = param as Type;
			TypeReference typeReference;
			if (type == null)
			{
				InlineSignature inlineSignature = param as InlineSignature;
				if (inlineSignature == null)
				{
					InlineSignature.ModifierType modifierType = param as InlineSignature.ModifierType;
					if (modifierType == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Unsupported inline signature parameter type: ");
						defaultInterpolatedStringHandler.AppendFormatted<object>(param);
						defaultInterpolatedStringHandler.AppendLiteral(" (");
						defaultInterpolatedStringHandler.AppendFormatted((param != null) ? param.GetType().FullDescription() : null);
						defaultInterpolatedStringHandler.AppendLiteral(")");
						throw new NotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					typeReference = modifierType.ToTypeReference(module);
				}
				else
				{
					typeReference = inlineSignature.ToFunctionPointer(module);
				}
			}
			else
			{
				typeReference = module.ImportReference(type);
			}
			return typeReference;
		}

		Mono.Cecil.CallSite ICallSiteGenerator.ToCallSite(ModuleDefinition module)
		{
			Mono.Cecil.CallSite callSite = new Mono.Cecil.CallSite(InlineSignature.GetTypeReference(module, this.ReturnType))
			{
				HasThis = this.HasThis,
				ExplicitThis = this.ExplicitThis,
				CallingConvention = (MethodCallingConvention)((byte)this.CallingConvention - 1)
			};
			foreach (object obj in this.Parameters)
			{
				callSite.Parameters.Add(new ParameterDefinition(InlineSignature.GetTypeReference(module, obj)));
			}
			return callSite;
		}

		private FunctionPointerType ToFunctionPointer(ModuleDefinition module)
		{
			FunctionPointerType functionPointerType = new FunctionPointerType
			{
				ReturnType = InlineSignature.GetTypeReference(module, this.ReturnType),
				HasThis = this.HasThis,
				ExplicitThis = this.ExplicitThis,
				CallingConvention = (MethodCallingConvention)((byte)this.CallingConvention - 1)
			};
			foreach (object obj in this.Parameters)
			{
				functionPointerType.Parameters.Add(new ParameterDefinition(InlineSignature.GetTypeReference(module, obj)));
			}
			return functionPointerType;
		}

		public class ModifierType
		{
			public override string ToString()
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
				Type type = this.Type as Type;
				string text;
				if (type == null)
				{
					object type2 = this.Type;
					text = ((type2 != null) ? type2.ToString() : null);
				}
				else
				{
					text = type.FullDescription();
				}
				defaultInterpolatedStringHandler.AppendFormatted(text);
				defaultInterpolatedStringHandler.AppendLiteral(" mod");
				defaultInterpolatedStringHandler.AppendFormatted(this.IsOptional ? "opt" : "req");
				defaultInterpolatedStringHandler.AppendLiteral("(");
				Type modifier = this.Modifier;
				defaultInterpolatedStringHandler.AppendFormatted((modifier != null) ? modifier.FullDescription() : null);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}

			internal TypeReference ToTypeReference(ModuleDefinition module)
			{
				if (this.IsOptional)
				{
					return new OptionalModifierType(module.ImportReference(this.Modifier), InlineSignature.GetTypeReference(module, this.Type));
				}
				return new RequiredModifierType(module.ImportReference(this.Modifier), InlineSignature.GetTypeReference(module, this.Type));
			}

			public bool IsOptional;

			public Type Modifier;

			public object Type;
		}
	}
}
