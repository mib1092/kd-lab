using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using IMCMS.Web.ViewModels;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    public class TeamController : BaseVersionableController<TeamMember>
    {
        public TeamController(IUnitOfWork uow, IVersionableRepository<TeamMember> repo) : base(uow, repo)
        {

        }

        public override ActiveSection? AdminBarActiveSection
        {
            get
            {
                return ActiveSection.Team;
            }
        }

        public override string[] SlugFields
        {
            get
            {
                return new[] { "Firstname", "Lastname" };
            }
        }

        protected override void OnSaving(TeamMember obj, FormCollection form, EditingType type)
        {
            base.OnSaving(obj, form, type);

            if (type == EditingType.Insert)
            {
                obj.Order = int.MaxValue;
            }

            if (string.IsNullOrEmpty(obj.Email)) obj.Email = ConfigurationManager.AppSettings["DefaultEmail"];
        }

        [HttpPost]
        public ActionResult Order(FormCollection form)
        {
            int i = 0;
            var all = _repo.GetAll().ToList();
            var ids = form["row"].Split(',');

            foreach (var item in ids)
            {
                int id = 0;
                int.TryParse(item, out id);

                all.First(x => x.ID == id).Order = i;
                i++;
            }
            _uow.Commit();
            return Json(new { status = 0 });
        }
    }
}