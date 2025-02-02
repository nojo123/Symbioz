﻿using Symbioz.DofusProtocol.Messages;
using Symbioz.DofusProtocol.Types;
using Symbioz.Enums;
using Symbioz.Network.Clients;
using Symbioz.Network.Servers;
using Symbioz.World.Models.Parties.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.World.Models.Parties
{
    public class Party
    {
        public int Id;
        public int BossCharacterId;

        public string Name;

        public int MAX_PARTY_MEMBER_COUNT = 8;

        public List<WorldClient> Members = new List<WorldClient>();
        public List<PartyMember> PMembers = new List<PartyMember>();
        public List<WorldClient> Guests = new List<WorldClient>();
        public List<PartyGuest> PGuests = new List<PartyGuest>();

        public Party(int id, int bossCharacterId, string name)
        {
            this.Id = id;
            this.BossCharacterId = bossCharacterId;
            this.Name = name;
            WorldServer.Instance.Parties.Add(this);
        }

        public void SetName(string name, WorldClient client)
        {
            if(client.Character.Id == this.BossCharacterId)
            {
                this.Name = name;
                foreach(WorldClient c in this.Members)
                {
                    c.Send(new PartyNameUpdateMessage((uint)this.Id, this.Name));
                }
            }
        }

        public int CountMembers()
        {
            return this.Members.Count + this.Guests.Count;
        }

        public void CreateInvitation(WorldClient by, WorldClient to, PartyTypeEnum type = PartyTypeEnum.PARTY_TYPE_CLASSICAL, ushort dungeonId = 0)
        {
            if (to.Character.PartyMember != null && to.Character.PartyMember.Loyal)
            {
                by.Character.Reply("Ce joueur est déjà dans un groupe et ne veut pas recevoir d'autre invitations");
                return;
            }
            this.NewGuest(to);
            if(type == PartyTypeEnum.PARTY_TYPE_DUNGEON)
            {
                to.Send(new PartyInvitationDungeonMessage((uint)this.Id,(sbyte)PartyTypeEnum.PARTY_TYPE_DUNGEON,this.Name,(sbyte)this.MAX_PARTY_MEMBER_COUNT,(uint)by.Character.Id,by.Character.Record.Name,(uint)to.Character.Id,dungeonId));
                return;
            }
            to.Send(new PartyInvitationMessage((uint)this.Id, (sbyte)PartyTypeEnum.PARTY_TYPE_CLASSICAL, by.Character.Record.Name, (sbyte)this.MAX_PARTY_MEMBER_COUNT, (uint)by.Character.Id, by.Character.Record.Name, (uint)to.Character.Record.Id));
        }

        public void AcceptInvitation(WorldClient client)
        {
            if(client.Character.PartyMember != null)
            {
                client.Character.PartyMember.Party.QuitParty(client);
            }
            this.RemoveGuest(client);
            this.NewMember(client);
            if(DungeonPartyProvider.Instance.GetDPCByCharacterId(client.Character.Id) != null)
            {
                List<ushort> dungeonsId = new List<ushort>();
                client.Send(new DungeonPartyFinderRegisterSuccessMessage((IEnumerable<ushort>)dungeonsId));
            }
        }

        public void RefuseInvitation(WorldClient client)
        {
            this.RemoveGuest(client);
            foreach (WorldClient c in this.Members)
            {
                c.Send(new PartyRefuseInvitationNotificationMessage((uint)this.Id, (uint)c.Character.Id));
            }
            if (this.CountMembers() < 2)
            {
                this.Delete();
            }
        }

        public void CancelInvitation(WorldClient by, WorldClient to)
        {
            foreach (WorldClient c in this.Members)
            {
                c.Send(new PartyRefuseInvitationNotificationMessage((uint)this.Id, (uint)to.Character.Id));
            }
            this.RemoveGuest(to);
            to.Send(new PartyInvitationCancelledForGuestMessage((uint)this.Id, (uint)to.Character.Id));
            if (this.CountMembers() < 2)
            {
                this.Delete();
            }
        }

        public void SendInvitationDetail(WorldClient to)
        {
            PartyGuest g = this.PGuests.Find(x => x.Character.Id == to.Character.Id);
            to.Send(new PartyInvitationDetailsMessage((uint)this.Id, (sbyte)PartyTypeEnum.PARTY_TYPE_CLASSICAL, this.Name, (uint)g.InvitedBy.Character.Id, g.InvitedBy.Character.Record.Name, (uint)this.BossCharacterId,
                from members in this.PMembers
                select members.GetPartyInvitationMemberInformations(),
                from guests in this.PGuests
                select guests.GetPartyGuestInformations()));
        }

        public void QuitParty(WorldClient client)
        {
            this.RemoveMember(client);
            foreach(WorldClient c in this.Members)
            {
                c.Send(new PartyMemberRemoveMessage((uint)this.Id, (uint)client.Character.Id));
            }
            if (client.Character.Id == this.BossCharacterId)
            {
                this.ChangeLeader();
            }
            client.Send(new PartyLeaveMessage((uint)this.Id));
            if (this.CountMembers() < 2)
            {
                this.Delete();
            }
        }

        public void PlayerKick(int playerId, WorldClient sender)
        {
            if(sender.Character.Id == this.BossCharacterId)
            {
                WorldClient client = WorldServer.Instance.WorldClients.Find(x => x.Character.Id == playerId);
                if (this.Members.Contains(client))
                {
                    this.RemoveMember(client);
                    client.Send(new PartyKickedByMessage((uint)this.Id, (uint)sender.Character.Id));
                    foreach(WorldClient c in this.Members)
                    {
                        c.Send(new PartyMemberRemoveMessage((uint)this.Id,(uint)client.Character.Id));
                    }
                    if(this.CountMembers() < 2)
                    {
                        this.Delete();
                    }
                }
            }
        }

        public void ChangeLeader(int newLeader = 0)
        {
            if (newLeader == 0)
            {
                this.BossCharacterId = this.PMembers.First().C.Id;
            }
            else
            {
                this.BossCharacterId = this.PMembers.Find(x => x.C.Id == newLeader).C.Id;
            }
            foreach(WorldClient client in Members)
            {
                client.Send(new PartyLeaderUpdateMessage((uint)this.Id, (uint) this.BossCharacterId));
            }
        }

        public PartyMemberInformations GetPartyMemberInformations(int id)
        {
            PartyMember m = this.PMembers.Find(x => x.C.Id == id);
            return m.GetPartyMemberInformations();
        }
        public PartyGuestInformations GetPartyGuestInformations(int id)
        {
            PartyGuest m = this.PGuests.Find(x => x.Character.Id == id);
            return m.GetPartyGuestInformations();
        }

        public void Delete()
        {
            foreach(WorldClient client in this.Members)
            {
                client.Send(new PartyDeletedMessage((uint)this.Id));
                client.Character.PartyMember = null;
            }
            foreach (WorldClient client in this.Guests)
            {
                client.Send(new PartyDeletedMessage((uint)this.Id));
                client.Character.PartyMember = null;
            }
            WorldServer.Instance.Parties.Remove(this);
        }

        public void NewMember(WorldClient c)
        {
            if(this.Members.Count + this.Guests.Count < this.MAX_PARTY_MEMBER_COUNT)
            {
                PartyMember m = new PartyMember(c.Character, this);
                foreach (WorldClient clients in this.Members)
                {
                    clients.Send(new PartyNewMemberMessage((uint)this.Id, m.GetPartyMemberInformations()));
                }
                this.Members.Add(c);
                this.PMembers.Add(m);
                c.Character.PartyMember = m;
                c.Send(new PartyJoinMessage((uint)this.Id, (sbyte)PartyTypeEnum.PARTY_TYPE_CLASSICAL, (uint)this.BossCharacterId, (sbyte)this.MAX_PARTY_MEMBER_COUNT,
                   from members in this.PMembers
                   select members.GetPartyMemberInformations(),
                   from guests in this.PGuests
                   select guests.GetPartyGuestInformations(),
                   false, this.Name));
            }
        }

        public void NewGuest(WorldClient c)
        {
            if (this.CountMembers() < this.MAX_PARTY_MEMBER_COUNT)
            {
                this.Guests.Add(c);
                PartyGuest g = new PartyGuest(c.Character, this, c);
                this.PGuests.Add(g);
                foreach(WorldClient clients in this.Members)
                {
                    clients.Send(new PartyNewGuestMessage((uint)this.Id, g.GetPartyGuestInformations()));
                }
            }
        }

        public void RemoveMember(WorldClient c)
        {
            this.Members.Remove(c);
            this.PMembers.Remove(c.Character.PartyMember);
        }

        public void RemoveGuest(WorldClient c)
        {
            this.Guests.Remove(c);
            this.PGuests.Remove(this.PGuests.Find(x=>x.Character == c.Character));
        }
    }
}
