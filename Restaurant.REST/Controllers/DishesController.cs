using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.REST.Models;
using Restaurant.REST.Services;
using Restaurant.Infrastructure.Models;

namespace Restaurant.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DishesController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishesController(IDishService dishService)
        {
            _dishService = dishService ?? throw new ArgumentNullException(nameof(dishService));
        }

        /// <summary>
        /// Отримати всі страви
        /// </summary>
        /// <param name="type">Фільтр за типом страви (dish, pizza, salad)</param>
        /// <returns>Список страв</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DishDto>>> GetAll([FromQuery] string? type = null)
        {
            IEnumerable<DishModel> dishes;

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (type.Equals("pizza", StringComparison.OrdinalIgnoreCase))
                {
                    dishes = await _dishService.GetAllPizzasAsync();
                }
                else if (type.Equals("salad", StringComparison.OrdinalIgnoreCase))
                {
                    dishes = await _dishService.GetAllSaladsAsync();
                }
                else
                {
                    dishes = (await _dishService.ReadAllAsync())
                        .Where(d => d is not PizzaModel and not SaladModel);
                }
            }
            else
            {
                dishes = await _dishService.ReadAllAsync();
            }

            var dishDtos = dishes.Select(d => MapToDto(d)).OrderBy(d => d.Name).ToList();
            return Ok(dishDtos);
        }

        /// <summary>
        /// Отримати страву за ID
        /// </summary>
        /// <param name="id">ID страви</param>
        /// <returns>Дані страви</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishDto>> GetById(Guid id)
        {
            var dish = await _dishService.ReadAsync(id);
            if (dish == null)
            {
                return NotFound(new { message = $"Страву з ID {id} не знайдено" });
            }

            return Ok(MapToDto(dish));
        }

        /// <summary>
        /// Отримати доступні страви
        /// </summary>
        /// <returns>Список доступних страв</returns>
        [HttpGet("available")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DishDto>>> GetAvailable()
        {
            var dishes = await _dishService.GetAvailableDishesAsync();
            var dishDtos = dishes.Select(d => MapToDto(d)).OrderBy(d => d.Name).ToList();
            return Ok(dishDtos);
        }

        /// <summary>
        /// Створити нову базову страву (тільки для адміністраторів)
        /// </summary>
        /// <param name="createDto">Дані для створення страви</param>
        /// <returns>Створена страва</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DishDto>> Create([FromBody] CreateDishDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return BadRequest(new { message = "Назва страви обов'язкова" });
            }

            if (createDto.Price <= 0)
            {
                return BadRequest(new { message = "Ціна має бути більше нуля" });
            }

            var dish = new DishModel
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Price = createDto.Price,
                Description = createDto.Description,
                IsAvailable = createDto.IsAvailable
            };

            var created = await _dishService.CreateAsync(dish);
            if (!created)
            {
                return BadRequest(new { message = "Не вдалося створити страву" });
            }

            return CreatedAtAction(nameof(GetById), new { id = dish.Id }, MapToDto(dish));
        }

        /// <summary>
        /// Оновити дані страви (тільки для адміністраторів)
        /// </summary>
        /// <param name="id">ID страви</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <returns>Оновлена страва</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DishDto>> Update(Guid id, [FromBody] UpdateDishDto updateDto)
        {
            var dish = await _dishService.ReadAsync(id);
            if (dish == null)
            {
                return NotFound(new { message = $"Страву з ID {id} не знайдено" });
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                dish.Name = updateDto.Name;
            }

            if (updateDto.Price.HasValue && updateDto.Price.Value > 0)
            {
                dish.Price = updateDto.Price.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Description))
            {
                dish.Description = updateDto.Description;
            }

            if (updateDto.IsAvailable.HasValue)
            {
                dish.IsAvailable = updateDto.IsAvailable.Value;
            }

            var updated = await _dishService.UpdateAsync(dish);
            if (!updated)
            {
                return BadRequest(new { message = "Не вдалося оновити страву" });
            }

            return Ok(MapToDto(dish));
        }

        /// <summary>
        /// Видалити страву (тільки для адміністраторів)
        /// </summary>
        /// <param name="id">ID страви</param>
        /// <returns>Статус видалення</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dish = await _dishService.ReadAsync(id);
            if (dish == null)
            {
                return NotFound(new { message = $"Страву з ID {id} не знайдено" });
            }

            var deleted = await _dishService.RemoveAsync(dish);
            if (!deleted)
            {
                return BadRequest(new { message = "Не вдалося видалити страву" });
            }

            return NoContent();
        }

        // === PIZZA ENDPOINTS ===

        /// <summary>
        /// Створити нову піцу (тільки для адміністраторів)
        /// </summary>
        /// <param name="createDto">Дані для створення піци</param>
        /// <returns>Створена піца</returns>
        [HttpPost("pizzas")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PizzaDto>> CreatePizza([FromBody] CreatePizzaDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return BadRequest(new { message = "Назва піци обов'язкова" });
            }

            if (createDto.Price <= 0)
            {
                return BadRequest(new { message = "Ціна має бути більше нуля" });
            }

            if (createDto.SizeCm < 20 || createDto.SizeCm > 50)
            {
                return BadRequest(new { message = "Розмір піци має бути від 20 до 50 см" });
            }

            var pizza = new PizzaModel
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Price = createDto.Price,
                Description = createDto.Description,
                IsAvailable = createDto.IsAvailable,
                SizeCm = createDto.SizeCm,
                DoughType = createDto.DoughType,
                ExtraCheese = createDto.ExtraCheese,
                Toppings = createDto.Toppings
            };

            var created = await _dishService.CreatePizzaAsync(pizza);
            if (!created)
            {
                return BadRequest(new { message = "Не вдалося створити піцу" });
            }

            return CreatedAtAction(nameof(GetById), new { id = pizza.Id }, MapToPizzaDto(pizza));
        }

        /// <summary>
        /// Оновити піцу (тільки для адміністраторів)
        /// </summary>
        /// <param name="id">ID піци</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <returns>Оновлена піца</returns>
        [HttpPut("pizzas/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PizzaDto>> UpdatePizza(Guid id, [FromBody] UpdatePizzaDto updateDto)
        {
            var dish = await _dishService.ReadAsync(id);
            if (dish is not PizzaModel pizza)
            {
                return NotFound(new { message = $"Піцу з ID {id} не знайдено" });
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                pizza.Name = updateDto.Name;
            }

            if (updateDto.Price.HasValue && updateDto.Price.Value > 0)
            {
                pizza.Price = updateDto.Price.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Description))
            {
                pizza.Description = updateDto.Description;
            }

            if (updateDto.IsAvailable.HasValue)
            {
                pizza.IsAvailable = updateDto.IsAvailable.Value;
            }

            if (updateDto.SizeCm.HasValue)
            {
                pizza.SizeCm = updateDto.SizeCm.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.DoughType))
            {
                pizza.DoughType = updateDto.DoughType;
            }

            if (updateDto.ExtraCheese.HasValue)
            {
                pizza.ExtraCheese = updateDto.ExtraCheese.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Toppings))
            {
                pizza.Toppings = updateDto.Toppings;
            }

            var updated = await _dishService.UpdateAsync(pizza);
            if (!updated)
            {
                return BadRequest(new { message = "Не вдалося оновити піцу" });
            }

            return Ok(MapToPizzaDto(pizza));
        }

        // === SALAD ENDPOINTS ===

        /// <summary>
        /// Створити новий салат (тільки для адміністраторів)
        /// </summary>
        /// <param name="createDto">Дані для створення салату</param>
        /// <returns>Створений салат</returns>
        [HttpPost("salads")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SaladDto>> CreateSalad([FromBody] CreateSaladDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return BadRequest(new { message = "Назва салату обов'язкова" });
            }

            if (createDto.Price <= 0)
            {
                return BadRequest(new { message = "Ціна має бути більше нуля" });
            }

            if (createDto.Calories < 0 || createDto.Calories > 2000)
            {
                return BadRequest(new { message = "Калорії мають бути від 0 до 2000" });
            }

            var salad = new SaladModel
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Price = createDto.Price,
                Description = createDto.Description,
                IsAvailable = createDto.IsAvailable,
                IsVegetarian = createDto.IsVegetarian,
                Calories = createDto.Calories,
                Dressing = createDto.Dressing,
                Ingredients = createDto.Ingredients
            };

            var created = await _dishService.CreateSaladAsync(salad);
            if (!created)
            {
                return BadRequest(new { message = "Не вдалося створити салат" });
            }

            return CreatedAtAction(nameof(GetById), new { id = salad.Id }, MapToSaladDto(salad));
        }

        /// <summary>
        /// Оновити салат (тільки для адміністраторів)
        /// </summary>
        /// <param name="id">ID салату</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <returns>Оновлений салат</returns>
        [HttpPut("salads/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SaladDto>> UpdateSalad(Guid id, [FromBody] UpdateSaladDto updateDto)
        {
            var dish = await _dishService.ReadAsync(id);
            if (dish is not SaladModel salad)
            {
                return NotFound(new { message = $"Салат з ID {id} не знайдено" });
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                salad.Name = updateDto.Name;
            }

            if (updateDto.Price.HasValue && updateDto.Price.Value > 0)
            {
                salad.Price = updateDto.Price.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Description))
            {
                salad.Description = updateDto.Description;
            }

            if (updateDto.IsAvailable.HasValue)
            {
                salad.IsAvailable = updateDto.IsAvailable.Value;
            }

            if (updateDto.IsVegetarian.HasValue)
            {
                salad.IsVegetarian = updateDto.IsVegetarian.Value;
            }

            if (updateDto.Calories.HasValue)
            {
                salad.Calories = updateDto.Calories.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Dressing))
            {
                salad.Dressing = updateDto.Dressing;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Ingredients))
            {
                salad.Ingredients = updateDto.Ingredients;
            }

            var updated = await _dishService.UpdateAsync(salad);
            if (!updated)
            {
                return BadRequest(new { message = "Не вдалося оновити салат" });
            }

            return Ok(MapToSaladDto(salad));
        }

        /// <summary>
        /// Отримати вегетаріанські салати
        /// </summary>
        /// <returns>Список вегетаріанських салатів</returns>
        [HttpGet("salads/vegetarian")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SaladDto>>> GetVegetarianSalads()
        {
            var salads = await _dishService.GetVegetarianSaladsAsync();
            var saladDtos = salads.Select(s => MapToSaladDto(s)).ToList();
            return Ok(saladDtos);
        }

        #region Helper Methods

        private DishDto MapToDto(DishModel dish)
        {
            return dish switch
            {
                PizzaModel pizza => MapToPizzaDto(pizza),
                SaladModel salad => MapToSaladDto(salad),
                _ => new DishDto
                {
                    Id = dish.Id,
                    Name = dish.Name,
                    Price = dish.Price,
                    Description = dish.Description,
                    IsAvailable = dish.IsAvailable,
                    DishType = "Dish"
                }
            };
        }

        private PizzaDto MapToPizzaDto(PizzaModel pizza)
        {
            return new PizzaDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                Price = pizza.Price,
                Description = pizza.Description,
                IsAvailable = pizza.IsAvailable,
                SizeCm = pizza.SizeCm,
                DoughType = pizza.DoughType,
                ExtraCheese = pizza.ExtraCheese,
                Toppings = pizza.Toppings
            };
        }

        private SaladDto MapToSaladDto(SaladModel salad)
        {
            return new SaladDto
            {
                Id = salad.Id,
                Name = salad.Name,
                Price = salad.Price,
                Description = salad.Description,
                IsAvailable = salad.IsAvailable,
                IsVegetarian = salad.IsVegetarian,
                Calories = salad.Calories,
                Dressing = salad.Dressing,
                Ingredients = salad.Ingredients
            };
        }

        #endregion
    }
}
