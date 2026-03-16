using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.Posts
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la publicación")]
        public string Id { get; set; }

        [BsonElement("eventId")]
        [Required]
        [DisplayName("Identificador del evento")]
        public string EventId { get; set; }

        [BsonElement("authorId")]
        [Required]
        [DisplayName("Identificador del autor")]
        public string AuthorId { get; set; }

        [BsonElement("content")]
        [Required]
        [StringLength(1000)]
        [DisplayName("Contenido")]
        public string Content { get; set; }

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