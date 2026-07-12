using System;

namespace UnityEngine.UIElements
{
	internal delegate void RegisterSerializedPropertyBindCallback<TValueType, TField, TFieldValue>(BaseCompositeField<TValueType, TField, TFieldValue> compositeField, TField field) where TField : TextValueField<TFieldValue>, new();
}
