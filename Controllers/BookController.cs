using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyBookShelf.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ContextDb _context;

        public BookController(ContextDb context)
        {
            _context = context;
        }

        // GET: /Book
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBook()
        {
          if (_context.Book == null)
          {
              return NotFound();
          }
          var result = await _context.Book.ToListAsync();
        // var result = 
        //         (
        //             from b in _context.Book
        //             join o in _context.Opinion on b.Id equals o.BookId into op
        //             from ord in op.DefaultIfEmpty()
        //             group ord by object. into gp
        //             select new Book{
        //                 Id = gp.Key.Id,
        //                 Title = gp.Key.Title,
        //                 Author = gp.Key.Author,
        //                 Genre = gp.Key.Genre,
        //                 Description = gp.Key.Description
        //             }
                    
        //         ).Distinct().ToList();
        //   result.OrderBy(x => x.)
            return result;
        }

        // GET: /Book/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
          if (_context.Book == null)
          {
              return NotFound();
          }
          var book = await _context.Book.FindAsync(id);


            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpGet("user/{userid}")]
        public async Task<ActionResult<IEnumerable<BookStatus>>> GetUserBooks(int userid)
        {
            if (_context.Book == null)
            {
                return NotFound();
            }
            var statuses = await _context.BookStatuses.ToListAsync();
            foreach(var s in statuses){
                if(s.UserId != userid) statuses.Remove(s);
                s.Book = 
                    (
                        from b in _context.Book
                        where b.Id == s.BookId
                        select b
                    ).First();
            }
            return statuses;
            
        }


        // PUT: /Book/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: /Book
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
          if (_context.Book == null)
          {
              return Problem("Entity set 'ContextDb.Book'  is null.");
          }
            _context.Book.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: /Book/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Book == null)
            {
                return NotFound();
            }
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
