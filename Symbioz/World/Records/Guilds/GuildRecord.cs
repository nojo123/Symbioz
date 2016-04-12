using Symbioz.DofusProtocol.Types;
using Symbioz.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symbioz.World.Records.Guilds
{
    [Table("Guilds")]
    public class GuildRecord : ITable
    {
        static ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        public static List<GuildRecord> Guilds = new List<GuildRecord>();

        public int Id;

        public string Name;

        public ushort SymbolShape;

        public int SymbolColor;

        public sbyte BackgroundShape;

        public int BackgroundColor;

        public GuildRecord(int id, string name, GuildEmblem emblem)
        {
            this.Id = id;
            this.Name = name;
            this.SymbolShape = emblem.symbolShape;
            this.SymbolColor = emblem.symbolColor;
            this.BackgroundShape = emblem.backgroundShape;
            this.BackgroundColor = emblem.backgroundColor;
        }

        public static GuildEmblem GetGuildEmblem(int guildId)
        {
            GuildRecord guild = Guilds.Find(x => x.Id == guildId);
            return new GuildEmblem(guild.SymbolShape, guild.SymbolColor, guild.BackgroundShape, guild.BackgroundColor);
        }

        public static GuildRecord GetGuild(int id)
        {
            return Guilds.Find(x => x.Id == id);
        }
        public static bool CanCreateGuild(string guildName)
        {
            return Guilds.Find(x => x.Name == guildName) == null;
        }
        public static int PopNextId()
        {
            Locker.EnterReadLock();
            try
            {
                var ids = Guilds.ConvertAll<int>(x => x.Id);
                ids.Sort();
                return ids.Count == 0 ? 1 : ids.Last() + 1;
            }
            finally
            {
                Locker.ExitReadLock();
            }
        }

    }
}