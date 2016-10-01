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
        //---------------------------------------------------------------------
        #region BusStops

        // GET: /Tickets/BusStop
        public ActionResult BusStops()
        {
            var db = new TicketsDBEntities();
            return View(db.BusStops.ToList());
        }

        // GET: /Tickets/SearchBusStops
        public ActionResult SearchBusStops()
        {
            return View();
        }

        // POST: /Tickets/SearchBusStopsResult
        [HttpPost]
        public ActionResult SearchBusStopResult(SearchOptions so)
        {
            List<SearchBusStopResult> result = new List<SearchBusStopResult>();
            //Number of conditions for the search
            int nc = 0;
            //Number of passed conditions
            int pc1 = 0;
            int pc2 = 0;
            int pc3 = 0;
            //Select all bus stops by name
            var db = new TicketsDBEntities();
            var busStops = db.BusStops.Where(x => x.Name.Trim().ToUpper().Contains(so.BusStopName.ToUpper()));
            foreach (BusStop bs in busStops)
            {
                //Select all voyages by bus stop ID
                var voyages = db.Voyages.Where(x => x.DepBusStopID.Equals(bs.Id) || x.ArrBusStopID.Equals(bs.Id));
                //
                foreach (Voyage v in voyages)
                {
                    //If checked departure/arrival conditions
                    if (so.IsDepStation == true && so.IsArrStation == true)
                    {
                        //Increase the number of conditions
                        nc++;
                        //Increase the number of passed conditions
                        pc1++;
                    }
                    else
                    {
                        //If checked departure condition
                        if (so.IsDepStation == true)
                        {
                            nc++;
                            if (v.DepBusStopID.Equals(bs.Id))
                            {
                                pc1++;
                            }
                        }
                        else
                        {
                            //If checked arrival condition
                            if (so.IsArrStation == true)
                            {
                                nc++;
                                if (v.ArrBusStopID.Equals(bs.Id))
                                {
                                    pc2++;
                                }
                            }
                        }
                    }
                    //If checked date condition
                    if (so.UseDT == true)
                    {
                        nc++;
                        DateTime dt = Convert.ToDateTime(so.DT);
                        if (v.DepDateTime == dt)
                        {
                            pc3++;
                        }
                    }
                    //Cheking the number of conditions and the number of passed conditions
                    if (nc != 0)
                    {
                        if (nc == (pc1 + pc2 + pc3))
                        {
                            //If the number of conditions equals the number of passed conditions
                            //Add the result
                            SearchBusStopResult ss = new SearchBusStopResult();
                            ss.BusStopName = bs.Name;
                            ss.DepDateTime = v.DepDateTime.ToString();
                            ss.VoyageName = v.VoyageName;
                            result.Add(ss);
                        }
                        //Reset counters
                        nc = 0;
                        pc1 = 0;
                        pc2 = 0;
                        pc3 = 0;
                    }
                }
            }
            //
            return View(result.ToList());
        }

        #endregion
        //---------------------------------------------------------------------
        #region Reservation

        // GET: /Tickets/Voyages
        public ActionResult Voyages()
        {
            var db = new TicketsDBEntities();
            return View(db.Voyages.ToList());
        }

        // GET: /Tickets/ReserveTicket
        public ActionResult ReserveTicket(int voyageID)
        {
            //Create a new ticket
            var ticket = new Ticket();
            //Get the ID of the selected voyage
            Session["voyageID"] = voyageID;
            //Get the ID of logged user
            var id = User.Identity.GetUserId();
            //Search the logged user by id
            var db = new TicketsDBEntities();
            var v = db.AspNetUsers.Where(x => x.Id == id.ToString());
            foreach (AspNetUser u in v)
            {
                //Fill the new ticket information
                ticket.Passenger = u.LastName.Trim() + ", " + u.FirstName.Trim();
                ticket.PassengerSeatNum = 1;
            }
            // GET: /Tickets/ReserveTicket
            return View(ticket);
        }

        // POST: /Tickets/OrdersInfo
        [HttpPost]
        public ActionResult ReserveTicket(Ticket ticket)
        {
            var db = new TicketsDBEntities();
            //Create a new order
            var order = new Order();
            order.VoyageID = Convert.ToInt32(Session["voyageID"]);
            //Set status
            order.Status = Helper.ORDER_STATUS_RESERVED;
            db.Orders.Add(order);
            //Save order
            db.SaveChanges();
            //-------------------------
            ticket.OrderID = order.Id;
            //Set status
            ticket.Status = Helper.ORDER_STATUS_RESERVED;
            db.Tickets.Add(ticket);
            //Save ticket
            db.SaveChanges();
            //Display orders of the current user
            return RedirectToAction("OrdersInfo", "Tickets");
        }

        // GET: /Tickets/OrdersInfo
        public ActionResult OrdersInfo()
        {
            OrderInfo orderInfo;
            //Create a list to display information
            List<OrderInfo> orderInfoList = new List<OrderInfo>();
            //Construct the user's full name
            if (User != null)
            {
                var db = new TicketsDBEntities();
                var id = User.Identity.GetUserId();
                var user = db.AspNetUsers.First(x => x.Id == id);
                var fullname = user.LastName.Trim() + ", " + user.FirstName.Trim();
                //Select all tickets by the passenger
                var tickets = db.Tickets.Where(x => x.Passenger.Equals(fullname));
                foreach (Ticket ticket in tickets)
                {
                    Order order = db.Orders.First(x => x.Id.Equals(ticket.OrderID));
                    Voyage voyage = db.Voyages.First(x => x.Id.Equals(order.VoyageID));
                    //Filling all the available information
                    orderInfo = new OrderInfo();
                    orderInfo.OrderId = order.Id;
                    orderInfo.VoyageId = voyage.Id;
                    orderInfo.VoyageName = voyage.VoyageName.Trim();
                    orderInfo.DepDT = voyage.DepDateTime;
                    orderInfo.ArrDT = voyage.ArrDateTime;
                    orderInfo.SeatNumber = ticket.PassengerSeatNum;
                    orderInfo.Price = voyage.TicketCost.ToString() + " RUB";
                    orderInfo.Status = order.Status;
                    orderInfoList.Add(orderInfo);
                }
            }
            //
            return View(orderInfoList.ToList());
        }

        // GET: /Tickets/OrdersInfo
        public ActionResult ProcessOrder(string process, int orderId)
        {
            var db = new TicketsDBEntities();
            //Selection of actions depending on the process
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
        
        #endregion
        //---------------------------------------------------------------------
    }
}