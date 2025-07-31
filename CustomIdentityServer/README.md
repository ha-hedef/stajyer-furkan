# Custom Identity Server 

##  Proje Hakkında
Custom Identity Server, kimlik doğrulama ve yetkilendirme süreçlerini modern güvenlik standartlarına uygun şekilde yöneten **C# / .NET Core tabanlı** bir uygulamadır.  
Bu proje, **JWT (JSON Web Token)**, **OWASP güvenlik ilkeleri**, **rol bazlı yetkilendirme**, **çok faktörlü kimlik doğrulama (MFA)** ve **sosyal giriş (Google & Facebook Login)** gibi özellikleri içeren **özelleştirilmiş bir Identity Server altyapısı** sunar.

Bu yapı sayesinde:
- Kullanıcılar güvenli şekilde giriş yapabilir,
- Rollerine göre API kaynaklarına erişebilir,
- Şifrelerini güvenli şekilde sıfırlayabilir,
- Gizlilik sözleşmelerini onaylayarak sistem politikalarına uygun davranabilir.

---

##  Projenin Amacı
- **Kurumsal seviyede güvenlik** sağlayan bir kimlik doğrulama sistemi geliştirmek.
- **Rol tabanlı erişim** ile kullanıcıların yetkilerini ayrıştırmak.
- **Sosyal giriş seçenekleri** (Google ve Facebook) ekleyerek kullanıcı deneyimini artırmak.
- **Çok faktörlü doğrulama (OTP, SMS, Email, WhatsApp)** ekleyerek ek güvenlik katmanı sağlamak.
- **Gizlilik sözleşmesi onayı**, **zorunlu şifre değişimi** ve **hesap kilitleme mekanizmaları** ile **OWASP güvenlik kurallarına uyum** göstermek.

---

##  Kullanılan Teknolojiler
- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core** (EF Core)
- **ASP.NET Core Identity**
- **JWT (JSON Web Token)**
- **PostgreSQL (Supabase ile entegre edilebilir)**
- **Google OAuth2 & Facebook OAuth2**
- **Swagger / Postman (API dokümantasyonu ve test için)**
- **OWASP Best Practices**

---

##  Temel Özellikler

###  Kimlik Doğrulama & Yetkilendirme
- JWT tabanlı authentication
- Rol tabanlı yetki kontrolü (Master, Superuser, User, Guest)
- Login attempt loglama & brute force saldırılarına karşı hesap kilitleme

###  Güvenlik
- **OWASP güvenlik standartları**
- Şifreleme, token validation
- Yanlış şifre girildiğinde hesap kilitleme
- Zorunlu aralıklı şifre değişikliği
- Şifre sıfırlama (email üzerinden link ile)

###  Çok Faktörlü Doğrulama (MFA)
- OTP (One Time Password) üretme & doğrulama
- SMS / Email / WhatsApp mock servisleri

###  Sosyal Giriş
- Google ile giriş (OAuth2)
- Facebook ile giriş (OAuth2)

###  Ek Özellikler
- Gizlilik sözleşmesi onayı (`/api/auth/accept-privacy-policy`)
- Varsayılan Master hesap ve PIN
- Refresh Token mekanizması


---



