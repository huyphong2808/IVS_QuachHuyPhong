using BLL;
using Components;
using Models.Model;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductManagement.Controllers
{
    public class CategoryController : Controller
    {
        public CategoryBLL _categoryBLL;
        public CategoryController()
        {
            _categoryBLL = new CategoryBLL();
        }
        public ActionResult Index(SearchCategoryModel Model, int? page)
        {
            var pageNumber = page ?? 1;
            int total = new int();
            List<CategoryViewModel> lstModel = new List<CategoryViewModel>();
            List<GetCatetoryModel> lstcombobox = new List<GetCatetoryModel>();
            if (!string.IsNullOrEmpty(Session["code_category"] as string))
            {
                Model.code = Session["code_category"].ToString();
            }
            if (!string.IsNullOrEmpty(Session["name_category"] as string))
            {
                Model.name = Session["name_category"].ToString();
            }
            if (Session["parent_id_category"] as int? != null)
            {
                Model.parent_id = (int)Session["parent_id_category"];
            }
            _categoryBLL.Search(Model, out lstModel, out total, pageNumber);
            var list = new StaticPagedList<CategoryViewModel>(lstModel, pageNumber, 15, total);
            ViewBag.ListSearch = lstModel.OrderByDescending(x => x.id);
            _categoryBLL.GetCategory(true, out lstcombobox);
            ViewBag.lstcombobox = lstcombobox;
            ViewBag.page = 0;
            if (page != null)
            {
                ViewBag.page = pageNumber - 1;
            }
            return View(new Tuple<SearchCategoryModel, IPagedList<CategoryViewModel>>(Model, list));
        }
        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(SearchCategoryModel Model, int? page)
        {
            var pageNumber = page ?? 1;
            List<CategoryViewModel> lstModel = new List<CategoryViewModel>();
            List<GetCatetoryModel> lstcombobox = new List<GetCatetoryModel>();
            int total = new int();
            _categoryBLL.Search(Model, out lstModel, out total, pageNumber);
            var list = new StaticPagedList<CategoryViewModel>(lstModel, pageNumber, 15, total);
            ViewBag.ListSearch = lstModel.OrderByDescending(x => x.id);
            Session["code_category"] = Model.code;
            Session["name_category"] = Model.name;
            Session["parent_id_category"] = Model.parent_id;
            TempData["CountResult"] = total.ToString() + " row(s) found!";
            _categoryBLL.GetCategory(true, out lstcombobox);
            ViewBag.lstcombobox = lstcombobox;
            return View(new Tuple<SearchCategoryModel, IPagedList<CategoryViewModel>>(Model, list));
        }
        #region ADD
        public ActionResult Add()
        {
            List<GetCatetoryModel> lstcombobox = new List<GetCatetoryModel>();
            _categoryBLL.GetCategory(true, out lstcombobox);
            ViewBag.Category = lstcombobox;
            return View();
        }

        [HttpPost]
        public ActionResult Add(CategoryModel Model)
        {
            List<GetCatetoryModel> lstcombobox = new List<GetCatetoryModel>();
            _categoryBLL.GetCategory(true, out lstcombobox);
            ViewBag.Category = lstcombobox;
            if (!ModelState.IsValid)
            {
                return View(Model);
            }
            List<string> lstMsg = new List<string>();
            int returnCode = _categoryBLL.Insert(Model, out lstMsg);
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                if (lstMsg != null)
                {
                    for (int i = 0; i < lstMsg.Count(); i++)
                    {
                        ModelState.AddModelError(string.Empty, lstMsg[i]);
                    }
                }
                return View(Model);
            }
            TempData["Success"] = "Inserted Successfully!";
            return RedirectToAction("Index");
        }
        #endregion
        #region View & Edit
        public ActionResult View(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            List<GetCatetoryModel> lstcombobox = new List<GetCatetoryModel>();
            _categoryBLL.GetCategory(true, out lstcombobox);
            ViewBag.Category = lstcombobox;
            CategoryViewModel Model = new CategoryViewModel();
            int returnCode = _categoryBLL.GetDetail(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new CategoryViewModel();
            }

            return View(Model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            List<GetCatetoryModel> lstcombobox = new List<GetCatetoryModel>();
            _categoryBLL.GetCategory(true, out lstcombobox);
            ViewBag.Category = lstcombobox;
            CategoryModel Model = new CategoryModel();
            int returnCode = _categoryBLL.GetByID(id, out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new CategoryModel();
            }
            return View(Model);
        }
        [HttpPost]
        public ActionResult Edit(CategoryModel Model)
        {
            List<GetCatetoryModel> lstcombobox = new List<GetCatetoryModel>();
            _categoryBLL.GetCategory(true, out lstcombobox);
            ViewBag.Category = lstcombobox;
            if (ModelState.IsValid)
            {
                List<string> lstMsg = new List<string>();
                int returnCode = _categoryBLL.Update(Model, out lstMsg);

                if (!((int)Common.ReturnCode.Succeed == returnCode))
                {
                    if (lstMsg != null)
                    {
                        for (int i = 0; i < lstMsg.Count(); i++)
                        {
                            ModelState.AddModelError(string.Empty, lstMsg[i]);
                        }
                    }
                    return View(Model);
                }
                TempData["Success"] = "Updated Successfully!";
                return RedirectToAction("View", new { @id = Model.id });
            }
            return View(Model);
        }
        #endregion

        [HttpPost]
        public ActionResult Delete(List<int> id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            List<string> lstMsg = new List<string>();

            int returnCode = _categoryBLL.Delete(id, out lstMsg);
            if (((int)Common.ReturnCode.Succeed == returnCode))
            {
                return Json(new { Message = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}