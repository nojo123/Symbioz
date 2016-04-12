using Symbioz.DofusProtocol.Types;
using Symbioz.Enums;
using Symbioz.World.PathProvider;
using Symbioz.World.Records;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.World.Models.Parties
{
    public class PartyMember
    {
        public Character C;
        public Party Party;
        public PartyMember(Character c, Party p)
        {
            this.Party = p;
            this.C = c;
        }
        public PartyMemberInformations GetPartyMemberInformations()
        {
            CharacterRecord record = this.C.Record;
            StatsRecord stats = this.C.StatsRecord;
            BasicStats current = this.C.CurrentStats;
            int id = record.Id;
            int level = (int)record.Level;
            string name = record.Name;
            EntityLook entityLook = this.C.Look.ToEntityLook();
            int breed = (int)(sbyte)record.Breed;
            bool sex = record.Sex;
            int hp = (int)this.C.CurrentStats.LifePoints;
            int maxhp = stats.LifePoints;
            int regen = (int)1;
            int align = (int)(sbyte)record.AlignmentSide;
            Point position = new Point(0,0);
            int mapid = (int)(short)record.MapId;
            PlayerStatus status = new PlayerStatus((sbyte)0);
            PartyCompanionMemberInformations[] memberInformationsArray = new PartyCompanionMemberInformations[0];
            return new PartyMemberInformations((uint)id, (byte)level, name, entityLook, (sbyte)breed, sex, (uint)hp, (uint)maxhp, (ushort)stats.Prospecting, (byte)regen, (ushort)stats.Initiative, (sbyte)align, (short)0, (short)0, this.C.Map.Id, (ushort)this.C.SubAreaId, status, (IEnumerable<PartyCompanionMemberInformations>)memberInformationsArray);
        }
    }
}
