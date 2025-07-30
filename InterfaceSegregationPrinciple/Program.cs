using System;

namespace ISP_Ornek
{
    // Ayrılmış küçük arayüzler
    public interface IParaIslemleri
    {
        void ParaYatir(decimal miktar);
        void ParaCek(decimal miktar);
    }

    public interface IKrediIslemleri
    {
        void KrediBasvurusuYap();
    }

    // Kredi hizmeti sunan banka
    public class BuyukBanka : IParaIslemleri, IKrediIslemleri
    {
        public void ParaYatir(decimal miktar) => Console.WriteLine($"{miktar} TL yatırıldı.");
        public void ParaCek(decimal miktar) => Console.WriteLine($"{miktar} TL çekildi.");
        public void KrediBasvurusuYap() => Console.WriteLine("Kredi başvurusu yapıldı.");
    }

    // Sadece temel hizmet sunan banka
    public class KucukBanka : IParaIslemleri
    {
        public void ParaYatir(decimal miktar) => Console.WriteLine($"{miktar} TL yatırıldı.");
        public void ParaCek(decimal miktar) => Console.WriteLine($"{miktar} TL çekildi.");
    }

    class Program
    {
        static void Main(string[] args)
        {
            IParaIslemleri banka1 = new KucukBanka();
            banka1.ParaYatir(100);

            IKrediIslemleri banka2 = new BuyukBanka();
            banka2.KrediBasvurusuYap();
        }
    }
}
