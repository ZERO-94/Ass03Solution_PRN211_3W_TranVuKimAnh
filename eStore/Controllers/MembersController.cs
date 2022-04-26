using BusinessObject;
using BusinessObject.Models;
using DataAccess.repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace eStore.Controllers
{
    public class MembersController : Controller
    {
        IMemberRepository memberRepository = null;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public MembersController(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            memberRepository = new MemberRepository();
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }

        // GET: MembersController
        public ActionResult Index()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }
            var memberList = memberRepository.GetAllMembers();

            return View(memberList);
        }

        [HttpGet]
        public ActionResult Login()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (email == null || password == null)
            {
                ViewBag.Message = "Email and Password cannot be empty!";
                return View();
            }

            var session = HttpContext.Session;

            if (email.Equals(_config.GetSection("Account").GetSection("Email").Value) && password.Equals(_config.GetSection("Account").GetSection("Password").Value))
            {
                session.SetString("Email", email);
                session.SetString("Role", "Admin");
                return Redirect("../Home/Index");
            }
            else if (memberRepository.CheckLogin(email, password) != null)
            {
                session.SetString("Email", email);
                session.SetString("Role", "Member");
                session.SetInt32("Id", memberRepository.CheckLogin(email, password).MemberId);
                return Redirect("../Home/Index");
            }
            else
            {
                ViewBag.Message = "Email or Password is incorrect!";
                return View();
            }
        }

        public ActionResult Logout()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            session.Clear();
            return RedirectToAction("Index", "Products");
        }

        // GET: MembersController/Details/5
        public ActionResult Details(int id, string? email)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            Member member;
            if (email != null)
            {
                member = memberRepository.GetMemberByEmail(email);
            }
            else
            {
                if (session.GetString("Email").Equals(_config.GetSection("Account").GetSection("Email").Value))
                {
                    member = memberRepository.GetMemberById(id);
                }
                else if (memberRepository.GetMemberByEmail(session.GetString("Email")).MemberId != id)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    member = memberRepository.GetMemberById(id);
                }
            }

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: MembersController/Create
        public ActionResult Create()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            return View();
        }

        // POST: MembersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Member member)
        {
            if (member.Email == null || member.CompanyName == null || member.City == null || member.Country == null || member.Password == null)
            {
                ViewBag.Message = "All fields must be filled to create a new member!";
                return View();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    memberRepository.CreateMember(member);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: MembersController/Edit/5
        public ActionResult Edit(int id)
        {
            var member = memberRepository.GetMemberById(id);
            var session = HttpContext.Session;
            if (session.GetString("Role") == null)
            {
                return RedirectToAction("Login", "Members");
            }
            else if (session.GetString("Role") != "Admin" && session.GetString("Email") != member.Email)
            {
                return RedirectToAction("Index", "Home");
            }

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: MembersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Member member)
        {
            if (member.Email == null || member.CompanyName == null || member.City == null || member.Country == null || member.Password == null)
            {
                ViewBag.Message = "All fields must be filled to update information!";
                return View();
            }

            try
            {
                if (id != member.MemberId)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    memberRepository.UpdateMember(member.MemberId, member);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: MembersController/Delete/5
        public ActionResult Delete(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            var member = memberRepository.GetMemberById(id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: MembersController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                memberRepository.DeleteMember(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }
    }
}
