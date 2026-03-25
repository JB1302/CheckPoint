using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.Comments
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del comentario")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("postId")]
        [Required]
        [DisplayName("Identificador de la publicación")]
        public string PostId { get; set; } = string.Empty;

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

        [BsonElement("isDeleted")]
        [Required]
        [DisplayName("Eliminado")]
        public bool IsDeleted { get; set; }

        [BsonIgnore]
        [DisplayName("Autor")]
        public string AuthorName { get; set; } = string.Empty;
    }
}