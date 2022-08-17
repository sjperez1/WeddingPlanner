using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // need this for password hasher
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;

public class WeddingController : Controller
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

    public WeddingController(MyContext context)
    {
        _context = context;
    }

    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        if(!isLoggedIn || userid == null)
        {
            return RedirectToAction("Forms", "User");
        }

        List<Wedding> allWeddings = _context.Weddings.Include(t => t.WeddingCreator).Include(t => t.WeddingParticipants).ToList();
        // Console.WriteLine(allWeddings.Count);

        return View("AllWeddings", allWeddings);
    }

    [HttpGet("/weddings/new")]
    public IActionResult NewWedding()
    {
        if(!isLoggedIn)
        {
            return RedirectToAction("Forms", "User");
        }
        return View("NewWedding");
    }

    [HttpPost("/weddings/create")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if(!isLoggedIn || userid == null)
        {
            return RedirectToAction("Forms", "User");
        }

        if(ModelState.IsValid == false)
        {
            return NewWedding();
        }

        newWedding.UserId = (int)userid;
        _context.Weddings.Add(newWedding);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    [HttpPost("/weddings/{weddingId}/attend")]
    public IActionResult Attend(int weddingId)
    {
        if(!isLoggedIn || userid == null)
        {
            return RedirectToAction("Forms", "User");
        }

        UserWeddingParticipation? existingParticipation = _context.UserWeddingParticipations.FirstOrDefault(uwp => uwp.UserId == (int)userid && uwp.WeddingId == weddingId);

        if(existingParticipation != null)
        {
            _context.Remove(existingParticipation);
        }
        else
        {
            UserWeddingParticipation newParticipation = new UserWeddingParticipation(){
                UserId = (int)userid,
                WeddingId = weddingId
            };
        _context.UserWeddingParticipations.Add(newParticipation);
        }
        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }

    [HttpGet("/weddings/{weddingId}")]
    public IActionResult OneWedding(int weddingId)
    {
        if(!isLoggedIn)
        {
            return RedirectToAction("Forms", "User");
        }
        Wedding? oneWedding = _context.Weddings.Include(wedding => wedding.WeddingParticipants).ThenInclude(uwp => uwp.WeddingAttendee).FirstOrDefault(wedding => wedding.WeddingId == weddingId);
        if(oneWedding == null)
        {
            return RedirectToAction("Dashboard");
        }
        return View("OneWedding", oneWedding);
    }
}