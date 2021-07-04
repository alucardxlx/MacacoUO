using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class CorrosiveAsh : Item
    {
        public override int LabelNumber { get { return 1151809; } } // Corrosive Ash

        [Constructable]
        public CorrosiveAsh()
            : this(1)
        {
        }

        [Constructable]
        public CorrosiveAsh(int amount) : base(0x423A)
        {
            this.Hue = 1360;
            this.Weight = 1;
            Name = "Pedra Elemental da Chama";
            Stackable = true;
            Amount = amount;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            else if (from.Backpack.GetAmount(typeof(QuartzGrit)) == 0)
                from.SendMessage("Voce precisa da Pedra Elemental de Quartzo");
            else if (from.Backpack.GetAmount(typeof(CursedOilstone)) == 0)
                from.SendMessage("Voce precsa da pedra elemental de vento");
            else
            {
                from.Backpack.ConsumeTotal(new Type[] { typeof(CursedOilstone), typeof(CorrosiveAsh) },
                                           new int[] { 1, 1 });

                this.Consume();

                from.AddToBackpack(new PedraElementalSuprema());
                from.SendLocalizedMessage(1151812); // You have managed to form the items into a rancid smelling, crag covered, hardened lump. In a moment of prescience, you realize what it must be named. The Whetstone of Enervation!
            }
        }

        public CorrosiveAsh(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
