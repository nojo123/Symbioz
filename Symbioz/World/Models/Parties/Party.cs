using Symbioz.DofusProtocol.Messages;
using Symbioz.DofusProtocol.Types;
using Symbioz.Enums;
using Symbioz.Network.Clients;
using Symbioz.Network.Servers;
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

        public int MaxPartyMembers = 8;

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

        public int CountMembers()
        {
            return this.Members.Count + this.Guests.Count;
        }

        public void CreateInvitation(WorldClient by, WorldClient to)
        {
            this.NewGuest(to);
            to.Send(new PartyInvitationMessage((uint)this.Id, (sbyte)PartyTypeEnum.PARTY_TYPE_CLASSICAL, by.Character.Record.Name, (sbyte)this.MaxPartyMembers, (uint)by.Character.Id, by.Character.Record.Name, (uint)to.Character.Record.Id));
            this.SaveToWorldServer();
        }

        public void AcceptInvitation(WorldClient client)
        {
            this.RemoveGuest(client);
            this.NewMember(client);
            this.SaveToWorldServer();
        }

        public void RefuseInvitation(WorldClient client)
        {
            this.RemoveGuest(client);
            if(this.CountMembers() < 2)
            {
                this.Delete();
            }
            this.SaveToWorldServer();
        }

        public void CancelInvitation(WorldClient by, WorldClient to)
        {
            this.RemoveGuest(to);
            to.Send(new PartyCancelInvitationMessage((uint)this.Id, (uint)to.Character.Id));
            foreach (WorldClient c in this.Members)
            {
                c.Send(new PartyCancelInvitationMessage((uint)this.Id, (uint)to.Character.Id));
                c.Send(new PartyCancelInvitationNotificationMessage((uint)this.Id, (uint)by.Character.Id, (uint) to.Character.Id));
            }
            if (this.Members.Count + this.Guests.Count < 2)
            {
                this.Delete();
            }
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
            this.SaveToWorldServer();
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
            this.SaveToWorldServer();
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
                client.Send(new PartyLeaveMessage((uint)this.Id));
                client.Character.PartyMember = null;
            }
            foreach (WorldClient client in this.Guests)
            {
                client.Send(new PartyLeaveMessage((uint)this.Id));
                client.Character.PartyMember = null;
            }
            WorldServer.Instance.Parties.Remove(this);
        }

        public void SaveToWorldServer()
        {
            WorldServer.Instance.Parties.Remove(this);
            WorldServer.Instance.Parties.Add(this);
        }

        public void NewMember(WorldClient c)
        {
            if(this.Members.Count + this.Guests.Count < this.MaxPartyMembers)
            {
                PartyMember m = new PartyMember(c.Character, this);
                foreach (WorldClient clients in this.Members)
                {
                    clients.Send(new PartyNewMemberMessage((uint)this.Id, m.GetPartyMemberInformations()));
                }
                this.Members.Add(c);
                this.PMembers.Add(m);
                c.Character.PartyMember = m;
                c.Send(new PartyJoinMessage((uint)this.Id, (sbyte)PartyTypeEnum.PARTY_TYPE_CLASSICAL, (uint)c.Character.Id, (sbyte)this.MaxPartyMembers,
                   from members in this.PMembers
                   select members.GetPartyMemberInformations(),
                   from guests in this.PGuests
                   select guests.GetPartyGuestInformations(),
                   false, this.Name));
                this.SaveToWorldServer();
            }
        }

        public void NewGuest(WorldClient c)
        {
            if (this.CountMembers() < this.MaxPartyMembers)
            {
                this.Guests.Add(c);
                PartyGuest g = new PartyGuest(c.Character, this, c);
                this.PGuests.Add(g);
                this.SaveToWorldServer();
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
            this.SaveToWorldServer();
        }

        public void RemoveGuest(WorldClient c)
        {
            this.Guests.Remove(c);
            this.PGuests.Remove(this.PGuests.Find(x=>x.Character == c.Character));
            this.SaveToWorldServer();
        }
    }
}
