using System;

namespace FileHelpers
{
	internal class TypesOfMessages
	{
		internal class Errors
		{
			internal class ClassWithOutDefaultConstructorClass : MessageBase
			{
				internal ClassWithOutDefaultConstructorClass()
					: base("The record class $ClassName$ needs a constructor with no args (public or private)")
				{
				}

				internal TypesOfMessages.Errors.ClassWithOutDefaultConstructorClass ClassName(string value)
				{
					this.mClassName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$ClassName$", this.mClassName);
				}

				private string mClassName;
			}

			internal class ClassWithOutFieldsClass : MessageBase
			{
				internal ClassWithOutFieldsClass()
					: base("The record class $ClassName$ don't contains any field")
				{
				}

				internal TypesOfMessages.Errors.ClassWithOutFieldsClass ClassName(string value)
				{
					this.mClassName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$ClassName$", this.mClassName);
				}

				private string mClassName;
			}

			internal class ClassWithOutRecordAttributeClass : MessageBase
			{
				internal ClassWithOutRecordAttributeClass()
					: base("The record class $ClassName$ must be marked with the [DelimitedRecord] or [FixedLengthRecord] Attribute")
				{
				}

				internal TypesOfMessages.Errors.ClassWithOutRecordAttributeClass ClassName(string value)
				{
					this.mClassName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$ClassName$", this.mClassName);
				}

				private string mClassName;
			}

			internal class EmptyClassNameClass : MessageBase
			{
				internal EmptyClassNameClass()
					: base("The ClassName can't be empty")
				{
				}

				protected override string GenerateText()
				{
					return base.SourceText;
				}
			}

			internal class EmptyFieldNameClass : MessageBase
			{
				internal EmptyFieldNameClass()
					: base("The $Position$th field name can't be empty")
				{
				}

				internal TypesOfMessages.Errors.EmptyFieldNameClass Position(string value)
				{
					this.mPosition = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$Position$", this.mPosition);
				}

				private string mPosition;
			}

			internal class EmptyFieldTypeClass : MessageBase
			{
				internal EmptyFieldTypeClass()
					: base("The $Position$th field type can't be empty")
				{
				}

				internal TypesOfMessages.Errors.EmptyFieldTypeClass Position(string value)
				{
					this.mPosition = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$Position$", this.mPosition);
				}

				private string mPosition;
			}

			internal class ExpectingFieldOptionalClass : MessageBase
			{
				internal ExpectingFieldOptionalClass()
					: base("The field: $FieldName$ must be marked as optional because the previous field is marked as optional. (Try adding [FieldOptional] to $FieldName$)")
				{
				}

				internal TypesOfMessages.Errors.ExpectingFieldOptionalClass FieldName(string value)
				{
					this.mFieldName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$FieldName$", this.mFieldName);
				}

				private string mFieldName;
			}

			internal class FieldNotFoundClass : MessageBase
			{
				internal FieldNotFoundClass()
					: base("The field: $FieldName$ was not found in the class: $ClassName$. Remember that this option is case sensitive")
				{
				}

				internal TypesOfMessages.Errors.FieldNotFoundClass FieldName(string value)
				{
					this.mFieldName = value;
					return this;
				}

				internal TypesOfMessages.Errors.FieldNotFoundClass ClassName(string value)
				{
					this.mClassName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string text = base.SourceText;
					text = StringHelper.ReplaceIgnoringCase(text, "$FieldName$", this.mFieldName);
					return StringHelper.ReplaceIgnoringCase(text, "$ClassName$", this.mClassName);
				}

				private string mFieldName;

				private string mClassName;
			}

			internal class FieldOptionalClass : MessageBase
			{
				internal FieldOptionalClass()
					: base("The field: $Field$ must be marked as optional because the previous field is marked with FieldOptional. (Try adding [FieldOptional] to $Field$)")
				{
				}

				internal TypesOfMessages.Errors.FieldOptionalClass Field(string value)
				{
					this.mField = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$Field$", this.mField);
				}

				private string mField;
			}

			internal class InvalidIdentifierClass : MessageBase
			{
				internal InvalidIdentifierClass()
					: base("The string '$Identifier$' not is a valid .NET identifier")
				{
				}

				internal TypesOfMessages.Errors.InvalidIdentifierClass Identifier(string value)
				{
					this.mIdentifier = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$Identifier$", this.mIdentifier);
				}

				private string mIdentifier;
			}

			internal class MissingFieldArrayLenghtInNotLastFieldClass : MessageBase
			{
				internal MissingFieldArrayLenghtInNotLastFieldClass()
					: base("The field: $FieldName$ is of an array type and must contain a [FieldArrayLength] attribute because it is not the last field")
				{
				}

				internal TypesOfMessages.Errors.MissingFieldArrayLenghtInNotLastFieldClass FieldName(string value)
				{
					this.mFieldName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$FieldName$", this.mFieldName);
				}

				private string mFieldName;
			}

			internal class MixOfStandardAndAutoPropertiesFieldsClass : MessageBase
			{
				internal MixOfStandardAndAutoPropertiesFieldsClass()
					: base("You can mix standard fields and automatic properties only if you use [FieldOrder()] over the fields and properties in the $ClassName$ class.")
				{
				}

				internal TypesOfMessages.Errors.MixOfStandardAndAutoPropertiesFieldsClass ClassName(string value)
				{
					this.mClassName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$ClassName$", this.mClassName);
				}

				private string mClassName;
			}

			internal class NullRecordClassClass : MessageBase
			{
				internal NullRecordClassClass()
					: base("The record type can't be null")
				{
				}

				protected override string GenerateText()
				{
					return base.SourceText;
				}
			}

			internal class PartialFieldOrderClass : MessageBase
			{
				internal PartialFieldOrderClass()
					: base("The field: $FieldName$ must be marked with FieldOrder because if you use this attribute in one field you must also use it on all of them.")
				{
				}

				internal TypesOfMessages.Errors.PartialFieldOrderClass FieldName(string value)
				{
					this.mFieldName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$FieldName$", this.mFieldName);
				}

				private string mFieldName;
			}

			internal class PartialFieldOrderInAutoPropertyClass : MessageBase
			{
				internal PartialFieldOrderInAutoPropertyClass()
					: base("The auto property: $PropertyName$ must be marked with FieldOrder because if you use this attribute in one field you must also use it on all of them.")
				{
				}

				internal TypesOfMessages.Errors.PartialFieldOrderInAutoPropertyClass PropertyName(string value)
				{
					this.mPropertyName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$PropertyName$", this.mPropertyName);
				}

				private string mPropertyName;
			}

			internal class SameFieldOrderClass : MessageBase
			{
				internal SameFieldOrderClass()
					: base("The field: $FieldName1$ has the same FieldOrder that: $FieldName2$ you must use different values")
				{
				}

				internal TypesOfMessages.Errors.SameFieldOrderClass FieldName1(string value)
				{
					this.mFieldName1 = value;
					return this;
				}

				internal TypesOfMessages.Errors.SameFieldOrderClass FieldName2(string value)
				{
					this.mFieldName2 = value;
					return this;
				}

				protected override string GenerateText()
				{
					string text = base.SourceText;
					text = StringHelper.ReplaceIgnoringCase(text, "$FieldName1$", this.mFieldName1);
					return StringHelper.ReplaceIgnoringCase(text, "$FieldName2$", this.mFieldName2);
				}

				private string mFieldName1;

				private string mFieldName2;
			}

			internal class SameMinMaxLengthForArrayNotLastFieldClass : MessageBase
			{
				internal SameMinMaxLengthForArrayNotLastFieldClass()
					: base("The array field: $FieldName$ must be of a fixed length because it is not the last field of the class, i.e. the min and max length of the [FieldArrayLength] attribute must be the same.")
				{
				}

				internal TypesOfMessages.Errors.SameMinMaxLengthForArrayNotLastFieldClass FieldName(string value)
				{
					this.mFieldName = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$FieldName$", this.mFieldName);
				}

				private string mFieldName;
			}

			internal class StructRecordClassClass : MessageBase
			{
				internal StructRecordClassClass()
					: base("The record type must be a class, and the type: $RecordType$ is a struct.")
				{
				}

				internal TypesOfMessages.Errors.StructRecordClassClass RecordType(string value)
				{
					this.mRecordType = value;
					return this;
				}

				protected override string GenerateText()
				{
					string sourceText = base.SourceText;
					return StringHelper.ReplaceIgnoringCase(sourceText, "$RecordType$", this.mRecordType);
				}

				private string mRecordType;
			}

			internal class TestQuoteClass : MessageBase
			{
				internal TestQuoteClass()
					: base("The Message class also allows to use \" in any part of the \" text \" .")
				{
				}

				protected override string GenerateText()
				{
					return base.SourceText;
				}
			}

			internal class WrongConverterClass : MessageBase
			{
				internal WrongConverterClass()
					: base("The converter for the field: $FieldName$ returns an object of Type: $ConverterReturnedType$  and the field is of type: $FieldType$")
				{
				}

				internal TypesOfMessages.Errors.WrongConverterClass FieldName(string value)
				{
					this.mFieldName = value;
					return this;
				}

				internal TypesOfMessages.Errors.WrongConverterClass ConverterReturnedType(string value)
				{
					this.mConverterReturnedType = value;
					return this;
				}

				internal TypesOfMessages.Errors.WrongConverterClass FieldType(string value)
				{
					this.mFieldType = value;
					return this;
				}

				protected override string GenerateText()
				{
					string text = base.SourceText;
					text = StringHelper.ReplaceIgnoringCase(text, "$FieldName$", this.mFieldName);
					text = StringHelper.ReplaceIgnoringCase(text, "$ConverterReturnedType$", this.mConverterReturnedType);
					return StringHelper.ReplaceIgnoringCase(text, "$FieldType$", this.mFieldType);
				}

				private string mFieldName;

				private string mConverterReturnedType;

				private string mFieldType;
			}
		}
	}
}
