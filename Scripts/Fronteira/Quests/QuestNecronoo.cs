using System;
using Server.Items;
using Server.Items.Functional.Pergaminhos;
using Server.Mobiles;
using Server.Ziden;

namespace Server.Engines.Quests
{
    public class NecroNoob : BaseQuest
    {
        public override bool DoneOnce { get { return false; } }

        //public override QuestChain ChainID => QuestChain.Tutorial;

        //public override Type NextQuest =>  typeof(CloseEnoughQuest)

        public override object Title
        {
            get
            {
                return "Seu Livro de Necromancia";
            }
        }
        public override object Description
        {
            get
            {
                return @"Hmm... voce por acaso teria um livro de necromancia ? Caso tenha, eu posso fazer algo por ele para ele ser seu livro pessoal, sabe ? Para que voce nao o perca quando morra.
Basta me trazer uma mascara... pois com ela, posso encantar o pergaminho para voce. Voce pode conseguir estes items na caverna dos selvagens.";
            }
        }
        public override object Refuse
        {
            get
            {
                return "Entendo";
            }
        }
        public override object Uncomplete
        {
            get
            {
                return "Preciso dos items para poder completar o pergaminho.";
            }
        }
        public override object Complete
        {
            get
            {
                return "Aqui esta seu pergaminho.";
            }
        }

        public NecroNoob()
            : base()
        {
            switch(Utility.Random(3))
            {
                case 0: this.AddObjective(new ObtainObjective(typeof(DeerMask), "Mascara do Veado", 1)); break;
                case 1: this.AddObjective(new ObtainObjective(typeof(BearMask), "Mascara do Urso", 1)); break;
                case 2: this.AddObjective(new ObtainObjective(typeof(TribalMask), "Mascara Tribal", 1)); break;
            }
            this.AddReward(new BaseReward(typeof(PergaminhoNecro), 1, "Pergaminho para livro de necromancia pessoal"));
        }

        public override void OnCompleted()
        {
            this.Owner.PlaySound(this.CompleteSound);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NecroNewba : MondainQuester
    {
        public override Type[] Quests
        {
            get
            {
                return new Type[] {
                    typeof(NecroNoob)
        };
            }
        }


        [Constructable]
        public NecroNewba()
            : base("Turpoleta", "O Garguleta Necromante")
        {
            this.SetSkill(SkillName.Anatomy, 120.0, 120.0);
            this.SetSkill(SkillName.Parry, 120.0, 120.0);
            this.SetSkill(SkillName.Healing, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Swords, 120.0, 120.0);
            this.SetSkill(SkillName.Focus, 120.0, 120.0);
        }

        public NecroNewba(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            this.Say("* estudando *");  // Know yourself, and you will become a true warrior.
        }

        public override void InitBody()
        {
            Body = 74;
        }

        public override void InitOutfit()
        {
            SetWearable(new Robe(Utility.RandomBlueHue()));
            SetWearable(new WizardsHat(Utility.RandomBlueHue()));
            SetWearable(new Shoes(Utility.RandomBlueHue()));
            SetWearable(new Spellbook());
            int hairHue = GetHairHue();

            Utility.AssignRandomHair(this, hairHue);
            Utility.AssignRandomFacialHair(this, hairHue);

            if (Body == 0x191)
            {
                FacialHairItemID = 0;
            }

            if (!Siege.SiegeShard)
                PackGold(100, 200);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}