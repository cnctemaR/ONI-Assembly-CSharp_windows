using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		DebugText.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		DebugText.Instance = this;
	}

	public void Draw(string text, Vector3 pos, Color color)
	{
		DebugText.Entry entry = new DebugText.Entry
		{
			text = text,
			pos = pos,
			color = color
		};
		this.entries.Add(entry);
	}

	private void LateUpdate()
	{
		foreach (Text text in this.texts)
		{
			global::UnityEngine.Object.Destroy(text.gameObject);
		}
		this.texts.Clear();
		foreach (DebugText.Entry entry in this.entries)
		{
			GameObject gameObject = new GameObject();
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.GetComponent<RectTransform>());
			gameObject.transform.SetPosition(entry.pos);
			rectTransform.localScale = new Vector3(0.02f, 0.02f, 1f);
			Text text2 = gameObject.AddComponent<Text>();
			text2.font = Assets.DebugFont;
			text2.text = entry.text;
			text2.color = entry.color;
			text2.horizontalOverflow = HorizontalWrapMode.Overflow;
			text2.verticalOverflow = VerticalWrapMode.Overflow;
			text2.alignment = TextAnchor.MiddleCenter;
			this.texts.Add(text2);
		}
		this.entries.Clear();
	}

	public static DebugText Instance;

	private List<DebugText.Entry> entries = new List<DebugText.Entry>();

	private List<Text> texts = new List<Text>();

	private struct Entry
	{
		public string text;

		public Vector3 pos;

		public Color color;
	}
}
