using System;

namespace AbstractFactoryOrnek
{
    // Ürün arayüzleri
    public interface IButon
    {
        void Ciz();
    }

    public interface ICheckbox
    {
        void Ciz();
    }

    // Windows ürünleri
    public class WindowsButon : IButon
    {
        public void Ciz() => Console.WriteLine("Windows butonu çizildi.");
    }

    public class WindowsCheckbox : ICheckbox
    {
        public void Ciz() => Console.WriteLine("Windows checkbox çizildi.");
    }

    // Mac ürünleri
    public class MacButon : IButon
    {
        public void Ciz() => Console.WriteLine("Mac butonu çizildi.");
    }

    public class MacCheckbox : ICheckbox
    {
        public void Ciz() => Console.WriteLine("Mac checkbox çizildi.");
    }

    // Abstract Factory
    public interface IUIFactory
    {
        IButon ButonOlustur();
        ICheckbox CheckboxOlustur();
    }

    // Concrete Factories
    public class WindowsUIFactory : IUIFactory
    {
        public IButon ButonOlustur() => new WindowsButon();
        public ICheckbox CheckboxOlustur() => new WindowsCheckbox();
    }

    public class MacUIFactory : IUIFactory
    {
        public IButon ButonOlustur() => new MacButon();
        public ICheckbox CheckboxOlustur() => new MacCheckbox();
    }

    // Kullanım
    class Program
    {
        static void Main()
        {
            IUIFactory factory;

            // Windows için
            factory = new WindowsUIFactory();
            factory.ButonOlustur().Ciz();
            factory.CheckboxOlustur().Ciz();

            // Mac için
            factory = new MacUIFactory();
            factory.ButonOlustur().Ciz();
            factory.CheckboxOlustur().Ciz();
        }
    }
}
