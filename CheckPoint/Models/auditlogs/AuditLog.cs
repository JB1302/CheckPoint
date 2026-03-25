using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.AuditLogs
{
    public class AuditLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del log")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        [Required]
        [DisplayName("Identificador del usuario")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("action")]
        [Required]
        [StringLength(50)]
        [DisplayName("Acción")]
        public string Action { get; set; } = string.Empty;

        [BsonElement("entityType")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de entidad")]
        public string EntityType { get; set; } = string.Empty;

        [BsonElement("entityId")]
        [Required]
        [DisplayName("Identificador de la entidad")]
        public string EntityId { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("metadata")]
        [DisplayName("Metadatos")]
        public string? Metadata { get; set; }

        [BsonIgnore]
        [DisplayName("Timestamp")]
        public DateTime Timestamp => CreatedAt;

        [BsonIgnore]
        [DisplayName("Nombre de usuario")]
        public string UserName { get; set; } = string.Empty;
    }
}