using Symbioz.DofusProtocol.Messages;
using Symbioz.DofusProtocol.Types;
using Symbioz.Enums;
using Symbioz.Network.Clients;
using Symbioz.Network.Messages;
using Symbioz.Network.Servers;
using Symbioz.World.Models.Party;
using Symbioz.World.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.World.Handlers
{
    class PartyHandler
    {
        [MessageHandler]
        public static void RequestParty(PartyInvitationRequestMessage message, WorldClient client)
        {
            Party p = new Party(WorldServer.Instance.Parties.Count + 1, client.Character.Id, "");
            WorldClient to = WorldServer.Instance.GetAllClientsOnline().Find(x => x.Character.Record.Name == message.name);
            p.NewGuest(to);
            p.NewMember(client);
            client.Send(new PartyJoinMessage((uint)p.Id,(sbyte)PartyTypeEnum.PARTY_TYPE_CLASSICAL,(uint)client.Character.Id,(sbyte)p.MaxPartyMembers,
                from entry in p.PMembers
                select entry.GetPartyMemberInformations(),
                from entry in p.PGuest
                select entry.GetPartyGuestInformations(),
                false, p.Name));
            to.Send(new PartyInvitationMessage(1,(sbyte)PartyTypeEnum.PARTY_TYPE_CLASSICAL,client.Character.Record.Name,8,(uint)client.Character.Id,client.Character.Record.Name,(uint)to.Character.Record.Id));
        }
        [MessageHandler]
        public static void PartyAcceptInvitation(PartyAcceptInvitationMessage message, WorldClient client)
        {
            Party p = WorldServer.Instance.GetPartyById((int)message.partyId);
            p.NewMember(client);
            p.RemoveGuest(client);
            foreach (WorldClient c in p.Members)
            {
                c.Send(new PartyJoinMessage((uint)p.Id, (sbyte)PartyTypeEnum.PARTY_TYPE_CLASSICAL, (uint)p.BossCharacterId, (sbyte)p.MaxPartyMembers,
                from entry in p.PMembers
                select entry.GetPartyMemberInformations(),
                from entry in p.PGuest
                select entry.GetPartyGuestInformations(),
                false, p.Name));
            }
        }
        [MessageHandler]
        public static void PartyLeaveRequest(PartyLeaveRequestMessage message, WorldClient client)
        {
            Party p = WorldServer.Instance.Parties.Find(x => x.Id == message.partyId);
            if(p.Members.Count+p.Guest.Count <= 2)
            {
                p.Delete();
            }
        }
    }
}
