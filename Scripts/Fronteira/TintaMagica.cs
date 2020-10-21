using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Ziden
{
    public class TintaMagica : Item
    {

        [CommandProperty(AccessLevel.Administrator)]
        public SlayerName Slayer { get; set; }

        [Constructable]
        public TintaMagica() : base(0xFBF)
        {
            Name = "Tinta Magica";
            Hue = TintaBranca.COR;
            Slayer = BaseRunicTool.GetRandomSlayer();
        }

        public TintaMagica(BaseCreature mob) : base(0xFBF)
        {
            Name = "Tinta Magica";
            Hue = TintaBranca.COR;
            Slayer = SlayerGroup.GetLootSlayerType(mob.GetType());
        }

        public TintaMagica(Serial s) : base(s) { }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            list.Add("Tinta Magica");
            list.Add("Use em um livro para encanta-lo");
            list.Add("Magias darao mais dano a monstro do tipo " + Slayer.ToString());
            list.Add("Imbuing: 70");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if(from.Skills[SkillName.Imbuing].Value <= 70)
            {
                from.SendMessage("Voce precisa de pelo menos 70 Imbuing para usar isto");
                return;
            }

            from.SendMessage("Selecione um livro sem propriedades magicas para encantar");
            from.Target = new InternalTarget(this);
            base.OnDoubleClick(from);
        }

        private class InternalTarget : Target
        {
            private TintaMagica tinta;
            public InternalTarget(TintaMagica tinta) : base(10, false, TargetFlags.None)
            {
                this.tinta = tinta;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Spellbook)
                {
                    var spellbook = (Spellbook)targeted;
                    if (spellbook.Slayer != SlayerName.None)
                    {
                        from.SendMessage("Este livro ja esta encantado");
                        return;
                    }
                    tinta.Consume();
                    spellbook.Slayer = tinta.Slayer;
                    from.SendMessage("Voce encantou o livro");
                    spellbook.PrivateMessage("* encantado *", from);
                    spellbook.InvalidateProperties();
                }
                else
                {
                    from.SendMessage("Voce precisa escolher um livro de magias");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            writer.Write(0);
            writer.Write((int)Slayer);
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            var v = reader.ReadInt();
            this.Slayer = (SlayerName)reader.ReadInt();
            base.Deserialize(reader);
        }
    }

    public class EscamaMagica : Item
    {
        [CommandProperty(AccessLevel.Administrator)]
        public SlayerName Slayer { get; set; }

        [Constructable]
        public EscamaMagica() : base(0x26B4)
        {
            Name = "Escama Divina";
            Hue = TintaBranca.COR;
            Slayer = BaseRunicTool.GetRandomSlayer();
        }

        public EscamaMagica(BaseCreature mob) : base(0xFBF)
        {
            Name = "Escama Divina";
            Hue = TintaBranca.COR;
            Slayer = SlayerGroup.GetLootSlayerType(mob.GetType());
        }

        public EscamaMagica(Serial s) : base(s) { }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            list.Add("Escama Magica");
            list.Add("Use em uma arma para encanta-la");
            list.Add("Ataques darao mais dano a " + Slayer.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Selecione um livro sem propriedades magicas para encantar");
            from.Target = new InternalTarget(this);
            base.OnDoubleClick(from);
        }

        private class InternalTarget : Target
        {
            private EscamaMagica tinta;
            public InternalTarget(EscamaMagica tinta) : base(10, false, TargetFlags.None)
            {
                this.tinta = tinta;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseWeapon)
                {
                    var spellbook = (BaseWeapon)targeted;
                    if (spellbook.Slayer != SlayerName.None)
                    {
                        from.SendMessage("Esta arma ja esta encantada");
                        return;
                    }
                    tinta.Consume();
                    spellbook.Slayer = tinta.Slayer;
                    from.SendMessage("Voce encantou a arma");
                    spellbook.PrivateMessage("* encantado *", from);
                    spellbook.InvalidateProperties();
                }
                else
                {
                    from.SendMessage("Voce precisa escolher uma arma");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            writer.Write(0);
            writer.Write((int)Slayer);
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            var v = reader.ReadInt();
            this.Slayer = (SlayerName)reader.ReadInt();
            base.Deserialize(reader);
        }
    }

    public class JoiaArma : Item
    {
        [CommandProperty(AccessLevel.Administrator)]
        public SlayerName Slayer { get; set; }

        [Constructable]
        public JoiaArma() : base(0xF26)
        {
            Name = "Joia Magica";
            Hue = TintaBranca.COR;
            Slayer = BaseRunicTool.GetRandomSlayer();
        }

        public JoiaArma(BaseCreature mob) : base(0xF26)
        {
            Name = "Joia Magica";
            Hue = TintaBranca.COR;
            Slayer = SlayerGroup.GetLootSlayerType(mob.GetType());
        }

        public JoiaArma(Serial s) : base(s) { }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            list.Add("Joia Magica");
            list.Add("Use em uma arma para encanta-la");
            list.Add("Ataques darao mais dano a " + Slayer.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Selecione um livro sem propriedades magicas para encantar");
            from.Target = new InternalTarget(this);
            base.OnDoubleClick(from);
        }

        private class InternalTarget : Target
        {
            private JoiaArma tinta;
            public InternalTarget(JoiaArma tinta) : base(10, false, TargetFlags.None)
            {
                this.tinta = tinta;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseWeapon)
                {
                    var spellbook = (BaseWeapon)targeted;
                    if (spellbook.Slayer != SlayerName.None)
                    {
                        from.SendMessage("Esta arma ja esta encantada");
                        return;
                    }
                    tinta.Consume();
                    spellbook.Slayer = tinta.Slayer;
                    from.SendMessage("Voce encantou a arma");
                    spellbook.PrivateMessage("* encantado *", from);
                    spellbook.InvalidateProperties();
                }
                else
                {
                    from.SendMessage("Voce precisa escolher uma arma");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            writer.Write(0);
            writer.Write((int)Slayer);
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            var v = reader.ReadInt();
            this.Slayer = (SlayerName)reader.ReadInt();
            base.Deserialize(reader);
        }
    }
}
