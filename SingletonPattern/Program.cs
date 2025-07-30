using System;

namespace SingletonOrnek
{
    public class VeritabaniBaglantisi
    {
        private static VeritabaniBaglantisi _instance;
        private static readonly object _kilit = new object();

        private VeritabaniBaglantisi()
        {
            Console.WriteLine("Veritabanı bağlantısı kuruldu.");
        }

        public static VeritabaniBaglantisi Instance
        {
            get
            {
                lock (_kilit)
                {
                    if (_instance == null)
                    {
                        _instance = new VeritabaniBaglantisi();
                    }
                }
                return _instance;
            }
        }

        public void SorguCalistir(string sorgu)
        {
            Console.WriteLine($"Sorgu çalıştırılıyor: {sorgu}");
        }
    }

    class Program
    {
        static void Main()
        {
            var db1 = VeritabaniBaglantisi.Instance;
            var db2 = VeritabaniBaglantisi.Instance;

            db1.SorguCalistir("SELECT * FROM Kullanici");

            Console.WriteLine(db1 == db2);
        }
    }
}
