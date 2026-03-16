using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.AuditLogs
{
    public class AuditLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del log")]
        public string Id { get; set; }

        [BsonElement("userId")]
        [Required]
        [DisplayName("Identificador del usuario")]
        public string UserId { get; set; }

        [BsonElement("action")]
        [Required]
        [StringLength(50)]
        [DisplayName("Acción")]
        public string Action { get; set; }

        [BsonElement("entityType")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de entidad")]
        public string EntityType { get; set; }

        [BsonElement("entityId")]
        [Required]
        [DisplayName("Identificador de la entidad")]
        public string EntityId { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("metadata")]
        [DisplayName("Metadatos")]
        public string Metadata { get; set; }
    }
}