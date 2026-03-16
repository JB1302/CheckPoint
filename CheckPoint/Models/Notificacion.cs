using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckPoint.Models
{
    public class Notificacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>EventoActualizado | NuevaPublicacion | InscripcionConfirmada | InscripcionCancelada | Sistema</summary>
        [BsonElement("type")]
        public string Type { get; set; } = "System";

        [BsonElement("referenceId")]
        public string ReferenceId { get; set; } = string.Empty;

        /// <summary>Evento | Publicacion | Inscripcion</summary>
        [BsonElement("referenceType")]
        public string ReferenceType { get; set; } = string.Empty;

        [BsonElement("isRead")]
        public bool IsRead { get; set; } = false;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


