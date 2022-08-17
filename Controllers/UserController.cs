using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // need this for password hasher
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;

public class UserController : Controller
{
    private int? userid
    {
        get{return HttpContext.Session.GetInt32("UserId");}
    }

    private bool isLoggedIn
    {
        get
        {
            return userid != null;
        }
    }
    
    // the following context things are needed to inject the context service into the controller
    private MyContext _context;

    public UserController(MyContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Forms()
    {
        if(isLoggedIn)
        {
            return RedirectToAction("Dashboard", "Wedding");
        }
        return View("Index");
    }

    [HttpPost("create")]
    public IActionResult CreateUser(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(_context.Users.Any(user => user.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "This email is already in use.");
            }
        }
        if(!ModelState.IsValid)
        {
            return View("Index");
        }

        PasswordHasher<User> hashedPass = new PasswordHasher<User>();
        newUser.Password = hashedPass.HashPassword(newUser, newUser.Password);
        _context.Users.Add(newUser); // have to specify which table you are adding to.
        _context.SaveChanges();

        // The following line is placed after "SaveChanges()" because it allows us to then access the Id from the database. 
        HttpContext.Session.SetInt32("UserId", newUser.UserId);
        return RedirectToAction("Dashboard", "Wedding");
    }

    [HttpPost("loginuser")]
    public IActionResult LoginUser(Login loginUser)
    {
        if(ModelState.IsValid == false)
        {

            return View("Index");
        }

        User? existingUser = _context.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);

        if(existingUser == null)
        {
            ModelState.AddModelError("LoginEmail", "The email/password entered is invalid.");
            return View("Index");
        }
        PasswordHasher<Login> hasher = new PasswordHasher<Login>();
        PasswordVerificationResult checkPassword = hasher.VerifyHashedPassword(loginUser, existingUser.Password, loginUser.LoginPassword);
        if(checkPassword == 0)
        {
            ModelState.AddModelError("LoginPassword", "The email/password entered is invalid.");
            return View("Index");
        }

        // if it reaches this point, it means that there have been no errors, so store in session and redirect to page that requires login.
        HttpContext.Session.SetInt32("UserId", existingUser.UserId);
        return RedirectToAction("Dashboard", "Wedding");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Forms");
    }
}