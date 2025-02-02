﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbioz.Core;
using Symbioz.SSync;
using System.Threading.Tasks;
using System.Net.Sockets;
using Symbioz.Network.Clients;
using Symbioz.Utils;
using Symbioz.Helper;
using Symbioz.Enums;
using Symbioz.DofusProtocol.Messages;
using Symbioz.DofusProtocol.Types;
using Symbioz.World.Records;
using Symbioz.World.Models;
using Symbioz.World.Models.Parties;

namespace Symbioz.Network.Servers
{
    public class WorldServer : Singleton<WorldServer>
    {
        public int InstanceMaxConnected = 0;
        public ServerStatusEnum ServerState = ServerStatusEnum.ONLINE;
        public List<WorldClient> WorldClients = new List<WorldClient>();
        public List<Party> Parties = new List<Party>();
        public SSyncServer Server { get; set; }
        public WorldServer()
        {
            this.Server = new SSyncServer(ConfigurationManager.Instance.Host, ConfigurationManager.Instance.WorldPort);
            this.Server.OnServerStarted += Server_OnServerStarted;
            this.Server.OnServerFailedToStart += Server_OnServerFailedToStart;
            this.Server.OnSocketAccepted += Server_OnSocketAccepted;
           
        }
        void Server_OnSocketAccepted(Socket socket)
        {
            Logger.World("New client connected!");
            WorldClients.Add(new WorldClient(socket));
            if (WorldClients.Count > InstanceMaxConnected)
                InstanceMaxConnected = WorldClients.Count;
        }

        void Server_OnServerFailedToStart(Exception ex)
        {
            Logger.Error("Unable to start WorldServer! : (" + ex.Message + ")");
        }
        
        void Server_OnServerStarted()
        {
            Logger.World("Server Started (" + Server.EndPoint.AsIpString() + ")");
        }
        public void Start()
        {
            Server.Start();
        }
        public void SetServerState(ServerStatusEnum state)
        {
            ServerState = state;
            foreach (var client in AuthServer.Instance.AuthClients)
            {
                var count = CharacterRecord.GetAccountCharacters(client.Account.Id).Count();
                var servers = new List<GameServerInformations>();
                servers.Add(new GameServerInformations((ushort)ConfigurationManager.Instance.ServerId, (sbyte)WorldServer.Instance.ServerState, 0, true, (sbyte)count, 1));
                client.Send(new ServersListMessage(servers, 0, true));
            }
        }
        public void Send(Message message)
        {
            WorldClients.ForEach(x => x.Send(message));
        }
        public void SendToOnlineCharacters(Message message)
        {
            GetAllClientsOnline().ForEach(x => x.Send(message));
        }
        public void SendOnSubarea(Message message,int subareaid)
        {
            GetAllClientsOnline().FindAll(x => x.Character.SubAreaId == subareaid).ForEach(x=>x.Send(message));
        }
        public void RemoveClient(WorldClient client)
        {
            WorldClients.Remove(client);
            Logger.World("Client Disconnected!");
        }
        public List<WorldClient> GetAllClientsOnline()
        {
            return WorldClients.FindAll(x => x.Character != null);
        }
        public WorldClient GetOnlineClient(int characterid)
        {
            return GetAllClientsOnline().Find(x => x.Character.Id == characterid);
        }
        public WorldClient GetOnlineClient(string characterName)
        {
            return GetAllClientsOnline().Find(x => x.Character.Record.Name == characterName);
        }
        public Party GetPartyById(int partyId)
        {
            return this.Parties.Find(x => x.Id == partyId);
        }
        public bool IsConnected(string characterName)
        {
            return GetOnlineClient(characterName) != null;
        }
        public bool IsConnected(int characterId)
        {
            return GetOnlineClient(characterId) != null;
        }

    }
}
