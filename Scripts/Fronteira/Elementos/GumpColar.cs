using Server.Network;
using Server.Commands;
using Server.Mobiles;
using Server.Items;
using Server.Fronteira.Elementos;
using Server.Leilaum;
using Server.Ziden;
using Server.Engines.Craft;
using Server.SkillHandlers;
using System;

namespace Server.Gumps
{
    public class ColarElementalGump : Gump
    {
        private ColarElemental e;

        public object DefBlacksmith { get; private set; }

        public ColarElementalGump(PlayerMobile pl, ColarElemental colar) : base(0, 0)
        {
            e = colar;
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(673 - 130, 334 - 20, 400, 150, 9200);
            AddHtml(673 - 125, 334 - 20, 100, 20, "Aprimorar Colar ?", false, false);
            var nivel = colar.Nivel;

            AddBackground(673 - 110, 334, 111, 101, 3500);
            AddHtml(711 - 110, 350, 183, 22, 20.ToString(), (bool)false, (bool)false);
            AddHtml(678 - 110, 406, 100, 22, "Jarro de Po Magico", (bool)true, (bool)false);
            //AddItem(703, 374, custos.Item);
            NewAuctionGump.AddItemCentered(673 - 100, 334, 111, 101, 3823, 0, this);

            AddBackground(673, 334, 111, 101, 3500);
            AddHtml(711, 350, 183, 22, 50.ToString(), (bool)false, (bool)false);
            AddHtml(678, 406, 120, 22, "Crtl. Therathan", (bool)true, (bool)false);
            //AddItem(703, 374, custos.Item);
            NewAuctionGump.AddItemCentered(673, 334, 111, 101, 3823, 0, this);

            AddBackground(784, 335, 111, 101, 3500);
            AddHtml(827, 350, 83, 22, 30.ToString(), (bool)false, (bool)false);
            AddHtml(793, 405, 100, 22, "Ess. " + colar.Elemento.ToString(), (bool)true, (bool)false);
            //AddItem(811, 367, 576);
            NewAuctionGump.AddItemCentered(784, 335, 111, 101, 0x571C, colar.Hue, this);

            AddButton(804, 435, 247, 248, (int)ElementoButtons.Upar, GumpButtonType.Reply, 0);
        }

        public static bool CheckForja(Mobile from, int range)
        {
            Map map = from.Map;

            if (map == null)
            {
                return false;
            }

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, range);
            bool tem = false;
            foreach (Item item in eable)
            {
                var type = item.GetType();
                if ((from.Z + 16) < item.Z || (item.Z + 16) < from.Z || !from.InLOS(item))
                {
                    continue;
                }
                if(item.ItemID == 0x2DD8)
                {
                    tem = true;
                    break;

                }
            }

            eable.Free();

            return tem;
        }

        public enum ElementoButtons
        {
            Nads,
            Upar,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var from = sender.Mobile as PlayerMobile;

            if(info.ButtonID!=1)
            {
                return;
            }

            if(!CheckForja(from, 4))
            {
                from.SendMessage("Voce precisa de uma forja elfica para fazer isto. Dizem que forjas elficas foram abandonadas na dungeon Caverna de Cristal, em Nujelm.");
                return;
            }

            if(!from.Backpack.HasItems(new Type[] { typeof(PedraMagica) }, new int[] { 20 }))
            {
                from.SendMessage("Falta po magico");
                return;
            }
            var cristal = from.Backpack.FindItemByType<CristalTherathan>();
            if (cristal == null ||  cristal.Amount < 50)
            {
                from.SendMessage("Falta cristal therathan");
                return;
            }
            var tipoEssencia = BaseEssencia.GetEssencia(e.Elemento);
            var essencia = from.Backpack.FindItemByType(tipoEssencia);
            if (essencia == null ||  essencia.Amount < 30)
            {
                from.SendMessage("Falta essencia elemental de "+e.Elemento);
                return;
            }

            essencia.Consume(30);
            cristal.Consume(50);
            from.Backpack.ConsumeTotal(new Type[] { typeof(PedraMagica) }, new int[] { 20 });

            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
            Effects.PlaySound(from.Location, from.Map, 0x243);

            Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

            Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

            e.Nivel += 1;
            from.PlayAttackAnimation();
            from.OverheadMessage("* encantou *");
            from.SendMessage("Seu colar absorveu a energia");
            from.CloseAllGumps();
        }
    }
}

