using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using DataProcessingContext;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace DataProcessingCore
{
    public abstract class EntityService<TEntity, TOutputDto, TCreateDto, TUpdateDto>
        where TEntity : EntityBase
        where TOutputDto : OutputDtoBase
        where TCreateDto : class
        where TUpdateDto : class
    {

        #region Protected Properties

        protected ApplicationDbContext AppDbContext { get; }
        protected DbSet<TEntity> EntityDbSet { get; }

        protected IMapper Mapper { get; }

        #endregion


        #region Constructor

        public EntityService(ApplicationDbContext dbContext, DbSet<TEntity> dbSet, IMapper mapper)
        {
            AppDbContext = dbContext;
            EntityDbSet = dbSet;
            Mapper = mapper;
        }

        #endregion


        #region Data Read

        public virtual IAsyncEnumerable<TOutputDto> GetEntitiesAsync(params Expression<Func<TEntity, bool>>[] conditions)
        {
            var query = EntityDbSet.AsQueryable();
            foreach (var condition in conditions)
            {
                query = query.Where(condition);
            }

            return query
                .ProjectTo<TOutputDto>(Mapper.ConfigurationProvider)
                .AsNoTracking()
                .AsAsyncEnumerable();
        }


        public virtual Task<TOutputDto?> FindEntityAsync(string id)
        {
            return EntityDbSet
                .ProjectTo<TOutputDto>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        #endregion

        #region Create

        public virtual async Task<OperationResult<TOutputDto>> CreateEntityAsync(TCreateDto dto, bool validateDtoProperties = false)
        {
            var errors = validateDtoProperties ? ValidateObject(dto) : null;
            if (errors is not null)
            {
                return new(errors, OperationError.ValidationError);
            }

            errors = await ValidateCreateInputAsync(dto);
            if (errors is not null)
            {
                return new(errors, OperationError.UnprocessableEntity);
            }

            var entity = MapForCreate(dto);

            EntityDbSet.Add(entity);

            errors = await TrySaveChangesAsync();
            if (errors is not null)
            {
                return new(errors, OperationError.DatabaseError);
            }
            return new(Mapper.Map<TOutputDto>(entity));
        }

        #endregion

        #region Update

        public virtual Task<OperationResult<TOutputDto>> UpdateEntityAsync(string id, TUpdateDto dto, bool validateDtoProperties = false)
        {
            var errors = validateDtoProperties ? ValidateObject(dto) : null;
            if (errors is not null)
            {
                return Task.FromResult(new OperationResult<TOutputDto>(errors, OperationError.ValidationError));
            }

            return UpdateEntityAsync(id, updateDto =>
            {
                updateDto = dto;
                return true;
            });
        }

        public virtual async Task<OperationResult<TOutputDto>> UpdateEntityAsync(string id, Func<TUpdateDto, bool> updateCallback, bool validateDtoProperties = false)
        {
            var entity = await EntityDbSet.FindAsync(id);
            if (entity is null)
            {
                return new(OperationError.EntityNotFound);
            }
            var dto = Mapper.Map<TUpdateDto>(entity);

            if (!updateCallback.Invoke(dto))
            {
                return new(OperationError.ExternalError);
            }

            var errors = validateDtoProperties ? ValidateObject(dto) : null;
            if (errors is not null)
            {
                return new(errors, OperationError.ValidationError);
            }

            errors = await ValidateUpdateInputAsync(dto, entity);
            if (errors is not null)
            {
                return new(errors, OperationError.UnprocessableEntity);
            }

            entity = MapForUpdate(dto, entity);

            var entry = AppDbContext.Entry(entity);
            if (entry.State == EntityState.Unchanged)
            {
                entry.State = EntityState.Modified;
            }

            errors = await TrySaveChangesAsync();
            if (errors is not null)
            {
                return new(errors, OperationError.DatabaseError);
            }
            return new(Mapper.Map<TOutputDto>(entity));
        }

        #endregion

        #region Delete

        public virtual async Task<DeleteResult> DeleteEntityAsync(string id)
        {
            var entity = await EntityDbSet.FindAsync(id);
            if (entity is null)
            {
                return DeleteResult.EntityNotFound;
            }
            EntityDbSet.Remove(entity);

            return await AppDbContext.SaveChangesAsync() > 0 ? DeleteResult.Success : DeleteResult.Failed;
        }

        #endregion


        #region Mapping

        /// <summary>
        /// Maps the given Dto to an entity instance for data creation.
        /// </summary>
        /// <remarks>
        /// Could be overridden to provide further adjustment on mapped element.
        /// </remarks>
        /// <param name="dto">The create input object to map.</param>
        /// <returns>The mapped instance of the entity.</returns>
        protected virtual TEntity MapForCreate(TCreateDto dto)
        {
            return Mapper.Map<TEntity>(dto);
        }

        /// <summary>
        /// Maps the given Dto into the original state of the entity to apply changes.
        /// </summary>
        /// <remarks>
        /// Could be overridden to provide further adjustment on mapped element.
        /// </remarks>
        /// <param name="dto">The update input object to map.</param>
        /// <param name="original">The original state of the entity.</param>
        /// <returns>The mapped instance of the entity.</returns>
        protected virtual TEntity MapForUpdate(TUpdateDto dto, TEntity original)
        {
            return Mapper.Map(dto, original);
        }

        #endregion


        #region Abstract Methods

        /// <summary>
        /// Apply the required validation to process the create input Dto.
        /// </summary>
        /// <param name="dto">The create input object to validate.</param>
        /// <returns>A dictionary of validation errors if any found; otherwise, <see langword="null"/>.</returns>
        public abstract Task<Dictionary<string, string>?> ValidateCreateInputAsync(TCreateDto dto);

        /// <summary>
        /// Apply the required validation to process the update input Dto.
        /// </summary>
        /// <param name="dto">The update input object to validate.</param>
        /// <param name="original">The original state of the entity.</param>
        /// <returns>A dictionary of validation errors if any found; otherwise, <see langword="null"/>.</returns>
        public abstract Task<Dictionary<string, string>?> ValidateUpdateInputAsync(TUpdateDto dto, TEntity original);

        #endregion

        #region Validation Methods

        protected async Task<Dictionary<string, string>?> ValidateName(string name, string? originalName = null)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            if (!typeof(IUniqueName).IsAssignableFrom(typeof(TEntity)))
            {
                var msg = $"The underlying entity of type {typeof(TEntity).FullName} doesn't implement {typeof(IUniqueName).Name} interface.";
                throw new InvalidOperationException(msg);
            }
            if (name != originalName && await EntityDbSet.AnyAsync(e => ((IUniqueName)e).Name == name))
            {
                return OneErrorDictionary(nameof(IUniqueName.Name), $"{typeof(TEntity).Name} name already exists.");
            }
            return null;
        }

        protected static async Task<Dictionary<string, string>?> ValidateName<T>(DbSet<T> dbSet, string name, string? originalName = null) 
            where T : class, IUniqueName
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            if (name != originalName && await dbSet.AnyAsync(e => e.Name == name))
            {
                return OneErrorDictionary(nameof(IUniqueName.Name), $"{typeof(T).Name} name already exists.");
            }
            return null;
        }


        protected static async Task<Dictionary<string, string>?> ValidateId<T>(DbSet<T> dbSet, string? id, string? originalId = null) where T : EntityBase
        {
            if (id is not null && id != originalId && !await dbSet.AnyAsync(e => e.Id == id))
            {
                return OneErrorDictionary($"{typeof(T).Name}Id", $"There's no {typeof(T).Name} found with the Id provided.");
            }
            return null;
        }

        protected static Dictionary<string, string> OneErrorDictionary(string key, string message) => new()
        {
            [key] = message
        };

        #endregion


        #region Helper Methods

        private async Task<Dictionary<string, string>?> TrySaveChangesAsync()
        {
            if (await AppDbContext.SaveChangesAsync() > 0)
            {
                return null;
            }
            return OneErrorDictionary("Server Error", "Failed to save data.");
        }

        private static Dictionary<string, string>? ValidateObject<T>(T objectToValidate) where T : class
        {
            List<ValidationResult> results = new();
            ValidationContext context = new(objectToValidate);

            Validator.TryValidateObject(objectToValidate, context, results, true);

            if (results.Count > 0)
            {
                var pairs = results.Select(e => new KeyValuePair<string, string>(e.MemberNames.First(), e.ErrorMessage ?? "Invalid value."));
                return new(pairs);
            }
            return null;
        }

        #endregion

    }
}
