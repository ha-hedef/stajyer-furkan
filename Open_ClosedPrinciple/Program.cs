using System;
using System.Collections.Generic;

namespace OCP_Ornek
{
    // Ortak arayüz
    public interface ISekil
    {
        double AlanHesapla();
    }

    // Dikdörtgen sınıfı
    public class Dikdortgen : ISekil
    {
        public double Genislik { get; set; }
        public double Yukseklik { get; set; }

        public double AlanHesapla()
        {
            return Genislik * Yukseklik;
        }
    }

    // Daire sınıfı
    public class Daire : ISekil
    {
        public double YariCap { get; set; }

        public double AlanHesapla()
        {
            return Math.PI * YariCap * YariCap;
        }
    }

    // Alan hesaplama servisi
    public class AlanHesaplamaServisi
    {
        public double ToplamAlanHesapla(List<ISekil> sekiller)
        {
            double toplamAlan = 0;
            foreach (var sekil in sekiller)
            {
                toplamAlan += sekil.AlanHesapla();
            }
            return toplamAlan;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sekiller = new List<ISekil>
            {
                new Dikdortgen { Genislik = 5, Yukseklik = 10 },
                new Daire { YariCap = 3 }
            };

            AlanHesaplamaServisi servis = new AlanHesaplamaServisi();
            Console.WriteLine($"Toplam Alan: {servis.ToplamAlanHesapla(sekiller)}");
        }
    }
}

