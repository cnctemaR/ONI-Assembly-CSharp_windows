using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
	private void OnEnable()
	{
		this.ChatInputField.onSubmit.AddListener(new UnityAction<string>(this.AddToChatOutput));
	}

	private void OnDisable()
	{
		this.ChatInputField.onSubmit.RemoveListener(new UnityAction<string>(this.AddToChatOutput));
	}

	private void AddToChatOutput(string newText)
	{
		this.ChatInputField.text = string.Empty;
		global::System.DateTime now = global::System.DateTime.Now;
		string text = string.Concat(new string[]
		{
			"[<#FFFF80>",
			now.Hour.ToString("d2"),
			":",
			now.Minute.ToString("d2"),
			":",
			now.Second.ToString("d2"),
			"</color>] ",
			newText
		});
		if (this.ChatDisplayOutput != null)
		{
			if (this.ChatDisplayOutput.text == string.Empty)
			{
				this.ChatDisplayOutput.text = text;
			}
			else
			{
				TMP_Text chatDisplayOutput = this.ChatDisplayOutput;
				chatDisplayOutput.text = chatDisplayOutput.text + "\n" + text;
			}
		}
		this.ChatInputField.ActivateInputField();
		this.ChatScrollbar.value = 0f;
	}

	public TMP_InputField ChatInputField;

	public TMP_Text ChatDisplayOutput;

	public Scrollbar ChatScrollbar;
}
