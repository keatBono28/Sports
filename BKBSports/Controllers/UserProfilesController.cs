using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BKBSports.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BKBSports.Controllers
{
    public class UserProfilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserProfiles
        public ActionResult Index()
        {
            return View(db.UserProfiles.ToList());
        }

        // GET: UserProfiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // GET: UserProfiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userId,firstName,lastName,preferredName,dateOfBirth,phoneNumber,profileCreationDate,profileTimestamp,profileImage,profileType")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                db.UserProfiles.Add(userProfile);
                db.SaveChanges(); 
                return RedirectToAction("Index");
            }

            return View(userProfile);
        }

        // GET: UserProfiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST: UserProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "profileImage")]UserProfile userProfile, bool useOldImage = false)
        {
            int userId = AuthorizeLoggedInUser();
            byte[] imageData = null;
            if (useOldImage == false)
            {
                // Convert the user upload to byte array
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase poImageFile = Request.Files["profileImageUpdate"];
                    using (var binary = new BinaryReader(poImageFile.InputStream))
                    {
                        imageData = binary.ReadBytes(poImageFile.ContentLength);
                    }
                }
            }
            UserProfile oldProfile = db.UserProfiles.Find(userId);
            if (ModelState.IsValid)
            {
                var newInfo = db.UserProfiles.Find(AuthorizeLoggedInUser());
                newInfo.firstName = userProfile.firstName;
                newInfo.lastName = userProfile.lastName;
                newInfo.preferredName = userProfile.preferredName;
                newInfo.phoneNumber = userProfile.phoneNumber;
                newInfo.dateOfBirth = userProfile.dateOfBirth;
                newInfo.profileCreationDate = oldProfile.profileCreationDate;
                newInfo.profileUpdateTimestamp = DateTime.Now;
                if (useOldImage == true)
                {
                    newInfo.profileImage = oldProfile.profileImage;
                }
                else
                {
                    // Double Check the input before saving
                    if (imageData != null)
                    {
                        newInfo.profileImage = imageData;
                    }
                    else
                    {
                        // If we got here, the user unchecked the box
                        // but did not upload the new image. 
                        // Therefor, set image to old image to avoid null
                        newInfo.profileImage = oldProfile.profileImage;
                    }
                }
                // Save the changes
                db.SaveChanges();
                // Return the user back their profile details
                return RedirectToAction("Details", new { id = userProfile.userId });
            }
            return View(userProfile);
        }

        // GET: UserProfiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST: UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProfile userProfile = db.UserProfiles.Find(id);
            db.UserProfiles.Remove(userProfile);
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

        //-- Method To Updload/Unload the byte array for the profile image --//
        public FileContentResult ProfileImage()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.Identity.GetUserId();
                if (userId == null)
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
                var bdUsers = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                var userImage = bdUsers.Users.Where(x => x.Id == userId).FirstOrDefault();
                return new FileContentResult(userImage.UserProfile.profileImage, "image/png");
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
