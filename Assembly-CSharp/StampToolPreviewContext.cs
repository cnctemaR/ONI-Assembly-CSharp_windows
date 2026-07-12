using System;
using UnityEngine;

public class StampToolPreviewContext
{
	public Transform previewParent;

	public InterfaceTool tool;

	public TemplateContainer stampTemplate;

	public global::System.Action frameAfterSetupFn;

	public Action<int> refreshFn;

	public global::System.Action onPlaceFn;

	public Action<string> onErrorChangeFn;

	public global::System.Action cleanupFn;
}
