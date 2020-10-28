using System;

namespace Producer 
{
    public class Gerät
    { 
        private int _herzfrequenz;
        private int[] _blutdruck;
        private string _patientname;
        public int Herzfrequenz {get => _herzfrequenz;}
        public int[] Blutdruck {get => _blutdruck;}
        public string Patientname {get => _patientname;}
        public Gerät (string patientname)
        {
            _herzfrequenz = 70;
            _blutdruck = new int[] {120, 80};
            _patientname = patientname;
        }

        public void Simulation()
        {
            var rand = new Random();
            int zufall1 = rand.Next(-1, 2);
            int zufall2 = rand.Next(-1, 2);
            int zufall3 = rand.Next(-1, 2);
            _herzfrequenz += zufall1;
            _blutdruck[0] += zufall2;
            _blutdruck[1] += zufall3;
        }
    }
}






  