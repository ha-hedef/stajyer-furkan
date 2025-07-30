using System;

namespace AdapterOrnek
{
    // Eski sistem
    public class EskiPrinter
    {
        public void Yazdir(string metin)
        {
            Console.WriteLine($"Eski yazıcı: {metin}");
        }
    }

    // Yeni sistem için beklenen arayüz
    public interface IYeniPrinter
    {
        void Print(string text);
    }

    // Adapter sınıfı: eski sistemi yeni arayüze uydurur
    public class PrinterAdapter : IYeniPrinter
    {
        private readonly EskiPrinter _eskiPrinter;

        public PrinterAdapter(EskiPrinter eskiPrinter)
        {
            _eskiPrinter = eskiPrinter;
        }

        public void Print(string text)
        {
            _eskiPrinter.Yazdir(text);
        }
    }

    class Program
    {
        static void Main()
        {
            EskiPrinter eskiPrinter = new EskiPrinter();
            IYeniPrinter printer = new PrinterAdapter(eskiPrinter);

            printer.Print("Merhaba Dünya!");
        }
    }
}
