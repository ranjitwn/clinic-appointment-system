using ClinicAppointment.API.DTOs;
using ClinicAppointment.API.Models;
using ClinicAppointment.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Gets all appointment categories.
        /// </summary>
        /// <response code="200">Returns the list of categories</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            return Ok(await _categoryService.GetCategoriesAsync());
        }

        /// <summary>
        /// Creates a new appointment category.
        /// </summary>
        /// <remarks>
        /// Categories are used to classify appointments (e.g. Consultation, Check-up).
        /// </remarks>
        /// <response code="201">Category created successfully</response>
        /// <response code="400">If the request data is invalid</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryCreateDto dto)
        {
            var category = await _categoryService.CreateCategoryAsync(dto);

            if (category == null)
                return BadRequest(new { message = "Category already exists." });

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        /// <summary>
        /// Gets a specific appointment category by ID.
        /// </summary>
        /// <response code="200">Returns the category</response>
        /// <response code="404">If the category is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryAsync(id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            return Ok(category);
        }

        /// <summary>
        /// Updates an appointment category.
        /// </summary>
        /// <remarks>
        /// Only Admin users can modify category data.
        /// </remarks>
        /// <response code="200">Category updated successfully</response>
        /// <response code="400">If update fails due to duplicate name or category not found.</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategory(int id, CategoryCreateDto dto)
        {
            var updated = await _categoryService.UpdateCategoryAsync(id, dto);

            if (!updated)
                return BadRequest(new { message = "Update failed — duplicate name or category not found." });

            return Ok(new { message = "Category updated successfully" });
        }


        /// <summary>
        /// Deletes an appointment category.
        /// </summary>
        /// <remarks>
        /// Only Admin users can delete category data.
        /// </remarks>
        /// <response code="200">Category deleted successfully</response>
        /// <response code="404">If the category is not found</response>
        /// <response code="409">If the category is used in appointments and cannot be deleted</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (result == null)
                return NotFound(new { message = "Category not found." });

            if (result == false)
                return Conflict(new { message = "Category is used in appointments." });

            return Ok(new { message = "Category deleted successfully" });
        }

    }
}
