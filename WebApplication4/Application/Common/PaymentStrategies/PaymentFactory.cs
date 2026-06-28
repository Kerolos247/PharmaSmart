namespace WebApplication4.Application.Common.PaymentStrategies
{
    public static class PaymentFactory
    {
        public static IDiscountStrategy GetStrategy()
        {
            var today = DateTime.Today;


            if (today.Month == 10 && today.Day == 6)
                return new October6Discount();


            if (today.Month == 2 && today.Day == 14)
                return new ValentineDiscount();

            if (today.Month == 12 && today.Day == 25)
                return new ChristmasDiscount();




            return new NoDiscount();
        }
    }
}
