using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeTypeReference : CodeObject
	{
		public CodeTypeReference()
		{
		}

		[global::System.MonoTODO("We should parse basetype from right to left in 2.0 profile.")]
		public CodeTypeReference(string baseType)
		{
			this.Parse(baseType);
		}

		[global::System.MonoTODO("We should parse basetype from right to left in 2.0 profile.")]
		public CodeTypeReference(Type baseType)
		{
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}
			if (baseType.IsGenericParameter)
			{
				this.baseType = baseType.Name;
				this.referenceOptions = CodeTypeReferenceOptions.GenericTypeParameter;
			}
			else if (baseType.IsGenericTypeDefinition)
			{
				this.baseType = baseType.FullName;
			}
			else if (baseType.IsGenericType)
			{
				this.baseType = baseType.GetGenericTypeDefinition().FullName;
				foreach (Type type in baseType.GetGenericArguments())
				{
					if (type.IsGenericParameter)
					{
						this.TypeArguments.Add(new CodeTypeReference(new CodeTypeParameter(type.Name)));
					}
					else
					{
						this.TypeArguments.Add(new CodeTypeReference(type));
					}
				}
			}
			else if (baseType.IsArray)
			{
				this.arrayRank = baseType.GetArrayRank();
				this.arrayElementType = new CodeTypeReference(baseType.GetElementType());
				this.baseType = this.arrayElementType.BaseType;
			}
			else
			{
				this.Parse(baseType.FullName);
			}
			this.isInterface = baseType.IsInterface;
		}

		public CodeTypeReference(CodeTypeReference arrayElementType, int arrayRank)
		{
			this.baseType = null;
			this.arrayRank = arrayRank;
			this.arrayElementType = arrayElementType;
		}

		[global::System.MonoTODO("We should parse basetype from right to left in 2.0 profile.")]
		public CodeTypeReference(string baseType, int arrayRank)
			: this(new CodeTypeReference(baseType), arrayRank)
		{
		}

		public CodeTypeReference(CodeTypeParameter typeParameter)
			: this(typeParameter.Name)
		{
			this.referenceOptions = CodeTypeReferenceOptions.GenericTypeParameter;
		}

		public CodeTypeReference(string typeName, CodeTypeReferenceOptions referenceOptions)
			: this(typeName)
		{
			this.referenceOptions = referenceOptions;
		}

		public CodeTypeReference(Type type, CodeTypeReferenceOptions referenceOptions)
			: this(type)
		{
			this.referenceOptions = referenceOptions;
		}

		public CodeTypeReference(string typeName, params CodeTypeReference[] typeArguments)
			: this(typeName)
		{
			this.TypeArguments.AddRange(typeArguments);
			if (this.baseType.IndexOf('`') < 0)
			{
				this.baseType = this.baseType + "`" + this.TypeArguments.Count;
			}
		}

		public CodeTypeReference ArrayElementType
		{
			get
			{
				return this.arrayElementType;
			}
			set
			{
				this.arrayElementType = value;
			}
		}

		public int ArrayRank
		{
			get
			{
				return this.arrayRank;
			}
			set
			{
				this.arrayRank = value;
			}
		}

		public string BaseType
		{
			get
			{
				if (this.arrayElementType != null && this.arrayRank > 0)
				{
					return this.arrayElementType.BaseType;
				}
				if (this.baseType == null)
				{
					return string.Empty;
				}
				return this.baseType;
			}
			set
			{
				this.baseType = value;
			}
		}

		internal bool IsInterface
		{
			get
			{
				return this.isInterface;
			}
		}

		private void Parse(string baseType)
		{
			if (baseType == null || baseType.Length == 0)
			{
				this.baseType = typeof(void).FullName;
				return;
			}
			int num = baseType.IndexOf('[');
			if (num == -1)
			{
				this.baseType = baseType;
				return;
			}
			int num2 = baseType.LastIndexOf(']');
			if (num2 < num)
			{
				this.baseType = baseType;
				return;
			}
			int num3 = baseType.LastIndexOf('>');
			if (num3 != -1 && num3 > num2)
			{
				this.baseType = baseType;
				return;
			}
			string[] array = baseType.Substring(num + 1, num2 - num - 1).Split(new char[] { ',' });
			if (num2 - num != array.Length)
			{
				this.baseType = baseType.Substring(0, num);
				int num4 = 0;
				int i = num;
				StringBuilder stringBuilder = new StringBuilder();
				while (i < baseType.Length)
				{
					char c = baseType[i];
					char c2 = c;
					switch (c2)
					{
					case '[':
						if (num4 > 1 && stringBuilder.Length > 0)
						{
							stringBuilder.Append(c);
						}
						num4++;
						break;
					default:
						if (c2 != ',')
						{
							stringBuilder.Append(c);
						}
						else if (num4 > 1)
						{
							while (i + 1 < baseType.Length)
							{
								if (baseType[i + 1] == ']')
								{
									break;
								}
								i++;
							}
						}
						else if (stringBuilder.Length > 0)
						{
							CodeTypeReference codeTypeReference = new CodeTypeReference(stringBuilder.ToString());
							this.TypeArguments.Add(codeTypeReference);
							stringBuilder.Length = 0;
						}
						break;
					case ']':
						num4--;
						if (num4 > 1 && stringBuilder.Length > 0)
						{
							stringBuilder.Append(c);
						}
						if (stringBuilder.Length != 0 && num4 % 2 == 0)
						{
							this.TypeArguments.Add(stringBuilder.ToString());
							stringBuilder.Length = 0;
						}
						break;
					}
					i++;
				}
			}
			else
			{
				this.arrayElementType = new CodeTypeReference(baseType.Substring(0, num));
				this.arrayRank = array.Length;
			}
		}

		[ComVisible(false)]
		public CodeTypeReferenceOptions Options
		{
			get
			{
				return this.referenceOptions;
			}
			set
			{
				this.referenceOptions = value;
			}
		}

		[ComVisible(false)]
		public CodeTypeReferenceCollection TypeArguments
		{
			get
			{
				if (this.typeArguments == null)
				{
					this.typeArguments = new CodeTypeReferenceCollection();
				}
				return this.typeArguments;
			}
		}

		private string baseType;

		private CodeTypeReference arrayElementType;

		private int arrayRank;

		private bool isInterface;

		private bool needsFixup;

		private CodeTypeReferenceCollection typeArguments;

		private CodeTypeReferenceOptions referenceOptions;
	}
}
