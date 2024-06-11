using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Text.Json;
using Newtonsoft.Json;


namespace MyBookShelf.Controllers
{
    [Route("[controller]/")]
    public class UserController : Controller
    {
        private readonly ContextDb _db;

        public UserController(ContextDb context)
        {
            _db = context;
        }


        private void SetViewDataFromSession() {
            if (HttpContext.Session.GetString("username") == null) {
                ViewData["Username"] = "";
                ViewData["IsAdmin"] = "";

                return;
            }

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");
        }




        [Route("/login")]
        public IActionResult Login()
        {
            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            return View();
            // if (HttpContext.Session.GetString("username") != null && HttpContext.Session.GetString("username") != "") {
            //     return RedirectToAction("Index", "Home");
            // }

            // SetViewDataFromSession();

            // return View();
        }



        [Route("/login")]
        [HttpPost]
        public IActionResult Login(IFormCollection form)
        {
            if (HttpContext.Session.GetString("username") is not null && HttpContext.Session.GetString("username") != "") {
                return RedirectToAction("Index", "Home");
            }


            if (form is null)
                return View();

            string username = form["username"].ToString();
            string password = form["password"].ToString();
            if(username == "" || password == ""){
                ViewData["LoginMessage"] = "Both fields must be filled";
                return View();
            }

            var results = from user in _db.User 
                        where user.Username == username
                        select user;

            if (results.Count() == 0){
                ViewData["LoginMessage"] = "Invalid username or password";
                return View();
            }
            User userModel = results.First();

            if(CreateMD5Hash(password) == userModel.Password){
                HttpContext.Session.SetString("username", username);
                HttpContext.Session.SetInt32("userid", userModel.Id);
                HttpContext.Session.SetString("isadmin", userModel.IsAdmin.ToString());
                HttpContext.Session.SetString("token", userModel.Token);


                ViewData["Username"] = HttpContext.Session.GetString("username");
                ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");


                ViewData["LoginMessage"] = "Login successful";
            }
            else{
                ViewData["LoginMessage"] = "Invalid username or password";
            }

            // SetViewDataFromSession();

            return RedirectToAction("AllBooks", "Bookshelf");
        }

        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [Route("/profile")]
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("username") == null) {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            var results = from user in _db.User 
                        where user.Id == HttpContext.Session.GetInt32("userid")
                        select new User {
                            Id = user.Id,
                            Username = user.Username,
                            Email = user.Email,
                            IsAdmin = user.IsAdmin,
                            Token = user.Token
                        };
            User userModel = results.First();
            return View(userModel);
        }




        [Route("/register")]
        public IActionResult Register()
        {
            // if(HttpContext.Session.GetString("username") is null || HttpContext.Session.GetString("isadmin").ToString() != "True"){
            //     return RedirectToAction("Index", "Home");
            // }
            if (HttpContext.Session.GetString("username") == null) {
                Console.WriteLine("FFFFFFFFFFFFFFFFF");
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("isadmin") != "True") {
                return RedirectToAction("Index", "Home");
            }

            // SetViewDataFromSession();
            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

            return View();
        }


        [HttpPost]
        [Route("/register")]
        public IActionResult Register(IFormCollection form)
        {
            // if(HttpContext.Session.GetString("username") is null || HttpContext.Session.GetString("isadmin").ToString() != "True"){
            //     return RedirectToAction("Index", "Home");
            // }
            if (HttpContext.Session.GetString("username") == null) {
                Console.WriteLine("NNNNNNNNNNNNNNN");
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("isadmin") != "True") {
                return RedirectToAction("Index", "Home");
            }

            if (form is null)
                return View();


            string username = form["username"].ToString() ?? "";
            string password1 = form["password1"].ToString() ?? "";
            string password2 = form["password2"].ToString() ?? "";
            string email = form["email"].ToString() ?? "";
            bool isadmin = false;

            if(username == "" || password1 == "" | password2 == "" || email == ""){
                ViewData["RegisterMessage"] = "Enter all data!";
                return View();
            }
            if(password1 != password2){
                ViewData["RegisterMessage"] = "Passwords are not the same!";
                return View();
            }

            int found = 
                    (
                        from user in _db.User
                        where user.Username == username || user.Email == email
                        select user
                    ).Count();
            if(found != 0){
                ViewData["RegisterMessage"] = "User already exists!";
                return View();
            }

            string token = "";
            do{
                token = GenerateRandomToken();
                found = 
                    (
                       from user in _db.User
                       where user.Token == token
                       select user 
                    ).Count();
            } while(found != 0);

            User newUser = new User{
                Username = username,
                Password = CreateMD5Hash(password1),
                Email = email,
                IsAdmin = false,
                Token = token
            };
            
            _db.User.Add(newUser);
            _db.SaveChanges();
            ViewData["RegisterMessage"] = "Added new user!";

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");
            // SetViewDataFromSession();
            
            return View();
        }

        



            public static string CreateMD5Hash(string input) {
                using (var md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    var sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }

            public static string GenerateRandomToken(){
                Random random = new Random();
                byte[] bytes = new byte[10];
                random.NextBytes(bytes);

                return Convert.ToHexString(bytes);
            }
    }
}


