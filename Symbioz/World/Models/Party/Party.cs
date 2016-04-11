using Symbioz.DofusProtocol.Messages;
using Symbioz.DofusProtocol.Types;
using Symbioz.Network.Clients;
using Symbioz.Network.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.World.Models.Party
{
    public class Party
    {
        public int Id;
        public int BossCharacterId;

        public string Name;

        public int MaxPartyMembers = 8;

        public List<WorldClient> Members = new List<WorldClient>();
        public List<PartyMember> PMembers = new List<PartyMember>();
        public List<WorldClient> Guest = new List<WorldClient>();
        public List<PartyMember> PGuest = new List<PartyMember>();

        public Party(int id, int bossCharacterId, string name)
        {
            this.Id = id;
            this.BossCharacterId = bossCharacterId;
            this.Name = name;
            WorldServer.Instance.Parties.Add(this);
        }

        public PartyMemberInformations GetPartyMemberInformations(int id)
        {
            PartyMember m = this.PMembers.Find(x => x.C.Id == id);
            return m.GetPartyMemberInformations();
        }
        public PartyGuestInformations GetPartyGuestInformations(int id)
        {
            PartyMember m = this.PGuest.Find(x => x.C.Id == id);
            return m.GetPartyGuestInformations();
        }

        public void Delete()
        {
            foreach(WorldClient client in this.Members)
            {
                client.Send(new PartyLeaveMessage((uint)this.Id));
                client.Character.PartyMember = null;
            }
            foreach (WorldClient client in this.Guest)
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
            if(this.Members.Count + this.Guest.Count < this.MaxPartyMembers)
            {
                this.Members.Add(c);
                this.PMembers.Add(new PartyMember(c.Character, this));
                this.SaveToWorldServer();
            }
        }

        public void NewGuest(WorldClient c)
        {
            if (this.Members.Count + this.Guest.Count < this.MaxPartyMembers)
            {
                this.Guest.Add(c);
                c.Character.PartyMember = new PartyMember(c.Character, this);
                this.PGuest.Add(c.Character.PartyMember);
                this.SaveToWorldServer();
            }
        }

        public void RemoveGuest(WorldClient c)
        {
            this.Guest.Remove(c);
            this.PGuest.Remove(c.Character.PartyMember);
            this.SaveToWorldServer();
        }
    }
}
