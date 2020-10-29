using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BooksTracker.Controllers;
using BooksTracker.Models;
using Microsoft.VisualBasic;
using BooksTracker.Models.Exceptions;
using System.Security.Cryptography.X509Certificates;
using System.Linq.Expressions;

namespace BooksTracker.Controllers
{
    public class BookController : Controller
    {
        
        /*Modify “BookController” (Controller):
        Modify “CreateBook()”.
        Check that “Title” is unique before saving books to the database.
        If “Title” is not unique do not add the new book.
        Ensure this comparison is case insensitive and trimmed.
        Set “CheckedOutDate” to today’s date.
        Set “ExtensionCount” to 0.
        Keep the logic to set DueDate and ReturnedDate.
        “PublishedDate” cannot be in the future.
        “Title” cannot exceed its size in the database.
        “Title” cannot be empty or whitespace.
        Trim “Title” before saving it’s value.
        Display itemized errors for every field that has an issue.

            */


        public IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public IActionResult Create(string title, string authorID, string publicationDate)
        {
            title = title != null ? title.Trim() : null;
            authorID = authorID != null ? authorID.Trim() : null;
            publicationDate = publicationDate != null ? publicationDate.Trim() : null;

            ViewBag.Authors = GetAuthors();

            if (Request.Query.Count > 0)
            {
                try
                {  // Get parameters come in as a string, so we have to convert those to the data types required.
                        Book createdBook = CreateBook(title, authorID, publicationDate);

                        ViewBag.Success = $"You have successfully checked out {createdBook.Title} until {createdBook.DueDate}.";
                    
                }
                catch (Exception e)
                {
                    // All expected data not provided, so this will be our error state.
                    //ViewBag.Error = $"Unable to check out book: {e.Message}";

                   ViewBag.Exception = e;

                    // Store our data to re-add to the form.
                   
                    ViewBag.BookTitle = title;
                    ViewBag.AuthorID = authorID;                  
                    ViewBag.PublicationDate = publicationDate;

                }
            }
            // else
            // No request, so this will be our inital state.

            return View();
        }
        public IActionResult List(string filter)
        {
                         
                if (filter == "on")
                {
                    ViewBag.Books = GetOverdueBooks();
                     ViewBag.Filter = "on";
                }
                else
                {
                    ViewBag.Books = GetBooks();
                }
            return View();
        }

        public IActionResult Details(string id, string command)
        {
            IActionResult result;


            // If ID wasn't provided, or if it won't parse to an int, or the ID doesn't exist.
            if (id == null || !int.TryParse(id, out int temp))
            {
                ViewBag.Error = "No book selected.";
                result = View();
            }
            else
            {
                if (command=="delete")
                {
                    DeleteBookByID(int.Parse(id));
                    result = RedirectToAction("List");
                }
                else
                {
                    try
                    {

                        if (command == "extend")
                        {
                            ExtendDueDateForBookByID(int.Parse(id));
                        }

                        else if (command == "return")
                        {
                            ReturnBookByID(int.Parse(id));
                        }
                    }
                    catch(Exception e)
                    {
                        ViewBag.Error = e;
                    }
                    
                    Book target = GetBookByID(int.Parse(id));
                    if (target == null)
                    {
                        ViewBag.Error = new Exception("No book selected.");
                    }
                    else
                    {
                        ViewBag.Book = target;
                    }
                    result = View();
                }
                
                }
            return result;
        }
        public Book CreateBook(string title, string authorID, string publicationDate)
        {
            /*
             Check that “Title” is unique before saving books to the database.
            If “Title” is not unique do not add the new book.
            Ensure this comparison is case insensitive and trimmed.
            */
            int authorIDParsed = 0;
            DateTime publicationDateParsed = new DateTime();
            Book newBook = null;

            using (LibraryContext context = new LibraryContext())
            {

                BookTitleValidationException exception = new BookTitleValidationException();

            if (string.IsNullOrWhiteSpace(title))
            {
                exception.SubExceptions.Add(new Exception("Title was not provided."));
            }

            else
                {
                  if (title.Length > 30)
                   {
                exception.SubExceptions.Add(new Exception("Title cannot be more than 30 characters long."));
                   }

                    if (context.Books.Where(x => x.Title.ToUpper() == title.ToUpper()).Count() > 0)
                    {
                        exception.SubExceptions.Add(new Exception("Title already exists."));
                    }
            }
                if (string.IsNullOrWhiteSpace(authorID))
                {
                    exception.SubExceptions.Add(new Exception("Author was not provided."));
                }
                else
                {

                    // Because of how TryParse works, if it succeeds, the value will be in publicationDateParsed, meaning we don't have to parse it again later.
                    if (!int.TryParse(authorID.Trim(), out authorIDParsed))
                    {
                        exception.SubExceptions.Add(new Exception("Author is not a selection."));
                    }
                    else
                    {
                        if (context.Authors.Where(x => x.ID == authorIDParsed).Count() < 1)
                        {
                            exception.SubExceptions.Add(new Exception("Author does not exist."));
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(publicationDate))
                {
                exception.SubExceptions.Add(new Exception("Publication date was not provided."));
                 }

            else
                {
              
                if (!DateTime.TryParse(publicationDate, out publicationDateParsed))
                {
                    exception.SubExceptions.Add(new Exception("Publication Date is not valid."));
                }
                else {
                    if (publicationDateParsed > DateTime.Today)
                    {
                        exception.SubExceptions.Add(new Exception("Publication Date cannot be in future."));
                    }
                }
            }

                // If any exceptions have been generated by any validation, throw them as one bundled exception.
                if (exception.SubExceptions.Count > 0)
                {
                    throw exception;
                }

                newBook = new Book()
            {
                Title = title,
                AuthorID = authorIDParsed,                
                PublicationDate = publicationDateParsed,
                CheckedOutDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(14),
                ReturnedDate = null,
                ExtensionCount = 0
            };
                context.Books.Add(newBook);
                context.SaveChanges();
            }

            return newBook;
        }

        public Book GetBookByID(int id)
        {
            Book target;
            using (LibraryContext context = new LibraryContext())
            {
                target = context.Books.Where(x => x.ID == id).Single();
                target.Author = context.Authors.Where(x => x.ID == target.AuthorID).SingleOrDefault();
            }
            return target;
        }
        public List<Author> GetAuthors()
        {
            List<Author> authors;
            using (LibraryContext context = new LibraryContext())
            {
                authors = context.Authors.ToList();
                foreach (Author author in authors)
                {
                    author.Books = context.Books.Where(x => x.AuthorID == author.ID).ToList();
                }
            }
            return authors;
        }

        //Add a “GetBooks()” method to get a list of all books in the database using Entity Framework.
        public List<Book> GetBooks()
        {
            List<Book> books;
            using (LibraryContext context = new LibraryContext())
            {
                books = context.Books.ToList();
                foreach (Book book in books)
                {
                    book.Author = context.Authors.Where(x => x.ID == book.AuthorID).Single();
                }
            }
            return books;
        }

        public List<Book> GetOverdueBooks()
        {
            /* Add a “GetOverdueBooks()” method.
         //Return a list of books with “DueDate” in the past, that have no “ReturnedDate”.*/

            List<Book> books;
            using (LibraryContext context = new LibraryContext())
            {
                books = context.Books.Where(x => x.DueDate < DateTime.Today && x.ReturnedDate == null).ToList();
                foreach (Book book in books)
                {
                    book.Author = context.Authors.Where(x => x.ID == book.AuthorID).Single();
                }
            }
            return books;
        }
        public void ExtendDueDateForBookByID(int id)
        {
            /**
             * A book can only be extended a maximum of 3 times.
                If a user tries to extend a book a fourth time do not update the database
                Display an error on the page calling the method informing the user they will have to speak to the librarian.
            Overdue books cannot be extended.
                Display an error on the page calling the method informing the user they will have to speak to the librarian.
             */
           

            using (LibraryContext context = new LibraryContext())
            {
                Book target = context.Books.Where(x => x.ID == id).Single();
                if (target.DueDate < DateTime.Today && target.ReturnedDate == null)
                {
                    throw new OverdueException("That book is overdue, it cannot be extended. Please see the librarian.");
                }
                if (target.ExtensionCount >= 3)
                {
                    throw new Exception(" Overdue books cannot be extended. You will have to speak to the librarian.");
                   
                }
                target.DueDate = DateTime.Today.AddDays(7);
                target.ExtensionCount++;
                context.SaveChanges();
            } 
           
        }

        public void DeleteBookByID(int id)
        {
            // Modify “DeleteBookByID()” to delete a book from the database using Entity Framework.
            
            using (LibraryContext context = new LibraryContext())
            {
                Book target=context.Books.Where(x => x.ID == id).Single();
                context.Books.Remove(target);
                context.SaveChanges();
            }
        }

        public void ReturnBookByID(int id)
        {
            /*
             * Add a “ReturnBookByID()” method.
            Set the returned date of the specified book to today’s date.
            Overdue books cannot be returned.
            Display an error on the page calling the method informing the user they will have to speak to the librarian.
            */
            
                 
            using (LibraryContext context = new LibraryContext())
            {
                Book target = context.Books.Where(x => x.ID == id).Single();

                if (target.DueDate< DateTime.Today && target.ReturnedDate==null)
                {
                   throw new Exception("Overdue books cannot be returned without paying a fee. You will have to speak to the librarian.");
                }
                target.ReturnedDate = DateTime.Today;
                context.SaveChanges();
            }
            }

    }
}

// Code borowed: @ link https://github.com/TECHCareers-by-Manpower/4.1-MVC/tree/master/MVC_4Point1

