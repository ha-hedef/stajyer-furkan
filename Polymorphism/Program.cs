using System;

namespace OOP_Polymorphism
{
    public class Sekil
    {
        public virtual void Ciz()
        {
            Console.WriteLine("Bir şekil çiziliyor.");
        }
    }

    public class Daire : Sekil
    {
        public override void Ciz()
        {
            Console.WriteLine("Bir daire çiziliyor.");
        }
    }

    public class Dikdortgen : Sekil
    {
        public override void Ciz()
        {
            Console.WriteLine("Bir dikdörtgen çiziliyor.");
        }
    }

    class Program
    {
        static void Main()
        {
            Sekil sekil1 = new Daire();
            Sekil sekil2 = new Dikdortgen();

            sekil1.Ciz(); // Daire çiziliyor
            sekil2.Ciz(); // Dikdörtgen çiziliyor
        }
    }
}
