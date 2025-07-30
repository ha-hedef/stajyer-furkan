using System;
using System.IO;

namespace SRP_Ornek
{
    // Kullanıcı bilgilerini tutan sınıf
    public class Kullanici
    {
        public string Ad { get; set; }
        public string Eposta { get; set; }
    }

    // Kullanıcı kaydını yöneten sınıf
    public class KullaniciYonetimi
    {
        public void KullaniciEkle(Kullanici kullanici)
        {
            Console.WriteLine($"{kullanici.Ad} adlı kullanıcı sisteme eklendi.");
        }
    }

    // Dosyaya kayıt yapan sınıf
    public class DosyaKayit
    {
        public void DosyayaYaz(string veri)
        {
            File.WriteAllText("kullanicilar.txt", veri);
            Console.WriteLine("Kullanıcı bilgileri dosyaya kaydedildi.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Kullanici yeniKullanici = new Kullanici { Ad = "Ahmet", Eposta = "ahmet@mail.com" };

            KullaniciYonetimi yonetim = new KullaniciYonetimi();
            yonetim.KullaniciEkle(yeniKullanici);

            DosyaKayit kayit = new DosyaKayit();
            kayit.DosyayaYaz($"{yeniKullanici.Ad} - {yeniKullanici.Eposta}");
        }
    }
}

