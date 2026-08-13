using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebAppECart.Models;
using WebAppECart.ViewModel;

namespace WebAppECart.Controllers
{
    public class ItemController : Controller
    {
        private AlohaTableRestaurantDBEntities objECartDbEntities;
        private List<ItemViewModel> ListOfItemViewModel;

        public ItemController()
        {
            objECartDbEntities = new AlohaTableRestaurantDBEntities();
            ListOfItemViewModel = new List<ItemViewModel>();
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
        public ActionResult ItemList()
        {
            IEnumerable<ItemListModel> ListOfItemViewModel = (from objItem in objECartDbEntities.Items
                                                              join objCategory in objECartDbEntities.Categories
                                                              on objItem.categoryId equals objCategory.categoryId
                                                              select new ItemListModel()
                                                              {
                                                                  
                                                                  ItemId = objItem.itemId,
                                                                  ImagePath = objItem.imagePath,
                                                                  ItemName = objItem.itemName,
                                                                  ItemCode = objItem.itemCode,
                                                                  ItemPrice = objItem.itemPrice,
                                                                  Description = objItem.description,
                                                                  ItemCategory = objCategory.categoryName
                                                              }
                                                                       ).ToList();


            return View(ListOfItemViewModel);


        }
        public ActionResult ItemSort(string sortOrder)
        {
            // sort by name||category descending order 
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_Desc" : "";
            ViewBag.CateSortParm = sortOrder == "Category" ? "Category" : "Category_Desc";
            var Items = from serchItem in objECartDbEntities.Items
                        join serchCate in objECartDbEntities.Categories
                        on serchItem.categoryId equals serchCate.categoryId
                        select serchItem;
            switch (sortOrder)
            {
                case "Name_Desc":
                    Items = Items.OrderByDescending(serchItem => serchItem.itemName );
                    break;
                case "Category":
                    Items = Items.OrderBy(serchItem => serchItem.categoryId);
                    break;
                case "Category_Desc":
                    Items = Items.OrderByDescending(serchItem => serchItem.categoryId);
                    break;
                default:
                    Items = Items.OrderBy(serchItem => serchItem.itemName);
                    break;
            }
            return View(Items.ToList());
        }
    }
}