using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LoginReg.Models;
using Microsoft.AspNetCore.Identity;
namespace LoginReg.Controllers;



public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("users/create")]
    public IActionResult CreateUser(User newUser)
    {
        System.Console.WriteLine(
            "Start of Route"
        );
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Success");
        }

        if (ModelState.IsValid)
        {

            PasswordHasher<User> Hasher = new PasswordHasher<User>();

            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            System.Console.WriteLine("Middle of Route");
            return RedirectToAction("Success");
        }
        else
        {
            System.Console.WriteLine(" eND OF ROUTE");
            return View("Index");
        }
    }

    public IActionResult Login(LoginUser userSubmission)
    {
        if (ModelState.IsValid)
        {

            User? userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.LEmail);

            if (userInDb == null)
            {
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
            }

            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LPassword);                                    // Result can be compared to 0 for failure        
            if (result == 0)
            {
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            return RedirectToAction("Success");
        }
        else
        {
            return View("Index");
        }
    }
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("UserId");
        return RedirectToAction("Index");
    }

    [SessionCheck]
    [HttpGet("success")]
    public IActionResult Success()
    {
        return View("Success");
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Success");
        }
        else
        {
            return View("Index");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            int? userId = context.HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }




}
