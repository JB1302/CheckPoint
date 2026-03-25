using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.EventRules
{
    public class EventRules
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la regla")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("eventId")]
        [Required]
        [DisplayName("Identificador del evento")]
        public string EventId { get; set; } = string.Empty;

        [BsonElement("rulesText")]
        [Required]
        [StringLength(1000)]
        [DisplayName("Texto de reglas")]
        public string RulesText { get; set; } = string.Empty;

        [BsonElement("checkInRequired")]
        [Required]
        [DisplayName("Check-in obligatorio")]
        public bool CheckInRequired { get; set; }

        [BsonElement("allowedPlatforms")]
        [Required]
        [DisplayName("Plataformas permitidas")]
        public List<string> AllowedPlatforms { get; set; } = new List<string>();

        [BsonElement("minRank")]
        [DisplayName("Rango mínimo")]
        public string? MinRank { get; set; }

        [BsonIgnore]
        [DisplayName("Nombre")]
        public string Name { get; set; } = string.Empty;

        [BsonIgnore]
        [DisplayName("Descripción")]
        public string Description { get; set; } = string.Empty;

        [BsonIgnore]
        [DisplayName("Prioridad")]
        public int Priority { get; set; } = 1;
    }
}