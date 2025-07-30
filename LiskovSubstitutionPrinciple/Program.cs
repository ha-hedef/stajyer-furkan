using System;
using System.Collections.Generic;

namespace LSP_Ornek
{
    // Kuşların ortak özellikleri
    public abstract class Kus
    {
        public abstract void SesCikar();
    }

    // Uçabilen kuşların özellikleri
    public abstract class UcanKus : Kus
    {
        public abstract void Uc();
    }

    public class Serce : UcanKus
    {
        public override void SesCikar() => Console.WriteLine("Cik cik");
        public override void Uc() => Console.WriteLine("Serçe uçuyor.");
    }

    public class Penguen : Kus
    {
        public override void SesCikar() => Console.WriteLine("Vak vak");
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Kus> kuslar = new List<Kus>
            {
                new Serce(),
                new Penguen()
            };

            foreach (var kus in kuslar)
            {
                kus.SesCikar();
            }

            UcanKus ucabilenKus = new Serce();
            ucabilenKus.Uc(); // Penguen burada sorun çıkarmaz çünkü doğru soyutlama yapılmıştır.
        }
    }
}
