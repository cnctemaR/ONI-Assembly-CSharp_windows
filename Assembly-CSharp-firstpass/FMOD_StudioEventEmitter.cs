using System;
using UnityEngine;

[AddComponentMenu("")]
public class FMOD_StudioEventEmitter : MonoBehaviour
{
	public FMODAsset asset;

	public string path = string.Empty;

	public bool startEventOnAwake = true;
}
