using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.Users
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("username")]
        [Required]
        [StringLength(100)]
        [DisplayName("Nombre de usuario")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("email")]
        [Required]
        [StringLength(150)]
        [EmailAddress]
        [DisplayName("Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("passwordHash")]
        [Required]
        [StringLength(255)]
        [DisplayName("Hash de contraseña")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("role")]
        [Required]
        [StringLength(50)]
        [DisplayName("Rol")]
        public string Role { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("isActive")]
        [Required]
        [DisplayName("Activo")]
        public bool IsActive { get; set; }

        [BsonElement("lastLoginAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [DisplayName("Último inicio de sesión")]
        public DateTime? LastLoginAt { get; set; }
    }
}