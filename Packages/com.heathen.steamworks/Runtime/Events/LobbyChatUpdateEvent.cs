﻿#if !DISABLESTEAMWORKS  && STEAMWORKSNET
using Steamworks;
using UnityEngine.Events;

namespace Heathen.SteamworksIntegration
{
    [System.Serializable]
    public class LobbyChatUpdateEvent : UnityEvent<LobbyChatUpdate_t> { }
}
#endif