using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal sealed class UxmlSerializableAdapter<T> : UxmlSerializableAdapterBase
	{
		public override object dataBoxed
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = (T)((object)value);
			}
		}

		public T CloneInstance(T value)
		{
			UxmlSerializableAdapter<T> uxmlSerializableAdapter = null;
			try
			{
				IUxmlSerializedDataDeserializeReference uxmlSerializedDataDeserializeReference = value as IUxmlSerializedDataDeserializeReference;
				bool flag = uxmlSerializedDataDeserializeReference != null;
				if (flag)
				{
					return (T)((object)uxmlSerializedDataDeserializeReference.DeserializeReference());
				}
				this.data = value;
				string text = JsonUtility.ToJson(this);
				uxmlSerializableAdapter = JsonUtility.FromJson<UxmlSerializableAdapter<T>>(text);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			finally
			{
				this.data = default(T);
			}
			return (uxmlSerializableAdapter != null) ? uxmlSerializableAdapter.data : default(T);
		}

		public override object CloneInstanceBoxed(object value)
		{
			return this.CloneInstance((T)((object)value));
		}

		public static readonly UxmlSerializableAdapter<T> SharedInstance = new UxmlSerializableAdapter<T>();

		public T data;
	}
}
