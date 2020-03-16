using NUnit.Framework;
using System;
using App;
using Moq;

namespace AppTests
{
    [TestFixture()]
    public class CustomerServiceTests
    {
        [Test()]
        public void AddCustomer_GivenValidInputs_ReturnsTrue()
        {
            var mockCompanyRepository = new Mock<ICompanyRepository>();
            var mockCompany = new Company
            {
                Classification = Classification.Gold,
                Id = 4,
                Name = "The Company"
            };

            CustomerDataAccess.CustomerDataAccessor = NSubstitute.Substitute.For<ICustomerDataAccess>();
            var mockCustomerDataAccess = new Mock<ICustomerDataAccess>();
            CustomerDataAccess.CustomerDataAccessor = mockCustomerDataAccess.Object;

            bool expected = true;

            mockCompanyRepository.Setup(x => x.GetById(4)).Returns(mockCompany);

            CustomerService customerService = new CustomerService(mockCompanyRepository.Object);

            var actual = customerService.AddCustomer("Joe", "Bloggs", "joe.bloggs@adomain.com", new DateTime(1980, 3, 27), 4);
            Assert.AreEqual(expected, actual);
        }
    }
}
