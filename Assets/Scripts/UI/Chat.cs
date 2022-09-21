using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class Chat : NetworkBehaviour
{
	public static Chat Instance {get; private set;}
	
	[SerializeField] ScrollRect sRect;
	
	[HideInInspector]
	public PlayerName PN;
	
	[SerializeField] private GameObject ChatBox;
    [SerializeField] private TMP_Text txt;
	[SerializeField] private InputField iField;
	
	[HideInInspector] public Canvas canvas;

	private void Awake()
	{
        if(Instance == null) Instance = this;
		else
		{
			Destroy(Instance);
			Instance = this;
		}
	}
	
	private void Start()
	{
		canvas = GetComponent<Canvas>();
	}

	/*public override void OnStartAuthority()
	{
		canvas = GetComponent<Canvas>();
		ChatBox.SetActive(true);
	}*/
	
	private void HandleNewMessage(string msg, string name)
	{
		//if(txt.isTextOverflowing) txt.text = "";

		string s = name != null ? name : "meucu";
		txt.text += "\n" + s + ": "+ msg;
		Canvas.ForceUpdateCanvases();
		sRect.verticalNormalizedPosition = 0f;
	}
	
	void Update()
	{
		if(Input.GetButtonDown("Cancel")) ChangeCanvasEnabled();
	}

	public void ChangeCanvasEnabled()
	{
		canvas.enabled = !canvas.enabled;
	}

	[Client]
	public void Send(string msg)
	{
		if(!Input.GetKeyDown(KeyCode.Return)) return;
		if(string.IsNullOrWhiteSpace(msg)) msg = iField.text;
		
		if(!isServer) Cmd_SendMessage(iField.text, PN.nickname); else SrvSendMessage(iField.text);
		
		iField.text = string.Empty;
	}
	
	[Command(requiresAuthority = false)]
	private void Cmd_SendMessage(string msg, string name)
	{
		Rpc_HandleMessage(msg, name);
	}

	private void SrvSendMessage(string msg)
	{
		Rpc_HandleMessage(msg, PN.nickname);
	}
	
	[ClientRpc]
	private void Rpc_HandleMessage(string msg, string name)
	{
		HandleNewMessage(msg, name);
	}
}
