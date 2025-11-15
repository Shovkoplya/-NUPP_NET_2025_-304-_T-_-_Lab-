using Microsoft.AspNetCore.Mvc;
using Restaurant.REST.Models;
using Restaurant.REST.Services;
using Restaurant.Infrastructure.Models;

namespace Restaurant.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        /// <summary>
        /// Отримати всіх клієнтів
        /// </summary>
        /// <returns>Список клієнтів</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
        {
            var customers = await _customerService.ReadAllAsync();
            var customerDtos = customers.Select(c => MapToDto(c)).ToList();
            return Ok(customerDtos);
        }

        /// <summary>
        /// Отримати клієнта за ID
        /// </summary>
        /// <param name="id">ID клієнта</param>
        /// <returns>Дані клієнта</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetById(Guid id)
        {
            var customer = await _customerService.GetWithProfileAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = $"Клієнта з ID {id} не знайдено" });
            }

            return Ok(MapToDto(customer));
        }

        /// <summary>
        /// Створити нового клієнта
        /// </summary>
        /// <param name="createDto">Дані для створення клієнта</param>
        /// <returns>Створений клієнт</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.FullName))
            {
                return BadRequest(new { message = "Повне ім'я обов'язкове" });
            }

            if (string.IsNullOrWhiteSpace(createDto.PhoneNumber))
            {
                return BadRequest(new { message = "Номер телефону обов'язковий" });
            }

            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                FullName = createDto.FullName,
                PhoneNumber = createDto.PhoneNumber,
                LoyaltyPoints = createDto.LoyaltyPoints
            };

            var created = await _customerService.CreateAsync(customer);
            if (!created)
            {
                return BadRequest(new { message = "Не вдалося створити клієнта" });
            }

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, MapToDto(customer));
        }

        /// <summary>
        /// Оновити дані клієнта
        /// </summary>
        /// <param name="id">ID клієнта</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <returns>Оновлений клієнт</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] UpdateCustomerDto updateDto)
        {
            var customer = await _customerService.ReadAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = $"Клієнта з ID {id} не знайдено" });
            }

            if (!string.IsNullOrWhiteSpace(updateDto.FullName))
            {
                customer.FullName = updateDto.FullName;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber))
            {
                customer.PhoneNumber = updateDto.PhoneNumber;
            }

            if (updateDto.LoyaltyPoints.HasValue)
            {
                customer.LoyaltyPoints = updateDto.LoyaltyPoints.Value;
            }

            var updated = await _customerService.UpdateAsync(customer);
            if (!updated)
            {
                return BadRequest(new { message = "Не вдалося оновити клієнта" });
            }

            var updatedCustomer = await _customerService.GetWithProfileAsync(id);
            return Ok(MapToDto(updatedCustomer!));
        }

        /// <summary>
        /// Видалити клієнта
        /// </summary>
        /// <param name="id">ID клієнта</param>
        /// <returns>Статус видалення</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var customer = await _customerService.ReadAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = $"Клієнта з ID {id} не знайдено" });
            }

            var deleted = await _customerService.RemoveAsync(customer);
            if (!deleted)
            {
                return BadRequest(new { message = "Не вдалося видалити клієнта" });
            }

            return NoContent();
        }

        #region Helper Methods

        private CustomerDto MapToDto(CustomerModel customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                LoyaltyPoints = customer.LoyaltyPoints,
                Profile = customer.Profile != null ? new CustomerProfileDto
                {
                    Id = customer.Profile.Id,
                    CustomerId = customer.Profile.CustomerId,
                    Email = customer.Profile.Email,
                    DateOfBirth = customer.Profile.DateOfBirth,
                    Address = customer.Profile.Address,
                    PreferredPaymentMethod = customer.Profile.PreferredPaymentMethod
                } : null
            };
        }

        #endregion
    }
}
