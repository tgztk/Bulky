﻿using System.Security.Claims;
using Bulky.DataAccess.Repository.Contracts;
using Bulky.Models.Entities;
using Bulky.Models.ViewModels;
using Bulky.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Product = Bulky.Models.Entities.Product;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area(areaName: "Admin")]
[Authorize]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    [BindProperty]
    public OrderViewModel OrderViewModel { get; set; }

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int orderId)
    {
        OrderViewModel = new()
        {
            OrderHeader = _unitOfWork.OrderHeaderRepository.Get(o => o.Id.Equals(orderId), includeProperties: nameof(ApplicationUser))!,
            OrderDetails = _unitOfWork.OrderDetailRepository.GetAll(o => o.OrderHeaderId.Equals(orderId), includeProperties: nameof(Product))
        };

        return View(OrderViewModel);
    }

    [HttpPost]
    [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
    public IActionResult UpdateOrderDetail()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeaderRepository
            .Get(o => o.Id.Equals(OrderViewModel.OrderHeader.Id));

        if (orderHeaderFromDb is not null)
        {
            orderHeaderFromDb.Name = OrderViewModel.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderViewModel.OrderHeader.City;
            orderHeaderFromDb.State = OrderViewModel.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderViewModel.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderViewModel.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order details updated successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id }); ;
        }

        return NotFound();
    }

    [HttpPost]
    [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
    public IActionResult StartProcessing()
    {
        _unitOfWork.OrderHeaderRepository.UpdateStatus(
            id: OrderViewModel.OrderHeader.Id,
            orderStatus: StaticDetails.StatusInProcess);

        _unitOfWork.Save();
        TempData["success"] = "Order details updated successfully.";
        return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id }); ;
    }


    [HttpPost]
    [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
    public IActionResult ShipOrder()
    {
        var orderHeader = _unitOfWork.OrderHeaderRepository.Get(o => o.Id.Equals(OrderViewModel.OrderHeader.Id));

        orderHeader.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
        orderHeader.Carrier = OrderViewModel.OrderHeader.Carrier;
        orderHeader.OrderStatus = StaticDetails.StatusShipped;
        orderHeader.ShippingDate = DateTime.Now;

        if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
        {
            orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        }

        _unitOfWork.OrderHeaderRepository.Update(orderHeader);
        _unitOfWork.Save();

        TempData["success"] = "Order shipped successfully.";
        return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
    }


    [HttpPost]
    [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
    public IActionResult CancelOrder()
    {
        var orderHeader = _unitOfWork.OrderHeaderRepository.Get(o => o.Id.Equals(OrderViewModel.OrderHeader.Id));

        if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id,
                StaticDetails.StatusCancelled,
                StaticDetails.StatusRefunded);
        }
        else
        {
            _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id,
                StaticDetails.StatusCancelled,
                StaticDetails.StatusCancelled);
        }

        _unitOfWork.Save();
        TempData["success"] = "Order cancelled successfully.";
        return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
    }

    [HttpDelete]
    public IActionResult Delete([FromRoute(Name = "id")] int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var order = _unitOfWork.OrderHeaderRepository.Get(p => p.Id.Equals(id));

        if (order is not null)
        {
            _unitOfWork.OrderHeaderRepository.Remove(order);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        return NotFound();
    }

    #region API

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> orderHeaderList;

        if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
        {
            orderHeaderList = _unitOfWork.OrderHeaderRepository
                .GetAll(includeProperties: nameof(ApplicationUser));
        }
        else
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            orderHeaderList = _unitOfWork.OrderHeaderRepository
                .GetAll(o => o.ApplicationUserId.Equals(userId), includeProperties: nameof(ApplicationUser));
        }

        switch (status)
        {
            case "paymentPending":
                orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus.Equals(StaticDetails.PaymentStatusPending));
                break;
            case "inProcess":
                orderHeaderList = orderHeaderList.Where(o => o.OrderStatus.Equals(StaticDetails.StatusInProcess));
                break;
            case "completed":
                orderHeaderList = orderHeaderList.Where(o => o.OrderStatus.Equals(StaticDetails.StatusShipped));
                break;
            case "approved":
                orderHeaderList = orderHeaderList.Where(o => o.OrderStatus.Equals(StaticDetails.StatusApproved));
                break;
            default:
                break;
        }

        return Json(new { data = orderHeaderList });
    }

    #endregion
}
