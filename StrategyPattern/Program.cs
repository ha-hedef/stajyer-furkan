using System;

namespace StrategyOrnek
{
    // Strateji arayüzü
    public interface IOdemeYontemi
    {
        void Ode(double tutar);
    }

    // Stratejiler
    public class KrediKartiOdeme : IOdemeYontemi
    {
        public void Ode(double tutar)
        {
            Console.WriteLine($"{tutar} TL kredi kartı ile ödendi.");
        }
    }

    public class PayPalOdeme : IOdemeYontemi
    {
        public void Ode(double tutar)
        {
            Console.WriteLine($"{tutar} TL PayPal ile ödendi.");
        }
    }

    // Context sınıfı
    public class OdemeServisi
    {
        private IOdemeYontemi _odemeYontemi;

        public void SetOdemeYontemi(IOdemeYontemi odemeYontemi)
        {
            _odemeYontemi = odemeYontemi;
        }

        public void OdemeYap(double tutar)
        {
            _odemeYontemi.Ode(tutar);
        }
    }

    class Program
    {
        static void Main()
        {
            OdemeServisi servis = new OdemeServisi();

            servis.SetOdemeYontemi(new KrediKartiOdeme());
            servis.OdemeYap(150);

            servis.SetOdemeYontemi(new PayPalOdeme());
            servis.OdemeYap(200);
        }
    }
}
