using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Server.Fronteira.Weeklies.Weekly;

namespace Server.Fronteira.Weeklies
{
    public class SaveWeekly
    {
        private static string FilePath = Path.Combine("Saves/Dungeons", "Semanal.bin");
        public static bool Carregado = false;
        public static HashSet<int> JaCompletou = new HashSet<int>();
        public static int SEMANA_ATUAL = 0;
        public static List<KillCombo> Kills = new List<KillCombo>();

        public static void Configure()
        {
            Console.WriteLine("Inicializando save das weeklies");
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }
       
        private static void Salva(GenericWriter writer)
        {
            Console.WriteLine("Salvando weeklies");
            writer.Write((int)2);
            writer.Write(SEMANA_ATUAL);
            writer.Write(Kills.Count);
            
            foreach(var kill in Kills)
            {
                writer.Write(kill.n);
                writer.Write(kill.qtd);
                writer.Write(kill.Monstro);
            }
            writer.Write(JaCompletou.Count);
            foreach (var c in JaCompletou)
                writer.Write(c);
        }

        private static void Carrega(GenericReader reader)
        {
            Console.WriteLine("Carregando weeklies");
            var ver = reader.ReadInt();
            if (ver > 1)
            {
                SEMANA_ATUAL = reader.ReadInt();
                var count = reader.ReadInt();
                for (var x = 0; x < count; x++)
                {

                    var n = reader.ReadString();
                    var qtd = reader.ReadInt();
                    var t = reader.ReadType();
                    Kills.Add(new KillCombo(n, t, qtd));
                }
                count = reader.ReadInt();
                for (var x = 0; x < count; x++)
                {
                    var s = reader.ReadInt();
                    JaCompletou.Add(s);
                }
            }

        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(FilePath, Salva);
        }

        public static void OnLoad()
        {
            if (!Carregado)
            {
                Console.WriteLine("Carregando weeklies");
                Persistence.Deserialize(FilePath, Carrega);
                Carregado = true;
            }

        }
    }
}
