using System.Collections.Generic;

namespace MindEase.Models
{
    public class Therapist
    {
        public int Id { get; set; }                 // PK (int zorunlu şart)
        public string FirstName { get; set; }        // Ad Soyad
        public string LastName { get; set; }             // Kısa tanıtım
        public string Specialty { get; set; }       // Uzmanlık alanı (Anksiyete, Çift Terapisi vs.)

        // İlişki: 1 Therapist -> N Appointment
        public List<Appointment> Appointments { get; set; }
    }
}