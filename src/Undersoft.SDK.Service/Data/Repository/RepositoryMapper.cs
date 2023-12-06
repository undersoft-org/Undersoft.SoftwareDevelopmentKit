using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.EntityFrameworkCore;
using Undersoft.SDK.Series;
using Undersoft.SDK.Uniques;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Service.Data.Repository;

using Series;
using Mapper;

public partial class Repository<TEntity>
{
    public virtual Task<TEntity> Map<TDto>(TDto model, TEntity entity)
    {
        return Task.Run(() => Mapper.Map(model, entity), Cancellation);
    }

    public virtual Task<TDto> Map<TDto>(TEntity entity, TDto model)
    {
        return Task.Run(() => Mapper.Map(entity, model), Cancellation);
    }

    public virtual Task<IList<TEntity>> Map<TDto>(
        IEnumerable<TDto> model,
        IEnumerable<TEntity> entity
    )
    {
        return Task.Run(() => (IList<TEntity>)Mapper.Map(model, entity).ToList(), Cancellation);
    }

    public virtual Task<IList<TDto>> Map<TDto>(
        IEnumerable<TEntity> entity,
        IEnumerable<TDto> model
    )
    {
        return Task.Run(() => (IList<TDto>)(Mapper.Map(entity, model).ToList()), Cancellation);
    }

    public virtual Task<ISeries<TEntity>> HashMap<TDto>(
        IEnumerable<TDto> model,
        IEnumerable<TEntity> entity
    )
    {
        return Task.Run(
            () => (ISeries<TEntity>)Mapper.Map(model, entity).ToRegistry(),
            Cancellation
        );
    }

    public virtual Task<ISeries<TDto>> HashMap<TDto>(
        IEnumerable<TEntity> entity,
        IEnumerable<TDto> model
    )
    {
        return Task.Run(() => (ISeries<TDto>)(Mapper.Map(entity, model).ToRegistry()), Cancellation);
    }

    public virtual Task<TDto> MapTo<TDto>(TEntity entity)
    {
        return Task.Run(() => Mapper.Map<TEntity, TDto>(entity), Cancellation);
    }

    public virtual Task<TDto> MapTo<TDto>(object entity)
    {
        return Task.Run(() => Mapper.Map<TDto>(entity), Cancellation);
    }

    public virtual Task<TEntity> MapFrom<TDto>(TDto model)
    {
        return Task.Run(() => Mapper.Map<TDto, TEntity>(model), Cancellation);
    }

    public virtual Task<TDto> MapFrom<TDto>(object model)
    {
        return Task.Run(() => Mapper.Map<TDto>(model), Cancellation);
    }

    public virtual Task<IList<TDto>> MapTo<TDto>(IEnumerable<object> entity)
    {
        return Task.Run(() => (Mapper.Map<IList<TDto>>(entity.Commit())), Cancellation);
    }

    public virtual Task<IList<TDto>> MapTo<TDto>(IEnumerable<TEntity> entity)
    {
        return Task.Run(() => Mapper.Map<IList<TDto>>(entity.Commit()), Cancellation);
    }

    public virtual async IAsyncEnumerable<TDto> MapToAsync<TDto>(IEnumerable<TEntity> entity)
    {
        foreach (var item in entity)
            yield return await Task.Run(() => Mapper.Map<TDto>(item));
    }

    public virtual Task<IList<TEntity>> MapFrom<TDto>(IEnumerable<TDto> model)
    {       
        return Task.Run(() => Mapper.Map<TDto[], IList<TEntity>>(model.Commit()), Cancellation);
    }

    public virtual async IAsyncEnumerable<TEntity> MapFromAsync<TDto>(IEnumerable<TDto> model)
    {
        foreach (var item in model)
            yield return await Task.Run(() => Mapper.Map<TDto, TEntity>(item));
    }

    public virtual Task<ISeries<TDto>> HashMapTo<TDto>(IEnumerable<object> entity)
    {
        return Task.Run(
            () => (ISeries<TDto>)(Mapper.Map<IEnumerable<TDto>>(entity.ToArray())).ToRegistry(),
            Cancellation
        );
    }

    public virtual Task<ISeries<TDto>> HashMapTo<TDto>(IEnumerable<TEntity> entity)
    {
        return Task.Run(
            () => (ISeries<TDto>)(Mapper.Map<IEnumerable<TDto>>(entity.ToArray())).ToRegistry(),
            Cancellation
        );
    }

    public virtual Task<ISeries<TEntity>> HashMapFrom<TDto>(IEnumerable<TDto> model)
    {
        return Task.Run(
            () =>
                (ISeries<TEntity>)
                    (
                        Mapper.Map<IEnumerable<TDto>, IEnumerable<TEntity>>(model.ToArray())
                    ).ToRegistry(),
            Cancellation
        );
    }

    public virtual Task<IQueryable<TDto>> QueryMapAsyncTo<TDto>(IQueryable<TEntity> entity) where TDto : class
    {
        return entity.ForEachAsync(e => Mapper.Map<TDto>(e));
    }

    public virtual IQueryable<TDto> QueryMapTo<TDto>(IQueryable<TEntity> entity) where TDto : class
    {
        return entity.ForEach(e => Mapper.Map<TDto>(e));
    }

    public virtual IQueryable<TEntity> QueryMapFrom<TDto>(IQueryable<TDto> model)
    {
        return model.ForEach(m => Mapper.Map<TDto, TEntity>(m));
    }

    public virtual Task<IQueryable<TEntity>> QueryMapAsyncFrom<TDto>(IQueryable<TDto> model)
    {
        return model.ForEachAsync(m => Mapper.Map<TDto, TEntity>(m));
    }

}
