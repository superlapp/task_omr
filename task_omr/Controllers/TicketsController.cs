using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using task_omr.Helpers;
using Microsoft.AspNet.Identity;

namespace task_omr.Controllers
{
    public class TicketsController : Controller
    {
        // GET: Tickets
        //---------------------------------------------------------------------
        public ActionResult BusStops()
        {
            var v = new TicketsDBEntities();
            return View(v.BusStops.ToList());
        }

        public ActionResult SearchBusStops()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchBusStopResult(SearchOptions so)
        {
            TicketsDBEntities db = new TicketsDBEntities();
            List<SearchBusStopResult> result = new List<SearchBusStopResult>();
            //
            int fc = 0;
            int p1 = 0;
            int p2 = 0;
            int p3 = 0;
            //
            bool add = false;
            //
            var busStops = db.BusStops.Where(x => x.Name.Trim().ToUpper().Contains(so.BusStopName.ToUpper()));
            foreach (BusStop bs in busStops)
            {
                var voyages = db.Voyages.Where(x => x.DepBusStopID.Equals(bs.Id) || x.ArrBusStopID.Equals(bs.Id));
                //
                foreach (Voyage v in voyages)
                {
                    //if (so.IsDepStation == true)
                    //{
                    //    if (so.IsDepStation.Equals(true))
                    //    {
                    //        fc++;
                    //        if (v.DepBusStopID.Equals(bs.Id))
                    //        {
                    //            p1++;
                    //        }
                    //    }
                    //    if (so.IsArrStation.Equals(true))
                    //    {
                    //        fc++;
                    //        if (v.ArrBusStopID.Equals(bs.Id))
                    //        {
                    //            p2++;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (so.IsDepStation == true && so.IsArrStation == true)
                    //    {
                    //        if (so.IsDepStation.Equals(true))
                    //        {
                    //            fc++;
                    //            fc++;
                    //            p1++;
                    //            p2++;
                    //        }
                    //    }
                    //}

                    if (so.UseDT == true)
                    {
                        fc++;
                        DateTime dt = Convert.ToDateTime(so.DT);
                        if (v.DepDateTime == dt)
                        {
                            p3++;
                        }
                        bool k = true;
                    }

                    //---------------------------------------------------------
                    if (fc == (p1 + p2 + p3))
                    {
                        SearchBusStopResult ss = new SearchBusStopResult();
                        ss.BusStopName = bs.Name;
                        ss.DepDateTime = v.DepDateTime.ToString();
                        ss.VoyageName = v.VoyageName;
                        result.Add(ss);
                    }
                    //
                    fc = 0;
                    p1 = 0;
                    p2 = 0;
                    p3 = 0;
                }
            }
            //
            return View(result.ToList());
        }
        //---------------------------------------------------------------------
        public ActionResult Voyages()
        {
            var v = new TicketsDBEntities();
            return View(v.Voyages.ToList());
        }

        public ActionResult ReserveTicket(int voyageID)
        {
            TicketsDBEntities db = new TicketsDBEntities();
            Ticket ticket = new Ticket();
            //
            Session["voyageID"] = voyageID;
            //
            string id = User.Identity.GetUserId();
            var v = db.AspNetUsers.Where(x => x.Id == id.ToString());
            foreach (AspNetUser u in v)
            {
                ticket.Passenger = u.LastName.Trim() + ", " + u.FirstName.Trim();
                ticket.PassengerSeatNum = 1;
            }
            return View(ticket);
        }

        [HttpPost]
        public ActionResult ReserveTicket(Ticket ticket)
        {
            TicketsDBEntities db = new TicketsDBEntities();
            //
            Order order = new Order();
            order.VoyageID = Convert.ToInt32(Session["voyageID"]);
            order.Status = Helper.ORDER_STATUS_RESERVED;
            db.Orders.Add(order);
            db.SaveChanges();
            //
            ticket.OrderID = order.Id;
            ticket.Status = Helper.ORDER_STATUS_RESERVED;
            db.Tickets.Add(ticket);
            db.SaveChanges();
            //
            return RedirectToAction("OrdersInfo", "Tickets");
        }
        //---------------------------------------------------------------------
        public ActionResult OrdersInfo()
        {
            var db = new TicketsDBEntities();
            OrderInfo oi;
            List<OrderInfo> ois = new List<OrderInfo>();
            //-------------------------
            string id = User.Identity.GetUserId();
            AspNetUser user = db.AspNetUsers.First(x => x.Id == id);
            string fullname = user.LastName.Trim() + ", " + user.FirstName.Trim();
            //-------------------------
            var tickets = db.Tickets.Where(x => x.Passenger.Equals(fullname));
            foreach (Ticket ticket in tickets)
            {
                Order order = db.Orders.First(x => x.Id.Equals(ticket.OrderID));
                Voyage voyage = db.Voyages.First(x => x.Id.Equals(order.VoyageID));
                //
                oi = new OrderInfo();
                oi.OrderId = order.Id;
                oi.VoyageId = voyage.Id;
                oi.VoyageName = voyage.VoyageName.Trim();
                oi.DepDT = voyage.DepDateTime;
                oi.ArrDT = voyage.ArrDateTime;
                oi.SeatNumber = ticket.PassengerSeatNum;
                oi.Price = voyage.TicketCost.ToString() + " RUB";
                oi.Status = order.Status;
                ois.Add(oi);
            }
            //
            return View(ois.ToList());
        }

        //[Authorize]
        public ActionResult ProcessOrder(string process, int orderId)
        {
            var db = new TicketsDBEntities();
            //
            switch (process)
            {
                case "purchase":
                    Order order = db.Orders.First(x => x.Id.Equals(orderId));
                    order.Status = Helper.ORDER_STATUS_PURCHASED;
                    Ticket ticket = db.Tickets.First(x => x.OrderID.Equals(orderId));
                    ticket.Status = Helper.ORDER_STATUS_PURCHASED;
                    db.SaveChanges();
                    //
                    ViewBag.Message = "Yout order has been purchased";
                    break;
                case "delete":
                    db.Orders.Remove(db.Orders.First(x => x.Id.Equals(orderId)));
                    db.Tickets.Remove(db.Tickets.First(x => x.OrderID.Equals(orderId)));
                    db.SaveChanges();
                    //
                    ViewBag.Message = "Your order has been deleted";
                    break;
            }
            return View();
        }
    }
}