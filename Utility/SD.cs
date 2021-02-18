using Lokanta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Utility
{
    public static class SD
    {
        public const string ManagerUser = "Manager";
        public const string CustomerEndUser = "Customer";
        public const string ShoppingCartCount = "ShoppingCartCount";
        public const string ssCouponCode = "CouponCode";

        public const string StatusSubmitted= "Submitted";
        public const string StatusInProcess = "Begin Prepared";
        public const string StatusReady= "Ready for Pickup";
        public const string StatusCompleted= "Completed";
        public const string StatusCancelled= "Cancelled";

        public const string PaymentStatusPending= "Pending";
        public const string PaymentStatusApproved= "Approved";
        public const string PaymentStatusRejected = "Rejected";


        public static double DiscountPrice(Coupon coupon,double OrderTotalOrginal)
        {
            if(coupon==null)
            {
                return Math.Round(OrderTotalOrginal, 2);
            }
            else
            {
                if(coupon.MinimunAmount> OrderTotalOrginal)
                {
                    return Math.Round(OrderTotalOrginal, 2);
                }
                else
                {
                    if(int.Parse(coupon.CouponType)==(int) Coupon.ECouponType.Dollar)
                    {
                        return Math.Round(OrderTotalOrginal - coupon.Discount, 2);
                    }
                    else
                    {
                        return Math.Round(OrderTotalOrginal - (OrderTotalOrginal* (coupon.Discount/100)), 2);
                    }
                }
            }
        }
    }
}
