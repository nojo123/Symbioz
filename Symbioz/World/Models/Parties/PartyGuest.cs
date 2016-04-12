using Symbioz.DofusProtocol.Types;
using Symbioz.Network.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.World.Models.Parties
{
    public class PartyGuest
    {
        public Character Character;
        public Party Party;
        public WorldClient InvitedBy;
        public PartyGuest(Character c, Party p, WorldClient i)
        {
            this.Party = p;
            this.Character = c;
            this.InvitedBy = i;
        }

        public PartyGuestInformations GetPartyGuestInformations()
        {
            Character c = this.Character;
            PartyCompanionMemberInformations[] memberInformationsArray = new PartyCompanionMemberInformations[0];
            return new PartyGuestInformations(c.Id, this.Party.Id, c.Record.Name, c.Look.ToEntityLook(), c.Record.Breed, c.Record.Sex, new PlayerStatus((sbyte)0), (IEnumerable<PartyCompanionMemberInformations>)memberInformationsArray);
        }
    }
}
