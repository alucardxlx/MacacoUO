using Server.Network;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Gumps
{
    public class SemElementoGump : Gump
    {
        public SemElementoGump() : base(0, 0)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(289, 126, 642, 513, 9200);
            AddHtml(551, 130, 123, 20, @"Elementos PvM", (bool)false, (bool)false);
            AddBackground(299, 150, 620, 478, 3500);
            AddBackground(717, 346, 127, 101, 3500);
            AddBackground(712, 458, 138, 101, 3500);
            AddItem(760, 388, 4968, 4968);
            AddHtml(715, 419, 131, 22, @"Pedra Elemental Suprema", (bool)true, (bool)false);
            AddHtml(715, 529, 133, 22, @"Cristal Elemental", (bool)true, (bool)false);
            AddButton(750, 592, 247, 248, (int)Buttons.Button4, GumpButtonType.Reply, 0);
            AddImage(238, 118, 10440);
            AddImage(897, 117, 10441);
            AddHtml(340, 168, 562, 18, @"Seu corpo ainda nao esta conectado com a energia deste mundo. ", (bool)false, (bool)false);
            AddHtml(340, 190, 562, 18, @"Voce eh apenas um fraco, ainda nao descobriu seu poder.", (bool)false, (bool)false);
            AddHtml(340, 213, 562, 24, @"Va a caverna de Shame e descubra...Mas esteja muito bem preparado.", (bool)false, (bool)false);
            AddItem(756, 494, 16395, 4968);
            AddHtml(772, 357, 19, 22, @"1", (bool)false, (bool)false);
            AddHtml(761, 468, 47, 22, @"100", (bool)false, (bool)false);
            AddImage(341, 249, 1550);
            AddHtml(705, 571, 153, 19, @"Destravar Potencial", (bool)false, (bool)false);
            AddHtml(714, 287, 148, 77, @"Voce precisara dos seguinte items", (bool)false, (bool)false);
        }

        public enum Buttons
        {
            Nada,
            Button4,
        }

        //0xA725
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case (int)Buttons.Button4:
                    {
                        if(from.Skills.Total < 6000)
                        {
                            from.SendMessage("Voce precisa de pelo menos 600 pontos de skill para conseguir fazer isto...");
                            return;
                        }
                        if (!sender.Mobile.Backpack.HasItem<CristalElemental>(100, true))
                            from.SendMessage("Voce precisa de 100 Pedras Elementais e 1 Pedra Elemental Suprema. Mate os bosses da dungeon de shame e una os elementos para construir a pedra.");
                        if (!sender.Mobile.Backpack.HasItem<PedraElementalSuprema>(1, true))
                            from.SendMessage("Voce precisa de 1 Pedra Elemental Suprema. Mate os bosses da dungeon de shame e una os elementos para construir a pedra.");

                        from.Backpack.ConsumeTotal(new System.Type[] { typeof(CristalElemental), typeof(PedraElementalSuprema) }, new int[] { 100, 1 });
                        ((PlayerMobile)from).Nivel += 1;
                        from.SendMessage("Voce agora pode canalizar energia elemental em seu corpo.");
                        from.SendMessage("Equipe armaduras elementais para ativar o elemento em seu corpo.");
                        break;
                    }

            }
        }
    }
}
