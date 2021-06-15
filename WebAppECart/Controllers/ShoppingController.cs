using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebAppECart.Models;
using WebAppECart.ViewModel;

namespace WebAppECart.Controllers
{
    public class ShoppingController : Controller
    {
        private AlohaTableRestaurantDBEntities objECartDbEntities;
        private List<ShoppingCartModel> listOfShoppingCartModels;
        public ShoppingController()
        {
            objECartDbEntities = new AlohaTableRestaurantDBEntities();
            listOfShoppingCartModels = new List<ShoppingCartModel>();
        }
        // GET: Shopping
        public ActionResult Index()
        {
            IEnumerable<ShoppingViewModel> listOfShoppingViewModels = (from objItem in objECartDbEntities.Items
                                                                       join objCart in objECartDbEntities.Categories
                                                                       on objItem.categoryId equals objCart.categoryId
                                                                       //where objItem.categoryId == 1
                                                                       select new ShoppingViewModel()
                                                                       {
                                                                           ImagePath = objItem.imagePath,
                                                                           ItemName = objItem.itemName,
                                                                           Description = objItem.description,
                                                                           ItemPrice = objItem.itemPrice,
                                                                           ItemId = objItem.itemId,
                                                                           Category = objCart.categoryName,
                                                                           ItemCode = objItem.itemCode
                                                                       }
                                                                       ).ToList();

            return View(listOfShoppingViewModels);
        }
        [HttpPost]
        public JsonResult Index(string ItemId, int Qty)
        {
            ShoppingCartModel objShoppingCartModel = new ShoppingCartModel();
            Item objItem = objECartDbEntities.Items.Single(model => model.itemId.ToString() == ItemId);

            // If shopping cart counter has value
            if (Session["CartCounter"] != null)
            {
                // creates new shopping cart
                listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            }
            // if there is already same item in a shopping cart
            if (listOfShoppingCartModels.Any(model => model.ItemId == ItemId))
            {
                objShoppingCartModel = listOfShoppingCartModels.Single(model => model.ItemId == ItemId);
                objShoppingCartModel.Quantity = Qty;
                objShoppingCartModel.Total = objShoppingCartModel.Quantity * objShoppingCartModel.UnitPrice;

            }
            // if there is no same item in a shopping cart
            else
            {
                objShoppingCartModel.ItemId = ItemId;
                objShoppingCartModel.ImagePath = objItem.imagePath;
                objShoppingCartModel.ItemName = objItem.itemName;
                objShoppingCartModel.UnitPrice = objItem.itemPrice;
                objShoppingCartModel.Quantity = Qty;
                objShoppingCartModel.Total = Qty * objShoppingCartModel.UnitPrice;
                listOfShoppingCartModels.Add(objShoppingCartModel);
            }

            int ttlQty = 0;
            foreach (var item in listOfShoppingCartModels)
            {
                ttlQty += (int)item.Quantity;
            }

            Session["CartCounter"] = ttlQty;
            Session["CartItem"] = listOfShoppingCartModels;

            return Json(new { Success = true, Counter = listOfShoppingCartModels.Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShoppingCart()
        {
            if (Session["CartCounter"] == null)
            {
                return View();
            }
            else
            {
                listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
                return View(listOfShoppingCartModels);
            }

        }
        [HttpPost]
        // Record in SQL Database
        public ActionResult AddOrder()
        {
            // Order Table
            int OrderId = 0;
            listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            Order orderObj = new Order()
            {
                //orderId=OrderId+1,
                OrderDate = DateTime.Now,
                OrderNumber = String.Format("{0:ddmmyyyyHHmmsss}", DateTime.Now)
            };
            objECartDbEntities.Orders.Add(orderObj);
            objECartDbEntities.SaveChanges();
            OrderId = orderObj.orderId;

            // OrderDetail Table
            foreach (var Item in listOfShoppingCartModels)
            {
                OrderDetail objOrderDetail = new OrderDetail();
                objOrderDetail.orderId = OrderId;
                objOrderDetail.itemId = Item.ItemId;
                objOrderDetail.quantity = Item.Quantity;
                objOrderDetail.unitPrice = Item.UnitPrice;
                objOrderDetail.total = Item.Total;

                objECartDbEntities.OrderDetails.Add(objOrderDetail);
                objECartDbEntities.SaveChanges();
            }

            // Clear the shopping cart
            Session["CartItem"] = null;
            Session["CartCounter"] = null;

            //Order placement flag indicator
            Session["OrderComplete"] = true;

            // Back to the shopping cart view
            return RedirectToAction("ShoppingCart");

        }
        [HttpPost]
        public JsonResult RemoveFromCart(string ItemId)
        {
            listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            foreach (var item in listOfShoppingCartModels)
            {
                if (item.ItemId == ItemId)
                {
                    listOfShoppingCartModels.Remove(item);
                    break;
                }
            }

            int ttlQty = 0;
            foreach (var item in listOfShoppingCartModels)
            {
                ttlQty += (int)item.Quantity;
            }
            Session["CartCounter"] = ttlQty;
            Session["CartItem"] = listOfShoppingCartModels;
            return Json(new { Success = true, Counter = listOfShoppingCartModels.Count }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index");
        }

    }

}