using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BKBSports.DAL;
using BKBSports.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BKBSports.Controllers
{
    public class ArticleController : Controller
    {
        private BKBSportsContext db = new BKBSportsContext();

        // GET: Article
        public ActionResult Index()
        {
            return View(db.Articles.ToList());
        }

        // GET: Article/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArticleModel articleModel = db.Articles.Find(id);
            if (articleModel == null)
            {
                return HttpNotFound();
            }
            return View(articleModel);
        }

        // GET: Article/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Article/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "articleId,articleCreateDate,articleUpdateTimestamp,authorId,layout,approvalIndicator,articleImage,articleContent")] ArticleModel articleModel)
        {
            if (ModelState.IsValid)
            {
                db.Articles.Add(articleModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(articleModel);
        }

        // GET: Article/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArticleModel articleModel = db.Articles.Find(id);
            if (articleModel == null)
            {
                return HttpNotFound();
            }
            return View(articleModel);
        }

        // POST: Article/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "articleId,articleCreateDate,articleUpdateTimestamp,authorId,layout,approvalIndicator,articleImage,articleContent")] ArticleModel articleModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(articleModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(articleModel);
        }

        // GET: Article/Delete/5
        public ActionResult Delete(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //ArticleModel articleModel = db.Articles.Find(id);
            //if (articleModel == null)
            //{
                return HttpNotFound();
            //}
            //return View(articleModel);
        }

        // POST: Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ArticleModel articleModel = db.Articles.Find(id);
            db.Articles.Remove(articleModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // -- Method to Authorize the logged in User to prevent hacks and updates that arent meant for original user
        private int AuthorizeLoggedInUser()
        {
            int userId = 0;
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (manager.FindById(User.Identity.GetUserId()) != null)
            {
                var currentUser = manager.FindById(User.Identity.GetUserId());
                userId = currentUser.UserProfile.userId;
                return userId;
            }
            else
            {
                return userId;
            }
        }

        //-- Method to find the logged in users account type: 1 = Public, 2 = Author, 3 = Admin --//
        private string ProfileTypeLoggedInUser()
        {
            string profileType = "";
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (manager.FindById(User.Identity.GetUserId()) != null)
            {
                var currentUser = manager.FindById(User.Identity.GetUserId());
                profileType = Convert.ToString(currentUser.UserProfile.profileType);
                return profileType;
            }
            else
            {
                return profileType = "Public";
            }
        }

        //-- Method To Updload/Unload the byte array for the article image --//
        public FileContentResult ArticleImage()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = AuthorizeLoggedInUser();
                if (userId == 0)
                {
                    string fileName = HttpContext.Server.MapPath(@"~/Images/noImg.png");
                    byte[] imageData = null;
                    FileInfo fileInfo = new FileInfo(fileName);
                    long imageFileLength = fileInfo.Length;
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    imageData = br.ReadBytes((int)imageFileLength);
                    return File(imageData, "image/png");
                }
                var articles = HttpContext.GetOwinContext().Get<BKBSportsContext>();
                var article = articles.Articles.Where(x => x.authorId == userId).FirstOrDefault();
                return new FileContentResult(article.articleImage, "image/png");
            }
            else
            {
                string fileName = HttpContext.Server.MapPath(@"~/Images/noImg.png");
                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
                return File(imageData, "image/png");
            }
        }
    }

}
