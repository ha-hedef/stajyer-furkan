using System;

namespace OOP_Encapsulation
{
    public class BankaHesabi
    {
        private decimal bakiye;

        public void ParaYatir(decimal miktar)
        {
            if (miktar > 0)
            {
                bakiye += miktar;
                Console.WriteLine($"{miktar} TL yatırıldı. Güncel bakiye: {bakiye} TL");
            }
        }

        public void ParaCek(decimal miktar)
        {
            if (miktar <= bakiye)
            {
                bakiye -= miktar;
                Console.WriteLine($"{miktar} TL çekildi. Güncel bakiye: {bakiye} TL");
            }
            else
            {
                Console.WriteLine("Yetersiz bakiye!");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            BankaHesabi hesap = new BankaHesabi();
            hesap.ParaYatir(500);
            hesap.ParaCek(200);
            hesap.ParaCek(400); // hata verir çünkü bakiye yetersiz
        }
    }
}
