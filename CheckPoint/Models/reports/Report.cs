using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.Reports
{
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del reporte")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("reporterId")]
        [Required]
        [DisplayName("Identificador del reportante")]
        public string ReporterId { get; set; } = string.Empty;

        [BsonElement("targetType")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de objetivo")]
        public string TargetType { get; set; } = string.Empty;

        [BsonElement("targetId")]
        [Required]
        [DisplayName("Identificador del objetivo")]
        public string TargetId { get; set; } = string.Empty;

        [BsonElement("reason")]
        [Required]
        [StringLength(500)]
        [DisplayName("Motivo")]
        public string Reason { get; set; } = string.Empty;

        [BsonElement("status")]
        [Required]
        [StringLength(50)]
        [DisplayName("Estado")]
        public string Status { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }

        [BsonIgnore]
        [DisplayName("Descripción")]
        public string Description { get; set; } = string.Empty;

        [BsonIgnore]
        [DisplayName("Notas de resolución")]
        public string? ResolutionNotes { get; set; }
    }
}