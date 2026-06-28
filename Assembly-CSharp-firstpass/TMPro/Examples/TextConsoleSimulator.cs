using System;
using System.Collections;
using UnityEngine;

namespace TMPro.Examples
{
	public class TextConsoleSimulator : MonoBehaviour
	{
		private void Awake()
		{
			this.m_TextComponent = base.gameObject.GetComponent<TextMeshPro>();
			if (this.m_TextComponent == null)
			{
				this.m_TextComponent = base.gameObject.GetComponent<TextMeshProUGUI>();
			}
			if (this.m_TextComponent as TextMeshPro != null)
			{
				this.m_textObjectType = TextConsoleSimulator.objectType.TextMeshPro;
			}
			else if (this.m_TextComponent as TextMeshProUGUI != null)
			{
				this.m_textObjectType = TextConsoleSimulator.objectType.TextMeshProUI;
			}
			else
			{
				this.m_textObjectType = TextConsoleSimulator.objectType.None;
			}
		}

		private void Start()
		{
			TextConsoleSimulator.objectType textObjectType = this.m_textObjectType;
			if (textObjectType != TextConsoleSimulator.objectType.TextMeshPro)
			{
				if (textObjectType == TextConsoleSimulator.objectType.TextMeshProUI)
				{
					base.StartCoroutine(this.RevealCharacters(this.m_TextComponent as TextMeshProUGUI));
				}
			}
			else
			{
				base.StartCoroutine(this.RevealCharacters(this.m_TextComponent as TextMeshPro));
			}
		}

		private IEnumerator RevealCharacters(TextMeshPro textComponent)
		{
			textComponent.ForceMeshUpdate();
			TMP_TextInfo textInfo = textComponent.textInfo;
			int totalVisibleCharacters = textInfo.characterCount;
			int counter = 0;
			int visibleCount = 0;
			for (;;)
			{
				visibleCount = counter % (totalVisibleCharacters + 1);
				textComponent.maxVisibleCharacters = visibleCount;
				if (visibleCount >= totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1f);
				}
				counter++;
				yield return new WaitForSeconds(0f);
			}
			yield break;
		}

		private IEnumerator RevealCharacters(TextMeshProUGUI textComponent)
		{
			textComponent.ForceMeshUpdate();
			TMP_TextInfo textInfo = textComponent.textInfo;
			int totalVisibleCharacters = textInfo.characterCount;
			int counter = 0;
			int visibleCount = 0;
			for (;;)
			{
				visibleCount = counter % (totalVisibleCharacters + 1);
				textComponent.maxVisibleCharacters = visibleCount;
				if (visibleCount >= totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1f);
				}
				counter++;
				yield return new WaitForSeconds(0f);
			}
			yield break;
		}

		private IEnumerator RevealWords(TextMeshPro textComponent)
		{
			textComponent.ForceMeshUpdate();
			int totalWordCount = textComponent.textInfo.wordCount;
			int totalVisibleCharacters = textComponent.textInfo.characterCount;
			int counter = 0;
			int currentWord = 0;
			int visibleCount = 0;
			for (;;)
			{
				currentWord = counter % (totalWordCount + 1);
				if (currentWord == 0)
				{
					visibleCount = 0;
				}
				else if (currentWord < totalWordCount)
				{
					visibleCount = textComponent.textInfo.wordInfo[currentWord - 1].lastCharacterIndex + 1;
				}
				else if (currentWord == totalWordCount)
				{
					visibleCount = totalVisibleCharacters;
				}
				textComponent.maxVisibleCharacters = visibleCount;
				if (visibleCount >= totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1f);
				}
				counter++;
				yield return new WaitForSeconds(0.1f);
			}
			yield break;
		}

		private IEnumerator RevealWords(TextMeshProUGUI textComponent)
		{
			textComponent.ForceMeshUpdate();
			int totalWordCount = textComponent.textInfo.wordCount;
			int totalVisibleCharacters = textComponent.textInfo.characterCount;
			int counter = 0;
			int currentWord = 0;
			int visibleCount = 0;
			for (;;)
			{
				currentWord = counter % (totalWordCount + 1);
				if (currentWord == 0)
				{
					visibleCount = 0;
				}
				else if (currentWord < totalWordCount)
				{
					visibleCount = textComponent.textInfo.wordInfo[currentWord - 1].lastCharacterIndex + 1;
				}
				else if (currentWord == totalWordCount)
				{
					visibleCount = totalVisibleCharacters;
				}
				textComponent.maxVisibleCharacters = visibleCount;
				if (visibleCount >= totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1f);
				}
				counter++;
				yield return new WaitForSeconds(0.1f);
			}
			yield break;
		}

		private TextConsoleSimulator.objectType m_textObjectType;

		private global::UnityEngine.Object m_TextComponent;

		private enum objectType
		{
			None,
			TextMeshPro,
			TextMeshProUI
		}
	}
}
