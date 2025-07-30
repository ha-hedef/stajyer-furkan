using System;

namespace DIP_Ornek
{
    // Soyutlama
    public interface IMesajGonderici
    {
        void Gonder(string mesaj);
    }

    // Email gönderici
    public class EmailGonderici : IMesajGonderici
    {
        public void Gonder(string mesaj)
        {
            Console.WriteLine($"Email gönderildi: {mesaj}");
        }
    }

    // SMS gönderici
    public class SmsGonderici : IMesajGonderici
    {
        public void Gonder(string mesaj)
        {
            Console.WriteLine($"SMS gönderildi: {mesaj}");
        }
    }

    // Üst seviye sınıf sadece soyutlamaya bağımlıdır
    public class BildirimServisi
    {
        private readonly IMesajGonderici _mesajGonderici;

        public BildirimServisi(IMesajGonderici mesajGonderici)
        {
            _mesajGonderici = mesajGonderici;
        }

        public void BildirimYolla(string mesaj)
        {
            _mesajGonderici.Gonder(mesaj);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IMesajGonderici email = new EmailGonderici();
            BildirimServisi bildirim1 = new BildirimServisi(email);
            bildirim1.BildirimYolla("Toplantı saat 10:00’da.");

            IMesajGonderici sms = new SmsGonderici();
            BildirimServisi bildirim2 = new BildirimServisi(sms);
            bildirim2.BildirimYolla("Şifre sıfırlama kodunuz: 1234");
        }
    }
}
