namespace AbsoluteAlexander_MaliktenderOnly
{
    using System.Diagnostics;
    using System.Collections.Generic;
    using Advanced_Combat_Tracker;
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using FFXIV_ACT_Plugin.Common;

    public class ActHelper
    {
        private static object lockObject = new object();
        private static dynamic plugin;
        private static IDataRepository DataRepository { get; set; }
        public static IDataSubscription DataSubscription { get; set; }
        public PrimaryPlayerDelegate OnPrimaryPlayerChanged { get; set; }
        public PartyListChangedDelegate OnPartyListChanged { get; set; }
        public ZoneChangedDelegate OnZoneChanged { get; set; }
        public CombatantAddedDelegate CombatantAdded { get; set; }


        public static bool Initialize()
        {

            lock (lockObject)
            {
                if (!ActGlobals.oFormActMain.Visible)
                {
                    return false;
                }

                var ffxivPlugin = (
                    from x in ActGlobals.oFormActMain.ActPlugins
                    where
                    x.pluginFile.Name.ToUpper().Contains("FFXIV_ACT_Plugin".ToUpper()) &&
                    x.lblPluginStatus.Text.ToUpper().Contains("FFXIV Plugin Started.".ToUpper())
                    select
                    x.pluginObj).FirstOrDefault();

                plugin = ffxivPlugin;

                if (plugin != null)
                {
                    DataRepository = plugin.DataRepository;
                    DataSubscription = plugin.DataSubscription;

                    var ff14PluginConfig = DataRepository.GetCurrentFFXIVProcess();

                    if (ff14PluginConfig != null && DataRepository != null)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    Trace.WriteLine("Error!, FFXIV_ACT_Plugin.dll not found.");
                    return false;
                }
            }
        }

        public static Combatant GetCombatantPlayer()
        {
            var result = default(Combatant);

            if (Initialize())
            {

                if (DataRepository.GetCurrentFFXIVProcess().ProcessName == null)
                {
                    return result;
                }

                object[] list = DataRepository.GetCombatantList().ToArray();
                if (list.Length > 0)
                {
                    var item = (dynamic)list[0];
                    var combatant = new Combatant();

                    combatant.Name = (string)item.Name;
                    combatant.ID = (uint)item.ID;
                    combatant.Job = (int)item.Job;
                    combatant.CurrentMP = (int)item.CurrentMP;
                    combatant.MaxMP = (int)item.MaxMP;

                    result = combatant;
                }

                return result;
            }
            return result;
            /*
            #endif
            */
        }

        public static string MyName()
        {
            if (Initialize())
            {
                if (DataRepository.GetCurrentFFXIVProcess().ProcessName == null)
                {
                    return "nonNameError";
                }
                dynamic list = DataRepository.GetCombatantList();
                foreach (dynamic item in list)
                {
                    return (string)item.Name;
                }
            }
            return "nonNameError";
        }


        public static List<Combatant> GetCombatantList()
        {
            var result = new List<Combatant>();
            if (Initialize())
            {
                if (DataRepository.GetCurrentFFXIVProcess().ProcessName == null)
                {
                    return result;
                }
                dynamic list = DataRepository.GetCombatantList();
                foreach (dynamic item in list)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    var combatant = new Combatant();
                    if ((int)item.CastTargetID == 0 || string.IsNullOrWhiteSpace((string)item.Name))
                    {
                        continue;
                    }
                    combatant.Name = (string)item.Name;
                    combatant.ID = (uint)item.ID;
                    combatant.Job = (int)item.Job;
                    combatant.CurrentMP = (int)item.CurrentMP;
                    combatant.MaxMP = (int)item.MaxMP;
                    combatant.IsCasting = (bool)item.IsCasting;
                    combatant.OwnerID = (uint)item.OwnerID;
                    combatant.Name = (string)item.Name;
                    combatant.type = (byte)item.type;
                    combatant.Level = (int)item.Level;
                    combatant.CurrentHP = (int)item.CurrentHP;
                    combatant.MaxHP = (int)item.MaxHP;
                    combatant.CurrentTP = (int)item.CurrentTP;
                    combatant.PosX = (float)item.PosX;
                    combatant.PosY = (float)item.PosY;
                    combatant.PosZ = (float)item.PosZ;

                    result.Add(combatant);
                }
            }
            return result;
        }

        public static List<Combatant> GetMobList()
        {
            var result = new List<Combatant>();
            if (Initialize())
            {
                if (DataRepository.GetCurrentFFXIVProcess().ProcessName == null)
                {
                    return result;
                }
                dynamic list = DataRepository.GetCombatantList();
                foreach (dynamic item in list)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    var combatant = new Combatant();
                    if ((byte)item.type == 2)
                    {
                        continue;
                    }
                    combatant.Name = (string)item.Name;
                    combatant.ID = (uint)item.ID;
                    combatant.Job = (int)item.Job;
                    combatant.CurrentMP = (int)item.CurrentMP;
                    combatant.MaxMP = (int)item.MaxMP;
                    combatant.IsCasting = (bool)item.IsCasting;
                    combatant.OwnerID = (uint)item.OwnerID;
                    combatant.Name = (string)item.Name;
                    combatant.type = (byte)item.type;
                    combatant.Level = (int)item.Level;
                    combatant.CurrentHP = (int)item.CurrentHP;
                    combatant.MaxHP = (int)item.MaxHP;
                    combatant.CurrentTP = (int)item.CurrentTP;
                    combatant.PosX = (float)item.PosX;
                    combatant.PosY = (float)item.PosY;
                    combatant.PosZ = (float)item.PosZ;

                    result.Add(combatant);
                }
            }
            return result;
        }
        public static List<Combatant> GetPTList()
        {
            var result = new List<Combatant>();
            if (Initialize())
            {
                if (DataRepository.GetCurrentFFXIVProcess().ProcessName == null)
                {
                    return result;
                }
                dynamic list = DataRepository.GetCombatantList();
                foreach (dynamic item in list)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    var combatant = new Combatant();
                    // jobIDが0の人はすべて除外する
                    if (((int)item.Job == 0
                        || (byte)item.type != 1)
                        )
                    {
                        continue;
                    }
                    combatant.Name = (string)item.Name;
                    combatant.ID = (uint)item.ID;
                    combatant.Job = (int)item.Job;
                    combatant.CurrentMP = (int)item.CurrentMP;
                    combatant.MaxMP = (int)item.MaxMP;
                    combatant.IsCasting = (bool)item.IsCasting;
                    combatant.OwnerID = (uint)item.OwnerID;
                    combatant.Name = (string)item.Name;
                    combatant.type = (byte)item.type;
                    combatant.Level = (int)item.Level;
                    combatant.CurrentHP = (int)item.CurrentHP;
                    combatant.MaxHP = (int)item.MaxHP;
                    combatant.CurrentTP = (int)item.CurrentTP;
                    combatant.PosX = (float)item.PosX;
                    combatant.PosY = (float)item.PosY;
                    combatant.PosZ = (float)item.PosZ;

                    result.Add(combatant);
                }
            }
            return result;
        }

        public static class LogParser
        {
            public static void RaiseLog(
                DateTime timestamp,
                string[] data,
                bool isImport = false)
            {
                var log = string.Join(
                    "|",
                    new[]
                    {
                    timestamp.ToString("O")
                    }.Union(data));

                ActGlobals.oFormActMain.BeginInvoke((MethodInvoker)delegate
                {
                    ActGlobals.oFormActMain.ParseRawLogLine(isImport, timestamp, log);
                });
            }
        }

        public static List<Combatant> GetMobCombatantList()
        {
            var result = new List<Combatant>();
            if (Initialize())
            {
                if (DataRepository.GetCurrentFFXIVProcess().ProcessName == null)
                {
                    return result;
                }
                dynamic list = DataRepository.GetCombatantList();
                foreach (dynamic item in list)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    var combatant = new Combatant();
                    if ((int)item.CastTargetID == 0)
                    {
                        continue;
                    }
                    combatant.Name = (string)item.Name;
                    combatant.ID = (uint)item.ID;
                    combatant.Job = (int)item.Job;
                    combatant.CurrentMP = (int)item.CurrentMP;
                    combatant.MaxMP = (int)item.MaxMP;
                    combatant.IsCasting = (bool)item.IsCasting;
                    combatant.OwnerID = (uint)item.OwnerID;
                    combatant.Name = (string)item.Name;
                    combatant.type = (byte)item.type;
                    combatant.Level = (int)item.Level;
                    combatant.CurrentHP = (int)item.CurrentHP;
                    combatant.MaxHP = (int)item.MaxHP;
                    combatant.CurrentTP = (int)item.CurrentTP;
                    combatant.PosX = (float)item.PosX;
                    combatant.PosY = (float)item.PosY;
                    combatant.PosZ = (float)item.PosZ;

                    result.Add(combatant);
                }
            }
            return result;
        }

        /// <summary>
        /// mobリストを作成する
        /// </summary>
        /// <returns></returns>
        public static List<string> CreatemobList()
        {
            dynamic list = ActHelper.GetCombatantList();

            List<string> createMobList = new List<string>();

            foreach (dynamic item in list.ToArray())
            {
                if (item == null)
                {
                    continue;
                }
                if ((byte)item.type == 2 && (uint)item.OwnerID == 0)
                {
                    // 重複しないようにする
                    if (!createMobList.Contains((string)item.Name))
                    {
                        createMobList.Add((string)item.Name);
                    }
                }
            }
            return createMobList;
        }

    }

    public class Combatant
    {
        public Combatant()
        {
            this.Name = string.Empty;
        }
        public int CastBuffID;
        public float CastDurationCurrent;
        public float CastDurationMax;
        public string CastSkillName = string.Empty;
        public uint CastTargetID;
        public int CurrentCP;
        public int CurrentGP;
        public int CurrentHP;
        public int CurrentMP;
        public int CurrentTP;
        public uint ID;
        public bool IsCasting;
        public int Job;
        public int Level;
        public int MaxCP;
        public int MaxGP;
        public int MaxHP;
        public int MaxMP;
        public int MaxTP;
        public string Name = string.Empty;
        public int Order;
        public uint OwnerID;
        public float PosX;
        public float PosY;
        public float PosZ;
        public byte type;
        public Combatant Player;

    }

    public class Player
    {
        public int JobID;
        public int Str;
        public int Dex;
        public int Vit;
        public int Intel;
        public int Mnd;
        public int Pie;
        public int Attack;
        public int Accuracy;
        public int Crit;
        public int AttackMagicPotency;
        public int HealMagicPotency;
        public int Det;
        public int SkillSpeed;
        public int SpellSpeed;
        public int WeaponDmg;
    }

    public class Mobinfo
    {
        public string Name;
        public uint ID;
        public int CurrentHP;
        public string X;
        public string Y;
        public bool IsCasting;
        public int Level;
        public byte type;

    }
}