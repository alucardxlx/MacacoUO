using System.Collections.Generic;

namespace Server.Fronteira.Talentos
{
    public class Talentos
    {
        private HashSet<Talento> _talentos = new HashSet<Talento>();

        public bool Tem(Talento t)
        {
            return _talentos.Contains(t);
        }

        public void Wipa()
        {
            _talentos.Clear();
        }

        public void Aprende(Talento t)
        {
            _talentos.Add(t);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(_talentos.Count);
            foreach(var talento in _talentos)
            {
                writer.Write((int)talento);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            var ct = reader.ReadInt();
            for (var x = 0; x < ct; x++)
            {
                _talentos.Add((Talento)reader.ReadInt());
            }
        }

        public void DeserialzieOld(GenericReader reader)
        {
            var ct = reader.ReadInt();
            for(var x = 0; x < ct; x++)
            {
               reader.ReadInt();
               reader.ReadInt();
            }
        }
    }
}