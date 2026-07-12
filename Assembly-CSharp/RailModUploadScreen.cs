using System;
using KSerialization;
using TMPro;
using UnityEngine;

public class RailModUploadScreen : KModalScreen
{
	[SerializeField]
	private KButton[] closeButtons;

	[SerializeField]
	private KButton submitButton;

	[SerializeField]
	private ToolTip submitButtonTooltip;

	[SerializeField]
	private TMP_InputField modName;

	[SerializeField]
	private TMP_InputField modDesc;

	[SerializeField]
	private TMP_InputField modVersion;

	[SerializeField]
	private TMP_InputField contentFolder;

	[SerializeField]
	private TMP_InputField previewImage;

	[SerializeField]
	private MultiToggle[] shareTypeToggles;

	[Serialize]
	private string previousFolderPath;
}
