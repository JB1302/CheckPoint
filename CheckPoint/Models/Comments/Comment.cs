using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.Comments
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del comentario")]
        public string Id { get; set; }

        [BsonElement("postId")]
        [Required]
        [DisplayName("Identificador de la publicación")]
        public string PostId { get; set; }

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

        [BsonElement("isDeleted")]
        [Required]
        [DisplayName("Eliminado")]
        public bool IsDeleted { get; set; }
    }
}