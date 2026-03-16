using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckPoint.Models
{
    public class Reporte
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("reporterId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReporterId { get; set; } = string.Empty;

        [BsonElement("targetId")]
        public string TargetId { get; set; } = string.Empty;

        /// <summary>Usuario | Evento | Publicacion | Comentario</summary>
        [BsonElement("targetType")]
        public string TargetType { get; set; } = string.Empty;

        [BsonElement("reason")]
        public string Reason { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Pendiente | Revisado | Resuelto | Descartado</summary>
        [BsonElement("status")]
        public string Status { get; set; } = "Pending";

        [BsonElement("reviewedBy")]
        public string ReviewedBy { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}


