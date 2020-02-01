namespace Tetra.Timing
{
    public class Beat
    {
        public float Update(float delta)
        {
            time += delta;

            if (time > lastOctupleWhole + octupleWhole)
                lastOctupleWhole += octupleWhole;

            if (time > lastQuadrupleWhole + quadrupleWhole)
                lastQuadrupleWhole += quadrupleWhole;

            if (time > lastDoubleWhole + doubleWhole)
                lastDoubleWhole += doubleWhole;

            if (time > lastWhole + whole)
                lastWhole += whole;

            if (time > lastHalf + half)
                lastHalf += half;

            if (time > lastQuarter + quarter)
                lastQuarter += quarter;

            if (time > lastEighth + eighth)
                lastEighth += eighth;

            return delta;
        }

        public float Get(Type type)
        {
            switch (type)
            {
                case Type.None:
                    return 0;

                case Type.Octuple:
                    return octupleWhole;

                case Type.Quadruple:
                    return quadrupleWhole;

                case Type.Double:
                    return doubleWhole;

                case Type.Whole:
                    return whole;

                case Type.Half:
                    return half;

                case Type.Quarter:
                    return quarter;

                case Type.Eighth:
                    return eighth;

                default:
                    return 0;
            }
        }

        public float GetLast(Type type)
        {
            switch (type)
            {
                case Type.None:
                    return time;

                case Type.Octuple:
                    return lastOctupleWhole;

                case Type.Quadruple:
                    return lastQuadrupleWhole;

                case Type.Double:
                    return lastDoubleWhole;

                case Type.Whole:
                    return lastWhole;

                case Type.Half:
                    return lastHalf;

                case Type.Quarter:
                    return lastQuarter;

                case Type.Eighth:
                    return lastEighth;

                default:
                    return 0;
            }
        }

        public float GetNext(Type type, int ahead = 1) => GetLast(type) + (Get(type) * ahead);
        public float GetUntilNext(Type type, int ahead = 1) => GetLast(type) + (Get(type) * ahead) - time;

        public float GetNextAfter(Type type, float after)
        {
            if (type == Type.None)
                return 0;

            int ahead = 1;
            float beat = GetNext(type, ahead);

            while (after > beat)
            {
                ahead++;
                beat = GetNext(type, ahead);
            }

            return beat;
        }

        public float GetUntilNextAfter(Type type, float after)
        {
            if (type == Type.None)
                return 0;

            int ahead = 1;
            float beat = GetUntilNext(type, ahead);

            while(after > beat)
            {
                ahead++;
                beat = GetUntilNext(type, ahead);
            }

            return beat;
        }

        public void SetBPM(int bpm)
        {
            this.bpm = bpm;
            whole = 60f / bpm;
            doubleWhole = whole * 2;
            quadrupleWhole = doubleWhole * 2;
            octupleWhole = quadrupleWhole * 2;
            half = whole * 0.5f;
            quarter = half * 0.5f;
            eighth = quarter * 0.5f;
        }

        public void Reset()
        {
            time =
            lastOctupleWhole =
            lastQuadrupleWhole =
            lastDoubleWhole =
            lastWhole =
            lastHalf =
            lastQuarter =
            lastEighth = 0;
        }

        public int bpm { get; private set; }

        private float octupleWhole { get; set; }
        private float quadrupleWhole { get; set; }
        private float doubleWhole { get; set; }
        private float whole { get; set; }
        private float half { get; set; }
        private float quarter { get; set; }
        private float eighth { get; set; }
        private float lastOctupleWhole { get; set; }
        private float lastQuadrupleWhole { get; set; }
        private float lastDoubleWhole { get; set; }
        private float lastWhole { get; set; }
        private float lastHalf { get; set; }
        private float lastQuarter { get; set; }
        private float lastEighth { get; set; }
        private float time { get; set; }

        public Beat() => SetBPM(120);
        
        public enum Type
        {
            None,
            Octuple,
            Quadruple,
            Double,
            Whole,
            Half,
            Quarter,
            Eighth
        }
    }
}