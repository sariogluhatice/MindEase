MindEase - Mental Health Support Platform
=========================================

Betül Taşkıran, Hatice Sarıoğlu, Pınar İşler
SOFT3111 - Fall 2025


Projeyi Calistirmak Icin
------------------------
1. Visual Studio'da MindEase.sln dosyasini acin
2. F5 ile calistirin veya terminalde "dotnet run" yazin
3. Tarayicida http://localhost:5063 adresine gidin


Test Kullanicilari
------------------
Admin girisi:
  Kullanici: hatice_admin
  Sifre: admin123

Normal kullanici:
  Kullanici: melisa_user
  Sifre: user2024

Terapist:
  Kullanici: john_therapist
  Sifre: healme


Proje Ozellikleri
-----------------
- Kullanici kayit ve giris sistemi
- Randevu olusturma, duzenleme, silme
- Farkli kullanici rolleri (admin, user, therapist)
- Randevularda arama ozelligi


Veritabani
----------
SQLite kullaniliyor (mindease.db)
Uygulama ilk calistiginda otomatik olusturuluyor.

Eger veritabanini sifirlamak isterseniz:
- Uygulamayi durdurun
- mindease.db dosyasini silin
- Tekrar calistirin


Notlar
------
- Tum sayfalar calisiyor
- Login olmadan Appointments sayfasina erisilemez
- Her kullanici sadece kendi randevularini gorebilir
