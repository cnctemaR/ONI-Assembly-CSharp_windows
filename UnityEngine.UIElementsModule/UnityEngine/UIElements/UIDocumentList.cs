using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class UIDocumentList
	{
		internal void RemoveFromListAndFromVisualTree(UIDocument uiDocument)
		{
			this.m_AttachedUIDocuments.Remove(uiDocument);
			VisualElement rootVisualElement = uiDocument.rootVisualElement;
			if (rootVisualElement != null)
			{
				rootVisualElement.RemoveFromHierarchy();
			}
		}

		internal void AddToListAndToVisualTree(UIDocument uiDocument, VisualElement visualTree, bool ignoreContentContainer, int firstInsertIndex = 0)
		{
			int num = 0;
			foreach (UIDocument uidocument in this.m_AttachedUIDocuments)
			{
				bool flag = uiDocument.sortingOrder > uidocument.sortingOrder;
				if (flag)
				{
					num++;
				}
				else
				{
					bool flag2 = uiDocument.sortingOrder < uidocument.sortingOrder;
					if (flag2)
					{
						break;
					}
					bool flag3 = uiDocument.m_UIDocumentCreationIndex > uidocument.m_UIDocumentCreationIndex;
					if (!flag3)
					{
						break;
					}
					num++;
				}
			}
			bool flag4 = num < this.m_AttachedUIDocuments.Count;
			if (flag4)
			{
				this.m_AttachedUIDocuments.Insert(num, uiDocument);
				bool flag5 = visualTree == null || uiDocument.rootVisualElement == null;
				if (flag5)
				{
					return;
				}
				bool flag6 = num > 0;
				if (flag6)
				{
					VisualElement visualElement = null;
					int num2 = 1;
					while (visualElement == null && num - num2 >= 0)
					{
						UIDocument uidocument2 = this.m_AttachedUIDocuments[num - num2++];
						visualElement = uidocument2.rootVisualElement;
					}
					bool flag7 = visualElement != null;
					if (flag7)
					{
						num = visualTree.IndexOf(visualElement, ignoreContentContainer) + 1;
					}
				}
				int num3 = visualTree.ChildCount(ignoreContentContainer);
				bool flag8 = num > num3;
				if (flag8)
				{
					num = num3;
				}
			}
			else
			{
				this.m_AttachedUIDocuments.Add(uiDocument);
			}
			bool flag9 = visualTree == null || uiDocument.rootVisualElement == null;
			if (!flag9)
			{
				int num4 = firstInsertIndex + num;
				bool flag10 = num4 < visualTree.ChildCount(ignoreContentContainer);
				if (flag10)
				{
					visualTree.Insert(num4, uiDocument.rootVisualElement, ignoreContentContainer);
				}
				else
				{
					visualTree.Add(uiDocument.rootVisualElement, ignoreContentContainer);
				}
			}
		}

		internal List<UIDocument> m_AttachedUIDocuments = new List<UIDocument>();
	}
}
