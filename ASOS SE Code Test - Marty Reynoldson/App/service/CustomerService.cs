using System;

namespace App
{
    public class CustomerService
    {
        private readonly ICompanyRepository _companyRepository;

        public CustomerService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public bool AddCustomer(string firstName, string lastName, string email, DateTime dateOfBirth, int companyId)
        {
            if (!HasValidCustomerInputs(firstName, lastName, email, dateOfBirth)) return false;

            var company = _companyRepository.GetById(companyId);
            var customer = PerformCreditChecks(new Customer
            {
                Company = company,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firstName,
                Surname = lastName
            });
           
            if (HasLowCredit(customer)) return false;

            CustomerDataAccess.AddCustomer(customer);

            return true;
        }

        public Customer PerformCreditChecks(Customer customer)
        {
            var company = customer.Company;
            int defaultCreditMultiplyer = 1;

            // made more sense to use classification instead of company name
            switch (company.Classification)
            {
                case Classification.Gold:
                    customer.HasCreditLimit = false;
                    break;
                case Classification.Silver:
                    customer = CalculateCreditLimit(customer, defaultCreditMultiplyer * 2);
                    break;
                case Classification.Bronze:
                    customer = CalculateCreditLimit(customer, defaultCreditMultiplyer);
                    break;
            }
            return customer;
        }

        public Customer CalculateCreditLimit(Customer customer, int multiplyer)
        {
            customer.HasCreditLimit = true;
            using (var customerCreditService = new CustomerCreditServiceClient())
            {
                var creditLimit = customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
                customer.CreditLimit = creditLimit * multiplyer;
            }
            return customer;
        }

        // TODO move responsiblities of helpers to util/other Classes

        private bool HasLowCredit(Customer customer)
        {
            return customer.HasCreditLimit && customer.CreditLimit < 500;
        }

        private bool HasValidCustomerInputs(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            return HasValidName(firstName, lastName) && HasValidEmail(email) && HasValidAge(dateOfBirth);
        }

        private bool HasValidName(string firstName, string lastName)
        {
            return !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName);
        }

        private bool HasValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }

        private bool HasValidAge(DateTime dateOfBirth)
        {
            return IsOlderThanTwentyOne(dateOfBirth);
        }

        private bool IsOlderThanTwentyOne(DateTime dateOfBirth)
        {
            var currentDateTime = DateTime.Now;
            int ageOfCustomer = currentDateTime.Year - dateOfBirth.Year;
            if (currentDateTime.Month < dateOfBirth.Month || (currentDateTime.Month == dateOfBirth.Month && currentDateTime.Day < dateOfBirth.Day)) ageOfCustomer--;
            return ageOfCustomer > 21;
        }
    }
}
