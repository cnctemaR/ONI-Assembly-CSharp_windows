using System;

namespace UnityEngine.UIElements.Internal
{
	internal class MultiColumnHeaderColumnIcon : Image
	{
		public bool isImageInline { get; set; }

		public MultiColumnHeaderColumnIcon()
		{
			base.AddToClassList(MultiColumnHeaderColumnIcon.ussClassName);
			base.RegisterCallback<CustomStyleResolvedEvent>(delegate(CustomStyleResolvedEvent evt)
			{
				this.UpdateClassList();
			}, TrickleDown.NoTrickleDown);
		}

		public void UpdateClassList()
		{
			base.parent.RemoveFromClassList(MultiColumnHeaderColumn.hasIconUssClassName);
			bool flag = base.image != null || base.sprite != null || base.vectorImage != null;
			if (flag)
			{
				base.parent.AddToClassList(MultiColumnHeaderColumn.hasIconUssClassName);
			}
		}

		public new static readonly string ussClassName = MultiColumnHeaderColumn.ussClassName + "__icon";
	}
}
