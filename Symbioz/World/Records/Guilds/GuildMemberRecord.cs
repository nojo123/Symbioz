using Symbioz.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symbioz.World.Records.Guilds
{
    [Table("Guilds_Members")]
    class GuildMemberRecord : ITable
    {
        static ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        public static List<GuildMemberRecord> Guilds_Members = new List<GuildMemberRecord>();

        public int CharacterId;
        public int GuildId;

        public GuildMemberRecord(int characterId, int guildId)
        {
            this.CharacterId = characterId;
            this.GuildId = guildId;
        }
        public static bool CanJoinGuild(int characterId)
        {
            return Guilds_Members.Find(x => x.CharacterId == characterId) == null;
        }
    }
}
