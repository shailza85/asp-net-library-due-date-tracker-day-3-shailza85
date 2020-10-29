using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BooksTracker.Models;

namespace BooksTracker.Models
{
   
    [Table("book")]
    public partial class Book
    {
        /**
         *Modify “Book” (Model):
            Add a property “ExtensionCount” - int(10), not nullable.
            Update your seed data for this table to include values for this field.
            Add a migration.
            Update the database.

         */
        [Key]
        [Column("ID", TypeName = "int(10)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [Column(TypeName = "varchar(30)")]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime PublicationDate { get; set; }
        [Column(TypeName = "datetime")]

        public DateTime CheckedOutDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DueDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReturnedDate { get; set; }

        [Column("ExtensionCount", TypeName = "int(10)")]
        public int ExtensionCount { get; set; }

        [Column("AuthorID", TypeName = "int(10)")]
        public int AuthorID { get; set; }

        // This attribute specifies which database field is the foreign key. Typically in the child (many side of the 1-many).
        [ForeignKey(nameof(AuthorID))]

        // InverseProperty links the two virtual properties together.
        [InverseProperty(nameof(Models.Author.Books))]
        public virtual Author Author { get; set; }
    }
}


