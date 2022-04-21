using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class RoomPlayer : NetworkBehaviour
{
	public enum EGameState
	{
		Lobby,
		GameCutscene,
		GameReady
	}

	public static readonly List<RoomPlayer> Players = new List<RoomPlayer>();

	public static Action<RoomPlayer> PlayerJoined;
	public static Action<RoomPlayer> PlayerLeft;
	public static Action<RoomPlayer> PlayerChanged;

	public static RoomPlayer Local;

	[Networked(OnChanged = nameof(OnStateChanged))] public NetworkBool IsReady { get; set; }
	[Networked(OnChanged = nameof(OnStateChanged))] public string Username { get; set; }
	[Networked] public NetworkBool HasFinished { get; set; }
	[Networked] public KartController Kart { get; set; }
	[Networked] public EGameState GameState { get; set; }
	[Networked] public int KartId { get; set; }

	public bool IsLeader => Object!=null && Object.IsValid && Object.HasStateAuthority;

	public override void Spawned()
	{
		base.Spawned();

		if (Object.HasInputAuthority)
		{
			Local = this;

			PlayerChanged?.Invoke(this);
			RPC_SetPlayerStats(ClientInfo.Username, ClientInfo.KartId);
		}

		Players.Add(this);
		PlayerJoined?.Invoke(this);

		DontDestroyOnLoad(gameObject);
	}

	[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, InvokeResim = true)]
	private void RPC_SetPlayerStats(string username, int kartId)
	{
		Username = username;
		KartId = kartId;
	}

	[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
	public void RPC_SetKartId(int id)
	{
		KartId = id;
	}

	[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
	public void RPC_ChangeReadyState(NetworkBool state)
	{
		Debug.Log($"Setting {Object.Name} ready state to {state}");
		IsReady = state;
	}

	private void OnDisable()
	{
		// OnDestroy does not get called for pooled objects
		PlayerLeft?.Invoke(this);
		Players.Remove(this);
	}

	private static void OnStateChanged(Changed<RoomPlayer> changed) => PlayerChanged?.Invoke(changed.Behaviour);

	public static void RemovePlayer(NetworkRunner runner, PlayerRef p)
	{
		var roomPlayer = Players.FirstOrDefault(x => x.Object.InputAuthority == p);
		if (roomPlayer != null)
		{
			if (roomPlayer.Kart != null)
				runner.Despawn(roomPlayer.Kart.Object);

			Players.Remove(roomPlayer);
			runner.Despawn(roomPlayer.Object);
		}
	}
}