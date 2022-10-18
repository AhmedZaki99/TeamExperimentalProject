using DataProcessingContext;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace DataProcessingAPI
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public abstract class EntityController<TEntity, TOutputDto, TCreateDto, TUpdateDto> : ControllerBase
        where TEntity : EntityBase
        where TOutputDto : OutputDtoBase
        where TCreateDto : class
        where TUpdateDto : class
    {

        #region Dependencies

        protected EntityService<TEntity, TOutputDto, TCreateDto, TUpdateDto> EntityService { get; }

        #endregion

        #region Constructor

        public EntityController(EntityService<TEntity, TOutputDto, TCreateDto, TUpdateDto> entityService)
        {
            EntityService = entityService;
        }

        #endregion


        #region Controller Actions

        /// <summary>
        /// Get entity by id.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TOutputDto>> GetEntityAsync([FromRoute] string id)
        {
            var entity = await EntityService.FindEntityAsync(id);
            if (entity is null)
            {
                return NotFound();
            }
            return entity;
        }

        /// <summary>
        /// Create new entity.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public virtual async Task<ActionResult<TOutputDto>> CreateEntityAsync([FromBody] TCreateDto entityDto)
        {
            var result = await EntityService.CreateEntityAsync(entityDto);
            if (result.Output is TOutputDto dto)
            {
                return CreatedAtAction(nameof(GetEntityAsync), new { id = dto.Id }, dto);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return result.ErrorType == OperationError.UnprocessableEntity ? UnprocessableEntity(ModelState) : ValidationProblem(ModelState);
        }

        /// <summary>
        /// Update entity by id.
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public virtual async Task<ActionResult<TOutputDto>> UpdateEntityAsync([FromRoute] string id, [FromBody] JsonPatchDocument<TUpdateDto> patchDoc)
        {
            var result = await EntityService.UpdateEntityAsync(id, inputDto =>
                patchDoc.TryApplyTo(inputDto, ModelState));

            if (result.Output is TOutputDto dto)
            {
                return dto;
            }
            if (result.ErrorType == OperationError.EntityNotFound)
            {
                return NotFound();
            }
            if (result.ErrorType != OperationError.ExternalError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }

            return result.ErrorType == OperationError.UnprocessableEntity ? UnprocessableEntity(ModelState) : ValidationProblem(ModelState);
        }

        /// <summary>
        /// Delete entity by id.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> DeleteEntityAsync([FromRoute] string id)
        {
            var deleteResult = await EntityService.DeleteEntityAsync(id);

            if (deleteResult == DeleteResult.EntityNotFound)
            {
                return NotFound();
            }
            else if (deleteResult == DeleteResult.Failed)
            {
                return Problem(statusCode: 500, title: "Server error.", detail: "Failed to delete the entity.");
            }

            return NoContent();
        }

        #endregion


    }
}
