namespace ECommerce.Shared.Enums
{
    public class OrderEnum
    {
        public enum OrderStatus
        {
            Pending, //chưa thanh toán
            Processing, // thanh toán xong - đang xử lí
            Shipped, // đã vận chuyển
            Delivered,//đã giao
            Cancelled//đã hủy 
        }
        public enum PaymentStatus
        {
            Pending,
            Completed,
            Failed,
            Refunded
        }
        public enum PaymentMethod
        {
            VnPay,
            Momo,
            CashOnDelivery
        }
        public enum ShippingMethod
        {
            Standard,
            Express,
            Overnight
        }
    }
}
