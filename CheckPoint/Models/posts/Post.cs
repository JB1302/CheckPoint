using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.Posts
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la publicación")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("eventId")]
        [Required]
        [DisplayName("Identificador del evento")]
        public string EventId { get; set; } = string.Empty;

        [BsonElement("authorId")]
        [Required]
        [DisplayName("Identificador del autor")]
        public string AuthorId { get; set; } = string.Empty;

        [BsonElement("content")]
        [Required]
        [StringLength(1000)]
        [DisplayName("Contenido")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [DisplayName("Fecha de actualización")]
        public DateTime? UpdatedAt { get; set; }
    }
}