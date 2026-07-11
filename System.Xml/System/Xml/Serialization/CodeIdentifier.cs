using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using Microsoft.CSharp;

namespace System.Xml.Serialization
{
	public class CodeIdentifier
	{
		[Obsolete("This class should never get constructed as it contains only static methods.")]
		public CodeIdentifier()
		{
		}

		public static string MakePascal(string identifier)
		{
			identifier = CodeIdentifier.MakeValid(identifier);
			if (identifier.Length <= 2)
			{
				return identifier.ToUpper(CultureInfo.InvariantCulture);
			}
			if (char.IsLower(identifier[0]))
			{
				return char.ToUpper(identifier[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + identifier.Substring(1);
			}
			return identifier;
		}

		public static string MakeCamel(string identifier)
		{
			identifier = CodeIdentifier.MakeValid(identifier);
			if (identifier.Length <= 2)
			{
				return identifier.ToLower(CultureInfo.InvariantCulture);
			}
			if (char.IsUpper(identifier[0]))
			{
				return char.ToLower(identifier[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + identifier.Substring(1);
			}
			return identifier;
		}

		public static string MakeValid(string identifier)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (num < identifier.Length && stringBuilder.Length < 511)
			{
				char c = identifier[num];
				if (CodeIdentifier.IsValid(c))
				{
					if (stringBuilder.Length == 0 && !CodeIdentifier.IsValidStart(c))
					{
						stringBuilder.Append("Item");
					}
					stringBuilder.Append(c);
				}
				num++;
			}
			if (stringBuilder.Length == 0)
			{
				return "Item";
			}
			return stringBuilder.ToString();
		}

		internal static string MakeValidInternal(string identifier)
		{
			if (identifier.Length > 30)
			{
				return "Item";
			}
			return CodeIdentifier.MakeValid(identifier);
		}

		private static bool IsValidStart(char c)
		{
			return char.GetUnicodeCategory(c) != UnicodeCategory.DecimalDigitNumber;
		}

		private static bool IsValid(char c)
		{
			switch (char.GetUnicodeCategory(c))
			{
			case UnicodeCategory.UppercaseLetter:
			case UnicodeCategory.LowercaseLetter:
			case UnicodeCategory.TitlecaseLetter:
			case UnicodeCategory.ModifierLetter:
			case UnicodeCategory.OtherLetter:
			case UnicodeCategory.NonSpacingMark:
			case UnicodeCategory.SpacingCombiningMark:
			case UnicodeCategory.DecimalDigitNumber:
			case UnicodeCategory.ConnectorPunctuation:
				return true;
			case UnicodeCategory.EnclosingMark:
			case UnicodeCategory.LetterNumber:
			case UnicodeCategory.OtherNumber:
			case UnicodeCategory.SpaceSeparator:
			case UnicodeCategory.LineSeparator:
			case UnicodeCategory.ParagraphSeparator:
			case UnicodeCategory.Control:
			case UnicodeCategory.Format:
			case UnicodeCategory.Surrogate:
			case UnicodeCategory.PrivateUse:
			case UnicodeCategory.DashPunctuation:
			case UnicodeCategory.OpenPunctuation:
			case UnicodeCategory.ClosePunctuation:
			case UnicodeCategory.InitialQuotePunctuation:
			case UnicodeCategory.FinalQuotePunctuation:
			case UnicodeCategory.OtherPunctuation:
			case UnicodeCategory.MathSymbol:
			case UnicodeCategory.CurrencySymbol:
			case UnicodeCategory.ModifierSymbol:
			case UnicodeCategory.OtherSymbol:
			case UnicodeCategory.OtherNotAssigned:
				return false;
			default:
				return false;
			}
		}

		internal static void CheckValidIdentifier(string ident)
		{
			if (!CodeGenerator.IsValidLanguageIndependentIdentifier(ident))
			{
				throw new ArgumentException(Res.GetString("Identifier '{0}' is not CLS-compliant.", new object[] { ident }), "ident");
			}
		}

		internal static string GetCSharpName(string name)
		{
			return CodeIdentifier.EscapeKeywords(name.Replace('+', '.'), CodeIdentifier.csharp);
		}

		private static int GetCSharpName(Type t, Type[] parameters, int index, StringBuilder sb)
		{
			if (t.DeclaringType != null && t.DeclaringType != t)
			{
				index = CodeIdentifier.GetCSharpName(t.DeclaringType, parameters, index, sb);
				sb.Append(".");
			}
			string name = t.Name;
			int num = name.IndexOf('`');
			if (num < 0)
			{
				num = name.IndexOf('!');
			}
			if (num > 0)
			{
				CodeIdentifier.EscapeKeywords(name.Substring(0, num), CodeIdentifier.csharp, sb);
				sb.Append("<");
				int num2 = int.Parse(name.Substring(num + 1), CultureInfo.InvariantCulture) + index;
				while (index < num2)
				{
					sb.Append(CodeIdentifier.GetCSharpName(parameters[index]));
					if (index < num2 - 1)
					{
						sb.Append(",");
					}
					index++;
				}
				sb.Append(">");
			}
			else
			{
				CodeIdentifier.EscapeKeywords(name, CodeIdentifier.csharp, sb);
			}
			return index;
		}

		internal static string GetCSharpName(Type t)
		{
			int num = 0;
			while (t.IsArray)
			{
				t = t.GetElementType();
				num++;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("global::");
			string @namespace = t.Namespace;
			if (@namespace != null && @namespace.Length > 0)
			{
				string[] array = @namespace.Split(new char[] { '.' });
				for (int i = 0; i < array.Length; i++)
				{
					CodeIdentifier.EscapeKeywords(array[i], CodeIdentifier.csharp, stringBuilder);
					stringBuilder.Append(".");
				}
			}
			Type[] array2 = ((t.IsGenericType || t.ContainsGenericParameters) ? t.GetGenericArguments() : new Type[0]);
			CodeIdentifier.GetCSharpName(t, array2, 0, stringBuilder);
			for (int j = 0; j < num; j++)
			{
				stringBuilder.Append("[]");
			}
			return stringBuilder.ToString();
		}

		private static void EscapeKeywords(string identifier, CodeDomProvider codeProvider, StringBuilder sb)
		{
			if (identifier == null || identifier.Length == 0)
			{
				return;
			}
			int num = 0;
			while (identifier.EndsWith("[]", StringComparison.Ordinal))
			{
				num++;
				identifier = identifier.Substring(0, identifier.Length - 2);
			}
			if (identifier.Length > 0)
			{
				CodeIdentifier.CheckValidIdentifier(identifier);
				identifier = codeProvider.CreateEscapedIdentifier(identifier);
				sb.Append(identifier);
			}
			for (int i = 0; i < num; i++)
			{
				sb.Append("[]");
			}
		}

		private static string EscapeKeywords(string identifier, CodeDomProvider codeProvider)
		{
			if (identifier == null || identifier.Length == 0)
			{
				return identifier;
			}
			string[] array = identifier.Split(new char[] { '.', ',', '<', '>' });
			StringBuilder stringBuilder = new StringBuilder();
			int num = -1;
			for (int i = 0; i < array.Length; i++)
			{
				if (num >= 0)
				{
					stringBuilder.Append(identifier.Substring(num, 1));
				}
				num++;
				num += array[i].Length;
				CodeIdentifier.EscapeKeywords(array[i].Trim(), codeProvider, stringBuilder);
			}
			if (stringBuilder.Length != identifier.Length)
			{
				return stringBuilder.ToString();
			}
			return identifier;
		}

		internal static CodeDomProvider csharp = new CSharpCodeProvider();

		internal const int MaxIdentifierLength = 511;
	}
}
