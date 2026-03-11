using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.Registrations
{
    public class Registration
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la inscripción")]
        public string Id { get; set; }

        [BsonElement("eventId")]
        [Required]
        [DisplayName("Identificador del evento")]
        public string EventId { get; set; }

        [BsonElement("userId")]
        [Required]
        [DisplayName("Identificador del usuario")]
        public string UserId { get; set; }

        [BsonElement("registeredAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de inscripción")]
        public DateTime RegisteredAt { get; set; }

        [BsonElement("status")]
        [Required]
        [StringLength(50)]
        [DisplayName("Estado")]
        public string Status { get; set; }
    }
}