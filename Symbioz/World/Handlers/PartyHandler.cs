using Symbioz.DofusProtocol.Messages;
using Symbioz.DofusProtocol.Types;
using Symbioz.Enums;
using Symbioz.Network.Clients;
using Symbioz.Network.Messages;
using Symbioz.Network.Servers;
using Symbioz.World.Models.Parties;
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
            Party p;
            if (client.Character.PartyMember == null)
            {
                p = new Party(WorldServer.Instance.Parties.Count + 1, client.Character.Id, "");
            }
            else
            {
                p = WorldServer.Instance.Parties.Find(x => x.Id == client.Character.PartyMember.Party.Id);
            }
            WorldClient to = WorldServer.Instance.WorldClients.Find(x => x.Character.Record.Name == message.name);
            p.CreateInvitation(client, to);
            if (p.Members.Count == 0)
            {
                p.BossCharacterId = client.Character.Id;
                p.NewMember(client);
            }
            
        }
        [MessageHandler]
        public static void PartyAcceptInvitation(PartyAcceptInvitationMessage message, WorldClient client)
        {
            Party p = WorldServer.Instance.GetPartyById((int)message.partyId);
            p.AcceptInvitation(client);
        }
        [MessageHandler]
        public static void PartyRefusedInvitation(PartyRefuseInvitationMessage message, WorldClient client)
        {
            Party p = WorldServer.Instance.GetPartyById((int)message.partyId);
            p.RefuseInvitation(client);
        }
        [MessageHandler]
        public static void PartyCancelInvitation(PartyCancelInvitationMessage message, WorldClient client)
        {
            Party p = WorldServer.Instance.GetPartyById((int)message.partyId);
            p.CancelInvitation(client, WorldServer.Instance.WorldClients.Find(x => x.Character.Id == message.guestId));
        }
        [MessageHandler]
        public static void PartyLeaveRequest(PartyLeaveRequestMessage message, WorldClient client)
        {
            Party p = WorldServer.Instance.Parties.Find(x => x.Id == message.partyId);
            p.QuitParty(client);
        }
        [MessageHandler]
        public static void PartyAbdicateRequest(PartyAbdicateThroneMessage message, WorldClient client)
        {
            Party p = WorldServer.Instance.Parties.Find(x => x.Id == message.partyId);
            if (p.BossCharacterId == client.Character.Id)
            {
                p.ChangeLeader((int)message.playerId);
            }
            else
            {
                client.Character.Reply("Vous devez être chef de groupe pour nommer votre successeur");
            }
        }
        [MessageHandler]
        public static void PartyKickRequest(PartyKickRequestMessage message, WorldClient client)
        {
            Party p = WorldServer.Instance.Parties.Find(x => x.Id == message.partyId);
            p.PlayerKick((int)message.playerId, client);
        }
        [MessageHandler]
        public static void PartyGetInvitationDetailsRequestRequest(PartyInvitationDetailsRequestMessage message, WorldClient client)
        {
            Party p = WorldServer.Instance.Parties.Find(x => x.Id == message.partyId);
            p.SendInvitationDetail(client);
        }
    }
}
