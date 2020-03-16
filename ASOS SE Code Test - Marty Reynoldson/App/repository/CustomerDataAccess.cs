namespace App
{
    public static class CustomerDataAccess
    {
        private static ICustomerDataAccess _customerDataAccessor;

        public static ICustomerDataAccess CustomerDataAccessor
        {
            get { return _customerDataAccessor ?? (_customerDataAccessor = new CustomerDataAccessImpl()); }
            set { _customerDataAccessor = value; }
        }

        public static void AddCustomer(Customer customer)
        {
            CustomerDataAccessor.AddCustomer(customer);
        }
    }
}
