using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BrightIdeas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginReg.Controllers
{
    public class HomeController : Controller
    {
        private BrightContext _context;

        public HomeController(BrightContext context){
            _context = context;
        }
        public IActionResult Index()
        {
            

            return View("Index");
        }
        [HttpPost]
        [Route("UserReg")]
        public IActionResult UserReg(User NewUser, string ConfirmPassword)
        {
            if(NewUser.Password != ConfirmPassword){
                return View("Index");
            }
            if(ModelState.IsValid){
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                _context.Add(NewUser);
                _context.SaveChanges();

                User LoggedIn = _context.users.Last();
                HttpContext.Session.SetString("UserName", LoggedIn.Name);
                HttpContext.Session.SetInt32("UserId", LoggedIn.UserId);
                HttpContext.Session.SetString("Alias", LoggedIn.Alias);

                return RedirectToAction("Success");
            }
            else{
                return View("Index");
            }
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(string Email, string Password)
        {
            //Check email in DB
            User RetrievedUser = _context.users.SingleOrDefault(User => User.Email == Email);
            var Hasher = new PasswordHasher<User>();
            if(RetrievedUser != null){
                if(0 != Hasher.VerifyHashedPassword(RetrievedUser, RetrievedUser.Password, Password)){
                    HttpContext.Session.SetString("UserName", RetrievedUser.Name);
                    HttpContext.Session.SetInt32("UserId", RetrievedUser.UserId);
                    HttpContext.Session.SetString("Alias", RetrievedUser.Alias);
                    return RedirectToAction("Success");
                }
                else{
                    return View("Index");
                }
            }
            
            return View("Index");
        }
        [Route("bright_ideas")]
        public IActionResult Success()
        {
            ViewBag.UserId = (int)HttpContext.Session.GetInt32("UserId");
            ViewBag.Alias = HttpContext.Session.GetString("Alias");
            List<Post> AllPosts = _context.posts.Include(p => p.Creator).Include(u => u.Users).ToList();
            ViewBag.Posts = AllPosts;
            return View("Success");
        }
        [HttpPost]
        public IActionResult AddPost(string PostText)
        {
            int UserId = (int)HttpContext.Session.GetInt32("UserId");
            Post NewPost = new Post{
                PostText = PostText,
                CreatorId = UserId,
            };
            _context.posts.Add(NewPost);
            _context.SaveChanges();

            return RedirectToAction("Success");
        }
        [Route("Like/{id}")]
        public IActionResult Like(int id){
            int UserId = (int)HttpContext.Session.GetInt32("UserId");
            Like NewLike = new Like{
                PostId = id,
                UserId = UserId,
            };
            List<Like> CheckPostId = _context.likes.Where(u => u.UserId == UserId).ToList();
            if(CheckPostId.Exists(p => p.PostId == id)){
                return RedirectToAction("Success");
            }
            else{
                _context.likes.Add(NewLike);
                _context.SaveChanges();
                
                return RedirectToAction("Success");
            }
        }
        [Route("users/{id}")]
        public IActionResult UserPage(int id){
            int UserId = (int)HttpContext.Session.GetInt32("UserId");
            User CurrentUser = _context.users.Include(p => p.Posts).SingleOrDefault(a => a.UserId == id);
            List<Post> UserPosts = _context.posts.Where(p => p.CreatorId == UserId).ToList();
            ViewBag.UserPosts = UserPosts;
            return View("User", CurrentUser);
        }
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [Route("bright_ideas/{id}")]
        public IActionResult Post(int id){
            Post CurrentPost = _context.posts.Include(p => p.Creator).SingleOrDefault(i => i.PostId == id);
            List<Like> PostLikes = _context.likes.Where(p => p.PostId == id).Include(u => u.User).ToList();
            ViewBag.PostLikes = PostLikes;
            return View("Post", CurrentPost);
        }
        [Route("Delete/{id}")]
        public IActionResult DeletePost(int id){
            Post DeletePost = _context.posts.SingleOrDefault(p => p.PostId == id);
            _context.posts.Remove(DeletePost);
            _context.SaveChanges();
            return RedirectToAction("Success");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
