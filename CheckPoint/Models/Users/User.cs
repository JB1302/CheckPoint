using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.Users
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del usuario")]
        public string Id { get; set; }

        [BsonElement("username")]
        [Required]
        [StringLength(100)]
        [DisplayName("Nombre de usuario")]
        public string Username { get; set; }

        [BsonElement("email")]
        [Required]
        [StringLength(150)]
        [EmailAddress]
        [DisplayName("Correo electrónico")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        [Required]
        [StringLength(255)]
        [DisplayName("Hash de contraseña")]
        public string PasswordHash { get; set; }

        [BsonElement("role")]
        [Required]
        [StringLength(50)]
        [DisplayName("Rol")]
        public string Role { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("isActive")]
        [Required]
        [DisplayName("Activo")]
        public bool IsActive { get; set; }
    }
}