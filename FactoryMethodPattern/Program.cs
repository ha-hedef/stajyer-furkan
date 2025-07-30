using System;

namespace FactoryMethodOrnek
{
    // Ortak arayüz
    public interface IArac
    {
        void Tasima();
    }

    // Concrete sınıflar
    public class Kamyon : IArac
    {
        public void Tasima() => Console.WriteLine("Yük kamyon ile taşınıyor.");
    }

    public class Gemi : IArac
    {
        public void Tasima() => Console.WriteLine("Yük gemi ile taşınıyor.");
    }

    // Factory Method
    public abstract class Lojistik
    {
        public abstract IArac AracOlustur();

        public void YukuTasima()
        {
            var arac = AracOlustur();
            arac.Tasima();
        }
    }

    public class KaraLojistik : Lojistik
    {
        public override IArac AracOlustur() => new Kamyon();
    }

    public class DenizLojistik : Lojistik
    {
        public override IArac AracOlustur() => new Gemi();
    }

    class Program
    {
        static void Main()
        {
            Lojistik kara = new KaraLojistik();
            kara.YukuTasima();

            Lojistik deniz = new DenizLojistik();
            deniz.YukuTasima();
        }
    }
}
