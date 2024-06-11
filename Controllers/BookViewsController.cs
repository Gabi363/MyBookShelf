using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net;
using Newtonsoft.Json;

public struct BookWithStatus {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public List<Opinion> Opinions { get; set; }
    public double Average { get; set; }
}



namespace MyBookShelf.Controllers
{
    [Route("[controller]/")]
    public class BookshelfController : Controller
    {
        private readonly ContextDb _db;

        public BookshelfController(ContextDb context)
        {
            _db = context;
        }




        [Route("/books")]
        public IActionResult AllBooks()
        {
            if(HttpContext.Session.GetString("username") == null){
                return RedirectToAction("Index", "Home");
            }

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            // List<Book> books = new List<Book>
            //         (
            //             from book in _db.Book 
            //             select new Book{
            //                 Id = book.Id,
            //                 Title = book.Title,
            //                 Author = book.Author,
            //                 Genre = book.Genre,
            //                 Description = book.Description
            //             }      
            //         );
            List<Book> books = new List<Book>();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5033/api/Book");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                books = JsonConvert.DeserializeObject<List<Book>>(result);
            }
            

            return View(books);
        }


        [Route("/addbook")]
        public IActionResult AddBook()
        {
            if(HttpContext.Session.GetString("username") is null && ViewData["username"] is null ){
                return RedirectToAction("Index", "Home");
            }
            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            return View();
        }


        [Route("/addbook")]
        [HttpPost]
        public IActionResult AddBook(IFormCollection form)
        {
            if(HttpContext.Session.GetString("username") is null && ViewData["username"] is null ){
                return RedirectToAction("Index", "Home");
            }

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            if (form is null)
                return View();


            string title = form["title"].ToString() ?? "";
            string author = form["author"].ToString() ?? "";
            string genre = form["genre"].ToString() ?? "";
            string description = form["description"].ToString() ?? "";

            if(title == "" | author == "" || genre == "" || description == ""){
                ViewData["AddBookMessage"] = "Enter all data!";
                return View();
            }

            int found = 
                    (
                        from book in _db.Book
                        where book.Title == title && book.Author == author
                        select book
                    ).Count();
            if(found != 0){
                ViewData["AddBookMessage"] = "Book already exists!";
                return View();
            }


            Book newBook = new Book{
                Title = title,
                Author = author,
                Genre = genre,
                Description = description
            };
            
            _db.Book.Add(newBook);
            _db.SaveChanges();

            // var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5033/api/Informacja");
            // httpWebRequest.ContentType = "application/json";
            // httpWebRequest.Method = "POST";

            // using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            // {
            //     string json = JsonSerializer.Serialize(new
            //                 {
            //                     Dane = "test2",
            //                     Priorytetowa = false
            //                 });
            //     streamWriter.Write(json);
            // }

            // var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            // using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            // {
            //     var result = streamReader.ReadToEnd();
            //     Console.WriteLine(result);
            // }



            Console.WriteLine("ADDED");

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");
            
            return RedirectToAction("AllBooks");        
        }



        [Route("/books/{id}")]
        public IActionResult BookDetails(int id)
        {
            if(HttpContext.Session.GetString("username") == null){
                return RedirectToAction("Index", "Home");
            }


            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            Book book = new Book();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5033/api/Book/" + id);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                book = JsonConvert.DeserializeObject<Book>(result);
            }

            BookWithStatus model = new BookWithStatus {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Description = book.Description,
            };

            int userid = HttpContext.Session.GetInt32("userid") ?? -1;
            var opinions = 
                    (
                        from o in _db.Opinion
                        where o.BookId == book.Id
                        select o
                    ).ToList();
            foreach(var o in opinions){
                var user = 
                        (
                            from u in _db.User
                            where o.UserId == u.Id
                            select u
                        ).First();
                o.User = user;
            }
            if(_db.Opinion.Where(o => o.BookId == book.Id).Count() > 0){
                var ave = _db.Opinion
                                .Where(o => o.BookId == book.Id)
                                .Average(o => o.Rank);
                model.Average = ave;
            }

            model.Opinions = opinions;

            var hasStatus = 
                    (
                        from s in _db.BookStatuses
                        where s.BookId == id && s.UserId == HttpContext.Session.GetInt32("userid")
                        select s
                    ).Count();
            if(hasStatus != 0){
                var status = 
                    (
                        from s in _db.BookStatuses
                        where s.BookId == id && s.UserId == HttpContext.Session.GetInt32("userid")
                        select s
                    ).First();
                model.Status = status.Status;
                
            }

            ViewData["bookid"] = id;

            return View(model);
        }


        [Route("/books/{id}")]
        [HttpPost]
        public IActionResult BookDetails(int id, string status)
        {
            if(HttpContext.Session.GetString("username") == null){
                return RedirectToAction("Index", "Home");
            }

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            int userid = HttpContext.Session.GetInt32("userid") ?? -1;
            BookStatus newStatus = new BookStatus{
                BookId = id,
                UserId = userid,
                Status = status
            };
            var result = 
                    (   from s in _db.BookStatuses
                        where s.UserId == userid && s.BookId == id
                        select s
                    ).FirstOrDefault();
            if(result != default){
                _db.Remove(result);
            }
            _db.Add(newStatus);
            _db.SaveChanges();

            return RedirectToAction("BookDetails", new { id });

        }


        [Route("/shelf")]
        public IActionResult Shelf()
        {
            if(HttpContext.Session.GetString("username") == null){
                return RedirectToAction("Index", "Home");
            }

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            Console.WriteLine("xxx"+HttpContext.Session.GetInt32("userid"));

            List<BookStatus> books = new List<BookStatus>();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5033/api/Book/user/" + HttpContext.Session.GetInt32("userid"));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                books = JsonConvert.DeserializeObject<List<BookStatus>>(result);
            }

            return View(books);
        }


        [Route("/shelf")]
        [HttpPost]
        public IActionResult Shelf(string bookid)
        {
            if(HttpContext.Session.GetString("username") == null){
                return RedirectToAction("Index", "Home");
            }

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            // List<BookStatus> books = new List<BookStatus>();
            // var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5033/api/Book/user/" + HttpContext.Session.GetInt32("userid"));
            // httpWebRequest.ContentType = "application/json";
            // httpWebRequest.Method = "GET";

            // var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            // using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            // {
            //     var result = streamReader.ReadToEnd();
            //     books = JsonConvert.DeserializeObject<List<BookStatus>>(result);
            // }
            int userid = HttpContext.Session.GetInt32("userid") ?? -1;
            Console.WriteLine(userid+"x"+bookid);
            int id = Int32.Parse(bookid);
            var status = 
                    (
                        from s in _db.BookStatuses
                        where s.BookId == id && s.UserId == userid
                        select s
                    ).FirstOrDefault();

            if(status != default)
            {
                _db.BookStatuses.Remove(status);
                _db.SaveChanges();
            }
            

            return RedirectToAction("Shelf");        
        }


        [Route("/addreview/{id}")]
        public IActionResult AddReview(int id)
        {
            if(HttpContext.Session.GetString("username") == null){
                return RedirectToAction("Index", "Home");
            }
            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            return View();
        }


        [Route("/addreview/{id}")]
        [HttpPost]
        public IActionResult AddReview(int id, IFormCollection form)
        {
            if(HttpContext.Session.GetString("username") == null){
                return RedirectToAction("Index", "Home");
            }

            if (form is null)
                return View();

            int rank;
            if(form["rank"] == "" || !Int32.TryParse(form["rank"], out rank) || Int32.Parse(form["rank"]) > 5 || Int32.Parse(form["rank"]) < 1){
                ViewData["ReviewMessage"] = "Wrong rank!";
                return View();
            }

            rank = Int32.Parse(form["rank"]);
            string? review = form["review"];
            int userid = HttpContext.Session.GetInt32("userid") ?? -1;

            Opinion newOpinion = new Opinion{
                BookId = id,
                UserId = userid,
                Rank = rank
            };

            if(review is not null){
                 newOpinion.Review = review;
            }

            _db.Opinion.Add(newOpinion);
            _db.SaveChanges();


            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            return RedirectToAction("BookDetails", new { id });
        }
        
    }
}


