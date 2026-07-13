using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.StyleSheets.Syntax;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[UxmlObject]
	internal class Syntax : StylePropertyValidation
	{
		[CreateProperty]
		public string property
		{
			get
			{
				return this.m_Property;
			}
			set
			{
				bool flag = string.Compare(this.m_Property, value, StringComparison.Ordinal) == 0;
				if (!flag)
				{
					this.m_Property = value;
					base.NotifyPropertyChanged(in Syntax.propertyBindingProperty);
				}
			}
		}

		public Syntax()
		{
		}

		public Syntax(string property)
		{
			this.property = property;
		}

		public static Expression GetSyntaxTree(Syntax syntax)
		{
			StyleSyntaxParser styleSyntaxParser = new StyleSyntaxParser();
			string expressionString = Syntax.GetExpressionString(syntax);
			return styleSyntaxParser.Parse(expressionString);
		}

		public static Expression GetSyntaxTree(List<Syntax> syntaxes)
		{
			StyleSyntaxParser styleSyntaxParser = new StyleSyntaxParser();
			string text = string.Join(" | ", syntaxes.UniqueSelect<Syntax, string>(new Func<Syntax, string>(Syntax.GetExpressionString)));
			return styleSyntaxParser.Parse(text);
		}

		private static string GetExpressionString(Syntax syntax)
		{
			bool flag = string.IsNullOrEmpty(syntax.property);
			string text;
			if (flag)
			{
				text = null;
			}
			else
			{
				bool flag2 = Syntax.k_SyntaxTerms.Contains(syntax.property);
				if (flag2)
				{
					text = "<" + syntax.property + ">";
				}
				else
				{
					string text2;
					text = (StylePropertyCache.TryGetSyntax(syntax.property, out text2) ? ("<'" + syntax.property + "'>") : syntax.property);
				}
			}
			return text;
		}

		private static readonly BindingId propertyBindingProperty = "property";

		private static readonly List<string> k_SyntaxTerms = new List<string> { "length", "length-percentage", "color", "url", "resource", "angle", "number", "time", "single-transition-property", "easing-function" };

		private string m_Property;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : StylePropertyValidation.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Syntax.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("property", "property", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Syntax();
			}

			public override void Deserialize(object obj)
			{
				Syntax syntax = (Syntax)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.property_UxmlAttributeFlags);
				if (flag)
				{
					syntax.property = this.property;
				}
			}

			[SerializeField]
			private string property;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags property_UxmlAttributeFlags;
		}
	}
}
