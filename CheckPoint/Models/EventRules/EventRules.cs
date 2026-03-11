using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.EventRules
{
    public class EventRules
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la regla")]
        public string Id { get; set; }

        [BsonElement("eventId")]
        [Required]
        [DisplayName("Identificador del evento")]
        public string EventId { get; set; }

        [BsonElement("rulesText")]
        [Required]
        [StringLength(1000)]
        [DisplayName("Texto de reglas")]
        public string RulesText { get; set; }

        [BsonElement("checkInRequired")]
        [Required]
        [DisplayName("Check-in obligatorio")]
        public bool CheckInRequired { get; set; }

        [BsonElement("allowedPlatforms")]
        [Required]
        [DisplayName("Plataformas permitidas")]
        public List<string> AllowedPlatforms { get; set; }

        [BsonElement("minRank")]
        [DisplayName("Rango mínimo")]
        public string MinRank { get; set; }
    }
}