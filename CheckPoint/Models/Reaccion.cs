using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckPoint.Models
{
    public class Reaccion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        /// <summary>Id de la Publicacion o Comentario que recibe la reaccion.</summary>
        [BsonElement("targetId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TargetId { get; set; } = string.Empty;

        /// <summary>Publicacion | Comentario</summary>
        [BsonElement("targetType")]
        public string TargetType { get; set; } = "Publicacion";

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        /// <summary>MeGusta | MeEncanta | Fuego</summary>
        [BsonElement("type")]
        public string Type { get; set; } = "Like";

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


