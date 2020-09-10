using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebAppECart.Models;
using WebAppECart.ViewModel;

namespace WebAppECart.Controllers
{
    public class ItemController : Controller
    {
        private ECartDBEntities objECartDbEntities;
        public ItemController()
        {
            objECartDbEntities = new ECartDBEntities();
        }
        // GET: Item
        public ActionResult Index()
        {
            ItemViewModel objItemViewModel = new ItemViewModel();
            objItemViewModel.CategorySelectListItem = (from objCart in objECartDbEntities.Categories
                                                       select new SelectListItem()
                                                       {
                                                           Text = objCart.categoryName,
                                                           Value = objCart.categoryId.ToString(),
                                                           Selected = true
                                                       });
            return View(objItemViewModel);
        }

        [HttpPost]
        public JsonResult Index(ItemViewModel objItemViewModel)
        {
            string NewImage = Guid.NewGuid() + Path.GetExtension(objItemViewModel.ImagePath.FileName);
            objItemViewModel.ImagePath.SaveAs(Server.MapPath("~/Images/" + NewImage));

            Item objItem = new Item();
            objItem.imagePath = "~/Images/" + NewImage;
            objItem.categoryId = objItemViewModel.CategoryId;
            objItem.description = objItemViewModel.Description;
            objItem.itemCode = objItemViewModel.ItemCode;
            objItem.itemId = Guid.NewGuid();
            objItem.itemName = objItemViewModel.ItemName;
            objItem.itemPrice = objItemViewModel.ItemPrice;
            objECartDbEntities.Items.Add(objItem);
            objECartDbEntities.SaveChanges();

            return Json(new { Success = true, Message = "Item is added Successfully." }, JsonRequestBehavior.AllowGet);
            //return Json("HHHH", JsonRequestBehavior.AllowGet);
        }
    }
}