using System;

namespace UnityEngine.UIElements
{
	internal class EntityIdField : BaseField<EntityId>
	{
		public EntityIdField()
			: this(null)
		{
		}

		public EntityIdField(string label)
			: base(label, null)
		{
			base.AddToClassList(EntityIdField.ussClassName);
			base.labelElement.AddToClassList(EntityIdField.labelUssClassName);
			base.visualInput.AddToClassList(EntityIdField.inputUssClassName);
			base.visualInput.Add(this.m_IntegerField);
			this.m_IntegerField.RegisterValueChangedCallback<int>(delegate(ChangeEvent<int> evt)
			{
				this.value = EntityId.From(evt.newValue);
			});
		}

		public override void SetValueWithoutNotify(EntityId newValue)
		{
			base.SetValueWithoutNotify(newValue);
			this.m_IntegerField.SetValueWithoutNotify(newValue.GetRawData());
		}

		private readonly IntegerField m_IntegerField = new IntegerField();

		public new static readonly string ussClassName = "unity-entityId-field";

		public new static readonly string labelUssClassName = EntityIdField.ussClassName + "__label";

		public new static readonly string inputUssClassName = EntityIdField.ussClassName + "__input";
	}
}
