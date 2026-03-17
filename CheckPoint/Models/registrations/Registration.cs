using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.Registrations
{
    public class Registration
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la inscripción")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("eventId")]
        [Required]
        [DisplayName("Identificador del evento")]
        public string EventId { get; set; } = string.Empty;

        [BsonElement("userId")]
        [Required]
        [DisplayName("Identificador del usuario")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("registeredAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de inscripción")]
        public DateTime RegisteredAt { get; set; }

        [BsonElement("status")]
        [Required]
        [StringLength(50)]
        [DisplayName("Estado")]
        public string Status { get; set; } = string.Empty;
    }
}