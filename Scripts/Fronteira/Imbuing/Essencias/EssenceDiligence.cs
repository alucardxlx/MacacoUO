using System;

namespace Server.Items
{
    public class EssenciaGelo : BaseItemElemental, ICommodity
    {
        public override ElementoPvM Elemento { get { return ElementoPvM.Gelo; } }

        [Constructable]
        public EssenciaGelo()
            : this(1)
        {
        }

        [Constructable]
        public EssenciaGelo(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
        }

        public EssenciaGelo(Serial serial)
            : base(serial)
        {
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
