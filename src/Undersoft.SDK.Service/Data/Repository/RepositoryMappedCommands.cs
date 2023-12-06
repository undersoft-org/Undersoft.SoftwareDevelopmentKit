using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Data.Repository;

using Undersoft.SDK;
using Uniques;

public partial class Repository<TEntity>
{
    public virtual async Task<TEntity> AddBy<TDto>(TDto model)
    {
        return await Task.Run(async () => this.Add(await MapFrom(model)));
    }
    public virtual async Task<TEntity> AddBy<TDto>(TDto model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
    {
        return Add(await MapFrom(model), predicate);
    }
    public virtual async Task<IEnumerable<TEntity>> AddBy<TDto>(IEnumerable<TDto> model)
    {
        return Add(await MapFrom(model));
    }
    public virtual async Task<IEnumerable<TEntity>> AddBy<TDto>(IEnumerable<TDto> models, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
    {
        return await Add(await MapFrom(models), predicate).CommitAsync();
    }

    public virtual IAsyncEnumerable<TEntity> AddByAsync<TDto>(IEnumerable<TDto> model)
    {
        return AddAsync(MapFromAsync(model));
    }
    public virtual IAsyncEnumerable<TEntity> AddByAsync<TDto>(IEnumerable<TDto> models, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
    {
        var mapTask = MapFrom(models);
        mapTask.Wait();
        return AddAsync(mapTask.Result);
    }

    public virtual async Task<TEntity> DeleteBy<TDto>(TDto model)
    {
        return this.Delete(await MapFrom(model));
    }
    public virtual async Task<TEntity> DeleteBy<TDto>(TDto model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
    {
        return Delete(predicate.Invoke(await MapFrom(model)));
    }
    public virtual IEnumerable<TEntity> DeleteBy<TDto>(IEnumerable<TDto> model)
    {
        var map = MapFrom(model);
        map.Wait();
        return Delete(map.Result);
    }
    public virtual IEnumerable<TEntity> DeleteBy<TDto>(IEnumerable<TDto> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
    {
        var map = MapFrom(model);
        map.Wait();
        return Delete(map.Result, predicate);
    }

    public virtual IAsyncEnumerable<TEntity> DeleteByAsync<TDto>(IEnumerable<TDto> model)
    {
        var map = MapFrom(model);
        map.Wait();
        return DeleteAsync(map.Result);
    }
    public virtual IAsyncEnumerable<TEntity> DeleteByAsync<TDto>(IEnumerable<TDto> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
    {
        var map = MapFrom(model);
        map.Wait();
        return DeleteAsync(map.Result, predicate);
    }

    public virtual async Task<TEntity> SetBy<TDto>(TDto model) where TDto : class, IOrigin
    {
        return await Set(model);
    }
    public virtual async Task<TEntity> SetBy<TDto>(TDto model, params object[] keys) where TDto : class, IOrigin
    {
        return await Set(model, keys);
    }
    public virtual async Task<TEntity> SetBy<TDto>(TDto model, Func<TDto, Expression<Func<TEntity, bool>>> predicate, params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions) where TDto : class, IOrigin
    {
        return await Set(model, predicate(model), conditions.ForEach(c => c(model)));
    }
    public virtual IEnumerable<TEntity> SetBy<TDto>(IEnumerable<TDto> entity) where TDto : class, IOrigin
    {
        return Set(entity).Commit();
    }
    public virtual IEnumerable<TEntity> SetBy<TDto>(IEnumerable<TDto> models, Func<TDto, Expression<Func<TEntity, bool>>> predicate, params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions) where TDto : class, IOrigin
    {
        return Set(models, predicate, conditions).Commit();
    }

    public virtual IAsyncEnumerable<TEntity> SetByAsync<TDto>(IEnumerable<TDto> entity) where TDto : class, IOrigin
    {
        return SetAsync(entity);
    }
    public virtual IAsyncEnumerable<TEntity> SetByAsync<TDto>(IEnumerable<TDto> models, Func<TDto, Expression<Func<TEntity, bool>>> predicate, params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions) where TDto : class, IOrigin
    {
        return SetAsync(models, predicate, conditions);
    }

    public virtual async Task<TEntity> PutBy<TDto>(TDto model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions)
    {
        return await Put(await MapFrom(model), predicate, conditions);
    }
    public virtual IEnumerable<TEntity> PutBy<TDto>(IEnumerable<TDto> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions)
    {
        var map = MapFrom(model);
        map.Wait();
        return Put(map.Result, predicate, conditions).Commit();
    }

    public virtual IAsyncEnumerable<TEntity> PutByAsync<TDto>(IEnumerable<TDto> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions)
    {
        var map = MapFrom(model);
        map.Wait();
        return PutAsync(map.Result, predicate, conditions);
    }

    public virtual async Task<TEntity> PatchBy<TDto>(TDto model) where TDto : class, IOrigin
    {
        return await Patch(model);
    }
    public virtual IEnumerable<TEntity> PatchBy<TDto>(IEnumerable<TDto> entity) where TDto : class, IOrigin
    {
        return Patch(entity).Commit();
    }
    public virtual async Task<TEntity> PatchBy<TDto>(TDto model, params object[] keys) where TDto : class, IOrigin
    {
        return await Patch(model, keys);
    }
    public virtual async Task<TEntity> PatchBy<TDto>(TDto model, Func<TDto, Expression<Func<TEntity, bool>>> predicate) where TDto : class, IOrigin
    {
        return await Patch(model, predicate);
    }
    public virtual IEnumerable<TEntity> PatchBy<TDto>(IEnumerable<TDto> models, Func<TDto, Expression<Func<TEntity, bool>>> predicate) where TDto : class, IOrigin
    {
        return Patch(models, predicate).Commit();
    }

    public virtual IAsyncEnumerable<TEntity> PatchByAsync<TDto>(IEnumerable<TDto> entity) where TDto : class, IOrigin
    {
        return PatchAsync(entity);
    }
    public virtual IAsyncEnumerable<TEntity> PatchByAsync<TDto>(IEnumerable<TDto> models, Func<TDto, Expression<Func<TEntity, bool>>> predicate) where TDto : class, IOrigin
    {
        return PatchAsync(models, predicate);
    }
}
