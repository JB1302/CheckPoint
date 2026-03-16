using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckPoint.Models
{
    public class BitacoraAuditoria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        /// <summary>Ejemplo: CrearEvento, EditarEvento, CancelarEvento, Inscribir, CancelarInscripcion</summary>
        [BsonElement("action")]
        public string Action { get; set; } = string.Empty;

        /// <summary>Evento | Inscripcion | Publicacion | Comentario | Usuario</summary>
        [BsonElement("entityType")]
        public string EntityType { get; set; } = string.Empty;

        [BsonElement("entityId")]
        public string EntityId { get; set; } = string.Empty;

        [BsonElement("oldValues")]
        public Dictionary<string, string> OldValues { get; set; } = new();

        [BsonElement("newValues")]
        public Dictionary<string, string> NewValues { get; set; } = new();

        [BsonElement("ipAddress")]
        public string IpAddress { get; set; } = string.Empty;

        [BsonElement("userAgent")]
        public string UserAgent { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


