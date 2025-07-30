using System;

namespace OOP_Inheritance
{
    // Temel sınıf
    public class Hayvan
    {
        public string Ad { get; set; }
        public void SesCikar()
        {
            Console.WriteLine($"{Ad} ses çıkarıyor.");
        }
    }

    // Türetilmiş sınıf
    public class Kopek : Hayvan
    {
        public void Havla()
        {
            Console.WriteLine($"{Ad} havlıyor.");
        }
    }

    class Program
    {
        static void Main()
        {
            Kopek kopek = new Kopek { Ad = "Karabaş" };
            kopek.SesCikar();
            kopek.Havla();
        }
    }
}
