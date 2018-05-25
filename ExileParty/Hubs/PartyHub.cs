﻿using System;
using System.Threading.Tasks;
using ExileParty.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ExileParty.Helper;
using System.Linq;

namespace ExileParty.Hubs
{
    [EnableCors("AllowAll")]
    public class PartyHub : Hub
    {
        private IDistributedCache _cache;

        public PartyHub(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task JoinParty(string partyName, PlayerModel player)
        {
            // set initial id of player
            player.ConnectionID = Context.ConnectionId;

            // look for party
            var foundParty = await _cache.GetAsync<PartyModel>(partyName);
            if (foundParty == null)
            {
                var party = new PartyModel() { Name = partyName, Players = new List<PlayerModel> { player } };
                await _cache.SetAsync<PartyModel>(partyName, party);
                await Clients.Caller.SendAsync("EnteredParty", party, player);
            } else {
                foundParty.Players.Add(player);
                await _cache.SetAsync<PartyModel>(partyName, foundParty);
                await Clients.Caller.SendAsync("EnteredParty", foundParty, player);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, partyName);
            await Clients.OthersInGroup(partyName).SendAsync("PlayerJoined", player);
            await Clients.Group(partyName).SendAsync("PlayerUpdated", player);
        }

        public async Task LeaveParty(string partyName, PlayerModel player)
        {
            var foundParty = await _cache.GetAsync<PartyModel>(partyName);
            if (foundParty != null)
            {
                //var foundPlayer = foundParty.Players.FirstOrDefault(x => x.ConnectionID == player.ConnectionID);
                foundParty.Players.Remove(player);
                await _cache.SetAsync<PartyModel>(partyName, foundParty);
            }

            await Clients.OthersInGroup(partyName).SendAsync("PlayerLeft", player);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, partyName);
        }
    }
}