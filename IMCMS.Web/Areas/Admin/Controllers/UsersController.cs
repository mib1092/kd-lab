using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IMCMS.Common;
using IMCMS.Models;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using IMCMS.Common.Hashing;
using IMCMS.Common.Controllers;
using IMCMS.Web.ViewModels;
using IMCMS.Web.Areas.Admin.ViewModels;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    [AuthorizeRoles(Common.Constants.ROLE_USERS, Common.Constants.ROLE_USERS_IM)]
    public class UsersController : BaseAdminController
	{
        readonly IUnitOfWork _uow;

		readonly IAdminUserRepository _repo;

		readonly IAdminRoleRepository _roleRepo;

		readonly IAdminUserSessionRepository _sessionRepo;

		public UsersController(IUnitOfWork uow, IAdminUserRepository repo, IAdminRoleRepository roleRepo, IAdminUserSessionRepository sessionRepo) : base(uow)
		{
			_uow = uow;
			_repo = repo;
			_roleRepo = roleRepo;
			_sessionRepo = sessionRepo;
		}

        public override ActiveSection? AdminBarActiveSection
        {
            get
            {
                return ActiveSection.Users;
            }
        }

        protected override void Initialize(RequestContext requestContext)
		{
			ViewBag.Roles = _roleRepo.GetAll().ToList();
			base.Initialize(requestContext);
		}

        [AuthorizeRoles(Constants.ROLE_USERS, Constants.ROLE_USERS_IM)]
        public ActionResult Index(string d, string c)
		{
			var q = _repo.GetAll();
			if (String.IsNullOrEmpty(d) || String.IsNullOrEmpty(c))
			{
				d = "asc";
				c = "EmailAddress";
			}
			//TODO: make this dynamic and generic. Look at:
			//http://stackoverflow.com/questions/41244/dynamic-linq-orderby-on-ienumerablet 
			//http://stackoverflow.com/questions/307512/how-do-i-apply-orderby-on-an-iqueryable-using-a-string-column-name-within-a-gene
			bool asc = (d == "asc");
			switch (c)
			{
				case "EmailAddress":
					q = asc ? q.OrderBy(i => i.EmailAddress) : q.OrderByDescending(i => i.EmailAddress);
					break;
				case "Disabled":
					q = asc ? q.OrderBy(i => i.EmailAddress) : q.OrderByDescending(i => i.EmailAddress);
					break;
			}
			ViewBag.Direction = d;
			ViewBag.Column = c;
			
			return View(new AdminBaseViewModel<IEnumerable<AdminUser>> { Item = q.ToList() });
		}

        [AuthorizeRoles(Constants.ROLE_USERS, Constants.ROLE_USERS_IM)]
        public ActionResult Create()
		{
			return View("Edit", new AdminBaseViewModel<AdminUser> { Item = new AdminUser() });
		}

		[HttpPost]
        [AuthorizeRoles(Constants.ROLE_USERS, Constants.ROLE_USERS_IM)]
        public ActionResult Create(AdminBaseViewModel<AdminUser> model, int[] userRoles)
		{
            var user = model.Item;

			// ensure the email address is unique
			if (!_repo.IsEmailUnique(user.EmailAddress))
				ModelState.AddModelError("NonUnique", "The email address is not unique");

			HandlePassword(user, true);


            if (!ModelState.IsValid)
            {
                return View("Edit", new AdminBaseViewModel<AdminUser> { Item = user });
			}

			try
			{
				// handle roles
				AddUpdateRoles(user, userRoles);

				_repo.Add(user);
				_uow.Commit();
				CreatedItem();
                SetListPage(Url.Action("Index"));
				return RedirectToAction("Edit", new { id = user.ID});
			}
			catch (Exception ex)
			{
				RaiseError(ex);
				return View("Edit", new AdminBaseViewModel<AdminUser> { Item = user });
			}
		}

        [AuthorizeRoles(Constants.ROLE_USERS, Constants.ROLE_USERS_IM)]
        public ActionResult Edit(int id)
		{
			var user = _repo.FindById(id);

			if (user == null) throw new HttpException(404, "User not found");

			return View(new AdminBaseViewModel<AdminUser> { Item = user });
		}

		[HttpPost]
        [AuthorizeRoles(Constants.ROLE_USERS, Constants.ROLE_USERS_IM)]
        public ActionResult Edit(int id, AdminBaseViewModel<AdminUser> model, int[] userRoles)
		{
            var user = model.Item;

			// get the user from the database
			var userFromDatabase = _repo.FindById(id);
            user.ID = id;

            // deal with possible password blanks
            HandlePassword(user, false);

			// kind of hack, but let's try to revalidate the modelstate
			ModelState.Clear();
			TryValidateModel(user);

			// let's check to see if the email is unique
			if (userFromDatabase.EmailAddress != user.EmailAddress)
				if (!_repo.IsEmailUnique(user.EmailAddress)) ModelState.AddModelError("NonUnique", "The email address is not unique");


			if (!ModelState.IsValid)
			{
				return View(new AdminBaseViewModel<AdminUser> { Item = user });
			}

			try
			{
				// set the ID from URL to prevent any screwness
				

				userFromDatabase.ExpireAllSessions();
				_sessionRepo.ExpireAllSessionsForUser(userFromDatabase.EmailAddress);
				_uow.Commit();

				_repo.Edit(user);
				AddUpdateRoles(userFromDatabase, userRoles);
				_uow.Commit();

				ModifiedItem();

				if (!Request.RequestContext.HttpContext.User.IsInRole(Constants.ROLE_USERS_IM)
					&& userFromDatabase.EmailAddress == Request.RequestContext.HttpContext.User.Identity.Name)
				{
					return RedirectToAction("Logout", "Account");
				}

                SetListPage(Url.Action("Index"));
                return RedirectToAction("Edit", new { id = user.ID });
			}
			catch (Exception ex)
			{
				RaiseError(ex);
				return View(new AdminBaseViewModel<AdminUser> { Item = user });
			}
		}

        [AuthorizeRoles(Constants.ROLE_USERS, Constants.ROLE_USERS_IM)]
        public ActionResult Sessions(string id)
		{
			IEnumerable<AdminUserSession> sessions;
			int parse = 0;
			if (int.TryParse(id, out parse))
			{
				var user = _repo.FindById(parse);
				sessions = user.Sessions.ToList();

				ViewBag.Username = user.EmailAddress;
			}
			else
			{
				sessions = _sessionRepo.GetAllForUsername(id).ToList();
				ViewBag.Username = id;
			}

			return View(sessions.Where(x => !x.IsExpired).OrderByDescending(x => x.Created));
		}

        [AuthorizeRoles(Constants.ROLE_USERS, Constants.ROLE_USERS_IM)]
        public ActionResult Sessions(FormCollection form, string username)
		{
			_sessionRepo.ExpireAllSessionsForUser(username);
			_uow.Commit();
			ModifiedItem();
            return RedirectToAction("Sessions", new { id = username });
		}

		/// <summary>
		/// Add or update user roles on the requeted user object
		/// </summary>
		/// <param name="user">User object to add/edit roles upon</param>
		/// <param name="roles">IDs of roles to add</param>
		private void AddUpdateRoles(AdminUser user, IEnumerable<int> roles)
		{
			// clear all roles out of the user, so only submitted roles exist
			user.Roles.Clear();

			// if it's null, or empty, roles may not be used?
			if (roles == null) return;

			// loop through, and add them to the current user
			foreach (var role in roles.Select(item => _roleRepo.FindByID(item)).Where(role => role != null))
			{
				user.Roles.Add(role);
			}
		}

		/// <summary>
		/// Deal with inserting and updating passwords
		/// </summary>
		/// <param name="user">Submitted user</param>
		/// <param name="isInserting">Is the method being called for insert user</param>
		private void HandlePassword(AdminUser user, bool isInserting)
		{
			if (isInserting)
			{
				if (String.IsNullOrEmpty(user.Password)) return;

				user.Password = Hash.HashPassword(user.Password);
			}
			else
			{
				if (String.IsNullOrEmpty(user.Password))
				{
					var userFromDatabase = _repo.FindById(user.ID);
					user.Password = userFromDatabase.Password;
				}
				else
				{
					user.Password = Hash.HashPassword(user.Password);
				}
			}
		}
	}
}
